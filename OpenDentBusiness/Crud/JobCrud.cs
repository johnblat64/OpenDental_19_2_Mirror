//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class JobCrud {
		///<summary>Gets one Job object from the database using the primary key.  Returns null if not found.</summary>
		public static Job SelectOne(long jobNum) {
			string command="SELECT * FROM job "
				+"WHERE JobNum = "+POut.Long(jobNum);
			List<Job> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Job object from the database using a query.</summary>
		public static Job SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Job> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Job objects from the database using a query.</summary>
		public static List<Job> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Job> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Job> TableToList(DataTable table) {
			List<Job> retVal=new List<Job>();
			Job job;
			foreach(DataRow row in table.Rows) {
				job=new Job();
				job.JobNum                 = PIn.Long  (row["JobNum"].ToString());
				job.UserNumConcept         = PIn.Long  (row["UserNumConcept"].ToString());
				job.UserNumExpert          = PIn.Long  (row["UserNumExpert"].ToString());
				job.UserNumEngineer        = PIn.Long  (row["UserNumEngineer"].ToString());
				job.UserNumApproverConcept = PIn.Long  (row["UserNumApproverConcept"].ToString());
				job.UserNumApproverJob     = PIn.Long  (row["UserNumApproverJob"].ToString());
				job.UserNumApproverChange  = PIn.Long  (row["UserNumApproverChange"].ToString());
				job.UserNumDocumenter      = PIn.Long  (row["UserNumDocumenter"].ToString());
				job.UserNumCustContact     = PIn.Long  (row["UserNumCustContact"].ToString());
				job.UserNumCheckout        = PIn.Long  (row["UserNumCheckout"].ToString());
				job.UserNumInfo            = PIn.Long  (row["UserNumInfo"].ToString());
				job.ParentNum              = PIn.Long  (row["ParentNum"].ToString());
				job.DateTimeCustContact    = PIn.DateT (row["DateTimeCustContact"].ToString());
				job.Priority               = PIn.Long  (row["Priority"].ToString());
				string category=row["Category"].ToString();
				if(category=="") {
					job.Category             =(JobCategory)0;
				}
				else try{
					job.Category             =(JobCategory)Enum.Parse(typeof(JobCategory),category);
				}
				catch{
					job.Category             =(JobCategory)0;
				}
				job.JobVersion             = PIn.String(row["JobVersion"].ToString());
				job.TimeEstimateDevelopment= TimeSpan.FromTicks(PIn.Long(row["TimeEstimateDevelopment"].ToString()));
				job.TimeActual             = TimeSpan.FromTicks(PIn.Long(row["TimeActual"].ToString()));
				job.DateTimeEntry          = PIn.DateT (row["DateTimeEntry"].ToString());
				job.Implementation         = PIn.String(row["Implementation"].ToString());
				job.Documentation          = PIn.String(row["Documentation"].ToString());
				job.Title                  = PIn.String(row["Title"].ToString());
				string phaseCur=row["PhaseCur"].ToString();
				if(phaseCur=="") {
					job.PhaseCur             =(JobPhase)0;
				}
				else try{
					job.PhaseCur             =(JobPhase)Enum.Parse(typeof(JobPhase),phaseCur);
				}
				catch{
					job.PhaseCur             =(JobPhase)0;
				}
				job.IsApprovalNeeded       = PIn.Bool  (row["IsApprovalNeeded"].ToString());
				job.AckDateTime            = PIn.DateT (row["AckDateTime"].ToString());
				job.UserNumQuoter          = PIn.Long  (row["UserNumQuoter"].ToString());
				job.UserNumApproverQuote   = PIn.Long  (row["UserNumApproverQuote"].ToString());
				job.UserNumCustQuote       = PIn.Long  (row["UserNumCustQuote"].ToString());
				job.Requirements           = PIn.String(row["Requirements"].ToString());
				job.UserNumTester          = PIn.Long  (row["UserNumTester"].ToString());
				job.PriorityTesting        = PIn.Long  (row["PriorityTesting"].ToString());
				job.DateTimeTested         = PIn.DateT (row["DateTimeTested"].ToString());
				job.TimeEstimateConcept    = TimeSpan.FromTicks(PIn.Long(row["TimeEstimateConcept"].ToString()));
				job.TimeEstimateWriteup    = TimeSpan.FromTicks(PIn.Long(row["TimeEstimateWriteup"].ToString()));
				job.TimeEstimateReview     = TimeSpan.FromTicks(PIn.Long(row["TimeEstimateReview"].ToString()));
				job.RequirementsJSON       = PIn.String(row["RequirementsJSON"].ToString());
				job.PatternReviewProject   = (OpenDentBusiness.JobPatternReviewProject)PIn.Int(row["PatternReviewProject"].ToString());
				job.PatternReviewStatus    = (OpenDentBusiness.JobPatternReviewStatus)PIn.Int(row["PatternReviewStatus"].ToString());
				job.ProposedVersion        = (OpenDentBusiness.JobProposedVersion)PIn.Int(row["ProposedVersion"].ToString());
				job.DateTimeConceptApproval= PIn.DateT (row["DateTimeConceptApproval"].ToString());
				job.DateTimeJobApproval    = PIn.DateT (row["DateTimeJobApproval"].ToString());
				job.DateTimeImplemented    = PIn.DateT (row["DateTimeImplemented"].ToString());
				job.TimeTesting            = TimeSpan.FromTicks(PIn.Long(row["TimeTesting"].ToString()));
				job.IsNotTested            = PIn.Bool  (row["IsNotTested"].ToString());
				retVal.Add(job);
			}
			return retVal;
		}

		///<summary>Converts a list of Job into a DataTable.</summary>
		public static DataTable ListToTable(List<Job> listJobs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Job";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("JobNum");
			table.Columns.Add("UserNumConcept");
			table.Columns.Add("UserNumExpert");
			table.Columns.Add("UserNumEngineer");
			table.Columns.Add("UserNumApproverConcept");
			table.Columns.Add("UserNumApproverJob");
			table.Columns.Add("UserNumApproverChange");
			table.Columns.Add("UserNumDocumenter");
			table.Columns.Add("UserNumCustContact");
			table.Columns.Add("UserNumCheckout");
			table.Columns.Add("UserNumInfo");
			table.Columns.Add("ParentNum");
			table.Columns.Add("DateTimeCustContact");
			table.Columns.Add("Priority");
			table.Columns.Add("Category");
			table.Columns.Add("JobVersion");
			table.Columns.Add("TimeEstimateDevelopment");
			table.Columns.Add("TimeActual");
			table.Columns.Add("DateTimeEntry");
			table.Columns.Add("Implementation");
			table.Columns.Add("Documentation");
			table.Columns.Add("Title");
			table.Columns.Add("PhaseCur");
			table.Columns.Add("IsApprovalNeeded");
			table.Columns.Add("AckDateTime");
			table.Columns.Add("UserNumQuoter");
			table.Columns.Add("UserNumApproverQuote");
			table.Columns.Add("UserNumCustQuote");
			table.Columns.Add("Requirements");
			table.Columns.Add("UserNumTester");
			table.Columns.Add("PriorityTesting");
			table.Columns.Add("DateTimeTested");
			table.Columns.Add("TimeEstimateConcept");
			table.Columns.Add("TimeEstimateWriteup");
			table.Columns.Add("TimeEstimateReview");
			table.Columns.Add("RequirementsJSON");
			table.Columns.Add("PatternReviewProject");
			table.Columns.Add("PatternReviewStatus");
			table.Columns.Add("ProposedVersion");
			table.Columns.Add("DateTimeConceptApproval");
			table.Columns.Add("DateTimeJobApproval");
			table.Columns.Add("DateTimeImplemented");
			table.Columns.Add("TimeTesting");
			table.Columns.Add("IsNotTested");
			foreach(Job job in listJobs) {
				table.Rows.Add(new object[] {
					POut.Long  (job.JobNum),
					POut.Long  (job.UserNumConcept),
					POut.Long  (job.UserNumExpert),
					POut.Long  (job.UserNumEngineer),
					POut.Long  (job.UserNumApproverConcept),
					POut.Long  (job.UserNumApproverJob),
					POut.Long  (job.UserNumApproverChange),
					POut.Long  (job.UserNumDocumenter),
					POut.Long  (job.UserNumCustContact),
					POut.Long  (job.UserNumCheckout),
					POut.Long  (job.UserNumInfo),
					POut.Long  (job.ParentNum),
					POut.DateT (job.DateTimeCustContact,false),
					POut.Long  (job.Priority),
					POut.Int   ((int)job.Category),
					            job.JobVersion,
					POut.Long (job.TimeEstimateDevelopment.Ticks),
					POut.Long (job.TimeActual.Ticks),
					POut.DateT (job.DateTimeEntry,false),
					            job.Implementation,
					            job.Documentation,
					            job.Title,
					POut.Int   ((int)job.PhaseCur),
					POut.Bool  (job.IsApprovalNeeded),
					POut.DateT (job.AckDateTime,false),
					POut.Long  (job.UserNumQuoter),
					POut.Long  (job.UserNumApproverQuote),
					POut.Long  (job.UserNumCustQuote),
					            job.Requirements,
					POut.Long  (job.UserNumTester),
					POut.Long  (job.PriorityTesting),
					POut.DateT (job.DateTimeTested,false),
					POut.Long (job.TimeEstimateConcept.Ticks),
					POut.Long (job.TimeEstimateWriteup.Ticks),
					POut.Long (job.TimeEstimateReview.Ticks),
					            job.RequirementsJSON,
					POut.Int   ((int)job.PatternReviewProject),
					POut.Int   ((int)job.PatternReviewStatus),
					POut.Int   ((int)job.ProposedVersion),
					POut.DateT (job.DateTimeConceptApproval,false),
					POut.DateT (job.DateTimeJobApproval,false),
					POut.DateT (job.DateTimeImplemented,false),
					POut.Long (job.TimeTesting.Ticks),
					POut.Bool  (job.IsNotTested),
				});
			}
			return table;
		}

		///<summary>Inserts one Job into the database.  Returns the new priKey.</summary>
		public static long Insert(Job job) {
			return Insert(job,false);
		}

		///<summary>Inserts one Job into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Job job,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				job.JobNum=ReplicationServers.GetKey("job","JobNum");
			}
			string command="INSERT INTO job (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="JobNum,";
			}
			command+="UserNumConcept,UserNumExpert,UserNumEngineer,UserNumApproverConcept,UserNumApproverJob,UserNumApproverChange,UserNumDocumenter,UserNumCustContact,UserNumCheckout,UserNumInfo,ParentNum,DateTimeCustContact,Priority,Category,JobVersion,TimeEstimateDevelopment,TimeActual,DateTimeEntry,Implementation,Documentation,Title,PhaseCur,IsApprovalNeeded,AckDateTime,UserNumQuoter,UserNumApproverQuote,UserNumCustQuote,Requirements,UserNumTester,PriorityTesting,DateTimeTested,TimeEstimateConcept,TimeEstimateWriteup,TimeEstimateReview,RequirementsJSON,PatternReviewProject,PatternReviewStatus,ProposedVersion,DateTimeConceptApproval,DateTimeJobApproval,DateTimeImplemented,TimeTesting,IsNotTested) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(job.JobNum)+",";
			}
			command+=
				     POut.Long  (job.UserNumConcept)+","
				+    POut.Long  (job.UserNumExpert)+","
				+    POut.Long  (job.UserNumEngineer)+","
				+    POut.Long  (job.UserNumApproverConcept)+","
				+    POut.Long  (job.UserNumApproverJob)+","
				+    POut.Long  (job.UserNumApproverChange)+","
				+    POut.Long  (job.UserNumDocumenter)+","
				+    POut.Long  (job.UserNumCustContact)+","
				+    POut.Long  (job.UserNumCheckout)+","
				+    POut.Long  (job.UserNumInfo)+","
				+    POut.Long  (job.ParentNum)+","
				+    POut.DateT (job.DateTimeCustContact)+","
				+    POut.Long  (job.Priority)+","
				+"'"+POut.String(job.Category.ToString())+"',"
				+"'"+POut.String(job.JobVersion)+"',"
				+"'"+POut.Long  (job.TimeEstimateDevelopment.Ticks)+"',"
				+"'"+POut.Long  (job.TimeActual.Ticks)+"',"
				+    DbHelper.Now()+","
				+    DbHelper.ParamChar+"paramImplementation,"
				+    DbHelper.ParamChar+"paramDocumentation,"
				+"'"+POut.String(job.Title)+"',"
				+"'"+POut.String(job.PhaseCur.ToString())+"',"
				+    POut.Bool  (job.IsApprovalNeeded)+","
				+    POut.DateT (job.AckDateTime)+","
				+    POut.Long  (job.UserNumQuoter)+","
				+    POut.Long  (job.UserNumApproverQuote)+","
				+    POut.Long  (job.UserNumCustQuote)+","
				+    DbHelper.ParamChar+"paramRequirements,"
				+    POut.Long  (job.UserNumTester)+","
				+    POut.Long  (job.PriorityTesting)+","
				+    POut.DateT (job.DateTimeTested)+","
				+"'"+POut.Long  (job.TimeEstimateConcept.Ticks)+"',"
				+"'"+POut.Long  (job.TimeEstimateWriteup.Ticks)+"',"
				+"'"+POut.Long  (job.TimeEstimateReview.Ticks)+"',"
				+    DbHelper.ParamChar+"paramRequirementsJSON,"
				+    POut.Int   ((int)job.PatternReviewProject)+","
				+    POut.Int   ((int)job.PatternReviewStatus)+","
				+    POut.Int   ((int)job.ProposedVersion)+","
				+    POut.DateT (job.DateTimeConceptApproval)+","
				+    POut.DateT (job.DateTimeJobApproval)+","
				+    POut.DateT (job.DateTimeImplemented)+","
				+"'"+POut.Long  (job.TimeTesting.Ticks)+"',"
				+    POut.Bool  (job.IsNotTested)+")";
			if(job.Implementation==null) {
				job.Implementation="";
			}
			OdSqlParameter paramImplementation=new OdSqlParameter("paramImplementation",OdDbType.Text,POut.StringParam(job.Implementation));
			if(job.Documentation==null) {
				job.Documentation="";
			}
			OdSqlParameter paramDocumentation=new OdSqlParameter("paramDocumentation",OdDbType.Text,POut.StringParam(job.Documentation));
			if(job.Requirements==null) {
				job.Requirements="";
			}
			OdSqlParameter paramRequirements=new OdSqlParameter("paramRequirements",OdDbType.Text,POut.StringParam(job.Requirements));
			if(job.RequirementsJSON==null) {
				job.RequirementsJSON="";
			}
			OdSqlParameter paramRequirementsJSON=new OdSqlParameter("paramRequirementsJSON",OdDbType.Text,POut.StringParam(job.RequirementsJSON));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramImplementation,paramDocumentation,paramRequirements,paramRequirementsJSON);
			}
			else {
				job.JobNum=Db.NonQ(command,true,"JobNum","job",paramImplementation,paramDocumentation,paramRequirements,paramRequirementsJSON);
			}
			return job.JobNum;
		}

		///<summary>Inserts one Job into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Job job) {
			return InsertNoCache(job,false);
		}

		///<summary>Inserts one Job into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Job job,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO job (";
			if(!useExistingPK && isRandomKeys) {
				job.JobNum=ReplicationServers.GetKeyNoCache("job","JobNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="JobNum,";
			}
			command+="UserNumConcept,UserNumExpert,UserNumEngineer,UserNumApproverConcept,UserNumApproverJob,UserNumApproverChange,UserNumDocumenter,UserNumCustContact,UserNumCheckout,UserNumInfo,ParentNum,DateTimeCustContact,Priority,Category,JobVersion,TimeEstimateDevelopment,TimeActual,DateTimeEntry,Implementation,Documentation,Title,PhaseCur,IsApprovalNeeded,AckDateTime,UserNumQuoter,UserNumApproverQuote,UserNumCustQuote,Requirements,UserNumTester,PriorityTesting,DateTimeTested,TimeEstimateConcept,TimeEstimateWriteup,TimeEstimateReview,RequirementsJSON,PatternReviewProject,PatternReviewStatus,ProposedVersion,DateTimeConceptApproval,DateTimeJobApproval,DateTimeImplemented,TimeTesting,IsNotTested) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(job.JobNum)+",";
			}
			command+=
				     POut.Long  (job.UserNumConcept)+","
				+    POut.Long  (job.UserNumExpert)+","
				+    POut.Long  (job.UserNumEngineer)+","
				+    POut.Long  (job.UserNumApproverConcept)+","
				+    POut.Long  (job.UserNumApproverJob)+","
				+    POut.Long  (job.UserNumApproverChange)+","
				+    POut.Long  (job.UserNumDocumenter)+","
				+    POut.Long  (job.UserNumCustContact)+","
				+    POut.Long  (job.UserNumCheckout)+","
				+    POut.Long  (job.UserNumInfo)+","
				+    POut.Long  (job.ParentNum)+","
				+    POut.DateT (job.DateTimeCustContact)+","
				+    POut.Long  (job.Priority)+","
				+"'"+POut.String(job.Category.ToString())+"',"
				+"'"+POut.String(job.JobVersion)+"',"
				+"'"+POut.Long(job.TimeEstimateDevelopment.Ticks)+"',"
				+"'"+POut.Long(job.TimeActual.Ticks)+"',"
				+    DbHelper.Now()+","
				+    DbHelper.ParamChar+"paramImplementation,"
				+    DbHelper.ParamChar+"paramDocumentation,"
				+"'"+POut.String(job.Title)+"',"
				+"'"+POut.String(job.PhaseCur.ToString())+"',"
				+    POut.Bool  (job.IsApprovalNeeded)+","
				+    POut.DateT (job.AckDateTime)+","
				+    POut.Long  (job.UserNumQuoter)+","
				+    POut.Long  (job.UserNumApproverQuote)+","
				+    POut.Long  (job.UserNumCustQuote)+","
				+    DbHelper.ParamChar+"paramRequirements,"
				+    POut.Long  (job.UserNumTester)+","
				+    POut.Long  (job.PriorityTesting)+","
				+    POut.DateT (job.DateTimeTested)+","
				+"'"+POut.Long(job.TimeEstimateConcept.Ticks)+"',"
				+"'"+POut.Long(job.TimeEstimateWriteup.Ticks)+"',"
				+"'"+POut.Long(job.TimeEstimateReview.Ticks)+"',"
				+    DbHelper.ParamChar+"paramRequirementsJSON,"
				+    POut.Int   ((int)job.PatternReviewProject)+","
				+    POut.Int   ((int)job.PatternReviewStatus)+","
				+    POut.Int   ((int)job.ProposedVersion)+","
				+    POut.DateT (job.DateTimeConceptApproval)+","
				+    POut.DateT (job.DateTimeJobApproval)+","
				+    POut.DateT (job.DateTimeImplemented)+","
				+"'"+POut.Long(job.TimeTesting.Ticks)+"',"
				+    POut.Bool  (job.IsNotTested)+")";
			if(job.Implementation==null) {
				job.Implementation="";
			}
			OdSqlParameter paramImplementation=new OdSqlParameter("paramImplementation",OdDbType.Text,POut.StringParam(job.Implementation));
			if(job.Documentation==null) {
				job.Documentation="";
			}
			OdSqlParameter paramDocumentation=new OdSqlParameter("paramDocumentation",OdDbType.Text,POut.StringParam(job.Documentation));
			if(job.Requirements==null) {
				job.Requirements="";
			}
			OdSqlParameter paramRequirements=new OdSqlParameter("paramRequirements",OdDbType.Text,POut.StringParam(job.Requirements));
			if(job.RequirementsJSON==null) {
				job.RequirementsJSON="";
			}
			OdSqlParameter paramRequirementsJSON=new OdSqlParameter("paramRequirementsJSON",OdDbType.Text,POut.StringParam(job.RequirementsJSON));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramImplementation,paramDocumentation,paramRequirements,paramRequirementsJSON);
			}
			else {
				job.JobNum=Db.NonQ(command,true,"JobNum","job",paramImplementation,paramDocumentation,paramRequirements,paramRequirementsJSON);
			}
			return job.JobNum;
		}

		///<summary>Updates one Job in the database.</summary>
		public static void Update(Job job) {
			string command="UPDATE job SET "
				+"UserNumConcept         =  "+POut.Long  (job.UserNumConcept)+", "
				+"UserNumExpert          =  "+POut.Long  (job.UserNumExpert)+", "
				+"UserNumEngineer        =  "+POut.Long  (job.UserNumEngineer)+", "
				+"UserNumApproverConcept =  "+POut.Long  (job.UserNumApproverConcept)+", "
				+"UserNumApproverJob     =  "+POut.Long  (job.UserNumApproverJob)+", "
				+"UserNumApproverChange  =  "+POut.Long  (job.UserNumApproverChange)+", "
				+"UserNumDocumenter      =  "+POut.Long  (job.UserNumDocumenter)+", "
				+"UserNumCustContact     =  "+POut.Long  (job.UserNumCustContact)+", "
				+"UserNumCheckout        =  "+POut.Long  (job.UserNumCheckout)+", "
				+"UserNumInfo            =  "+POut.Long  (job.UserNumInfo)+", "
				+"ParentNum              =  "+POut.Long  (job.ParentNum)+", "
				+"DateTimeCustContact    =  "+POut.DateT (job.DateTimeCustContact)+", "
				+"Priority               =  "+POut.Long  (job.Priority)+", "
				+"Category               = '"+POut.String(job.Category.ToString())+"', "
				+"JobVersion             = '"+POut.String(job.JobVersion)+"', "
				+"TimeEstimateDevelopment=  "+POut.Long  (job.TimeEstimateDevelopment.Ticks)+", "
				+"TimeActual             =  "+POut.Long  (job.TimeActual.Ticks)+", "
				//DateTimeEntry not allowed to change
				+"Implementation         =  "+DbHelper.ParamChar+"paramImplementation, "
				+"Documentation          =  "+DbHelper.ParamChar+"paramDocumentation, "
				+"Title                  = '"+POut.String(job.Title)+"', "
				+"PhaseCur               = '"+POut.String(job.PhaseCur.ToString())+"', "
				+"IsApprovalNeeded       =  "+POut.Bool  (job.IsApprovalNeeded)+", "
				+"AckDateTime            =  "+POut.DateT (job.AckDateTime)+", "
				+"UserNumQuoter          =  "+POut.Long  (job.UserNumQuoter)+", "
				+"UserNumApproverQuote   =  "+POut.Long  (job.UserNumApproverQuote)+", "
				+"UserNumCustQuote       =  "+POut.Long  (job.UserNumCustQuote)+", "
				+"Requirements           =  "+DbHelper.ParamChar+"paramRequirements, "
				+"UserNumTester          =  "+POut.Long  (job.UserNumTester)+", "
				+"PriorityTesting        =  "+POut.Long  (job.PriorityTesting)+", "
				+"DateTimeTested         =  "+POut.DateT (job.DateTimeTested)+", "
				+"TimeEstimateConcept    =  "+POut.Long  (job.TimeEstimateConcept.Ticks)+", "
				+"TimeEstimateWriteup    =  "+POut.Long  (job.TimeEstimateWriteup.Ticks)+", "
				+"TimeEstimateReview     =  "+POut.Long  (job.TimeEstimateReview.Ticks)+", "
				+"RequirementsJSON       =  "+DbHelper.ParamChar+"paramRequirementsJSON, "
				+"PatternReviewProject   =  "+POut.Int   ((int)job.PatternReviewProject)+", "
				+"PatternReviewStatus    =  "+POut.Int   ((int)job.PatternReviewStatus)+", "
				+"ProposedVersion        =  "+POut.Int   ((int)job.ProposedVersion)+", "
				+"DateTimeConceptApproval=  "+POut.DateT (job.DateTimeConceptApproval)+", "
				+"DateTimeJobApproval    =  "+POut.DateT (job.DateTimeJobApproval)+", "
				+"DateTimeImplemented    =  "+POut.DateT (job.DateTimeImplemented)+", "
				+"TimeTesting            =  "+POut.Long  (job.TimeTesting.Ticks)+", "
				+"IsNotTested            =  "+POut.Bool  (job.IsNotTested)+" "
				+"WHERE JobNum = "+POut.Long(job.JobNum);
			if(job.Implementation==null) {
				job.Implementation="";
			}
			OdSqlParameter paramImplementation=new OdSqlParameter("paramImplementation",OdDbType.Text,POut.StringParam(job.Implementation));
			if(job.Documentation==null) {
				job.Documentation="";
			}
			OdSqlParameter paramDocumentation=new OdSqlParameter("paramDocumentation",OdDbType.Text,POut.StringParam(job.Documentation));
			if(job.Requirements==null) {
				job.Requirements="";
			}
			OdSqlParameter paramRequirements=new OdSqlParameter("paramRequirements",OdDbType.Text,POut.StringParam(job.Requirements));
			if(job.RequirementsJSON==null) {
				job.RequirementsJSON="";
			}
			OdSqlParameter paramRequirementsJSON=new OdSqlParameter("paramRequirementsJSON",OdDbType.Text,POut.StringParam(job.RequirementsJSON));
			Db.NonQ(command,paramImplementation,paramDocumentation,paramRequirements,paramRequirementsJSON);
		}

		///<summary>Updates one Job in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Job job,Job oldJob) {
			string command="";
			if(job.UserNumConcept != oldJob.UserNumConcept) {
				if(command!="") { command+=",";}
				command+="UserNumConcept = "+POut.Long(job.UserNumConcept)+"";
			}
			if(job.UserNumExpert != oldJob.UserNumExpert) {
				if(command!="") { command+=",";}
				command+="UserNumExpert = "+POut.Long(job.UserNumExpert)+"";
			}
			if(job.UserNumEngineer != oldJob.UserNumEngineer) {
				if(command!="") { command+=",";}
				command+="UserNumEngineer = "+POut.Long(job.UserNumEngineer)+"";
			}
			if(job.UserNumApproverConcept != oldJob.UserNumApproverConcept) {
				if(command!="") { command+=",";}
				command+="UserNumApproverConcept = "+POut.Long(job.UserNumApproverConcept)+"";
			}
			if(job.UserNumApproverJob != oldJob.UserNumApproverJob) {
				if(command!="") { command+=",";}
				command+="UserNumApproverJob = "+POut.Long(job.UserNumApproverJob)+"";
			}
			if(job.UserNumApproverChange != oldJob.UserNumApproverChange) {
				if(command!="") { command+=",";}
				command+="UserNumApproverChange = "+POut.Long(job.UserNumApproverChange)+"";
			}
			if(job.UserNumDocumenter != oldJob.UserNumDocumenter) {
				if(command!="") { command+=",";}
				command+="UserNumDocumenter = "+POut.Long(job.UserNumDocumenter)+"";
			}
			if(job.UserNumCustContact != oldJob.UserNumCustContact) {
				if(command!="") { command+=",";}
				command+="UserNumCustContact = "+POut.Long(job.UserNumCustContact)+"";
			}
			if(job.UserNumCheckout != oldJob.UserNumCheckout) {
				if(command!="") { command+=",";}
				command+="UserNumCheckout = "+POut.Long(job.UserNumCheckout)+"";
			}
			if(job.UserNumInfo != oldJob.UserNumInfo) {
				if(command!="") { command+=",";}
				command+="UserNumInfo = "+POut.Long(job.UserNumInfo)+"";
			}
			if(job.ParentNum != oldJob.ParentNum) {
				if(command!="") { command+=",";}
				command+="ParentNum = "+POut.Long(job.ParentNum)+"";
			}
			if(job.DateTimeCustContact != oldJob.DateTimeCustContact) {
				if(command!="") { command+=",";}
				command+="DateTimeCustContact = "+POut.DateT(job.DateTimeCustContact)+"";
			}
			if(job.Priority != oldJob.Priority) {
				if(command!="") { command+=",";}
				command+="Priority = "+POut.Long(job.Priority)+"";
			}
			if(job.Category != oldJob.Category) {
				if(command!="") { command+=",";}
				command+="Category = '"+POut.String(job.Category.ToString())+"'";
			}
			if(job.JobVersion != oldJob.JobVersion) {
				if(command!="") { command+=",";}
				command+="JobVersion = '"+POut.String(job.JobVersion)+"'";
			}
			if(job.TimeEstimateDevelopment != oldJob.TimeEstimateDevelopment) {
				if(command!="") { command+=",";}
				command+="TimeEstimateDevelopment = '"+POut.Long  (job.TimeEstimateDevelopment.Ticks)+"'";
			}
			if(job.TimeActual != oldJob.TimeActual) {
				if(command!="") { command+=",";}
				command+="TimeActual = '"+POut.Long  (job.TimeActual.Ticks)+"'";
			}
			//DateTimeEntry not allowed to change
			if(job.Implementation != oldJob.Implementation) {
				if(command!="") { command+=",";}
				command+="Implementation = "+DbHelper.ParamChar+"paramImplementation";
			}
			if(job.Documentation != oldJob.Documentation) {
				if(command!="") { command+=",";}
				command+="Documentation = "+DbHelper.ParamChar+"paramDocumentation";
			}
			if(job.Title != oldJob.Title) {
				if(command!="") { command+=",";}
				command+="Title = '"+POut.String(job.Title)+"'";
			}
			if(job.PhaseCur != oldJob.PhaseCur) {
				if(command!="") { command+=",";}
				command+="PhaseCur = '"+POut.String(job.PhaseCur.ToString())+"'";
			}
			if(job.IsApprovalNeeded != oldJob.IsApprovalNeeded) {
				if(command!="") { command+=",";}
				command+="IsApprovalNeeded = "+POut.Bool(job.IsApprovalNeeded)+"";
			}
			if(job.AckDateTime != oldJob.AckDateTime) {
				if(command!="") { command+=",";}
				command+="AckDateTime = "+POut.DateT(job.AckDateTime)+"";
			}
			if(job.UserNumQuoter != oldJob.UserNumQuoter) {
				if(command!="") { command+=",";}
				command+="UserNumQuoter = "+POut.Long(job.UserNumQuoter)+"";
			}
			if(job.UserNumApproverQuote != oldJob.UserNumApproverQuote) {
				if(command!="") { command+=",";}
				command+="UserNumApproverQuote = "+POut.Long(job.UserNumApproverQuote)+"";
			}
			if(job.UserNumCustQuote != oldJob.UserNumCustQuote) {
				if(command!="") { command+=",";}
				command+="UserNumCustQuote = "+POut.Long(job.UserNumCustQuote)+"";
			}
			if(job.Requirements != oldJob.Requirements) {
				if(command!="") { command+=",";}
				command+="Requirements = "+DbHelper.ParamChar+"paramRequirements";
			}
			if(job.UserNumTester != oldJob.UserNumTester) {
				if(command!="") { command+=",";}
				command+="UserNumTester = "+POut.Long(job.UserNumTester)+"";
			}
			if(job.PriorityTesting != oldJob.PriorityTesting) {
				if(command!="") { command+=",";}
				command+="PriorityTesting = "+POut.Long(job.PriorityTesting)+"";
			}
			if(job.DateTimeTested != oldJob.DateTimeTested) {
				if(command!="") { command+=",";}
				command+="DateTimeTested = "+POut.DateT(job.DateTimeTested)+"";
			}
			if(job.TimeEstimateConcept != oldJob.TimeEstimateConcept) {
				if(command!="") { command+=",";}
				command+="TimeEstimateConcept = '"+POut.Long  (job.TimeEstimateConcept.Ticks)+"'";
			}
			if(job.TimeEstimateWriteup != oldJob.TimeEstimateWriteup) {
				if(command!="") { command+=",";}
				command+="TimeEstimateWriteup = '"+POut.Long  (job.TimeEstimateWriteup.Ticks)+"'";
			}
			if(job.TimeEstimateReview != oldJob.TimeEstimateReview) {
				if(command!="") { command+=",";}
				command+="TimeEstimateReview = '"+POut.Long  (job.TimeEstimateReview.Ticks)+"'";
			}
			if(job.RequirementsJSON != oldJob.RequirementsJSON) {
				if(command!="") { command+=",";}
				command+="RequirementsJSON = "+DbHelper.ParamChar+"paramRequirementsJSON";
			}
			if(job.PatternReviewProject != oldJob.PatternReviewProject) {
				if(command!="") { command+=",";}
				command+="PatternReviewProject = "+POut.Int   ((int)job.PatternReviewProject)+"";
			}
			if(job.PatternReviewStatus != oldJob.PatternReviewStatus) {
				if(command!="") { command+=",";}
				command+="PatternReviewStatus = "+POut.Int   ((int)job.PatternReviewStatus)+"";
			}
			if(job.ProposedVersion != oldJob.ProposedVersion) {
				if(command!="") { command+=",";}
				command+="ProposedVersion = "+POut.Int   ((int)job.ProposedVersion)+"";
			}
			if(job.DateTimeConceptApproval != oldJob.DateTimeConceptApproval) {
				if(command!="") { command+=",";}
				command+="DateTimeConceptApproval = "+POut.DateT(job.DateTimeConceptApproval)+"";
			}
			if(job.DateTimeJobApproval != oldJob.DateTimeJobApproval) {
				if(command!="") { command+=",";}
				command+="DateTimeJobApproval = "+POut.DateT(job.DateTimeJobApproval)+"";
			}
			if(job.DateTimeImplemented != oldJob.DateTimeImplemented) {
				if(command!="") { command+=",";}
				command+="DateTimeImplemented = "+POut.DateT(job.DateTimeImplemented)+"";
			}
			if(job.TimeTesting != oldJob.TimeTesting) {
				if(command!="") { command+=",";}
				command+="TimeTesting = '"+POut.Long  (job.TimeTesting.Ticks)+"'";
			}
			if(job.IsNotTested != oldJob.IsNotTested) {
				if(command!="") { command+=",";}
				command+="IsNotTested = "+POut.Bool(job.IsNotTested)+"";
			}
			if(command=="") {
				return false;
			}
			if(job.Implementation==null) {
				job.Implementation="";
			}
			OdSqlParameter paramImplementation=new OdSqlParameter("paramImplementation",OdDbType.Text,POut.StringParam(job.Implementation));
			if(job.Documentation==null) {
				job.Documentation="";
			}
			OdSqlParameter paramDocumentation=new OdSqlParameter("paramDocumentation",OdDbType.Text,POut.StringParam(job.Documentation));
			if(job.Requirements==null) {
				job.Requirements="";
			}
			OdSqlParameter paramRequirements=new OdSqlParameter("paramRequirements",OdDbType.Text,POut.StringParam(job.Requirements));
			if(job.RequirementsJSON==null) {
				job.RequirementsJSON="";
			}
			OdSqlParameter paramRequirementsJSON=new OdSqlParameter("paramRequirementsJSON",OdDbType.Text,POut.StringParam(job.RequirementsJSON));
			command="UPDATE job SET "+command
				+" WHERE JobNum = "+POut.Long(job.JobNum);
			Db.NonQ(command,paramImplementation,paramDocumentation,paramRequirements,paramRequirementsJSON);
			return true;
		}

		///<summary>Returns true if Update(Job,Job) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Job job,Job oldJob) {
			if(job.UserNumConcept != oldJob.UserNumConcept) {
				return true;
			}
			if(job.UserNumExpert != oldJob.UserNumExpert) {
				return true;
			}
			if(job.UserNumEngineer != oldJob.UserNumEngineer) {
				return true;
			}
			if(job.UserNumApproverConcept != oldJob.UserNumApproverConcept) {
				return true;
			}
			if(job.UserNumApproverJob != oldJob.UserNumApproverJob) {
				return true;
			}
			if(job.UserNumApproverChange != oldJob.UserNumApproverChange) {
				return true;
			}
			if(job.UserNumDocumenter != oldJob.UserNumDocumenter) {
				return true;
			}
			if(job.UserNumCustContact != oldJob.UserNumCustContact) {
				return true;
			}
			if(job.UserNumCheckout != oldJob.UserNumCheckout) {
				return true;
			}
			if(job.UserNumInfo != oldJob.UserNumInfo) {
				return true;
			}
			if(job.ParentNum != oldJob.ParentNum) {
				return true;
			}
			if(job.DateTimeCustContact != oldJob.DateTimeCustContact) {
				return true;
			}
			if(job.Priority != oldJob.Priority) {
				return true;
			}
			if(job.Category != oldJob.Category) {
				return true;
			}
			if(job.JobVersion != oldJob.JobVersion) {
				return true;
			}
			if(job.TimeEstimateDevelopment != oldJob.TimeEstimateDevelopment) {
				return true;
			}
			if(job.TimeActual != oldJob.TimeActual) {
				return true;
			}
			//DateTimeEntry not allowed to change
			if(job.Implementation != oldJob.Implementation) {
				return true;
			}
			if(job.Documentation != oldJob.Documentation) {
				return true;
			}
			if(job.Title != oldJob.Title) {
				return true;
			}
			if(job.PhaseCur != oldJob.PhaseCur) {
				return true;
			}
			if(job.IsApprovalNeeded != oldJob.IsApprovalNeeded) {
				return true;
			}
			if(job.AckDateTime != oldJob.AckDateTime) {
				return true;
			}
			if(job.UserNumQuoter != oldJob.UserNumQuoter) {
				return true;
			}
			if(job.UserNumApproverQuote != oldJob.UserNumApproverQuote) {
				return true;
			}
			if(job.UserNumCustQuote != oldJob.UserNumCustQuote) {
				return true;
			}
			if(job.Requirements != oldJob.Requirements) {
				return true;
			}
			if(job.UserNumTester != oldJob.UserNumTester) {
				return true;
			}
			if(job.PriorityTesting != oldJob.PriorityTesting) {
				return true;
			}
			if(job.DateTimeTested != oldJob.DateTimeTested) {
				return true;
			}
			if(job.TimeEstimateConcept != oldJob.TimeEstimateConcept) {
				return true;
			}
			if(job.TimeEstimateWriteup != oldJob.TimeEstimateWriteup) {
				return true;
			}
			if(job.TimeEstimateReview != oldJob.TimeEstimateReview) {
				return true;
			}
			if(job.RequirementsJSON != oldJob.RequirementsJSON) {
				return true;
			}
			if(job.PatternReviewProject != oldJob.PatternReviewProject) {
				return true;
			}
			if(job.PatternReviewStatus != oldJob.PatternReviewStatus) {
				return true;
			}
			if(job.ProposedVersion != oldJob.ProposedVersion) {
				return true;
			}
			if(job.DateTimeConceptApproval != oldJob.DateTimeConceptApproval) {
				return true;
			}
			if(job.DateTimeJobApproval != oldJob.DateTimeJobApproval) {
				return true;
			}
			if(job.DateTimeImplemented != oldJob.DateTimeImplemented) {
				return true;
			}
			if(job.TimeTesting != oldJob.TimeTesting) {
				return true;
			}
			if(job.IsNotTested != oldJob.IsNotTested) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Job from the database.</summary>
		public static void Delete(long jobNum) {
			string command="DELETE FROM job "
				+"WHERE JobNum = "+POut.Long(jobNum);
			Db.NonQ(command);
		}

	}
}