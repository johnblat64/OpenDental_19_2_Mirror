//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class AppointmentRuleCrud {
		///<summary>Gets one AppointmentRule object from the database using the primary key.  Returns null if not found.</summary>
		public static AppointmentRule SelectOne(long appointmentRuleNum) {
			string command="SELECT * FROM appointmentrule "
				+"WHERE AppointmentRuleNum = "+POut.Long(appointmentRuleNum);
			List<AppointmentRule> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one AppointmentRule object from the database using a query.</summary>
		public static AppointmentRule SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AppointmentRule> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of AppointmentRule objects from the database using a query.</summary>
		public static List<AppointmentRule> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AppointmentRule> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<AppointmentRule> TableToList(DataTable table) {
			List<AppointmentRule> retVal=new List<AppointmentRule>();
			AppointmentRule appointmentRule;
			foreach(DataRow row in table.Rows) {
				appointmentRule=new AppointmentRule();
				appointmentRule.AppointmentRuleNum= PIn.Long  (row["AppointmentRuleNum"].ToString());
				appointmentRule.RuleDesc          = PIn.String(row["RuleDesc"].ToString());
				appointmentRule.CodeStart         = PIn.String(row["CodeStart"].ToString());
				appointmentRule.CodeEnd           = PIn.String(row["CodeEnd"].ToString());
				appointmentRule.IsEnabled         = PIn.Bool  (row["IsEnabled"].ToString());
				retVal.Add(appointmentRule);
			}
			return retVal;
		}

		///<summary>Converts a list of AppointmentRule into a DataTable.</summary>
		public static DataTable ListToTable(List<AppointmentRule> listAppointmentRules,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="AppointmentRule";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("AppointmentRuleNum");
			table.Columns.Add("RuleDesc");
			table.Columns.Add("CodeStart");
			table.Columns.Add("CodeEnd");
			table.Columns.Add("IsEnabled");
			foreach(AppointmentRule appointmentRule in listAppointmentRules) {
				table.Rows.Add(new object[] {
					POut.Long  (appointmentRule.AppointmentRuleNum),
					            appointmentRule.RuleDesc,
					            appointmentRule.CodeStart,
					            appointmentRule.CodeEnd,
					POut.Bool  (appointmentRule.IsEnabled),
				});
			}
			return table;
		}

		///<summary>Inserts one AppointmentRule into the database.  Returns the new priKey.</summary>
		public static long Insert(AppointmentRule appointmentRule) {
			return Insert(appointmentRule,false);
		}

		///<summary>Inserts one AppointmentRule into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(AppointmentRule appointmentRule,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				appointmentRule.AppointmentRuleNum=ReplicationServers.GetKey("appointmentrule","AppointmentRuleNum");
			}
			string command="INSERT INTO appointmentrule (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="AppointmentRuleNum,";
			}
			command+="RuleDesc,CodeStart,CodeEnd,IsEnabled) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(appointmentRule.AppointmentRuleNum)+",";
			}
			command+=
				 "'"+POut.String(appointmentRule.RuleDesc)+"',"
				+"'"+POut.String(appointmentRule.CodeStart)+"',"
				+"'"+POut.String(appointmentRule.CodeEnd)+"',"
				+    POut.Bool  (appointmentRule.IsEnabled)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				appointmentRule.AppointmentRuleNum=Db.NonQ(command,true,"AppointmentRuleNum","appointmentRule");
			}
			return appointmentRule.AppointmentRuleNum;
		}

		///<summary>Inserts one AppointmentRule into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AppointmentRule appointmentRule) {
			return InsertNoCache(appointmentRule,false);
		}

		///<summary>Inserts one AppointmentRule into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AppointmentRule appointmentRule,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO appointmentrule (";
			if(!useExistingPK && isRandomKeys) {
				appointmentRule.AppointmentRuleNum=ReplicationServers.GetKeyNoCache("appointmentrule","AppointmentRuleNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="AppointmentRuleNum,";
			}
			command+="RuleDesc,CodeStart,CodeEnd,IsEnabled) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(appointmentRule.AppointmentRuleNum)+",";
			}
			command+=
				 "'"+POut.String(appointmentRule.RuleDesc)+"',"
				+"'"+POut.String(appointmentRule.CodeStart)+"',"
				+"'"+POut.String(appointmentRule.CodeEnd)+"',"
				+    POut.Bool  (appointmentRule.IsEnabled)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				appointmentRule.AppointmentRuleNum=Db.NonQ(command,true,"AppointmentRuleNum","appointmentRule");
			}
			return appointmentRule.AppointmentRuleNum;
		}

		///<summary>Updates one AppointmentRule in the database.</summary>
		public static void Update(AppointmentRule appointmentRule) {
			string command="UPDATE appointmentrule SET "
				+"RuleDesc          = '"+POut.String(appointmentRule.RuleDesc)+"', "
				+"CodeStart         = '"+POut.String(appointmentRule.CodeStart)+"', "
				+"CodeEnd           = '"+POut.String(appointmentRule.CodeEnd)+"', "
				+"IsEnabled         =  "+POut.Bool  (appointmentRule.IsEnabled)+" "
				+"WHERE AppointmentRuleNum = "+POut.Long(appointmentRule.AppointmentRuleNum);
			Db.NonQ(command);
		}

		///<summary>Updates one AppointmentRule in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(AppointmentRule appointmentRule,AppointmentRule oldAppointmentRule) {
			string command="";
			if(appointmentRule.RuleDesc != oldAppointmentRule.RuleDesc) {
				if(command!="") { command+=",";}
				command+="RuleDesc = '"+POut.String(appointmentRule.RuleDesc)+"'";
			}
			if(appointmentRule.CodeStart != oldAppointmentRule.CodeStart) {
				if(command!="") { command+=",";}
				command+="CodeStart = '"+POut.String(appointmentRule.CodeStart)+"'";
			}
			if(appointmentRule.CodeEnd != oldAppointmentRule.CodeEnd) {
				if(command!="") { command+=",";}
				command+="CodeEnd = '"+POut.String(appointmentRule.CodeEnd)+"'";
			}
			if(appointmentRule.IsEnabled != oldAppointmentRule.IsEnabled) {
				if(command!="") { command+=",";}
				command+="IsEnabled = "+POut.Bool(appointmentRule.IsEnabled)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE appointmentrule SET "+command
				+" WHERE AppointmentRuleNum = "+POut.Long(appointmentRule.AppointmentRuleNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(AppointmentRule,AppointmentRule) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(AppointmentRule appointmentRule,AppointmentRule oldAppointmentRule) {
			if(appointmentRule.RuleDesc != oldAppointmentRule.RuleDesc) {
				return true;
			}
			if(appointmentRule.CodeStart != oldAppointmentRule.CodeStart) {
				return true;
			}
			if(appointmentRule.CodeEnd != oldAppointmentRule.CodeEnd) {
				return true;
			}
			if(appointmentRule.IsEnabled != oldAppointmentRule.IsEnabled) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one AppointmentRule from the database.</summary>
		public static void Delete(long appointmentRuleNum) {
			string command="DELETE FROM appointmentrule "
				+"WHERE AppointmentRuleNum = "+POut.Long(appointmentRuleNum);
			Db.NonQ(command);
		}

	}
}