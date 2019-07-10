﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.HL7;

namespace OpenDental {
	///<summary>This is a replacement for ContrAppt module. This new version is turned off by default and can only be turned on by manually setting pref.ApptModuleUses2019Overhaul=1.  If, someday, it's shown to work flawlessly, then it will be turned on for everyone, with the option to switch to the old original version in an emergency.</summary>
	public partial class ContrApptJ:UserControl {

		///<summary>This allows appointment overlap.  This will be reworked when we finalize that we want this to be permanent.</summary>
		public static bool AllowOverlap=true;

		#region Fields - Private
		///<summary>Used for blockouts.  OpNum.</summary>
		private long _blockoutClickedOnOp;
		///<summary>Used for blockouts.  Already handles week view.  Time is not rounded.</summary>
		private DateTime _dateTimeClickedBlockout;
		///<summary>The last dateTime that the waiting room was refreshed.  Local computer time.</summary>
		private DateTime _dateTimeWaitingRmRefreshed;
		///<summary></summary>
		private bool _doPrintCardFamily;
		private FormASAP _formASAP;
		private FormConfirmList _formConfirmList;
		private FormRecallList _formRecallList;
		private FormTrackNext _formTrackNext;
		private FormUnsched _formUnsched;
		///<summary>This prevents extra refreshes during the convoluted startup sequence.  Remains false until the end of InitializeOnStartup().</summary>
		private bool _hasInitializedOnStartup;
		///<summary></summary>
		private bool _hasLaidOutScrollBar;
		///<summary>This is a list of ApptViews that are available in comboView, which will be filtered for the currently selected clinic if clincs are enabled.  This list will contain the same number of items as comboView minus 1 for 'none' and is filled at the same time as comboView.  Use this list when accessing the view by comboView.SelectedIndex.</summary>
		private List<ApptView> _listApptViews=new List<ApptView>();
		private List<Provider> _listProvidersSearch;
		private List<ScheduleOpening> _listScheduleOpenings;
		private MenuItem menuItemBreakAppt;
		private Patient _patCur;
		///<summary>If the user has done a blockout/copy, then this will contain the blockout that is on the "clipboard".</summary>
		private Schedule _scheduleBlockoutClipboard;
		#endregion Fields - Private

		#region Constructor
		public ContrApptJ() {
			Logger.openlog.Log("Initializing appointment module...",Logger.Severity.INFO);
			InitializeComponent();
			gridReminders.ContextMenu=menuReminderEdit;
		}
		#endregion Constructor

		#region Events - ContrAppt
		private void ContrAppt_Load(object sender,EventArgs e) {

		}

		private void ContrAppt_Resize(object sender,EventArgs e) {
			LayoutPanels();
		}
		#endregion Events - ContrAppt

		#region Events - contrApptPanel
		private void contrApptPanel_ApptDoubleClicked(object sender,UI.ApptEventArgs e) {
			//security handled inside the form
			long patnum=e.Appt.PatNum;
			FormApptEdit formApptEdit=new FormApptEdit(contrApptPanel.SelectedAptNum);
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult==DialogResult.OK) {//saved or deleted here
				Appointment apt=Appointments.GetOneApt(contrApptPanel.ClickedAptNum);
				if(apt!=null) {
					Appointment aptOld=apt.Copy(); //this needs to happen before TryAdjustAppointmentPattern.
					if(TryAdjustAppointmentPattern(apt)) {
						MsgBox.Show("Appointment is too long and would overlap another appointment.  Automatically shortened to fit.");
						try {
							Appointments.Update(apt,aptOld);//Appointments S-Class handles Signalods
						}
						catch(ApplicationException ex) {
							MessageBox.Show(ex.Message);
						}
					}
				}
				ModuleSelected(patnum);//apt.PatNum);//apt might be null if user deleted appt.
			}
			else if(formApptEdit.DialogResult==DialogResult.Cancel && formApptEdit.HasProcsChangedAndCancel) { //If user canceled but changed the procs on appt first
				//Refresh the grid, don't need to check length because it didn't change.  Plus user might not want to change length.
				ModuleSelected(patnum);
				Signalods.SetInvalidAppt(formApptEdit.GetAppointmentOld());//use old here because they cancelled.  Only calling this because there is no S-Class call.
			}
		}

		private void contrApptPanel_ApptMainAreaDoubleClicked(object sender,UI.ApptClickEventArgs e) {
			FormPatientSelect formPatientSelect=new FormPatientSelect();
			if(_patCur!=null) {
				formPatientSelect.InitialPatNum=_patCur.PatNum;
			}
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			if(_patCur==null || formPatientSelect.SelectedPatNum!=_patCur.PatNum) {//if the patient was changed
				RefreshModuleDataPatient(formPatientSelect.SelectedPatNum);
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
			if(_patCur!=null && PatRestrictionL.IsRestricted(_patCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			if(AppointmentL.PromptForMerge(_patCur,out _patCur)) {
				RefreshModuleDataPatient(_patCur.PatNum);
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
			if(_patCur!=null && _patCur.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show("Appointments cannot be scheduled for "+_patCur.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			Appointment appt=null;
			bool updateAppt=false;
			if(formPatientSelect.NewPatientAdded) {
				Operatory curOp=Operatories.GetOperatory(e.OpNum);
				//if(contrApptPanel.IsWeeklyView) {//handled before this event
				//	dateSelected=WeekStartDate.AddDays(SheetClickedonDay);
				DateTime dateTimeAskedToArrive=DateTime.MinValue;
				if(_patCur.AskToArriveEarly > 0) {
					dateTimeAskedToArrive=e.DateT.AddMinutes(-_patCur.AskToArriveEarly);
					MessageBox.Show("Ask patient to arrive "+_patCur.AskToArriveEarly
						+" minutes early at "+dateTimeAskedToArrive.ToShortTimeString()+".");
				}
				appt=Appointments.CreateApptForNewPatient(_patCur,curOp,e.DateT,dateTimeAskedToArrive,null,contrApptPanel.ListSchedules);
				//New patient. Set to prospective if operatory is set to set prospective.
				if(curOp.SetProspective) {
					if(MsgBox.Show(MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
						Patient patOld=_patCur.Copy();
						_patCur.PatStatus=PatientStatus.Prospective;
						Patients.Update(_patCur,patOld);
					}
				}
				FormApptEdit formApptEdit=new FormApptEdit(appt.AptNum);//this is where security log entry is made
				formApptEdit.IsNew=true;
				formApptEdit.ShowDialog();
				if(formApptEdit.DialogResult==DialogResult.OK) {
					if(appt.IsNewPatient) {
						AutomationL.Trigger(AutomationTrigger.CreateApptNewPat,null,appt.PatNum,appt.AptNum);
					}
					AutomationL.Trigger(AutomationTrigger.CreateAppt,null,appt.PatNum,appt.AptNum);
					RefreshModuleDataPatient(_patCur.PatNum);
					FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
					if(!HasValidStartTime(appt)) {
						Appointment apptOld=appt.Copy();
						MsgBox.Show("Appointment start time would overlap another appointment.  Moving appointment to pinboard.");
						DataTable dataTable=Appointments.GetPeriodApptsTable(contrApptPanel.DateStart,contrApptPanel.DateStart,appt.AptNum,false);
						if(dataTable.Rows.Count==0){
							return;//silently fail
						}
						DataRow dataRow=dataTable.Rows[0];
						SendToPinboardDataRow(dataRow);
						appt.AptStatus=ApptStatus.UnschedList;
						try {
							Appointments.Update(appt,apptOld);//Appointments S-Class handles Signalods
						}
						catch(ApplicationException ex) {
							MessageBox.Show(ex.Message);
						}
						RefreshPeriod();
						return;//It's ok to skip the rest of the method here. The appointment is now on the pinboard and must be rescheduled
					}
					appt=Appointments.GetOneApt(appt.AptNum);  //Need to get appt from DB so we have the time pattern
					updateAppt=true;						
				}
			}
			else {//(also, new patient not added)
				if(Appointments.HasOutstandingAppts(_patCur.PatNum) | (Plugins.HookMethod(this,"ContrAppt.ContrApptSheet2_DoubleClick_apptOtherShow"))) {
					DisplayOtherDlg(true,e.DateT,e.OpNum);
				}
				else {
					FormApptsOther formApptsOther=new FormApptsOther(_patCur.PatNum);//doesn't actually get shown
					CheckStatus();
					formApptsOther.IsInitialClick=true;
					formApptsOther.DateTNew=e.DateT;
					formApptsOther.OpNumNew=e.OpNum;
					formApptsOther.MakeAppointment();
					if(formApptsOther.AptNumsSelected.Count>0) {
						contrApptPanel.SelectedAptNum=formApptsOther.AptNumsSelected[0];
					}
					appt=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					updateAppt=true;
				}
			}
			if(updateAppt && appt!=null) {
				#region Provider Term Date Check
				//Prevents appointments with providers that are past their term end date from being scheduled
				string message=Providers.CheckApptProvidersTermDates(appt);
				if(message!="") {
					MessageBox.Show(message);//translated in Providers S class method
					return;
				}
				#endregion Provider Term Date Check				
				Appointment aptOld=appt.Copy();
				if(TryAdjustAppointmentPattern(appt)) {
					MsgBox.Show("Appointment is too long and would overlap another appointment.  Automatically shortened to fit.");
				}
				try {
					Appointments.Update(appt,aptOld);//Appointments S-Class handles Signalods
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
				}
				RefreshPeriod();
			}
		}

		private void contrApptPanel_ApptMoved(object sender,UI.ApptEventArgs e) {
			Appointment appt=e.Appt;
			Appointment apptOld=e.ApptOld;
			MoveAppointment(appt,apptOld);
			#region Update UI and cache
			ProcFeeHelper procFeeHelper=new ProcFeeHelper(e.Appt.PatNum);
			//check if the proc fees on the moved appointment need updating
			List<Procedure> procsForApt = Procedures.GetProcsForSingle(appt.AptNum,false);//get the procedures on the appointment
			bool isUpdatingFees = false;
			List<Procedure> listProcsNew = procsForApt.Select(x => Procedures.UpdateProcInAppointment(appt,x.Copy())).ToList();
			if(procsForApt.Exists(x => x.ProvNum!=listProcsNew.FirstOrDefault(y => y.ProcNum==x.ProcNum).ProvNum)) {//Either the primary or hygienist changed.
				string promptText = "";
				isUpdatingFees=Procedures.ShouldFeesChange(listProcsNew,procsForApt,ref promptText,procFeeHelper);
				if(isUpdatingFees) {//Made it pass the pref check.
					if(promptText!="" && !MsgBox.Show(this,MsgBoxButtons.YesNo,promptText)) {
						isUpdatingFees=false;
					}
				}
			}
			Procedures.SetProvidersInAppointment(appt,procsForApt,isUpdatingFees,procFeeHelper);
			RefreshModuleDataPatient(appt.PatNum);
			FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			RefreshPeriod();				
			Recalls.SynchScheduledApptFull(appt.PatNum);
			AppointmentEvent.Fire(ODEventType.AppointmentEdited,appt);
			#endregion Update UI and cache
			//Plugins.HookAddCode(this, "ContrAppt.ContrApptSheet2_MouseUp_end",appt,apptOld);
		}

		private void contrApptPanel_ApptMovedToPinboard(object sender,UI.ApptDataEventArgs e) {
			SendToPinboardDataRow(e.DataRowAppt);//sets selectedAptNum=-1. do before refresh prev
		}

		private void contrApptPanel_ApptNullFound(object sender,EventArgs e) {
			//todo:
			//ApptIsNull(e. ??
		}

		private void contrApptPanel_ApptResized(object sender,UI.ApptEventArgs e) {
			RefreshModuleDataPatient(e.Appt.PatNum);
			FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			RefreshPeriod(true);
			AppointmentEvent.Fire(ODEventType.AppointmentEdited,e.Appt);
		}

		private void ContrApptPanel_DateChanged(object sender, EventArgs e){
			SetWeeklyView(contrApptPanel.IsWeeklyView);//because weekly view changed internally as well.
		}

		private void contrApptPanel_SelectedApptChanged(object sender,UI.ApptDataEventArgs e) {
			pinBoard.SelectedIndex=-1;
			//In old ContrAppt, this section was at the end of mouse down:
			//Todo: Need to revisit and test, because this now happens before ShowDraggableApptSingle
			if(e.DataRowAppt==null){				
				RefreshModuleScreenButtonsRight();//just disables the buttons on the right
				return;
			}
			long patNumNew=PIn.Long(e.DataRowAppt["PatNum"].ToString());
			if(_patCur==null || _patCur.PatNum!=patNumNew){//oldPatNum) {//patient changed
				//This needs to be called after ShowDraggableApptSingle(...)
				//due to S_Contr_PatientSelected(...) indirectly stoping the drag if there are any popups.
				RefreshModuleDataPatient(patNumNew);
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
			else {//patient not changed
				//This is called either way. But we avoid some code by calling it directly here.
				RefreshModuleScreenButtonsRight();
			}
		}

		private void ContrApptPanel_ApptRightClicked(object sender, UI.ApptClickEventArgs e){
			if(PrefC.IsODHQ) {
				menuApt.MenuItems.RemoveByKey(MenuItemNames.Jobs);
				menuApt.MenuItems.RemoveByKey(MenuItemNames.JobsSpacer);
				MenuItem menuSpacer=new MenuItem("-");
				menuSpacer.Name=MenuItemNames.JobsSpacer;
				menuApt.MenuItems.Add(menuSpacer);
				MenuItem menuJobs=new MenuItem("Jobs");
				menuJobs.Name=MenuItemNames.Jobs;
				menuJobs.MenuItems.Add("Attach Job",new EventHandler(menuJobs_Attach));
				List<JobLink> jobLinks = JobLinks.GetForApptNum(contrApptPanel.SelectedAptNum);
				List<Job> listJobs = Jobs.GetMany(jobLinks.Select(x => x.JobNum).ToList());
				foreach(Job job in listJobs) {
					MenuItem menuItemJob=new MenuItem(job.ToString(),new EventHandler(menuJobs_GoToJob));
					menuItemJob.Tag=job.JobNum;
					menuJobs.MenuItems.Add(menuItemJob);
				}
				menuApt.MenuItems.Add(menuJobs);
			}
			menuApt.MenuItems.RemoveByKey(MenuItemNames.PhoneDiv);
			menuApt.MenuItems.RemoveByKey(MenuItemNames.HomePhone);
			menuApt.MenuItems.RemoveByKey(MenuItemNames.WorkPhone);
			menuApt.MenuItems.RemoveByKey(MenuItemNames.WirelessPhone);
			menuApt.MenuItems.RemoveByKey(MenuItemNames.TextDiv);
			menuApt.MenuItems.RemoveByKey(MenuItemNames.SendText);
			menuApt.MenuItems.RemoveByKey(MenuItemNames.SendConfirmationText);
			menuApt.MenuItems.RemoveByKey(MenuItemNames.OrthoChart);
			MenuItem menuItem;
			if(PrefC.GetBool(PrefName.ApptModuleShowOrthoChartItem)) {
				menuItem=menuApt.MenuItems.Add("Go To "+OrthoChartTabs.GetFirst(true).TabName,new EventHandler(menuApt_Click));
				menuItem.Name=MenuItemNames.OrthoChart;
			}
			//Phone numbers
			if(!String.IsNullOrEmpty(_patCur.HmPhone)||!String.IsNullOrEmpty(_patCur.WkPhone)||!String.IsNullOrEmpty(_patCur.WirelessPhone)) {
				menuItem=menuApt.MenuItems.Add("-");
				menuItem.Name=MenuItemNames.PhoneDiv;
			}
			if(!String.IsNullOrEmpty(_patCur.HmPhone)) {
				menuItem=menuApt.MenuItems.Add("Call Home Phone "+_patCur.HmPhone,new EventHandler(menuApt_Click));
				menuItem.Name=MenuItemNames.HomePhone;
			}
			if(!String.IsNullOrEmpty(_patCur.WkPhone)) {
				menuItem=menuApt.MenuItems.Add("Call Work Phone "+_patCur.WkPhone,new EventHandler(menuApt_Click));
				menuItem.Name=MenuItemNames.WorkPhone;
			}
			if(!String.IsNullOrEmpty(_patCur.WirelessPhone)) {
				menuItem=menuApt.MenuItems.Add("Call Wireless Phone "+_patCur.WirelessPhone,new EventHandler(menuApt_Click));
				menuItem.Name=MenuItemNames.WirelessPhone;
			}
			//Texting
			menuItem=menuApt.MenuItems.Add("-");
			menuItem.Name=MenuItemNames.TextDiv;
			menuItem=menuApt.MenuItems.Add("Send Text",menuApt_Click);
			menuItem.Name=MenuItemNames.SendText;
			if(!SmsPhones.IsIntegratedTextingEnabled() && !Programs.IsEnabled(ProgramName.CallFire)) {
				menuItem.Enabled=false;
			}
			menuItem=menuApt.MenuItems.Add("Send Confirmation Text",menuApt_Click);
			menuItem.Name=MenuItemNames.SendConfirmationText;
			if(!SmsPhones.IsIntegratedTextingEnabled() && !Programs.IsEnabled(ProgramName.CallFire)) {
				menuItem.Enabled=false;
			}
			Plugins.HookAddCode(this,"ContrAppt.MouseDownAppointment_menuApt_right_click",menuApt);
			menuApt.Show(contrApptPanel,e.Location);
		}

		private void ContrApptPanel_ApptMainAreaRightClicked(object sender, UI.ApptClickEventArgs e){
			MenuItem menuEdit=menuBlockout.MenuItems[0];
			MenuItem menuCut=menuBlockout.MenuItems[1];
			MenuItem menuCopy=menuBlockout.MenuItems[2];
			MenuItem menuPaste=menuBlockout.MenuItems[3];
			MenuItem menuDelete=menuBlockout.MenuItems[4];
			MenuItem menuCutCopyPaste=menuBlockout.MenuItems[6];
			MenuItem menuClearForDay=new MenuItem();
			MenuItem menuClearForDayOp=new MenuItem();
			MenuItem menuClearForDayClinics=new MenuItem();
			if(PrefC.HasClinicsEnabled) {//No clear for day if clinics enabled
				menuClearForDayOp=menuBlockout.MenuItems[7];
				menuClearForDayClinics=menuBlockout.MenuItems[8];
			}
			else {//Clinics disabled, no clear for day clinics
				menuClearForDay=menuBlockout.MenuItems[7];
				menuClearForDayOp=menuBlockout.MenuItems[8];					
			}
			if(!Security.IsAuthorized(Permissions.Blockouts,true)) {
				menuCutCopyPaste.Enabled=false;
				menuClearForDay.Enabled=false;
				menuClearForDayOp.Enabled=false;
				menuClearForDayClinics.Enabled=false;
			}
			_blockoutClickedOnOp=e.OpNum;
			_dateTimeClickedBlockout=e.DateT;
			int clickedOnBlockCount=0;
			string blockoutFlags="";
			List<Schedule> ListSchedulesBlockout=Schedules.GetListForType(contrApptPanel.ListSchedules,ScheduleType.Blockout,0);
			//List<ScheduleOp> listForSched;
			for(int i=0;i<ListSchedulesBlockout.Count;i++) {
				if(ListSchedulesBlockout[i].SchedDate.Date!=e.DateT.Date) {
					continue;
				}
				if(ListSchedulesBlockout[i].StartTime > e.DateT.TimeOfDay
					|| ListSchedulesBlockout[i].StopTime <= e.DateT.TimeOfDay) {
					continue;
				}
				//listForSched=ScheduleOps.GetForSched(ListForType[i].ScheduleNum);
				for(int p=0;p<ListSchedulesBlockout[i].Ops.Count;p++) {
					if(ListSchedulesBlockout[i].Ops[p]==e.OpNum) {
						clickedOnBlockCount++;
						blockoutFlags=Defs.GetDef(DefCat.BlockoutTypes,ListSchedulesBlockout[i].BlockoutType).ItemValue;
						break;//out of ops loop
					}
				}
			}
			if(clickedOnBlockCount>0) {
				menuPaste.Enabled=false;//Can't paste on top of an existing blockout
				menuEdit.Enabled=true;
				menuCopy.Enabled=true;
				menuCut.Enabled=true;
				menuDelete.Enabled=true;
				if(blockoutFlags.Contains(BlockoutType.DontCopy.GetDescription())) {
					//users without blockout permission are still allowed to add and edit this blockout. 
					menuCut.Enabled=false;
					menuCopy.Enabled=false;
				}
				else if(blockoutFlags.Contains(BlockoutType.NoSchedule.GetDescription())) {
					//users without blockout permission are still allowed to add and edit this blockout.
					if(!Security.IsAuthorized(Permissions.Blockouts,true)) {
						menuCut.Enabled=false;
						menuCopy.Enabled=false;
					}
				}
				else { //this is not a blockout type that this user is allowed to edit. 
					if(!Security.IsAuthorized(Permissions.Blockouts,true)) {
						menuEdit.Enabled=false;
						menuCopy.Enabled=false;
						menuCut.Enabled=false;
						menuDelete.Enabled=false;
					}
				}
				if(clickedOnBlockCount>1) {
					FormPopupFade.ShowMessage(this,"There are multiple blockouts in this slot.  You should try to delete or move one of them.");
				}
			}
			else {//Not clicked on blockout
				menuEdit.Enabled=false;
				menuCut.Enabled=false;
				menuCopy.Enabled=false;
				if(_scheduleBlockoutClipboard==null) {
					menuPaste.Enabled=false;
				}
				else {
					menuPaste.Enabled=true;
				}
				menuDelete.Enabled=false;
			}
			bool isTextingEnabled=SmsPhones.IsIntegratedTextingEnabled();
			if(PrefC.GetBool(PrefName.WebSchedAsapEnabled)) {
				if(!SmsPhones.IsIntegratedTextingEnabled()) {//Some customers have the Bundle without texting
					MenuItem[] arrMenuItems=menuBlockout.MenuItems.Find(MenuItemNames.TextAsapList,false);
					if(arrMenuItems.Length > 0) {
						arrMenuItems[0].Text=Lans.g(this,"Email ASAP List");
					}
				}
			}
			else if(isTextingEnabled) {//Don't have Web Sched ASAP but do have texting
				MenuItem[] arrMenuItems=menuBlockout.MenuItems.Find(MenuItemNames.TextAsapList,false);
				if(arrMenuItems.Length > 0) {
					arrMenuItems[0].Text=Lans.g(this,"Text ASAP List (manual)");
				}
			}
			else {//Don't have Web Sched ASAP or texting
				MenuItem[] arrMenuItems=menuBlockout.MenuItems.Find(MenuItemNames.TextAsapList,false);
				if(arrMenuItems.Length > 0) {
					arrMenuItems[0].Visible=false;
				}
			}
			SetMenuItemProperty(menuBlockout,MenuItemNames.TextApptsForDayOp,x => x.Visible=isTextingEnabled);
			SetMenuItemProperty(menuBlockout,MenuItemNames.TextApptsForDayView,x => x.Visible=isTextingEnabled);
			SetMenuItemProperty(menuBlockout,MenuItemNames.TextApptsForDay,x => {
				x.Visible=isTextingEnabled;
				x.Text=MenuItemNames.TextApptsForDay+(PrefC.HasClinicsEnabled ? ", Clinic only" : "");
			});
			//Fun, but not needed----
			menuBlockout.MenuItems[menuBlockout.MenuItems.Count-1].Text="Update Provs on Future Appts ("+Operatories.GetOperatory(e.OpNum).Abbrev+")";
			menuBlockout.Show(contrApptPanel,e.Location);
		}

		///<summary>Finds the MenuItem on the contextMenu and performs the action on it.</summary>
		private void SetMenuItemProperty(ContextMenu contextMenu,string menuItemName,Action<MenuItem> actionSetMenuItem) {
			MenuItem[] arrMenuItems=contextMenu.MenuItems.Find(menuItemName,false);
			if(arrMenuItems.Length > 0) {
				actionSetMenuItem(arrMenuItems[0]);
			}
		}
		#endregion Events - contrApptPanel

		#region Events - toolBarMain
		private void toolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag.GetType()==typeof(string)) {
				//standard predefined button
				switch(e.Button.Tag.ToString()) {
					case "Lists":
						toolBarLists_Click();
						break;
					case "Print":
						toolBarPrint_Click();
						break;
					case "RapidCall":
						try {
							OpenDental.Bridges.RapidCall.ShowPage();
						}
						catch(Exception ex) {
							MessageBox.Show(ex.Message);
						}
						break;
				}
			}
			else if(e.Button.Tag.GetType()==typeof(Program)) {
				Patient pat=null;
				if(_patCur!=null) {
					pat=Patients.GetPat(_patCur.PatNum);
				}
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,pat);
			}
		}

		private void toolBarLists_Click() {
			FormApptLists FormA=new FormApptLists();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.Cancel) {
				return;
			}
			switch(FormA.SelectionResult) {
				case ApptListSelection.Recall:
					ListRecall_Click();
					break;
				case ApptListSelection.Confirm:
					ListConfirm_Click();
					break;
				case ApptListSelection.Planned:
					ListPlanned_Click();
					break;
				case ApptListSelection.Unsched:
					ListUnsched_Click();
					break;
				case ApptListSelection.ASAP:
					ListASAP_Click();
					break;
				case ApptListSelection.Radiology:
					ListRadiology_Click();
					break;
				case ApptListSelection.InsVerify:
					ListInsVerify_Click();
					break;
			}
		}

		private void ListRecall_Click() {
			if(_formRecallList==null || _formRecallList.IsDisposed) {
				_formRecallList=new FormRecallList();
			}
			_formRecallList.Show();
			if(_formRecallList.WindowState==FormWindowState.Minimized) {
				_formRecallList.WindowState=FormWindowState.Normal;
			}
			_formRecallList.BringToFront();
		}

		private void ListConfirm_Click() {
			if(_formConfirmList==null || _formConfirmList.IsDisposed) {
				_formConfirmList=new FormConfirmList();
			}
			_formConfirmList.Show();
			if(_formConfirmList.WindowState==FormWindowState.Minimized) {
				_formConfirmList.WindowState=FormWindowState.Normal;
			}
			_formConfirmList.BringToFront();
		}

		private void ListPlanned_Click() {
			if(_formTrackNext==null || _formTrackNext.IsDisposed) {
				_formTrackNext=new FormTrackNext();
			}
			_formTrackNext.Show();
			if(_formTrackNext.WindowState==FormWindowState.Minimized) {
				_formTrackNext.WindowState=FormWindowState.Normal;
			}
			_formTrackNext.BringToFront();
		}

		private void ListUnsched_Click() {
			//Reselect existing window if available, if not create a new instance
			if(_formUnsched==null || _formUnsched.IsDisposed) {
				_formUnsched=new FormUnsched();
			}
			_formUnsched.Show();
			if(_formUnsched.WindowState==FormWindowState.Minimized) {//only applicable if re-using an existing instance
				_formUnsched.WindowState=FormWindowState.Normal;
			}
			_formUnsched.BringToFront();
		}

		private void ListASAP_Click() {
			if(_formASAP==null || _formASAP.IsDisposed) {
				_formASAP=new FormASAP();
			}
			_formASAP.Show();
			if(_formASAP.WindowState==FormWindowState.Minimized) {
				_formASAP.WindowState=FormWindowState.Normal;
			}
			_formASAP.BringToFront();
		}

		private void ListRadiology_Click() {
			List<FormRadOrderList> listFormROLs=Application.OpenForms.OfType<FormRadOrderList>().ToList();
			if(listFormROLs.Count > 0) {
				listFormROLs[0].RefreshRadOrdersForUser(Security.CurUser);
				listFormROLs[0].BringToFront();
			}
			else {
				FormRadOrderList FormPRL=new FormRadOrderList(Security.CurUser);
				FormPRL.Show();
			}
		}

		private void ListInsVerify_Click() {
			List<FormInsVerificationList> listFormROLs=Application.OpenForms.OfType<FormInsVerificationList>().ToList();
			if(listFormROLs.Count>0) {
				listFormROLs[0].FillGrids();
				listFormROLs[0].BringToFront();
			}
			else if(Security.IsAuthorized(Permissions.InsuranceVerification)) {
				FormInsVerificationList FormIVL=new FormInsVerificationList();
				FormIVL.FormClosing+=FormIVL_FormClosing;
				FormIVL.Show();
			}
		}

		private void FormIVL_FormClosing(object sender,FormClosingEventArgs e) {
			//Action does not currently need to be taken when leaving the insurance verification list window.
		}

		private void toolBarPrint_Click() {
			if(contrApptPanel.ListOpsVisible.Count==0) {//no ops visible.
				MsgBox.Show("There must be at least one operatory showing in order to Print Appointments.");
				return;
			}
			if(PrinterSettings.InstalledPrinters.Count==0) {
				MessageBox.Show("Printer not installed");
				return;
			}
			List<long> listVisOpNums=contrApptPanel.ListOpsVisible.Select(x => x.OperatoryNum).ToList();
			List<long> listApptNums=new List<long>();
			for(int i=0;i<contrApptPanel.TableAppointments.Rows.Count;i++){
				long opNum=PIn.Long(contrApptPanel.TableAppointments.Rows[i]["Op"].ToString());
				if(listVisOpNums.Contains(opNum)){
					listApptNums.Add(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["AptNum"].ToString()));
				}
			}
			FormApptPrintSetup formApptPrintSetup=new FormApptPrintSetup(listApptNums);
			formApptPrintSetup.ShowDialog();
			if(formApptPrintSetup.DialogResult!=DialogResult.OK) {
				return;
			}
			contrApptPanel.DateTimePrintStart=formApptPrintSetup.ApptPrintStartTime;
			contrApptPanel.DateTimePrintStop=formApptPrintSetup.ApptPrintStopTime;
			contrApptPanel.PrintingSizeFont=formApptPrintSetup.ApptPrintFontSize;
			contrApptPanel.PrintingColsPerPage=formApptPrintSetup.ApptPrintColsPerPage;
			contrApptPanel.IsPrintPreview=formApptPrintSetup.IsPrintPreview;
			contrApptPanel.PagesPrinted=0;
			contrApptPanel.PrintingPageRow=0;
			contrApptPanel.PrintingPageColumn=0;
			DateTime dateTimePrintStart=formApptPrintSetup.ApptPrintStartTime;//to avoid marshal by reference error in next line
			PrinterL.TryPrintOrDebugClassicPreview(contrApptPanel.PrintPage,
				"Daily appointment view for "+dateTimePrintStart.ToShortDateString()+" printed",
				totalPages:0,
				printSit:PrintSituation.Appointments,
				isForcedPreview:contrApptPanel.IsPrintPreview
			);
			if(_patCur==null) {
				ModuleSelected(0);
			}
			else {
				ModuleSelected(_patCur.PatNum);
			}
		}
		#endregion Events - toolBarMain

		#region Events - PanelCalendar Upper
		///<summary>Clicked today.</summary>
		private void butToday_Click(object sender,System.EventArgs e) {
			SetDateSelected(DateTimeOD.Today);
		}

		///<summary>Clicked back one day.</summary>
		private void butBack_Click(object sender,System.EventArgs e) {
			SetDateSelected(contrApptPanel.DateSelected.AddDays(-1));
		}

		///<summary>Clicked forward one day.</summary>
		private void butFwd_Click(object sender,System.EventArgs e) {
			SetDateSelected(contrApptPanel.DateSelected.AddDays(1));
		}
				
		private void butBackMonth_Click(object sender,EventArgs e) {
			SetDateSelected(contrApptPanel.DateSelected.AddMonths(-1));
		}

		private void butBackWeek_Click(object sender,EventArgs e) {
			SetDateSelected(contrApptPanel.DateSelected.AddDays(-7));
		}

		private void butFwdWeek_Click(object sender,EventArgs e) {
			SetDateSelected(contrApptPanel.DateSelected.AddDays(7));
		}

		private void butFwdMonth_Click(object sender,EventArgs e) {
			SetDateSelected(contrApptPanel.DateSelected.AddMonths(1));
		}

		private void radioDay_Click(object sender,EventArgs e) {
			SetWeeklyView(false);
		}

		private void radioWeek_Click(object sender,EventArgs e) {
			SetWeeklyView(true);
		}

		///<summary>Clicked a date on the calendar.</summary>
		private void Calendar2_DateSelected(object sender,System.Windows.Forms.DateRangeEventArgs e) {
			//Do not use the DateChanged event, because that reacts to programmatic changes to the calendar date, causing weird loops.
			SetDateSelected(Calendar2.SelectionStart);
		}

		///<summary></summary>
		private void comboView_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboView.SelectedIndex==0) {
				SetView(0,true);
			}
			else {
				SetView(_listApptViews[comboView.SelectedIndex-1].ApptViewNum,true);
			}
		}
		#endregion Events - PanelCalendar Upper

		#region Events - PanelCalendar Buttons
		//Left Buttons------------------------------------------------------------------------------------------------
		///<summary>Sends current appointment to unscheduled list.</summary>
		private void butUnsched_Click(object sender,System.EventArgs e) {
			if(contrApptPanel.SelectedAptNum==-1) {
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			Appointment appt = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appt)) {
				return;
			}
			if(!AppointmentL.ValidateApptUnsched(appt)) {
				return;
			}
			if(PrefC.GetBool(PrefName.UnscheduledListNoRecalls) && Appointments.IsRecallAppointment(appt)) {
				if(MsgBox.Show(MsgBoxButtons.YesNo,"Recall appointments cannot be sent to the Unscheduled List.\r\nDelete appointment instead?")) {
					butDelete_Click(this,new EventArgs());//using "this" hides the normal prompt.
				}
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Send Appointment to Unscheduled List?")) {
				return;
			}	
			Patient pat=Patients.GetPat(appt.PatNum);
			AppointmentL.SetApptUnschedHelper(appt,pat);
			ModuleSelected(pat.PatNum);
			Plugins.HookAddCode(this,"ContrAppt.OnUnsched_Click_end",appt,_patCur);
		}

