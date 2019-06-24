/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDental.DivvyConnect;
using System.Net;
using System.Xml;
using System.Text;
using CodeBase;
using System.Linq;
using System.Threading;

namespace OpenDental{
///<summary></summary>
	public partial class FormRecallList:ODForm {
		private int pagesPrinted;
		private DataTable addrTable;
		private int patientsPrinted;
		private DataTable _tableRecalls;
		private bool headingPrinted;
		private int headingPrintH;
		private List<EmailAddress> _listEmailAddresses;
		///<summary>The clinics that are signed up for Web Sched.</summary>
		private List<long> _listClinicNumsWebSched=new List<long>();
		private ODThread _threadWebSchedSignups=null;
		///<summary>Indicates whether the Reg Key is currently on Open Dental support.</summary>
		private YN _isOnSupport=YN.Unknown;
		///<summary>The user has clicked the Web Sched button while a thread was busy checking which clinics are signed up for Web Sched.</summary>
		private bool _hasClickedWebSched;
		///<summary>A Func that can be called to get the main recall list table.</summary>
		private Func<DataTable> _getRecallTable;
		///<summary>Short list, for the two combo boxes.</summary>
		private List<Provider> _listProv;

		///<summary>True if the thread checking the clinics that are signed up for Web Sched has finished.</summary>
		private bool _isDoneCheckingWebSchedClinics {
			get { return _threadWebSchedSignups==null; }
		}

		///<summary>Each tab should have an ODGrid set as the tag, returns the grid attached to the currently selected tab.</summary>
		private ODGrid _gridCur { get { return (ODGrid)tabControl.SelectedTab.Tag; } }

		///<summary>Gets the DefNum for the Mailed Status of the corresponding grid (recall or reactivation).</summary>
		private long _statusMailed {
			get { return PrefC.GetLong(IsRecallGridSelected()?PrefName.RecallStatusMailed:PrefName.ReactivationStatusMailed); }
		}

		///<summary>Gets the DefNum for the Emailed Status of the corresponding grid (recall or reactivation).</summary>
		private long _statusEmailed {
			get { return PrefC.GetLong(IsRecallGridSelected()?PrefName.RecallStatusEmailed:PrefName.ReactivationStatusEmailed); }
		}

		///<summary>Returns the PatNum of the first selected row in the corresponding grid.  Can return 0.</summary>
		private long _patNumCur {
			get {
				try {
					return (_gridCur==gridRecentlyContacted) ? _gridCur.SelectedTag<Recalls.RecallRecent>().PatNum : _gridCur.SelectedTag<PatRowTag>().PatNum;
				}
				catch(Exception e) {
					e.DoNothing();
					return 0;
				}
			}
		}

		private bool IsRecallGridSelected() {
			return _gridCur==gridMain;
		}

		private bool IsReactivationGridSelected() {
			return _gridCur==gridReactivations;
		}

		private bool DoGroupFamilies() {
			return (IsRecallGridSelected() && checkGroupFamilies.Checked) || (IsReactivationGridSelected() && checkReactGroupFamilies.Checked);
		}
		
		///<summary></summary>
		public FormRecallList(){
			InitializeComponent();// Required for Windows Form Designer support
			gridMain.ContextMenu=menuRightClick;
			gridRecentlyContacted.ContextMenu=menuRightClick;
			gridReactivations.ContextMenu=menuRightClick;
			if(!PrefC.GetBool(PrefName.ShowFeatureReactivations)) {
				tabControl.Controls.Remove(tabPageReactivations);
			}
			Lan.F(this);
		}

