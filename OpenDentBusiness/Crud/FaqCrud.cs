//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class FaqCrud {
		///<summary>Gets one Faq object from the database using the primary key.  Returns null if not found.</summary>
		public static Faq SelectOne(long faqNum) {
			string command="SELECT * FROM faq "
				+"WHERE FaqNum = "+POut.Long(faqNum);
			List<Faq> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Faq object from the database using a query.</summary>
		public static Faq SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Faq> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Faq objects from the database using a query.</summary>
		public static List<Faq> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Faq> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Faq> TableToList(DataTable table) {
			List<Faq> retVal=new List<Faq>();
			Faq faq;
			foreach(DataRow row in table.Rows) {
				faq=new Faq();
				faq.FaqNum          = PIn.Long  (row["FaqNum"].ToString());
				faq.QuestionText    = PIn.String(row["QuestionText"].ToString());
				faq.AnswerText      = PIn.String(row["AnswerText"].ToString());
				faq.EmbeddedMediaUrl= PIn.String(row["EmbeddedMediaUrl"].ToString());
				faq.IsStickied      = PIn.Bool  (row["IsStickied"].ToString());
				faq.ManualVersion   = PIn.Int   (row["ManualVersion"].ToString());
				faq.ImageUrl        = PIn.String(row["ImageUrl"].ToString());
				faq.DateTEntry      = PIn.DateT (row["DateTEntry"].ToString());
				retVal.Add(faq);
			}
			return retVal;
		}

		///<summary>Converts a list of Faq into a DataTable.</summary>
		public static DataTable ListToTable(List<Faq> listFaqs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Faq";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("FaqNum");
			table.Columns.Add("QuestionText");
			table.Columns.Add("AnswerText");
			table.Columns.Add("EmbeddedMediaUrl");
			table.Columns.Add("IsStickied");
			table.Columns.Add("ManualVersion");
			table.Columns.Add("ImageUrl");
			table.Columns.Add("DateTEntry");
			foreach(Faq faq in listFaqs) {
				table.Rows.Add(new object[] {
					POut.Long  (faq.FaqNum),
					            faq.QuestionText,
					            faq.AnswerText,
					            faq.EmbeddedMediaUrl,
					POut.Bool  (faq.IsStickied),
					POut.Int   (faq.ManualVersion),
					            faq.ImageUrl,
					POut.DateT (faq.DateTEntry,false),
				});
			}
			return table;
		}

		///<summary>Inserts one Faq into the database.  Returns the new priKey.</summary>
		public static long Insert(Faq faq) {
			return Insert(faq,false);
		}

		///<summary>Inserts one Faq into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Faq faq,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				faq.FaqNum=ReplicationServers.GetKey("faq","FaqNum");
			}
			string command="INSERT INTO faq (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="FaqNum,";
			}
			command+="QuestionText,AnswerText,EmbeddedMediaUrl,IsStickied,ManualVersion,ImageUrl,DateTEntry) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(faq.FaqNum)+",";
			}
			command+=
				 "'"+POut.String(faq.QuestionText)+"',"
				+"'"+POut.String(faq.AnswerText)+"',"
				+"'"+POut.String(faq.EmbeddedMediaUrl)+"',"
				+    POut.Bool  (faq.IsStickied)+","
				+    POut.Int   (faq.ManualVersion)+","
				+"'"+POut.String(faq.ImageUrl)+"',"
				+    DbHelper.Now()+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				faq.FaqNum=Db.NonQ(command,true,"FaqNum","faq");
			}
			return faq.FaqNum;
		}

		///<summary>Inserts one Faq into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Faq faq) {
			return InsertNoCache(faq,false);
		}

		///<summary>Inserts one Faq into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Faq faq,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO faq (";
			if(!useExistingPK && isRandomKeys) {
				faq.FaqNum=ReplicationServers.GetKeyNoCache("faq","FaqNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="FaqNum,";
			}
			command+="QuestionText,AnswerText,EmbeddedMediaUrl,IsStickied,ManualVersion,ImageUrl,DateTEntry) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(faq.FaqNum)+",";
			}
			command+=
				 "'"+POut.String(faq.QuestionText)+"',"
				+"'"+POut.String(faq.AnswerText)+"',"
				+"'"+POut.String(faq.EmbeddedMediaUrl)+"',"
				+    POut.Bool  (faq.IsStickied)+","
				+    POut.Int   (faq.ManualVersion)+","
				+"'"+POut.String(faq.ImageUrl)+"',"
				+    DbHelper.Now()+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				faq.FaqNum=Db.NonQ(command,true,"FaqNum","faq");
			}
			return faq.FaqNum;
		}

		///<summary>Updates one Faq in the database.</summary>
		public static void Update(Faq faq) {
			string command="UPDATE faq SET "
				+"QuestionText    = '"+POut.String(faq.QuestionText)+"', "
				+"AnswerText      = '"+POut.String(faq.AnswerText)+"', "
				+"EmbeddedMediaUrl= '"+POut.String(faq.EmbeddedMediaUrl)+"', "
				+"IsStickied      =  "+POut.Bool  (faq.IsStickied)+", "
				+"ManualVersion   =  "+POut.Int   (faq.ManualVersion)+", "
				+"ImageUrl        = '"+POut.String(faq.ImageUrl)+"' "
				//DateTEntry not allowed to change
				+"WHERE FaqNum = "+POut.Long(faq.FaqNum);
			Db.NonQ(command);
		}

		///<summary>Updates one Faq in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Faq faq,Faq oldFaq) {
			string command="";
			if(faq.QuestionText != oldFaq.QuestionText) {
				if(command!="") { command+=",";}
				command+="QuestionText = '"+POut.String(faq.QuestionText)+"'";
			}
			if(faq.AnswerText != oldFaq.AnswerText) {
				if(command!="") { command+=",";}
				command+="AnswerText = '"+POut.String(faq.AnswerText)+"'";
			}
			if(faq.EmbeddedMediaUrl != oldFaq.EmbeddedMediaUrl) {
				if(command!="") { command+=",";}
				command+="EmbeddedMediaUrl = '"+POut.String(faq.EmbeddedMediaUrl)+"'";
			}
			if(faq.IsStickied != oldFaq.IsStickied) {
				if(command!="") { command+=",";}
				command+="IsStickied = "+POut.Bool(faq.IsStickied)+"";
			}
			if(faq.ManualVersion != oldFaq.ManualVersion) {
				if(command!="") { command+=",";}
				command+="ManualVersion = "+POut.Int(faq.ManualVersion)+"";
			}
			if(faq.ImageUrl != oldFaq.ImageUrl) {
				if(command!="") { command+=",";}
				command+="ImageUrl = '"+POut.String(faq.ImageUrl)+"'";
			}
			//DateTEntry not allowed to change
			if(command=="") {
				return false;
			}
			command="UPDATE faq SET "+command
				+" WHERE FaqNum = "+POut.Long(faq.FaqNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(Faq,Faq) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Faq faq,Faq oldFaq) {
			if(faq.QuestionText != oldFaq.QuestionText) {
				return true;
			}
			if(faq.AnswerText != oldFaq.AnswerText) {
				return true;
			}
			if(faq.EmbeddedMediaUrl != oldFaq.EmbeddedMediaUrl) {
				return true;
			}
			if(faq.IsStickied != oldFaq.IsStickied) {
				return true;
			}
			if(faq.ManualVersion != oldFaq.ManualVersion) {
				return true;
			}
			if(faq.ImageUrl != oldFaq.ImageUrl) {
				return true;
			}
			//DateTEntry not allowed to change
			return false;
		}

		///<summary>Deletes one Faq from the database.</summary>
		public static void Delete(long faqNum) {
			string command="DELETE FROM faq "
				+"WHERE FaqNum = "+POut.Long(faqNum);
			Db.NonQ(command);
		}

	}
}