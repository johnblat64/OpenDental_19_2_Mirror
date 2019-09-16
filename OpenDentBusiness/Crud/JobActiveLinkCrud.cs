//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class JobActiveLinkCrud {
		///<summary>Gets one JobActiveLink object from the database using the primary key.  Returns null if not found.</summary>
		public static JobActiveLink SelectOne(long jobActiveLinkNum) {
			string command="SELECT * FROM jobactivelink "
				+"WHERE JobActiveLinkNum = "+POut.Long(jobActiveLinkNum);
			List<JobActiveLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one JobActiveLink object from the database using a query.</summary>
		public static JobActiveLink SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<JobActiveLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of JobActiveLink objects from the database using a query.</summary>
		public static List<JobActiveLink> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<JobActiveLink> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<JobActiveLink> TableToList(DataTable table) {
			List<JobActiveLink> retVal=new List<JobActiveLink>();
			JobActiveLink jobActiveLink;
			foreach(DataRow row in table.Rows) {
				jobActiveLink=new JobActiveLink();
				jobActiveLink.JobActiveLinkNum= PIn.Long  (row["JobActiveLinkNum"].ToString());
				jobActiveLink.JobNum          = PIn.Long  (row["JobNum"].ToString());
				jobActiveLink.UserNum         = PIn.Long  (row["UserNum"].ToString());
				jobActiveLink.DateTimeEntry   = PIn.DateT (row["DateTimeEntry"].ToString());
				jobActiveLink.DateTimeEnd     = PIn.DateT (row["DateTimeEnd"].ToString());
				retVal.Add(jobActiveLink);
			}
			return retVal;
		}

		///<summary>Converts a list of JobActiveLink into a DataTable.</summary>
		public static DataTable ListToTable(List<JobActiveLink> listJobActiveLinks,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="JobActiveLink";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("JobActiveLinkNum");
			table.Columns.Add("JobNum");
			table.Columns.Add("UserNum");
			table.Columns.Add("DateTimeEntry");
			table.Columns.Add("DateTimeEnd");
			foreach(JobActiveLink jobActiveLink in listJobActiveLinks) {
				table.Rows.Add(new object[] {
					POut.Long  (jobActiveLink.JobActiveLinkNum),
					POut.Long  (jobActiveLink.JobNum),
					POut.Long  (jobActiveLink.UserNum),
					POut.DateT (jobActiveLink.DateTimeEntry,false),
					POut.DateT (jobActiveLink.DateTimeEnd,false),
				});
			}
			return table;
		}

		///<summary>Inserts one JobActiveLink into the database.  Returns the new priKey.</summary>
		public static long Insert(JobActiveLink jobActiveLink) {
			return Insert(jobActiveLink,false);
		}

		///<summary>Inserts one JobActiveLink into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(JobActiveLink jobActiveLink,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				jobActiveLink.JobActiveLinkNum=ReplicationServers.GetKey("jobactivelink","JobActiveLinkNum");
			}
			string command="INSERT INTO jobactivelink (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="JobActiveLinkNum,";
			}
			command+="JobNum,UserNum,DateTimeEntry,DateTimeEnd) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(jobActiveLink.JobActiveLinkNum)+",";
			}
			command+=
				     POut.Long  (jobActiveLink.JobNum)+","
				+    POut.Long  (jobActiveLink.UserNum)+","
				+    DbHelper.Now()+","
				+    POut.DateT (jobActiveLink.DateTimeEnd)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				jobActiveLink.JobActiveLinkNum=Db.NonQ(command,true,"JobActiveLinkNum","jobActiveLink");
			}
			return jobActiveLink.JobActiveLinkNum;
		}

		///<summary>Inserts one JobActiveLink into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(JobActiveLink jobActiveLink) {
			return InsertNoCache(jobActiveLink,false);
		}

		///<summary>Inserts one JobActiveLink into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(JobActiveLink jobActiveLink,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO jobactivelink (";
			if(!useExistingPK && isRandomKeys) {
				jobActiveLink.JobActiveLinkNum=ReplicationServers.GetKeyNoCache("jobactivelink","JobActiveLinkNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="JobActiveLinkNum,";
			}
			command+="JobNum,UserNum,DateTimeEntry,DateTimeEnd) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(jobActiveLink.JobActiveLinkNum)+",";
			}
			command+=
				     POut.Long  (jobActiveLink.JobNum)+","
				+    POut.Long  (jobActiveLink.UserNum)+","
				+    DbHelper.Now()+","
				+    POut.DateT (jobActiveLink.DateTimeEnd)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				jobActiveLink.JobActiveLinkNum=Db.NonQ(command,true,"JobActiveLinkNum","jobActiveLink");
			}
			return jobActiveLink.JobActiveLinkNum;
		}

		///<summary>Updates one JobActiveLink in the database.</summary>
		public static void Update(JobActiveLink jobActiveLink) {
			string command="UPDATE jobactivelink SET "
				+"JobNum          =  "+POut.Long  (jobActiveLink.JobNum)+", "
				+"UserNum         =  "+POut.Long  (jobActiveLink.UserNum)+", "
				//DateTimeEntry not allowed to change
				+"DateTimeEnd     =  "+POut.DateT (jobActiveLink.DateTimeEnd)+" "
				+"WHERE JobActiveLinkNum = "+POut.Long(jobActiveLink.JobActiveLinkNum);
			Db.NonQ(command);
		}

		///<summary>Updates one JobActiveLink in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(JobActiveLink jobActiveLink,JobActiveLink oldJobActiveLink) {
			string command="";
			if(jobActiveLink.JobNum != oldJobActiveLink.JobNum) {
				if(command!="") { command+=",";}
				command+="JobNum = "+POut.Long(jobActiveLink.JobNum)+"";
			}
			if(jobActiveLink.UserNum != oldJobActiveLink.UserNum) {
				if(command!="") { command+=",";}
				command+="UserNum = "+POut.Long(jobActiveLink.UserNum)+"";
			}
			//DateTimeEntry not allowed to change
			if(jobActiveLink.DateTimeEnd != oldJobActiveLink.DateTimeEnd) {
				if(command!="") { command+=",";}
				command+="DateTimeEnd = "+POut.DateT(jobActiveLink.DateTimeEnd)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE jobactivelink SET "+command
				+" WHERE JobActiveLinkNum = "+POut.Long(jobActiveLink.JobActiveLinkNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(JobActiveLink,JobActiveLink) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(JobActiveLink jobActiveLink,JobActiveLink oldJobActiveLink) {
			if(jobActiveLink.JobNum != oldJobActiveLink.JobNum) {
				return true;
			}
			if(jobActiveLink.UserNum != oldJobActiveLink.UserNum) {
				return true;
			}
			//DateTimeEntry not allowed to change
			if(jobActiveLink.DateTimeEnd != oldJobActiveLink.DateTimeEnd) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one JobActiveLink from the database.</summary>
		public static void Delete(long jobActiveLinkNum) {
			string command="DELETE FROM jobactivelink "
				+"WHERE JobActiveLinkNum = "+POut.Long(jobActiveLinkNum);
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<JobActiveLink> listNew,List<JobActiveLink> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<JobActiveLink> listIns    =new List<JobActiveLink>();
			List<JobActiveLink> listUpdNew =new List<JobActiveLink>();
			List<JobActiveLink> listUpdDB  =new List<JobActiveLink>();
			List<JobActiveLink> listDel    =new List<JobActiveLink>();
			listNew.Sort((JobActiveLink x,JobActiveLink y) => { return x.JobActiveLinkNum.CompareTo(y.JobActiveLinkNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((JobActiveLink x,JobActiveLink y) => { return x.JobActiveLinkNum.CompareTo(y.JobActiveLinkNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			JobActiveLink fieldNew;
			JobActiveLink fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.JobActiveLinkNum<fieldDB.JobActiveLinkNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.JobActiveLinkNum>fieldDB.JobActiveLinkNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(Update(listUpdNew[i],listUpdDB[i])) {
					rowsUpdatedCount++;
				}
			}
			for(int i=0;i<listDel.Count;i++) {
				Delete(listDel[i].JobActiveLinkNum);
			}
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}

	}
}