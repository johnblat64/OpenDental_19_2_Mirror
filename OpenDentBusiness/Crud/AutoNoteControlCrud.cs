//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class AutoNoteControlCrud {
		///<summary>Gets one AutoNoteControl object from the database using the primary key.  Returns null if not found.</summary>
		public static AutoNoteControl SelectOne(long autoNoteControlNum) {
			string command="SELECT * FROM autonotecontrol "
				+"WHERE AutoNoteControlNum = "+POut.Long(autoNoteControlNum);
			List<AutoNoteControl> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one AutoNoteControl object from the database using a query.</summary>
		public static AutoNoteControl SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AutoNoteControl> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of AutoNoteControl objects from the database using a query.</summary>
		public static List<AutoNoteControl> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AutoNoteControl> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<AutoNoteControl> TableToList(DataTable table) {
			List<AutoNoteControl> retVal=new List<AutoNoteControl>();
			AutoNoteControl autoNoteControl;
			foreach(DataRow row in table.Rows) {
				autoNoteControl=new AutoNoteControl();
				autoNoteControl.AutoNoteControlNum= PIn.Long  (row["AutoNoteControlNum"].ToString());
				autoNoteControl.Descript          = PIn.String(row["Descript"].ToString());
				autoNoteControl.ControlType       = PIn.String(row["ControlType"].ToString());
				autoNoteControl.ControlLabel      = PIn.String(row["ControlLabel"].ToString());
				autoNoteControl.ControlOptions    = PIn.String(row["ControlOptions"].ToString());
				retVal.Add(autoNoteControl);
			}
			return retVal;
		}

		///<summary>Converts a list of AutoNoteControl into a DataTable.</summary>
		public static DataTable ListToTable(List<AutoNoteControl> listAutoNoteControls,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="AutoNoteControl";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("AutoNoteControlNum");
			table.Columns.Add("Descript");
			table.Columns.Add("ControlType");
			table.Columns.Add("ControlLabel");
			table.Columns.Add("ControlOptions");
			foreach(AutoNoteControl autoNoteControl in listAutoNoteControls) {
				table.Rows.Add(new object[] {
					POut.Long  (autoNoteControl.AutoNoteControlNum),
					            autoNoteControl.Descript,
					            autoNoteControl.ControlType,
					            autoNoteControl.ControlLabel,
					            autoNoteControl.ControlOptions,
				});
			}
			return table;
		}

		///<summary>Inserts one AutoNoteControl into the database.  Returns the new priKey.</summary>
		public static long Insert(AutoNoteControl autoNoteControl) {
			return Insert(autoNoteControl,false);
		}

		///<summary>Inserts one AutoNoteControl into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(AutoNoteControl autoNoteControl,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				autoNoteControl.AutoNoteControlNum=ReplicationServers.GetKey("autonotecontrol","AutoNoteControlNum");
			}
			string command="INSERT INTO autonotecontrol (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="AutoNoteControlNum,";
			}
			command+="Descript,ControlType,ControlLabel,ControlOptions) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(autoNoteControl.AutoNoteControlNum)+",";
			}
			command+=
				 "'"+POut.String(autoNoteControl.Descript)+"',"
				+"'"+POut.String(autoNoteControl.ControlType)+"',"
				+"'"+POut.String(autoNoteControl.ControlLabel)+"',"
				+    DbHelper.ParamChar+"paramControlOptions)";
			if(autoNoteControl.ControlOptions==null) {
				autoNoteControl.ControlOptions="";
			}
			OdSqlParameter paramControlOptions=new OdSqlParameter("paramControlOptions",OdDbType.Text,POut.StringParam(autoNoteControl.ControlOptions));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramControlOptions);
			}
			else {
				autoNoteControl.AutoNoteControlNum=Db.NonQ(command,true,"AutoNoteControlNum","autoNoteControl",paramControlOptions);
			}
			return autoNoteControl.AutoNoteControlNum;
		}

		///<summary>Inserts one AutoNoteControl into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AutoNoteControl autoNoteControl) {
			return InsertNoCache(autoNoteControl,false);
		}

		///<summary>Inserts one AutoNoteControl into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AutoNoteControl autoNoteControl,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO autonotecontrol (";
			if(!useExistingPK && isRandomKeys) {
				autoNoteControl.AutoNoteControlNum=ReplicationServers.GetKeyNoCache("autonotecontrol","AutoNoteControlNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="AutoNoteControlNum,";
			}
			command+="Descript,ControlType,ControlLabel,ControlOptions) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(autoNoteControl.AutoNoteControlNum)+",";
			}
			command+=
				 "'"+POut.String(autoNoteControl.Descript)+"',"
				+"'"+POut.String(autoNoteControl.ControlType)+"',"
				+"'"+POut.String(autoNoteControl.ControlLabel)+"',"
				+    DbHelper.ParamChar+"paramControlOptions)";
			if(autoNoteControl.ControlOptions==null) {
				autoNoteControl.ControlOptions="";
			}
			OdSqlParameter paramControlOptions=new OdSqlParameter("paramControlOptions",OdDbType.Text,POut.StringParam(autoNoteControl.ControlOptions));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramControlOptions);
			}
			else {
				autoNoteControl.AutoNoteControlNum=Db.NonQ(command,true,"AutoNoteControlNum","autoNoteControl",paramControlOptions);
			}
			return autoNoteControl.AutoNoteControlNum;
		}

		///<summary>Updates one AutoNoteControl in the database.</summary>
		public static void Update(AutoNoteControl autoNoteControl) {
			string command="UPDATE autonotecontrol SET "
				+"Descript          = '"+POut.String(autoNoteControl.Descript)+"', "
				+"ControlType       = '"+POut.String(autoNoteControl.ControlType)+"', "
				+"ControlLabel      = '"+POut.String(autoNoteControl.ControlLabel)+"', "
				+"ControlOptions    =  "+DbHelper.ParamChar+"paramControlOptions "
				+"WHERE AutoNoteControlNum = "+POut.Long(autoNoteControl.AutoNoteControlNum);
			if(autoNoteControl.ControlOptions==null) {
				autoNoteControl.ControlOptions="";
			}
			OdSqlParameter paramControlOptions=new OdSqlParameter("paramControlOptions",OdDbType.Text,POut.StringParam(autoNoteControl.ControlOptions));
			Db.NonQ(command,paramControlOptions);
		}

		///<summary>Updates one AutoNoteControl in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(AutoNoteControl autoNoteControl,AutoNoteControl oldAutoNoteControl) {
			string command="";
			if(autoNoteControl.Descript != oldAutoNoteControl.Descript) {
				if(command!="") { command+=",";}
				command+="Descript = '"+POut.String(autoNoteControl.Descript)+"'";
			}
			if(autoNoteControl.ControlType != oldAutoNoteControl.ControlType) {
				if(command!="") { command+=",";}
				command+="ControlType = '"+POut.String(autoNoteControl.ControlType)+"'";
			}
			if(autoNoteControl.ControlLabel != oldAutoNoteControl.ControlLabel) {
				if(command!="") { command+=",";}
				command+="ControlLabel = '"+POut.String(autoNoteControl.ControlLabel)+"'";
			}
			if(autoNoteControl.ControlOptions != oldAutoNoteControl.ControlOptions) {
				if(command!="") { command+=",";}
				command+="ControlOptions = "+DbHelper.ParamChar+"paramControlOptions";
			}
			if(command=="") {
				return false;
			}
			if(autoNoteControl.ControlOptions==null) {
				autoNoteControl.ControlOptions="";
			}
			OdSqlParameter paramControlOptions=new OdSqlParameter("paramControlOptions",OdDbType.Text,POut.StringParam(autoNoteControl.ControlOptions));
			command="UPDATE autonotecontrol SET "+command
				+" WHERE AutoNoteControlNum = "+POut.Long(autoNoteControl.AutoNoteControlNum);
			Db.NonQ(command,paramControlOptions);
			return true;
		}

		///<summary>Returns true if Update(AutoNoteControl,AutoNoteControl) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(AutoNoteControl autoNoteControl,AutoNoteControl oldAutoNoteControl) {
			if(autoNoteControl.Descript != oldAutoNoteControl.Descript) {
				return true;
			}
			if(autoNoteControl.ControlType != oldAutoNoteControl.ControlType) {
				return true;
			}
			if(autoNoteControl.ControlLabel != oldAutoNoteControl.ControlLabel) {
				return true;
			}
			if(autoNoteControl.ControlOptions != oldAutoNoteControl.ControlOptions) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one AutoNoteControl from the database.</summary>
		public static void Delete(long autoNoteControlNum) {
			string command="DELETE FROM autonotecontrol "
				+"WHERE AutoNoteControlNum = "+POut.Long(autoNoteControlNum);
			Db.NonQ(command);
		}

	}
}