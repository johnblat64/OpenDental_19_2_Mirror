﻿using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using OpenDental;
using System.Linq;

namespace CentralManager {
	public partial class FormCentralPatientTransfer:Form {
		private CentralConnection _sourceConnection;
		///<summary>Contains a list of ODGridRows. The ODGridRow contains information about the selected patient and Central Connection of the patient.</summary>
		private List<DataRow> _listPatientDataRows;
		private List<CentralConnection> _listConnections;
		private List<CentralConnection> _listConnectionsToTransferTo;
		private List<Sheet> _listSheetsForSelectedPats;

		public FormCentralPatientTransfer(CentralConnection sourceConn,List<CentralConnection> listConnections) {
			_sourceConnection=sourceConn;
			_listConnections=listConnections.FindAll(x => x.CentralConnectionNum!=sourceConn.CentralConnectionNum);//Remove the source connection from list
			_listPatientDataRows=new List<DataRow>();
			_listConnectionsToTransferTo=new List<CentralConnection>();
			InitializeComponent();
		}

		private void FormCentralConnectionPatientTransfer_Load(object sender,EventArgs e) {
			labelSourceDb.Text+=_sourceConnection.DatabaseName;
			FillPatients();
			FillDatabases();
		}

		private void FillDatabases() {
			Cursor=Cursors.WaitCursor;
			gridDatabasesTo.BeginUpdate();
			gridDatabasesTo.Columns.Clear();
			gridDatabasesTo.Columns.Add(new ODGridColumn(Lan.g("TableDatabases","Databases"),150));
			gridDatabasesTo.Columns.Add(new ODGridColumn(Lan.g("TableDatabases","Note"),200));
			gridDatabasesTo.Columns.Add(new ODGridColumn("Status",30,HorizontalAlignment.Center));
			gridDatabasesTo.Rows.Clear();
			ODGridRow row;
			foreach(CentralConnection conn in _listConnectionsToTransferTo) {
				row=new ODGridRow();
				if(conn.DatabaseName=="") {//uri
					row.Cells.Add(conn.ServiceURI);
				}
				else {
					row.Cells.Add(conn.ServerName+", "+conn.DatabaseName);
				}
				row.Cells.Add(conn.Note);
				row.Cells.Add(conn.ConnectionStatus);
				row.Tag=conn;
				gridDatabasesTo.Rows.Add(row);
			}
			gridDatabasesTo.EndUpdate();
			Cursor=Cursors.Default;
		}

		private void FillPatients() {
			Cursor=Cursors.WaitCursor;
			gridPatients.BeginUpdate();
			gridPatients.Columns.Clear();
			gridPatients.Columns.Add(new ODGridColumn(Lan.g("TablePatients","PatNum"),140));
			gridPatients.Columns.Add(new ODGridColumn(Lan.g("TablePatients","LName"),140));
			gridPatients.Columns.Add(new ODGridColumn(Lan.g("TablePatients","FName"),140,true));
			gridPatients.Rows.Clear();
			ODGridRow row;
			foreach(DataRow pat in _listPatientDataRows) {
				row=new ODGridRow();
				row.Cells.Add(pat["PatNum"].ToString());//PatNum
				row.Cells.Add(pat["LName"].ToString());//LName
				row.Cells.Add(pat["FName"].ToString());//FName
				row.Tag=pat;
				gridPatients.Rows.Add(row);
			}
			gridPatients.EndUpdate();
			Cursor=Cursors.Default;
		}

		private void butAddPatients_Click(object sender,EventArgs e) {
			FormCentralPatientSearch FormCPS=new FormCentralPatientSearch();
			FormCPS.ListConns=new List<CentralConnection> { _sourceConnection };
			FormCPS.IsSelectionMode=true;
			if(FormCPS.ShowDialog()==DialogResult.OK) { 
				DataRow selectedPatRow=FormCPS.SelectedPatient;
				if(_listPatientDataRows.AsEnumerable().Any(x => x["PatNum"].ToString()==selectedPatRow["PatNum"].ToString())) {
					return;//No need to add the same patient to the grid.
				}
				_listPatientDataRows.Add(selectedPatRow);
				FillPatients();
			}
		}

		private void butAddDatabases_Click(object sender,EventArgs e) {
			FormCentralConnections FormCC=new FormCentralConnections();
			FormCC.LabelText.Text=Lans.g("PatientTransfer","Select databases to transfer patients to.");
			FormCC.Text=Lans.g("PatientTransfer","Select Databases");
			FormCC.IsSelectionMode=true;
			foreach(CentralConnection conn in _listConnections) {
				FormCC.ListConns.Add(conn.Copy());
			}
			if(FormCC.ShowDialog()==DialogResult.OK) {
				_listConnectionsToTransferTo.AddRange(FormCC.ListConns.FindAll(x=>!x.CentralConnectionNum.In(_listConnectionsToTransferTo.Select(y=>y.CentralConnectionNum))));
				FillDatabases();
			}
		}