		private void butBreak_Click(object sender,System.EventArgs e) {
			if(PrefC.GetBool(PrefName.BrokenApptAdjustment) 
				&& PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType)==0) 
			{
				//They want broken appointment adjustments but don't have it set up.
				MsgBox.Show("Broken appointment adjustment type is not setup yet.  Please go to Setup | Appointment | Appts Preferences to fix this.");
				return;
			}
			if(contrApptPanel.SelectedAptNum==-1) {
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			Appointment appt = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appt)) {
				return;
			}
			Patient pat=Patients.GetPat(appt.PatNum);
			if((appt.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentEdit)) //seperate permissions for completed appts.
				|| (appt.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentCompleteEdit))) 
			{
				return;
			}
			if(appt.AptStatus == ApptStatus.PtNote || appt.AptStatus == ApptStatus.PtNoteCompleted) {
				MsgBox.Show("Only appointments may be broken, not notes.");
				return;
			}
			ProcedureCode procCodeBroke=null;//Will not chart if it stays null.
			ApptBreakSelection postBreakSelection=ApptBreakSelection.None;
			bool hasBrokenProcs=AppointmentL.HasBrokenApptProcs();//When true, we show FormApptBreak.cs
			if(hasBrokenProcs) {//If true, user cannot get here from right click 'Break Appointment' directly.
				FormApptBreak formApptBreak=new FormApptBreak(appt);
				if(formApptBreak.ShowDialog()!=DialogResult.OK) {
					return;
				}
				procCodeBroke=formApptBreak.SelectedProcCode;
				postBreakSelection=formApptBreak.FormApptBreakSelection;
			}
			else if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Break appointment?")) {
				return;
			}
			//This hook is specifically called after we know a valid appointment has been identified.
			Plugins.HookAddCode(this, "ContrAppt.OnBreak_Click_validation_end",appt);
			AppointmentL.BreakApptHelper(appt,pat,procCodeBroke);
			if(hasBrokenProcs) {//FormApptBreak was shown and user made a selection
				switch(postBreakSelection) {
					case ApptBreakSelection.Unsched:
						if(AppointmentL.ValidateApptUnsched(appt)) {
							AppointmentL.SetApptUnschedHelper(appt,pat,false);
						}
						break;
					case ApptBreakSelection.Pinboard:
						if(AppointmentL.ValidateApptToPinboard(appt)) {
							AppointmentL.CopyAptToPinboardHelper(appt);
						}
						break;
					case ApptBreakSelection.ApptBook:
						//Intentionally blank.
						break;
				}
			}
			ModuleSelected(pat.PatNum);//Must be ran after the "D9986" break logic due to the addition of a completed procedure.
			Plugins.HookAddCode(this,"ContrAppt.OnBreak_Click_end",appt,_patCur);
		}

		private void butComplete_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AppointmentEdit)) {
				return;
			}
			if(contrApptPanel.SelectedAptNum==-1) {
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			Appointment appt = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appt)) {
				return;
			}
			Patient pat=Patients.GetPat(appt.PatNum);
			if(appt.AptDateTime.Date>DateTime.Today) {
				if(!PrefC.GetBool(PrefName.ApptAllowFutureComplete)){
					MsgBox.Show(this,"Not allowed to set future appointments complete.");
					return;
				}
			}
			List<Procedure> listProcs=Procedures.GetProcsForSingle(appt.AptNum,false);
			if(appt.AptStatus!=ApptStatus.PtNote && appt.AptStatus!=ApptStatus.PtNoteCompleted  //Ptnote cannot have procs attached
				&& !PrefC.GetBool(PrefName.ApptAllowEmptyComplete)//Appointments must have at least 1 proc
				&& listProcs.Count==0)
			{
				MsgBox.Show("Appointments without procedures attached cannot be set complete.");
				return;
			}
			if(appt.AptStatus == ApptStatus.PtNoteCompleted) {
				return;
			}
			if(ProcedureCodes.DoAnyBypassLockDate()) {
				foreach(Procedure proc in listProcs) {
					if(!Security.IsAuthorized(Permissions.ProcComplCreate,appt.AptDateTime,proc.CodeNum,proc.ProcFee)) {
						return;
					}
				}
			}
			else if(!Security.IsAuthorized(Permissions.ProcComplCreate,appt.AptDateTime)) {
				return;
			}
			if(listProcs.Count>0 && appt.AptDateTime.Date>DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show("Not allowed to set procedures complete with future dates.");
				return;
			}
			#region Provider Term Date Check
			//Prevents appointments with providers that are past their term end date from being completed
			string message=Providers.CheckApptProvidersTermDates(appt,isSetComplete:true);
			if(message!="") {
				MsgBox.Show(message);
				return;
			}
			#endregion Provider Term Date Check
			bool removeCompletedProcs=ProcedureL.DoRemoveCompletedProcs(appt,listProcs.FindAll(x => x.ProcStatus==ProcStat.C));
			ODTuple<Appointment,List<Procedure>> result=Appointments.CompleteClick(appt,listProcs,removeCompletedProcs);
			appt=result.Item1;
			listProcs=result.Item2;
			if(appt.AptStatus!=ApptStatus.PtNote) {
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,listProcs.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).ToList(),appt.PatNum);
			}
			ModuleSelected(appt.PatNum);
			AppointmentEvent.Fire(ODEventType.AppointmentEdited,appt);
			Plugins.HookAddCode(this,"ContrAppt.OnComplete_Click_end",appt,_patCur);
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(contrApptPanel.SelectedAptNum==-1){
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			Appointment appt = Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(appt)) { return; }
			if((appt.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentEdit)) //seperate permission for completed appts.
				|| (appt.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentCompleteEdit))) 
			{
				return;
			}
			DataRow dataRow=contrApptPanel.GetDataRowForSelected();
			if(dataRow==null){
				MsgBox.Show("Appointment not found.");
				return;
			}
			bool showDeletePrompt=true;
			if(sender==this){//this method was called from somewhere else in this class rather than the delete button.
				//Already prompted in that other place.  If we want a prompt, use butDelete as the sender.
				showDeletePrompt=false;
			}
			if(appt.AptStatus == ApptStatus.PtNote | appt.AptStatus == ApptStatus.PtNoteCompleted) {
				if(showDeletePrompt && !MsgBox.Show(MsgBoxButtons.OKCancel,"Delete Patient Note?")) {
					return;
				}
				if(appt.Note != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(appt.Note,appt.AptStatus),"Question...",MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog CommlogCur = new Commlog();
						CommlogCur.PatNum = appt.PatNum;
						CommlogCur.CommDateTime = DateTime.Now;
						CommlogCur.CommType =Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						CommlogCur.Note = "Deleted Patient NOTE from schedule, saved copy: ";
						CommlogCur.Note += appt.Note;
						CommlogCur.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(CommlogCur);
					}
				}
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,_patCur.PatNum,
					dataRow["procs"].ToString()+", "+dataRow["AptDateTime"].ToString()+", "+"NOTE Deleted",
					appt.AptNum,appt.DateTStamp);
			}
			else {
				if(showDeletePrompt && !MsgBox.Show(MsgBoxButtons.OKCancel,"Delete Appointment?")) {
					return;
				}
				if(appt.Note != "") {
					if(MessageBox.Show(Commlogs.GetDeleteApptCommlogMessage(appt.Note,appt.AptStatus),"Question...",MessageBoxButtons.YesNo) == DialogResult.Yes) {
						Commlog CommlogCur = new Commlog();
						CommlogCur.PatNum = appt.PatNum;
						CommlogCur.CommDateTime = DateTime.Now;
						CommlogCur.CommType =Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						CommlogCur.Note = "Deleted Appointment & saved note: ";
						if(appt.ProcDescript != "") {
							CommlogCur.Note += appt.ProcDescript + ": ";
						}
						CommlogCur.Note += appt.Note;
						CommlogCur.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(CommlogCur);
					}
				}
				if(appt.AptStatus!=ApptStatus.Complete) {// seperate log entry for editing completed appointments.
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,_patCur.PatNum,
						dataRow["procs"].ToString()+", "+dataRow["AptDateTime"].ToString()+", "+"Deleted",
						appt.AptNum,appt.DateTStamp);
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,_patCur.PatNum,
						dataRow["procs"].ToString()+", "+dataRow["AptDateTime"].ToString()+", "+"Deleted",
						appt.AptNum,appt.DateTStamp);
				}
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S17 - Appt Deletion event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(_patCur,Patients.GetPat(_patCur.Guarantor),EventTypeHL7.S17,appt);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=appt.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=_patCur.PatNum;
						HL7Msgs.Insert(hl7Msg);
#if DEBUG
						MessageBox.Show(this,messageHL7.ToString());
