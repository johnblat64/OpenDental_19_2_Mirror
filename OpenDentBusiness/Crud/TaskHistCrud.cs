//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class TaskHistCrud {
		///<summary>Gets one TaskHist object from the database using the primary key.  Returns null if not found.</summary>
		public static TaskHist SelectOne(long taskHistNum) {
			string command="SELECT * FROM taskhist "
				+"WHERE TaskHistNum = "+POut.Long(taskHistNum);
			List<TaskHist> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one TaskHist object from the database using a query.</summary>
		public static TaskHist SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<TaskHist> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of TaskHist objects from the database using a query.</summary>
		public static List<TaskHist> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<TaskHist> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<TaskHist> TableToList(DataTable table) {
			List<TaskHist> retVal=new List<TaskHist>();
			TaskHist taskHist;
			foreach(DataRow row in table.Rows) {
				taskHist=new TaskHist();
				taskHist.TaskHistNum      = PIn.Long  (row["TaskHistNum"].ToString());
				taskHist.UserNumHist      = PIn.Long  (row["UserNumHist"].ToString());
				taskHist.DateTStamp       = PIn.DateT (row["DateTStamp"].ToString());
				taskHist.IsNoteChange     = PIn.Bool  (row["IsNoteChange"].ToString());
				taskHist.TaskNum          = PIn.Long  (row["TaskNum"].ToString());
				taskHist.TaskListNum      = PIn.Long  (row["TaskListNum"].ToString());
				taskHist.DateTask         = PIn.Date  (row["DateTask"].ToString());
				taskHist.KeyNum           = PIn.Long  (row["KeyNum"].ToString());
				taskHist.Descript         = PIn.String(row["Descript"].ToString());
				taskHist.TaskStatus       = (OpenDentBusiness.TaskStatusEnum)PIn.Int(row["TaskStatus"].ToString());
				taskHist.IsRepeating      = PIn.Bool  (row["IsRepeating"].ToString());
				taskHist.DateType         = (OpenDentBusiness.TaskDateType)PIn.Int(row["DateType"].ToString());
				taskHist.FromNum          = PIn.Long  (row["FromNum"].ToString());
				taskHist.ObjectType       = (OpenDentBusiness.TaskObjectType)PIn.Int(row["ObjectType"].ToString());
				taskHist.DateTimeEntry    = PIn.DateT (row["DateTimeEntry"].ToString());
				taskHist.UserNum          = PIn.Long  (row["UserNum"].ToString());
				taskHist.DateTimeFinished = PIn.DateT (row["DateTimeFinished"].ToString());
				taskHist.PriorityDefNum   = PIn.Long  (row["PriorityDefNum"].ToString());
				taskHist.ReminderGroupId  = PIn.String(row["ReminderGroupId"].ToString());
				taskHist.ReminderType     = (OpenDentBusiness.TaskReminderType)PIn.Int(row["ReminderType"].ToString());
				taskHist.ReminderFrequency= PIn.Int   (row["ReminderFrequency"].ToString());
				taskHist.DateTimeOriginal = PIn.DateT (row["DateTimeOriginal"].ToString());
				taskHist.SecDateTEdit     = PIn.DateT (row["SecDateTEdit"].ToString());
				retVal.Add(taskHist);
			}
			return retVal;
		}

		///<summary>Converts a list of TaskHist into a DataTable.</summary>
		public static DataTable ListToTable(List<TaskHist> listTaskHists,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="TaskHist";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("TaskHistNum");
			table.Columns.Add("UserNumHist");
			table.Columns.Add("DateTStamp");
			table.Columns.Add("IsNoteChange");
			table.Columns.Add("TaskNum");
			table.Columns.Add("TaskListNum");
			table.Columns.Add("DateTask");
			table.Columns.Add("KeyNum");
			table.Columns.Add("Descript");
			table.Columns.Add("TaskStatus");
			table.Columns.Add("IsRepeating");
			table.Columns.Add("DateType");
			table.Columns.Add("FromNum");
			table.Columns.Add("ObjectType");
			table.Columns.Add("DateTimeEntry");
			table.Columns.Add("UserNum");
			table.Columns.Add("DateTimeFinished");
			table.Columns.Add("PriorityDefNum");
			table.Columns.Add("ReminderGroupId");
			table.Columns.Add("ReminderType");
			table.Columns.Add("ReminderFrequency");
			table.Columns.Add("DateTimeOriginal");
			table.Columns.Add("SecDateTEdit");
			foreach(TaskHist taskHist in listTaskHists) {
				table.Rows.Add(new object[] {
					POut.Long  (taskHist.TaskHistNum),
					POut.Long  (taskHist.UserNumHist),
					POut.DateT (taskHist.DateTStamp,false),
					POut.Bool  (taskHist.IsNoteChange),
					POut.Long  (taskHist.TaskNum),
					POut.Long  (taskHist.TaskListNum),
					POut.DateT (taskHist.DateTask,false),
					POut.Long  (taskHist.KeyNum),
					            taskHist.Descript,
					POut.Int   ((int)taskHist.TaskStatus),
					POut.Bool  (taskHist.IsRepeating),
					POut.Int   ((int)taskHist.DateType),
					POut.Long  (taskHist.FromNum),
					POut.Int   ((int)taskHist.ObjectType),
					POut.DateT (taskHist.DateTimeEntry,false),
					POut.Long  (taskHist.UserNum),
					POut.DateT (taskHist.DateTimeFinished,false),
					POut.Long  (taskHist.PriorityDefNum),
					            taskHist.ReminderGroupId,
					POut.Int   ((int)taskHist.ReminderType),
					POut.Int   (taskHist.ReminderFrequency),
					POut.DateT (taskHist.DateTimeOriginal,false),
					POut.DateT (taskHist.SecDateTEdit,false),
				});
			}
			return table;
		}

		///<summary>Inserts one TaskHist into the database.  Returns the new priKey.</summary>
		public static long Insert(TaskHist taskHist) {
			return Insert(taskHist,false);
		}

		///<summary>Inserts one TaskHist into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(TaskHist taskHist,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				taskHist.TaskHistNum=ReplicationServers.GetKey("taskhist","TaskHistNum");
			}
			string command="INSERT INTO taskhist (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="TaskHistNum,";
			}
			command+="UserNumHist,DateTStamp,IsNoteChange,TaskNum,TaskListNum,DateTask,KeyNum,Descript,TaskStatus,IsRepeating,DateType,FromNum,ObjectType,DateTimeEntry,UserNum,DateTimeFinished,PriorityDefNum,ReminderGroupId,ReminderType,ReminderFrequency,DateTimeOriginal,SecDateTEdit) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(taskHist.TaskHistNum)+",";
			}
			command+=
				     POut.Long  (taskHist.UserNumHist)+","
				+    DbHelper.Now()+","
				+    POut.Bool  (taskHist.IsNoteChange)+","
				+    POut.Long  (taskHist.TaskNum)+","
				+    POut.Long  (taskHist.TaskListNum)+","
				+    POut.Date  (taskHist.DateTask)+","
				+    POut.Long  (taskHist.KeyNum)+","
				+    DbHelper.ParamChar+"paramDescript,"
				+    POut.Int   ((int)taskHist.TaskStatus)+","
				+    POut.Bool  (taskHist.IsRepeating)+","
				+    POut.Int   ((int)taskHist.DateType)+","
				+    POut.Long  (taskHist.FromNum)+","
				+    POut.Int   ((int)taskHist.ObjectType)+","
				+    POut.DateT (taskHist.DateTimeEntry)+","
				+    POut.Long  (taskHist.UserNum)+","
				+    POut.DateT (taskHist.DateTimeFinished)+","
				+    POut.Long  (taskHist.PriorityDefNum)+","
				+"'"+POut.String(taskHist.ReminderGroupId)+"',"
				+    POut.Int   ((int)taskHist.ReminderType)+","
				+    POut.Int   (taskHist.ReminderFrequency)+","
				+    POut.DateT (taskHist.DateTimeOriginal)+","
				+    POut.DateT (taskHist.SecDateTEdit)+")";
			if(taskHist.Descript==null) {
				taskHist.Descript="";
			}
			OdSqlParameter paramDescript=new OdSqlParameter("paramDescript",OdDbType.Text,POut.StringParam(taskHist.Descript));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramDescript);
			}
			else {
				taskHist.TaskHistNum=Db.NonQ(command,true,"TaskHistNum","taskHist",paramDescript);
			}
			return taskHist.TaskHistNum;
		}

		///<summary>Inserts one TaskHist into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(TaskHist taskHist) {
			return InsertNoCache(taskHist,false);
		}

		///<summary>Inserts one TaskHist into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(TaskHist taskHist,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO taskhist (";
			if(!useExistingPK && isRandomKeys) {
				taskHist.TaskHistNum=ReplicationServers.GetKeyNoCache("taskhist","TaskHistNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="TaskHistNum,";
			}
			command+="UserNumHist,DateTStamp,IsNoteChange,TaskNum,TaskListNum,DateTask,KeyNum,Descript,TaskStatus,IsRepeating,DateType,FromNum,ObjectType,DateTimeEntry,UserNum,DateTimeFinished,PriorityDefNum,ReminderGroupId,ReminderType,ReminderFrequency,DateTimeOriginal,SecDateTEdit) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(taskHist.TaskHistNum)+",";
			}
			command+=
				     POut.Long  (taskHist.UserNumHist)+","
				+    DbHelper.Now()+","
				+    POut.Bool  (taskHist.IsNoteChange)+","
				+    POut.Long  (taskHist.TaskNum)+","
				+    POut.Long  (taskHist.TaskListNum)+","
				+    POut.Date  (taskHist.DateTask)+","
				+    POut.Long  (taskHist.KeyNum)+","
				+    DbHelper.ParamChar+"paramDescript,"
				+    POut.Int   ((int)taskHist.TaskStatus)+","
				+    POut.Bool  (taskHist.IsRepeating)+","
				+    POut.Int   ((int)taskHist.DateType)+","
				+    POut.Long  (taskHist.FromNum)+","
				+    POut.Int   ((int)taskHist.ObjectType)+","
				+    POut.DateT (taskHist.DateTimeEntry)+","
				+    POut.Long  (taskHist.UserNum)+","
				+    POut.DateT (taskHist.DateTimeFinished)+","
				+    POut.Long  (taskHist.PriorityDefNum)+","
				+"'"+POut.String(taskHist.ReminderGroupId)+"',"
				+    POut.Int   ((int)taskHist.ReminderType)+","
				+    POut.Int   (taskHist.ReminderFrequency)+","
				+    POut.DateT (taskHist.DateTimeOriginal)+","
				+    POut.DateT (taskHist.SecDateTEdit)+")";
			if(taskHist.Descript==null) {
				taskHist.Descript="";
			}
			OdSqlParameter paramDescript=new OdSqlParameter("paramDescript",OdDbType.Text,POut.StringParam(taskHist.Descript));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramDescript);
			}
			else {
				taskHist.TaskHistNum=Db.NonQ(command,true,"TaskHistNum","taskHist",paramDescript);
			}
			return taskHist.TaskHistNum;
		}

		///<summary>Deletes one TaskHist from the database.</summary>
		public static void Delete(long taskHistNum) {
			string command="DELETE FROM taskhist "
				+"WHERE TaskHistNum = "+POut.Long(taskHistNum);
			Db.NonQ(command);
		}

	}
}