		private void FormRecallList_Load(object sender, System.EventArgs e) {
			//AptNumsSelected=new List<long>();
			CheckClinicsSignedUpForWebSched();
#if DEBUG
			butECards.Visible=true;
#endif
			checkGroupFamilies.Checked=PrefC.GetBool(PrefName.RecallGroupByFamily);
			//Fill sort types
			for(int i=0;i<Enum.GetNames(typeof(RecallListSort)).Length;i++){
				comboSort.Items.Add(Lan.g("enumRecallListSort",Enum.GetNames(typeof(RecallListSort))[i]));
			}
			comboSort.SelectedIndex=0;
			comboReactSortBy.SetItemsWithEnum<ReactivationListSort>();
			comboReactSortBy.SelectedIndex=0;
			//Fill number reminders
			comboNumberReminders.Items.Add(new ODBoxItem<RecallListShowNumberReminders>(Lan.g(this,"All"),(RecallListShowNumberReminders.All)));
			comboShowReactivate.Items.Add(new ODBoxItem<RecallListShowNumberReminders>(Lan.g(this,"All"),(RecallListShowNumberReminders.All)));
			int maxReactivationNums=PrefC.GetInt(PrefName.ReactivationCountContactMax);
			for(int i=0;i<=6;i++) {
				ODBoxItem<RecallListShowNumberReminders> item=new ODBoxItem<RecallListShowNumberReminders>(i==6?"6+":i.ToString(),(RecallListShowNumberReminders)(i+1));
				comboNumberReminders.Items.Add(item);
				if(maxReactivationNums>-1 && maxReactivationNums<(i+1)) {
					continue; //pref doesn't allow us more contacts than this anyway
				}
				comboShowReactivate.Items.Add(item);
			}
			comboNumberReminders.SelectedIndex=0;
			comboShowReactivate.SelectedIndex=0;
			//Set recall days
			int daysPast=PrefC.GetInt(PrefName.RecallDaysPast);
			int daysFuture=PrefC.GetInt(PrefName.RecallDaysFuture);
			if(daysPast==-1){
				textDateStart.Text="";
			}
			else{
				textDateStart.Text=DateTime.Today.AddDays(-daysPast).ToShortDateString();
			}
			if(daysFuture==-1) {
				textDateEnd.Text="";
			}
			else {
				textDateEnd.Text=DateTime.Today.AddDays(daysFuture).ToShortDateString();
			}
			//Set Days Since in reactivations
			int daysSince=PrefC.GetInt(PrefName.ReactivationDaysPast);
			validDateSince.Text=daysSince==-1?"":DateTime.Today.AddDays(-daysSince).ToShortDateString();
			_listProv=Providers.GetDeepCopy(isShort:true);
			comboProv.Items.Add("All");
			for(int i=0;i<_listProv.Count;i++){
				comboProv.Items.Add(_listProv[i].GetLongDesc());					
			}
			comboProv.SelectedIndex=0;
			comboReactProv.Items.Add("All");
			for(int i=0;i<_listProv.Count;i++){
				comboReactProv.Items.Add(_listProv[i].GetLongDesc());					
			}
			comboReactProv.SelectedIndex=0;
			//Clinic Visibility
			//Sites
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				labelSite.Visible=false;
				labelReactSite.Visible=false;
				comboSite.Visible=false;
				comboReactSite.Visible=false;
			}
			else {
				List<Site> listSites=Sites.GetDeepCopy();
				comboSite.SetItems(listSites,(site) => site.Description,specialValue:ComboBoxSpecialValues.All);
				comboSite.SelectedIndex=0;
				comboReactSite.SetItems(listSites,(site) => site.Description,specialValue:ComboBoxSpecialValues.All);
				comboReactSite.SelectedIndex=0;
			}
			//RecallUnschedStatuses
			List<Def> listRecallUnschedStatusDefs=Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,true);
			comboStatus.SetItems(listRecallUnschedStatusDefs,(status) => status.ItemName,specialValue:ComboBoxSpecialValues.None);
			comboReactStatus.SetItems(listRecallUnschedStatusDefs,(status) => status.ItemName,specialValue:ComboBoxSpecialValues.None);
			//Billing Types
			comboBillingTypes.SetItems(Defs.GetDefsForCategory(DefCat.BillingTypes),(def) => def.ItemName,specialValue:ComboBoxSpecialValues.All);
			comboBillingTypes.SelectedIndex=0;
			//Set grid fill methods
			gridMain.SetFillGrid(() => FillMain());
			gridRecentlyContacted.SetFillGrid(() => FillGridRecent());
			gridReactivations.SetFillGrid(() => FillReactivationGrid());
			//Etc.
			FillMain();
			FillComboEmail();
			Plugins.HookAddCode(this,"FormRecallList.Load_End",_tableRecalls);
		}

		private void CheckClinicsSignedUpForWebSched() {
			if(_threadWebSchedSignups!=null) {
				return;
			}
			_threadWebSchedSignups=new ODThread(new ODThread.WorkerDelegate((ODThread o) => {
				_listClinicNumsWebSched=WebServiceMainHQProxy.GetEServiceClinicsAllowed(
					Clinics.GetDeepCopy().Select(x => x.ClinicNum).ToList(),
					eServiceCode.WebSched);
				_isOnSupport=YN.Yes;
			}));
			//Swallow all exceptions and allow thread to exit gracefully.
			_threadWebSchedSignups.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { 	}));
			_threadWebSchedSignups.AddExitHandler(new ODThread.WorkerDelegate((ODThread o) => {
				ThreadWebSchedSignupsExitHandler();
			}));
			_threadWebSchedSignups.Name="CheckWebSchedSignups";
			_threadWebSchedSignups.Start(true);
		}

		private void ThreadWebSchedSignupsExitHandler() { 
			if(IsDisposed) {
				return;
			}
			if(InvokeRequired) {
				Invoke((Action)(() => { ThreadWebSchedSignupsExitHandler(); }));
				return;
			}
			_threadWebSchedSignups=null;
			Cursor=Cursors.Default;
			if(_hasClickedWebSched) {
				try {
					SendWebSched();
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Error sending Web Sched notifications. Error message:")+" "+ex.Message);
				}
			}
			_hasClickedWebSched=false;
		}

		private void FillComboEmail() {
			_listEmailAddresses=EmailAddresses.GetDeepCopy();//Does not include user specific email addresses.
			List<Clinic> listClinicsAll=Clinics.GetDeepCopy();
			for(int i=0;i<listClinicsAll.Count;i++) {//Exclude any email addresses that are associated to a clinic.
				_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==listClinicsAll[i].EmailAddressNum);
			}
			//Exclude default practice email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum));
			//Exclude web mail notification email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum));
			comboEmailFrom.Items.Add(Lan.g(this,"Practice/Clinic"));//default
			comboEmailFrom.SelectedIndex=0;
			comboReactEmailFrom.Items.Add(Lan.g(this,"Practice/Clinic"));//default
			comboReactEmailFrom.SelectedIndex=0;
			//Add all email addresses which are not associated to a user, a clinic, or either of the default email addresses.
			for(int i=0;i<_listEmailAddresses.Count;i++) {
				comboEmailFrom.Items.Add(_listEmailAddresses[i].EmailUsername);
				comboReactEmailFrom.Items.Add(_listEmailAddresses[i].EmailUsername);
			}
			//Add user specific email address if present.
			EmailAddress emailAddressMe=EmailAddresses.GetForUser(Security.CurUser.UserNum);//can be null
			if(emailAddressMe!=null) {
				_listEmailAddresses.Insert(0,emailAddressMe);
				comboEmailFrom.Items.Insert(1,Lan.g(this,"Me")+" <"+emailAddressMe.EmailUsername+">");//Just below Practice/Clinic
				comboReactEmailFrom.Items.Insert(1,Lan.g(this,"Me")+" <"+emailAddressMe.EmailUsername+">");//Just below Practice/Clinic
			}
		}

		///<summary>Shows a progress window and fills the main Recall List grid. Goes to database.</summary>
		private void FillMain() {
			//Verification
			if(textDateStart.errorProvider1.GetError(textDateStart)!=""|| textDateEnd.errorProvider1.GetError(textDateEnd)!="") {
				return;
			}
			//Create action for the progress bar.
			Action fillGridAction=() => {
				//remember which recallnums were selected
				List<string> recallNums=new List<string>();
				for(int i=0;i<gridMain.SelectedIndices.Length;i++){
					recallNums.Add(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["RecallNum"].ToString());
				}
				DateTime fromDate;
				DateTime toDate;
				if(textDateStart.Text==""){
					fromDate=DateTime.MinValue;
				}
				else{
					fromDate=PIn.Date(textDateStart.Text);
				}
				if(textDateEnd.Text=="") {
					toDate=DateTime.MaxValue;
				}
				else {
					toDate=PIn.Date(textDateEnd.Text);
				}
				long provNum=-1;
				if(comboProv.SelectedIndex>0){
					provNum=_listProv[comboProv.SelectedIndex-1].ProvNum;
				}
				long siteNum=0;
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex>0) {
					siteNum=comboSite.SelectedTag<Site>().SiteNum;
				}
				RecallListSort sortBy=(RecallListSort)comboSort.SelectedIndex;
				RecallListShowNumberReminders showReminders=(RecallListShowNumberReminders)comboNumberReminders.SelectedIndex;
				bool checkGroupFamilesChecked=checkGroupFamilies.Checked;
				_getRecallTable=new Func<DataTable>(() => {
					//Storing this as a Func so that we can make the exact same call before sending Web Sched.
					return Recalls.GetRecallList(fromDate,toDate,checkGroupFamilesChecked,provNum,comboClinic.SelectedClinicNum,siteNum,sortBy,showReminders,
						doShowReminded:checkShowReminded.Checked);
				});
				_tableRecalls=_getRecallTable();
				RecallListEvent.Fire(ODEventType.RecallList,Lans.g(this,"Filling the Recall List grid..."));
				int scrollval=gridMain.ScrollValue;
				gridMain.BeginUpdate();
				gridMain.Columns.Clear();
				ODGridColumn col;
				List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.RecallList);
				for(int i=0;i<fields.Count;i++) {
					if(fields[i].Description=="") {
						col=new ODGridColumn(fields[i].InternalName,fields[i].ColumnWidth);
					}
					else {
						col=new ODGridColumn(fields[i].Description,fields[i].ColumnWidth);
					}
					col.Tag=fields[i].InternalName;
					gridMain.Columns.Add(col);
				}
				gridMain.Rows.Clear();
				ODGridRow row;
				List<long> listConflictingPatNums=new List<long>();
				if(checkConflictingTypes.Checked) {
					listConflictingPatNums=Recalls.GetConflictingPatNums(_tableRecalls.Rows.OfType<DataRow>().Select(x => PIn.Long(x["PatNum"].ToString())).ToList());
				}
				for(int i=0;i<_tableRecalls.Rows.Count;i++){
					if(checkConflictingTypes.Checked) {
						//If the RecallType checkbox is checked, show patients with future scheduled appointments that have conflicting recall appointments.
						//Ex. A patient is scheduled for a perio recall but their recall type is set to prophy
						long patNum=PIn.Long(_tableRecalls.Rows[i]["PatNum"].ToString());
						long recallTypeNum=PIn.Long(_tableRecalls.Rows[i]["RecallTypeNum"].ToString());
						if(!patNum.In(listConflictingPatNums)) {
							//The patient does not have any conflicting recall type.
							//Continue since we don't want to show them when the RecallTypes checkbox is checked. 
							continue;
						}
						if(!RecallTypes.IsSpecialRecallType(recallTypeNum)) {
							//Make sure recall type is Perio or Prophy
							continue;
						}
					}
					row=new ODGridRow();
					for(int f=0;f<fields.Count;f++) {
						switch(fields[f].InternalName){
							case "Due Date":
								row.Cells.Add(_tableRecalls.Rows[i]["dueDate"].ToString());
								break;
							case "Patient":
								row.Cells.Add(_tableRecalls.Rows[i]["patientName"].ToString());
								break;
							case "Age":
								row.Cells.Add(_tableRecalls.Rows[i]["age"].ToString());
								break;
							case "Type":
								row.Cells.Add(_tableRecalls.Rows[i]["recallType"].ToString());
								break;
							case "Interval":
								row.Cells.Add(_tableRecalls.Rows[i]["recallInterval"].ToString());
								break;
							case "#Remind":
								row.Cells.Add(_tableRecalls.Rows[i]["numberOfReminders"].ToString());
								break;
							case "LastRemind":
								row.Cells.Add(_tableRecalls.Rows[i]["dateLastReminder"].ToString());
								break;
							case "Contact":
								row.Cells.Add(_tableRecalls.Rows[i]["contactMethod"].ToString());
								break;
							case "Status":
								row.Cells.Add(_tableRecalls.Rows[i]["status"].ToString());
								break;
							case "Note":
								row.Cells.Add(_tableRecalls.Rows[i]["Note"].ToString());
								break;
							case "BillingType":
								row.Cells.Add(_tableRecalls.Rows[i]["billingType"].ToString());
								break;
							case "WebSched":
								row.Cells.Add(_tableRecalls.Rows[i]["webSchedSendDesc"].ToString());
								break;
						}
					}
					row.Tag=new PatRowTag(
						PIn.Long(_tableRecalls.Rows[i]["PatNum"].ToString()),
						PIn.Long(_tableRecalls.Rows[i]["RecallNum"].ToString()),
						PIn.Long(_tableRecalls.Rows[i]["RecallStatus"].ToString()),
						PIn.Int(_tableRecalls.Rows[i]["numberOfReminders"].ToString()),
						PIn.String(_tableRecalls.Rows[i]["Email"].ToString()),
						PIn.Enum<ContactMethod>(_tableRecalls.Rows[i]["PreferRecallMethod"].ToString()),
						PIn.Long(_tableRecalls.Rows[i]["Guarantor"].ToString()));
					gridMain.Rows.Add(row);
				}
				gridMain.EndUpdate();
				//reselect original items
				for(int i=0;i<_tableRecalls.Rows.Count;i++){
					if(recallNums.Contains(_tableRecalls.Rows[i]["RecallNum"].ToString())){
						gridMain.SetSelected(i,true);
					}
				}
				labelPatientCount.Text=Lan.g(this,"Patient Count:")+" "+_tableRecalls.Rows.Count.ToString();
			};
			//Show progress window while filling the grid.
			ODProgress.ShowAction(
				fillGridAction,
				startingMessage:Lans.g(this,"Retrieving data for the Recall List grid..."),
				eventType:typeof(RecallListEvent),
				odEventType:ODEventType.RecallList
			);
		}
		
		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary> 
		private void grid_CellClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			//row selected before this event triggered
			SetFamilyColors();
			//comboStatus.SelectedIndex=-1;//mess with this later
			//SelectedPatNum=PIn.PLong(table.Rows[e.Row]["PatNum"].ToString());
		}

		private void SetFamilyColors() { 
			if(_gridCur==gridRecentlyContacted) {
				return; //family colors not applicable here
			}
			if(_gridCur.SelectedIndices.Length!=1) { //If we don't have a single row selected, reset colors to black
				for(int i=0;i<_gridCur.Rows.Count;i++) {
					_gridCur.Rows[i].ColorText=Color.Black;
				}
				_gridCur.Invalidate();
				return;
			}
			//only one row is selected so highlight family members if we can
			long guar=_gridCur.SelectedTag<PatRowTag>().GuarantorNum;
			int famCount=0;
			for(int i=0;i<_gridCur.Rows.Count;i++) {
				if(((PatRowTag)_gridCur.Rows[i].Tag).GuarantorNum==guar){ //family member
					famCount++;
					_gridCur.Rows[i].ColorText=Color.Red;
				}
				else {
					_gridCur.Rows[i].ColorText=Color.Black;
				}
			}
			if(famCount==1) {//only the highlighted patient is red at this point
				_gridCur.Rows[_gridCur.SelectedIndices[0]].ColorText=Color.Black;
			}
			_gridCur.Invalidate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.Columns[e.Col].Tag.ToString()=="WebSched") {//A column's tag is its display field internal name.
				MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(PIn.String(_tableRecalls.Rows[e.Row]["webSchedSendError"].ToString()));
				msgBox.Text=Lan.g(this,"Web Sched Notification Send Error");
				msgBox.ShowDialog();
				return;
			}
			long selectedPatNum=((PatRowTag)gridMain.Rows[e.Row].Tag).PatNum;
			Recall recall=Recalls.GetRecall(((PatRowTag)gridMain.Rows[e.Row].Tag).PriKeyNum);
			if(recall==null) {
				MsgBox.Show(this,"Recall for this patient has been removed.");
				FillMain();
				return;
			}
			FormRecallEdit FormR=new FormRecallEdit();
			FormR.RecallCur=recall.Copy();
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK) {
				return;
			}
			if(recall.RecallStatus!=FormR.RecallCur.RecallStatus//if the status has changed
				|| (recall.IsDisabled != FormR.RecallCur.IsDisabled)//or any of the three disabled fields was changed
				|| (recall.DisableUntilDate != FormR.RecallCur.DisableUntilDate)
				|| (recall.DisableUntilBalance != FormR.RecallCur.DisableUntilBalance)
				|| (recall.Note != FormR.RecallCur.Note))//or a note was added
			{
				//make a commlog entry
				//unless there is an existing recall commlog entry for today
				bool recallEntryToday=false;
				List<Commlog> CommlogList=Commlogs.Refresh(selectedPatNum);
				for(int i=0;i<CommlogList.Count;i++) {
					if(CommlogList[i].CommDateTime.Date==DateTime.Today	
						&& CommlogList[i].CommType==Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL)) {
						recallEntryToday=true;
					}
				}
				if(!recallEntryToday) {
					Commlog commlogCur=new Commlog();
					commlogCur.CommDateTime=DateTime.Now;
					commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.RECALL);
					commlogCur.PatNum=selectedPatNum;
					commlogCur.Note="";
					if(recall.RecallStatus!=FormR.RecallCur.RecallStatus) {
						if(FormR.RecallCur.RecallStatus==0) {
							commlogCur.Note+=Lan.g(this,"Status None");
						}
						else {
							commlogCur.Note+=Defs.GetName(DefCat.RecallUnschedStatus,FormR.RecallCur.RecallStatus);
						}
					}
					if(recall.DisableUntilDate!=FormR.RecallCur.DisableUntilDate && FormR.RecallCur.DisableUntilDate.Year>1880) {
						if(commlogCur.Note!="") {
							commlogCur.Note+=",  ";
						}
						commlogCur.Note+=Lan.g(this,"Disabled until ")+FormR.RecallCur.DisableUntilDate.ToShortDateString();
					}
					if(recall.DisableUntilBalance!=FormR.RecallCur.DisableUntilBalance && FormR.RecallCur.DisableUntilBalance>0) {
						if(commlogCur.Note!="") {
							commlogCur.Note+=",  ";
						}
						commlogCur.Note+=Lan.g(this,"Disabled until balance below ")+FormR.RecallCur.DisableUntilBalance.ToString("c");
					}
					if(recall.Note!=FormR.RecallCur.Note) {
						if(commlogCur.Note!="") {
							commlogCur.Note+=",  ";
						}
						commlogCur.Note+=FormR.RecallCur.Note;
					}
					commlogCur.Note+=".  ";
					commlogCur.UserNum=Security.CurUser.UserNum;
					commlogCur.IsNew=true;
					FormCommItem FormCI=new FormCommItem(commlogCur);
					FormCI.ShowDialog();
				}
			}
			FillMain();
			for(int i=0;i<gridMain.Rows.Count;i++) {
				if(((PatRowTag)gridMain.Rows[i].Tag).PatNum==selectedPatNum){
					gridMain.SetSelected(i,true);
				}
			}
			SetFamilyColors();
		}

		///<summary>Creates a recall appointment and returns the AptNum in a list.  Validation should be done prior to invoking this method.
		///Shows an error message to the user and then returns an empty list if anything goes wrong.</summary>
		private List<long> SchedPatRecall(long recallNum,Patient pat,List<InsSub> subList,List<InsPlan> planList) {
			try {
				if(Recalls.HasProphyOrPerioScheduled(_patNumCur)) {
					MsgBox.Show(this,"Recall has already been scheduled.");
					_gridCur.FillGrid();
					return new List<long>();
				}
				if(PatRestrictionL.IsRestricted(pat.PatNum,PatRestrict.ApptSchedule)) {
					return new List<long>();
				}
				Appointment apt=AppointmentL.CreateRecallApt(pat,planList,recallNum,subList);
				return new List<long>() { apt.AptNum };
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return new List<long>();
			}
		}

		///<summary>Creates recall appointments for the family and returns a list of all the AptNums.
		///Validation should be done prior to invoking this method.
		///Shows a message to the user if there were any restricted patients or no appointments created.</summary>
		public List<long> SchedFamRecall(Family fam,List<InsSub> subList,List<InsPlan> planList) {
			Appointment apt;
			List<long> pinAptNums=new List<long>();
			int patsRestricted=0;
			List<Recall> listFamRecalls=Recalls.GetList(fam.ListPats.Select(x => x.PatNum).ToList());
			bool doRefreshGrid=false;
			for(int i=0;i<fam.ListPats.Length;i++) {
				if(PatRestrictionL.IsRestricted(fam.ListPats[i].PatNum,PatRestrict.ApptSchedule,true)) {
					patsRestricted++;
					continue;
				}
				if(Recalls.HasProphyOrPerioScheduled(fam.ListPats[i].PatNum,listFamRecalls)) {
					doRefreshGrid=true;
					continue;
				}
				try{
					//Passes in -1 as the RecallNum. This will create an appointment for either a Perio or Prophy recall type only. 
					apt=AppointmentL.CreateRecallApt(fam.ListPats[i],planList,-1,subList);
				}
				catch(Exception ex) {
					ex.DoNothing();
					continue;
				}
				pinAptNums.Add(apt.AptNum);
			}
			if(patsRestricted>0) {
				MessageBox.Show(Lan.g(this,"Family members skipped due to patient restriction")+" "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)
					+": "+patsRestricted+".");
			}
			if(pinAptNums.Count==0) {
				MsgBox.Show(this,"No recall is due.");
				if(doRefreshGrid) {
					_gridCur.FillGrid();
				}
				return new List<long>();
			}
			return pinAptNums;
		}
		
		///<summary>Automatically open the eService Setup window so that they can easily click the Enable button. 
		///Calls CheckClinicsSignedUpForWebSched() before exiting.</summary>
		private void OpenSignupPortal() {
			FormEServicesSetup FormESS=new FormEServicesSetup(FormEServicesSetup.EService.SignupPortal);
			FormESS.ShowDialog();
			//User may have made changes to signups. Reload the valid clinics from HQ.
			CheckClinicsSignedUpForWebSched();
		}

		private void panelWebSched_MouseClick(object sender,MouseEventArgs e) {
			SendWebSched();
		}

		private void SendWebSched() {
			#region Check Web Sched Pref and Show Promo
			if(IsDisposed) {//The user closed the form while the thread checking Web Sched signups was still running.
				return;
			}
			if(!_isDoneCheckingWebSchedClinics) {//The thread has not finished getting the list. 
				_hasClickedWebSched=true;//The thread checking Web Sched signups will call this method on exit.
				Cursor=Cursors.AppStarting;
				return;
			}
			if(_isOnSupport!=YN.Yes) {
				MsgBox.Show(this,"You must be on support to use this feature.");
				return;
			}
			bool needsToBeSignedUp=WebSchedRecalls.TemplatesHaveURLTags();
			if(needsToBeSignedUp && _listClinicNumsWebSched.Count==0) {//No clinics are signed up for Web Sched
				string message=PrefC.HasClinicsEnabled ?
					"No clinics are signed up for Web Sched Recall. Open Sign Up Portal?" : 
					"This practice is not signed up for Web Sched Recall. Open Sign Up Portal?";
				message+="\r\n\r\nAlternatively, you could remove all URL tags from Web Sched text and emails templates to use this feature.";
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
					return;
				}
				OpenSignupPortal();
				return;
			}
			//At least one clinic is signed up for Web Sched or the templates are not using URLs.
			List<long> listClinicNumsNotSignedUp=new List<long>();
			List<int> listGridIndicesNotSignUp=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				long clinicNum=PIn.Long(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["ClinicNum"].ToString());
				//We don't want to send users to the sign up portal for clinic 0 if clinics are enabled because there will be nothing for them to do there. 
				if(clinicNum==0 && !_listClinicNumsWebSched.Contains(0)) {
					continue;//We will deselect these rows later.
				}
				if(_listClinicNumsWebSched.Count > 0 && !PrefC.HasClinicsEnabled) {
					//The office is signed up for Web Sched, but the patient's clinic might not be 0.
					continue;//We will let them send for this patient.
				}
				if(!_listClinicNumsWebSched.Contains(clinicNum)) {
					listClinicNumsNotSignedUp.Add(clinicNum);
				}
				listGridIndicesNotSignUp.Add(i);
			}
			if(needsToBeSignedUp && listClinicNumsNotSignedUp.Count > 0) {
				string message=Lan.g(this,"You have selected recalls whose clinic is not signed up for Web Sched recall. "
					+"Do you want to go to the sign up portal to sign these clinics up? "
					+"Clicking 'No' will deselect these recalls and send the remaining.");
				if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)==DialogResult.Yes) {
					OpenSignupPortal();
					return;
				}
				//De-select any rows that are not allowed to send WebSched.				
				for(int i=0;i<listGridIndicesNotSignUp.Count;i++) {
					gridMain.SetSelected(listGridIndicesNotSignUp[i],false);
				}
			}
			#endregion
			#region Recall List Validation
			if(gridMain.Rows.Count < 1) {
				MessageBox.Show(Lan.g(this,"There are no Patients in the Recall table.  Must have at least one."));
				return;
			}
			if(!EmailAddresses.ExistsValidEmail()) {
				MsgBox.Show(this,"You need to enter an SMTP server name in email setup before you can send email.");
				return;
			}
			if(PrefC.GetLong(PrefName.RecallStatusEmailed)==0
				|| PrefC.GetLong(PrefName.RecallStatusTexted)==0
				|| PrefC.GetLong(PrefName.RecallStatusEmailedTexted)==0) 
			{
				MsgBox.Show(this,"You need to set an email status, text status, and email and text status first in the Recall Setup window.");
				return;
			}
