using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental.UI {
	//Jordan is the only one allowed to edit this file.
	//The primary use of this control is currently as a filter for reports, etc.
	//There should be no problems with this use.  The user cannot "see" clinics that they shouldn't, which is great and very useful.
	//But we need to be very careful in the case of using this combobox as a picker for fields on an object.
	//Since clinics with no permission would be hidden, this could inadvertently causes changes in which clinics are linked.  
	//Also, if the "all" option really means "all", then the calling form must specifically test IsAllSelected. 
	//"All" should not be used when the intent is ClinicNum=0
	//If the calling form instead uses ListSelectedClinicNums when All is selected, that's very different, and will exclude clinics not showing due to no permission.
	//This control is currently used in 29 places.  Most of these are at the top, in the filter section of reports.  Here are the exceptions:
	//FormEServiceSetup has 4 comboBoxes.  I understand clinicPickerEClipboard, but not the other 3, so I can't police those 3.
	//FormPatientAddAll has 5 comboBoxes.  They don't include All or Unassigned, and they are all totally harmless, without potential for bugs.
	//FormPharmacyEdit: Includes All and Unassigned. It's multiselect. Does not test "All".  Because it uses a synch mechanism, clinics that aren't showing aren't affected either way.
	//FormRxEdit: Does not include All.  Includes Unassigned.  Single select. On save, if no selection, then it doesn't change the existing clinicNum.  I assume, but have not thoroughly verified, that the user must have permission for the clinic of this patient in order to get into their Rx.  So, we shouldn't have to worry about the clinic for this Rx not showing in the combobox, or at least it would be extremely rare and not worth addressing because an immediate OK would not change the value.

		//Should be ready now for implementation in any of the following 56 locations (and others I missed):
	//FormAdjMulti(picker), FormAdjust, FormApptEdit, FormApptViewEdit, FormApptViews, FormAsapSetup, FormBilling, FormBillingDefaults, 
	//FormBillingOptions(multi-select and picker), FormClaimEdit,
	//FormClaimPayEdit, FormClaimPayList, FormClaimSend, FormClearinghouseEdit, FormClearinghouses, FormClockEventEdit, FormCloneAdd, FormDepositEdit,
	//FormDeposits, FormDoseSpotAssignClinicId, FormDunningEdit(picker), FormDunningSetup(picker), FormEhrPatientExports, FormEServicesECR, FormEtrans835(multi-select), 
	//FormFeeSchedTools(multi-select badly implemented),FormFeeSchedTools2, FormLabCases, FormMedLabs, FormOperatoryEdit(picker), FormOrthoAutoClaims, FormPatientEdit, 
	//FormPatientSelect(might need to be more powerful for HQ use), FormPayConnectSetup, FormPayment, FormPayPlan, FormPaySimpleSetup, FormPaySplitEdit,
	//FormPendingPayments, FormPodiumSetup, FormProcBroken, FormProcCodes(higher priority), FormProcEdit, FormProcEditAll(pickerbox), FormSchedule,
	//FormScheduleDayEdit, FormScheduleEdit, FormSheetDefDefaults, FormSmsTextMessaging, FormTimeCardManage, FormTransworldSetup, 
	//FormWebForms(multi and picker), FormWebFormsSetup(picker), FormWebSchedASAPHistory, FormXChargeSetup, FormXWebTransactions
	//Many of these Forms are obviously not urgent at all because the clinic gets assigned automatically and wouldn't be changed.
	//But this control is ready to drop into any of the above forms, should the need arise.  Some of the forms need more help than others.

	///<summary>A Clinic comboBox that can scale up to thousands of Clinics.  Also handles multi select.  Fills itself with a filtered list of clinics that include only clinics that the current user has permission to access.  Automatically handles Visibility, based on PrefC.HasClinicsEnabled.  Notice that you have no access to the list of clinics or to the indices, which are instead handled with ClinicNums.</summary>
	public partial class ComboBoxClinicPicker:UserControl {
		#region Fields - Private Constant 
		///<summary>Represents all clinics.  This is done with a dummy clinic added to the top of the list with a ClinicNum of -2.</summary>
		private const long CLINIC_NUM_ALL=-2;
		///<summary>HQ/unassigned/default/none clinic with ClinicNum=0. Sometimes this dummy is filled with info from pref table instead of clinic table.</summary>
		private const long CLINIC_NUM_UNASSIGNED=0;
		#endregion Fields - Private Constant 

		#region Fields - Private
		private bool _doIncludeAll=false;
		private bool _doIncludeUnassigned=false;
		private FormComboPicker _formComboPicker;
		private string _hqDescription="Unassigned";
		private bool _isMultiSelect=false;
		private bool _isTestModeNoDb;
		///<summary>Not exposed as public property.</summary>
		private List<Clinic> _listClinics=new List<Clinic>();
		///<summary>If the SelectedClinicNum gets set to a clinic that's not in the list because the user has no permission, then this is where that info is stored.  If this is null, then it instead behaves normally.</summary>
		private Clinic _selectedClinicNoPermission;
		//<summary>Not exposed as public property. Must stay synched with _listSelectedIndices.</summary>
		private int _selectedIndex=-1;
		///<summary>Not exposed as public property. Must stay synched with _selectedIndex.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		private bool _showLabel=true;
		/// <summary>If this gets changed, then all places where this combo is used must be slightly adjusted.  This is a design weakness of this control, so just don't change it.  Wide enough to handle both "Clinic" and "Clinics".</summary>
		private int _widthLabelArea=35;
		#endregion Fields - Private

		#region Constructor
		public ComboBoxClinicPicker() {
			InitializeComponent();
			Size=new Size(200,21);//same as default
			Name="comboClinic";//default that can be changed
			FillClinics();
		}

		public ComboBoxClinicPicker(bool isTestModeNoDb){
			_isTestModeNoDb=isTestModeNoDb;
			InitializeComponent();
			Size=new Size(200,21);
			Name="comboClinic";
			FillClinics();
		}
		#endregion Constructor

		#region Events - Public Raise
		///<summary></summary>
		private void OnSelectionChangeCommitted(object sender,EventArgs e){
			SelectionChangeCommitted?.Invoke(sender,e);
		}
		[Category("Behavior"),Description("Occurs when user selects a Clinic from the drop-down list.")]
		public event EventHandler SelectionChangeCommitted;

		///<summary></summary>
		private void OnSelectedIndexChanged(object sender,EventArgs e){
			SelectedIndexChanged?.Invoke(sender,e);
		}
		[Category("Behavior"),Description("Try not to use this. The preferred technique is to use SelectionChangeCommitted to react to each user click. In contrast, this event will fire even if the selection programmatically changes.")]
		public event EventHandler SelectedIndexChanged;
		#endregion Events - Public Raise

		#region Events - Private
		private void ComboBoxClinicPicker_Paint(object sender, PaintEventArgs e){
			//the comboBox that shows in the designer is just a fake placeholder.  This control draws its own "combobox".  No hover effects.
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			SolidBrush solidBrushBack=new SolidBrush(Color.FromArgb(225,225,225));
			Pen penOutline=new Pen(Color.FromArgb(173,173,173),1);
			Pen penArrow=new Pen(Color.FromArgb(20,20,20),1.5f);
			Rectangle rectangleCombo=new Rectangle();
			if(ShowLabel){
				rectangleCombo.X=_widthLabelArea;
			}
			else{
				rectangleCombo.X=0;
			}
			rectangleCombo.Y=0;
			rectangleCombo.Width=this.Width-rectangleCombo.X-1;
			rectangleCombo.Height=21-1;//the minus one is so it's not touching the edge of the control and hiding the drawing
			g.FillRectangle(solidBrushBack,rectangleCombo);
			g.DrawRectangle(penOutline,rectangleCombo);//.X,rectangleCombo.Y,rectangleCombo);
			//the down arrow, starting at the left
			g.DrawLine(penArrow,Width-13,9,Width-9.5f,12);
			g.DrawLine(penArrow,Width-9.5f,12,Width-6,9);
			RectangleF rectangleFString=new RectangleF();
			rectangleFString.X=rectangleCombo.X+2;
			rectangleFString.Y=rectangleCombo.Y+4;
			rectangleFString.Width=rectangleCombo.Width-2;
			rectangleFString.Height=rectangleCombo.Height-4;
			int widthMax=rectangleCombo.Width-15;
			g.DrawString(GetDisplayText(widthMax),this.Font,Brushes.Black,rectangleFString);
		}

		private void ComboBoxClinicPicker_MouseDown(object sender, MouseEventArgs e){
			if(ShowLabel && e.X<_widthLabelArea){
				return;
			}
			if(_selectedClinicNoPermission!=null){
				MsgBox.Show("Not allowed");
				return;
			}
			_formComboPicker=new FormComboPicker();
			_formComboPicker.FormClosed += _formComboPicker_FormClosed;
			_formComboPicker.ListStrings=_listClinics.Select(x => x.Abbr).ToList();
			_formComboPicker.PointInitialUR=this.PointToScreen(new Point(this.Width,0));
			if(ShowLabel){
				_formComboPicker.Width=this.Width-_widthLabelArea;
			}
			else{
				_formComboPicker.Width=this.Width;
			}
			//SelectedIndices and SelectedIndex effectively set each other
			if(IsMultiSelect){
				_formComboPicker.IsMultiSelect=true;
				_formComboPicker.SelectedIndices=_listSelectedIndices;
			}
			else{
				_formComboPicker.SelectedIndex=_selectedIndex;
			}
			_formComboPicker.Show();
		}

		private void _formComboPicker_FormClosed(object sender, FormClosedEventArgs e){
			//This is designed to fire whether or not changed. For example, FormPatientAddAll.ComboClinic1 needs to change the others even if it doesn't change.
			//important to set both _selectedIndex and _listSelectedIndices
			_selectedIndex=_formComboPicker.SelectedIndex;
			_listSelectedIndices=_formComboPicker.SelectedIndices;
			OnSelectionChangeCommitted(sender,e);
			OnSelectedIndexChanged(sender,e);
			Invalidate();
		}
		#endregion Events - Private

		#region Properties - Public
		[Category("OD")]
		[Description("Set to true to include 'All' as a selection option. 'All' can sometimes (e.g. FormOperatories) be intended to included more clinics than are actually showing in list.  'All' will still be hidden if PrefName.EnterpriseApptList ==true.")]
		[DefaultValue(false)]
		public bool DoIncludeAll {
			get {
				return _doIncludeAll;
			}
			set {
				//If EnterpriseApptList is on, never allow All to show.
				_doIncludeAll=PrefC.GetBoolSilent(PrefName.EnterpriseApptList,false) ? false : value;
				FillClinics();
			}
		}

		[Category("OD")]
		[Description("Set to true to include 'Unassigned' as a selection option. The word 'Unassigned' can be changed with the HqDescription property.  This is ClinicNum=0.")]
		[DefaultValue(false)]//yes, true might be slightly better default, but then this could not be a drop-in replacement for ComboBoxClinic without causing bugs -- many bugs.
		public bool DoIncludeUnassigned {
			get {
				return _doIncludeUnassigned;
			}
			set {
				_doIncludeUnassigned=value;
				FillClinics();
			}
		}

		[Category("OD")]
		[Description("The display value for ClinicNum 0. Default is 'Unassigned', but might want 'Default', 'HQ', 'None', etc.  Do not specify 'All' here, because that is not accurate.  Only used when 'DoIncludeUnassigned'")]
		[DefaultValue("Unassigned")]
		public string HqDescription {
			get {
				return _hqDescription;
			}
			set {
				_hqDescription=value;
				FillClinics();
			}
		}

		///<summary>True if the special dummy 'All' option is selected. It needs to have been added, first.  The intent of All can vary, and the processing logic would be in the calling form.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAllSelected{
			get{
				return SelectedClinicNum==CLINIC_NUM_ALL;
			}
			set{
				SetSelectedClinicNum(CLINIC_NUM_ALL);//bypasses the Property that would not allow this externally.
			}
		}

		[Category("OD")]
		[Description("")]
		[DefaultValue(false)]
		public bool IsMultiSelect{
			get{
				return _isMultiSelect;
			}
			set{
				if(_isMultiSelect==value){
					return;
				}
				_isMultiSelect=value;
				if(_isMultiSelect){
					labelClinic.Text="Clinics";
				}
			}
		}

		///<summary>True if the 'unassigned'/default/hq/none/all clinic with ClinicNum=0 is selected.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUnassignedSelected {
			get{
				return SelectedClinicNum==CLINIC_NUM_UNASSIGNED;
			}
			set{
				SelectedClinicNum=CLINIC_NUM_UNASSIGNED;
			}
		}

		///<summary>True if SelectedIndex==-1.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsNothingSelected {
			get{
				return _selectedIndex==-1;
			}
			set{
				//not going to check permission here because it's coming from code rather than user.
				_selectedClinicNoPermission=null;
				_selectedIndex=-1;
				_listSelectedIndices=new List<int>();
				OnSelectedIndexChanged(this,new EventArgs());
				Invalidate();
			}
		}

		///<summary>Getter returns -1 if no clinic is selected. The setter is special.  If you set it to a clinicNum that this user does not have permission for, then the combobox displays that and becomes read-only to prevent changing it.  0 (Unassigned) can also be set from here if it's present. This is common if pulling zero from the database.  But if you need to manually set to 0 in other situations, you should use the property IsUnassignedSelected.  You are not allowed to manually set to -2 (All) or -1 (none) from here.  Instead, set IsAllSelected=true or IsNothingSelected=true.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long SelectedClinicNum {
			get {
				if(!DesignMode && !_isTestModeNoDb) {
					if(!PrefC.HasClinicsEnabled) {
						return 0; //when clinics are turned off this control doesn't appear and the 0 clinic is implicitly always selected
					}
				}
				if(_selectedClinicNoPermission!=null){
					return _selectedClinicNoPermission.ClinicNum;
				}
				if(_selectedIndex==-1) {
					return -1;
				}
				return _listClinics[_selectedIndex].ClinicNum;
			}
			set {
				if(value==CLINIC_NUM_ALL){
					throw new ApplicationException("Clinic num cannot be set to -2.  Instead, set IsAllSelected=true.");
				}
				if(value==-1){
					throw new ApplicationException("Clinic num cannot be set to -1.  Instead, set IsNothingSelected=true.");
				}
				SetSelectedClinicNum(value);				
			}
		}

		///<summary>Also can used when IsMultiSelect=false.  In the case where "All" is selected, this will return a list of all clinicNums in the list, which is not technically the same as All clinics in the database, because some clinics might be hidden from this user.  If unassigned(=0) is in the list when All is selected, then it will be included here.  If the calling form wishes to instead test All, and use other logic, it should test IsAllSelected. When setting, this isn't as rigorous as setting SelectedClinicNum.  This property will simply skip setting any clinics that aren't present because of no permission, but it will not disable the control to prevent changes.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<long> ListSelectedClinicNums {
			//This control only internally keeps track of selected indices, so there's no local field for this.
			//The local indices need to be converted to/from ClinicNums.
			get{
				if(!DesignMode && !_isTestModeNoDb) {
					if(!PrefC.HasClinicsEnabled) {
						return new List<long>(); //when clinics are turned off this control doesn't appear and the 0 clinic is implicitly always selected
					}
				}
				List<long> listSelectedClinicNums=new List<long>();
				if(_selectedClinicNoPermission!=null){
					listSelectedClinicNums.Add(_selectedClinicNoPermission.ClinicNum);
					return listSelectedClinicNums;
				}
				if(_listSelectedIndices.Count==0) {
					return listSelectedClinicNums;
				}
				if(_doIncludeAll && _listSelectedIndices.Contains(0)){
					//The "All" item was selected
					foreach(Clinic clinic in _listClinics){
						if(clinic.ClinicNum==CLINIC_NUM_ALL){
							continue;
						}
						//seems to include the "unassigned/default/hq/none/all" clinicNum=0, if present
						listSelectedClinicNums.Add(clinic.ClinicNum);
					}
					return listSelectedClinicNums;
				}
				foreach(int idx in _listSelectedIndices){
					listSelectedClinicNums.Add(_listClinics[idx].ClinicNum);
				}
				return listSelectedClinicNums;
			}
			set{
				_selectedClinicNoPermission=null;
				_listSelectedIndices=new List<int>();
				for(int i=0;i<_listClinics.Count;i++) {
					if(value.Contains(_listClinics[i].ClinicNum)) {
						_selectedIndex=i;//this only works when there is one, but it's still important to try to stay synched
						_listSelectedIndices.Add(i);
					}
				}
				OnSelectedIndexChanged(this,new EventArgs());
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Normally true to show label on left.  Set to false if not enough space and you want to do your own separate label.  Make sure to manually set visibility of that label, based on whether Clinic feature is turned on.")]
		[DefaultValue(true)]
		public bool ShowLabel {
			get {
				return _showLabel;
			}
			set {
				if(_showLabel==value){
					return;
				}
				_showLabel=value; 
				if(_showLabel){
					labelClinic.Visible=true;
				}
				else{
					labelClinic.Visible=false;
				}
				Invalidate();
			}
		}

		#endregion Properties - Public

		#region Properties - Protected
		protected override Size DefaultSize{
			get{
				return new Size(200,21);
			}
		}

		protected override void OnHandleDestroyed(EventArgs e){
			base.OnHandleDestroyed(e);
			if(_formComboPicker!=null && !_formComboPicker.IsDisposed){
				_formComboPicker.Close();
				_formComboPicker.Dispose();
			}
		}

		protected override void OnLoad(EventArgs e){
			base.OnLoad(e);
			if(!DesignMode && !_isTestModeNoDb) {
				Visible=PrefC.HasClinicsEnabled;
			}		
		}
		#endregion Properties - Protected
		
		#region Methods - Public
		///<summary>Returns empty string if no clinic is selected.  Can return "All" or the "Unassigned" override abbreviation.</summary>
		public string GetSelectedAbbr() {
			if(_selectedIndex==-1) {
				return "";
			}
			return _listClinics[_selectedIndex].Abbr;
		}

		///<summary>Returns null if no clinic selected.</summary>
		public Clinic GetSelectedClinic() {
			if(_selectedIndex==-1) {
				return null;
			}
			return _listClinics[_selectedIndex];
		}

		///<summary>Gets a string of all selected clinic Abbr's, separated by commas.</summary>
		public string GetStringSelectedClinics() {
			List<Clinic> listSelectedClinics=_listSelectedIndices.Select(x => _listClinics[x]).ToList();
			if(listSelectedClinics.Any(x => x.ClinicNum==CLINIC_NUM_ALL)) {
				return "All Clinics";
			}
			return string.Join(",",listSelectedClinics.Select(clinic => clinic.Abbr));
		}
		#endregion Methods - Public

		#region Methods - Private
		///<summary>This runs on initialization and with each property change.  Performance hit should be very small. This also does an Invalidate so that the "combobox" will update.</summary>
		private void FillClinics() {
			if(!_isTestModeNoDb){
				if(!Db.HasDatabaseConnection && !Security.IsUserLoggedIn) {
					return;
				}
			}
			try {
				if(!_isTestModeNoDb){
					if(!PrefC.HasClinicsEnabled) {//Already not visible after load handler.
						return;
					}
				}
				_listClinics.Clear();
				if(DoIncludeAll) {//Comes first
					_listClinics.Add(new Clinic {
						Abbr="All",
						Description="All",
						ClinicNum=CLINIC_NUM_ALL
					});
				}
				//Does not  guarantee that HQ clinic will be included. Only if user has permission to view it.
				List<Clinic> listClinicsForUser=null;
				if(_isTestModeNoDb){
					listClinicsForUser=new List<Clinic>();
					listClinicsForUser.Add(new Clinic{Abbr="Clinic1",Description="Clinic1",ClinicNum=1 });
					listClinicsForUser.Add(new Clinic{Abbr="Clinic2",Description="Clinic2",ClinicNum=2 });
					listClinicsForUser.Add(new Clinic{Abbr="Clinic3",Description="Clinic3",ClinicNum=3 });
					if(DoIncludeAll){
						listClinicsForUser.Add(new Clinic{Abbr=HqDescription,Description=HqDescription,ClinicNum=0 });
					}
				}
				else{
					listClinicsForUser=Clinics.GetForUserod(Security.CurUser,true,HqDescription);
				}
				//Add HQ clinic when necessary.
				Clinic clinicUnassigned=listClinicsForUser.Find(x => x.ClinicNum==CLINIC_NUM_UNASSIGNED);
				//unassigned is next
				if(DoIncludeUnassigned  && clinicUnassigned!=null) {
					_listClinics.Add(clinicUnassigned);
				}
				//then, the other items, except unassigned
				listClinicsForUser.RemoveAll(x => x.ClinicNum==CLINIC_NUM_UNASSIGNED);
				_listClinics.AddRange(listClinicsForUser);
				//Will already be ordered alphabetically if that pref was set.  Unfortunately, a restart is required for that pref.
				//Setting selected---------------------------------------------------------------------------------------------------------------
				if(Clinics.ClinicNum==0) {
					if(DoIncludeUnassigned) {
						SelectedClinicNum=CLINIC_NUM_UNASSIGNED;
					}
					else if(DoIncludeAll) {
						SetSelectedClinicNum(CLINIC_NUM_ALL);
					}
				}
				else {
					//if Security.CurUser.ClinicIsRestricted, there will be only one clinic in the list, and it will not include default (0).
					SelectedClinicNum=Clinics.ClinicNum;
				}
			}
			catch(Exception e){
				e.DoNothing();
			}
			Invalidate();
		}

		///<summary>If multiple items are selected, we string them together with commas.  But if the string is wider than widthMax, we instead show "Multiple".</summary>
		private string GetDisplayText(int widthMax){
			if(_listSelectedIndices.Count==0){
				return "";
			}
			if(_listSelectedIndices.Contains(0) && _listClinics[0].ClinicNum==CLINIC_NUM_ALL){
				return "All";
			}
			if(_selectedClinicNoPermission!=null){
				return _selectedClinicNoPermission.Abbr;
			}
			string str="";
			for(int i=0;i<_listSelectedIndices.Count;i++){
				//impossible: if(_listClinics[_listSelectedIndices[i]].ClinicNum==CLINIC_NUM_ALL){
				if(i>0){
					str+=",";
				}
				str+=_listClinics[_listSelectedIndices[i]].Abbr;//automatically handles CLINIC_NUM_UNASSIGNED
			}
			if(_listSelectedIndices.Count>1){
				if(TextRenderer.MeasureText(str,this.Font).Width>widthMax){
					return "Multiple";
				}
			}
			return str;
		}

		///<summary>This is used separately from the SelectedClinicNum setter in order to internally set to -2 without an exception.  But this doesn't work for -1.</summary>
		private void SetSelectedClinicNum(long value){
			int idx=_listClinics.FindIndex(x=>x.ClinicNum==value);
			if(idx==-1){
				//this user does not have permission for the selected clinic
				//so _selectedIndex and _listSelectedIndices are completely meaningless
				if(_isTestModeNoDb){
					_selectedClinicNoPermission=new Clinic{Abbr=value.ToString(), Description=value.ToString(),ClinicNum=value };
				}
				else{
					_selectedClinicNoPermission=Clinics.GetClinic(value);
					//could still possibly be null in a corrupted db.  In that case, we still need to be able to return what was set.
					if(_selectedClinicNoPermission==null){
						_selectedClinicNoPermission=new Clinic{Abbr=value.ToString(), Description=value.ToString(),ClinicNum=value };
					}
				}
			}
			else{
				_selectedClinicNoPermission=null;
				_selectedIndex=idx;
				_listSelectedIndices=new List<int>();
				_listSelectedIndices.Add(idx);
			}
			OnSelectedIndexChanged(this,new EventArgs());
			Invalidate();
		}
		#endregion Methods - Private

		
	}
}




