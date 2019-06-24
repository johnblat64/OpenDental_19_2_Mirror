//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class SupplyCrud {
		///<summary>Gets one Supply object from the database using the primary key.  Returns null if not found.</summary>
		public static Supply SelectOne(long supplyNum) {
			string command="SELECT * FROM supply "
				+"WHERE SupplyNum = "+POut.Long(supplyNum);
			List<Supply> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Supply object from the database using a query.</summary>
		public static Supply SelectOne(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Supply> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Supply objects from the database using a query.</summary>
		public static List<Supply> SelectMany(string command) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Supply> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Supply> TableToList(DataTable table) {
			List<Supply> retVal=new List<Supply>();
			Supply supply;
			foreach(DataRow row in table.Rows) {
				supply=new Supply();
				supply.SupplyNum       = PIn.Long  (row["SupplyNum"].ToString());
				supply.SupplierNum     = PIn.Long  (row["SupplierNum"].ToString());
				supply.CatalogNumber   = PIn.String(row["CatalogNumber"].ToString());
				supply.Descript        = PIn.String(row["Descript"].ToString());
				supply.Category        = PIn.Long  (row["Category"].ToString());
				supply.ItemOrder       = PIn.Int   (row["ItemOrder"].ToString());
				supply.LevelDesired    = PIn.Float (row["LevelDesired"].ToString());
				supply.IsHidden        = PIn.Bool  (row["IsHidden"].ToString());
				supply.Price           = PIn.Double(row["Price"].ToString());
				supply.BarCodeOrID     = PIn.String(row["BarCodeOrID"].ToString());
				supply.DispDefaultQuant= PIn.Float (row["DispDefaultQuant"].ToString());
				supply.DispUnitsCount  = PIn.Int   (row["DispUnitsCount"].ToString());
				supply.DispUnitDesc    = PIn.String(row["DispUnitDesc"].ToString());
				supply.LevelOnHand     = PIn.Float (row["LevelOnHand"].ToString());
				retVal.Add(supply);
			}
			return retVal;
		}

		///<summary>Converts a list of Supply into a DataTable.</summary>
		public static DataTable ListToTable(List<Supply> listSupplys,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Supply";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("SupplyNum");
			table.Columns.Add("SupplierNum");
			table.Columns.Add("CatalogNumber");
			table.Columns.Add("Descript");
			table.Columns.Add("Category");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("LevelDesired");
			table.Columns.Add("IsHidden");
			table.Columns.Add("Price");
			table.Columns.Add("BarCodeOrID");
			table.Columns.Add("DispDefaultQuant");
			table.Columns.Add("DispUnitsCount");
			table.Columns.Add("DispUnitDesc");
			table.Columns.Add("LevelOnHand");
			foreach(Supply supply in listSupplys) {
				table.Rows.Add(new object[] {
					POut.Long  (supply.SupplyNum),
					POut.Long  (supply.SupplierNum),
					            supply.CatalogNumber,
					            supply.Descript,
					POut.Long  (supply.Category),
					POut.Int   (supply.ItemOrder),
					POut.Float (supply.LevelDesired),
					POut.Bool  (supply.IsHidden),
					POut.Double(supply.Price),
					            supply.BarCodeOrID,
					POut.Float (supply.DispDefaultQuant),
					POut.Int   (supply.DispUnitsCount),
					            supply.DispUnitDesc,
					POut.Float (supply.LevelOnHand),
				});
			}
			return table;
		}

		///<summary>Inserts one Supply into the database.  Returns the new priKey.</summary>
		public static long Insert(Supply supply) {
			return Insert(supply,false);
		}

		///<summary>Inserts one Supply into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Supply supply,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				supply.SupplyNum=ReplicationServers.GetKey("supply","SupplyNum");
			}
			string command="INSERT INTO supply (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="SupplyNum,";
			}
			command+="SupplierNum,CatalogNumber,Descript,Category,ItemOrder,LevelDesired,IsHidden,Price,BarCodeOrID,DispDefaultQuant,DispUnitsCount,DispUnitDesc,LevelOnHand) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(supply.SupplyNum)+",";
			}
			command+=
				     POut.Long  (supply.SupplierNum)+","
				+"'"+POut.String(supply.CatalogNumber)+"',"
				+"'"+POut.String(supply.Descript)+"',"
				+    POut.Long  (supply.Category)+","
				+    POut.Int   (supply.ItemOrder)+","
				+    POut.Float (supply.LevelDesired)+","
				+    POut.Bool  (supply.IsHidden)+","
				+"'"+POut.Double(supply.Price)+"',"
				+"'"+POut.String(supply.BarCodeOrID)+"',"
				+    POut.Float (supply.DispDefaultQuant)+","
				+    POut.Int   (supply.DispUnitsCount)+","
				+"'"+POut.String(supply.DispUnitDesc)+"',"
				+    POut.Float (supply.LevelOnHand)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				supply.SupplyNum=Db.NonQ(command,true,"SupplyNum","supply");
			}
			return supply.SupplyNum;
		}

		///<summary>Inserts one Supply into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Supply supply) {
			return InsertNoCache(supply,false);
		}

		///<summary>Inserts one Supply into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Supply supply,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO supply (";
			if(!useExistingPK && isRandomKeys) {
				supply.SupplyNum=ReplicationServers.GetKeyNoCache("supply","SupplyNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="SupplyNum,";
			}
			command+="SupplierNum,CatalogNumber,Descript,Category,ItemOrder,LevelDesired,IsHidden,Price,BarCodeOrID,DispDefaultQuant,DispUnitsCount,DispUnitDesc,LevelOnHand) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(supply.SupplyNum)+",";
			}
			command+=
				     POut.Long  (supply.SupplierNum)+","
				+"'"+POut.String(supply.CatalogNumber)+"',"
				+"'"+POut.String(supply.Descript)+"',"
				+    POut.Long  (supply.Category)+","
				+    POut.Int   (supply.ItemOrder)+","
				+    POut.Float (supply.LevelDesired)+","
				+    POut.Bool  (supply.IsHidden)+","
				+"'"+POut.Double(supply.Price)+"',"
				+"'"+POut.String(supply.BarCodeOrID)+"',"
				+    POut.Float (supply.DispDefaultQuant)+","
				+    POut.Int   (supply.DispUnitsCount)+","
				+"'"+POut.String(supply.DispUnitDesc)+"',"
				+    POut.Float (supply.LevelOnHand)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				supply.SupplyNum=Db.NonQ(command,true,"SupplyNum","supply");
			}
			return supply.SupplyNum;
		}

		///<summary>Updates one Supply in the database.</summary>
		public static void Update(Supply supply) {
			string command="UPDATE supply SET "
				+"SupplierNum     =  "+POut.Long  (supply.SupplierNum)+", "
				+"CatalogNumber   = '"+POut.String(supply.CatalogNumber)+"', "
				+"Descript        = '"+POut.String(supply.Descript)+"', "
				+"Category        =  "+POut.Long  (supply.Category)+", "
				+"ItemOrder       =  "+POut.Int   (supply.ItemOrder)+", "
				+"LevelDesired    =  "+POut.Float (supply.LevelDesired)+", "
				+"IsHidden        =  "+POut.Bool  (supply.IsHidden)+", "
				+"Price           = '"+POut.Double(supply.Price)+"', "
				+"BarCodeOrID     = '"+POut.String(supply.BarCodeOrID)+"', "
				+"DispDefaultQuant=  "+POut.Float (supply.DispDefaultQuant)+", "
				+"DispUnitsCount  =  "+POut.Int   (supply.DispUnitsCount)+", "
				+"DispUnitDesc    = '"+POut.String(supply.DispUnitDesc)+"', "
				+"LevelOnHand     =  "+POut.Float (supply.LevelOnHand)+" "
				+"WHERE SupplyNum = "+POut.Long(supply.SupplyNum);
			Db.NonQ(command);
		}

		///<summary>Updates one Supply in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Supply supply,Supply oldSupply) {
			string command="";
			if(supply.SupplierNum != oldSupply.SupplierNum) {
				if(command!="") { command+=",";}
				command+="SupplierNum = "+POut.Long(supply.SupplierNum)+"";
			}
			if(supply.CatalogNumber != oldSupply.CatalogNumber) {
				if(command!="") { command+=",";}
				command+="CatalogNumber = '"+POut.String(supply.CatalogNumber)+"'";
			}
			if(supply.Descript != oldSupply.Descript) {
				if(command!="") { command+=",";}
				command+="Descript = '"+POut.String(supply.Descript)+"'";
			}
			if(supply.Category != oldSupply.Category) {
				if(command!="") { command+=",";}
				command+="Category = "+POut.Long(supply.Category)+"";
			}
			if(supply.ItemOrder != oldSupply.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(supply.ItemOrder)+"";
			}
			if(supply.LevelDesired != oldSupply.LevelDesired) {
				if(command!="") { command+=",";}
				command+="LevelDesired = "+POut.Float(supply.LevelDesired)+"";
			}
			if(supply.IsHidden != oldSupply.IsHidden) {
				if(command!="") { command+=",";}
				command+="IsHidden = "+POut.Bool(supply.IsHidden)+"";
			}
			if(supply.Price != oldSupply.Price) {
				if(command!="") { command+=",";}
				command+="Price = '"+POut.Double(supply.Price)+"'";
			}
			if(supply.BarCodeOrID != oldSupply.BarCodeOrID) {
				if(command!="") { command+=",";}
				command+="BarCodeOrID = '"+POut.String(supply.BarCodeOrID)+"'";
			}
			if(supply.DispDefaultQuant != oldSupply.DispDefaultQuant) {
				if(command!="") { command+=",";}
				command+="DispDefaultQuant = "+POut.Float(supply.DispDefaultQuant)+"";
			}
			if(supply.DispUnitsCount != oldSupply.DispUnitsCount) {
				if(command!="") { command+=",";}
				command+="DispUnitsCount = "+POut.Int(supply.DispUnitsCount)+"";
			}
			if(supply.DispUnitDesc != oldSupply.DispUnitDesc) {
				if(command!="") { command+=",";}
				command+="DispUnitDesc = '"+POut.String(supply.DispUnitDesc)+"'";
			}
			if(supply.LevelOnHand != oldSupply.LevelOnHand) {
				if(command!="") { command+=",";}
				command+="LevelOnHand = "+POut.Float(supply.LevelOnHand)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE supply SET "+command
				+" WHERE SupplyNum = "+POut.Long(supply.SupplyNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(Supply,Supply) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Supply supply,Supply oldSupply) {
			if(supply.SupplierNum != oldSupply.SupplierNum) {
				return true;
			}
			if(supply.CatalogNumber != oldSupply.CatalogNumber) {
				return true;
			}
			if(supply.Descript != oldSupply.Descript) {
				return true;
			}
			if(supply.Category != oldSupply.Category) {
				return true;
			}
			if(supply.ItemOrder != oldSupply.ItemOrder) {
				return true;
			}
			if(supply.LevelDesired != oldSupply.LevelDesired) {
				return true;
			}
			if(supply.IsHidden != oldSupply.IsHidden) {
				return true;
			}
			if(supply.Price != oldSupply.Price) {
				return true;
			}
			if(supply.BarCodeOrID != oldSupply.BarCodeOrID) {
				return true;
			}
			if(supply.DispDefaultQuant != oldSupply.DispDefaultQuant) {
				return true;
			}
			if(supply.DispUnitsCount != oldSupply.DispUnitsCount) {
				return true;
			}
			if(supply.DispUnitDesc != oldSupply.DispUnitDesc) {
				return true;
			}
			if(supply.LevelOnHand != oldSupply.LevelOnHand) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Supply from the database.</summary>
		public static void Delete(long supplyNum) {
			string command="DELETE FROM supply "
				+"WHERE SupplyNum = "+POut.Long(supplyNum);
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<Supply> listNew,List<Supply> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<Supply> listIns    =new List<Supply>();
			List<Supply> listUpdNew =new List<Supply>();
			List<Supply> listUpdDB  =new List<Supply>();
			List<Supply> listDel    =new List<Supply>();
			listNew.Sort((Supply x,Supply y) => { return x.SupplyNum.CompareTo(y.SupplyNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((Supply x,Supply y) => { return x.SupplyNum.CompareTo(y.SupplyNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			Supply fieldNew;
			Supply fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.SupplyNum<fieldDB.SupplyNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.SupplyNum>fieldDB.SupplyNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(Update(listUpdNew[i],listUpdDB[i])) {
					rowsUpdatedCount++;
				}
			}
			for(int i=0;i<listDel.Count;i++) {
				Delete(listDel[i].SupplyNum);
			}
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}

	}
}