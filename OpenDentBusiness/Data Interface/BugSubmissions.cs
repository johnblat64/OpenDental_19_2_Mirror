using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using System.Xml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	///<summary></summary>
	public class BugSubmissions {

		public static IBugSubmissions MockBugSubmissions {
			get;
			set;
		}

		#region Get Methods
		///<summary></summary>
		public static List<BugSubmission> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod());
			}
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission";
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(command);
			},false);
			return listBugSubs;
		}

		///<summary>Returns a list of bug submissions and their corresponding bugs.
		///If a bugsubmission is not associated to a bug then the BugObj field on the bugsubmission object will be null.
		///Performs grouping logic in order to minimize the amount of bugsubmissions in the return results.</summary>
		public static List<BugSubmission> GetBugSubsForRegKeys(List<string> listRegKeys,DateTime dateFrom,DateTime dateTo) {
			if(listRegKeys==null || listRegKeys.Count==0) {
				return new List<BugSubmission>();//No point in going through middle tier.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),listRegKeys,dateFrom,dateTo);
			}
			List<BugSubmission> listRetVals=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission "
					+"LEFT JOIN bug ON bug.BugId=bugsubmission.BugId "
					+"WHERE bugsubmission.RegKey IN("+string.Join(",",listRegKeys.Select(x => "'"+POut.String(x)+"'"))+") "
					+"AND "+DbHelper.BetweenDates("SubmissionDateTime",dateFrom,dateTo)+" "
					+"ORDER BY bug.CreationDate DESC, bugsubmission.SubmissionDateTime DESC";
				//The query selects all columns for the bugsubmission and bug tables in one query.  Hopefully we never have conflicting columns that differ.
				DataTable table=Db.GetTable(command);
				//Make a clone of the table structure for the bug objects and only fill it with entries where the BugId row is valid.
				DataTable tableBugs=table.Clone();
				foreach(DataRow row in table.Select().Where(x => PIn.Long(x["BugId"].ToString(),false)!=0)) {
					tableBugs.ImportRow(row);
				}
				//Extract all of the bug objects from the subset table.
				List<Bug> listBugs=Crud.BugCrud.TableToList(tableBugs);
				//Extract all of the bugsubmission objects from the results.
				List<BugSubmission> listBugSubs = Crud.BugSubmissionCrud.TableToList(table);
				//Associate any bug object with its corresponding bugsubmission object.
				listBugSubs.ForEach(x => x.BugObj=listBugs.FirstOrDefault(y => y.BugId==x.BugId));
				//Group the bug submissions by RegKey, ExceptionStackTrace, and BugId.
				//Each grouping will be ordered by ProgramVersion individually.
				//Take the first bugsubmission from each group and add it to listGroupedBugSubs for the return value.
				foreach(BugSubmission bugSub in listBugSubs) {
					if(bugSub.TagCustom!=null) {//Used to skip already considered bug submissions via grouping logic.
						continue;
					}
					List<BugSubmission> listGroupedSubs=listBugSubs.FindAll(x => x.RegKey==bugSub.RegKey
						&& x.ExceptionStackTrace==bugSub.ExceptionStackTrace
						&& x.BugId==bugSub.BugId);
					listGroupedSubs.ForEach(x => x.TagCustom=true);//Flag all bugsubmissions in this group as handled.
					//Only add the most pertinent item from the group to our return value.
					listRetVals.Add(listGroupedSubs.OrderByDescending(x => new Version(x.Info.DictPrefValues[PrefName.ProgramVersion])).First());
				}
			},false);
			return listRetVals;
		}

		///<summary></summary>
		public static List<BugSubmission> GetAllInRange(DateTime dateFrom,DateTime dateTo,List<string> listVersionFilters=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listVersionFilters);
			}
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission WHERE "+DbHelper.BetweenDates("SubmissionDateTime",dateFrom,dateTo);
				if(!listVersionFilters.IsNullOrEmpty()) {
					command+=" AND (";
					for(int i=0;i<listVersionFilters.Count;i++) {
						command+=$"DbVersion LIKE '{POut.String(listVersionFilters[i])}%'";
						if(i<listVersionFilters.Count-1) {
							command+=" OR ";
						}
					}
					command+=")";
				}
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(command);
			},false);
			return listBugSubs;
		}

		public static List<BugSubmission> GetForBugId(long bugId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<BugSubmission>>(MethodBase.GetCurrentMethod(),bugId);
			}
			List<BugSubmission> listBugSubs=new List<BugSubmission>();
			if(bugId==0) {
				return listBugSubs;
			}
			DataAction.RunBugsHQ(() => { 
				string command="SELECT * FROM bugsubmission WHERE BugId="+POut.Long(bugId);
				listBugSubs=Crud.BugSubmissionCrud.SelectMany(command);
			},false);
			return listBugSubs;
		}

		///<summary>Gets one BugSubmission from the db.</summary>
		public static BugSubmission GetOne(long bugSubmissionId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<BugSubmission>(MethodBase.GetCurrentMethod(),bugSubmissionId);
			}
			BugSubmission bugSub=null;
			DataAction.RunBugsHQ(() => { 
				bugSub=Crud.BugSubmissionCrud.SelectOne(bugSubmissionId);
			},false);
			return bugSub;
		}
		
		#endregion

		#region Modification Methods

		#region Insert
		///<summary></summary>
		public static long Insert(BugSubmission bugSubmission) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),bugSubmission);
			}
			long retVal=0;
			//Always use the connection store config file because creating BugSubmissions should always happen via OpenDentalWebServiceHQ.
			DataAction.RunBugsHQ(() => { 
				retVal=Crud.BugSubmissionCrud.Insert(bugSubmission);
			});
			return retVal;
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(BugSubmission subNew, BugSubmission subOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),subNew,subOld);
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Crud.BugSubmissionCrud.Update(subNew,subOld);
			},false);
		}
		
		///<summary>Updates all bugIds for given bugSubmissionNums.</summary>
		public static void UpdateBugIds(long bugId,List<long> listBugSubmissionNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugId,listBugSubmissionNums);
				return;
			}
			if(listBugSubmissionNums==null || listBugSubmissionNums.Count==0) {
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Db.NonQ("UPDATE bugsubmission SET BugId="+POut.Long(bugId)
					+" WHERE BugSubmissionNum IN ("+string.Join(",",listBugSubmissionNums)+")");
			},false);
		}

		///<summary>Updates various columns based on in memory changes in listSubs.</summary>
		public static void UpdateMany(List<BugSubmission> listSubs,params string[] listColumns) {
			if(listSubs.Count==0 || listColumns.Count()==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSubs,listColumns);
				return;
			}
			List<string> listColumnUpdates=new List<string>();
			foreach(string column in listColumns) {
				List<string> listCases=new List<string>();
				switch(column) {
					#region IsHidden
					case "IsHidden":
						foreach(BugSubmission sub in listSubs) {
							listCases.Add("WHEN "+POut.Long(sub.BugSubmissionNum)+" THEN "+POut.Bool(sub.IsHidden));
						}
						break;
					#endregion
					#region BugId
					case "BugId":
						foreach(BugSubmission sub in listSubs) {
							listCases.Add("WHEN "+POut.Long(sub.BugSubmissionNum)+" THEN "+POut.Long(sub.BugId));
						}
						break;
					#endregion
				}
				listColumnUpdates.Add(column+"=(CASE BugSubmissionNum "+string.Join(" ",listCases)+" END)");
			}
			DataAction.RunBugsHQ(() => { 
				Db.NonQ("UPDATE bugsubmission SET "
					+string.Join(",",listColumnUpdates)+" "
					+"WHERE BugSubmissionNum IN ("+string.Join(",",listSubs.Select(x => x.BugSubmissionNum))+")");
			},false);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long bugSubmissionId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),bugSubmissionId);
				return;
			}
			DataAction.RunBugsHQ(() => { 
				Crud.BugSubmissionCrud.Delete(bugSubmissionId);
			},false);
		}
		#endregion

		#endregion

		#region Misc Methods
		
		///<summary>Attempts to submit an exception to HQ.
		///Checks PrefName.SendUnhandledExceptionsToHQ prior to web call.
		///Returns BugSubmissionResult.UpdateRequired when submitter is not on most recent stable or any version of the beta.
		///Returns BugSubmissionResult.Failed when an error occured in the web call method.
		///Returns BugSubmissionResult.Success when bugSubmissions was successfully created at HQ.</summary>
		public static BugSubmissionResult SubmitException(Exception ex,string threadName="",long patNumCur=-1,string moduleName="") {			
			if(MockBugSubmissions!=null) {
				return MockBugSubmissions.SubmitException(ex,threadName,patNumCur,moduleName);
			}
			if(!PrefC.GetBool(PrefName.SendUnhandledExceptionsToHQ)) {
				return BugSubmissionResult.None;
			}
			return BugSubmissions.ParseBugSubmissionResult(
				WebServiceMainHQProxy.GetWebServiceMainHQInstance().SubmitUnhandledException(
					PayloadHelper.CreatePayload(
						PayloadHelper.CreatePayloadContent(
							new BugSubmission(ex,threadName,patNumCur,moduleName),"bugSubmission")
						,eServiceCode.BugSubmission)));
		}

		///<summary>After calling WebServiceMainHQ.SubmitUnhandledException(...) this will digest the result and provide the result.</summary>
		public static BugSubmissionResult ParseBugSubmissionResult(string result) {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			if(doc.SelectSingleNode("//Error")!=null) {
				return BugSubmissionResult.Failed;
			}
			XmlNode responseNode=doc.SelectSingleNode("//Success");
			if(responseNode!=null) {
				return (responseNode.InnerText=="true"? BugSubmissionResult.UpdateRequired : BugSubmissionResult.Success);
			}
			return BugSubmissionResult.None;//Just in case;
		}
		
		public static string GetSubmissionDescription(Patient patCur,BugSubmission sub) {
			return "Caller Name and #: "+patCur.GetNameLF() +" (work) "+patCur.WkPhone+"\r\n"
				+"Quick desc: "+sub.ExceptionMessageText+"\r\n"
				+"OD version: "+sub.Info.DictPrefValues[PrefName.ProgramVersion]+"\r\n"
				+"Windows version: "+sub.Info.WindowsVersion+"\r\n"
				+"Comps affected: "+sub.Info.CompName+"\r\n"
				+"Database name: "+sub.Info.DatabaseName+"\r\n"
				+"Example PatNum: " +sub.Info.PatientNumCur+"\r\n"
				+"Details: "+"\r\n"
				+"Duplicable?: "+"\r\n"
				+"Steps to duplicate: "+"\r\n"
				+"Exception:  "+sub.ExceptionStackTrace;
		}

		///<summary>Pass in the BugSubmission for the bug that was just submitted.  This method will check the database for key phrases that are part of
		///unhandled exceptions that HQ cannot do anything programmatically to fix.
		///E.g. we don't want to document corrupted tables because there is nothing we can do to help the customer sans them calling us for support,
		///3rd party plug-ins can crash and burn and where the user needs to call the 3rd party or disable the plug-in., etc.</summary>
		public static bool IsValidBug(BugSubmission bug) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),bug);
			}
			List<string> listPhrases=new List<string>();
			//Query the database for all phrases that we don't accept and see if the exception text passed in contains any of the phrases from the database.
			DataAction.RunBugsHQ(() => {
				listPhrases=Db.GetListString("SELECT Phrase FROM bugsubmissionfilter WHERE Phrase!=''");
			});
			return !listPhrases.Any(x => bug.ExceptionMessageText.Contains(x) || bug.ExceptionStackTrace.Contains(x));
		}

		///<summary>Removes file names and line numbers to reveal a simplified stack path.</summary>
		public static string SimplifyStackTrace(string stackTrace) {
			return Regex.Replace(stackTrace.ToLower(),@" in [a-z0-9.\\: ]+\n","\n");//Case insensitive.
		}
		#endregion

	}

	public enum BugSubmissionResult {
		///<summary></summary>
		None,
		///<summary>Submitter is not on support or there was an exception in the web method</summary>
		Failed,
		///<summary>Submitter must be on the most recent stable or any beta version.</summary>
		UpdateRequired,
		///<summary>Submitter sucesfully insert a bugSubmission at HQ</summary>
		Success,
	}

	///<summary>Used to test bug submissions.</summary>
	public interface IBugSubmissions {
		BugSubmissionResult SubmitException(Exception ex,string threadName="",long patNumCur=-1,string moduleName="");
	}
}