#if !DEBUG
			if(EServiceSignals.GetListenerServiceStatus().In(
				eServiceSignalSeverity.None,
				eServiceSignalSeverity.NotEnabled,
				eServiceSignalSeverity.Critical)) 
			{
				MsgBox.Show(this,"Your eConnector is not currently running. Please enable the eConnector to send Web Sched Recalls.");
				return;
			}
#endif
			//If the user didn't manually select any recalls we will automatically select all rows that have an email or text prefer recall method.
			if(gridMain.SelectedIndices.Length==0) {
				ContactMethod cmeth;
				for(int i=0;i<_tableRecalls.Rows.Count;i++) {
					cmeth=(ContactMethod)PIn.Long(_tableRecalls.Rows[i]["PreferRecallMethod"].ToString());
					if(!cmeth.In(ContactMethod.Email,ContactMethod.TextMessage)) {
						continue;
					}
					gridMain.SetSelected(i,true);
				}
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No patients prefer contact via email or text.");
				return;
			}
			//Now that there are rows guaranteed to be selected, check each row to see if their recall will yield available Web Sched time slots.
			//Deselect the ones that do not have email or wireless phone specified or are assigned to clinic num 0 when clinics are enabled.
			//Also deselect ones that were just sent but are still in the list because the grid hasn't refreshed yet.
			int skippedContact=0;
			int skippedTimeSlot=0;
			int skippedClinic0=0;
			int skippedNotInList=0;
			DataTable tableRecallsCur=_getRecallTable();
			List<long> listPatNumsInTableRecallCur=tableRecallsCur.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList();
			for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--) {
				DataRow row=_tableRecalls.Rows[gridMain.SelectedIndices[i]];
				//Check that they at least have an email or wireless phone.
				string email=_tableRecalls.Rows[gridMain.SelectedIndices[i]]["Email"].ToString();
				string wirelessPhone=_tableRecalls.Rows[gridMain.SelectedIndices[i]]["WirelessPhone"].ToString();
				if(email.Trim()=="" && wirelessPhone.Trim()=="") {
					skippedContact++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				long clinicNum=PIn.Long(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["ClinicNum"].ToString());
				//If this practice has clinics enabled for Web Sched, then they will not have Web Sched enabled for clinic num 0. They will need to assign
				//patients to a clinic in order to send Web Sched. We will prompt them below to assign them.
				if(clinicNum==0 && !_listClinicNumsWebSched.Contains(0)) {
					skippedClinic0++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				//Check that the patient is still in the recall list (they haven't been sent something since the grid has been refreshed)
				long patNum=PIn.Long(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["PatNum"].ToString());
				if(!patNum.In(listPatNumsInTableRecallCur)) {
					skippedNotInList++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				AutoCommStatus smsSendStatus=PIn.Enum<AutoCommStatus>(PIn.Int(row["webSchedSmsSendStatus"].ToString()));
				AutoCommStatus emailSendStatus=PIn.Enum<AutoCommStatus>(PIn.Int(row["webSchedEmailSendStatus"].ToString()));
				//The eConnector will attempt to send a webschedrecall if either SmsSendStatus or EmailSendStatus is SendNotAttempted and neither of them
				//are SendFailed or SendSuccessful.
				if((smsSendStatus==AutoCommStatus.SendNotAttempted || emailSendStatus==AutoCommStatus.SendNotAttempted)
					&& smsSendStatus.In(AutoCommStatus.SendNotAttempted,AutoCommStatus.DoNotSend)
					&& emailSendStatus.In(AutoCommStatus.SendNotAttempted,AutoCommStatus.DoNotSend))
				{
					continue;//The eConnector is about to send this anyway.
				}
				//Check to see if they'll have any potential time slots via their Web Sched link.
				long recallNum=PIn.Long(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["RecallNum"].ToString());
				DateTime dateDue=PIn.Date(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["DateDue"].ToString());
				if(dateDue.Date<=DateTime.Now.Date) {
					dateDue=DateTime.Now;
				}
				DateTime dateEnd=dateDue.AddMonths(2);
				//This takes a long time to run for lots of recalls.  Might consider making a faster overload in the future (213 recalls ~ 10 seconds).
				bool hasTimeSlots=false;
				try {
					if(needsToBeSignedUp) {
						hasTimeSlots=(OpenDentBusiness.WebTypes.WebSched.TimeSlot.TimeSlots.GetAvailableWebSchedTimeSlots(recallNum,dateDue,dateEnd).Count > 0);
					}
					else {
						hasTimeSlots=true;
					}
				}
				catch(Exception) {
				}
				if(!hasTimeSlots) {
					skippedTimeSlot++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
			}
			string skippedMessage="";
			if(skippedContact > 0) {
				skippedMessage+=Lan.g(this,"Selected patients skipped due to missing email addresses and wireless phone:")+" "
					+skippedContact.ToString()+"\r\n";
			}
			if(skippedTimeSlot > 0) {
				skippedMessage+=Lan.g(this,"Selected patients skipped due to no available Web Sched time slots found:")+" "
					+skippedTimeSlot.ToString()+"\r\n";
			}
			if(skippedClinic0 > 0) {
				skippedMessage+=Lan.g(this,"Selected patients skipped due to not being assigned to a clinic:")+" "
					+skippedClinic0.ToString()+"\r\n";
			}
			if(skippedNotInList > 0) {
				FillMain();
				skippedMessage+=Lan.g(this,"Selected patients skipped due to no longer being in the recall list:")+" "
					+skippedNotInList.ToString()+"\r\n";
			}
			if(skippedMessage != "") {
				MessageBox.Show(skippedMessage);
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No Web Sched emails or texts sent.");
				return;
			}
#endregion
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Send Web Sched emails and/or texts to all of the selected patients?")) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			List<long> recallNums=new List<long>();
			//Loop through all selected patients and grab their corresponding RecallNum.
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				recallNums.Add(PIn.Long(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["RecallNum"].ToString()));
			}
			EmailAddress emailAddressFrom;
			if(comboEmailFrom.SelectedIndex==0) {
				//clinic/practice default. Set to null here to pull patient's clinic email address in Recalls.SendWebSchedNotifications().
				emailAddressFrom=null;
			}
			else { //me or static email address, email address for 'me' is the first one in _listEmailAddresses
				emailAddressFrom=_listEmailAddresses[comboEmailFrom.SelectedIndex-1];//-1 to account for predefined "Clinic/Practice" items in combobox
			}
			List<string> listWebSchedErrors=Recalls.PrepWebSchedNotifications(recallNums
				,checkGroupFamilies.Checked
				,(RecallListSort)comboSort.SelectedIndex
				,WebSchedRecallSource.FormRecallList
				,emailAddressFrom);
			Cursor=Cursors.Default;
			if(listWebSchedErrors.Count > 0) {
				//Show the error (already translated) to the user and then refresh the grid in case any were successful.
				MsgBoxCopyPaste msgBCP=new MsgBoxCopyPaste(string.Join("\r\n",listWebSchedErrors));
				msgBCP.Show();
			}
			FillMain();
		}

		private void checkGroupFamilies_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			_gridCur.FillGrid();
			Cursor=Cursors.Default;
		}

		private void checkRecallTypes_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillMain();
			Cursor=Cursors.Default;
		}

		private void butReport_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
		  if(gridMain.Rows.Count < 1){
        MessageBox.Show(Lan.g(this,"There are no Patients in the Recall table.  Must have at least one to run report."));    
        return;
      }
			List<long> recallNums=new List<long>();
      if(gridMain.SelectedIndices.Length < 1){
        for(int i=0;i<gridMain.Rows.Count;i++){
          recallNums.Add(PIn.Long(_tableRecalls.Rows[i]["RecallNum"].ToString()));
        }
      }
      else{
        for(int i=0;i<gridMain.SelectedIndices.Length;i++){
          recallNums.Add(PIn.Long(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["RecallNum"].ToString()));
        }
      }
      FormRpRecall FormRPR=new FormRpRecall(recallNums);
      FormRPR.ShowDialog();
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butLabels_Click(object sender, System.EventArgs e) {
			if(!IsGridEmpty() && IsStatusSet(PrefName.RecallStatusMailed,PrefName.ReactivationStatusMailed) && IsAnyPatToContact("labels",ContactMethod.Mail)) {
				CommItemTypeAuto commType=IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT;
				addrTable=GetAddrTable();
				pagesPrinted=0;
				patientsPrinted=0;
				PrinterL.TryPreview(pdLabels_PrintPage,
					Lan.g(this,(commType==CommItemTypeAuto.RECALL?"Recall":"Reactivation")+" list labels printed"),
					PrintSituation.LabelSheet,
					new Margins(0,0,0,0),
					PrintoutOrigin.AtMargin,
					totalPages:(int)Math.Ceiling((double)addrTable.Rows.Count/30)
				);
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change statuses and make commlog entries for all of the selected patients?")) {
					ProcessComms(commType);
				}
				_gridCur.FillGrid();
				Cursor=Cursors.Default;
			}
		}		

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butLabelOne_Click(object sender,EventArgs e) {
			if(IsAnyRowSelected()) {
				CommItemTypeAuto commType=IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT;
				addrTable=GetAddrTable();
				patientsPrinted=0;
				string text;
				while(patientsPrinted<addrTable.Rows.Count) {
					text="";
					if(DoGroupFamilies() && addrTable.Rows[patientsPrinted]["famList"].ToString()!="") {//print family label
						text=addrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
					}
					else {//print single label
						text=addrTable.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
					}
					text+=addrTable.Rows[patientsPrinted]["address"].ToString()+"\r\n";
					text+=addrTable.Rows[patientsPrinted]["City"].ToString()+", "
						+addrTable.Rows[patientsPrinted]["State"].ToString()+" "
						+addrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
					LabelSingle.PrintText(0,text);
					patientsPrinted++;
				}
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Did all the labels finish printing correctly?  Statuses will be changed and commlog entries made for all of the selected patients.  Click Yes only if labels printed successfully.")) {
					ProcessComms(commType);
				}
				_gridCur.FillGrid();
				Cursor=Cursors.Default;
			}
		}

		///<summary>Changes made to printing recall postcards need to be made in FormConfirmList.butPostcards_Click() as well.
		///Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butPostcards_Click(object sender,System.EventArgs e) {
			if(!IsGridEmpty() && IsStatusSet(PrefName.RecallStatusMailed,PrefName.ReactivationStatusMailed) && IsAnyPatToContact("postcards",ContactMethod.Mail)) {
				CommItemTypeAuto commType=IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT;
				addrTable=GetAddrTable();
				pagesPrinted=0;
				patientsPrinted=0;
				PaperSize paperSize;
				PrintoutOrientation orient=PrintoutOrientation.Default;
				long postcardsPerSheet=PrefC.GetLong(commType==CommItemTypeAuto.RECALL?PrefName.RecallPostcardsPerSheet:PrefName.ReactivationPostcardsPerSheet);
				if(postcardsPerSheet==1) {
					paperSize=new PaperSize("Postcard",500,700);
					orient=PrintoutOrientation.Landscape;
				}
				else if(postcardsPerSheet==3) {
					paperSize=new PaperSize("Postcard",850,1100);
				}
				else {//4
					paperSize=new PaperSize("Postcard",850,1100);
					orient=PrintoutOrientation.Landscape;
				}
				int totalPages=(int)Math.Ceiling((double)addrTable.Rows.Count/(double)postcardsPerSheet);
				PrinterL.TryPreview(pdCards_PrintPage,
					Lan.g(this,(commType==CommItemTypeAuto.RECALL?"Recall":"Reactivation")+" list postcards printed"),
					PrintSituation.Postcard,
					new Margins(0,0,0,0),
					PrintoutOrigin.AtMargin,
					paperSize,
					orient,
					totalPages
				);
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Did all the postcards finish printing correctly?  Statuses will be changed and commlog entries made for all of the selected patients.  Click Yes only if postcards printed successfully.")) {
					ProcessComms(commType);
				}
				_gridCur.FillGrid();
				Cursor=Cursors.Default;
			}
		}

		private void butUndo_Click(object sender,EventArgs e) {
			FormRecallListUndo form=new FormRecallListUndo();
			form.ShowDialog();
			if(form.DialogResult==DialogResult.OK) {
				FillMain();
			}
		}

		private void butECards_Click(object sender,EventArgs e) {
			if(!Programs.IsEnabled(ProgramName.Divvy)) {
				if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"The Divvy Program Link is not enabled. Would you like to enable it now?")) {
					FormProgramLinkEdit FormPE=new FormProgramLinkEdit();
					FormPE.ProgramCur=Programs.GetCur(ProgramName.Divvy);
					FormPE.ShowDialog();
					DataValid.SetInvalid(InvalidType.Programs);
				}
				if(!Programs.IsEnabled(ProgramName.Divvy)) {
					return;
				}
			}
			if(gridMain.Rows.Count < 1) {
				MessageBox.Show(Lan.g(this,"There are no Patients in the Recall table.  Must have at least one to send."));
				return;
			}
			if(PrefC.GetLong(PrefName.RecallStatusMailed)==0) {
				MsgBox.Show(this,"You need to set a status first in the Recall Setup window.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				ContactMethod cmeth;
				for(int i=0;i<_tableRecalls.Rows.Count;i++) {
					cmeth=(ContactMethod)PIn.Long(_tableRecalls.Rows[i]["PreferRecallMethod"].ToString());
					if(cmeth!=ContactMethod.Mail && cmeth!=ContactMethod.None) {
						continue;
					}
					gridMain.SetSelected(i,true);
				}
				if(gridMain.SelectedIndices.Length==0) {
					MsgBox.Show(this,"No patients of mail type.");
					return;
				}
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Send postcards for all of the selected patients?")) {
				return;
			}
			RecallListSort sortBy=(RecallListSort)comboSort.SelectedIndex;
			List<long> recallNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				recallNums.Add(PIn.Long(_tableRecalls.Rows[gridMain.SelectedIndices[i]]["RecallNum"].ToString()));
			}
			addrTable=Recalls.GetAddrTable(recallNums,checkGroupFamilies.Checked,sortBy);
			DivvyConnect.Postcard postcard;
			DivvyConnect.Recipient recipient;
			DivvyConnect.Postcard[] listPostcards=new DivvyConnect.Postcard[gridMain.SelectedIndices.Length];
			string message;
			long clinicNum;
			Clinic clinic;
			string phone;
			for(int i=0;i<addrTable.Rows.Count;i++) {
				postcard=new DivvyConnect.Postcard();
				recipient=new DivvyConnect.Recipient();
				recipient.Name=addrTable.Rows[i]["patientNameFL"].ToString();
				recipient.ExternalRecipientID=addrTable.Rows[i]["patNums"].ToString();
				recipient.Address1=addrTable.Rows[i]["Address"].ToString();//Includes Address2
				recipient.City=addrTable.Rows[i]["City"].ToString();
				recipient.State=addrTable.Rows[i]["State"].ToString();
				recipient.Zip=addrTable.Rows[i]["Zip"].ToString();
				postcard.AppointmentDateTime=PIn.Date(addrTable.Rows[i]["dateDue"].ToString());//js I don't know why they would ask for this.  We put this in our message.
				//Body text, family card ------------------------------------------------------------------
				if(checkGroupFamilies.Checked	&& addrTable.Rows[i]["famList"].ToString()!=""){
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg2);
					}
					else {
						message=PrefC.GetString(PrefName.RecallPostcardFamMsg3);
					}
					message=message.Replace("[FamilyList]",addrTable.Rows[i]["famList"].ToString());
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						message=PrefC.GetString(PrefName.RecallPostcardMessage);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						message=PrefC.GetString(PrefName.RecallPostcardMessage2);
					}
					else {
						message=PrefC.GetString(PrefName.RecallPostcardMessage3);
					}
					message=message.Replace("[DueDate]",addrTable.Rows[i]["dateDue"].ToString());
					message=message.Replace("[NameF]",addrTable.Rows[i]["patientNameF"].ToString());
					message=message.Replace("[NameFL]", addrTable.Rows[i]["patientNameFL"].ToString());
				}
				Clinic clinicCur=Clinics.GetClinicForRecall(PIn.Long(addrTable.Rows[i]["recallNums"].ToString().Split(',').FirstOrDefault()));
				message=message.Replace("[ClinicName]",clinicCur.Abbr);
				message=message.Replace("[ClinicPhone]",clinicCur.Phone);
				message=message.Replace("[PracticeName]",PrefC.GetString(PrefName.PracticeTitle));
				message=message.Replace("[PracticePhone]",PrefC.GetString(PrefName.PracticePhone));
				string officePhone=clinicCur.Phone;
				if(string.IsNullOrEmpty(officePhone)) {
					officePhone=PrefC.GetString(PrefName.PracticePhone);
				}
				message=message.Replace("[OfficePhone]",clinicCur.Phone);
				postcard.Message=message;
				postcard.Recipient=recipient;
				postcard.DesignID=PIn.Int(ProgramProperties.GetPropVal(ProgramName.Divvy,"DesignID for Recall Cards"));
				listPostcards[i]=postcard;
			}
			DivvyConnect.Practice practice=new DivvyConnect.Practice();
			clinicNum=PIn.Long(addrTable.Rows[patientsPrinted]["ClinicNum"].ToString());
			if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0 //if using clinics
				&& Clinics.GetClinic(clinicNum)!=null)//and this patient assigned to a clinic
			{
				clinic=Clinics.GetClinic(clinicNum);
				practice.Company=clinic.Description;
				practice.Address1=clinic.Address;
				practice.Address2=clinic.Address2;
				practice.City=clinic.City;
				practice.State=clinic.State;
				practice.Zip=clinic.Zip;
				phone=clinic.Phone;
			}
			else {
				practice.Company=PrefC.GetString(PrefName.PracticeTitle);
				practice.Address1=PrefC.GetString(PrefName.PracticeAddress);
				practice.Address2=PrefC.GetString(PrefName.PracticeAddress2);
				practice.City=PrefC.GetString(PrefName.PracticeCity);
				practice.State=PrefC.GetString(PrefName.PracticeST);
				practice.Zip=PrefC.GetString(PrefName.PracticeZip);
				phone=PrefC.GetString(PrefName.PracticePhone);
			}
			practice.Phone=TelephoneNumbers.ReFormat(phone);
			DivvyConnect.PostcardServiceClient client=new DivvyConnect.PostcardServiceClient();
			DivvyConnect.PostcardReturnMessage returnMessage=new DivvyConnect.PostcardReturnMessage();
			string messages="";
			Cursor=Cursors.WaitCursor;
			try {
				returnMessage=client.SendPostcards(
				  Guid.Parse(ProgramProperties.GetPropVal(ProgramName.Divvy,"API Key")),
				  ProgramProperties.GetPropVal(ProgramName.Divvy,"Username"),
				  ProgramProperties.GetPropVal(ProgramName.Divvy,"Password"),
				  listPostcards,practice);
			}
			catch (Exception ex) {
				messages+="Exception: "+ex.Message+"\r\nData: "+ex.Data+"\r\n";
			}
			messages+="MessageCode: "+returnMessage.MessageCode.ToString();//MessageCode enum. 0=CompletedSuccessfully, 1=CompletedWithErrors, 2=Failure
			MsgBox.Show(this,"Return Messages: "+returnMessage.Message+"\r\n"+messages);
			if(returnMessage.MessageCode==DivvyConnect.MessageCode.CompletedSucessfully) {
				Cursor=Cursors.WaitCursor;
				ProcessComms((IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT),CommItemMode.Mail);
			}
			else if(returnMessage.MessageCode==DivvyConnect.MessageCode.CompletedWithErrors) {
				for(int i=0;i<returnMessage.PostcardMessages.Length;i++) {
					//todo: process return messages. Update commlog and change recall statuses for postcards that were sent.
				}
			}
			FillMain();
			Cursor=Cursors.Default;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butEmail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)) {
				return;
			}
			if(IsGridEmpty()) {
				return;
			}
			if(!EmailAddresses.ExistsValidEmail()) {
				MsgBox.Show(this,"You need to enter an SMTP server name in e-mail setup before you can send e-mail.");
				return;
			}
			if(!IsStatusSet(PrefName.RecallStatusEmailed,PrefName.ReactivationStatusEmailed)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				if(!IsAnyPatToContact("email",ContactMethod.Email,false)) {
					//deselect the ones that do not have email addresses specified
					int skipped=0;
					for(int i=_gridCur.SelectedIndices.Length-1;i>=0;i--) {
						PatRowTag tag=(PatRowTag)_gridCur.Rows[i].Tag;
						if(string.IsNullOrWhiteSpace(tag.Email)) {
							skipped++;
							_gridCur.SetSelected(_gridCur.SelectedIndices[i],false);
						}
					}
					if(_gridCur.SelectedIndices.Length==0){
						MsgBox.Show(this,"None of the selected patients had email addresses entered.");
						return;
					}
					if(skipped>0){
						MessageBox.Show(Lan.g(this,"Selected patients skipped due to missing email addresses: ")+skipped.ToString());
					}
				}
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Send email to all of the selected patients?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			addrTable=GetAddrTable();
			//Email
			EmailMessage message;
			string str="";
			EmailAddress emailAddress;
			int sentEmailCount=0;
			for(int i=0;i<addrTable.Rows.Count;i++){
				message=new EmailMessage();
				message.PatNum=PIn.Long(addrTable.Rows[i]["emailPatNum"].ToString());
				message.ToAddress=PIn.String(addrTable.Rows[i]["email"].ToString());//might be guarantor email
				Clinic clinicCur;
				if(IsRecallGridSelected()) {
					clinicCur=Clinics.GetClinicForRecall(PIn.Long(addrTable.Rows[i]["recallNums"].ToString().Split(',').FirstOrDefault()));
				}
				else {
					clinicCur=Clinics.GetClinic(PIn.Long(addrTable.Rows[i]["ClinicNum"].ToString()));
				}
				long clinicNumEmail=clinicCur?.ClinicNum??Clinics.ClinicNum;
				ComboBox cbEmail=IsRecallGridSelected()?comboEmailFrom:comboReactEmailFrom;
				if(cbEmail.SelectedIndex==0) { //clinic/practice default
					clinicNumEmail=PIn.Long(addrTable.Rows[i]["ClinicNum"].ToString());
					emailAddress=EmailAddresses.GetByClinic(clinicNumEmail);
				}
				else { //me or static email address, email address for 'me' is the first one in _listEmailAddresses
					emailAddress=_listEmailAddresses[cbEmail.SelectedIndex-1];//-1 to account for predefined "Clinic/Practice" item in combobox
				}
				message.FromAddress=emailAddress.GetFrom();
				if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
					message.Subject=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailSubject:PrefName.ReactivationEmailSubject);
				}
				else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
					message.Subject=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailSubject2:PrefName.ReactivationEmailSubject);
				}
				else {
					message.Subject=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailSubject3:PrefName.ReactivationEmailSubject);
				}
				//family
				if(DoGroupFamilies() && addrTable.Rows[i]["famList"].ToString()!="") {
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						str=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailFamMsg:PrefName.ReactivationEmailFamMsg);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						str=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailFamMsg2:PrefName.ReactivationEmailFamMsg);
					}
					else {
						str=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailFamMsg3:PrefName.ReactivationEmailFamMsg);
					}
					str=str.Replace("[FamilyList]",addrTable.Rows[i]["famList"].ToString());
				}
				//single
				else {
					if(addrTable.Rows[i]["numberOfReminders"].ToString()=="0") {
						str=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailMessage:PrefName.ReactivationEmailMessage);
					}
					else if(addrTable.Rows[i]["numberOfReminders"].ToString()=="1") {
						str=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailMessage2:PrefName.ReactivationEmailMessage);
					}
					else {
						str=PrefC.GetString(IsRecallGridSelected()?PrefName.RecallEmailMessage3:PrefName.ReactivationEmailMessage);
					}
					str=str.Replace("[DueDate]",PIn.Date(addrTable.Rows[i]["dateDue"].ToString()).ToShortDateString());
					str=str.Replace("[NameF]",addrTable.Rows[i]["patientNameF"].ToString());
					str=str.Replace("[NameFL]",addrTable.Rows[i]["patientNameFL"].ToString());
				}
				string officePhone="";
				string mainPhone=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
				if(clinicCur==null) {
					str=str.Replace("[ClinicName]",PrefC.GetString(PrefName.PracticeTitle));
					str=str.Replace("[ClinicPhone]",mainPhone);
					officePhone=mainPhone;
				}
				else {
					str=str.Replace("[ClinicName]",clinicCur.Abbr);
					str=str.Replace("[ClinicPhone]",TelephoneNumbers.ReFormat(clinicCur.Phone));
					officePhone=clinicCur.Phone;
				}
				str=str.Replace("[PracticeName]",PrefC.GetString(PrefName.PracticeTitle));
				str=str.Replace("[PracticePhone]",mainPhone);
				str=str.Replace("[OfficePhone]",officePhone);
				message.BodyText=EmailMessages.FindAndReplacePostalAddressTag(str,clinicNumEmail);
				try{
					EmailMessages.SendEmailUnsecure(message,emailAddress);
					sentEmailCount++;
				}
				catch(Exception ex){
					Cursor=Cursors.Default;
					str=ex.Message+"\r\n";
					if(ex.GetType()==typeof(System.ArgumentException)){
						str+=$"Go to Setup, {(IsRecallGridSelected()?"Recall":"Reactivation")}.  The subject for an email may not be multiple lines.\r\n";
					}
					MessageBox.Show(str+"Patient:"+addrTable.Rows[i]["patientNameFL"].ToString());
					break;
				}
				message.MsgDateTime=DateTime.Now;
				message.SentOrReceived=EmailSentOrReceived.Sent;
				EmailMessages.Insert(message);
				ProcessComms(IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT,CommItemMode.Email);
			}
			_gridCur.FillGrid();
			if(sentEmailCount>0) {
				SecurityLogs.MakeLogEntry(Permissions.EmailSend,0,$"{(IsRecallGridSelected()?"Recall":"Reactivation")} Emails Sent: "+sentEmailCount);
			}
			Cursor=Cursors.Default;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void ProcessComms(CommItemTypeAuto commType,CommItemMode mode=CommItemMode.Mail) {
			Cursor=Cursors.WaitCursor;
			long status=mode==CommItemMode.Mail?_statusMailed:_statusEmailed;
			foreach(ODGridRow row in _gridCur.SelectedGridRows) {
				PatRowTag tag=(PatRowTag)row.Tag;
				Commlogs.InsertForRecallOrReactivation(tag.PatNum,mode,tag.NumReminders,status,commType);
				if(commType==CommItemTypeAuto.RECALL) { //RECALL
					Recalls.UpdateStatus(tag.PatNum,status);
				}
				else { //REACTIVATION
					Reactivations.UpdateStatus(tag.PriKeyNum,status);
				}
			}
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdLabels_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)addrTable.Rows.Count/30);
			Graphics g=ev.Graphics;
			float yPos=63;//75;
			float xPos=50;
			string text="";
			while(yPos<1000 && patientsPrinted<addrTable.Rows.Count){
				text="";
				if(DoGroupFamilies() && addrTable.Rows[patientsPrinted]["famList"].ToString()!=""){//print family label
					text=addrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else {//print single label
					text=addrTable.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
				}
				text+=addrTable.Rows[patientsPrinted]["address"].ToString()+"\r\n";
				text+=addrTable.Rows[patientsPrinted]["City"].ToString()+", "
					+addrTable.Rows[patientsPrinted]["State"].ToString()+" "
					+addrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				Rectangle rect=new Rectangle((int)xPos,(int)yPos,275,100);
				MapAreaRoomControl.FitText(text,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,rect,new StringFormat(),g);
				//reposition for next label
				xPos+=275;
				if(xPos>850){//drop a line
					xPos=50;
					yPos+=100;
				}
				patientsPrinted++;
			}
			pagesPrinted++;
			if(pagesPrinted==totalPages){
				ev.HasMorePages=false;
				pagesPrinted=0;//because it has to print again from the print preview
				patientsPrinted=0;
			}
			else{
				ev.HasMorePages=true;
			}
			g.Dispose();
		}
	
		///<summary>raised for each page to be printed.</summary>
		private void pdCards_PrintPage(object sender, PrintPageEventArgs ev){
			long postCardsPerSheet=PrefC.GetLong(IsRecallGridSelected()?PrefName.RecallPostcardsPerSheet:PrefName.ReactivationPostcardsPerSheet);
			int totalPages=(int)Math.Ceiling((double)addrTable.Rows.Count/(double)postCardsPerSheet);
			Graphics g=ev.Graphics;
			int yAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustDown)*100);
			int xAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustRight)*100);
			float yPos=0+yAdj;//these refer to the upper left origin of each postcard
			float xPos=0+xAdj;
			const int bottomPageMargin=100;
			long clinicNum;
			Clinic clinic;
			string str;
			while(yPos<ev.PageBounds.Height-bottomPageMargin && patientsPrinted<addrTable.Rows.Count){
				//Return Address--------------------------------------------------------------------------
				clinicNum=PIn.Long(addrTable.Rows[patientsPrinted]["ClinicNum"].ToString());
				string practicePhone=TelephoneNumbers.ReFormat(PrefC.GetString(PrefName.PracticePhone));
				if(PrefC.GetBool(PrefName.RecallCardsShowReturnAdd)){
					if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0 //if using clinics
						&& Clinics.GetClinic(clinicNum)!=null)//and this patient assigned to a clinic
					{
						clinic=Clinics.GetClinic(clinicNum);
						str=clinic.Description+"\r\n";
						g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,xPos+45,yPos+60);
						str=clinic.Address+"\r\n";
						if(clinic.Address2!="") {
							str+=clinic.Address2+"\r\n";
						}
						str+=clinic.City+",  "+clinic.State+"  "+clinic.Zip+"\r\n";
						str+=TelephoneNumbers.ReFormat(clinic.Phone);
					}
					else {
						str=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
						g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,xPos+45,yPos+60);
						str=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
						if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
							str+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
						}
						str+=PrefC.GetString(PrefName.PracticeCity)+",  "+PrefC.GetString(PrefName.PracticeST)+"  "+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
						str+=practicePhone;
					}
					g.DrawString(str,new Font(FontFamily.GenericSansSerif,8),Brushes.Black,xPos+45,yPos+75);
				}
				//Body text, family card ------------------------------------------------------------------
				if(DoGroupFamilies()	&& addrTable.Rows[patientsPrinted]["famList"].ToString()!=""){
					str=GetPostcardMessage(addrTable.Rows[patientsPrinted]["numberOfReminders"].ToString(),isFam:true);
					str=str.Replace("[FamilyList]",addrTable.Rows[patientsPrinted]["famList"].ToString());
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					str=GetPostcardMessage(addrTable.Rows[patientsPrinted]["numberOfReminders"].ToString(),isFam:false);
					str=str.Replace("[DueDate]",addrTable.Rows[patientsPrinted]["dateDue"].ToString());
					str=str.Replace("[NameF]",addrTable.Rows[patientsPrinted]["patientNameF"].ToString());
					str=str.Replace("[NameFL]",addrTable.Rows[patientsPrinted]["patientNameFL"].ToString());
				}
				if(PrefC.HasClinicsEnabled && Clinics.GetClinic(clinicNum)!=null) {//has clinics and patient is assigned to a clinic.  
					Clinic clinicCur=Clinics.GetClinic(clinicNum);
					str=str.Replace("[ClinicName]",clinicCur.Abbr);
					str=str.Replace("[ClinicPhone]",TelephoneNumbers.ReFormat(clinicCur.Phone));
					string officePhone=clinicCur.Phone;
					if(string.IsNullOrEmpty(officePhone)) {
						officePhone=practicePhone;
					}
					str=str.Replace("[OfficePhone]",TelephoneNumbers.ReFormat(officePhone));
				}
				else {//use practice information for default. 
					str=str.Replace("[ClinicName]",PrefC.GetString(PrefName.PracticeTitle));
					str=str.Replace("[ClinicPhone]",practicePhone);
					str=str.Replace("[OfficePhone]",practicePhone);
				}
				str=str.Replace("[PracticeName]",PrefC.GetString(PrefName.PracticeTitle));
				str=str.Replace("[PracticePhone]",practicePhone);
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,10),Brushes.Black,new RectangleF(xPos+45,yPos+180,250,190));
				//Patient's Address-----------------------------------------------------------------------
				if(DoGroupFamilies() && addrTable.Rows[patientsPrinted]["famList"].ToString()!="")//print family card
				{
					str=addrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else{//print single card
					str=addrTable.Rows[patientsPrinted]["patientNameFL"].ToString()+"\r\n";
				}
				str+=addrTable.Rows[patientsPrinted]["address"].ToString()+"\r\n";
				str+=addrTable.Rows[patientsPrinted]["City"].ToString()+", "
					+addrTable.Rows[patientsPrinted]["State"].ToString()+" "
					+addrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,xPos+320,yPos+240);
				if(postCardsPerSheet==1){
					//Setting it to this value will cause it to break out of the while loop.
					yPos=ev.PageBounds.Height-bottomPageMargin;
				}
				else if(postCardsPerSheet==3){
					yPos+=366;
				}
				else{//4
					xPos+=550;
					if(xPos>1000){
						xPos=0+xAdj;
						yPos+=425;
					}
				}
				patientsPrinted++;
			}//while
			pagesPrinted++;
			if(pagesPrinted==totalPages){
				ev.HasMorePages=false;
				pagesPrinted=0;
				patientsPrinted=0;
			}
			else{
				ev.HasMorePages=true;
			}
		}

		///<summary>Shared functionality with Recalls, Recently Contacted, and Reactivations, be careful when making changes.</summary>
		private void butRefresh_Click(object sender, System.EventArgs e) {
			_gridCur.SetSelected(false);
			_gridCur.FillGrid();
		}

		private void butSetStatus_Click(object sender, System.EventArgs e) {
			if(IsAnyRowSelected()) {
				long newStatus=comboStatus.SelectedTag<Def>()!=null?comboStatus.SelectedTag<Def>().DefNum:0;
				gridMain.SelectedTags<PatRowTag>().ForEach(tag => Recalls.UpdateStatus(tag.PriKeyNum,newStatus));
				CommCreate(CommItemTypeAuto.RECALL,doIncludeNote:true);
			}
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butGotoFamily_Click(object sender,EventArgs e) {
			//button does not show when Recently Contacted tab is selected.
			if(IsOneRowSelected()) {
				if(!Security.IsAuthorized(Permissions.FamilyModule)) {
					return;
				}
				WindowState=FormWindowState.Minimized;
				GotoModule.GotoFamily(_patNumCur);
			}
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butGotoAccount_Click(object sender,EventArgs e) {
			//button does not show when Recently Contacted tab is selected.
			if(IsOneRowSelected()) {
				if(!Security.IsAuthorized(Permissions.AccountModule)) {
					return;
				}
				WindowState=FormWindowState.Minimized;
				GotoModule.GotoAccount(_patNumCur);
			}
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butCommlog_Click(object sender,EventArgs e) {
			CommCreate(IsRecallGridSelected()?CommItemTypeAuto.RECALL:CommItemTypeAuto.REACT,doIncludeNote:false);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void CommCreate(CommItemTypeAuto commType,bool doIncludeNote) {
			if(IsAnyRowSelected()) {
				List<long> listPatNums=_gridCur.SelectedTags<PatRowTag>().Select(x => x.PatNum).ToList();
				//show the first one, and then make all the others very similar
				Commlog commlogCur=new Commlog();
				commlogCur.PatNum=listPatNums[0];
				commlogCur.CommDateTime=DateTime.Now;
				commlogCur.SentOrReceived=CommSentOrReceived.Sent;
				commlogCur.Mode_=CommItemMode.Phone;//user can change this, of course.
				commlogCur.CommType=Commlogs.GetTypeAuto(commType);
				commlogCur.UserNum=Security.CurUser.UserNum;
				if(doIncludeNote) {
					commlogCur.Note=Lan.g(this,(commType==CommItemTypeAuto.RECALL?"Recall ":"Reactivation ")+" reminder.");
					if(commType==CommItemTypeAuto.RECALL && comboStatus.SelectedIndex>0) {
						commlogCur.Note+="  "+comboStatus.SelectedTag<Def>().ItemName;
					}
					else if(commType==CommItemTypeAuto.REACT && comboReactStatus.SelectedIndex>0) {
						commlogCur.Note+="  "+comboReactStatus.SelectedTag<Def>().ItemName;
					}
					else{
						commlogCur.Note+="  "+Lan.g(this,"Status None");
					}
				}
				commlogCur.IsNew=true;
				FormCommItem FormCI=new FormCommItem(commlogCur);
				if(FormCI.ShowDialog()!=DialogResult.OK) {
					_gridCur.FillGrid();
					return;
				}
				for(int i=1;i<_gridCur.SelectedIndices.Length;i++) {
					commlogCur.PatNum=listPatNums[i];
					Commlogs.Insert(commlogCur);
				}
				_gridCur.FillGrid();
			}
		}		

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,$"{(IsRecallGridSelected()?"Recall":"Reactivation")} list printed"),PrintoutOrientation.Landscape);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
				//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,$"{(IsRecallGridSelected()?"Recall":"Reactivation")} List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(IsRecallGridSelected()) {
					text=textDateStart.Text+" "+Lan.g(this,"to")+" "+textDateEnd.Text;
				}
				else {//Reactivation
					text=$"Since {validDateSince.Text}";
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
#endregion
			yPos=_gridCur.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}
	
		public override void OnProcessSignals(List<Signalod> listSignals) {
			if(listSignals.Any(x => x.IType==InvalidType.WebSchedRecallReminders)) {
				FillMain();
			}
		}

		private void tabControl_SelectedIndexChanged(object sender,EventArgs e) {
			if(tabControl.SelectedTab==tabPageRecentlyContacted && gridRecentlyContacted.Columns.Count==0) {//The grid has not been initialized yet.
				datePickerRecent.SetDateTimeFrom(DateTime.Today.AddMonths(-1));
				datePickerRecent.SetDateTimeTo(DateTime.Today);
				FillGridRecent();
			}
		}

		private void FillGridRecent() {
			ODProgress.ShowAction(//Show progress window while filling the grid.
				() => {
					List<Recalls.RecallRecent> listRecent=Recalls.GetRecentRecalls(datePickerRecent.GetDateTimeFrom(),datePickerRecent.GetDateTimeTo(),
						comboClinicRecent.ListSelectedClinicNums);
					RecallListEvent.Fire(ODEventType.RecallList,Lans.g(this,"Filling the Recently Contacted grid..."));
					gridRecentlyContacted.BeginUpdate();
					gridRecentlyContacted.Columns.Clear();
					ODGridColumn col=new ODGridColumn(Lan.g(this,"Date Time Sent"),140,GridSortingStrategy.DateParse);
					gridRecentlyContacted.Columns.Add(col);
					col=new ODGridColumn(Lan.g(this,"Patient"),200);
					gridRecentlyContacted.Columns.Add(col);
					col=new ODGridColumn(Lan.g(this,"Reminder Type"),180);
					gridRecentlyContacted.Columns.Add(col);
					col=new ODGridColumn(Lan.g(this,"Age"),50,GridSortingStrategy.AmountParse);
					gridRecentlyContacted.Columns.Add(col);
					col=new ODGridColumn(Lan.g(this,"Due Date"),100,GridSortingStrategy.DateParse);
					gridRecentlyContacted.Columns.Add(col);
					col=new ODGridColumn(Lan.g(this,"Recall Type"),130);
					gridRecentlyContacted.Columns.Add(col);
					col=new ODGridColumn(Lan.g(this,"Recall Status"),130);
					gridRecentlyContacted.Columns.Add(col);
					gridRecentlyContacted.Rows.Clear();
					foreach(Recalls.RecallRecent recent in listRecent) {
						ODGridRow row=new ODGridRow();
						row.Cells.Add(recent.DateSent.ToString());
						row.Cells.Add(recent.PatientName);
						row.Cells.Add(recent.ReminderType);
						row.Cells.Add(recent.Age.ToString());
						if(recent.DueDate.Year < 1880) {
							row.Cells.Add("");
						}
						else {
							row.Cells.Add(recent.DueDate.ToShortDateString());
						}
						row.Cells.Add(recent.RecallType);
						row.Cells.Add(recent.RecallStatus);
						row.Tag=recent;
						gridRecentlyContacted.Rows.Add(row);
					}
					gridRecentlyContacted.EndUpdate();
				},
				startingMessage:Lans.g(this,"Retieving data for the Recently Contacted grid..."),
				eventType:typeof(RecallListEvent),
				odEventType:ODEventType.RecallList
			);
		}

		private void FillReactivationGrid() {
			if(!Defs.GetDefsForCategory(DefCat.CommLogTypes).Any(x => x.ItemValue==CommItemTypeAuto.REACT.GetDescription(true))) {
				MsgBox.Show(this,Lan.g(this,"First you must set up a Reactivation commlog type in definitions"));
				return;
			}
			//Verification
			if(validDateSince.errorProvider1.GetError(validDateSince)!="") {
				return;
			}
			//Remember which reactivationnums were selected
			List<PatRowTag> listSelectedRows=gridReactivations.SelectedTags<PatRowTag>();
			//Determine the search settings from the UI
			DateTime dateSince=DateTime.MinValue;
			if(!string.IsNullOrWhiteSpace(validDateSince.Text)) {
				dateSince=PIn.Date(validDateSince.Text);
			}
			long provNum=-1;
			if(comboReactProv.SelectedIndex>0){
				provNum=_listProv[comboReactProv.SelectedIndex-1].ProvNum;
			}
			//-1 will show all patients without filtering clinics.
			long clinicNum=(PrefC.HasClinicsEnabled ? comboReactClinic.SelectedClinicNum : -1);
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboReactSite.SelectedIndex>0) {
				siteNum=comboReactSite.SelectedTag<Site>().SiteNum;
			}
			long billingType=0;
			if(comboBillingTypes.SelectedIndex>0) {
				billingType=comboBillingTypes.SelectedTag<Def>().DefNum;
			}
			DataTable tableReactivations=Reactivations.GetReactivationList(dateSince,checkReactGroupFamilies.Checked,checkReactShowDNC.Checked,
				provNum,clinicNum,siteNum,billingType,comboReactSortBy.SelectedTag<ReactivationListSort>(),comboShowReactivate.SelectedTag<RecallListShowNumberReminders>());
			//Fill in the grid
			gridReactivations.BeginUpdate();
			gridReactivations.Columns.Clear();
			gridReactivations.AddColumn("Last Seen",75);
			gridReactivations.AddColumn("Patient",90);
			gridReactivations.AddColumn("Age",30);
			gridReactivations.AddColumn("Provider",90);
			if(PrefC.HasClinicsEnabled) {
				gridReactivations.AddColumn("Clinic",75);
			}
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				gridReactivations.AddColumn("Site",75);
			}
			gridReactivations.AddColumn("Billing Type",85);
			gridReactivations.AddColumn("#Remind",55);
			gridReactivations.AddColumn("Last Contacted",100);
			gridReactivations.AddColumn("Contact Method",100);
			gridReactivations.AddColumn("Status",80);
			gridReactivations.AddColumn("Note",150);
			//Rows
			gridReactivations.Rows.Clear();
			foreach(DataRow row in tableReactivations.Rows) {
				ODGridRow rowNew=new ODGridRow();
				if(PIn.Bool(row["DoNotContact"].ToString())) {
					rowNew.ColorBackG=Color.Orange;
				}
				rowNew.Cells.Add(PIn.Date(row["DateLastProc"].ToString()).ToShortDateString());
				rowNew.Cells.Add(Patients.GetNameLF(row["LName"].ToString(),row["FName"].ToString(),row["Preferred"].ToString(),row["MiddleI"].ToString()));
				rowNew.Cells.Add(Patients.DateToAge(PIn.Date(row["Birthdate"].ToString())).ToString());
				rowNew.Cells.Add(Providers.GetLongDesc(PIn.Long(row["PriProv"].ToString())));
				if(PrefC.HasClinicsEnabled) {
					rowNew.Cells.Add(Clinics.GetDesc(PIn.Long(row["ClinicNum"].ToString())));
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					rowNew.Cells.Add(Sites.GetDescription(PIn.Long(row["SiteNum"].ToString())));
				}
				rowNew.Cells.Add(row["BillingType"].ToString());
				rowNew.Cells.Add(row["ContactedCount"].ToString());
				rowNew.Cells.Add(row["DateLastContacted"].ToString());
				rowNew.Cells.Add(PIn.Enum<ContactMethod>(row["PreferRecallMethod"].ToString()).ToString()); 
				long status=PIn.Long(row["ReactivationStatus"].ToString());
				rowNew.Cells.Add(status>0?Defs.GetDef(DefCat.RecallUnschedStatus,status).ItemName:"");
				rowNew.Cells.Add(row["ReactivationNote"].ToString());
				rowNew.Tag=new PatRowTag(
					PIn.Long(row["PatNum"].ToString()),
					PIn.Long(row["ReactivationNum"].ToString()),
					status,
					PIn.Int(row["ContactedCount"].ToString()),
					row["Email"].ToString(),
					PIn.Enum<ContactMethod>(row["PreferRecallMethod"].ToString()),
					PIn.Long(row["Guarantor"].ToString()));
				gridReactivations.Rows.Add(rowNew);
				if(listSelectedRows.Any(x => x.PriKeyNum==((PatRowTag)rowNew.Tag).PriKeyNum)) {
					gridReactivations.SetSelected(gridReactivations.Rows.Count-1,true);
				}
			}
			labelReactPatCount.Text=Lan.g(this,"Patient Count:")+" "+gridReactivations.Rows.Count.ToString();
			gridReactivations.EndUpdate();
		}

		private void gridReactivations_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PatRowTag tag=(PatRowTag)gridReactivations.Rows[e.Row].Tag;
			FormReactivationEdit formRE;
			if(tag.PriKeyNum==0) {//Patient has never been contacted for reactivations before.
				formRE=new FormReactivationEdit(tag.PatNum);
			}
			else {
				formRE=new FormReactivationEdit(Reactivations.GetOne(tag.PriKeyNum));
			}
			formRE.ShowDialog();
			if(formRE.DialogResult==DialogResult.Yes) { //indicates the reactivation status changed
				CommCreate(CommItemTypeAuto.REACT,doIncludeNote:true);
			}
			if(formRE.DialogResult==DialogResult.OK || formRE.DialogResult==DialogResult.Yes || formRE.DialogResult==DialogResult.Abort) {
				FillReactivationGrid();
			}
			SetFamilyColors();
		}

		private void butReactSetStatus_Click(object sender,EventArgs e) {
			long status=comboReactStatus.SelectedTag<Def>()!=null?comboReactStatus.SelectedTag<Def>().DefNum:0;
			if(IsAnyRowSelected()) {
				foreach(PatRowTag tag in gridReactivations.SelectedGridRows.Select(x => ((PatRowTag)x.Tag))) {
					if(tag.PriKeyNum==0) { //They don't have a reactivation so create one
						Reactivations.Insert(new Reactivation() {
							PatNum=tag.PatNum,
							ReactivationStatus=status
						});
					}
					else { //update the reactivation status
						Reactivations.UpdateStatus(tag.PriKeyNum,status);
					}
				}
				CommCreate(CommItemTypeAuto.REACT,doIncludeNote:true);
			}
		}

		public List<long> SchedPatReact(long patNum) {
			List<long> listRet=new List<long>();
			if(PatRestrictionL.IsRestricted(patNum,PatRestrict.ApptSchedule)) {
				return listRet;
			}
			FormApptEdit formAE=new FormApptEdit(0,patNum);
			formAE.ShowDialog();
			if(formAE.DialogResult==DialogResult.OK) {
				listRet.Add(formAE.GetAppointmentCur().AptNum);
			}
			return listRet;
		}

		public List<long> SchedFamReact(Family fam) {
			List<long> listRet=new List<long>();
			foreach(Patient pat in fam.ListPats) {
				if(PatRestrictionL.IsRestricted(pat.PatNum,PatRestrict.ApptSchedule)) {
					MsgBox.Show(Lan.g(this,$"Skipping family member {pat.GetNameFirstOrPrefL()} due to patient restriction")+" "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule));
					continue;
				}
				listRet.AddRange(SchedPatReact(pat.PatNum));
			}
			return listRet;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsGridEmpty() {
			if(_gridCur.Rows.Count < 1) {
				MsgBox.Show(this,"There are no Patients in the table.  Must have at least one.");    
        return true;
      }
			return false;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsAnyRowSelected() {
			if(IsGridEmpty()) {
				return false;
			}
			else if(_gridCur.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a patient first.");
				return false;
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsOneRowSelected() {
			if(!IsAnyRowSelected()) {
				return false;
			}
			if(_gridCur.SelectedIndices.Length>1) {
				MsgBox.Show(this,"Please select only one patient first.");
				return false;
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsStatusSet(PrefName prefNameRecall,PrefName prefNameReactivation) {
			if((IsRecallGridSelected() && PrefC.GetLong(prefNameRecall)==0) || (IsReactivationGridSelected() && PrefC.GetLong(prefNameReactivation)==0)) {
				MsgBox.Show(this,$"You need to set a status first in the "+(IsRecallGridSelected()?"Recall":"Reactivations")+" Setup window.");
				return false;
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private bool IsAnyPatToContact(string previewType,ContactMethod method,bool doAskPreview=true) {
			if(_gridCur.SelectedIndices.Length==0) { //try to select rows that don't have a status
				for(int i=0;i<_gridCur.Rows.Count;i++) {
					PatRowTag tag=(PatRowTag)_gridCur.Rows[i].Tag;
					if(tag.StatusDefNum!=0) {
						continue; //we only want rows without a status
					}
					if(tag.ContactMethodPreferred!=method) {
						//Only allow selecting rows with a different preferred contact method only when method is 'Mail'.
						if(method!=ContactMethod.Mail) {
							continue;
						}
						//The only other preferred contact method that is allowed is None.
						if(tag.ContactMethodPreferred!=ContactMethod.None) {
							continue;
						}
					}
					_gridCur.SetSelected(i,true);
				}
				if(_gridCur.SelectedIndices.Length==0){
					MsgBox.Show(this,$"No patients of {method.ToString()} type.");
					return false;
				}
				if(doAskPreview && !MsgBox.Show(this,true,$"Preview {previewType} for all of the selected patients?")) {
					return false;
				}
			}
			return true;
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private string GetPostcardMessage(string numReminders,bool isFam) {
			PrefName prefName=isFam?PrefName.ReactivationPostcardFamMsg:PrefName.ReactivationPostcardMessage;
			if(IsRecallGridSelected()) {
				if(numReminders=="0") {
					prefName=isFam?PrefName.RecallPostcardFamMsg:PrefName.RecallPostcardMessage;
				}
				else if(numReminders=="1") {
					prefName=isFam?PrefName.RecallPostcardFamMsg2:PrefName.RecallPostcardMessage2;
				}
				else {
					prefName=isFam?PrefName.RecallPostcardFamMsg3:PrefName.RecallPostcardMessage3;
				}
			}
			return PrefC.GetString(prefName);
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private void butSched_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(!IsOneRowSelected()) {
				return;
			}
			Family fam=Patients.GetFamily(_patNumCur);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<long> pinAptNums=new List<long>();
			switch(((UI.Button)sender).Tag.ToString()) {
				case "SchedPatRecall":
					pinAptNums=SchedPatRecall(_gridCur.SelectedTag<PatRowTag>().PriKeyNum,fam.GetPatient(_patNumCur),subList,planList);
					break;
				case "SchedFamRecall":
					if(!Recalls.IsRecallProphyOrPerio(Recalls.GetRecall(_gridCur.SelectedTag<PatRowTag>().PriKeyNum))) {
						MsgBox.Show(this,"Only recall types of Prophy or Perio can be scheduled for families.");
						return;
					}
					pinAptNums=SchedFamRecall(fam,subList,planList);
					break;
				case "SchedPatReact":
					pinAptNums=SchedPatReact(_patNumCur);
					break;
				case "SchedFamReact":
					pinAptNums=SchedFamReact(fam);
					break;
			}
			if(pinAptNums.Count<1) {
				return;
			}
			WindowState=FormWindowState.Minimized;
			GotoModule.PinToAppt(pinAptNums,_patNumCur);
			//no securitylog entry needed.  It will be made as each appt is dragged off pinboard.
			_gridCur.SetSelected(false);
			_gridCur.FillGrid();
		}

		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.</summary>
		private DataTable GetAddrTable() {
			if(IsRecallGridSelected()) {//RECALL
				addrTable=Recalls.GetAddrTable(_gridCur.SelectedTags<PatRowTag>().Select(x => x.PriKeyNum).ToList()
					,checkGroupFamilies.Checked,comboSort.SelectedTag<RecallListSort>());
			}
			else { //REACTIVATION
				List<Patient> listPats=Patients.GetMultPats(_gridCur.SelectedTags<PatRowTag>().Select(x => x.PatNum).ToList()).ToList();
				List<Patient> listGuars=Patients.GetMultPats(listPats.Select(x => x.Guarantor).Distinct().ToList()).ToList();
				addrTable=Reactivations.GetAddrTable(listPats,listGuars,checkReactGroupFamilies.Checked,comboReactSortBy.SelectedTag<ReactivationListSort>());
			}
			return addrTable;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
		
		///<summary>Shared functionality with Recalls and Reactivations, be careful when making changes.
		///Contains the various properties we might need when the user wants to do an operation for a specific row or set of rows.
		///TODO in the future, use this to replace the need to use _tableRecalls in this form.</summary>
		private class PatRowTag {
			public long PatNum;
			///<summary>Can be either the recall num or the reactivation num depending on the grid.</summary>
			public long PriKeyNum;
			public long StatusDefNum;
			public int NumReminders;
			public string Email;
			public ContactMethod ContactMethodPreferred;
			public long GuarantorNum;

			public PatRowTag(long patNum,long priKeyNum,long statusDefNum,int numReminders,string email,ContactMethod contact,long guarNum) {
				PatNum=patNum;
				PriKeyNum=priKeyNum;
				StatusDefNum=statusDefNum;
				NumReminders=numReminders;
				Email=email;
				ContactMethodPreferred=contact;
				GuarantorNum=guarNum;
			}
		}
	}
	
}
