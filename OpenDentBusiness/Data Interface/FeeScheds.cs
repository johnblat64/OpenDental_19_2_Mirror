using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using OpenDentBusiness;
using System.IO;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FeeScheds{
		#region Get Methods
		///<summary>Gets the fee sched from the first insplan, the patient, or the provider in that order.  Uses procProvNum if>0, otherwise pat.PriProv.
		///Either returns a fee schedule (fk to definition.DefNum) or 0.</summary>
		public static long GetFeeSched(Patient pat,List<InsPlan> planList,List<PatPlan> patPlans,List<InsSub> subList,long procProvNum) {
			//No need to check RemotingRole; no call to db.
			//there's not really a good place to put this function, so it's here.
			long priPlanFeeSched=0;
			PatPlan patPlanPri = patPlans.FirstOrDefault(x => x.Ordinal==1);
			if(patPlanPri!=null) {
				InsPlan planCur=InsPlans.GetPlan(InsSubs.GetSub(patPlanPri.InsSubNum,subList).PlanNum,planList);
				if(planCur!=null) {
					priPlanFeeSched=planCur.FeeSched;
				}
			}
			return GetFeeSched(priPlanFeeSched,pat.FeeSched,procProvNum!=0?procProvNum:pat.PriProv);//use procProvNum, but if 0 then default to pat.PriProv
		}

		///<summary>A simpler version of the same function above.  The required numbers can be obtained in a fairly simple query.
		///Might return a 0 if the primary provider does not have a fee schedule set.</summary>
		public static long GetFeeSched(long priPlanFeeSched,long patFeeSched,long provNum) {
			//No need to check RemotingRole; no call to db.
			long provFeeSched=(Providers.GetFirstOrDefault(x => x.ProvNum==provNum)??new Provider()).FeeSched;//defaults to 0
			return new[] { priPlanFeeSched,patFeeSched,provFeeSched }.FirstOrDefault(x => x>0);//defaults to 0 if all fee scheds are 0
		}

		///<summary>Gets the fee schedule from the primary MEDICAL insurance plan, 
		///the first insurance plan, the patient, or the provider in that order.</summary>
		public static long GetMedFeeSched(Patient pat,List<InsPlan> planList,List<PatPlan> patPlans,List<InsSub> subList,long procProvNum) {
			//No need to check RemotingRole; no call to db.
			long retVal = 0;
			if(PatPlans.GetInsSubNum(patPlans,1) != 0){
				//Pick the medinsplan with the ordinal closest to zero
				int planOrdinal=10; //This is a hack, but I doubt anyone would have more than 10 plans
				bool hasMedIns=false; //Keep track of whether we found a medical insurance plan, if not use dental insurance fee schedule.
				InsSub subCur;
				foreach(PatPlan patplan in patPlans){
					subCur=InsSubs.GetSub(patplan.InsSubNum,subList);
					if(patplan.Ordinal<planOrdinal && InsPlans.GetPlan(subCur.PlanNum,planList).IsMedical) {
						planOrdinal=patplan.Ordinal;
						hasMedIns=true;
					}
				}
				if(!hasMedIns) { //If this patient doesn't have medical insurance (under ordinal 10)
					return GetFeeSched(pat,planList,patPlans,subList,procProvNum);  //Use dental insurance fee schedule
				}
				subCur=InsSubs.GetSub(PatPlans.GetInsSubNum(patPlans,planOrdinal),subList);
				InsPlan PlanCur=InsPlans.GetPlan(subCur.PlanNum, planList);
				if (PlanCur==null){
					retVal=0;
				} 
				else {
					retVal=PlanCur.FeeSched;
				}
			}
			if (retVal==0){
				if (pat.FeeSched!=0){
					retVal=pat.FeeSched;
				} 
				else {
					if (pat.PriProv==0){
						retVal=Providers.GetFirst(true).FeeSched;
					} 
					else {
						Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
						Provider provider=Providers.GetFirstOrDefault(x => x.ProvNum==pat.PriProv)??providerFirst;
						retVal=provider.FeeSched;
					}
				}
			}
			return retVal;
		}
		#endregion

		#region Modification Methods
		
		#region Insert
		#endregion

		#region Update
		#endregion

		#region Delete
		#endregion

		#endregion

		#region Misc Methods

		///<summary>Deprecated cache pattern. Manipulates the FeeCache passed in by copying one fee schedule to another utilizing the filters passed in.</summary>
		///<param name="feeCache">The fee cache instance that will get manipulated during the copy process.</param>
		///<param name="fromFeeSched">The fee schedule to copy from.</param>
		///<param name="fromClinicNum">Set in order to only copy fees for this clinic.</param>
		///<param name="fromProvNum">Set in order to only copy fees for this provider.</param>
		///<param name="toFeeSched">The fee schedule to copy into.</param>
		///<param name="listClinicNumsTo">Set in order to only copy fees to these clinics. 
		///ClinicNum 0 will be added by this method if this list is null or empty on entry.</param>
		///<param name="toProvNum">Set in order to only copy fees into this provider.</param>
		///<returns>List of security log entries with details about each fee that was copied.</returns>
		public static List<string> CopyFeeSchedule(FeeCache feeCache,FeeSched fromFeeSched,long fromClinicNum,
			long fromProvNum,FeeSched toFeeSched,List<long> listClinicNumsTo,long toProvNum)
		{
			//No need to check RemotingRole; manipulates the FeeCache passed in.
			List<string> listSecurityLogEntries=new List<string>();
			if(listClinicNumsTo==null) {
				listClinicNumsTo=new List<long>();
			}
			if(listClinicNumsTo.Count==0) {
				listClinicNumsTo.Add(0);
			}
			feeCache.BeginTransaction();
			//copy any values over
			//Modifies in-memory list, adding 100% match entries based on combo boxes.
			//First, we've cleared out the "To" fee schedule based on the "To" combo box settings so that fee sched will have 0 entries for the criteria in the combo boxes.
			//Next, we take all the fees from the "From" fee schedule with those matching criteria, and copy them to the "To" fee schedule with the "To" schedule's criteria.
			listClinicNumsTo.ForEach(clinicNum => feeCache.RemoveFees(toFeeSched.FeeSchedNum,clinicNum,toProvNum));
			List<Fee> listNewFees=feeCache.GetListFees(fromFeeSched.FeeSchedNum,fromClinicNum,fromProvNum);
			//fill listBestFees with the "best" fee for each proccode.
			List<Fee> listBestFees=listNewFees
				.GroupBy(x => x.CodeNum)
				.Select(x => feeCache.GetFee(x.Key,fromFeeSched.FeeSchedNum,fromClinicNum,fromProvNum,doGetExactMatch: true))
				.Where(x => x!=null).ToList();
			int blockValue=0;
			int blockMax=(listBestFees.Count * listClinicNumsTo.Count);
			object locker=new object();
			List<Action> listActions=new List<Action>();
			foreach(long clinicNumTo in listClinicNumsTo) {
				listActions.Add(() => {
					foreach(Fee fee in listBestFees) {
						Fee newFee=fee.Copy();
						newFee.FeeNum=0;
						newFee.ProvNum=toProvNum;
						newFee.ClinicNum=clinicNumTo;
						newFee.FeeSched=toFeeSched.FeeSchedNum;
						feeCache.Add(newFee);//FeeCache can handle concurrent updates.
						ProcedureCode procCode=ProcedureCodes.GetProcCode(fee.CodeNum);
						string securityLogEntry="Fee Schedule \""+fromFeeSched.Description+"\" copied to Fee Schedule \""+toFeeSched.Description+"\", "
							+"Proc Code \""+procCode.ProcCode+"\", Fee \""+fee.Amount+"\"";
						FeeSchedEvent.Fire(ODEventType.FeeSched,
							new ProgressBarHelper(Lans.g("FormFeeSchedTools","Copying fees, please wait")+"...",blockValue:blockValue,blockMax:blockMax,
							progressStyle:ProgBarStyle.Continuous));
						lock(locker) {
							blockValue++;
							listSecurityLogEntries.Add(securityLogEntry);
						}
					}
				});
			}
			//Temporarily forcing this list of actions to run synchronously by setting numThreads to 1.
			//Due to our FeeCache having a LimitedSizeQueue, it is not ready to handle copying a fee schedule to more than six clinics.
			//Once our class-wide FeeCache has been enhanced to handle more than six (5 clinics + 1 HQ) clinics, this can go back to being truly parallel.
			ODThread.RunParallel(listActions,TimeSpan.FromMinutes(30),numThreads:1);
			return listSecurityLogEntries;
		}

		///<summary>Copies one fee schedule to one or more fee schedules.  fromClinicNum, fromProvNum, and toProvNum can be zero.  Set listClinicNumsTo to copy to multiple clinic overrides.  If this list is null or empty, clinicNum 0 will be used.</summary>
		public static void CopyFeeSchedule(FeeSched fromFeeSched,long fromClinicNum,long fromProvNum,FeeSched toFeeSched,List<long> listClinicNumsTo,long toProvNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fromFeeSched,fromClinicNum,fromProvNum,toFeeSched,listClinicNumsTo,toProvNum);
				return;
			}
			if(listClinicNumsTo==null) {
				listClinicNumsTo=new List<long>();
			}
			if(listClinicNumsTo.Count==0) {
				listClinicNumsTo.Add(0);
			}
			//Delete all fees that exactly match setting in "To" combo selections.
			foreach(long clinicNum in listClinicNumsTo){
				Fees.DeleteFees(toFeeSched.FeeSchedNum,clinicNum,toProvNum);
			}
			//Copy:
			List<Fee> listNewFees=Fees.GetListExact(fromFeeSched.FeeSchedNum,fromClinicNum,fromProvNum);
			int blockValue=0;
			int blockMax=(listNewFees.Count * listClinicNumsTo.Count);
			object locker=new object();
			List<Action> listActions=new List<Action>();
			foreach(long clinicNumTo in listClinicNumsTo) {
				listActions.Add(() => {
					foreach(Fee fee in listNewFees) {
						Fee newFee=fee.Copy();
						newFee.FeeNum=0;
						newFee.ProvNum=toProvNum;
						newFee.ClinicNum=clinicNumTo;
						newFee.FeeSched=toFeeSched.FeeSchedNum;
						Fees.Insert(newFee);
						ProcedureCode procCode=ProcedureCodes.GetProcCode(fee.CodeNum);
						string securityLogText="Fee Schedule \""+fromFeeSched.Description+"\" copied to Fee Schedule \""+toFeeSched.Description+"\", ";
						if(clinicNumTo!=0){
							securityLogText+="To Clinic \""+Clinics.GetDesc(clinicNumTo)+"\", ";
						}
						securityLogText+="Proc Code \""+procCode.ProcCode+"\", Fee \""+fee.Amount+"\"";
						SecurityLogs.MakeLogEntry(Permissions.FeeSchedEdit,0,securityLogText);
						FeeSchedEvent.Fire(ODEventType.FeeSched,
							new ProgressBarHelper(Lans.g("FormFeeSchedTools","Copying fees, please wait")+"...",blockValue:blockValue,blockMax:blockMax,
							progressStyle:ProgBarStyle.Continuous));
						lock(locker) {
							blockValue++;
						}
					}
				});
			}
			//Research and testing will determine whether we can run this on multiple threads.
			ODThread.RunParallel(listActions,TimeSpan.FromMinutes(30),numThreads:1);
		}

		///<summary>Deprecated.  Part of old cache pattern.  Imports a canadian fee schedule. Called only in FormFeeSchedTools, located here to allow unit testing. Fires FeeSchedEvents
		///for a progress bar.</summary>
		///<param name="feeCache"></param>
		public static List<Fee> ImportCanadaFeeSchedule(FeeCache feeCache,FeeSched feeSched,string feeData,long clinicNum,long provNum,
			out int numImported,out int numSkipped) 
		{
			//No need to check RemotingRole; no call to db.
			string[] feeLines=feeData.Split('\n');
			double feeAmt;
			numImported=0;
			numSkipped=0;
			List<Fee> listNewFees=feeCache.GetListFees(feeSched.FeeSchedNum,clinicNum,provNum);
			for(int i=0;i<feeLines.Length;i++) {
				string[] fields=feeLines[i].Split('\t');
				if(fields.Length>1) {// && fields[1]!=""){//we no longer skip blank fees
					string procCode=fields[0];
					if(ProcedureCodes.IsValidCode(procCode)) { //The Fees.Import() function will not import fees for codes that do not exist.
						if(fields[1]=="") {
							feeAmt=-1;//triggers deletion of existing fee, but no insert.
						}
						else {
							feeAmt=PIn.Double(fields[1],doUseEnUSFormat: true);//The fees are always in the format "1.00" so we need to parse accordingly.
						}
						listNewFees=Fees.Import(procCode,feeAmt,feeSched.FeeSchedNum,clinicNum,provNum,listNewFees);
						numImported++;
					}
					else {
						numSkipped++;
					}
					FeeSchedEvent.Fire(ODEventType.FeeSched,
					new ProgressBarHelper(Lans.g("FeeScheds","Processing fees, please wait")+"...","",(numImported+numSkipped),feeLines.Length,
					ProgBarStyle.Continuous));
				}
			}
			return listNewFees;
		}

		///<summary>Replaces ImportCanadaFeeSchedule.  Imports a canadian fee schedule. Called only in FormFeeSchedTools, located here to allow unit testing. 
		///Fires FeeSchedEvents for a progress bar.</summary>
		public static List<Fee> ImportCanadaFeeSchedule2(FeeSched feeSched,string feeData,long clinicNum,long provNum,out int numImported,out int numSkipped) {
			//No need to check RemotingRole; no call to db.
			string[] feeLines=feeData.Split('\n');
			numImported=0;
			numSkipped=0;
			List<Fee> listFees=Fees.GetListExact(feeSched.FeeSchedNum,clinicNum,provNum);
			List<Fee> listFeesImported=new List<Fee>(listFees);
			for(int i=0;i<feeLines.Length;i++) {
				string[] fields=feeLines[i].Split('\t');
				if(fields.Length>1) {// && fields[1]!=""){//we no longer skip blank fees
					string procCode=fields[0];
					if(ProcedureCodes.IsValidCode(procCode)) { 
						long codeNum = ProcedureCodes.GetCodeNum(procCode);
						Fee fee=Fees.GetFee(codeNum,feeSched.FeeSchedNum,clinicNum,provNum,listFees);//gets best match
						if(fields[1]=="") {//an empty entry will delete an existing fee, but not insert a blank override
							if(fee==null){//nothing to do
								
							}
							else{
								//doesn't matter if the existing fee is an override or not.
								Fees.Delete(fee);
								listFeesImported.Remove(fee);
							}
						}
						else {//value found in text file
							if(fee==null){//no current fee
								fee=new Fee();
								fee.Amount=PIn.Double(fields[1],doUseEnUSFormat: true);//The fees are always in the format "1.00" so we need to parse accordingly.
								fee.FeeSched=feeSched.FeeSchedNum;
								fee.CodeNum=codeNum;
								fee.ClinicNum=clinicNum;
								fee.ProvNum=provNum;
								Fees.Insert(fee);
								listFeesImported.Add(fee);
							}
							else{
								fee.Amount=PIn.Double(fields[1],doUseEnUSFormat: true);
								Fees.Update(fee);
							}
						}
						numImported++;
					}
					else {
						numSkipped++;
					}
					FeeSchedEvent.Fire(ODEventType.FeeSched,
					new ProgressBarHelper(Lans.g("FeeScheds","Processing fees, please wait")+"...","",(numImported+numSkipped),feeLines.Length,
					ProgBarStyle.Continuous));
				}
			}
			return listFeesImported;
		}

		///<summary>Deprecated fee cache pattern.  Exports a fee schedule. Called only in FormFeeSchedTools, located here to allow unit testing. Fires FeeSchedEvents
		///for a progress bar.</summary>
		public static void ExportFeeSchedule(FeeCache feeCache,long feeSchedNum,long clinicNum,long provNum,string fileName) {
			//No need to check RemotingRole; no call to db.
			//CreateText will overwrite any content if the file already exists.
			using(StreamWriter sr=File.CreateText(fileName)) {
				//Get every single procedure code from the cache which will already be ordered by ProcCat and then ProcCode.
				//Even if the code does not have a fee, include it in the export because that will trigger a 'deletion' when importing over other schedules.
				int rowNum=0;
				List<ProcedureCode> listProcCodes=ProcedureCodes.GetListDeep();
				foreach(ProcedureCode procCode in listProcCodes) {
					//Get the best matching fee for the current selections. 
					Fee fee=feeCache.GetFee(procCode.CodeNum,feeSchedNum,clinicNum,provNum);
					sr.Write(procCode.ProcCode+"\t");
					if(fee!=null && fee.Amount!=-1) {
						sr.Write(fee.Amount.ToString("n"));
					}
					sr.Write("\t");
					sr.Write(procCode.AbbrDesc+"\t");
					sr.WriteLine(procCode.Descript);
					double percent=((rowNum*1.0)/listProcCodes.Count*100);
					FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(
						Lans.g("FeeScheds","Exporting fees, please wait")+"...",percent.ToString(),blockValue:(int)percent,progressStyle:ProgBarStyle.Continuous));
					rowNum++;
				}
			}
		}

		///<summary>ExportFeeSchedule method above is deprecated.  This is the replacement. Exports a fee schedule. 
		///Called only in FormFeeSchedTools. Fires FeeSchedEvents for a progress bar.</summary>
		public static void ExportFeeScheduleNew(long feeSchedNum,long clinicNum,long provNum,string fileName) {
			//No need to check RemotingRole; no call to db.
			//CreateText will overwrite any content if the file already exists.
			using(StreamWriter sr=File.CreateText(fileName)) {
				//Get every single procedure code from the cache which will already be ordered by ProcCat and then ProcCode.
				//Even if the code does not have a fee, include it in the export because that will trigger a 'deletion' when importing over other schedules.
				int rowNum=0;
				List<ProcedureCode> listProcCodes=ProcedureCodes.GetListDeep();
				List<Fee> listFees=Fees.GetListForScheds(feeSchedNum,clinicNum,provNum);//gets best matches
				foreach(ProcedureCode procCode in listProcCodes) {
					//Get the best matching fee (not exact match) for the current selections. 
					Fee fee=Fees.GetFee(procCode.CodeNum,feeSchedNum,clinicNum,provNum,listFees);
					sr.Write(procCode.ProcCode+"\t");
					if(fee!=null && fee.Amount!=-1) {
						sr.Write(fee.Amount.ToString("n"));
					}
					sr.Write("\t");
					sr.Write(procCode.AbbrDesc+"\t");
					sr.WriteLine(procCode.Descript);
					double percent=((rowNum*1.0)/listProcCodes.Count*100);
					FeeSchedEvent.Fire(ODEventType.FeeSched,new ProgressBarHelper(
						"Exporting fees, please wait...",percent.ToString(),blockValue:(int)percent,progressStyle:ProgBarStyle.Continuous));
					rowNum++;
				}
			}
		}

		///<summary>Updates writeoff estimated for claimprocs for the passed in clinics. Called only in FormFeeSchedTools, located here to allow unit
		///testing. Requires an ODProgressExtended to display UI updates.  If clinics are enabled and the user is not clinic restricted and chooses to run
		///for all clinics, set doUpdatePrevClinicPref to true so that the ClinicNums will be stored in the preference table as they are finished to allow
		///for pausing/resuming the process.</summary>
		public static long GlobalUpdateWriteoffs(List<Clinic> listWriteoffClinics,ODProgressExtended progress,bool doUpdatePrevClinicPref=false) {
			//No need to check RemotingRole; no call to db.
			long totalWriteoffsUpdated=0;
			List<Fee> listFeesHQ=Fees.GetByClinicNum(0);//All HQ fees
			Dictionary<long,List<Procedure>> dictPatProcs;
			Dictionary<long,Dictionary<long,Tuple<int,List<Procedure>>>> dictFamProcs;
			Dictionary<long,List<ClaimProc>> dictClaimProcs;
			List<Fee> listFeesHQandClinic;
			Lookup<FeeKey2,Fee> lookupFeesByCodeAndSched;
			List<InsSub> listInsSubs;
			List<InsPlan> listInsPlans;
			List<PatPlan> listPatPlans;
			List<Benefit> listBenefits;
			List<Action> listActions;
			foreach(Clinic clinicCur in listWriteoffClinics) {
				progress.Fire(ODEventType.FeeSched,new ProgressBarHelper(clinicCur.Abbr,"0%",0,100,ProgBarStyle.Blocks,"WriteoffProgress"));
				long rowCurIndex = 0; //reset for each clinic.
				object lockObj=new object();//used to lock rowCurIndex so the threads will correctly increment the count
				progress.Fire(ODEventType.FeeSched,new ProgressBarHelper(Lans.g("FeeSchedEvent","Getting list to update writeoffs..."),
						progressBarEventType: ProgBarEventType.TextMsg));
				listFeesHQandClinic=Fees.GetByClinicNum(clinicCur.ClinicNum);//could be empty for some clinics that don't use overrides
				listFeesHQandClinic.AddRange(listFeesHQ);
				lookupFeesByCodeAndSched=(Lookup<FeeKey2,Fee>)listFeesHQandClinic.ToLookup(x => new FeeKey2(x.CodeNum,x.FeeSched));
				dictPatProcs=Procedures.GetAllTp(clinicCur.ClinicNum)
					.GroupBy(x => x.PatNum)
					.ToDictionary(x => x.Key,x => Procedures.SortListByTreatPlanPriority(x.ToList()).ToList());
				#region Has Paused or Cancelled
				while(progress.IsPaused) {
					progress.AllowResume();
					if(progress.IsCanceled) {
						break;
					}
				}
				if(progress.IsCanceled) {
					break;
				}
				#endregion Has Paused or Cancelled
				if(dictPatProcs.Count==0) {
					continue;
				}
				int procCount=dictPatProcs.Sum(x => x.Value.Count);
				List<Procedure> listProcsCur;
				//dict of key=GuarNum, value=dict of key=PatNum, value=tuple with item1=patient age, item2=list of TP procs for the pat (or empty list if none)
				dictFamProcs=Patients.GetFamilies(dictPatProcs.Keys.ToList()).Where(x => x.Guarantor!=null)
					.ToDictionary(x => x.Guarantor.PatNum,x => x.ListPats
						.ToDictionary(y => y.PatNum,y => Tuple.Create(y.Age,(dictPatProcs.TryGetValue(y.PatNum,out listProcsCur)?listProcsCur:new List<Procedure>()))));
				listInsSubs=InsSubs.GetListInsSubs(dictFamProcs.SelectMany(x => x.Value.Keys).ToList());
				listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				listPatPlans=PatPlans.GetPatPlansForPats(dictPatProcs.Keys.ToList());
				listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
				#region Has Paused or Cancelled
				while(progress.IsPaused) {
					progress.AllowResume();
					if(progress.IsCanceled) {
						break;
					}
				}
				if(progress.IsCanceled) {
					break;
				}
				#endregion Has Paused or Cancelled
				//dictionary of key=PatNum, value=list of claimprocs, i.e. a dictionary linking each PatNum to a list of claimprocs for the given procs
				dictClaimProcs=ClaimProcs.GetForProcs(dictPatProcs.SelectMany(x => x.Value.Select(y => y.ProcNum)).ToList())
					.GroupBy(x => x.PatNum)
					.ToDictionary(x => x.Key,x => x.ToList());
				#region Has Paused or Cancelled
				while(progress.IsPaused) {
					progress.AllowResume();
					if(progress.IsCanceled) {
						break;
					}
				}
				if(progress.IsCanceled) {
					break;
				}
				#endregion Has Paused or Cancelled
				progress.Fire(ODEventType.FeeSched,new ProgressBarHelper(Lans.g("FeeSchedEvent","Updating writeoff estimates for patients..."),
						progressBarEventType: ProgBarEventType.TextMsg));
				listActions=dictFamProcs.Select(x => new Action(() => {
					#region Has Cancelled
					if(progress.IsCanceled) {
						return;
					}
					#endregion Has Cancelled
					long guarNum=x.Key;
					List<InsSub> listInsSubsCur=listInsSubs.FindAll(y => y.Subscriber.In(x.Value.Keys));
					List<InsPlan> listInsPlansCur=listInsPlans.FindAll(y => listInsSubsCur.Exists(z => z.PlanNum==y.PlanNum));
					List<SubstitutionLink> listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(listInsPlansCur);
					List<PatPlan> listPatPlansCur;
					List<Benefit> listBenefitsCur;
					List<ClaimProc> listClaimProcsCur;
					List<Procedure> listProcs;
					foreach(KeyValuePair<long,Tuple<int,List<Procedure>>> kvp in x.Value) {//foreach patient in the family
						listProcs=kvp.Value.Item2;
						if((listProcs?.Count??0)==0) {
							continue;
						}
						long patNum=kvp.Key;
						int patAge=kvp.Value.Item1;
						listPatPlansCur=listPatPlans.FindAll(y => y.PatNum==patNum);
						listBenefitsCur=listBenefits
							.FindAll(y => listInsPlansCur.Exists(z => z.PlanNum==y.PlanNum) || listPatPlansCur.Exists(z => z.PatPlanNum==y.PatPlanNum));
						if(!dictClaimProcs.TryGetValue(patNum,out listClaimProcsCur)) {
							listClaimProcsCur=new List<ClaimProc>();
						}
						foreach(Procedure procCur in listProcs) {//foreach proc for this patient
							Procedures.ComputeEstimates(procCur,patNum,ref listClaimProcsCur,false,listInsPlansCur,listPatPlansCur,listBenefitsCur,
								null,null,true,patAge,listInsSubsCur,listSubstLinks:listSubstitutionLinks,lookupFees:lookupFeesByCodeAndSched);
							double percentage=0;
							lock(lockObj) {
								percentage=Math.Ceiling(((double)(++rowCurIndex)/procCount)*100);
							}
							progress.Fire(ODEventType.FeeSched,
								new ProgressBarHelper(clinicCur.Abbr,(int)percentage+"%",(int)percentage,100,ProgBarStyle.Blocks,"WriteoffProgress"));
						}
					}
				})).ToList();
				ODThread.RunParallel(listActions,TimeSpan.FromHours(3),
					onException:new ODThread.ExceptionDelegate((ex) => {
						//Notify the user what went wrong via the text box.
						progress.Fire(ODEventType.FeeSched,new ProgressBarHelper("Error updating writeoffs: "+ex.Message,
							progressBarEventType:ProgBarEventType.TextMsg));
					})
				);
				if(listWriteoffClinics.Count>1) {//only show if more than one clinic
					progress.Fire(ODEventType.FeeSched,
						new ProgressBarHelper(rowCurIndex+" "+Lans.g("FeeSchedTools","procedures processed from")+" "+clinicCur.Abbr,
							progressBarEventType:ProgBarEventType.TextMsg));
				}
				totalWriteoffsUpdated+=rowCurIndex;
				if(doUpdatePrevClinicPref && rowCurIndex==procCount) {
					//if storing previously completed clinic and we actually completed this clinic's procs, update the pref
					if(listWriteoffClinics.Last().ClinicNum==clinicCur.ClinicNum) {
						//if this is the last clinic in the list, clear the last clinic pref so the next time it will run for all clinics
						Prefs.UpdateString(PrefName.GlobalUpdateWriteOffLastClinicCompleted,"");
					}
					else {
						Prefs.UpdateString(PrefName.GlobalUpdateWriteOffLastClinicCompleted,POut.Long(clinicCur.ClinicNum));
					}
					Signalods.SetInvalid(InvalidType.Prefs);
				}
				#region Has Cancelled
				if(progress.IsCanceled) {
					break;
				}
				#endregion Has Cancelled
			}
			progress.OnProgressDone();
			progress.Fire(ODEventType.FeeSched,new ProgressBarHelper("Writeoffs updated. "+totalWriteoffsUpdated+" procedures processed.\r\nDone.",
				progressBarEventType: ProgBarEventType.TextMsg));
			return totalWriteoffsUpdated;
		}

		#endregion

		#region CachePattern

		private class FeeSchedCache : CacheListAbs<FeeSched> {
			protected override List<FeeSched> GetCacheFromDb() {
				string command="SELECT * FROM feesched ORDER BY ItemOrder";
				return Crud.FeeSchedCrud.SelectMany(command);
			}
			protected override List<FeeSched> TableToList(DataTable table) {
				return Crud.FeeSchedCrud.TableToList(table);
			}
			protected override FeeSched Copy(FeeSched feeSched) {
				return feeSched.Copy();
			}
			protected override DataTable ListToTable(List<FeeSched> listFeeScheds) {
				return Crud.FeeSchedCrud.ListToTable(listFeeScheds,"FeeSched");
			}
			protected override void FillCacheIfNeeded() {
				FeeScheds.GetTableFromCache(false);
			}
			protected override bool IsInListShort(FeeSched feeSched) {
				return !feeSched.IsHidden;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static FeeSchedCache _feeSchedCache=new FeeSchedCache();

		public static int GetCount(bool isShort=false) {
			return _feeSchedCache.GetCount(isShort);
		}

		public static List<FeeSched> GetDeepCopy(bool isShort=false) {
			return _feeSchedCache.GetDeepCopy(isShort);
		}

		public static FeeSched GetFirst(bool isShort=true) {
			return _feeSchedCache.GetFirst(isShort);
		}

		public static FeeSched GetFirst(Func<FeeSched,bool> match,bool isShort=true) {
			return _feeSchedCache.GetFirst(match,isShort);
		}

		public static FeeSched GetFirstOrDefault(Func<FeeSched,bool> match,bool isShort=false) {
			return _feeSchedCache.GetFirstOrDefault(match,isShort);
		}

		public static List<FeeSched> GetWhere(Predicate<FeeSched> match,bool isShort=false) {
			return _feeSchedCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_feeSchedCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_feeSchedCache.FillCacheFromTable(table);
				return table;
			}
			return _feeSchedCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(FeeSched feeSched) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				feeSched.FeeSchedNum=Meth.GetLong(MethodBase.GetCurrentMethod(),feeSched);
				return feeSched.FeeSchedNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			feeSched.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.FeeSchedCrud.Insert(feeSched);
		}

		///<summary></summary>
		public static void Update(FeeSched feeSched) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),feeSched);
				return;
			}
			Crud.FeeSchedCrud.Update(feeSched);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.</summary>
		public static bool Sync(List<FeeSched> listNew,List<FeeSched> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			return Crud.FeeSchedCrud.Sync(listNew,listOld,Security.CurUser.UserNum);
		}

		///<summary>Returns the description of the fee schedule.  Appends (hidden) if the fee schedule has been hidden.</summary>
		public static string GetDescription(long feeSchedNum) {
			//No need to check RemotingRole; no call to db.
			string feeSchedDesc="";
			FeeSched feeSched=GetFirstOrDefault(x => x.FeeSchedNum==feeSchedNum);
			if(feeSched!=null) {
				feeSchedDesc=feeSched.Description+(feeSched.IsHidden ? " ("+Lans.g("FeeScheds","hidden")+")" : "");
			}
			return feeSchedDesc;
		}

		///<summary>Returns whether the FeeSched is hidden.  Defaults to true if not found.</summary>
		public static bool GetIsHidden(long feeSchedNum) {
			//No need to check RemotingRole; no call to db.
			FeeSched feeSched=GetFirstOrDefault(x => x.FeeSchedNum==feeSchedNum);
			return (feeSched==null ? true : feeSched.IsHidden);
		}

		///<summary>Returns whether the FeeSched has IsGlobal set to true.  Defaults to false if not found.</summary>
		public static bool IsGlobal(long feeSchedNum) {
			//No need to check RemotingRole; no call to db.
			FeeSched feeSched=GetFirstOrDefault(x => x.FeeSchedNum==feeSchedNum);
			return (feeSched==null ? false : feeSched.IsGlobal);
		}

		///<summary>Will return null if exact name not found.</summary>
		public static FeeSched GetByExactName(string description){
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.Description==description);
		}

		///<summary>Will return null if exact name not found.</summary>
		public static FeeSched GetByExactName(string description,FeeScheduleType feeSchedType){
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.FeeSchedType==feeSchedType && x.Description==description);
		}

		///<summary>Used to find FeeScheds of a certain type from within a given list.</summary>
		public static List<FeeSched> GetListForType(FeeScheduleType feeSchedType,bool includeHidden,List<FeeSched> listFeeScheds=null) {
			//No need to check RemotingRole; no call to db.
			listFeeScheds=listFeeScheds??GetDeepCopy();
			List<FeeSched> retVal=new List<FeeSched>();
			for(int i=0;i<listFeeScheds.Count;i++) {
				if(!includeHidden && listFeeScheds[i].IsHidden){
					continue;
				}
				if(listFeeScheds[i].FeeSchedType==feeSchedType){
					retVal.Add(listFeeScheds[i]);
				}
			}
			return retVal;
		}

		///<summary>Deletes FeeScheds that are hidden and not attached to any insurance plans.  Returns the number of deleted fee scheds.</summary>
		public static long CleanupAllowedScheds() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			long result;
			//Detach allowed FeeSchedules from any hidden InsPlans.
			string command="UPDATE insplan "
				+"SET AllowedFeeSched=0 "
				+"WHERE IsHidden=1";
			Db.NonQ(command);
			//Delete unattached FeeSchedules.
			command="DELETE FROM feesched "
				+"WHERE FeeSchedNum NOT IN (SELECT AllowedFeeSched FROM insplan) "
				+"AND FeeSchedType="+POut.Int((int)FeeScheduleType.OutNetwork);
			result=Db.NonQ(command);
			//Delete all orphaned fees.
			command="SELECT FeeNum FROM fee "
				+"WHERE FeeSched NOT IN (SELECT FeeSchedNum FROM feesched)";
			List<long> listFeeNums=Db.GetListLong(command);
			Fees.DeleteMany(listFeeNums);
			return result;
		}

		///<summary>Hides FeeScheds that are not hidden and not in use by anything. Returns the number of fee scheds that were hidden.</summary>
		public static long HideUnusedScheds() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			ODEvent.Fire(ODEventType.HideUnusedFeeSchedules,Lans.g("FormFeeScheds","Finding unused fee schedules..."));
			string command=@"SELECT feesched.FeeSchedNum 
				FROM feesched
				LEFT JOIN provider ON provider.FeeSched=feesched.FeeSchedNum
				LEFT JOIN patient ON patient.FeeSched=feesched.FeeSchedNum
				LEFT JOIN insplan ON insplan.FeeSched=feesched.FeeSchedNum
					OR insplan.AllowedFeeSched=feesched.FeeSchedNum
					OR insplan.CopayFeeSched=feesched.FeeSchedNum
				LEFT JOIN discountplan ON discountplan.FeeSchedNum=feesched.FeeSchedNum
				WHERE COALESCE(provider.FeeSched,patient.FeeSched,insplan.FeeSched,discountplan.FeeSchedNum) IS NULL
				AND feesched.IsHidden=0";
			List<long> listFeeScheds=Db.GetListLong(command);
			if(listFeeScheds.Count==0) {
				return 0;
			}
			ODEvent.Fire(ODEventType.HideUnusedFeeSchedules,Lans.g("FormFeeScheds","Hiding unused fee schedules..."));
			command="UPDATE feesched SET IsHidden=1 WHERE FeeSchedNum IN("+string.Join(",",listFeeScheds.Select(x => POut.Long(x)))+")";
			long rowsChanged=Db.NonQ(command);
			return rowsChanged;
		}
	}
}