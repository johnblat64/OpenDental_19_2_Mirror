//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class StateAbbrCrud {
		///<summary>Gets one StateAbbr object from the database using the primary key.  Returns null if not found.</summary>
		public static StateAbbr SelectOne(long stateAbbrNum) {
			string command="SELECT * FROM stateabbr "
				+"WHERE StateAbbrNum = "+POut.Long(stateAbbrNum);
			List<StateAbbr> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one StateAbbr object from the database using a query.</summary>
		public static StateAbbr SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<StateAbbr> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of StateAbbr objects from the database using a query.</summary>
		public static List<StateAbbr> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<StateAbbr> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<StateAbbr> TableToList(DataTable table) {
			List<StateAbbr> retVal=new List<StateAbbr>();
			StateAbbr stateAbbr;
			foreach(DataRow row in table.Rows) {
				stateAbbr=new StateAbbr();
				stateAbbr.StateAbbrNum    = PIn.Long  (row["StateAbbrNum"].ToString());
				stateAbbr.Description     = PIn.String(row["Description"].ToString());
				stateAbbr.Abbr            = PIn.String(row["Abbr"].ToString());
				stateAbbr.MedicaidIDLength= PIn.Int   (row["MedicaidIDLength"].ToString());
				retVal.Add(stateAbbr);
			}
			return retVal;
		}

		///<summary>Converts a list of StateAbbr into a DataTable.</summary>
		public static DataTable ListToTable(List<StateAbbr> listStateAbbrs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="StateAbbr";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("StateAbbrNum");
			table.Columns.Add("Description");
			table.Columns.Add("Abbr");
			table.Columns.Add("MedicaidIDLength");
			foreach(StateAbbr stateAbbr in listStateAbbrs) {
				table.Rows.Add(new object[] {
					POut.Long  (stateAbbr.StateAbbrNum),
					            stateAbbr.Description,
					            stateAbbr.Abbr,
					POut.Int   (stateAbbr.MedicaidIDLength),
				});
			}
			return table;
		}

		///<summary>Inserts one StateAbbr into the database.  Returns the new priKey.</summary>
		public static long Insert(StateAbbr stateAbbr) {
			return Insert(stateAbbr,false);
		}

		///<summary>Inserts one StateAbbr into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(StateAbbr stateAbbr,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				stateAbbr.StateAbbrNum=ReplicationServers.GetKey("stateabbr","StateAbbrNum");
			}
			string command="INSERT INTO stateabbr (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="StateAbbrNum,";
			}
			command+="Description,Abbr,MedicaidIDLength) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(stateAbbr.StateAbbrNum)+",";
			}
			command+=
				 "'"+POut.String(stateAbbr.Description)+"',"
				+"'"+POut.String(stateAbbr.Abbr)+"',"
				+    POut.Int   (stateAbbr.MedicaidIDLength)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				stateAbbr.StateAbbrNum=Db.NonQ(command,true,"StateAbbrNum","stateAbbr");
			}
			return stateAbbr.StateAbbrNum;
		}

		///<summary>Inserts one StateAbbr into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(StateAbbr stateAbbr) {
			return InsertNoCache(stateAbbr,false);
		}

		///<summary>Inserts one StateAbbr into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(StateAbbr stateAbbr,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO stateabbr (";
			if(!useExistingPK && isRandomKeys) {
				stateAbbr.StateAbbrNum=ReplicationServers.GetKeyNoCache("stateabbr","StateAbbrNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="StateAbbrNum,";
			}
			command+="Description,Abbr,MedicaidIDLength) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(stateAbbr.StateAbbrNum)+",";
			}
			command+=
				 "'"+POut.String(stateAbbr.Description)+"',"
				+"'"+POut.String(stateAbbr.Abbr)+"',"
				+    POut.Int   (stateAbbr.MedicaidIDLength)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				stateAbbr.StateAbbrNum=Db.NonQ(command,true,"StateAbbrNum","stateAbbr");
			}
			return stateAbbr.StateAbbrNum;
		}

		///<summary>Updates one StateAbbr in the database.</summary>
		public static void Update(StateAbbr stateAbbr) {
			string command="UPDATE stateabbr SET "
				+"Description     = '"+POut.String(stateAbbr.Description)+"', "
				+"Abbr            = '"+POut.String(stateAbbr.Abbr)+"', "
				+"MedicaidIDLength=  "+POut.Int   (stateAbbr.MedicaidIDLength)+" "
				+"WHERE StateAbbrNum = "+POut.Long(stateAbbr.StateAbbrNum);
			Db.NonQ(command);
		}

		///<summary>Updates one StateAbbr in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(StateAbbr stateAbbr,StateAbbr oldStateAbbr) {
			string command="";
			if(stateAbbr.Description != oldStateAbbr.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(stateAbbr.Description)+"'";
			}
			if(stateAbbr.Abbr != oldStateAbbr.Abbr) {
				if(command!="") { command+=",";}
				command+="Abbr = '"+POut.String(stateAbbr.Abbr)+"'";
			}
			if(stateAbbr.MedicaidIDLength != oldStateAbbr.MedicaidIDLength) {
				if(command!="") { command+=",";}
				command+="MedicaidIDLength = "+POut.Int(stateAbbr.MedicaidIDLength)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE stateabbr SET "+command
				+" WHERE StateAbbrNum = "+POut.Long(stateAbbr.StateAbbrNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(StateAbbr,StateAbbr) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(StateAbbr stateAbbr,StateAbbr oldStateAbbr) {
			if(stateAbbr.Description != oldStateAbbr.Description) {
				return true;
			}
			if(stateAbbr.Abbr != oldStateAbbr.Abbr) {
				return true;
			}
			if(stateAbbr.MedicaidIDLength != oldStateAbbr.MedicaidIDLength) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one StateAbbr from the database.</summary>
		public static void Delete(long stateAbbrNum) {
			string command="DELETE FROM stateabbr "
				+"WHERE StateAbbrNum = "+POut.Long(stateAbbrNum);
			Db.NonQ(command);
		}

	}
}