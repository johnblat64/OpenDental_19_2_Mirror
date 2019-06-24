//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class PatientRaceCrud {
		///<summary>Gets one PatientRace object from the database using the primary key.  Returns null if not found.</summary>
		public static PatientRace SelectOne(long patientRaceNum) {
			string command="SELECT * FROM patientrace "
				+"WHERE PatientRaceNum = "+POut.Long(patientRaceNum);
			List<PatientRace> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one PatientRace object from the database using a query.</summary>
		public static PatientRace SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PatientRace> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of PatientRace objects from the database using a query.</summary>
		public static List<PatientRace> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PatientRace> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<PatientRace> TableToList(DataTable table) {
			List<PatientRace> retVal=new List<PatientRace>();
			PatientRace patientRace;
			foreach(DataRow row in table.Rows) {
				patientRace=new PatientRace();
				patientRace.PatientRaceNum= PIn.Long  (row["PatientRaceNum"].ToString());
				patientRace.PatNum        = PIn.Long  (row["PatNum"].ToString());
				patientRace.Race          = (OpenDentBusiness.PatRace)PIn.Int(row["Race"].ToString());
				patientRace.CdcrecCode    = PIn.String(row["CdcrecCode"].ToString());
				retVal.Add(patientRace);
			}
			return retVal;
		}

		///<summary>Converts a list of PatientRace into a DataTable.</summary>
		public static DataTable ListToTable(List<PatientRace> listPatientRaces,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="PatientRace";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("PatientRaceNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("Race");
			table.Columns.Add("CdcrecCode");
			foreach(PatientRace patientRace in listPatientRaces) {
				table.Rows.Add(new object[] {
					POut.Long  (patientRace.PatientRaceNum),
					POut.Long  (patientRace.PatNum),
					POut.Int   ((int)patientRace.Race),
					            patientRace.CdcrecCode,
				});
			}
			return table;
		}

		///<summary>Inserts one PatientRace into the database.  Returns the new priKey.</summary>
		public static long Insert(PatientRace patientRace) {
			return Insert(patientRace,false);
		}

		///<summary>Inserts one PatientRace into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(PatientRace patientRace,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				patientRace.PatientRaceNum=ReplicationServers.GetKey("patientrace","PatientRaceNum");
			}
			string command="INSERT INTO patientrace (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="PatientRaceNum,";
			}
			command+="PatNum,Race,CdcrecCode) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(patientRace.PatientRaceNum)+",";
			}
			command+=
				     POut.Long  (patientRace.PatNum)+","
				+    POut.Int   ((int)patientRace.Race)+","
				+"'"+POut.String(patientRace.CdcrecCode)+"')";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				patientRace.PatientRaceNum=Db.NonQ(command,true,"PatientRaceNum","patientRace");
			}
			return patientRace.PatientRaceNum;
		}

		///<summary>Inserts one PatientRace into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PatientRace patientRace) {
			return InsertNoCache(patientRace,false);
		}

		///<summary>Inserts one PatientRace into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PatientRace patientRace,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO patientrace (";
			if(!useExistingPK && isRandomKeys) {
				patientRace.PatientRaceNum=ReplicationServers.GetKeyNoCache("patientrace","PatientRaceNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="PatientRaceNum,";
			}
			command+="PatNum,Race,CdcrecCode) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(patientRace.PatientRaceNum)+",";
			}
			command+=
				     POut.Long  (patientRace.PatNum)+","
				+    POut.Int   ((int)patientRace.Race)+","
				+"'"+POut.String(patientRace.CdcrecCode)+"')";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				patientRace.PatientRaceNum=Db.NonQ(command,true,"PatientRaceNum","patientRace");
			}
			return patientRace.PatientRaceNum;
		}

		///<summary>Updates one PatientRace in the database.</summary>
		public static void Update(PatientRace patientRace) {
			string command="UPDATE patientrace SET "
				+"PatNum        =  "+POut.Long  (patientRace.PatNum)+", "
				+"Race          =  "+POut.Int   ((int)patientRace.Race)+", "
				+"CdcrecCode    = '"+POut.String(patientRace.CdcrecCode)+"' "
				+"WHERE PatientRaceNum = "+POut.Long(patientRace.PatientRaceNum);
			Db.NonQ(command);
		}

		///<summary>Updates one PatientRace in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(PatientRace patientRace,PatientRace oldPatientRace) {
			string command="";
			if(patientRace.PatNum != oldPatientRace.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(patientRace.PatNum)+"";
			}
			if(patientRace.Race != oldPatientRace.Race) {
				if(command!="") { command+=",";}
				command+="Race = "+POut.Int   ((int)patientRace.Race)+"";
			}
			if(patientRace.CdcrecCode != oldPatientRace.CdcrecCode) {
				if(command!="") { command+=",";}
				command+="CdcrecCode = '"+POut.String(patientRace.CdcrecCode)+"'";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE patientrace SET "+command
				+" WHERE PatientRaceNum = "+POut.Long(patientRace.PatientRaceNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(PatientRace,PatientRace) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(PatientRace patientRace,PatientRace oldPatientRace) {
			if(patientRace.PatNum != oldPatientRace.PatNum) {
				return true;
			}
			if(patientRace.Race != oldPatientRace.Race) {
				return true;
			}
			if(patientRace.CdcrecCode != oldPatientRace.CdcrecCode) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one PatientRace from the database.</summary>
		public static void Delete(long patientRaceNum) {
			string command="DELETE FROM patientrace "
				+"WHERE PatientRaceNum = "+POut.Long(patientRaceNum);
			Db.NonQ(command);
		}

	}
}