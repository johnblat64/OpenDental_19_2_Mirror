//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class InsVerifyHistCrud {
		///<summary>Gets one InsVerifyHist object from the database using the primary key.  Returns null if not found.</summary>
		public static InsVerifyHist SelectOne(long insVerifyHistNum) {
			string command="SELECT * FROM insverifyhist "
				+"WHERE InsVerifyHistNum = "+POut.Long(insVerifyHistNum);
			List<InsVerifyHist> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one InsVerifyHist object from the database using a query.</summary>
		public static InsVerifyHist SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<InsVerifyHist> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of InsVerifyHist objects from the database using a query.</summary>
		public static List<InsVerifyHist> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<InsVerifyHist> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<InsVerifyHist> TableToList(DataTable table) {
			List<InsVerifyHist> retVal=new List<InsVerifyHist>();
			InsVerifyHist insVerifyHist;
			foreach(DataRow row in table.Rows) {
				insVerifyHist=new InsVerifyHist();
				insVerifyHist.InsVerifyHistNum             = PIn.Long  (row["InsVerifyHistNum"].ToString());
				insVerifyHist.VerifyUserNum                = PIn.Long  (row["VerifyUserNum"].ToString());
				insVerifyHist.InsVerifyNum                 = PIn.Long  (row["InsVerifyNum"].ToString());
				insVerifyHist.DateLastVerified             = PIn.Date  (row["DateLastVerified"].ToString());
				insVerifyHist.UserNum                      = PIn.Long  (row["UserNum"].ToString());
				insVerifyHist.VerifyType                   = (OpenDentBusiness.VerifyTypes)PIn.Int(row["VerifyType"].ToString());
				insVerifyHist.FKey                         = PIn.Long  (row["FKey"].ToString());
				insVerifyHist.DefNum                       = PIn.Long  (row["DefNum"].ToString());
				insVerifyHist.DateLastAssigned             = PIn.Date  (row["DateLastAssigned"].ToString());
				insVerifyHist.Note                         = PIn.String(row["Note"].ToString());
				insVerifyHist.DateTimeEntry                = PIn.DateT (row["DateTimeEntry"].ToString());
				insVerifyHist.HoursAvailableForVerification= PIn.Double(row["HoursAvailableForVerification"].ToString());
				insVerifyHist.SecDateTEdit                 = PIn.DateT (row["SecDateTEdit"].ToString());
				retVal.Add(insVerifyHist);
			}
			return retVal;
		}

		///<summary>Converts a list of InsVerifyHist into a DataTable.</summary>
		public static DataTable ListToTable(List<InsVerifyHist> listInsVerifyHists,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="InsVerifyHist";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("InsVerifyHistNum");
			table.Columns.Add("VerifyUserNum");
			table.Columns.Add("InsVerifyNum");
			table.Columns.Add("DateLastVerified");
			table.Columns.Add("UserNum");
			table.Columns.Add("VerifyType");
			table.Columns.Add("FKey");
			table.Columns.Add("DefNum");
			table.Columns.Add("DateLastAssigned");
			table.Columns.Add("Note");
			table.Columns.Add("DateTimeEntry");
			table.Columns.Add("HoursAvailableForVerification");
			table.Columns.Add("SecDateTEdit");
			foreach(InsVerifyHist insVerifyHist in listInsVerifyHists) {
				table.Rows.Add(new object[] {
					POut.Long  (insVerifyHist.InsVerifyHistNum),
					POut.Long  (insVerifyHist.VerifyUserNum),
					POut.Long  (insVerifyHist.InsVerifyNum),
					POut.DateT (insVerifyHist.DateLastVerified,false),
					POut.Long  (insVerifyHist.UserNum),
					POut.Int   ((int)insVerifyHist.VerifyType),
					POut.Long  (insVerifyHist.FKey),
					POut.Long  (insVerifyHist.DefNum),
					POut.DateT (insVerifyHist.DateLastAssigned,false),
					            insVerifyHist.Note,
					POut.DateT (insVerifyHist.DateTimeEntry,false),
					POut.Double(insVerifyHist.HoursAvailableForVerification),
					POut.DateT (insVerifyHist.SecDateTEdit,false),
				});
			}
			return table;
		}

		///<summary>Inserts one InsVerifyHist into the database.  Returns the new priKey.</summary>
		public static long Insert(InsVerifyHist insVerifyHist) {
			return Insert(insVerifyHist,false);
		}

		///<summary>Inserts one InsVerifyHist into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(InsVerifyHist insVerifyHist,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				insVerifyHist.InsVerifyHistNum=ReplicationServers.GetKey("insverifyhist","InsVerifyHistNum");
			}
			string command="INSERT INTO insverifyhist (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="InsVerifyHistNum,";
			}
			command+="VerifyUserNum,InsVerifyNum,DateLastVerified,UserNum,VerifyType,FKey,DefNum,DateLastAssigned,Note,DateTimeEntry,HoursAvailableForVerification,SecDateTEdit) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(insVerifyHist.InsVerifyHistNum)+",";
			}
			command+=
				     POut.Long  (insVerifyHist.VerifyUserNum)+","
				+    POut.Long  (insVerifyHist.InsVerifyNum)+","
				+    POut.Date  (insVerifyHist.DateLastVerified)+","
				+    POut.Long  (insVerifyHist.UserNum)+","
				+    POut.Int   ((int)insVerifyHist.VerifyType)+","
				+    POut.Long  (insVerifyHist.FKey)+","
				+    POut.Long  (insVerifyHist.DefNum)+","
				+    POut.Date  (insVerifyHist.DateLastAssigned)+","
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.DateT (insVerifyHist.DateTimeEntry)+","
				+"'"+POut.Double(insVerifyHist.HoursAvailableForVerification)+"',"
				+    POut.DateT (insVerifyHist.SecDateTEdit)+")";
			if(insVerifyHist.Note==null) {
				insVerifyHist.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringNote(insVerifyHist.Note));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramNote);
			}
			else {
				insVerifyHist.InsVerifyHistNum=Db.NonQ(command,true,"InsVerifyHistNum","insVerifyHist",paramNote);
			}
			return insVerifyHist.InsVerifyHistNum;
		}

		///<summary>Inserts one InsVerifyHist into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(InsVerifyHist insVerifyHist) {
			return InsertNoCache(insVerifyHist,false);
		}

		///<summary>Inserts one InsVerifyHist into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(InsVerifyHist insVerifyHist,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO insverifyhist (";
			if(!useExistingPK && isRandomKeys) {
				insVerifyHist.InsVerifyHistNum=ReplicationServers.GetKeyNoCache("insverifyhist","InsVerifyHistNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="InsVerifyHistNum,";
			}
			command+="VerifyUserNum,InsVerifyNum,DateLastVerified,UserNum,VerifyType,FKey,DefNum,DateLastAssigned,Note,DateTimeEntry,HoursAvailableForVerification,SecDateTEdit) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(insVerifyHist.InsVerifyHistNum)+",";
			}
			command+=
				     POut.Long  (insVerifyHist.VerifyUserNum)+","
				+    POut.Long  (insVerifyHist.InsVerifyNum)+","
				+    POut.Date  (insVerifyHist.DateLastVerified)+","
				+    POut.Long  (insVerifyHist.UserNum)+","
				+    POut.Int   ((int)insVerifyHist.VerifyType)+","
				+    POut.Long  (insVerifyHist.FKey)+","
				+    POut.Long  (insVerifyHist.DefNum)+","
				+    POut.Date  (insVerifyHist.DateLastAssigned)+","
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.DateT (insVerifyHist.DateTimeEntry)+","
				+"'"+POut.Double(insVerifyHist.HoursAvailableForVerification)+"',"
				+    POut.DateT (insVerifyHist.SecDateTEdit)+")";
			if(insVerifyHist.Note==null) {
				insVerifyHist.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringNote(insVerifyHist.Note));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramNote);
			}
			else {
				insVerifyHist.InsVerifyHistNum=Db.NonQ(command,true,"InsVerifyHistNum","insVerifyHist",paramNote);
			}
			return insVerifyHist.InsVerifyHistNum;
		}

		///<summary>Deletes one InsVerifyHist from the database.</summary>
		public static void Delete(long insVerifyHistNum) {
			string command="DELETE FROM insverifyhist "
				+"WHERE InsVerifyHistNum = "+POut.Long(insVerifyHistNum);
			Db.NonQ(command);
		}

	}
}