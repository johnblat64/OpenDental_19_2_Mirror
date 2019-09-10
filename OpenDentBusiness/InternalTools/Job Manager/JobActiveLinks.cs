using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobActiveLinks{
		#region Get Methods
		///<summary></summary>
		public static JobActiveLink GetForJobAndUser(long jobNum,long userNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<JobActiveLink>(MethodBase.GetCurrentMethod(),jobNum,userNum);
			}
			string command="SELECT * FROM jobactivelink WHERE JobNum = "+POut.Long(jobNum)+" AND UserNum = "+POut.Long(userNum);
			return Crud.JobActiveLinkCrud.SelectOne(command);
		}		

		public static void ManageLink(Job job,Job jobOld,long userNum,bool isActive) {
			//No need for remoting call here.
			if(!jobOld.PhaseCur.In(JobPhase.Cancelled,JobPhase.Complete,JobPhase.Documentation) 
				&& job.PhaseCur.In(JobPhase.Cancelled,JobPhase.Complete,JobPhase.Documentation)) 
			{
				DeleteForJobNum(job.JobNum);
				return;
			}
			JobActiveLink activeLink=GetForJobAndUser(job.JobNum,userNum);
			if(activeLink==null) {
				if(!isActive) {
					return;
				}
				activeLink=new JobActiveLink();
				activeLink.JobNum=job.JobNum;
				activeLink.UserNum=userNum;
				Insert(activeLink);
			}
			else {
				if(isActive) {
					return;
				}
				Delete(activeLink.JobActiveLinkNum);
			}
			//Only occurs if an actual db change happened for a single user
			//Mass delete does not make a log entry
			JobLogs.MakeLogEntryForActive(job,userNum,isActive);
		}

		///<summary></summary>
		public static List<JobActiveLink> GetJobActiveLinksForJobNums(List<long> listJobNums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobActiveLink>>(MethodBase.GetCurrentMethod(),listJobNums);
			}
			if(listJobNums.Count<=0) {
				return new List<JobActiveLink>();
			}
			string command="SELECT * FROM jobactivelink WHERE JobNum IN("+String.Join(",",listJobNums)+")";
			return Crud.JobActiveLinkCrud.SelectMany(command);
		}
		
		///<summary>Gets one JobActiveLink from the db.</summary>
		public static JobActiveLink GetOne(long jobActiveLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<JobActiveLink>(MethodBase.GetCurrentMethod(),jobActiveLinkNum);
			}
			return Crud.JobActiveLinkCrud.SelectOne(jobActiveLinkNum);
		}
		#endregion Get Methods
		#region Modification Methods
		#region Insert
		///<summary></summary>
		public static long Insert(JobActiveLink jobActiveLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				jobActiveLink.JobActiveLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobActiveLink);
				return jobActiveLink.JobActiveLinkNum;
			}
			return Crud.JobActiveLinkCrud.Insert(jobActiveLink);
		}
		#endregion Insert
		#region Update
		///<summary></summary>
		public static void Update(JobActiveLink jobActiveLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobActiveLink);
				return;
			}
			Crud.JobActiveLinkCrud.Update(jobActiveLink);
		}
		#endregion Update
		#region Delete
		///<summary></summary>
		public static void Delete(long jobActiveLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobActiveLinkNum);
				return;
			}
			Crud.JobActiveLinkCrud.Delete(jobActiveLinkNum);
		}

		///<summary>Deletes the payment as well as all splits.
		///Surround with try catch, throws an exception if trying to delete a payment attached to a deposit.</summary>
		public static void DeleteForJobNum(long jobNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
				return;
			}
			string command="DELETE from jobactivelink WHERE JobNum = "+POut.Long(jobNum);
			Db.NonQ(command);
		}
		#endregion Delete
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		



	}
}