#endif
					}
				}
			}
			Appointments.Delete(contrApptPanel.SelectedAptNum,true);//Appointments S-Class handles Signalods
			AppointmentEvent.Fire(ODEventType.AppointmentEdited,appt);
			contrApptPanel.SelectedAptNum=-1;
			pinBoard.SelectedIndex=-1;
			for(int i=0;i<pinBoard.ListPinBoardItems.Count;i++) {
				if(contrApptPanel.SelectedAptNum==pinBoard.ListPinBoardItems[i].AptNum) {
					pinBoard.SelectedIndex=i;
					pinBoard.ClearSelected();
					pinBoard.SelectedIndex=-1;
				}
			}
			if(_patCur==null) {
				ModuleSelected(0);
			}
			else {
				ModuleSelected(_patCur.PatNum);
			}
			Recalls.SynchScheduledApptFull(appt.PatNum);
			Plugins.HookAddCode(this,"ContrAppt.OnDelete_Click_end",appt,_patCur);
		}

		//Confirmation list------------------------------------------------------------------------------------------
		private void ListConfirmed_MouseDown(object sender, MouseEventArgs e){
			if(listConfirmed.IndexFromPoint(e.X,e.Y)==-1) {
				return;
			}
			if(contrApptPanel.SelectedAptNum==-1) {
				return;
			}
			Appointment aptOld=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(aptOld==null) {
				MsgBox.Show(this,"Patient appointment was removed.");
				contrApptPanel.SelectedAptNum=-1;
				ModuleSelected(_patCur.PatNum);//keep same pat
				return;
			}
			DateTime datePrevious=aptOld.DateTStamp;
			long newStatus=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true)[listConfirmed.IndexFromPoint(e.X,e.Y)].DefNum;
			long oldStatus=aptOld.Confirmed;
			Appointments.SetConfirmed(aptOld,newStatus);//Appointments S-Class handles Signalods
			if(newStatus!=oldStatus) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,_patCur.PatNum,Lans.g(this,"Appointment confirmation status changed from")+" "
					+Defs.GetName(DefCat.ApptConfirmed,oldStatus)+" "+Lans.g(this,"to")+" "+Defs.GetName(DefCat.ApptConfirmed,newStatus)
					+" "+Lans.g(this,"from the appointment module")+".",contrApptPanel.SelectedAptNum,datePrevious);
			}
			RefreshPeriod();
			AppointmentL.ShowKioskManagerIfNeeded(aptOld,newStatus);
		}

		//Right Make Buttons----------------------------------------------------------------------------------------
		private void butMakeAppt_Click(object sender,System.EventArgs e) {
			if(_patCur==null) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			if(AppointmentL.PromptForMerge(_patCur,out _patCur)) {
				RefreshModuleDataPatient(_patCur.PatNum);
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
			if(_patCur!=null && _patCur.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show("Appointments cannot be scheduled for "+_patCur.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			if(Appointments.HasOutstandingAppts(_patCur.PatNum)) {
				DisplayOtherDlg(false);
				return;
			}
			FormApptsOther formApptsOther=new FormApptsOther(_patCur.PatNum);//doesn't actually get shown
			CheckStatus();
			formApptsOther.IsInitialClick=false;
			formApptsOther.MakeAppointment();
			SendToPinBoardAptNums(formApptsOther.AptNumsSelected);
		}

		private void butMakeRecall_Click(object sender,EventArgs e) {
			if(_patCur==null) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(_patCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			if(AppointmentL.PromptForMerge(_patCur,out _patCur)) {
				RefreshModuleDataPatient(_patCur.PatNum);
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
			if(_patCur!=null && _patCur.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show("Appointments cannot be scheduled for "+_patCur.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			if(Appointments.HasOutstandingAppts(_patCur.PatNum,true)) {
				DisplayOtherDlg(false);
				return;
			}
			FormApptsOther formApptsOther=new FormApptsOther(_patCur.PatNum);//doesn't actually get shown
			formApptsOther.IsInitialClick=false;
			formApptsOther.MakeRecallAppointment();
			if(formApptsOther.DialogResult!=DialogResult.OK) {
				return;
			}
			SendToPinBoardAptNums(formApptsOther.AptNumsSelected);
			if(contrApptPanel.IsWeeklyView) {
				return;
			}
			dateSearch.Text=formApptsOther.DateJumpToString;
			if(!groupSearch.Visible) {//if search not already visible
				ShowSearch();
			}
			DoSearch();
		}

		private void butFamRecall_Click(object sender,EventArgs e) {
			if(_patCur==null) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(Appointments.HasOutstandingAppts(_patCur.PatNum)) {
				DisplayOtherDlg(false);
				return;
			}
			FormApptsOther formApptsOther=new FormApptsOther(_patCur.PatNum);//doesn't actually get shown
			formApptsOther.IsInitialClick=false;
			formApptsOther.MakeRecallFamily();
			if(formApptsOther.DialogResult!=DialogResult.OK) {
				return;
			}
			SendToPinBoardAptNums(formApptsOther.AptNumsSelected);
			if(contrApptPanel.IsWeeklyView) {
				return;
			}
			dateSearch.Text=formApptsOther.DateJumpToString;
			if(!groupSearch.Visible) {//if search not already visible
				ShowSearch();
			}
			DoSearch();
		}

		private void butViewAppts_Click(object sender,EventArgs e) {
			DisplayOtherDlg(false);
		}
		#endregion Events - PanelCalendar Buttons

		#region Events - pinBoard
		private void pinBoard_SelectedIndexChanged(object sender,EventArgs e) {
			contrApptPanel.SelectedAptNum=-1;
			RefreshModuleScreenButtonsRight();
		}

		private void pinBoard_PreparingToDragFromPinboard(object sender,UI.ApptDataEventArgs e) {
			string pattern=PIn.String(e.DataRowAppt["Pattern"].ToString());
			string patternShowing=contrApptPanel.GetPatternShowing(pattern);
			SizeF sizeAppt=contrApptPanel.SetSize(pattern);
			//the whole point of having all of this here is to set the width of the dragging appt to be same as in the main area instead of like pinboard
			Bitmap bitmap=new Bitmap((int)sizeAppt.Width,(int)sizeAppt.Height);
			using(Graphics g = Graphics.FromImage(bitmap)){
				contrApptPanel.GetBitmapForPinboard(g,e.DataRowAppt,patternShowing,bitmap.Width,bitmap.Height);
			}
			pinBoard.SetBitmapTempPinAppt(bitmap);
			bitmap.Dispose();//?
		}
		
		private void pinBoard_ApptMovedFromPinboard(object sender,UI.ApptDataEventArgs e) {
			//try/finally only. No catch. We probably want to have an exception popup if there is a real bug.
			//Any return from this point forward will cause HideDraggableTempApptSingle();					
			try {
				//Make sure there are operatories for the appointment to be scheduled and make sure the user dragged the appointment to a valid location.
				if(contrApptPanel.ListOpsVisible.Count==0) {
					return;
				}
				ApptStatus apptStatus=(ApptStatus)PIn.Int(e.DataRowAppt["AptStatus"].ToString());
				long patNum=PIn.Long(e.DataRowAppt["PatNum"].ToString());
				if(apptStatus==ApptStatus.Planned//if Planned appt is on pinboard
					&&(!Security.IsAuthorized(Permissions.AppointmentCreate)//and no permission to create a new appt
						||PatRestrictionL.IsRestricted(patNum,PatRestrict.ApptSchedule)))//or pat restricted
				{
					return;
				}
				//security prevents moving an appointment by preventing placing it on the pinboard, not here
				//We do not ask user, "Move Appointment?" because that's just slow.
				//convert loc to new time
				Appointment apptCur=Appointments.GetOneApt(PIn.Long(e.DataRowAppt["AptNum"].ToString()));
				if(apptCur==null) {
					MsgBox.Show("This appointment has been deleted since it was moved to the pinboard. It will now be cleared from the pinboard.");
					pinBoard.ClearSelected();
					return;
				}
				//This hook is specifically called after we know a valid appointment has been identified.
				Plugins.HookAddCode(this, "ContrAppt.pinBoard_MouseUp_validation_end",apptCur);
				Appointment apptOld=apptCur.Copy();
				RefreshModuleDataPatient(apptCur.PatNum);//redundant?
																	   //Patient pat=Patients.GetPat(pinBoard.SelectedAppt.PatNum);
				if(apptCur.IsNewPatient && contrApptPanel.DateSelected!=apptCur.AptDateTime) {
					Procedures.SetDateFirstVisit(contrApptPanel.DateSelected,4,_patCur);
				}
				//e.Location is in ContrAppt coords
				TimeSpan? timeSpanNew=contrApptPanel.YPosToTime(e.Location.Y-contrApptPanel.Top);//in contrApptPanel coords
				if(timeSpanNew==null){
					return;
				}
				TimeSpan timeSpanNewRounded=UI.UserControlApptsPanelJ.RoundTimeToNearestIncrement(timeSpanNew.Value,contrApptPanel.MinPerIncr);
				contrApptPanel.RoundToNearestDateAndOp(e.Location.X-contrApptPanel.Location.X,//passing in as coordinates of the control
					out DateTime dateNew,
					out int opIdx,e.BitmapAppt.Width);
				if(opIdx<0){
					MsgBox.Show("Invalid operatory");
					return;
				}
				apptCur.AptDateTime=dateNew+timeSpanNewRounded;
				//Compare beginning of new appointment against end to see if the appointment spans two days
				if(apptCur.AptDateTime.Day!=apptCur.AptDateTime.AddMinutes(apptCur.Pattern.Length*5).Day) {
					MsgBox.Show("You cannot have an appointment that starts and ends on different days.");
					return;
				}
				//Prevent double-booking
				if(contrApptPanel.IsDoubleBooked(apptCur)) {
					return;
				}
				Operatory opCur=contrApptPanel.ListOpsVisible[opIdx];
				apptCur.Op=opCur.OperatoryNum;
					//opCur.OperatoryNum;
				//Set providers----------------------Similar to UpdateAppointments()
				long assignedDent=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,opCur,false,apptCur.AptDateTime);
				long assignedHyg=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,opCur,true,apptCur.AptDateTime);
				List<Procedure> procsForSingleApt=null;
				if(apptCur.AptStatus!=ApptStatus.PtNote&&apptCur.AptStatus!=ApptStatus.PtNoteCompleted) {
					#region Update Appt's DateTimeAskedToArrive
					if(_patCur.AskToArriveEarly>0) {
						apptCur.DateTimeAskedToArrive=apptCur.AptDateTime.AddMinutes(-_patCur.AskToArriveEarly);
						MessageBox.Show("Ask patient to arrive "+_patCur.AskToArriveEarly
							+" minutes early at "+apptCur.DateTimeAskedToArrive.ToShortTimeString()+".");
					}
					else {
						apptCur.DateTimeAskedToArrive=DateTime.MinValue;
					}
					#endregion Update Appt's DateTimeAskedToArrive
					#region Update Appt's Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
					//if no dentist/hygienist is assigned to spot, then keep the original dentist/hygienist without prompt.  All appts must have prov.
					if((assignedDent!=0&&assignedDent!=apptCur.ProvNum)||(assignedHyg!=0&&assignedHyg!=apptCur.ProvHyg)) {
						if(MsgBox.Show(MsgBoxButtons.YesNo,"Change provider?")) {
							if(assignedDent!=0) {//the dentist will only be changed if the spot has a dentist.
								apptCur.ProvNum=assignedDent;
							}
							if(assignedHyg!=0||PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {//the hygienist will only be changed if the spot has a hygienist.
								apptCur.ProvHyg=assignedHyg;
							}
							if(opCur.IsHygiene) {
								apptCur.IsHygiene=true;
							}
							else {//op not marked as hygiene op
								if(assignedDent==0) {//no dentist assigned
									if(assignedHyg!=0) {//hyg is assigned (we don't really have to test for this)
										apptCur.IsHygiene=true;
									}
								}
								else {//dentist is assigned
									if(assignedHyg==0) {//hyg is not assigned
										apptCur.IsHygiene=false;
									}
									//if both dentist and hyg are assigned, it's tricky
									//only explicitly set it if user has a dentist assigned to the op
									if(opCur.ProvDentist!=0) {
										apptCur.IsHygiene=false;
									}
								}
							}
							bool isplanned=apptCur.AptStatus==ApptStatus.Planned;
							procsForSingleApt=Procedures.GetProcsForSingle(apptCur.AptNum,isplanned);
							List<long> codeNums=new List<long>();
							for(int p = 0;p<procsForSingleApt.Count;p++) {
								codeNums.Add(procsForSingleApt[p].CodeNum);
							}
							string calcPattern=Appointments.CalculatePattern(apptCur.ProvNum,apptCur.ProvHyg,codeNums,true);
							if(apptCur.Pattern!=calcPattern && !PrefC.GetBool(PrefName.AppointmentTimeIsLocked)) {
								if(apptCur.TimeLocked) {
									if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Appointment length is locked.  Change length for new provider anyway?")) {
										apptCur.Pattern=calcPattern;
									}
								}
								else {//appt time not locked
									if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change length for new provider?")) {
										apptCur.Pattern=calcPattern;
									}
								}
							}
						}
					}
					#region Provider Term Date Check
					//Prevents appointments with providers that are past their term end date from being scheduled
					string message=Providers.CheckApptProvidersTermDates(apptCur);
					if(message!="") {
						MessageBox.Show(message);//translated in Providers S class method
						return;
					}
					#endregion Provider Term Date Check
					#endregion Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
				}
				#region Prevent overlap
				if(!AllowOverlap){
					if(!Appointments.TryAdjustAppointmentOp(apptCur,contrApptPanel.ListOpsVisible)) {
						MessageBox.Show("Appointment overlaps existing appointment or blockout.");
						return;
					}
				}
				#endregion Prevent overlap
				#region Detect Frequency Conflicts
				//Detect frequency conflicts with procedures in the appointment
				if(PrefC.GetBool(PrefName.InsChecksFrequency)) {
					procsForSingleApt=Procedures.GetProcsForSingle(apptCur.AptNum,false);
					string frequencyConflicts=""; 
					try {
						frequencyConflicts=Procedures.CheckFrequency(procsForSingleApt,apptCur.PatNum,apptCur.AptDateTime);
					}
					catch(Exception ex) {
						MessageBox.Show("There was an error checking frequencies."
							+"  Disable the Insurance Frequency Checking feature or try to fix the following error:"
							+"\r\n"+ex.Message);
						return;
					}
					if(frequencyConflicts!="" && MessageBox.Show("Scheduling this appointment for this date will cause frequency conflicts for the following procedures"
							+":\r\n"+frequencyConflicts+"\r\n"+"Do you want to continue?","",MessageBoxButtons.YesNo)==DialogResult.No) 
					{
						return;
					}
				}
				#endregion Detect Frequency Conflicts
				#region Patient status
				//Operatory opCur=Operatories.GetOperatory(apptCur.Op);
				Operatory opOld=Operatories.GetOperatory(apptOld.Op);
				if(opOld==null||opCur.SetProspective!=opOld.SetProspective) {
					if(opCur.SetProspective && _patCur.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
						if(MsgBox.Show(MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
							Patient patOld=_patCur.Copy();
							_patCur.PatStatus=PatientStatus.Prospective;
							Patients.Update(_patCur,patOld);
						}
					}
					else if(!opCur.SetProspective && _patCur.PatStatus==PatientStatus.Prospective) {
						//Do we need to warn about changing FROM prospective? Assume so for now.
						if(MsgBox.Show(MsgBoxButtons.OKCancel,"Patient's status will change from Prospective to Patient.")) {
							Patient patOld=_patCur.Copy();
							_patCur.PatStatus=PatientStatus.Patient;
							Patients.Update(_patCur,patOld);
						}
					}
				}
				#endregion Patient status
				#region Update Appt's AptStatus, ClinicNum
				if(apptCur.AptStatus==ApptStatus.Broken) {
					apptCur.AptStatus=ApptStatus.Scheduled;
				}
				if(apptCur.AptStatus==ApptStatus.UnschedList) {
					apptCur.AptStatus=ApptStatus.Scheduled;
				}
				//original position of provider settings
				if(opCur.ClinicNum==0) {
					apptCur.ClinicNum=_patCur.ClinicNum;
				}
				else {
					apptCur.ClinicNum=opCur.ClinicNum;
				}
				#endregion Update Appt's AptStatus, ClinicNum
				bool isCreate=false;
				#region Update/Insert Appt in db
				if(!AppointmentL.IsSpecialtyMismatchAllowed(_patCur.PatNum,apptCur.ClinicNum)) {//ClinicNum just set using either opCur or PatCur.
					return;
				}
				if(apptCur.AptStatus==ApptStatus.Planned) {//if Planned appt is on pinboard
					MsgBox.Show("Planned appts not yet implemented");
					return;
					//todo:
					#region Planned appointment
					/*
					List<ApptField> listApptFields=new List<ApptField>();
					for(int i = 0;i<pinBoard.SelectedAppt.TableApptFields.Rows.Count;i++) {//Duplicate the appointment fields.
						if(apptOld.AptNum==PIn.Long(pinBoard.SelectedAppt.TableApptFields.Rows[i]["AptNum"].ToString())) {
							ApptField apptField = new ApptField();
							apptField.FieldName=PIn.String(pinBoard.SelectedAppt.TableApptFields.Rows[i]["FieldName"].ToString());
							apptField.FieldValue=PIn.String(pinBoard.SelectedAppt.TableApptFields.Rows[i]["FieldValue"].ToString());
							listApptFields.Add(apptField);
						}
					}
					bool procAlreadyAttached;
					try {
						ODTuple<Appointment,bool> aptTuple=Appointments.SchedulePlannedApt(apptCur,PatCur,listApptFields,apptCur.AptDateTime,apptCur.Op);//Appointments S-Class handles Signalods
						apptCur=aptTuple.Item1;
						procAlreadyAttached=aptTuple.Item2;
						isCreate=true;
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
					if(procAlreadyAttached) {
						MessageBox.Show("One or more procedures could not be scheduled because they were already attached to another appointment. "
							+"Someone probably forgot to update the Next appointment in the Chart module.");
						FormApptEdit formAE=new FormApptEdit(apptCur.AptNum);
						CheckStatus();
						formAE.IsNew=true;
						formAE.ShowDialog();//to force refresh of aptDescript
						if(formAE.DialogResult!=DialogResult.OK) {//apt gets deleted from within aptEdit window.
							RefreshModuleScreenPatient();
							RefreshPeriod();
							return;
						}
					}
					else {
						SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,apptCur.PatNum,
							apptCur.AptDateTime.ToString()+", "+apptCur.ProcDescript,
							apptCur.AptNum,apptOld.DateTStamp);
					}
					procsForSingleApt=Procedures.GetProcsForSingle(apptCur.AptNum,false);*/
					#endregion Planned appointment
				}
				else {//simple drag off pinboard to a new date/time
					#region Previously scheduled appointment (not a planned appointment)
					apptCur.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;//Causes the confirmation status to be reset.
					try {
						Appointments.Update(apptCur,apptOld);//Appointments S-Class handles Signalods
						if(apptOld.AptStatus==ApptStatus.UnschedList&&apptOld.AptDateTime==DateTime.MinValue) { //If new appt is being added to schedule from pinboard
							SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,apptCur.PatNum,
								apptCur.AptDateTime.ToString()+", "+apptCur.ProcDescript,
								apptCur.AptNum,apptOld.DateTStamp);
							isCreate=true;
						}
						else { //If existing appt is being moved
							SecurityLogs.MakeLogEntry(Permissions.AppointmentMove,apptCur.PatNum,
								apptCur.ProcDescript+", from "+apptOld.AptDateTime.ToString()+", to "+apptCur.AptDateTime.ToString(),
								apptCur.AptNum,apptOld.DateTStamp);
							if(apptOld.AptStatus==ApptStatus.UnschedList) {
								isCreate=true;
							}
						}
						if(apptCur.Confirmed!=apptOld.Confirmed) {
							//Log confirmation status changes.
							SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,apptCur.PatNum,
								"Appointment confirmation status automatically changed from "
								+Defs.GetName(DefCat.ApptConfirmed,apptOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,apptCur.Confirmed)
								+" from the appointment module"+".",apptCur.AptNum,apptOld.DateTStamp);
						}
						//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
						if(HL7Defs.IsExistingHL7Enabled()) {
							//S12 - New Appt Booking event, S13 - Appt Rescheduling
							MessageHL7 messageHL7=null;
							if(isCreate) {
								messageHL7=MessageConstructor.GenerateSIU(_patCur,Patients.GetPat(_patCur.Guarantor),EventTypeHL7.S12,apptCur);
							}
							else {
								messageHL7=MessageConstructor.GenerateSIU(_patCur,Patients.GetPat(_patCur.Guarantor),EventTypeHL7.S13,apptCur);
							}
							//Will be null if there is no outbound SIU message defined, so do nothing
							if(messageHL7!=null) {
								HL7Msg hl7Msg=new HL7Msg();
								hl7Msg.AptNum=apptCur.AptNum;
								hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
								hl7Msg.MsgText=messageHL7.ToString();
								hl7Msg.PatNum=_patCur.PatNum;
								HL7Msgs.Insert(hl7Msg);
#if DEBUG
								MessageBox.Show(this,messageHL7.ToString());
#endif
							}
						}
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
					#endregion Previously scheduled appointment (not a planned appointment)
				}
				#endregion Update/Insert Appt in db
				if(procsForSingleApt==null) {
					procsForSingleApt=Procedures.GetProcsForSingle(apptCur.AptNum,false);
				}
				#region Update UI and cache
				ProcFeeHelper procFeeHelper=new ProcFeeHelper(apptCur.PatNum);
				bool isUpdatingFees=false;
				List<Procedure> listProcsNew=procsForSingleApt.Select(x => Procedures.UpdateProcInAppointment(apptCur,x.Copy())).ToList();
				if(procsForSingleApt.Exists(x => x.ProvNum!=listProcsNew.FirstOrDefault(y => y.ProcNum==x.ProcNum).ProvNum)) {//Either the primary or hygienist changed.
					string promptText="";
					isUpdatingFees=Procedures.ShouldFeesChange(listProcsNew,procsForSingleApt,ref promptText,procFeeHelper);
					if(isUpdatingFees) {//Made it pass the pref check.
						if(promptText!="" && !MsgBox.Show(this,MsgBoxButtons.YesNo,promptText)) {
								isUpdatingFees=false;
						}
					}
				}
				Procedures.SetProvidersInAppointment(apptCur,procsForSingleApt,isUpdatingFees,procFeeHelper);
				pinBoard.ClearSelected();
				contrApptPanel.SelectedAptNum=apptCur.AptNum;
				//SetDateSelected(apptCur.AptDateTime);
				//RefreshModuleScreenButtonsRight();
				//RefreshPeriod(isRefreshSchedules:true);//date moving to for this computer; This line may not be needed
				RefreshPeriod();
				if(isCreate) {//new appointment is being added to the schedule from the pinboard, trigger ScheduleProcedure automation
					List<string> procCodes=procsForSingleApt.Select(x => ProcedureCodes.GetProcCode(x.CodeNum).ProcCode).ToList();					
					AutomationL.Trigger(AutomationTrigger.ScheduleProcedure,procCodes,apptCur.PatNum);
				}
				AppointmentEvent.Fire(ODEventType.AppointmentEdited,apptCur);
				Recalls.SynchScheduledApptFull(apptCur.PatNum);
				Plugins.HookAddCode(this,"ContrAppt.pinBoard_MouseUp_end",apptCur,_patCur);
				#endregion Update UI and cache
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
			finally {
				pinBoard.HideDraggableTempApptSingle();
			}
		}

		private void butClearPin_Click(object sender,EventArgs e) {
			if(pinBoard.ListPinBoardItems.Count==0) {
				MsgBox.Show("There are no appointments on the pinboard to clear.");
				return;
			}
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			DataRow dataRow=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].DataRowAppt;
			DateTime aptDateTime=PIn.DateT(dataRow["AptDateTime"].ToString());
			long aptNum=PIn.Long(dataRow["AptNum"].ToString());
			ApptStatus aptStatus=(ApptStatus)PIn.Int(dataRow["AptStatus"].ToString());
			if(aptStatus==ApptStatus.UnschedList) {//unscheduled status
				if(aptDateTime.Year<1880) {//Indicates that this was a brand new appt
					Appointment aptCur=Appointments.GetOneApt(aptNum);
					if(aptCur==null || aptCur.AptDateTime.Year>1880){//If appointment is already deleted or if date is now present
						//don't do anything to db.  Appt removed from pinboard above, and Refresh will happen below.
					}
					else{
						Appointments.Delete(aptNum,true);
					}
				}
				else {//was actually on the unscheduled list
					//do nothing to database
				}
			}
			else if(aptDateTime.Year>1880) {//already scheduled
				//do nothing to database
			}
			else if(aptStatus==ApptStatus.Planned) {
				//do nothing except remove it from pinboard
			}
			else {//Not sure when this would apply, since new appts start out as unsched.  Maybe patient notes?  Leave it just in case.
				//this gets rid of new appointments that never made it off the pinboard
				Appointments.Delete(aptNum,true);
			}
			pinBoard.ClearSelected();
			if(_patCur==null) {//not sure how to test this. Doesn't seem possible.
				RefreshModuleScreenButtonsRight();
			}
			else {
				ModuleSelected(_patCur.PatNum);
			}
			/*}
			else {
			RefreshModuleDataPatient(pinBoard.ApptList[pinBoard.SelectedIndex].PatNum);
			FormOpenDental.S_Contr_PatientSelected(PatCur,true,false);
			}*/
		}
		#endregion Events - pinBoard

		#region Events - LR Tabs
		private void gridEmpOrProv_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(contrApptPanel.IsWeeklyView) {
				MsgBox.Show("Not available in weekly view");
				return;
			}
			if(!Security.IsAuthorized(Permissions.Schedules)) {
				return;
			}
			FormScheduleDayEdit formScheduleDayEdit=new FormScheduleDayEdit(contrApptPanel.DateSelected,Clinics.ClinicNum);
			formScheduleDayEdit.ShowOkSchedule=true;
			formScheduleDayEdit.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Schedules,0,"");
			RefreshPeriod();
			if(formScheduleDayEdit.GotoScheduleOnClose) {
				FormSchedule formSchedule = new FormSchedule();
				formSchedule.ShowDialog();
			}
		}

		///<summary>Logic mimics UserControlTasks.gridMain_CellDoubleClick()</summary>
		private void gridReminders_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Col==0){
				//no longer allow double click on checkbox, because it's annoying.
				return;
			}
			ODGridRow row=gridReminders.Rows[e.Row];
			Task reminderTask=((Task)row.Tag);
			//It's important to grab the task directly from the db because the status in this list is fake, being the "unread" status instead.
			Task task=Tasks.GetOne(reminderTask.TaskNum);
			FormTaskEdit formTaskEdit=new FormTaskEdit(task);
			formTaskEdit.Show();//non-modal
		}

		private void timerWaitingRoom_Tick(object sender,EventArgs e) {
			FillWaitingRoom();
		}
		#endregion Events - LR Tabs

		#region Events - Menu Popup
		private void menuApt_Popup(object sender,EventArgs e) {
			if(menuItemBreakAppt==null) {
				return;
			}
			menuItemBreakAppt.MenuItems.Clear();
			menuItemBreakAppt.Tag=contrApptPanel.SelectedAptNum;//Refresh later, just in case.
			MenuItem item=null;
			bool dontAllowUnscheduled=PrefC.GetBool(PrefName.UnscheduledListNoRecalls) 
				&& Appointments.IsRecallAppointment(Appointments.GetOneApt(contrApptPanel.SelectedAptNum));
			BrokenApptProcedure brokenApptProcs=(BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
			/*
			if(brokenApptProcs.In(BrokenApptProcedure.Missed,BrokenApptProcedure.Both)) {
				if(dontAllowUnscheduled) {
					item=menuItemBreakAppt.MenuItems.Add("Missed - Delete Appointment");
					item.Click+=new EventHandler(BreakDeleteApptClick);
				}
				else {
					item=menuItemBreakAppt.MenuItems.Add("Missed - Send to Unscheduled List");
					item.Click+=new EventHandler(BreakToUnschedClick);
				}
				item.Tag=ProcedureCodes.GetProcCode("D9986");
				item=menuItemBreakAppt.MenuItems.Add("Missed - Copy To Pinboard");
				item.Click+=new EventHandler(BreakToPinClick);
				item.Tag=ProcedureCodes.GetProcCode("D9986");
				item=menuItemBreakAppt.MenuItems.Add("Missed - Leave on Appt Book");
				item.Click+=new EventHandler(BreakApptClick);
				item.Tag=ProcedureCodes.GetProcCode("D9986");
			}
			if(brokenApptProcs.In(BrokenApptProcedure.Cancelled,BrokenApptProcedure.Both)) {				
				if(menuItemBreakAppt.MenuItems.Count > 0) {
					menuItemBreakAppt.MenuItems.Add("-");//Horizontal bar.
				}
				if(dontAllowUnscheduled) {
					item=menuItemBreakAppt.MenuItems.Add("Cancelled - Delete Appointment");
					item.Click+=new EventHandler(BreakDeleteApptClick);
				}		
				else {
					item=menuItemBreakAppt.MenuItems.Add("Cancelled - Send to Unscheduled List");
					item.Click+=new EventHandler(BreakToUnschedClick);
				}
				item.Tag=ProcedureCodes.GetProcCode("D9987");
				item=menuItemBreakAppt.MenuItems.Add("Cancelled - Copy To Pinboard");
				item.Click+=new EventHandler(BreakToPinClick);
				item.Tag=ProcedureCodes.GetProcCode("D9987");
				item=menuItemBreakAppt.MenuItems.Add("Cancelled - Leave on Appt Book");
				item.Click+=new EventHandler(BreakApptClick);
				item.Tag=ProcedureCodes.GetProcCode("D9987");
			}*/
		}
		#endregion Events - Menu Popup

		#region Events - Menu Apt Click
		private void menuWeekly_Click(object sender,System.EventArgs e) {
			/*
			switch(((MenuItem)sender).Index) {
				case 0:
					CopyToPin_Click();
					break;
			}*/
		}

		private void menuApt_Click(object sender,System.EventArgs e) {
			switch(((MenuItem)sender).Name) {
				case MenuItemNames.CopyToPinboard: //Menu: Copy to Pinboard
					CopyToPin_Click();
					break;
				case MenuItemNames.CopyAppointmentStructure: //Menu: Copy Appointment Structure
					CopyApptStructure(Appointments.GetOneApt(contrApptPanel.ClickedAptNum));
					break;
				//1: divider
				case MenuItemNames.SendToUnscheduledList: //Menu: Send to Unscheduled List
					butUnsched_Click(this,new EventArgs());
					break;
				case MenuItemNames.BreakAppointment: //Menu: Break Appointment
					butBreak_Click(this,new EventArgs());
					break;
				case MenuItemNames.SetComplete: // Menu: Set Complete
					butComplete_Click(this,new EventArgs());
					break;
				case MenuItemNames.Delete: // Menu: Delete
					butDelete_Click(butDelete,new EventArgs());//setting sender to butDelete causes proper prompt
					break;
				case MenuItemNames.OtherAppointments: // Menu: Other Appointments
					DisplayOtherDlg(false);
					break;
				//8: divider
				case MenuItemNames.PrintLabel: // Menu: Print Label
					PrintApptLabel();
					break;
				case MenuItemNames.PrintCard: // Menu: Print Card
					_doPrintCardFamily=false;
					PrintApptCard();
					break;
				case MenuItemNames.PrintCardEntireFamily: // Menu: Print Card for Entire Family
					_doPrintCardFamily=true;
					PrintApptCard();
					break;
				case MenuItemNames.RoutingSlip: // Menu: Routing Slip
					//for now, this only allows one type of routing slip.  But it could be easily changed.
					Appointment apt=Appointments.GetOneApt(contrApptPanel.ClickedAptNum);
					if(ApptIsNull(apt)) { return; }
					FormRpRouting FormR=new FormRpRouting();
					FormR.AptNum=contrApptPanel.ClickedAptNum;
					List<SheetDef> customSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.RoutingSlip);
					if(customSheetDefs.Count==0) {
						FormR.SheetDefNum=0;
					}
					else {
						FormR.SheetDefNum=customSheetDefs[0].SheetDefNum;
					}
					FormR.ShowDialog();
					break;
				case MenuItemNames.OrthoChart: //Open Patient Ortho Chart
					FormOrthoChart FormOC=new FormOrthoChart(_patCur);
					FormOC.ShowDialog();
					break;
				case MenuItemNames.HomePhone: //Call Home Phone
					if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
						Bridges.DentalTek.PlaceCall(_patCur.HmPhone);
					}
					else {
						AutomaticCallDialingDisabledMessage();
					}
					 break;
				case MenuItemNames.WorkPhone: //Call Work Phone
					if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
						Bridges.DentalTek.PlaceCall(_patCur.WkPhone);
					}
					else {
						AutomaticCallDialingDisabledMessage();
					}
					break;
				case MenuItemNames.WirelessPhone: //Call Wireless Phone
					if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
						Bridges.DentalTek.PlaceCall(_patCur.WirelessPhone);
					}
					else {
						AutomaticCallDialingDisabledMessage();
					}
					break;
				case MenuItemNames.SendText:
					Appointment appt=Appointments.GetOneApt(contrApptPanel.ClickedAptNum);
					if(ApptIsNull(appt)) { return; }
					FormOpenDental.S_OnTxtMsg_Click(appt.PatNum,"");
					break;
				case MenuItemNames.SendConfirmationText:
					appt=Appointments.GetOneApt(contrApptPanel.ClickedAptNum);
					if(ApptIsNull(appt)) { return; }
					Patient pat=Patients.GetPat(appt.PatNum);
					string message=PrefC.GetString(PrefName.ConfirmTextMessage);
					message=message.Replace("[NameF]",pat.GetNameFirst());
					message=message.Replace("[NameFL]",pat.GetNameFL());
					message=message.Replace("[date]",appt.AptDateTime.ToString(PrefC.PatientCommunicationDateFormat));
					message=message.Replace("[time]",appt.AptDateTime.ToShortTimeString());
					bool wasTextSent=FormOpenDental.S_OnTxtMsg_Click(pat.PatNum,message);
					if(wasTextSent) {
						Appointments.SetConfirmed(appt,PrefC.GetLong(PrefName.ConfirmStatusTextMessaged));
					}
					break;
				/*case MenuItemNames.BringOverlapToFront:
					ContrApptSheet2.OverlapOrdering.CycleOverlappingAppts(contrApptPanel.SelectedAptNum);//Change priorities
					contrApptPanel.SelectedAptNum=-1;//removed selected appointment as to show changes with overlap ordering
					ContrApptSheet2.DoubleBufferDraw(createApptShadows:true);
					break;*/
			}
		}
		#endregion Events - Menu Appt Click

		#region Events - Menu Blockout Click
		private void menuBlockCopy_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			//not even enabled if not right click on a blockout
			Schedule SchedCur=GetClickedBlockout();
			if(SchedCur==null) {
				MessageBox.Show("Blockout not found.");
				return;//should never happen
			}
			_scheduleBlockoutClipboard=SchedCur.Copy();
		}

		private void menuBlockCut_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			//not even enabled if not right click on a blockout
			Schedule SchedCur=GetClickedBlockout();
			if(SchedCur==null) {
				MessageBox.Show("Blockout not found.");
				return;//should never happen
			}
			_scheduleBlockoutClipboard=SchedCur.Copy();
			Schedules.Delete(SchedCur);
			Schedules.BlockoutLogHelper(BlockoutAction.Cut,SchedCur);
			RefreshPeriodSchedules();
		}

		private void menuBlockPaste_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			Schedule sched=_scheduleBlockoutClipboard.Copy();
			sched.Ops=new List<long>();
			sched.Ops.Add(_blockoutClickedOnOp);
			sched.SchedDate=_dateTimeClickedBlockout.Date;
			TimeSpan timeSpanOriginalLength=sched.StopTime-sched.StartTime;
			sched.StartTime=UserControlApptsPanelJ.RoundTimeDown(_dateTimeClickedBlockout.TimeOfDay,contrApptPanel.MinPerIncr);
			sched.StopTime=sched.StartTime+timeSpanOriginalLength;
			if(sched.StopTime >= TimeSpan.FromDays(1)) {//long span that spills over to next day
				MsgBox.Show("This Blockout would go past midnight.");
				return;
			}
			sched.ScheduleNum=0;//Because Schedules.Overlaps() ignores matching ScheduleNums and we used the Copy() function above. Also, we insert below, so a new key will be created anyway.
			List<Schedule> listOverlapSchedules;
			if(Schedules.Overlaps(sched,out listOverlapSchedules)) {
				if(!PrefC.GetBool(PrefName.ReplaceExistingBlockout) || !Schedules.IsAppointmentBlocking(sched.BlockoutType)) {
					MsgBox.Show(this,"Blockouts not allowed to overlap.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Creating this blockout will cause blockouts to overlap. Continuing will delete the existing "
						+"blockout(s). Continue?")) {
					return;
				}
				Schedules.DeleteMany(listOverlapSchedules.Select(x => x.ScheduleNum).ToList());
			}
			Schedules.Insert(sched,true);
			Schedules.BlockoutLogHelper(BlockoutAction.Paste,sched);
			RefreshPeriodSchedules();
		}

		private void menuBlockEdit_Click(object sender,EventArgs e) {
			//Pre-calculate the list of blockouts to show.  If the user doesn't have the permission then a modified list is shown.
			List<Def> listUserBlockoutDefs=Defs.GetDefsForCategory(DefCat.BlockoutTypes, true);	
			if(!Security.IsAuthorized(Permissions.Blockouts,true)) {
				//The modified list will only show blockouts marked as "DontCopy" or "NoSchedule"
				listUserBlockoutDefs.RemoveAll(x => !x.ItemValue.Contains(BlockoutType.DontCopy.GetDescription()) 
					&& !x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()));
			}
			//not even enabled if not right click on a blockout
			Schedule scheduleClicked=GetClickedBlockout();
			if(scheduleClicked==null) {
				MessageBox.Show("Blockout not found.");
				return;//should never happen
			}
			FormScheduleBlockEdit FormSB=new FormScheduleBlockEdit(scheduleClicked,Clinics.ClinicNum,listUserBlockoutDefs);
			FormSB.ShowDialog();
			RefreshPeriodSchedules();
		}

		private void menuBlockDelete_Click(object sender,EventArgs e) {
			//If the user doesn't have permission to delete, the menu option will not be enabled.
			Schedule SchedCur=GetClickedBlockout();
			if(SchedCur==null) {
				MessageBox.Show("Blockout not found.");
				return;//should never happen
			}
			Schedules.Delete(SchedCur);
			Schedules.BlockoutLogHelper(BlockoutAction.Delete,SchedCur);
			RefreshPeriodSchedules();
		}

		private void menuBlockAdd_Click(object sender,EventArgs e) {
			//Pre-calculate the list of Blockout Types to show in FormScheduleBlockEdit
			List<Def> listDefsBlockoutTypes=Defs.GetDefsForCategory(DefCat.BlockoutTypes, true);	
			if(!Security.IsAuthorized(Permissions.Blockouts,true)) {
				//This is a special case, so we only keep blockouts that are marked as NoSchedule or DontCopy
				listDefsBlockoutTypes.RemoveAll(x => !x.ItemValue.Contains(BlockoutType.DontCopy.GetDescription()) 
					&& !x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()));
			}
			Schedule schedule=new Schedule();
			schedule.SchedDate=_dateTimeClickedBlockout.Date;
			schedule.StartTime=UserControlApptsPanelJ.RoundTimeDown(_dateTimeClickedBlockout.TimeOfDay,contrApptPanel.MinPerIncr);
			schedule.StopTime=schedule.StartTime+TimeSpan.FromHours(1);
			if(schedule.StartTime>TimeSpan.FromHours(23)) {//if user clicked anywhere during the last hour of the day, set blockout to the last hour of the day.
				schedule.StartTime=new TimeSpan(23,00,00);
				schedule.StopTime=new TimeSpan(23,59,00);
			}
			schedule.Ops.Add(_blockoutClickedOnOp);//jordan 2019-05-20-This is new behavior to prefill op
			schedule.SchedType=ScheduleType.Blockout;
			FormScheduleBlockEdit formScheduleBlockEdit=new FormScheduleBlockEdit(schedule,Clinics.ClinicNum, listDefsBlockoutTypes);
			formScheduleBlockEdit.IsNew=true;
			formScheduleBlockEdit.ShowDialog();
			RefreshPeriodSchedules();
		}

		private void menuBlockCutCopyPaste_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			FormBlockoutCutCopyPaste FormB=new FormBlockoutCutCopyPaste();
			FormB.DateSelected=_dateTimeClickedBlockout.Date;
			if(contrApptPanel.ApptViewCur==null) {
				FormB.ApptViewNumCur=0;
			}
			else {
				FormB.ApptViewNumCur=contrApptPanel.ApptViewCur.ApptViewNum;
			}
			FormB.ShowDialog();
			RefreshPeriodSchedules();
		}

		private void menuBlockClearDay_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Clear all blockouts for day? (This may include blockouts not shown in the current appointment view)")) {
				return;
			}
			Schedules.ClearBlockoutsForDay(_dateTimeClickedBlockout.Date);
			Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:_dateTimeClickedBlockout.Date);
			RefreshPeriodSchedules();
		}

		private void menuBlockClearOp_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Clear all blockouts for day in this operatory?")) {
				return;
			}
			Schedules.ClearBlockoutsForOp(_blockoutClickedOnOp,_dateTimeClickedBlockout.Date);
			Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:_dateTimeClickedBlockout.Date,opNum:_blockoutClickedOnOp);
			RefreshPeriodSchedules();
		}

		private void menuBlockClearClinic_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Blockouts)) {
				return;
			}
			if(!MsgBox.Show(this,true,"Clear all blockouts for day for this clinic?")) {
				return;
			}
			Operatory operatory=Operatories.GetOperatory(_blockoutClickedOnOp);
			Schedules.ClearBlockoutsForClinic(operatory.ClinicNum,_dateTimeClickedBlockout.Date);
			Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:_dateTimeClickedBlockout.Date,clinicNum:operatory.ClinicNum);
			RefreshPeriodSchedules();
		}

		private void menuBlockTypes_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			FormDefinitions FormD=new FormDefinitions(DefCat.BlockoutTypes);
			FormD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Definitions.");
			RefreshPeriodSchedules();
		}
		
		private Schedule GetClickedBlockout() {
			List<Schedule> listScheduleForType=Schedules.GetListForType(contrApptPanel.ListSchedules,ScheduleType.Blockout,0);
			//now find which blockout
			Schedule schedule=null;
			//TimeSpan timeSpanClicked=contrApptPanel.GetTimeClicked();
			for(int i=0;i<listScheduleForType.Count;i++) {
				//skip if op doesn't match
				if(!listScheduleForType[i].Ops.Contains(_blockoutClickedOnOp)) {
					continue;
				}
				if(listScheduleForType[i].SchedDate.Date!=_dateTimeClickedBlockout.Date) {
					continue;
				}
				if(listScheduleForType[i].StartTime <= _dateTimeClickedBlockout.TimeOfDay
					&& _dateTimeClickedBlockout.TimeOfDay < listScheduleForType[i].StopTime) 
				{
					schedule=listScheduleForType[i];
					break;
				}
			}
			return schedule;//might be null;
		}
		#endregion Events - Menu Blockout Click

		#region Events - Menu TextAppts Click
		private void menuTextASAPList_Click(object sender,EventArgs e) {
			TimeSpan timeClicked=UserControlApptsPanelJ.RoundTimeDown(contrApptPanel.GetTimeClicked(),contrApptPanel.MinPerIncr);
			DateTime dateTimeClicked=contrApptPanel.DateSelected+timeClicked;
			if(PrefC.GetBool(PrefName.WebSchedAsapEnabled)) {
				DisplayFormAsapForWebSched(contrApptPanel.GetOpNumClicked(),dateTimeClicked);
			}
			else {//Texting the ASAP list manually
				if(_formASAP!=null && !_formASAP.IsDisposed) {
					_formASAP.Close();
				}
				_formASAP=new FormASAP(dateTimeClicked);
				_formASAP.Show();
				if(_formASAP.WindowState==FormWindowState.Minimized) {
					_formASAP.WindowState=FormWindowState.Normal;
				}
				_formASAP.BringToFront();
			}
		}

		private void menuTextApptsForDayOp_Click(object sender,EventArgs e) {
			DateTime dateClicked=contrApptPanel.DateSelected;
			long opNumClicked=contrApptPanel.GetOpNumClicked();
			List<long> listPatNums=new List<long>();
			for(int i=0;i<contrApptPanel.TableAppointments.Rows.Count;i++){
				if(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["Op"].ToString())!=opNumClicked){
					continue;
				}
				if(PIn.DateT(contrApptPanel.TableAppointments.Rows[i]["AptDateTime"].ToString()).Date!=dateClicked){
					continue;
				}
				listPatNums.Add(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["PatNum"].ToString()));
			}
			SendTextMessages(listPatNums);
		}

		private void menuTextApptsForDayView_Click(object sender,EventArgs e) {
			DateTime dateClicked=contrApptPanel.DateSelected;
			List<long> listPatNums=new List<long>();
			for(int i=0;i<contrApptPanel.TableAppointments.Rows.Count;i++){
				long opNum=PIn.Long(contrApptPanel.TableAppointments.Rows[i]["Op"].ToString());
				if(!contrApptPanel.ListOpsVisible.Any(y => y.OperatoryNum==opNum)){//Make sure the appointments are visible in the current view.
					continue;
				}
				if(PIn.DateT(contrApptPanel.TableAppointments.Rows[i]["AptDateTime"].ToString()).Date!=dateClicked){
					continue;
				}
				listPatNums.Add(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["PatNum"].ToString()));
			}
			SendTextMessages(listPatNums);
		}

		private void menuTextApptsForDay_Click(object sender,EventArgs e) {
			List<Appointment> listAppts=Appointments.GetForPeriodList(contrApptPanel.DateSelected,contrApptPanel.DateSelected,Clinics.ClinicNum)
				.Where(x => !x.AptStatus.In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted)).ToList();
			if(PrefC.HasClinicsEnabled && Clinics.ClinicNum==0) {
				//The above query would have gotten appointments for all clinics. We need to filter to just ClinicNums of 0.
				listAppts=listAppts.Where(x => x.ClinicNum==0).ToList();
			}
			SendTextMessages(listAppts.Select(x => x.PatNum).ToList());
		}

		private void menuUpdateProvs_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup) || !Security.IsAuthorized(Permissions.AppointmentEdit)) {
				return;
			}
			Operatory operatory=Operatories.GetOperatory(contrApptPanel.GetOpNumClicked());
			if(Security.CurUser.ClinicIsRestricted && !Clinics.GetForUserod(Security.CurUser).Exists(x => x.ClinicNum==operatory.ClinicNum)) {
				MsgBox.Show("You are restricted from accessing the clinic belonging to the selected operatory.  No changes will be made.");
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,
				"WARNING: We recommend backing up your database before running this tool.  "
				+"This tool may take a very long time to run and should be run after hours.  "
				+"In addition, this tool could potentially change hundreds of appointments.  "
				+"The changes made by this tool can only be manually reversed.  "
				+"Are you sure you want to continue?"))
			{
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Update Provs on Future Appts tool run on operatory "+operatory.Abbrev+".");
			List<Appointment> listAppts=Appointments.GetAppointmentsForOpsByPeriod(new List<long>() {operatory.OperatoryNum},DateTime.Now);//no end date, so all future
			List<Appointment> listApptsOld=new List<Appointment>();
			foreach(Appointment appt in listAppts) {
				listApptsOld.Add(appt.Copy());
			}
			MoveAppointments(listAppts,listApptsOld,operatory);
			MsgBox.Show("Done");
		}

		private void FormASAP_FormClosed(object sender,FormClosedEventArgs e) {
			RefreshModuleDataPeriod();
			RefreshModuleScreenPeriod();
		}
		#endregion Events - Menu TextAppts Click

		#region Events - Menu Jobs
		private void menuJobs_Attach(object sender,System.EventArgs e) {
			if(contrApptPanel.SelectedAptNum<=0) {
				return;
			}
			//Atach new job
			FormJobSearch FormJS = new FormJobSearch();
			FormJS.ShowDialog();
			if(FormJS.DialogResult!=DialogResult.OK || FormJS.SelectedJobNum==0) {
				return;
			}
			JobLink jobLink = new JobLink();
			jobLink.JobNum=FormJS.SelectedJobNum;
			jobLink.FKey=contrApptPanel.SelectedAptNum;
			jobLink.LinkType=JobLinkType.Appointment;
			JobLinks.Insert(jobLink);
			Signalods.SetInvalid(InvalidType.Jobs,KeyType.Job,jobLink.JobNum);
			return;
		}

		private void menuJobs_GoToJob(object sender,System.EventArgs e) {
				FormOpenDental.S_GoToJob(((long)((MenuItem)sender).Tag));
		}
		#endregion Events - Menu Jobs

		#region Events - Search
		private void butProvDentist_Click(object sender,EventArgs e) {
			_listProvidersSearch=new List<Provider>();
			_listBoxProviders.Items.Clear();
			List<Provider> listProvidersShort=Providers.GetDeepCopy(true);
			for(int i=0;i<listProvidersShort.Count;i++){
				if(ApptViewItemL.ProvIsInView(listProvidersShort[i].ProvNum)) {
					if(!listProvidersShort[i].IsSecondary) {
						_listProvidersSearch.Add(listProvidersShort[i]);
						_listBoxProviders.Items.Add(new ODBoxItem<Provider>(listProvidersShort[i].Abbr,listProvidersShort[i]));
					}
				}
			}
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show("There is no appointment on the pinboard.");
				return;
			}
			DoSearch();
		}

		private void butProvHygenist_Click(object sender,EventArgs e) {
			_listProvidersSearch=new List<Provider>();
			_listBoxProviders.Items.Clear();
			List<Provider> listProvidersShort=Providers.GetDeepCopy(true);
			for(int i=0;i<listProvidersShort.Count;i++) {
				if(ApptViewItemL.ProvIsInView(listProvidersShort[i].ProvNum)) {
					if(listProvidersShort[i].IsSecondary) {
						_listProvidersSearch.Add(listProvidersShort[i]);
						_listBoxProviders.Items.Add(new ODBoxItem<Provider>(listProvidersShort[i].Abbr,listProvidersShort[i]));
					}
				}
			}
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show("There is no appointment on the pinboard.");
				return;
			}
			DoSearch();
		}

		private void butProvPick_Click(object sender,EventArgs e) {
			FormProvidersMultiPick formProvidersMultiPick=new FormProvidersMultiPick();
			formProvidersMultiPick.SelectedProviders=_listProvidersSearch;
			formProvidersMultiPick.ShowDialog();
			if(formProvidersMultiPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_listBoxProviders.Items.Clear();
			for(int i=0;i<formProvidersMultiPick.SelectedProviders.Count;i++) {
				_listBoxProviders.Items.Add(new ODBoxItem<Provider>(formProvidersMultiPick.SelectedProviders[i].Abbr,formProvidersMultiPick.SelectedProviders[i]));
			}
			_listProvidersSearch=formProvidersMultiPick.SelectedProviders;
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show("There is no appointment on the pinboard.");
				return;
			}
			DoSearch();
		}

		private void butSearch_Click(object sender,System.EventArgs e) {
			if(pinBoard.ListPinBoardItems.Count==0) {
				MsgBox.Show("An appointment must be placed on the pinboard before a search can be done.");
				return;
			}
			if(pinBoard.SelectedIndex==-1) {
				if(pinBoard.ListPinBoardItems.Count==1) {
					pinBoard.SelectedIndex=0;
				}
				else {
					MsgBox.Show("An appointment on the pinboard must be selected before a search can be done.");
					return;
				}
			}
			if(!groupSearch.Visible) {//if search not already visible
				dateSearch.Text=DateTime.Today.ToShortDateString();
				ShowSearch();
			}
			DoSearch();
		}

		private void butSearchMore_Click(object sender,System.EventArgs e) {
			if(pinBoard.SelectedIndex==-1) {
				MsgBox.Show("There is no appointment on the pinboard.");
				return;
			}
			if(_listScheduleOpenings==null || _listScheduleOpenings.Count<1) {
				return;
			}
			dateSearch.Text=_listScheduleOpenings[_listScheduleOpenings.Count-1].DateTimeAvail.ToShortDateString();
			DoSearch();
		}

		private void listSearchResults_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			int clickedI=listSearchResults.IndexFromPoint(e.X,e.Y);
			if(clickedI==-1) {
				return;
			}
			SetDateSelected(_listScheduleOpenings[clickedI].DateTimeAvail);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(pinBoard.SelectedIndex==-1) {
				if(pinBoard.ListPinBoardItems.Count>0) {//if there are any appointments on the pinboard.
					pinBoard.SelectedIndex=pinBoard.ListPinBoardItems.Count-1;//select last appt
				}
				else {
					MsgBox.Show("There are no appointments on the pinboard.");
					return;
				}
			}
			DoSearch();
		}

		private void butSearchCloseX_Click(object sender,System.EventArgs e) {
			groupSearch.Visible=false;
		}

		private void butSearchClose_Click(object sender,System.EventArgs e) {
			groupSearch.Visible=false;
		}
		#endregion Events - Search

		#region Methods - Public Initialize
		///<summary>Called from FormOpenDental upon startup.</summary>
		public void InitializeOnStartup(){
			//I hate this.  It's firing way too frequently
			if(_hasInitializedOnStartup) {
				return;
			}
			LayoutPanels();
			contrApptPanel.DateSelected=DateTime.Today;
			contrApptPanel.MinPerIncr=PrefC.GetInt(PrefName.AppointmentTimeIncrement);
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AppointmentColors,true);
			Color colorOpen=listDefs[0].ItemColor;
			Color colorClosed=listDefs[1].ItemColor;
			Color colorHoliday=listDefs[3].ItemColor;
			Color colorBlockText=listDefs[4].ItemColor;
			Color colorTimeLine=PrefC.GetColor(PrefName.AppointmentTimeLineColor);
			contrApptPanel.SetColors(colorOpen,colorClosed,colorHoliday,colorBlockText,colorTimeLine);
			menuWeeklyApt.MenuItems.Clear();
			MenuItem menuItem=menuWeeklyApt.MenuItems.Add("Copy to Pinboard",new EventHandler(menuWeekly_Click));
			menuItem.Name=MenuItemNames.CopyToPinboard;
			menuApt.MenuItems.Clear();
			menuItem=menuApt.MenuItems.Add("Copy to Pinboard",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.CopyToPinboard;
			if(PrefC.IsODHQ) {
				menuItem=menuApt.MenuItems.Add("Copy Appointment Structure",new EventHandler(menuApt_Click));
				menuItem.Name=MenuItemNames.CopyAppointmentStructure;
			}
			menuApt.MenuItems.Add("-");
			menuItem=menuApt.MenuItems.Add("Send to Unscheduled List",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.SendToUnscheduledList;
			menuItemBreakAppt=menuApt.MenuItems.Add("Break Appointment",new EventHandler(menuApt_Click));
			menuItemBreakAppt.Name=MenuItemNames.BreakAppointment;
			//menuItem=menuApt.MenuItems.Add("Mark as ASAP",new EventHandler(OnASAP_Click));
			//menuItem.Name=MenuItemNames.MarkAsAsap;
			menuItem=menuApt.MenuItems.Add("Set Complete",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.SetComplete;
			menuItem=menuApt.MenuItems.Add("Delete",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.Delete;
			menuItem=menuApt.MenuItems.Add("Other Appointments",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.OtherAppointments;
			menuApt.MenuItems.Add("-");
			menuItem=menuApt.MenuItems.Add("Print Label",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.PrintLabel;
			menuItem=menuApt.MenuItems.Add("Print Card",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.PrintCard;
			menuItem=menuApt.MenuItems.Add("Print Card for Entire Family",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.PrintCardEntireFamily;
			menuItem=menuApt.MenuItems.Add("Routing Slip",new EventHandler(menuApt_Click));
			menuItem.Name=MenuItemNames.RoutingSlip;
			menuBlockout.MenuItems.Clear();
			menuItem=menuBlockout.MenuItems.Add("Edit Blockout",menuBlockEdit_Click);
			menuItem.Name=MenuItemNames.RoutingSlip;//js RoutingSlip seems wrong for this and the items below.
			menuItem=menuBlockout.MenuItems.Add("Cut Blockout",menuBlockCut_Click);
			menuItem.Name=MenuItemNames.RoutingSlip;
			menuItem=menuBlockout.MenuItems.Add("Copy Blockout",menuBlockCopy_Click);
			menuItem.Name=MenuItemNames.RoutingSlip;
			menuItem=menuBlockout.MenuItems.Add("Paste Blockout",menuBlockPaste_Click);
			menuItem.Name=MenuItemNames.RoutingSlip;
			menuItem=menuBlockout.MenuItems.Add("Delete Blockout",menuBlockDelete_Click);
			menuItem.Name=MenuItemNames.RoutingSlip;
			menuItem=menuBlockout.MenuItems.Add("Add Blockout",menuBlockAdd_Click);
			menuItem.Name=MenuItemNames.RoutingSlip;
			menuItem=menuBlockout.MenuItems.Add("Blockout Cut-Copy-Paste",menuBlockCutCopyPaste_Click);
			menuItem.Name=MenuItemNames.RoutingSlip;
			if(!PrefC.HasClinicsEnabled) {//Clear All Blockouts for Day is too aggressive when Clinics are enabled.
				menuItem=menuBlockout.MenuItems.Add("Clear All Blockouts for Day",menuBlockClearDay_Click);
				menuItem.Name=MenuItemNames.ClearAllBlockoutsForDay;
			}
			menuItem=menuBlockout.MenuItems.Add("Clear All Blockouts for Day, Op only",menuBlockClearOp_Click);
			menuItem.Name=MenuItemNames.ClearAllBlockoutsForDayOpOnly;
			if(PrefC.HasClinicsEnabled) {
				menuItem=menuBlockout.MenuItems.Add("Clear All Blockouts for Day, Clinic only",menuBlockClearClinic_Click);
				menuItem.Name=MenuItemNames.ClearAllBlockoutsForDayClinicOnly;
			}
			menuItem=menuBlockout.MenuItems.Add("Edit Blockout Types",menuBlockTypes_Click);
			menuItem.Name=MenuItemNames.EditBlockoutTypes;
			menuItem=menuBlockout.MenuItems.Add("-");//Designer code to insert a horizontal separator
			menuItem.Name=MenuItemNames.BlockoutSpacer;
			menuItem=menuBlockout.MenuItems.Add("Text ASAP List",menuTextASAPList_Click);
			menuItem.Name=MenuItemNames.TextAsapList;
			menuItem=menuBlockout.MenuItems.Add(MenuItemNames.TextApptsForDayOp,menuTextApptsForDayOp_Click);
			menuItem.Name=MenuItemNames.TextApptsForDayOp;
			menuItem=menuBlockout.MenuItems.Add(MenuItemNames.TextApptsForDayView,menuTextApptsForDayView_Click);
			menuItem.Name=MenuItemNames.TextApptsForDayView;
			menuItem=menuBlockout.MenuItems.Add(MenuItemNames.TextApptsForDay,menuTextApptsForDay_Click);
			menuItem.Name=MenuItemNames.TextApptsForDay;
			menuItem=menuBlockout.MenuItems.Add("Update Provs on Future Appts",menuUpdateProvs_Click);
			menuItem.Name=MenuItemNames.UpdateProvsOnFutureAppts;
			//skippped translation
			LayoutToolBar();
			//Appointment action buttons
			toolTip1.SetToolTip(butUnsched,"Send to Unscheduled List");
			toolTip1.SetToolTip(butBreak,"Break");
			toolTip1.SetToolTip(butComplete,"Set Complete");
			toolTip1.SetToolTip(butDelete,"Delete");
			//toolTip1.SetToolTip(butOther,,"Other Appointments");
			if(PrefC.IsODHQ) {
				butGraph.Visible=true;
			}
			SetWeeklyView(PrefC.GetBool(PrefName.ApptModuleDefaultToWeek));
			//later: SendToPinboardEvent.Fired+=HandlePinClicked;
			_hasInitializedOnStartup=true;
		}

		///<summary>stub</summary>
		public void LayoutToolBar(){
			toolBarMain.Buttons.Clear();
			toolBarMain.Buttons.Add(new ODToolBarButton("",2,"Appointment Lists","Lists"));
			toolBarMain.Buttons.Add(new ODToolBarButton("",1,"Print Schedule","Print"));
			if(!ProgramProperties.IsAdvertisingDisabled(ProgramName.RapidCall)) {
				toolBarMain.Buttons.Add(new ODToolBarButton("",3,"Rapid Call","RapidCall"));
			}
			ProgramL.LoadToolbar(toolBarMain,ToolBarsAvail.ApptModule);
			toolBarMain.Invalidate();
			Plugins.HookAddCode(this,"ContrAppt.LayoutToolBar_end",_patCur);
		}
		#endregion Methods - Public Initialize

		#region Methods - Public Module Select
		///<summary>Refreshes the module for the passed in patient.  A patNum of 0 is acceptable.  Any ApptNums within listPinApptNums will get forcefully added to the main DataSet for the appointment module.</summary>
		public void ModuleSelected(long patNum,List<long> listPinApptNums=null,List<long> listOpNums=null,List<long> listProvNums=null){
			if(IsHqNoneView()) {
				return;
			}
			contrApptPanel.BeginUpdate();
			RefreshModuleDataPatient(patNum);
			RefreshModuleDataPeriod(listPinApptNums,listOpNums,listProvNums,isRefreshSchedules:true);
			RefreshModuleScreenButtonsRight();
			RefreshModuleScreenPeriod();
			LayoutScrollOpProv();//only runs once
			contrApptPanel.EndUpdate();
		}

		///<summary>stub</summary>
		public void ModuleUnselected(){
			//todo:
		}

		///<summary></summary>
		public void ModuleSelectedWithPinboard(long patNum,List<long> listPinApptNums){
			ModuleSelected(patNum,listPinApptNums);
			SendToPinBoardAptNums(listPinApptNums);
		}

		///<summary>>Refreshes everything except the patient info. isRefreshBubble will refresh the appointment bubble.  If another workstation made a change, then refreshes datatables.</summary>
		public void RefreshPeriod(bool isRefreshBubble=true,List<long> listOpNums=null,List<long> listProvNums=null,bool isRefreshAppointments=true,
			bool isRefreshSchedules=false)
			{
			if(IsHqNoneView()) {
				return;
			}
			contrApptPanel.BeginUpdate();
			//todo:
			//long oldBubbleNum=bubbleAptNum;
			RefreshModuleDataPeriod(listOpNums:listOpNums,listProvNums:listProvNums,isRefreshAppointments:isRefreshAppointments,isRefreshSchedules:isRefreshSchedules);
			RefreshModuleScreenPeriod();
			//if(!isRefreshBubble) {
			//	bubbleAptNum=oldBubbleNum;
			//}
			contrApptPanel.EndUpdate();
		}

		/// <summary>Wrapper for RefreshPeriod, refreshes the schedules, but not the appointments</summary>
		public void RefreshPeriodSchedules() {
			RefreshPeriod(isRefreshAppointments:false,isRefreshSchedules:true);
		}
		#endregion Methods - Public Module Select

		#region Methods - Public Other
		///<summary>Displays the Other Appointments for the current patient, then refreshes screen as needed.  initialClick specifies whether the user 
		///doubleclicked on a blank time to get to this dialog.</summary>
		public void DisplayOtherDlg(bool initialClick,DateTime dateTime,long opNum) {
			if(_patCur==null) {
				return;
			}
			FormApptsOther formApptsOther=new FormApptsOther(_patCur.PatNum);
			formApptsOther.IsInitialClick=initialClick;
			formApptsOther.DateTNew=dateTime;
			formApptsOther.OpNumNew=opNum;
			formApptsOther.ShowDialog();
			ProcessOtherDlg(formApptsOther.OResult,formApptsOther.SelectedPatNum,formApptsOther.DateJumpToString,formApptsOther.AptNumsSelected.ToArray());
		}

		///<summary>Displays the Other Appointments for the current patient, then refreshes screen as needed.  initialClick specifies whether the user 
		///doubleclicked on a blank time to get to this dialog.</summary>
		public void DisplayOtherDlg(bool initialClick) {
			if(_patCur==null) {
				return;
			}
			FormApptsOther FormAO=new FormApptsOther(_patCur.PatNum);
			FormAO.IsInitialClick=initialClick;
			FormAO.ShowDialog();
			ProcessOtherDlg(FormAO.OResult,FormAO.SelectedPatNum,FormAO.DateJumpToString,FormAO.AptNumsSelected.ToArray());
		}

		///<summary>The key press from the main form is passed down to this module.  This is guaranteed to be between the keys of F1 and F12.</summary>
		public void FunctionKeyPress(Keys keys) {
			string keyName=Enum.GetName(typeof(Keys),keys);//keyName will be F1, F2, ... F12
			int fKeyVal=int.Parse(keyName.TrimStart('F'));//strip off the F and convert to an int
			if(_listApptViews.Count<fKeyVal) {
				return;
			}
			SetView(_listApptViews[fKeyVal-1].ApptViewNum,true);
		}

		///<summary>Used by parent form when a dialog needs to be displayed, but mouse might be down.  This forces a mouse up, and cleans up any mess so that dlg can show.</summary>
		public void MouseUpForced() {
			pinBoard.MouseUpForced();
			contrApptPanel.MouseUpForced();
		}

		///<summary>This is public so that FormOpenDental can pass refreshed tasks here in order to avoid an extra query.</summary>
		public void RefreshReminders(List <Task> listReminderTasks) {
			/*
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
			List<Task> listSortedReminderTasks=listReminderTasks
				.Where(x => x.DateTimeEntry.Date <= DateTimeOD.Today)
				.OrderBy(x => x.DateTimeEntry)
				.ToList();
			tabReminders.Text=Lan.g(this,"Reminders");
			if(listSortedReminderTasks.Count > 0) {
				tabReminders.Text+="*";
			}
			gridReminders.BeginUpdate();
			if(gridReminders.Columns.Count==0) {
				gridReminders.Columns.Clear();
				ODGridColumn col=new ODGridColumn("",17);//The status column showing new/viewed in a checkbox.
				col.ImageList=imageListTasks;
				gridReminders.Columns.Add(col);
				col=new ODGridColumn(Lan.g("TableTasks","Description"),200);//any width
				gridReminders.Columns.Add(col);
			}
			gridReminders.Rows.Clear();
			for(int i=0;i<listSortedReminderTasks.Count;i++) {
				ODGridRow row=new ODGridRow();
				SetReminderGridRow(row,listSortedReminderTasks[i]);
				gridReminders.Rows.Add(row);
			}
			gridReminders.EndUpdate();
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);*/
		}
		#endregion Methods - Public Other

		#region Methods - Private Refresh Data
		///<summary>Gets op nums and prov nums for current view if not passed in.  Will refresh the appointments and schedules if the respective doRefreshes are set.</summary>
		private void RefreshModuleDataPeriod(List<long> listPinApptNums = null,List<long> listOpNums = null,List<long> listProvNums = null
			,bool isRefreshAppointments=true,bool isRefreshSchedules=false)
		{
			long apptViewNum=-1;
			if(listOpNums==null) {
				apptViewNum=GetApptViewNumForUser();
				listOpNums=ApptViewItems.GetOpsForView(apptViewNum);
			}
			if(listProvNums==null) {
				if(apptViewNum<0) {
					apptViewNum=GetApptViewNumForUser();//Only run this query if we have to (haven't run it yet from this method).
				}
				listProvNums=ApptViewItems.GetProvsForView(apptViewNum);
			}
			RefreshAppointmentsIfNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listPinApptNums,listOpNums,listProvNums,isRefreshAppointments);
			RefreshSchedulesIfNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listOpNums,isRefreshSchedules);
			RefreshWaitingRoomTable();
			_dateTimeWaitingRmRefreshed=DateTime.Now;
			//SchedListPeriod=Schedules.ConvertTableToList(_dtSchedule);//happens internally in contrApptPanel when setting TableSchedul
			ApptView viewCur=null;
			if(comboView.SelectedIndex>0) {
				viewCur=_listApptViews[comboView.SelectedIndex-1];
			}
			GetForCurView(viewCur,contrApptPanel.IsWeeklyView,contrApptPanel.ListSchedules);
		}

		///<summary>Fills PatCur from the database.</summary>
		public void RefreshModuleDataPatient(long patNum){
			if(patNum==0) {
				_patCur=null;
				return;
			}
			//We have to go to the db because we need to get the most recent patient info, mainly the AskedToArriveEarly time.
			_patCur=Patients.GetPat(patNum);
			Plugins.HookAddCode(this, "ContrAppt.RefreshModuleDataPatient_end");
		}

		/// <summary>This corresponds to the old ApptViewItemL.GetForCurView.  Its job is to set the ApptViewCur and then send VisOps and VisProvs data to the drawing.</summary>
		private void GetForCurView(ApptView apptViewCur,bool isWeekly,List<Schedule> listSchedulesDaily){
			//contrApptPanel.BeginUpdate();//already handled in RefreshModuleDataPeriod
			contrApptPanel.ApptViewCur=apptViewCur;
			List<Provider> visProvs=null;
			List<Operatory> visOps=null;
			int rowsPerIncr=0;
			List<ApptViewItem> listApptViewItemRowElements=null;
			List<ApptViewItem> listApptViewItems=null;
			ApptViewItemL.FillForApptView(isWeekly,apptViewCur,out visProvs,out visOps,out listApptViewItems,out listApptViewItemRowElements,out rowsPerIncr);
			ApptViewItemL.AddOpsForScheduledProvs(isWeekly,listSchedulesDaily,apptViewCur,ref visOps);
			visOps.Sort(ApptViewItemL.CompareOps);
			visProvs.Sort(ApptViewItemL.CompareProvs);
			contrApptPanel.ListProvsVisible=visProvs;
			contrApptPanel.ListOpsVisible=visOps;
			contrApptPanel.RowsPerIncr=rowsPerIncr;
			contrApptPanel.ListApptViewItemRowElements=listApptViewItemRowElements;
			contrApptPanel.ListApptViewItems=listApptViewItems;
			//contrApptPanel.EndUpdate();
		}

		///<summary>If needed, refreshes the _dtAppointments, _dtApptFields, and _dtPatFields tables.</summary>
		private void RefreshAppointmentsIfNeeded(DateTime dateStart,DateTime dateEnd,List<long> listPinApptNums=null,
			List<long> listOpNums=null,List<long> listProvNums=null,bool isRefreshNeeded=false)
		{
			if(contrApptPanel.TableAppointments!=null && contrApptPanel.TableApptFields!=null && contrApptPanel.TablePatFields!=null && !isRefreshNeeded) {
				return;//If all data is already in memory and we are not forcing the refresh.
			}
			contrApptPanel.TableAppointments=Appointments.GetPeriodApptsTable(dateStart,dateEnd,0,false,listPinApptNums,listOpNums,listProvNums,false);
			contrApptPanel.TableApptFields=Appointments.GetApptFields(contrApptPanel.TableAppointments);
			contrApptPanel.TablePatFields=Appointments.GetPatFields(contrApptPanel.TableAppointments.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList());
			//contrApptPanel.OverlapOrdering.UpdateApptOrder(contrApptPanel.TableAppointments);
		}

		/// <summary>If needed, refreshes the _dtSchedule, _dtEmpSched, and _dtProvSched tables.</summary>
		private void RefreshSchedulesIfNeeded(DateTime dateStart,DateTime dateEnd,List<long> listOpNums,bool isRefreshNeeded=false) {
			if(contrApptPanel.TableSchedule!=null && contrApptPanel.TableEmpSched!=null && contrApptPanel.TableProvSched!=null && !isRefreshNeeded) {
				return;//If all data is already in memory and we are not forcing the refresh.
			}
			contrApptPanel.TableEmpSched=Schedules.GetPeriodEmployeeSchedTable(dateStart,dateEnd,Clinics.ClinicNum);
			contrApptPanel.TableProvSched=Schedules.GetPeriodProviderSchedTable(dateStart,dateEnd,Clinics.ClinicNum);
			contrApptPanel.TableSchedule=Schedules.GetPeriodSchedule(dateStart,dateEnd,listOpNums,false);
		}

		///<summary>Always refreshes the _dtWaitingRoom table.</summary>
		private void RefreshWaitingRoomTable() {
			contrApptPanel.TableWaitingRoom=Appointments.GetPeriodWaitingRoomTable();
		}
		#endregion Methods - Private Refresh Data

		#region Methods - Private Refresh Screen
		///<summary>Happens once per minute.  It used to just move the red timebar down without querying the database.  
		///But now it queries the database so that the waiting room list shows accurately.  Always updates the waiting room.</summary>
		public void TickRefresh(){
			try {
				//dates already set
				if(PrefC.GetBool(PrefName.ApptModuleRefreshesEveryMinute)) {
					if(PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)==0) {//Signal processing is disabled.
						RefreshPeriod(false);
					}
					else {
						//Calling Signalods.RefreshTimed() was causing issues for large customers. This resulted in 100,000+ rows of signalod's returned.
						//Now we only query for the specific signals we care about. Instead of using Signalods.SignalLastRefreshed we now use Signalods.ApptSignalLastRefreshed.
						//Signalods.ApptSignalLastRefreshed mimics the behavior of Signalods.SignalLastRefreshed but is guaranteed to not be stale from inactive sessions.
						List<Signalod> listSignals=Signalods.RefreshTimed(Signalods.ApptSignalLastRefreshed,new List<InvalidType>(){ InvalidType.Appointment,InvalidType.Schedules });
						bool isApptRefresh=Signalods.IsApptRefreshNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listSignals);
						bool isSchedRefresh=Signalods.IsSchedRefreshNeeded(contrApptPanel.DateStart,contrApptPanel.DateEnd,listSignals);
						//either we have signals from other machines telling us to refresh, or we aren't using signals, in which case we still want to refresh
						RefreshPeriod(false,isRefreshAppointments:isApptRefresh,isRefreshSchedules:isSchedRefresh);
					}
				}
				else {
					contrApptPanel.RedrawAsNeeded();//just for the red timebar
				}
				Signalods.ApptSignalLastRefreshed=MiscData.GetNowDateTime();
			}
			catch {
				//prevents rare malfunctions. For instance, during editing of views, if tickrefresh happens.
			}
			//GC.Collect();	
		}

		///<summary>Redraws screen based on data already gathered.  RefreshModuleDataPeriod will have already retrieved the data from the db.</summary>
		public void RefreshModuleScreenPeriod() {
			Calendar2.SetSelectionRange(contrApptPanel.DateStart,contrApptPanel.DateEnd);
			//LayoutPanels();
			labelDate.Text=contrApptPanel.DateStart.ToString("ddd");
			labelDate2.Text=contrApptPanel.DateStart.ToString("-  MMM d");
			List<long> opNums = null;
			if(PrefC.HasClinicsEnabled && Clinics.ClinicNum>0) {
				opNums = Operatories.GetOpsForClinic(Clinics.ClinicNum).Select(x => x.OperatoryNum).ToList();
			}
			List<LabCase> labCaseList=LabCases.GetForPeriod(contrApptPanel.DateStart,contrApptPanel.DateEnd,opNums);
			FillLab(labCaseList);
			FillProduction(contrApptPanel.DateStart,contrApptPanel.DateEnd);
			FillProductionGoal(contrApptPanel.DateStart,contrApptPanel.DateEnd);
			bool hasNotes=true;
			if(PrefC.IsODHQ){
				hasNotes=Security.IsAuthorized(Permissions.Schedules,true);
			}
			FillProvSched(hasNotes);
			FillEmpSched(hasNotes);
			FillWaitingRoom();
			contrApptPanel.RedrawAsNeeded();
		}

		///<summary>Sets buttons at right to enabled/disabled. Sets value of listConfirmed. Was previously called RefreshModuleScreenPatient.</summary>
		public void RefreshModuleScreenButtonsRight(){
			if(_patCur==null) {
				panelMakeButtons.Enabled=false;
			}
			else {
				panelMakeButtons.Enabled=true;
			}
			//considered only doing this once when starting program, but would then need to refresh it if we change definitions.  Might still try that.
			listConfirmed.Items.Clear();
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			for(int i=0;i<listDefs.Count;i++) {
				this.listConfirmed.Items.Add(listDefs[i].ItemValue);
			}
			DataRow dataRow=contrApptPanel.GetDataRowForSelected();
			if(dataRow!=null) {
				butUnsched.Enabled=true;
				butBreak.Enabled=true;
				butComplete.Enabled=true;
				butDelete.Enabled=true;
				string confirmed=dataRow["Confirmed"].ToString();
				listConfirmed.SelectedIndex=Defs.GetOrder(DefCat.ApptConfirmed,PIn.Long(confirmed));//could be -1
				if(!Security.IsAuthorized(Permissions.ApptConfirmStatusEdit,true)) {//Suppress message because it would be very annoying to users.
					listConfirmed.Enabled=false;
				}
				else {
					listConfirmed.Enabled=true;
				}
			}
			else {//even if an appt on the pinboard is selected, these are all grayed out
				butUnsched.Enabled=false;
				butBreak.Enabled=false;
				butComplete.Enabled=false;
				butDelete.Enabled=false;
				listConfirmed.Enabled=false;
				if(pinBoard.SelectedIndex!=-1){
					dataRow=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].DataRowAppt;
					listConfirmed.SelectedIndex=Defs.GetOrder(DefCat.ApptConfirmed,PIn.Long(dataRow["Confirmed"].ToString()));//could be -1
				}
			}
		}

		///<summary>Fills comboView and _listApptViews with the current list of views. Triggers ModuleSelected().  Also called from FormOpenDental.RefreshLocalData().</summary>
		public void FillViews() {
			comboView.Items.Clear();
			_listApptViews.Clear();
			comboView.Items.Add(Lan.g(this,"none"));
			string f="";
			foreach(ApptView apptView in ApptViews.GetDeepCopy()) {
				if(PrefC.HasClinicsEnabled && Clinics.ClinicNum!=apptView.ClinicNum) {
					//This is intentional, we do NOT want 'Headquarters' to have access to clinic specific apptviews.  
					//Likewise, we do not want clinic specific views to be accessible from specific clinic filters.
					continue;
				}
				_listApptViews.Add(apptView.Copy());
				if(_listApptViews.Count<=12)
					f="F"+_listApptViews.Count.ToString()+"-";
				else
					f="";
				comboView.Items.Add(f+apptView.Description);
			}
			ApptView apptViewCur=GetApptViewForUser();
			if(apptViewCur!=null) {
				SetView(apptViewCur.ApptViewNum,false);//this also triggers ModuleSelected()
			}
			else {
				SetView(0,false);//this also triggers ModuleSelected()
			}
		}

		///<summary>Fills the lab summary for the day.</summary>
		private void FillLab(List<LabCase> labCaseList) {
			int notRec=0;
			for(int i=0;i<labCaseList.Count;i++) {
				if(labCaseList[i].DateTimeChecked.Year>1880) {
					continue;
				}
				if(labCaseList[i].DateTimeRecd.Year>1880) {
					continue;
				}
				notRec++;
			}
			if(notRec==0) {
				textLab.Font=new Font(FontFamily.GenericSansSerif,8,FontStyle.Regular);
				textLab.ForeColor=Color.Black;
				textLab.Text=Lan.g(this,"All Received");
			}
			else {
				textLab.Font=new Font(FontFamily.GenericSansSerif,8,FontStyle.Bold);
				textLab.ForeColor=Color.DarkRed;
				textLab.Text=notRec.ToString()+Lan.g(this," NOT RECEIVED");
			}
		}

		///<summary>Fills the production summary for the day. ContrApptSheet2.Controls should be current with ContrApptSingle(s) for the select Op and date.</summary>
		private void FillProduction(DateTime start,DateTime end) {
			if(!contrApptPanel.ListApptViewItemRowElements.Exists(x => x.ElementDesc=="Production") 
				&& !contrApptPanel.ListApptViewItemRowElements.Exists(x => x.ElementDesc=="NetProduction"))
			{
				textProduction.Text="";
				return;
			}
			decimal grossproduction=0;
			decimal netproduction=0;
			int indexProv;
			foreach(DataRow dataRow in contrApptPanel.TableAppointments.Rows){
				indexProv=-1;
				bool isHygiene=PIn.Bool(dataRow["IsHygiene"].ToString());
				long provNum=PIn.Long(dataRow["ProvNum"].ToString());
				long provHyg=PIn.Long(dataRow["ProvHyg"].ToString());
				long opNum=PIn.Long(dataRow["Op"].ToString());
				if(PrefC.GetBool(PrefName.ApptModuleProductionUsesOps)) {
					if(isHygiene){
						if(provHyg==0) {//if no hyg prov set.
							indexProv=contrApptPanel.GetIndexOp(opNum);
						}
						else {
							indexProv=contrApptPanel.GetIndexOp(opNum);
						}
					}
					else {//not hyg
						indexProv=contrApptPanel.GetIndexOp(opNum);
					}
				}
				else {//use provider bars in appointment view
					if(isHygiene) {
						if(provHyg==0) {//if no hyg prov set.
							indexProv=contrApptPanel.GetIndexProv(provNum);
						}
						else {
							indexProv=contrApptPanel.GetIndexProv(provHyg);
						}
					}
					else {//not hyg
						indexProv=contrApptPanel.GetIndexProv(provNum);
					}
				}
				if(indexProv==-1) {
					continue;
				}
				ApptStatus aptStatus=(ApptStatus)(PIn.Int(dataRow["AptStatus"].ToString()));
				long clinicNum=PIn.Long(dataRow["ClinicNum"].ToString());
				decimal productionVal=PIn.Decimal(dataRow["productionVal"].ToString());
				decimal writeoffPPO=PIn.Decimal(dataRow["writeoffPPO"].ToString());
				if(aptStatus!=ApptStatus.Broken
					&& aptStatus!=ApptStatus.UnschedList
					&& aptStatus!=ApptStatus.PtNote
					&& aptStatus!=ApptStatus.PtNoteCompleted) 
				{
					//When the program is restricted to a specific clinic, only count up production for the corresponding clinic.
					if(PrefC.HasClinicsEnabled 
						&& Clinics.ClinicNum!=0
						&& Clinics.ClinicNum!=clinicNum) {
						continue;//This appointment is for a different clinic.  Do not include this production in the daily prod.
					}
					//In order to get production numbers split by provider, it would require generating total production numbers
					//in another table from the business layer.  But that will only work if hyg procedures are appropriately assigned
					//when setting appointments.
					grossproduction+=productionVal;
					netproduction+=productionVal-writeoffPPO;
				}
			}
			if(PrefC.GetBool(PrefName.ApptModuleAdjustmentsInProd) && contrApptPanel.TableAppointments.Rows.Count>0) {
				List<long> listProvNumsForApptView=new List<long>();
				//If the PrefName.ApptModuleProductionUsesOps is true, the list will be filled with OpNums for the appointment view. Otherwise, the list will be empty.
				List<long> listOpsForApptView=new List<long>();
				long apptViewNum=GetApptViewNumForUser();
				if(PrefC.GetBool(PrefName.ApptModuleProductionUsesOps)) {
					listOpsForApptView=ApptViewItems.GetOpsForView(apptViewNum);
				}
				else {
					listProvNumsForApptView=ApptViewItems.GetProvsForView(apptViewNum);
				}
				netproduction+=Adjustments.GetAdjustAmtForAptView(start,end,Clinics.ClinicNum,listOpsForApptView,listProvNumsForApptView);
			}
			textProduction.Text=grossproduction.ToString("c0");
			if(grossproduction!=netproduction) {
				textProduction.Text+=", net:"+netproduction.ToString("c0");
			}
		}

		private void FillProductionGoal(DateTime start,DateTime end) {
			if(!contrApptPanel.ListApptViewItemRowElements.Exists(x => x.ElementDesc.In("Production","NetProduction"))) {
				textProdGoal.Text="";
				return;
			}
			decimal prodGoalAmt=0;
			long apptViewNum=GetApptViewNumForUser();
			//If the PrefName.ApptModuleProductionUsesOps is false, it will be filled with ProvNums for the appointment view. Otherwise, the list will be empty.
			List<long> listProvNumsForApptView=new List<long>();
			//If the PrefName.ApptModuleProductionUsesOps is true, the list will be filled with OpNums for the appointment view. Otherwise, the list will be empty.
			List<long> listOpsForApptView=new List<long>();
			if(PrefC.GetBool(PrefName.ApptModuleProductionUsesOps)) {
				listOpsForApptView=ApptViewItems.GetOpsForView(apptViewNum);
			}
			else {
				listProvNumsForApptView=ApptViewItems.GetProvsForView(apptViewNum);
			}
			//This will return a dict of production goals for either the providers for the provider bars for the appointment view or the providers 
			//scheduled for the appointment view ops. 
			Dictionary<long,decimal> dictProvProdGoal=Providers.GetProductionGoalForProviders(listProvNumsForApptView,listOpsForApptView,start,end);
			foreach(KeyValuePair<long,decimal> prov in dictProvProdGoal) {
				prodGoalAmt+=prov.Value;
			}
			textProdGoal.Text=prodGoalAmt.ToString("c0");
		}

		///<summary></summary>
		private void FillProvSched(bool hasNotes) {
			DataTable table=contrApptPanel.TableProvSched;
			gridProv.BeginUpdate();
			gridProv.Columns.Clear();
			ODGridColumn col=new ODGridColumn("Provider",80);
			gridProv.Columns.Add(col);
			col=new ODGridColumn("Schedule",70);
			gridProv.Columns.Add(col);
			if(hasNotes) {
				col=new ODGridColumn("Notes",100);
				gridProv.Columns.Add(col);
			}
			gridProv.Rows.Clear();
			ODGridRow row;
			foreach(DataRow dRow in table.Rows) { 
				row=new ODGridRow();
				row.Cells.Add(dRow["ProvAbbr"].ToString());
				row.Cells.Add(dRow["schedule"].ToString());
				if(hasNotes) {
					row.Cells.Add(dRow["Note"].ToString());
				}
				gridProv.Rows.Add(row);
			}
			gridProv.EndUpdate();
		}

		///<summary></summary>
		private void FillEmpSched(bool hasNotes) {
			DataTable table=contrApptPanel.TableEmpSched;
			gridEmpSched.BeginUpdate();
			gridEmpSched.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g("TableApptEmpSched","Employee"),80);
			gridEmpSched.Columns.Add(col);
			col=new ODGridColumn(Lan.g("TableApptEmpSched","Schedule"),70);
			gridEmpSched.Columns.Add(col);
			if(hasNotes) {
				col=new ODGridColumn(Lan.g("TableApptEmpSched","Notes"),100);
				gridEmpSched.Columns.Add(col);
			}
			gridEmpSched.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(table.Rows[i]["empName"].ToString());
				row.Cells.Add(table.Rows[i]["schedule"].ToString());
				if(hasNotes) {
					row.Cells.Add(table.Rows[i]["Note"].ToString());
				}
				gridEmpSched.Rows.Add(row);
			}
			gridEmpSched.EndUpdate();
		}

		///<summary>Once per second, this grid refills itself in order to show the time ticking by.  This does not require a trip to the database.  Set refreshOpsForDay true in order to forcefully go to the database to get the current ops for the day for waiting room filtering.</summary>
		private void FillWaitingRoom() {
			if(contrApptPanel.TableWaitingRoom==null || contrApptPanel.TableWaitingRoom.Rows.Count==0){
				return;
			}
			TimeSpan timeSpanDeltaSinceRefresh=DateTime.Now-_dateTimeWaitingRmRefreshed;
			DataTable table=contrApptPanel.TableWaitingRoom;
			List<Operatory> listOpsForClinic=new List<Operatory>();
			List<Operatory> listOpsForApptView=new List<Operatory>();
			if(PrefC.GetBool(PrefName.WaitingRoomFilterByView)) {
				//In order to filter the waiting room by appointment view, we need to always grab the operatories visible for TODAY.
				//This way, regardless of what day the customer is looking at, the waiting room will only change when they change appointment views.
				//Always use the schedules from SchedListPeriod which is refreshed any time RefreshModuleDataPeriod() is invoked.
				ApptView viewCur=null;
				if(comboView.SelectedIndex>0) {
					viewCur=_listApptViews[comboView.SelectedIndex-1];
				}
				List<Schedule> listSchedulesForToday=contrApptPanel.ListSchedules.FindAll(x => x.SchedDate==DateTime.Today);
				listOpsForApptView=ApptViewItemL.GetOpsForApptView(viewCur,contrApptPanel.IsWeeklyView,listSchedulesForToday);
			}
			if(PrefC.HasClinicsEnabled) {//Using clinics
				listOpsForClinic=Operatories.GetOpsForClinic(Clinics.ClinicNum);
			}
			gridWaiting.BeginUpdate();
			gridWaiting.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g("TableApptWaiting","Patient"),130);
			gridWaiting.Columns.Add(col);
			col=new ODGridColumn(Lan.g("TableApptWaiting","Waited"),100,HorizontalAlignment.Center);
			gridWaiting.Columns.Add(col);
			gridWaiting.Rows.Clear();
			DateTime waitTime;
			ODGridRow row;
			int waitingRoomAlertTime=PrefC.GetInt(PrefName.WaitingRoomAlertTime);
			Color waitingRoomAlertColor=PrefC.GetColor(PrefName.WaitingRoomAlertColor);
			for(int i=0;i<table.Rows.Count;i++) {
				//Always filter the waiting room by appointment view first, regardless of using clinics or not.
				if(PrefC.GetBool(PrefName.WaitingRoomFilterByView)) {
					bool isInView=false;
					for(int j=0;j<listOpsForApptView.Count;j++) {
						if(listOpsForApptView[j].OperatoryNum==PIn.Long(table.Rows[i]["OpNum"].ToString())) {
							isInView=true;
							break;
						}
					}
					if(!isInView) {
						continue;
					}
				}
				//We only want to filter the waiting room by the clinic's operatories when clinics are enabled and they are not using 'Headquarters' mode.
				if(PrefC.HasClinicsEnabled && Clinics.ClinicNum!=0) {
					bool isInView=false;
					for(int j=0;j<listOpsForClinic.Count;j++) {
						if(listOpsForClinic[j].OperatoryNum==PIn.Long(table.Rows[i]["OpNum"].ToString())) {
							isInView=true;
							break;
						}
					}
					if(!isInView) {
						continue;
					}
				}
				row=new ODGridRow();
				row.Cells.Add(table.Rows[i]["patName"].ToString());
				waitTime=DateTime.Parse(table.Rows[i]["waitTime"].ToString());//we ignore date
				waitTime+=timeSpanDeltaSinceRefresh;
				row.Cells.Add(waitTime.ToString("H:mm:ss"));
				row.Bold=false;
				if(waitingRoomAlertTime>0 && waitingRoomAlertTime<=waitTime.Minute+(waitTime.Hour*60)) {
					row.ColorText=waitingRoomAlertColor;
					row.Bold=true;
				}
				gridWaiting.Rows.Add(row);
			}
			gridWaiting.EndUpdate();
		}
		#endregion Methods - Private Refresh Screen

		#region Methods - Private Search
		///<summary>Positions the search box, fills it with initial data except date, and makes it visible.</summary>
		private void ShowSearch() {
			_listProvidersSearch=new List<Provider>();
			List<Provider> listProvidersShort=Providers.GetDeepCopy(true);
			groupSearch.Location=new Point(panelCalendar.Location.X,panelCalendar.Location.Y+pinBoard.Bottom+2);
			textBefore.Text="";
			textAfter.Text="";
			_listBoxProviders.Items.Clear();
			if(pinBoard.SelectedIndex==-1){
				return;
			}
			DataRow dataRow=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].DataRowAppt;
			bool isHygiene=PIn.Bool(dataRow["IsHygiene"].ToString());
			long provHyg=PIn.Long(dataRow["ProvHyg"].ToString());
			long provNum=PIn.Long(dataRow["ProvNum"].ToString());
			long aptNum=PIn.Long(dataRow["AptNum"].ToString());
			for(int i=0;i<listProvidersShort.Count;i++) {
				if(isHygiene && listProvidersShort[i].ProvNum==provHyg) {
					//If their appiontment is hygine, the list will start with just their hygine provider
					_listBoxProviders.Items.Add(new ODBoxItem<Provider>(listProvidersShort[i].Abbr,listProvidersShort[i]));
					_listProvidersSearch.Add(listProvidersShort[i]);
				}
				else if(!isHygiene && listProvidersShort[i].ProvNum==provNum) {
					//If their appointment is not hygine, they will start with just their primary provider
					_listBoxProviders.Items.Add(new ODBoxItem<Provider>(listProvidersShort[i].Abbr,listProvidersShort[i]));
					_listProvidersSearch.Add(listProvidersShort[i]);
				}
			}
			Plugins.HookAddCode(this,"ContrAppt.ShowSearch_end",_listBoxProviders,aptNum);
			groupSearch.Visible=true;
		}

		private void DoSearch() {
			Cursor=Cursors.WaitCursor;
			DateTime afterDate;
			try {
				afterDate=PIn.Date(dateSearch.Text);
				if(afterDate.Year<1880) {
					throw new Exception();
				}
			}
			catch {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			TimeSpan beforeTime=new TimeSpan(0);
			if(textBefore.Text!="") {
				try {
					string[] hrmin=textBefore.Text.Split(new char[] { ':' },StringSplitOptions.RemoveEmptyEntries);//doesn't work with foreign times.
					string hr="0";
					if(hrmin.Length>0) {
						hr=hrmin[0];
					}
					string min="0";
					if(hrmin.Length>1) {
						min=hrmin[1];
					}
					beforeTime=TimeSpan.FromHours(PIn.Double(hr))
						+TimeSpan.FromMinutes(PIn.Double(min));
					if(radioBeforePM.Checked && beforeTime.Hours<12) {
						beforeTime=beforeTime+TimeSpan.FromHours(12);
					}
				}
				catch {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Invalid time.");
					return;
				}
			}
			TimeSpan afterTime=new TimeSpan(0);
			if(textAfter.Text!="") {
				try {
					string[] hrmin=textAfter.Text.Split(new char[] { ':' },StringSplitOptions.RemoveEmptyEntries);//doesn't work with foreign times.
					string hr="0";
					if(hrmin.Length>0) {
						hr=hrmin[0];
					}
					string min="0";
					if(hrmin.Length>1) {
						min=hrmin[1];
					}
					afterTime=TimeSpan.FromHours(PIn.Double(hr))
						+TimeSpan.FromMinutes(PIn.Double(min));
					if(radioAfterPM.Checked && afterTime.Hours<12) {
						afterTime=afterTime+TimeSpan.FromHours(12);
					}
				}
				catch {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Invalid time.");
					return;
				}
			}
			if(_listBoxProviders.Items.Count==0) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please pick a provider.");
				return;
			}
			long[] providers=new long[_listBoxProviders.Items.Count];
			List<long> providerNums = new List<long>();
			for(int i=0;i<providers.Length;i++) {
				providers[i]=_listProvidersSearch[i].ProvNum;
				providerNums.Add(_listProvidersSearch[i].ProvNum);
				//providersList.Add(providers[i]);
			}
			List<long> listOpNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				if(Clinics.ClinicNum!=0) {//not HQ
					listClinicNums.Add(Clinics.ClinicNum);
					listOpNums=Operatories.GetOpsForClinic(Clinics.ClinicNum).Select(x => x.OperatoryNum).ToList();//get ops for the currently selected clinic only
				}
				else {//HQ
					if(comboView.SelectedIndex==0) {//none view
						MsgBox.Show("Must have a view selected to search for appointment.");//this should never get hit. Just in case.
						return;
					}
					long apptViewNum=_listApptViews[comboView.SelectedIndex-1].ApptViewNum;//get the currently selected HQ view for appt search.
					//get the disctinct clinic nums for the operatories in the current appointment view
					List<long> listOpsForView=ApptViewItems.GetOpsForView(apptViewNum);
					List<Operatory> listOperatories=Operatories.GetOperatories(listOpsForView,true);
					listClinicNums=listOperatories.Select(x => x.ClinicNum).Distinct().ToList();
					listOpNums=listOperatories.Select(x => x.OperatoryNum).ToList();
				}
			}
			else {//all non hidden ops
				listOpNums=Operatories.GetDeepCopy(true).Select(x => x.OperatoryNum).ToList();
			}
			//the result might be empty
			//todo: what if pinBoard.SelectedIndex==-1?
			long aptNum=pinBoard.ListPinBoardItems[pinBoard.SelectedIndex].AptNum;
			_listScheduleOpenings=ApptSearch.GetSearchResults(aptNum,afterDate,afterDate.AddDays(731)
				,providerNums,listOpNums,listClinicNums,beforeTime,afterTime);
			listSearchResults.Items.Clear();
			for(int i=0;i<_listScheduleOpenings.Count;i++) {
				listSearchResults.Items.Add(
					_listScheduleOpenings[i].DateTimeAvail.ToString("ddd")+"\t"+_listScheduleOpenings[i].DateTimeAvail.ToShortDateString()+"     "
					+_listScheduleOpenings[i].DateTimeAvail.ToShortTimeString());
			}
			if(listSearchResults.Items.Count>0) {
				listSearchResults.SetSelected(0,true);
				SetDateSelected(_listScheduleOpenings[0].DateTimeAvail);
			}
			Cursor=Cursors.Default;
			//scroll to make visible?
			//highlight schedule?
		}
		#endregion Methods - Private Search

		#region Methods - Private Other
		private void AutomaticCallDialingDisabledMessage() {
			if(ProgramProperties.IsAdvertisingDisabled(ProgramName.DentalTekSmartOfficePhone)) {
				return;
			}
			MessageBox.Show("Automatic dialing of patient phone numbers requires an additional service.\r\n"
				+"Contact Open Dental for more information.");
			try {
				System.Diagnostics.Process.Start("http://www.opendental.com/resources/redirects/redirectdentaltekinfo.html");
			}
			catch(Exception) {
				MessageBox.Show("Could not find http://www.opendental.com/contact.html \r\n"
					+"Please set up a default web browser.");
			}
		}

		///<summary>Handles the display and refresh when the appointment we are trying to operate on is null.</summary>
		private bool ApptIsNull(Appointment appt) {
			if(appt==null) {
				MsgBox.Show("Selected appointment no longer exists.");
				RefreshPeriod();
				return true;
			}
			return false;
		}

		///<summary>Copied from FormApptsOther. Does not limit appointment creation, only warns user. This check should be run before creating a new appointment. </summary>
		private void CheckStatus() {
			if(_patCur.PatStatus == PatientStatus.Inactive
				|| _patCur.PatStatus == PatientStatus.Archived
				|| _patCur.PatStatus == PatientStatus.Prospective) {
				MsgBox.Show(this,"Warning. Patient is not active.");
			}
			if(_patCur.PatStatus == PatientStatus.Deceased) {
				MsgBox.Show(this,"Warning. Patient is deceased.");
			}
		}

		///<summary>Copies several fields from the supplied Appointment to a new Appointment object, inserts it into the database, and sends the new 
		///appointment to the Pinboard. Only used for HQ currently.</summary>
		private void CopyApptStructure(Appointment appt) {
			if(ApptIsNull(appt)) {
				return;
			}
			Appointment apptNew=Appointments.CopyStructure(appt);
			Appointments.Insert(apptNew);
			DataTable dataTable=Appointments.GetPeriodApptsTable(contrApptPanel.DateStart,contrApptPanel.DateStart,apptNew.AptNum,false);
			if(dataTable.Rows.Count==0){
				return;//silently fail
			}
			DataRow dataRow=dataTable.Rows[0];
			SendToPinboardDataRow(dataRow);
		}

		private void CopyToPin_Click() {
			if(!Security.IsAuthorized(Permissions.AppointmentMove)) {
				return;
			}
			//cannot allow moving completed procedure because it could cause completed procs to change date.  Security must block this.
			//ContrApptSingle3[thisIndex].DataRoww;
			Appointment appt=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(appt==null) {
				MsgBox.Show("Appointment not found.");
				return;
			}
			if(appt.AptStatus==ApptStatus.Complete) {
				MsgBox.Show("Not allowed to move completed appointments.");
				return;
			}
			if(PatRestrictionL.IsRestricted(appt.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			DataRow dataRow=contrApptPanel.GetDataRowForSelected();
			if(dataRow==null){
				return;//silently fail
			}
			SendToPinboardDataRow(dataRow);
		}

		///<summary>Brings up FormASAP ready to send for an open time slot.</summary>
		private void DisplayFormAsapForWebSched(long opNum,DateTime dateTimeClicked,long aptNum=0) {
			DateTime dateTimeSlotStart=contrApptPanel.DateSelected.Date;//Midnight
			DateTime dateTimeSlotEnd=contrApptPanel.DateSelected.Date.AddDays(1);//Midnight tomorrow
			//Loop through all other appts in the op to find a slot that will not overlap.
			for(int i=0;i<contrApptPanel.TableAppointments.Rows.Count;i++){
				if(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["Op"].ToString())!=opNum){
					continue;
				}
				if(PIn.Long(contrApptPanel.TableAppointments.Rows[i]["AptNum"].ToString())==aptNum){
					continue;
				}
				DateTime dateTimeAppt=PIn.DateT(contrApptPanel.TableAppointments.Rows[i]["AptDateTime"].ToString());
				if(contrApptPanel.IsWeeklyView){
					if(dateTimeAppt.Date!=contrApptPanel.DateSelected.Date){
						continue;
					}
				}
				string pattern=contrApptPanel.TableAppointments.Rows[i]["Pattern"].ToString();
				DateTime dateTimeEndApt=dateTimeAppt.AddMinutes(pattern.Length*5);
				if(dateTimeEndApt.Between(dateTimeSlotStart,dateTimeClicked)) {
					dateTimeSlotStart=dateTimeEndApt;
				}
				if(dateTimeAppt.Between(dateTimeClicked,dateTimeSlotEnd)) {
					dateTimeSlotEnd=dateTimeAppt;
				}
			}
			////Next, loop through blockouts that don't allow scheduling and adjust the slot size
			List<Def> listDefsBlockoutTypes=Defs.GetDefsForCategory(DefCat.BlockoutTypes);
			foreach(Schedule blockout in Schedules.GetForType(contrApptPanel.ListSchedules,ScheduleType.Blockout,0)
				.Where(x => x.SchedDate==dateTimeClicked.Date && x.Ops.Contains(opNum))) 
			{
				if(Schedules.CanScheduleInBlockout(blockout.BlockoutType,listDefsBlockoutTypes)) {
					continue;
				}
				DateTime dateTimeBlockoutStop=blockout.SchedDate.Add(blockout.StopTime);
				if(dateTimeBlockoutStop.Between(dateTimeSlotStart,dateTimeClicked)) {
					//Move start time to be later to account for this blockout.
					dateTimeSlotStart=dateTimeBlockoutStop;
				}
				DateTime dateTimeBlockoutStart=blockout.SchedDate.Add(blockout.StartTime);
				if(dateTimeBlockoutStart.Between(dateTimeClicked,dateTimeSlotEnd)) {
					//Move stop time to be earlier to account for this blockout.
					dateTimeSlotEnd=dateTimeBlockoutStart;
				}
				if(dateTimeClicked.Between(dateTimeBlockoutStart,dateTimeBlockoutStop,isUpperBoundInclusive: false)) {
					MsgBox.Show("Unable to schedule appointments on blockouts marked 'Block appointments scheduling'.");
					return;
				}
			}
			dateTimeSlotStart=ODMathLib.Max(dateTimeSlotStart,dateTimeClicked.AddHours(-1));
			dateTimeSlotEnd=ODMathLib.Min(dateTimeSlotEnd,dateTimeClicked.AddHours(3));
			if(_formASAP==null || _formASAP.IsDisposed) {
				_formASAP=new FormASAP();
			}
			_formASAP.ShowFormForWebSched(dateTimeClicked,dateTimeSlotStart,dateTimeSlotEnd,opNum);
			_formASAP.FormClosed+=FormASAP_FormClosed;
		}

		///<summary>Returns an ApptView for the currently logged in user and clinic combination. Can return null.
		///Will return the first available appointment view if this is the first time that this computer has connected to this database.</summary>
		private ApptView GetApptViewForUser() {
			//load the recently used apptview from the db, either the userodapptview table if an entry exists or the computerpref table if an entry for this computer exists
			ApptView apptView=null;
			UserodApptView userodApptViewCur=UserodApptViews.GetOneForUserAndClinic(Security.CurUser.UserNum,Clinics.ClinicNum);
			if(userodApptViewCur!=null) { //if there is an entry in the userodapptview table for this user
				if(_hasInitializedOnStartup //if either ContrAppt has already been initialized
					|| (Security.CurUser.ClinicIsRestricted //or the current user is restricted
					&& Clinics.ClinicNum!=ComputerPrefs.LocalComputer.ClinicNum)) //and FormOpenDental.ClinicNum (set to the current user's clinic) is not the computerpref clinic
				{
					apptView=ApptViews.GetApptView(userodApptViewCur.ApptViewNum); //then load the view for the user in the userodapptview table
				}
			}
			if(apptView==null //if no entry in the userodapptview table
				&& Clinics.ClinicNum==ComputerPrefs.LocalComputer.ClinicNum) //and if the program level ClinicNum is the stored recent ClinicNum for this computer 
			{
				apptView=ApptViews.GetApptView(ComputerPrefs.LocalComputer.ApptViewNum);//use the computerpref for this computer and user
			}
			//Larger offices do not want to take the time to load all the data required to display the "none" view.
			//Therefore, for a NEW computer that is connecting to the database for the first time, load up the first available view that is not the none view.
			if(apptView==null //if no entry in the ComputerPref table
				&& _listApptViews.Count>0) //There is an appointment view other than "none" to select
			{ 
				apptView=_listApptViews[0];
			}
			return apptView;
		}

		/// <summary></summary>
		private long GetApptViewNumForUser() {
			if(contrApptPanel.ApptViewCur!=null) {
				return contrApptPanel.ApptViewCur.ApptViewNum;
			}
			//GetApptViewForUser needs a CurUser to the specific appointment views for the clinic/user combination
			if(Security.CurUser==null) {
				//No valid user so the appointment view will be set to whatever the computerpref table has for the current computer
				return ComputerPrefs.LocalComputer.ApptViewNum;
			}
			ApptView apptView=GetApptViewForUser();
			if(apptView==null) {
				return 0;
			}
			return apptView.ApptViewNum;
		}

		///<summary>Checks if the appointment's start time overlaps another appt in the Op which the apt resides.  Tests all appts for the day, even if not visible.
		///Calling RefreshPeriod() is not necessary before calling this method. It goes to the db only as much as is necessary.
		///Returns true if no overlap found. Returns false if given apt start time conflicts with another apt in the Op.</summary>
		private static bool HasValidStartTime(Appointment apt) {
			bool notUsed;
			//Only valid if no adjust was needed.
			return !Appointments.TryAdjustAppointment(apt,UI.UserControlApptsPanelJ.GetListOpsVisible(),false,false,false,false,out notUsed,out notUsed);
		}

		///<summary>Returns true if the none appointment view is selected, clinics is turned on, and the Headquarters clinic is selected.
		///Also disables pretty much every control available in the appointment module if it is going to return true, otherwise re-enables them.</summary>
		private bool IsHqNoneView() {
			if(PrefC.HasClinicsEnabled && Clinics.ClinicNum==0 && comboView.SelectedIndex==0) {
				contrApptPanel.Visible=false;
				labelNoneView.Visible=true;
				butBack.Enabled=false;
				butBackMonth.Enabled=false;
				butBackWeek.Enabled=false;
				butToday.Enabled=false;
				butFwd.Enabled=false;
				butFwdMonth.Enabled=false;
				butFwdWeek.Enabled=false;
				Calendar2.Enabled=false;
				panelMakeButtons.Enabled=false;
				pinBoard.ClearSelected();
				textLab.Text="";
				textProduction.Text="";
				//TODO Change this to only stop printing and lists
				toolBarMain.Visible=false;
				return true;
			}
			else {
				contrApptPanel.Visible=true;
				labelNoneView.Visible=false;
				butBack.Enabled=true;
				butBackMonth.Enabled=true;
				butBackWeek.Enabled=true;
				butToday.Enabled=true;
				butFwd.Enabled=true;
				butFwdMonth.Enabled=true;
				butFwdWeek.Enabled=true;
				Calendar2.Enabled=true;
				toolBarMain.Visible=true;
				return false;
			}
		}

		private void LayoutPanels(){
			//Not quite enough control with automatic layout
			if(Calendar2.Width>panelCalendar.Width) {//for example, Chinese calendar
				panelCalendar.Width=Calendar2.Width+1;
			}
			toolBarMain.Location=new Point(ClientSize.Width-panelCalendar.Width-2,0);
			panelCalendar.Location=new Point(ClientSize.Width-panelCalendar.Width-2,toolBarMain.Height);
			panelAptInfo.Location=new Point(ClientSize.Width-panelCalendar.Width-2,toolBarMain.Height+panelCalendar.Height);
			panelMakeButtons.Location=new Point(panelAptInfo.Right+2,panelAptInfo.Top);
			contrApptPanel.Width=ClientSize.Width-panelCalendar.Width-2;//height automatic
			tabControl.Location=new Point(panelAptInfo.Left,panelAptInfo.Bottom+1);
			if(tabControl.Top>contrApptPanel.Bottom) {
				tabControl.Height=0;
			}
			else {
				tabControl.Height=contrApptPanel.Height-tabControl.Top+21;
			}
		}

		/// <summary>Called from ModuleSelected.  Just runs once for the purpose of setting start time.</summary>
		public void LayoutScrollOpProv() {
			if(_hasLaidOutScrollBar) {//Should only run on startup
				return;
			}
			//int rowsPerHr=60/contrApptPanel.MinPerIncr*contrApptPanel.RowsPerIncr;
			//use the row setting from the selected view.
			if(_listApptViews.Count>0 && comboView.SelectedIndex>0) {					
				TimeSpan apptTimeScrollStart=_listApptViews[comboView.SelectedIndex-1].ApptTimeScrollStart;
				if(_listApptViews[comboView.SelectedIndex-1].IsScrollStartDynamic) {//Scroll start time at the earliest scheduled operatory or appointment
					//jordan I would like to get rid of IsScrollStartDynamic
					//Get the schedules that have any operatory visible
					List<Schedule> listVisScheds=new List<Schedule>();
					foreach(Schedule sched in contrApptPanel.ListSchedules) {
						if(sched.Ops.Any(x => contrApptPanel.ListOpsVisible.Exists(y => x==y.OperatoryNum))//The schedule is linked to a visible operatory
							|| contrApptPanel.ListOpsVisible.Exists(x => x.ProvDentist==sched.ProvNum && !x.IsHygiene)//The dentist is in a visible operatory
							|| contrApptPanel.ListOpsVisible.Exists(x => x.ProvHygienist==sched.ProvNum && x.IsHygiene))//The hygienist is in a visible operatory
						{
							listVisScheds.Add(sched);
						}
					}
					long schedProvUnassinged=PrefC.GetLong(PrefName.ScheduleProvUnassigned);
					bool opShowsDefaultProv=false;
					foreach(Operatory op in contrApptPanel.ListOpsVisible) {
						if((op.ProvDentist!=0 && !op.IsHygiene)
							||(op.ProvHygienist!=0 && op.IsHygiene))
						{
							continue;//The operatory has a provider assigned to it
						}
						if(contrApptPanel.ListSchedules.Any(x => x.Ops.Contains(op.OperatoryNum))) {
							continue;//The operatory has a scheduled assigned to it
						}
						opShowsDefaultProv=true;//The operatory will have the provider for unassigned operatories
						break;
					}
					if(opShowsDefaultProv && contrApptPanel.ListSchedules.Exists(x => x.ProvNum==schedProvUnassinged)) {//The provider for unassigned ops has a schedule
						//Add that provider's earliest schedule
						listVisScheds.Add(contrApptPanel.ListSchedules.FindAll(x => x.ProvNum==schedProvUnassinged).OrderBy(x => x.StartTime).FirstOrDefault());
					}
					//Get the appointment times that are in a visible operatory
					List<TimeSpan> listVisAptTimes=new List<TimeSpan>();
					foreach(DataRow row in contrApptPanel.TableAppointments.Rows) {
						long opNum=PIn.Long(row["Op"].ToString());
						if(!contrApptPanel.ListOpsVisible.Exists(x => x.OperatoryNum==opNum) //The appointment is in a visible operatory
							|| !new[] { "1","2","4","5","7","8" }.Contains(row["AptStatus"].ToString())) //Scheduled,Complete,ASAP,Broken,PtNote,PtNoteComp
						{
							continue;
						}
						listVisAptTimes.Add(PIn.Date(row["AptDateTime"].ToString()).TimeOfDay);
					}
					TimeSpan earliestApt=new TimeSpan();
					TimeSpan earliestOp=new TimeSpan();
					if(listVisAptTimes.Count>0 && listVisScheds.Count>0) {//There is at least one schedule and at least one appointment visible
						earliestApt=listVisScheds.Min(x => x.StartTime);
						earliestOp=listVisAptTimes.Min();
						if(TimeSpan.Compare(earliestOp,earliestApt)==1) {//earliestOp is later than earliestApt
							apptTimeScrollStart=earliestApt;
						}
						else {//earliestApt is later than earliestOp or they are both equal
							apptTimeScrollStart=earliestOp;
						}
					}
					else if(listVisScheds.Count>0) {//There is at least one visible schedule and no visible appointments
						apptTimeScrollStart=listVisScheds.Min(x => x.StartTime);
					}
					else if(listVisAptTimes.Count>0) {//There is at least one visible appointment and no visible schedules
						apptTimeScrollStart=listVisAptTimes.Min();
					}
					//else apptTimeScrollStart will remain as the start time listed in the appt view		
				}
				//Even if we skip isScrollStartDynamic, above, the code below will still handle ApptTimeScrollStart
				contrApptPanel.SetScrollByTime(apptTimeScrollStart);
			}
			else{//else no view selected
				contrApptPanel.SetScrollByTime(TimeSpan.FromHours(8));
			}
			_hasLaidOutScrollBar=true;
			contrApptPanel.RedrawAsNeeded();//Todo: probably not needed.
		}

		///<summary>Used for the UpdateProvs tool to reassign all future appointments for one op to another prov.</summary>
		private void MoveAppointments(List<Appointment> listAppts,List<Appointment> listApptsOld,Operatory operatoryCur)
		{
			Appointment appt=null;
			Appointment apptOld=null;
			List<Schedule> listSchedsForOp=Schedules.GetSchedsForOp(operatoryCur,listAppts.Select(x => x.AptDateTime).ToList());
			List<Operatory> listOpsForClinic=contrApptPanel.ListOpsVisible.Select(x => x.Copy()).ToList();
			if(((ApptSchedEnforceSpecialty)PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty)).In(ApptSchedEnforceSpecialty.Block,ApptSchedEnforceSpecialty.Warn)) {
				//if specialties are enforced, don't auto-move appt into an op assigned to a different clinic than the curOp's clinic
				listOpsForClinic.RemoveAll(x => x.ClinicNum!=operatoryCur.ClinicNum);
			}
			for(int i=0;i<listAppts.Count;i++) {
				MoveAppointment(appt,apptOld,listSchedsForOp,true);
			}
		}

		///<summary>Mostly for moving a single appointment.  Similar to the logic which runs in pinBoard_MouseUp(), but pinBoard_MouseUp() has additional things that are done.  This is also used for the UpdateProvs tool to reassign all future appointments for one op to another prov.</summary>
		private void MoveAppointment(Appointment appt,Appointment apptOld,List<Schedule> listSchedsForOp=null,bool isOpUpdate=false){
			bool timeWasMoved=appt.AptDateTime!=apptOld.AptDateTime;
			bool isOpChanged=appt.Op!=apptOld.Op;
			Operatory operatoryCur=Operatories.GetOperatory(appt.Op);
			Patient patCur=null;
			//List<Schedule> listSchedsForOp=Schedules.GetSchedsForOp(appt.Op,listAppts.Select(x => x.AptDateTime).ToList());
			List<Operatory> listOpsForClinic=contrApptPanel.ListOpsVisible.Select(x => x.Copy()).ToList();
			if(((ApptSchedEnforceSpecialty)PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty)).In(ApptSchedEnforceSpecialty.Block,ApptSchedEnforceSpecialty.Warn)) {
				//if specialties are enforced, don't auto-move appt into an op assigned to a different clinic than the curOp's clinic
				listOpsForClinic.RemoveAll(x => x.ClinicNum!=operatoryCur.ClinicNum);
			}
			patCur=Patients.GetPat(appt.PatNum);
			bool provChanged=false;
			bool hygChanged=false;
			long assignedDent=0;
			long assignedHyg=0;
			if(isOpUpdate) {
				assignedDent=Schedules.GetAssignedProvNumForSpot(listSchedsForOp,operatoryCur,false,appt.AptDateTime);
				assignedHyg=Schedules.GetAssignedProvNumForSpot(listSchedsForOp,operatoryCur,true,appt.AptDateTime);
			}
			else { 
				assignedDent=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,operatoryCur,false,appt.AptDateTime);
				assignedHyg=Schedules.GetAssignedProvNumForSpot(contrApptPanel.ListSchedules,operatoryCur,true,appt.AptDateTime);
			}
			List<Procedure> procsForSingleApt=null;
			if(appt.AptStatus!=ApptStatus.PtNote && appt.AptStatus!=ApptStatus.PtNoteCompleted) {
				if(timeWasMoved) {
					#region Update Appt's DateTimeAskedToArrive
					if(patCur.AskToArriveEarly>0) {
						appt.DateTimeAskedToArrive=appt.AptDateTime.AddMinutes(-patCur.AskToArriveEarly);
						MessageBox.Show(Lan.g(this,"Ask patient to arrive")+" "+patCur.AskToArriveEarly
							+" "+Lan.g(this,"minutes early at")+" "+appt.DateTimeAskedToArrive.ToShortTimeString()+".");
					}
					else {
						if(appt.DateTimeAskedToArrive.Year>1880 && (apptOld.AptDateTime-apptOld.DateTimeAskedToArrive).TotalMinutes>0) {
							appt.DateTimeAskedToArrive=appt.AptDateTime-(apptOld.AptDateTime-apptOld.DateTimeAskedToArrive);
							if(MessageBox.Show(Lan.g(this,"Ask patient to arrive")+" "+(apptOld.AptDateTime-apptOld.DateTimeAskedToArrive).TotalMinutes
								+" "+Lan.g(this,"minutes early at")+" "+appt.DateTimeAskedToArrive.ToShortTimeString()+"?","",MessageBoxButtons.YesNo)==DialogResult.No) {
								appt.DateTimeAskedToArrive=apptOld.DateTimeAskedToArrive;
							}
						}
						else {
							appt.DateTimeAskedToArrive=DateTime.MinValue;
						}
					}
					#endregion Update Appt's DateTimeAskedToArrive
				}
				#region Update Appt's Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
				//if no dentist/hygenist is assigned to spot, then keep the original dentist/hygenist without prompt.  All appts must have prov.
				if((assignedDent!=0 && assignedDent!=appt.ProvNum) || (assignedHyg!=0 && assignedHyg!=appt.ProvHyg)) {
					object[] parameters3={ appt,assignedDent,assignedHyg,procsForSingleApt,this };//Only used in following plugin hook.
					if((Plugins.HookMethod(this,"ContrAppt.ContrApptSheet2_MouseUp_apptProvChangeQuestion",parameters3))) {
						appt=(Appointment)parameters3[0];
						assignedDent=(long)parameters3[1];
						assignedDent=(long)parameters3[2];
						goto PluginApptProvChangeQuestionEnd;
					}
					if(isOpUpdate || MsgBox.Show(this,MsgBoxButtons.YesNo,"Change provider?")) {//Short circuit logic.  If we're updating op through right click, never ask.
						if(assignedDent!=0) {//the dentist will only be changed if the spot has a dentist.
							appt.ProvNum=assignedDent;
							provChanged=true;
						}
						if(assignedHyg!=0 || PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {//the hygienist will only be changed if the spot has a hygienist.
							appt.ProvHyg=assignedHyg;
							hygChanged=true;
						}
						if(operatoryCur.IsHygiene) {
							appt.IsHygiene=true;
						}
						else {//op not marked as hygiene op
							if(assignedDent==0) {//no dentist assigned
								if(assignedHyg!=0) {//hyg is assigned (we don't really have to test for this)
									appt.IsHygiene=true;
								}
							}
							else {//dentist is assigned
								if(assignedHyg==0) {//hyg is not assigned
									appt.IsHygiene=false;
								}
								//if both dentist and hyg are assigned, it's tricky
								//only explicitly set it if user has a dentist assigned to the op
								if(operatoryCur.ProvDentist!=0) {
									appt.IsHygiene=false;
								}
							}
						}
						procsForSingleApt=Procedures.GetProcsForSingle(appt.AptNum,false);
						List<long> codeNums=new List<long>();
						for(int p = 0;p<procsForSingleApt.Count;p++) {
							codeNums.Add(procsForSingleApt[p].CodeNum);
						}
						if(!isOpUpdate) { 
							string calcPattern=Appointments.CalculatePattern(appt.ProvNum,appt.ProvHyg,codeNums,true);
							if(appt.Pattern!=calcPattern && !PrefC.GetBool(PrefName.AppointmentTimeIsLocked)) {//Updating op provs will not change apt lengths.
								if(appt.TimeLocked) {
									if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Appointment length is locked.  Change length for new provider anyway?")) {
										appt.Pattern=calcPattern;
									}
								}
								else {//appt time not locked
									if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Change length for new provider?")) {
										appt.Pattern=calcPattern;
									}
								}
							}
						}
					}
					PluginApptProvChangeQuestionEnd: { }
				}
				#region Provider Term Date Check
				//Prevents appointments with providers that are past their term end date from being scheduled
				string message=Providers.CheckApptProvidersTermDates(appt);
				if(message!="") {
					MessageBox.Show(this,message);//translated in Providers S class method
					return;
				}
				#endregion Provider Term Date Check
				#endregion Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
			}
			#region Prevent overlap
			if(!AllowOverlap){
				if(!isOpUpdate && !Appointments.TryAdjustAppointmentOp(appt,listOpsForClinic)) {
					MessageBox.Show(Lan.g(this,"Appointment overlaps existing appointment or blockout."));
					return;
				}
			}
			#endregion Prevent overlap
			#region Detect Frequency Conflicts
			//Detect frequency conflicts with procedures in the appointment
			if(!isOpUpdate && PrefC.GetBool(PrefName.InsChecksFrequency)) {
				procsForSingleApt=Procedures.GetProcsForSingle(appt.AptNum,false);
				string frequencyConflicts="";
				try {
					frequencyConflicts=Procedures.CheckFrequency(procsForSingleApt,appt.PatNum,appt.AptDateTime);
				}
				catch(Exception e) {
					MessageBox.Show(Lan.g(this,"There was an error checking frequencies."
						+"  Disable the Insurance Frequency Checking feature or try to fix the following error:")
						+"\r\n"+e.Message);
					return;
				}
				if(frequencyConflicts!="" && MessageBox.Show(Lan.g(this,"Scheduling this appointment for this date will cause frequency conflicts for the following procedures")
					+":\r\n"+frequencyConflicts+"\r\n"+Lan.g(this,"Do you want to continue?"),"",MessageBoxButtons.YesNo)==DialogResult.No)
				{
					return;
				}
			}
			#endregion Detect Frequency Conflicts
			#region Patient status
			if(!isOpUpdate) {
				Operatory opCur=Operatories.GetOperatory(appt.Op);
				Operatory opOld=Operatories.GetOperatory(apptOld.Op);
				if(opOld==null||opCur.SetProspective!=opOld.SetProspective) {
					if(opCur.SetProspective&&patCur.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will be set to Prospective.")) {
							Patient patOld=patCur.Copy();
							patCur.PatStatus=PatientStatus.Prospective;
							Patients.Update(patCur,patOld);
						}
					}
					else if(!opCur.SetProspective&&patCur.PatStatus==PatientStatus.Prospective) {
						//Do we need to warn about changing FROM prospective? Assume so for now.
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Patient's status will change from Prospective to Patient.")) {
							Patient patOld=patCur.Copy();
							patCur.PatStatus=PatientStatus.Patient;
							Patients.Update(patCur,patOld);
						}
					}
				}
			}
			#endregion Patient status
			#region Update Appt's AptStatus, ClinicNum, Confirmed
			object[] parameters2 = { appt.AptDateTime,apptOld.AptDateTime,appt.AptStatus };
			if((Plugins.HookMethod(this,"ContrAppt.ContrApptSheet2_MouseUp_apptDoNotUnbreakApptSameDay",parameters2))) {
				appt.AptStatus=(ApptStatus)parameters2[2];
				goto PluginApptDoNotUnbreakApptSameDay;
			}
			if(appt.AptStatus==ApptStatus.Broken&&(timeWasMoved||isOpChanged)) {
				appt.AptStatus=ApptStatus.Scheduled;
			}
			PluginApptDoNotUnbreakApptSameDay: { }
			//original location of provider code
			if(operatoryCur.ClinicNum==0) {
				appt.ClinicNum=patCur.ClinicNum;
			}
			else {
				appt.ClinicNum=operatoryCur.ClinicNum;
			}
			if(appt.AptDateTime!=apptOld.AptDateTime
				&& appt.Confirmed!=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum
				&& appt.AptDateTime.Date!=DateTime.Today) 
			{
				string prompt=(PrefC.GetBool(PrefName.ApptConfirmAutoEnabled) ? "Do you want to resend the eConfirmation?" 
					: "Reset Confirmation Status?");
				bool doResetConf=MsgBox.Show(this,MsgBoxButtons.YesNo,prompt);
				if(doResetConf) {
					appt.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;//Causes the confirmation status to be reset.
				}
				if(PrefC.GetBool(PrefName.ApptConfirmAutoEnabled)) {
					List<ConfirmationRequest> listConfirmations=ConfirmationRequests.GetAllForAppts(new List<long> { appt.AptNum});
					foreach(ConfirmationRequest request in listConfirmations) {
						//If they selected No, this will force the econnector to not delete the row and therefore not send another eConfirmation.
						//If they selected Yes, we will clear the DoNotResend flag so that it will get sent.
						request.DoNotResend=!doResetConf;
						ConfirmationRequests.Update(request);
					}
				}
			}
			#endregion Update Appt's AptStatus, ClinicNum, Confirmed
			try {
				//Should only need this check if changing/updating Op. Assumes we didn't previously schedule the apt somewhere it shouldn't have been.
				if(!AppointmentL.IsSpecialtyMismatchAllowed(patCur.PatNum,appt.ClinicNum)) {
					return;
				}
				if(isOpUpdate) {
					Appointments.MoveValidatedAppointment(appt,apptOld,patCur,operatoryCur,listSchedsForOp,listOpsForClinic,provChanged,hygChanged,timeWasMoved,isOpChanged,isOpUpdate);
				}
				else { 
					Appointments.MoveValidatedAppointment(appt,apptOld,patCur,operatoryCur,contrApptPanel.ListSchedules,listOpsForClinic,provChanged,hygChanged,timeWasMoved,isOpChanged,isOpUpdate);
				}
			}
			catch(Exception e) {
				MsgBox.Show(this,e.Message);
			}
		}

		///<summary></summary>
		private void PrintApptCard() {
			PrinterL.TryPrintOrDebugRpPreview(pd2_PrintApptCard,
				"Appointment reminder postcard printed",
				printSit:PrintSituation.Postcard,
				auditPatNum:_patCur.PatNum,
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void pd2_PrintApptCard(object sender,PrintPageEventArgs ev) {
			Graphics g=ev.Graphics;
			long apptClinicNum=0;
			if(contrApptPanel.SelectedAptNum>0){
				apptClinicNum=PIn.Long(contrApptPanel.GetDataRowForSelected()["ClinicNum"].ToString());
			}		
			Clinic clinic=Clinics.GetClinic(apptClinicNum);
			//Return Address--------------------------------------------------------------------------
			string str="";
			string phone="";
			if(PrefC.HasClinicsEnabled && clinic!=null) {//Use clinic on appointment if clinic exists and has clinics enabled
				str=clinic.Description+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,60,60);
				str=clinic.Address+"\r\n";
				if(clinic.Address2!="") {
					str+=clinic.Address2+"\r\n";
				}
				str+=clinic.City+"  "+clinic.State+"  "+clinic.Zip+"\r\n";
				phone=clinic.Phone;
			}
			else {//Otherwise use practice information
				str=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,60,60);
				str=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
				if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
					str+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
				}
				str+=PrefC.GetString(PrefName.PracticeCity)+"  "
					+PrefC.GetString(PrefName.PracticeST)+"  "
					+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
				phone=PrefC.GetString(PrefName.PracticePhone);
			}
			if(CultureInfo.CurrentCulture.Name=="en-US" && phone.Length==10) {
				str+="("+phone.Substring(0,3)+")"+phone.Substring(3,3)+"-"+phone.Substring(6);
			}
			else {//any other phone format
				str+=phone;
			}
			g.DrawString(str,new Font(FontFamily.GenericSansSerif,8),Brushes.Black,60,75);
			//Body text-------------------------------------------------------------------------------
			string name;
			str="Appointment Reminders:"+"\r\n\r\n";
			Appointment[] aptsOnePat;
			Family fam=Patients.GetFamily(_patCur.PatNum);
			Patient pat=fam.GetPatient(_patCur.PatNum);
			for(int i=0;i<fam.ListPats.Length;i++) {
				if(!_doPrintCardFamily && fam.ListPats[i].PatNum!=pat.PatNum) {
					continue;
				}
				name=fam.ListPats[i].FName;
				if(name.Length>15) {//trim name so it won't be too long
					name=name.Substring(0,15);
				}
				aptsOnePat=Appointments.GetForPat(fam.ListPats[i].PatNum);
				for(int a=0;a<aptsOnePat.Length;a++) {
					if(aptsOnePat[a].AptDateTime.Date<=DateTime.Today) {
						continue;//ignore old appts
					}
					if(aptsOnePat[a].AptStatus!=ApptStatus.Scheduled){
						continue;
					}
					str+=name+": "+aptsOnePat[a].AptDateTime.ToShortDateString()+" "+aptsOnePat[a].AptDateTime.ToShortTimeString()+"\r\n";
				}
			}
			g.DrawString(str,new Font(FontFamily.GenericSansSerif,9),Brushes.Black,40,180);
			//Patient's Address-----------------------------------------------------------------------
			Patient guar;
			if(_doPrintCardFamily) {
				guar=fam.ListPats[0].Copy();
			}
			else {
				guar=pat.Copy();
			}
			str=guar.FName+" "+guar.LName+"\r\n"
				+guar.Address+"\r\n";
			if(guar.Address2!="") {
				str+=guar.Address2+"\r\n";
			}
			str+=guar.City+"  "+guar.State+"  "+guar.Zip;
			g.DrawString(str,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,300,240);
			//CommLog entry---------------------------------------------------------------------------
			Commlog CommlogCur=new Commlog();
			CommlogCur.CommDateTime=DateTime.Now;
			CommlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			CommlogCur.Note="Appointment card sent";
			CommlogCur.PatNum=pat.PatNum;
			CommlogCur.UserNum=Security.CurUser.UserNum;
			//there is no dialog here because it is just a simple entry
			Commlogs.Insert(CommlogCur);
			ev.HasMorePages = false;
		}

		private void PrintApptLabel() {
			Appointment apt=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
			if(ApptIsNull(apt)) { return; }
			LabelSingle.PrintAppointment(contrApptPanel.SelectedAptNum);
		}

		///<summary>Processes the OtherResult from a call to FormApptsOther.</summary>
		private void ProcessOtherDlg(OtherResult result,long patNum,string strDateJumpTo,params long[] arraySelectedAptNums) {
			if(result==OtherResult.Cancel) {
				return;
			}
			switch(result) {
				case OtherResult.CopyToPinBoard:
					SendToPinBoardAptNums(arraySelectedAptNums.ToList());
					RefreshModuleDataPatient(patNum);
					FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
					RefreshPeriod();
					break;
				case OtherResult.NewToPinBoard:
					SendToPinBoardAptNums(arraySelectedAptNums.ToList());
					RefreshModuleDataPatient(patNum);
					FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
					RefreshPeriod();
					break;
				case OtherResult.PinboardAndSearch:
					SendToPinBoardAptNums(arraySelectedAptNums.ToList());
					if(contrApptPanel.IsWeeklyView) {
						break;
					}
					dateSearch.Text=strDateJumpTo;
					if(!groupSearch.Visible) {//if search not already visible
						ShowSearch();
					}
					DoSearch();
					break;
				case OtherResult.CreateNew:
					contrApptPanel.SelectedAptNum=arraySelectedAptNums[0];
					RefreshModuleDataPatient(patNum);
					FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
					Appointment apt=Appointments.GetOneApt(contrApptPanel.SelectedAptNum);
					Appointment aptOld=apt.Copy();
					if(!HasValidStartTime(apt)) {
						MsgBox.Show("Appointment start time would overlap another appointment.  Moving appointment to pinboard.");
						DataRow dataRow=contrApptPanel.GetDataRowForSelected();
						if(dataRow==null){
							return;//silently fail
						}
						SendToPinboardDataRow(dataRow);
						apt.AptStatus=ApptStatus.UnschedList;
						try {
							Appointments.Update(apt,aptOld);
						}
						catch(ApplicationException ex) {
							MessageBox.Show(ex.Message);
						}
						RefreshPeriod();
						break;
					}
					if(TryAdjustAppointmentPattern(apt)) {
						MsgBox.Show(this,"Appointment is too long and would overlap another appointment.  Automatically shortened to fit.");						
						try {
							Appointments.Update(apt,aptOld);//Appointments S-Class handles Signalods
						}
						catch(ApplicationException ex) {
							MessageBox.Show(ex.Message);
						}
					}
					RefreshPeriod();
					break;
				case OtherResult.GoTo:
					contrApptPanel.SelectedAptNum=arraySelectedAptNums[0];
					contrApptPanel.DateSelected=PIn.Date(strDateJumpTo);
					if(_patCur.PatNum!=patNum) {
						//ModuleSelected->RefreshModuleScreenPeriod, Appt won't be selected if PatCur.PatNum!=Appt.PatNum
						_patCur=Patients.GetPat(patNum);
					}
					FormOpenDental.S_Contr_PatientSelected(_patCur,true,false,true);
					break;
			}
		}

		///<summary>Brings up the window to send text messagse to the patients.</summary>
		private void SendTextMessages(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				MsgBox.Show("No appointments this day to send text messages to.");
				return;
			}
			Clinic curClinic=Clinics.GetClinic(Clinics.ClinicNum)??Clinics.GetDefaultForTexting()??Clinics.GetPracticeAsClinicZero();
			List<PatComm> listPatComms=Patients.GetPatComms(listPatNums,curClinic,false);
			List<string> listPatsSkipped=new List<string>();
			for(int i=listPatComms.Count-1;i>=0;i--) {
				//Remove patients that can't receive texts.
				PatComm patComm=listPatComms[i];
				if(patComm.IsSmsAnOption) {
					continue;
				}
				listPatsSkipped.Add(patComm.FName+" "+patComm.LName+": "+patComm.GetReasonCantText());
				listPatComms.RemoveAt(i);
			}
			if(listPatsSkipped.Count > 0) {
				string msg=listPatsSkipped.Count+" of the "+listPatNums.Distinct().Count()+" "
					+"patients cannot receive text messages:"+"\r\n"+string.Join("\r\n",listPatsSkipped);
				if(listPatsSkipped.Count < 8) {
					MessageBox.Show(msg);
				}
				else {
					new MsgBoxCopyPaste(msg).ShowDialog();
				}
			}
			if(listPatComms.Count==0) {
				return;
			}
			FormTxtMsgMany formTxtMsgMany=new FormTxtMsgMany(listPatComms,"",Clinics.ClinicNum,SmsMessageSource.DirectSms);
			formTxtMsgMany.DoCombineNumbers=true;
			formTxtMsgMany.Show();
		}

		/// <summary>Sets the index of comboView for the specified ApptViewNum.  Then, does a ModuleSelected().  If saveToDb, then it will remember the ApptViewNum and currently selected ClinicNum for this workstation.</summary>
		private void SetView(long apptViewNum,bool saveToDb) {
			comboView.SelectedIndex=0;//this would be circular if we used event indexchanged, but we use changeCommitted.
			for(int i=0;i<_listApptViews.Count;i++) {
				if(apptViewNum==_listApptViews[i].ApptViewNum) {
					comboView.SelectedIndex=i+1;//+1 for 'none'
					break;
				}
			}
			if(!_hasInitializedOnStartup) {
				return;//prevent ModuleSelected().
			}
			if(_hasInitializedOnStartup && !Visible) {
				return;
			}
			if(saveToDb) {
				ComputerPrefs.LocalComputer.ApptViewNum=apptViewNum;
				ComputerPrefs.LocalComputer.ClinicNum=Clinics.ClinicNum;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
				UserodApptViews.InsertOrUpdate(Security.CurUser.UserNum,Clinics.ClinicNum,apptViewNum);
			}
			if(_patCur==null) {
				ModuleSelected(0,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
			}
			else {
				ModuleSelected(_patCur.PatNum,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
			}
		}

		///<summary>Switches between weekly view and daily view.   Calls either RefreshPeriod or ModuleSelected.</summary>
		private void SetWeeklyView(bool isWeeklyView) {
			//Completely independent from and does not affect contrApptPanel.DateSelected.
			//if the weekly view doesn't change, then use SetDateSelected or RefreshPeriod
			if(isWeeklyView) {
				radioWeek.Checked=true;
				butFwd.Enabled=false;
				butBack.Enabled=false;
			}
			else {
				radioDay.Checked=true;
				butFwd.Enabled=true;
				butBack.Enabled=true;
			}
			contrApptPanel.IsWeeklyView=isWeeklyView;
			if(!_hasInitializedOnStartup) {
				return;//prevent refreshing repeatedly on startup
			}
			long apptViewNum=0;
			if(contrApptPanel.ApptViewCur!=null) {
				apptViewNum=contrApptPanel.ApptViewCur.ApptViewNum;
			}
			if(isWeeklyView) {
				if(_patCur==null) {
					ModuleSelected(0,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				}
				else {
					ModuleSelected(_patCur.PatNum,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				}
			}
			else {
				RefreshPeriod(listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum),isRefreshSchedules:true);
			}
		}

		///<summary>This is a good way to set contrApptPanel.DateSelected while also refreshing the module.  If you are not changing the date or patient, then instead, simply use RefreshPeriod().</summary>
		private void SetDateSelected(DateTime date){
			contrApptPanel.BeginUpdate();//otherwise, the appointments will disappear
			contrApptPanel.DateSelected=date;
			long apptViewNum=0;
			if(contrApptPanel.ApptViewCur!=null) {
				apptViewNum=contrApptPanel.ApptViewCur.ApptViewNum;
			}
			if(contrApptPanel.IsWeeklyView) {
				if(_patCur==null) {
					ModuleSelected(0,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				}
				else {
					ModuleSelected(_patCur.PatNum,listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum));
				}
			}
			else {
				RefreshPeriod(listOpNums:ApptViewItems.GetOpsForView(apptViewNum),listProvNums:ApptViewItems.GetProvsForView(apptViewNum),isRefreshSchedules:true);
			}
			contrApptPanel.EndUpdate();
		}

		///<summary>Used when passing a family to the pinboard.</summary>
		private void SendToPinBoardAptNums(List<long> aptNums) {
			if(IsHqNoneView()) {
				MsgBox.Show("Appointments can't be sent to the pinboard when an appointment view or clinic hasn't been selected.");
				return;
			}
			if(aptNums.Count==0) {
				return;
			}
			long patNum=0;
			for(int i=0;i<aptNums.Count;i++){
				//sometimes, before this method was called, module was refreshed, and these appts were included.
				DataRow dataRow=null;
				for(int r=0;r<contrApptPanel.TableAppointments.Rows.Count;r++){
					if(contrApptPanel.TableAppointments.Rows[r]["AptNum"].ToString()!=aptNums[i].ToString()){
						continue;
					}
					dataRow=contrApptPanel.TableAppointments.Rows[r];
				}
				if(dataRow==null){
					//but sometimes, we need to go get the row manually
					DataTable dataTable=Appointments.GetPeriodApptsTable(contrApptPanel.DateStart,contrApptPanel.DateEnd,aptNums[i],false);
					if(dataTable.Rows.Count==0){
						continue;//fail silently?
					}
					dataRow=dataTable.Rows[0];
				}
				string pattern=PIn.String(dataRow["Pattern"].ToString());
				string patternShowing=contrApptPanel.GetPatternShowing(pattern);
				SizeF sizeAppt=contrApptPanel.SetSize(pattern);
				Bitmap bitmap=new Bitmap(pinBoard.Width-2,(int)sizeAppt.Height);
				using(Graphics g = Graphics.FromImage(bitmap)){
					contrApptPanel.GetBitmapForPinboard(g,dataRow,patternShowing,bitmap.Width,bitmap.Height);
				}
				//Bitmap bitmap=contrApptPanel.GetBitmapForPinboard(dataRow,pinBoard.Width);
				long aptNum=PIn.Long(dataRow["AptNum"].ToString());
				pinBoard.AddAppointment(bitmap,aptNum,dataRow);
				bitmap.Dispose();//?
				if(i==aptNums.Count-1) { //Set the pt to the last appt on the pinboard.
					patNum=PIn.Long(dataRow["PatNum"].ToString());
				}
			}
			if(patNum==0 && _patCur!=null) {
				patNum=_patCur.PatNum;
			}
			RefreshModuleDataPatient(patNum); 
			if(_patCur!=null) {
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			}
		}

		///<summary>Used when dragging an appt to the pinboard.</summary>
		private void SendToPinboardDataRow(DataRow dataRow) {
			if(IsHqNoneView()) {
				MsgBox.Show("Appointments can't be sent to the pinboard when an appointment view or clinic hasn't been selected.");
				return;
			}
			string pattern=PIn.String(dataRow["Pattern"].ToString());
			string patternShowing=contrApptPanel.GetPatternShowing(pattern);
			SizeF sizeAppt=contrApptPanel.SetSize(pattern);
			Bitmap bitmap=new Bitmap(pinBoard.Width-2,(int)sizeAppt.Height);
			using(Graphics g = Graphics.FromImage(bitmap)){
				contrApptPanel.GetBitmapForPinboard(g,dataRow,patternShowing,bitmap.Width,bitmap.Height);
			}
			//Bitmap bitmap=contrApptPanel.GetBitmapForPinboard(dataRow,pinBoard.Width);
			long aptNum=PIn.Long(dataRow["AptNum"].ToString());
			pinBoard.AddAppointment(bitmap,aptNum,dataRow);
			bitmap.Dispose();//?
			long patNum=PIn.Long(dataRow["PatNum"].ToString());
			RefreshModuleDataPatient(patNum); 
			//if(_patCur!=null) {
				FormOpenDental.S_Contr_PatientSelected(_patCur,true,false);
			//}
		}

		///<summary>Shortens appt.Pattern if overlap is found in neighboring op within appt.Op. Pattern will be adjusted to a minimum of 1 until no overlap occurs. Calling RefreshPeriod() is not necessary before calling this method. It goes to the db only as much as is necessary. Returns true if patter was adjusted. Returns false if pattern was not adjusted.</summary>
		public static bool TryAdjustAppointmentPattern(Appointment appt) {
			bool isPatternChanged;
			bool notUsed;
			Appointments.TryAdjustAppointment(appt,UI.UserControlApptsPanelJ.GetListOpsVisible(),false,true,true,true,out isPatternChanged,out notUsed);
			return isPatternChanged;
		}




		#endregion Methods - Private Other

		
	}
}

//todo:
//RefreshReminders
//Where are pinboard TableApptFields stored?  Need them when dragging off in order to make a copy.
//Dispose of bitmaps on pinboard items when they are removed from list
//FormApptsOther.MakeRecallAppointment, and similar methods.
//FormOpenDental.GotoModule_ModuleSelected needs to be revisited for changing date
//Check that appt colored procs are wrapping properly
//Full line-by-line review and compare old vs new
//MsgBox translations
//Hooks
//isInsuranceColor not used
//Chased down all "todo"s in the two new files.
//Make a checkbox to turn this feature on and off

//Needs more testing:
//Grids at lower right
//Search
//DrawWebSchedASAPSlots (I don't know how to test this)
//Blockouts

//For Documentation, new feature description:
//Multiple appointments can be scheduled in the same operatory.  (But this is not really worth putting in the manual because it's obvious)


