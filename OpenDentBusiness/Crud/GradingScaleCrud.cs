//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class GradingScaleCrud {
		///<summary>Gets one GradingScale object from the database using the primary key.  Returns null if not found.</summary>
		public static GradingScale SelectOne(long gradingScaleNum) {
			string command="SELECT * FROM gradingscale "
				+"WHERE GradingScaleNum = "+POut.Long(gradingScaleNum);
			List<GradingScale> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one GradingScale object from the database using a query.</summary>
		public static GradingScale SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<GradingScale> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of GradingScale objects from the database using a query.</summary>
		public static List<GradingScale> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<GradingScale> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<GradingScale> TableToList(DataTable table) {
			List<GradingScale> retVal=new List<GradingScale>();
			GradingScale gradingScale;
			foreach(DataRow row in table.Rows) {
				gradingScale=new GradingScale();
				gradingScale.GradingScaleNum= PIn.Long  (row["GradingScaleNum"].ToString());
				gradingScale.ScaleType      = (OpenDentBusiness.EnumScaleType)PIn.Int(row["ScaleType"].ToString());
				gradingScale.Description    = PIn.String(row["Description"].ToString());
				retVal.Add(gradingScale);
			}
			return retVal;
		}

		///<summary>Converts a list of GradingScale into a DataTable.</summary>
		public static DataTable ListToTable(List<GradingScale> listGradingScales,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="GradingScale";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("GradingScaleNum");
			table.Columns.Add("ScaleType");
			table.Columns.Add("Description");
			foreach(GradingScale gradingScale in listGradingScales) {
				table.Rows.Add(new object[] {
					POut.Long  (gradingScale.GradingScaleNum),
					POut.Int   ((int)gradingScale.ScaleType),
					            gradingScale.Description,
				});
			}
			return table;
		}

		///<summary>Inserts one GradingScale into the database.  Returns the new priKey.</summary>
		public static long Insert(GradingScale gradingScale) {
			return Insert(gradingScale,false);
		}

		///<summary>Inserts one GradingScale into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(GradingScale gradingScale,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				gradingScale.GradingScaleNum=ReplicationServers.GetKey("gradingscale","GradingScaleNum");
			}
			string command="INSERT INTO gradingscale (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="GradingScaleNum,";
			}
			command+="ScaleType,Description) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(gradingScale.GradingScaleNum)+",";
			}
			command+=
				     POut.Int   ((int)gradingScale.ScaleType)+","
				+"'"+POut.String(gradingScale.Description)+"')";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				gradingScale.GradingScaleNum=Db.NonQ(command,true,"GradingScaleNum","gradingScale");
			}
			return gradingScale.GradingScaleNum;
		}

		///<summary>Inserts one GradingScale into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(GradingScale gradingScale) {
			return InsertNoCache(gradingScale,false);
		}

		///<summary>Inserts one GradingScale into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(GradingScale gradingScale,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO gradingscale (";
			if(!useExistingPK && isRandomKeys) {
				gradingScale.GradingScaleNum=ReplicationServers.GetKeyNoCache("gradingscale","GradingScaleNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="GradingScaleNum,";
			}
			command+="ScaleType,Description) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(gradingScale.GradingScaleNum)+",";
			}
			command+=
				     POut.Int   ((int)gradingScale.ScaleType)+","
				+"'"+POut.String(gradingScale.Description)+"')";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				gradingScale.GradingScaleNum=Db.NonQ(command,true,"GradingScaleNum","gradingScale");
			}
			return gradingScale.GradingScaleNum;
		}

		///<summary>Updates one GradingScale in the database.</summary>
		public static void Update(GradingScale gradingScale) {
			string command="UPDATE gradingscale SET "
				+"ScaleType      =  "+POut.Int   ((int)gradingScale.ScaleType)+", "
				+"Description    = '"+POut.String(gradingScale.Description)+"' "
				+"WHERE GradingScaleNum = "+POut.Long(gradingScale.GradingScaleNum);
			Db.NonQ(command);
		}

		///<summary>Updates one GradingScale in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(GradingScale gradingScale,GradingScale oldGradingScale) {
			string command="";
			if(gradingScale.ScaleType != oldGradingScale.ScaleType) {
				if(command!="") { command+=",";}
				command+="ScaleType = "+POut.Int   ((int)gradingScale.ScaleType)+"";
			}
			if(gradingScale.Description != oldGradingScale.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(gradingScale.Description)+"'";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE gradingscale SET "+command
				+" WHERE GradingScaleNum = "+POut.Long(gradingScale.GradingScaleNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(GradingScale,GradingScale) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(GradingScale gradingScale,GradingScale oldGradingScale) {
			if(gradingScale.ScaleType != oldGradingScale.ScaleType) {
				return true;
			}
			if(gradingScale.Description != oldGradingScale.Description) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one GradingScale from the database.</summary>
		public static void Delete(long gradingScaleNum) {
			string command="DELETE FROM gradingscale "
				+"WHERE GradingScaleNum = "+POut.Long(gradingScaleNum);
			Db.NonQ(command);
		}

	}
}