		private void butRemovePats_Click(object sender,EventArgs e) {
			if(gridPatients.SelectedIndices.Count()==0) {
				MsgBox.Show(this,"At least one patient must be selected.");
				return;
			}
			foreach(ODGridRow row in gridPatients.SelectedGridRows) {
				_listPatientDataRows.Remove((DataRow)row.Tag);
			}
			FillPatients();
		}

		private void butRemoveDb_Click(object sender,EventArgs e) {
			if(gridDatabasesTo.SelectedIndices.Count()==0) {
				MsgBox.Show(this,"At least one connection must be selected.");
				return;
			}
			foreach(ODGridRow row in gridDatabasesTo.SelectedGridRows) {
				_listConnectionsToTransferTo.Remove((CentralConnection)row.Tag);
			}
			FillDatabases();
		}

		private void butTransfer_Click(object sender,EventArgs e) {
			if(gridPatients.Rows.Count==0) {
				MsgBox.Show(this,"At least one patient must be selected to transfer.");
				return;
			}
			if(gridDatabasesTo.Rows.Count==0) {
				MsgBox.Show(this,"At least one database must be selected to transfer.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The transfer process may take a long time. Continue?")) {
				return;
			}
			ODProgress.ShowAction(() => RunTransfer(), startingMessage: Lans.g(this,"Transferring patient(s)..."),
				actionException: err => this.Invoke(() => FriendlyException.Show(Lan.g(this,"Error transferring patient(s)."),err)));
		}

		private void RunTransfer() {
			//Get data from the source connection. This will have all the patients selected from the sourceconnection
			string stringFailedConns="";
			_listSheetsForSelectedPats=GetDataFromSourceConnection(_sourceConnection);
			if(_listSheetsForSelectedPats.IsNullOrEmpty()) {
				string connString=CentralConnections.GetConnectionString(_sourceConnection);
				stringFailedConns+=connString+"\r\n";
			}
			//Insert the sheets to each of the databases. The sheets will have a patnum=0;
			List<Action> listActions=new List<Action>();
			object locker=new object();
			foreach(CentralConnection conn in _listConnectionsToTransferTo) {
				List<Sheet> listSheets=_listSheetsForSelectedPats.Select(x => x.Copy()).ToList();
				listActions.Add(() => {
					if(!InsertSheetsToConnection(conn,listSheets)) {
						string connString=CentralConnections.GetConnectionString(conn);
						lock(locker) {
							stringFailedConns+=connString+"\r\n";
						}
					}
				});
			}
			ODThread.RunParallel(listActions);
			if(stringFailedConns!="") {
				stringFailedConns=Lans.g(this,"There were some transfers that failed due to connection issues.  Fix connections and try again.\r\n"
					+"Failed Connections:")+"\r\n"+stringFailedConns;
				CodeBase.MsgBoxCopyPaste msgBoxCP=new CodeBase.MsgBoxCopyPaste(stringFailedConns);
				msgBoxCP.ShowDialog();
			}
			else {
				MsgBox.Show(this,"Transfers Completed Successfully\r\nGo to each database you transferred patients to and retrieve Webforms to finish the "
					+"transfer process.");
			}
		}

		private bool InsertSheetsToConnection(CentralConnection conn,List<Sheet> listSheets) {
			if(!CentralConnectionHelper.UpdateCentralConnection(conn,false)) {
				conn.ConnectionStatus=Lans.g(this,"OFFLINE");
				return false;
			}
			//make sure the sheets have patnum=0 and the sheet is marked as new and sheetnum=0.
			foreach(Sheet sheet in listSheets) {
				sheet.SheetNum=0;
				sheet.PatNum=0;
				sheet.IsNew=true;
			}
			Sheets.SaveNewSheetList(listSheets);
			return true;
		}

		private List<Sheet> GetDataFromSourceConnection(CentralConnection conn) {
			if(!CentralConnectionHelper.UpdateCentralConnection(conn,false)) {
				conn.ConnectionStatus=Lans.g(this,"OFFLINE");
				return new List<Sheet>();
			}
			List<Sheet> listSheetsToTransfer=new List<Sheet>();
			List<long> listPatNumsToTransfer=_listPatientDataRows.Select(x=>PIn.Long(x["PatNum"].ToString())).ToList();
			return GetSheetsForTransfer(listPatNumsToTransfer);
		}

		private List<Sheet> GetSheetsForTransfer(List<long> listPatNumsToTransfer) {
			List<Sheet> retVal=new List<Sheet>();
			List<Patient> listPatients=Patients.GetMultPats(listPatNumsToTransfer).ToList();
			foreach(Patient pat in listPatients) {
				Sheet sheetCemt=SheetUtil.CreateSheet(SheetsInternal.GetSheetDef(SheetInternalType.PatientTransferCEMT));
				SheetFiller.FillFieldsForPatientTransferCEMT(sheetCemt,pat);
				retVal.Add(sheetCemt);
			}
			return retVal;
		}

	}
}
