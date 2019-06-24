//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class VaccineObsCrud {
		///<summary>Gets one VaccineObs object from the database using the primary key.  Returns null if not found.</summary>
		public static VaccineObs SelectOne(long vaccineObsNum) {
			string command="SELECT * FROM vaccineobs "
				+"WHERE VaccineObsNum = "+POut.Long(vaccineObsNum);
			List<VaccineObs> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one VaccineObs object from the database using a query.</summary>
		public static VaccineObs SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<VaccineObs> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of VaccineObs objects from the database using a query.</summary>
		public static List<VaccineObs> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<VaccineObs> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<VaccineObs> TableToList(DataTable table) {
			List<VaccineObs> retVal=new List<VaccineObs>();
			VaccineObs vaccineObs;
			foreach(DataRow row in table.Rows) {
				vaccineObs=new VaccineObs();
				vaccineObs.VaccineObsNum     = PIn.Long  (row["VaccineObsNum"].ToString());
				vaccineObs.VaccinePatNum     = PIn.Long  (row["VaccinePatNum"].ToString());
				vaccineObs.ValType           = (OpenDentBusiness.VaccineObsType)PIn.Int(row["ValType"].ToString());
				vaccineObs.IdentifyingCode   = (OpenDentBusiness.VaccineObsIdentifier)PIn.Int(row["IdentifyingCode"].ToString());
				vaccineObs.ValReported       = PIn.String(row["ValReported"].ToString());
				vaccineObs.ValCodeSystem     = (OpenDentBusiness.VaccineObsValCodeSystem)PIn.Int(row["ValCodeSystem"].ToString());
				vaccineObs.VaccineObsNumGroup= PIn.Long  (row["VaccineObsNumGroup"].ToString());
				vaccineObs.UcumCode          = PIn.String(row["UcumCode"].ToString());
				vaccineObs.DateObs           = PIn.Date  (row["DateObs"].ToString());
				vaccineObs.MethodCode        = PIn.String(row["MethodCode"].ToString());
				retVal.Add(vaccineObs);
			}
			return retVal;
		}

		///<summary>Converts a list of VaccineObs into a DataTable.</summary>
		public static DataTable ListToTable(List<VaccineObs> listVaccineObss,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="VaccineObs";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("VaccineObsNum");
			table.Columns.Add("VaccinePatNum");
			table.Columns.Add("ValType");
			table.Columns.Add("IdentifyingCode");
			table.Columns.Add("ValReported");
			table.Columns.Add("ValCodeSystem");
			table.Columns.Add("VaccineObsNumGroup");
			table.Columns.Add("UcumCode");
			table.Columns.Add("DateObs");
			table.Columns.Add("MethodCode");
			foreach(VaccineObs vaccineObs in listVaccineObss) {
				table.Rows.Add(new object[] {
					POut.Long  (vaccineObs.VaccineObsNum),
					POut.Long  (vaccineObs.VaccinePatNum),
					POut.Int   ((int)vaccineObs.ValType),
					POut.Int   ((int)vaccineObs.IdentifyingCode),
					            vaccineObs.ValReported,
					POut.Int   ((int)vaccineObs.ValCodeSystem),
					POut.Long  (vaccineObs.VaccineObsNumGroup),
					            vaccineObs.UcumCode,
					POut.DateT (vaccineObs.DateObs,false),
					            vaccineObs.MethodCode,
				});
			}
			return table;
		}

		///<summary>Inserts one VaccineObs into the database.  Returns the new priKey.</summary>
		public static long Insert(VaccineObs vaccineObs) {
			return Insert(vaccineObs,false);
		}

		///<summary>Inserts one VaccineObs into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(VaccineObs vaccineObs,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				vaccineObs.VaccineObsNum=ReplicationServers.GetKey("vaccineobs","VaccineObsNum");
			}
			string command="INSERT INTO vaccineobs (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="VaccineObsNum,";
			}
			command+="VaccinePatNum,ValType,IdentifyingCode,ValReported,ValCodeSystem,VaccineObsNumGroup,UcumCode,DateObs,MethodCode) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(vaccineObs.VaccineObsNum)+",";
			}
			command+=
				     POut.Long  (vaccineObs.VaccinePatNum)+","
				+    POut.Int   ((int)vaccineObs.ValType)+","
				+    POut.Int   ((int)vaccineObs.IdentifyingCode)+","
				+"'"+POut.String(vaccineObs.ValReported)+"',"
				+    POut.Int   ((int)vaccineObs.ValCodeSystem)+","
				+    POut.Long  (vaccineObs.VaccineObsNumGroup)+","
				+"'"+POut.String(vaccineObs.UcumCode)+"',"
				+    POut.Date  (vaccineObs.DateObs)+","
				+"'"+POut.String(vaccineObs.MethodCode)+"')";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				vaccineObs.VaccineObsNum=Db.NonQ(command,true,"VaccineObsNum","vaccineObs");
			}
			return vaccineObs.VaccineObsNum;
		}

		///<summary>Inserts one VaccineObs into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(VaccineObs vaccineObs) {
			return InsertNoCache(vaccineObs,false);
		}

		///<summary>Inserts one VaccineObs into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(VaccineObs vaccineObs,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO vaccineobs (";
			if(!useExistingPK && isRandomKeys) {
				vaccineObs.VaccineObsNum=ReplicationServers.GetKeyNoCache("vaccineobs","VaccineObsNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="VaccineObsNum,";
			}
			command+="VaccinePatNum,ValType,IdentifyingCode,ValReported,ValCodeSystem,VaccineObsNumGroup,UcumCode,DateObs,MethodCode) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(vaccineObs.VaccineObsNum)+",";
			}
			command+=
				     POut.Long  (vaccineObs.VaccinePatNum)+","
				+    POut.Int   ((int)vaccineObs.ValType)+","
				+    POut.Int   ((int)vaccineObs.IdentifyingCode)+","
				+"'"+POut.String(vaccineObs.ValReported)+"',"
				+    POut.Int   ((int)vaccineObs.ValCodeSystem)+","
				+    POut.Long  (vaccineObs.VaccineObsNumGroup)+","
				+"'"+POut.String(vaccineObs.UcumCode)+"',"
				+    POut.Date  (vaccineObs.DateObs)+","
				+"'"+POut.String(vaccineObs.MethodCode)+"')";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				vaccineObs.VaccineObsNum=Db.NonQ(command,true,"VaccineObsNum","vaccineObs");
			}
			return vaccineObs.VaccineObsNum;
		}

		///<summary>Updates one VaccineObs in the database.</summary>
		public static void Update(VaccineObs vaccineObs) {
			string command="UPDATE vaccineobs SET "
				+"VaccinePatNum     =  "+POut.Long  (vaccineObs.VaccinePatNum)+", "
				+"ValType           =  "+POut.Int   ((int)vaccineObs.ValType)+", "
				+"IdentifyingCode   =  "+POut.Int   ((int)vaccineObs.IdentifyingCode)+", "
				+"ValReported       = '"+POut.String(vaccineObs.ValReported)+"', "
				+"ValCodeSystem     =  "+POut.Int   ((int)vaccineObs.ValCodeSystem)+", "
				+"VaccineObsNumGroup=  "+POut.Long  (vaccineObs.VaccineObsNumGroup)+", "
				+"UcumCode          = '"+POut.String(vaccineObs.UcumCode)+"', "
				+"DateObs           =  "+POut.Date  (vaccineObs.DateObs)+", "
				+"MethodCode        = '"+POut.String(vaccineObs.MethodCode)+"' "
				+"WHERE VaccineObsNum = "+POut.Long(vaccineObs.VaccineObsNum);
			Db.NonQ(command);
		}

		///<summary>Updates one VaccineObs in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(VaccineObs vaccineObs,VaccineObs oldVaccineObs) {
			string command="";
			if(vaccineObs.VaccinePatNum != oldVaccineObs.VaccinePatNum) {
				if(command!="") { command+=",";}
				command+="VaccinePatNum = "+POut.Long(vaccineObs.VaccinePatNum)+"";
			}
			if(vaccineObs.ValType != oldVaccineObs.ValType) {
				if(command!="") { command+=",";}
				command+="ValType = "+POut.Int   ((int)vaccineObs.ValType)+"";
			}
			if(vaccineObs.IdentifyingCode != oldVaccineObs.IdentifyingCode) {
				if(command!="") { command+=",";}
				command+="IdentifyingCode = "+POut.Int   ((int)vaccineObs.IdentifyingCode)+"";
			}
			if(vaccineObs.ValReported != oldVaccineObs.ValReported) {
				if(command!="") { command+=",";}
				command+="ValReported = '"+POut.String(vaccineObs.ValReported)+"'";
			}
			if(vaccineObs.ValCodeSystem != oldVaccineObs.ValCodeSystem) {
				if(command!="") { command+=",";}
				command+="ValCodeSystem = "+POut.Int   ((int)vaccineObs.ValCodeSystem)+"";
			}
			if(vaccineObs.VaccineObsNumGroup != oldVaccineObs.VaccineObsNumGroup) {
				if(command!="") { command+=",";}
				command+="VaccineObsNumGroup = "+POut.Long(vaccineObs.VaccineObsNumGroup)+"";
			}
			if(vaccineObs.UcumCode != oldVaccineObs.UcumCode) {
				if(command!="") { command+=",";}
				command+="UcumCode = '"+POut.String(vaccineObs.UcumCode)+"'";
			}
			if(vaccineObs.DateObs.Date != oldVaccineObs.DateObs.Date) {
				if(command!="") { command+=",";}
				command+="DateObs = "+POut.Date(vaccineObs.DateObs)+"";
			}
			if(vaccineObs.MethodCode != oldVaccineObs.MethodCode) {
				if(command!="") { command+=",";}
				command+="MethodCode = '"+POut.String(vaccineObs.MethodCode)+"'";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE vaccineobs SET "+command
				+" WHERE VaccineObsNum = "+POut.Long(vaccineObs.VaccineObsNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(VaccineObs,VaccineObs) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(VaccineObs vaccineObs,VaccineObs oldVaccineObs) {
			if(vaccineObs.VaccinePatNum != oldVaccineObs.VaccinePatNum) {
				return true;
			}
			if(vaccineObs.ValType != oldVaccineObs.ValType) {
				return true;
			}
			if(vaccineObs.IdentifyingCode != oldVaccineObs.IdentifyingCode) {
				return true;
			}
			if(vaccineObs.ValReported != oldVaccineObs.ValReported) {
				return true;
			}
			if(vaccineObs.ValCodeSystem != oldVaccineObs.ValCodeSystem) {
				return true;
			}
			if(vaccineObs.VaccineObsNumGroup != oldVaccineObs.VaccineObsNumGroup) {
				return true;
			}
			if(vaccineObs.UcumCode != oldVaccineObs.UcumCode) {
				return true;
			}
			if(vaccineObs.DateObs.Date != oldVaccineObs.DateObs.Date) {
				return true;
			}
			if(vaccineObs.MethodCode != oldVaccineObs.MethodCode) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one VaccineObs from the database.</summary>
		public static void Delete(long vaccineObsNum) {
			string command="DELETE FROM vaccineobs "
				+"WHERE VaccineObsNum = "+POut.Long(vaccineObsNum);
			Db.NonQ(command);
		}

	}
}