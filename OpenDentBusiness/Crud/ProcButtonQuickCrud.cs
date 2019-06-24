//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class ProcButtonQuickCrud {
		///<summary>Gets one ProcButtonQuick object from the database using the primary key.  Returns null if not found.</summary>
		public static ProcButtonQuick SelectOne(long procButtonQuickNum) {
			string command="SELECT * FROM procbuttonquick "
				+"WHERE ProcButtonQuickNum = "+POut.Long(procButtonQuickNum);
			List<ProcButtonQuick> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one ProcButtonQuick object from the database using a query.</summary>
		public static ProcButtonQuick SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ProcButtonQuick> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of ProcButtonQuick objects from the database using a query.</summary>
		public static List<ProcButtonQuick> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ProcButtonQuick> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<ProcButtonQuick> TableToList(DataTable table) {
			List<ProcButtonQuick> retVal=new List<ProcButtonQuick>();
			ProcButtonQuick procButtonQuick;
			foreach(DataRow row in table.Rows) {
				procButtonQuick=new ProcButtonQuick();
				procButtonQuick.ProcButtonQuickNum= PIn.Long  (row["ProcButtonQuickNum"].ToString());
				procButtonQuick.Description       = PIn.String(row["Description"].ToString());
				procButtonQuick.CodeValue         = PIn.String(row["CodeValue"].ToString());
				procButtonQuick.Surf              = PIn.String(row["Surf"].ToString());
				procButtonQuick.YPos              = PIn.Int   (row["YPos"].ToString());
				procButtonQuick.ItemOrder         = PIn.Int   (row["ItemOrder"].ToString());
				procButtonQuick.IsLabel           = PIn.Bool  (row["IsLabel"].ToString());
				retVal.Add(procButtonQuick);
			}
			return retVal;
		}

		///<summary>Converts a list of ProcButtonQuick into a DataTable.</summary>
		public static DataTable ListToTable(List<ProcButtonQuick> listProcButtonQuicks,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="ProcButtonQuick";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("ProcButtonQuickNum");
			table.Columns.Add("Description");
			table.Columns.Add("CodeValue");
			table.Columns.Add("Surf");
			table.Columns.Add("YPos");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("IsLabel");
			foreach(ProcButtonQuick procButtonQuick in listProcButtonQuicks) {
				table.Rows.Add(new object[] {
					POut.Long  (procButtonQuick.ProcButtonQuickNum),
					            procButtonQuick.Description,
					            procButtonQuick.CodeValue,
					            procButtonQuick.Surf,
					POut.Int   (procButtonQuick.YPos),
					POut.Int   (procButtonQuick.ItemOrder),
					POut.Bool  (procButtonQuick.IsLabel),
				});
			}
			return table;
		}

		///<summary>Inserts one ProcButtonQuick into the database.  Returns the new priKey.</summary>
		public static long Insert(ProcButtonQuick procButtonQuick) {
			return Insert(procButtonQuick,false);
		}

		///<summary>Inserts one ProcButtonQuick into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(ProcButtonQuick procButtonQuick,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				procButtonQuick.ProcButtonQuickNum=ReplicationServers.GetKey("procbuttonquick","ProcButtonQuickNum");
			}
			string command="INSERT INTO procbuttonquick (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="ProcButtonQuickNum,";
			}
			command+="Description,CodeValue,Surf,YPos,ItemOrder,IsLabel) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(procButtonQuick.ProcButtonQuickNum)+",";
			}
			command+=
				 "'"+POut.String(procButtonQuick.Description)+"',"
				+"'"+POut.String(procButtonQuick.CodeValue)+"',"
				+"'"+POut.String(procButtonQuick.Surf)+"',"
				+    POut.Int   (procButtonQuick.YPos)+","
				+    POut.Int   (procButtonQuick.ItemOrder)+","
				+    POut.Bool  (procButtonQuick.IsLabel)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				procButtonQuick.ProcButtonQuickNum=Db.NonQ(command,true,"ProcButtonQuickNum","procButtonQuick");
			}
			return procButtonQuick.ProcButtonQuickNum;
		}

		///<summary>Inserts one ProcButtonQuick into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ProcButtonQuick procButtonQuick) {
			return InsertNoCache(procButtonQuick,false);
		}

		///<summary>Inserts one ProcButtonQuick into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ProcButtonQuick procButtonQuick,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO procbuttonquick (";
			if(!useExistingPK && isRandomKeys) {
				procButtonQuick.ProcButtonQuickNum=ReplicationServers.GetKeyNoCache("procbuttonquick","ProcButtonQuickNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="ProcButtonQuickNum,";
			}
			command+="Description,CodeValue,Surf,YPos,ItemOrder,IsLabel) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(procButtonQuick.ProcButtonQuickNum)+",";
			}
			command+=
				 "'"+POut.String(procButtonQuick.Description)+"',"
				+"'"+POut.String(procButtonQuick.CodeValue)+"',"
				+"'"+POut.String(procButtonQuick.Surf)+"',"
				+    POut.Int   (procButtonQuick.YPos)+","
				+    POut.Int   (procButtonQuick.ItemOrder)+","
				+    POut.Bool  (procButtonQuick.IsLabel)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				procButtonQuick.ProcButtonQuickNum=Db.NonQ(command,true,"ProcButtonQuickNum","procButtonQuick");
			}
			return procButtonQuick.ProcButtonQuickNum;
		}

		///<summary>Updates one ProcButtonQuick in the database.</summary>
		public static void Update(ProcButtonQuick procButtonQuick) {
			string command="UPDATE procbuttonquick SET "
				+"Description       = '"+POut.String(procButtonQuick.Description)+"', "
				+"CodeValue         = '"+POut.String(procButtonQuick.CodeValue)+"', "
				+"Surf              = '"+POut.String(procButtonQuick.Surf)+"', "
				+"YPos              =  "+POut.Int   (procButtonQuick.YPos)+", "
				+"ItemOrder         =  "+POut.Int   (procButtonQuick.ItemOrder)+", "
				+"IsLabel           =  "+POut.Bool  (procButtonQuick.IsLabel)+" "
				+"WHERE ProcButtonQuickNum = "+POut.Long(procButtonQuick.ProcButtonQuickNum);
			Db.NonQ(command);
		}

		///<summary>Updates one ProcButtonQuick in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(ProcButtonQuick procButtonQuick,ProcButtonQuick oldProcButtonQuick) {
			string command="";
			if(procButtonQuick.Description != oldProcButtonQuick.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(procButtonQuick.Description)+"'";
			}
			if(procButtonQuick.CodeValue != oldProcButtonQuick.CodeValue) {
				if(command!="") { command+=",";}
				command+="CodeValue = '"+POut.String(procButtonQuick.CodeValue)+"'";
			}
			if(procButtonQuick.Surf != oldProcButtonQuick.Surf) {
				if(command!="") { command+=",";}
				command+="Surf = '"+POut.String(procButtonQuick.Surf)+"'";
			}
			if(procButtonQuick.YPos != oldProcButtonQuick.YPos) {
				if(command!="") { command+=",";}
				command+="YPos = "+POut.Int(procButtonQuick.YPos)+"";
			}
			if(procButtonQuick.ItemOrder != oldProcButtonQuick.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(procButtonQuick.ItemOrder)+"";
			}
			if(procButtonQuick.IsLabel != oldProcButtonQuick.IsLabel) {
				if(command!="") { command+=",";}
				command+="IsLabel = "+POut.Bool(procButtonQuick.IsLabel)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE procbuttonquick SET "+command
				+" WHERE ProcButtonQuickNum = "+POut.Long(procButtonQuick.ProcButtonQuickNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(ProcButtonQuick,ProcButtonQuick) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(ProcButtonQuick procButtonQuick,ProcButtonQuick oldProcButtonQuick) {
			if(procButtonQuick.Description != oldProcButtonQuick.Description) {
				return true;
			}
			if(procButtonQuick.CodeValue != oldProcButtonQuick.CodeValue) {
				return true;
			}
			if(procButtonQuick.Surf != oldProcButtonQuick.Surf) {
				return true;
			}
			if(procButtonQuick.YPos != oldProcButtonQuick.YPos) {
				return true;
			}
			if(procButtonQuick.ItemOrder != oldProcButtonQuick.ItemOrder) {
				return true;
			}
			if(procButtonQuick.IsLabel != oldProcButtonQuick.IsLabel) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one ProcButtonQuick from the database.</summary>
		public static void Delete(long procButtonQuickNum) {
			string command="DELETE FROM procbuttonquick "
				+"WHERE ProcButtonQuickNum = "+POut.Long(procButtonQuickNum);
			Db.NonQ(command);
		}

	}
}