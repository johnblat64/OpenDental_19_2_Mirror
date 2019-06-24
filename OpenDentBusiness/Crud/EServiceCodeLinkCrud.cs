//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class EServiceCodeLinkCrud {
		///<summary>Gets one EServiceCodeLink object from the database using the primary key.  Returns null if not found.</summary>
		public static EServiceCodeLink SelectOne(long eServiceCodeLinkNum) {
			string command="SELECT * FROM eservicecodelink "
				+"WHERE EServiceCodeLinkNum = "+POut.Long(eServiceCodeLinkNum);
			List<EServiceCodeLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one EServiceCodeLink object from the database using a query.</summary>
		public static EServiceCodeLink SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EServiceCodeLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of EServiceCodeLink objects from the database using a query.</summary>
		public static List<EServiceCodeLink> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EServiceCodeLink> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<EServiceCodeLink> TableToList(DataTable table) {
			List<EServiceCodeLink> retVal=new List<EServiceCodeLink>();
			EServiceCodeLink eServiceCodeLink;
			foreach(DataRow row in table.Rows) {
				eServiceCodeLink=new EServiceCodeLink();
				eServiceCodeLink.EServiceCodeLinkNum= PIn.Long  (row["EServiceCodeLinkNum"].ToString());
				eServiceCodeLink.CodeNum            = PIn.Long  (row["CodeNum"].ToString());
				eServiceCodeLink.EService           = (OpenDentBusiness.eServiceCode)PIn.Int(row["EService"].ToString());
				retVal.Add(eServiceCodeLink);
			}
			return retVal;
		}

		///<summary>Converts a list of EServiceCodeLink into a DataTable.</summary>
		public static DataTable ListToTable(List<EServiceCodeLink> listEServiceCodeLinks,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="EServiceCodeLink";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("EServiceCodeLinkNum");
			table.Columns.Add("CodeNum");
			table.Columns.Add("EService");
			foreach(EServiceCodeLink eServiceCodeLink in listEServiceCodeLinks) {
				table.Rows.Add(new object[] {
					POut.Long  (eServiceCodeLink.EServiceCodeLinkNum),
					POut.Long  (eServiceCodeLink.CodeNum),
					POut.Int   ((int)eServiceCodeLink.EService),
				});
			}
			return table;
		}

		///<summary>Inserts one EServiceCodeLink into the database.  Returns the new priKey.</summary>
		public static long Insert(EServiceCodeLink eServiceCodeLink) {
			return Insert(eServiceCodeLink,false);
		}

		///<summary>Inserts one EServiceCodeLink into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(EServiceCodeLink eServiceCodeLink,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				eServiceCodeLink.EServiceCodeLinkNum=ReplicationServers.GetKey("eservicecodelink","EServiceCodeLinkNum");
			}
			string command="INSERT INTO eservicecodelink (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="EServiceCodeLinkNum,";
			}
			command+="CodeNum,EService) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(eServiceCodeLink.EServiceCodeLinkNum)+",";
			}
			command+=
				     POut.Long  (eServiceCodeLink.CodeNum)+","
				+    POut.Int   ((int)eServiceCodeLink.EService)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				eServiceCodeLink.EServiceCodeLinkNum=Db.NonQ(command,true,"EServiceCodeLinkNum","eServiceCodeLink");
			}
			return eServiceCodeLink.EServiceCodeLinkNum;
		}

		///<summary>Inserts one EServiceCodeLink into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EServiceCodeLink eServiceCodeLink) {
			return InsertNoCache(eServiceCodeLink,false);
		}

		///<summary>Inserts one EServiceCodeLink into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EServiceCodeLink eServiceCodeLink,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO eservicecodelink (";
			if(!useExistingPK && isRandomKeys) {
				eServiceCodeLink.EServiceCodeLinkNum=ReplicationServers.GetKeyNoCache("eservicecodelink","EServiceCodeLinkNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="EServiceCodeLinkNum,";
			}
			command+="CodeNum,EService) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(eServiceCodeLink.EServiceCodeLinkNum)+",";
			}
			command+=
				     POut.Long  (eServiceCodeLink.CodeNum)+","
				+    POut.Int   ((int)eServiceCodeLink.EService)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				eServiceCodeLink.EServiceCodeLinkNum=Db.NonQ(command,true,"EServiceCodeLinkNum","eServiceCodeLink");
			}
			return eServiceCodeLink.EServiceCodeLinkNum;
		}

		///<summary>Updates one EServiceCodeLink in the database.</summary>
		public static void Update(EServiceCodeLink eServiceCodeLink) {
			string command="UPDATE eservicecodelink SET "
				+"CodeNum            =  "+POut.Long  (eServiceCodeLink.CodeNum)+", "
				+"EService           =  "+POut.Int   ((int)eServiceCodeLink.EService)+" "
				+"WHERE EServiceCodeLinkNum = "+POut.Long(eServiceCodeLink.EServiceCodeLinkNum);
			Db.NonQ(command);
		}

		///<summary>Updates one EServiceCodeLink in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(EServiceCodeLink eServiceCodeLink,EServiceCodeLink oldEServiceCodeLink) {
			string command="";
			if(eServiceCodeLink.CodeNum != oldEServiceCodeLink.CodeNum) {
				if(command!="") { command+=",";}
				command+="CodeNum = "+POut.Long(eServiceCodeLink.CodeNum)+"";
			}
			if(eServiceCodeLink.EService != oldEServiceCodeLink.EService) {
				if(command!="") { command+=",";}
				command+="EService = "+POut.Int   ((int)eServiceCodeLink.EService)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE eservicecodelink SET "+command
				+" WHERE EServiceCodeLinkNum = "+POut.Long(eServiceCodeLink.EServiceCodeLinkNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(EServiceCodeLink,EServiceCodeLink) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(EServiceCodeLink eServiceCodeLink,EServiceCodeLink oldEServiceCodeLink) {
			if(eServiceCodeLink.CodeNum != oldEServiceCodeLink.CodeNum) {
				return true;
			}
			if(eServiceCodeLink.EService != oldEServiceCodeLink.EService) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one EServiceCodeLink from the database.</summary>
		public static void Delete(long eServiceCodeLinkNum) {
			string command="DELETE FROM eservicecodelink "
				+"WHERE EServiceCodeLinkNum = "+POut.Long(eServiceCodeLinkNum);
			Db.NonQ(command);
		}

	}
}