using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental{
	//todo lindsay: put designer code into a separate file
///<summary></summary>
	public class FormRpHiddenPaySplits : ODForm {
		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.ODDateRangePicker odDateRangePicker;
		private CheckBox checkAllProv;
		private ListBox listBoxProv;
		private Label label1;
		private ListBox listBoxClinic;
		private Label labelClinic;
		private CheckBox checkAllClinics;
		private CheckBox checkAllUnearnedTypes;
		private ListBox listBoxUnearnedTypes;
		private Label label3;
		private System.ComponentModel.Container components = null;
		private List<Provider> _listProviders;
		private List<Clinic> _listClinics;
		private List<Def> _listUnearnedTypes;

		///<summary></summary>
		public FormRpHiddenPaySplits(){
			InitializeComponent();
 			Lan.F(this);
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpHiddenPaySplits));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.odDateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.listBoxProv = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxClinic = new System.Windows.Forms.ListBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.checkAllClinics = new System.Windows.Forms.CheckBox();
			this.checkAllUnearnedTypes = new System.Windows.Forms.CheckBox();
			this.listBoxUnearnedTypes = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Autosize = true;
			this.butOK.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butOK.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butOK.CornerRadius = 4F;
			this.butOK.Location = new System.Drawing.Point(354, 298);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.ButOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Autosize = true;
			this.butCancel.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCancel.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCancel.CornerRadius = 4F;
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(435, 298);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			// 
			// odDateRangePicker
			// 
			this.odDateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.odDateRangePicker.DefaultDateTimeFrom = new System.DateTime(2019, 6, 17, 0, 0, 0, 0);
			this.odDateRangePicker.DefaultDateTimeTo = new System.DateTime(2019, 6, 21, 0, 0, 0, 0);
			this.odDateRangePicker.Location = new System.Drawing.Point(12, 12);
			this.odDateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.odDateRangePicker.Name = "odDateRangePicker";
			this.odDateRangePicker.Size = new System.Drawing.Size(453, 24);
			this.odDateRangePicker.TabIndex = 6;
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(13, 74);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(40, 16);
			this.checkAllProv.TabIndex = 62;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.CheckAllProv_Click);
			// 
			// listBoxProv
			// 
			this.listBoxProv.Location = new System.Drawing.Point(12, 93);
			this.listBoxProv.Name = "listBoxProv";
			this.listBoxProv.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxProv.Size = new System.Drawing.Size(160, 199);
			this.listBoxProv.TabIndex = 61;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 54);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 60;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxClinic
			// 
			this.listBoxClinic.Location = new System.Drawing.Point(347, 93);
			this.listBoxClinic.Name = "listBoxClinic";
			this.listBoxClinic.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxClinic.Size = new System.Drawing.Size(160, 199);
			this.listBoxClinic.TabIndex = 53;
			this.listBoxClinic.Visible = false;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(344, 56);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(104, 16);
			this.labelClinic.TabIndex = 52;
			this.labelClinic.Text = "Clinics";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelClinic.Visible = false;
			// 
			// checkAllClinics
			// 
			this.checkAllClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClinics.Location = new System.Drawing.Point(347, 74);
			this.checkAllClinics.Name = "checkAllClinics";
			this.checkAllClinics.Size = new System.Drawing.Size(95, 16);
			this.checkAllClinics.TabIndex = 54;
			this.checkAllClinics.Text = "All";
			this.checkAllClinics.Visible = false;
			this.checkAllClinics.Click += new System.EventHandler(this.CheckAllClinics_Click);
			// 
			// checkAllUnearnedTypes
			// 
			this.checkAllUnearnedTypes.Checked = true;
			this.checkAllUnearnedTypes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllUnearnedTypes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllUnearnedTypes.Location = new System.Drawing.Point(178, 73);
			this.checkAllUnearnedTypes.Name = "checkAllUnearnedTypes";
			this.checkAllUnearnedTypes.Size = new System.Drawing.Size(154, 17);
			this.checkAllUnearnedTypes.TabIndex = 64;
			this.checkAllUnearnedTypes.Text = "All";
			this.checkAllUnearnedTypes.Click += new System.EventHandler(this.CheckAllUnearnedTypes_Click);
			// 
			// listBoxUnearnedTypes
			// 
			this.listBoxUnearnedTypes.Location = new System.Drawing.Point(178, 93);
			this.listBoxUnearnedTypes.Name = "listBoxUnearnedTypes";
			this.listBoxUnearnedTypes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxUnearnedTypes.Size = new System.Drawing.Size(163, 199);
			this.listBoxUnearnedTypes.TabIndex = 63;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(175, 54);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 65;
			this.label3.Text = "Unearned Types";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRpTpPreAllocation
			// 
			this.ClientSize = new System.Drawing.Size(522, 337);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkAllUnearnedTypes);
			this.Controls.Add(this.listBoxUnearnedTypes);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listBoxProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkAllClinics);
			this.Controls.Add(this.odDateRangePicker);
			this.Controls.Add(this.listBoxClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpTpPreAllocation";
			this.Text = "Payments not Shown on Account";
			this.Load += new System.EventHandler(this.FormRpTpPreAllocation_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private void FormRpTpPreAllocation_Load(object sender,EventArgs e) {
			odDateRangePicker.SetDateTimeFrom(DateTime.Today.AddMonths(-1));
			odDateRangePicker.SetDateTimeTo(DateTime.Today);
			if(PrefC.HasClinicsEnabled) {
				labelClinic.Visible=true;
				checkAllClinics.Visible=true;
				checkAllClinics.Checked=true;
				listBoxClinic.Visible=true;
				listBoxClinic.SelectedIndices.Clear();
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				if(!Security.CurUser.ClinicIsRestricted) {
					listBoxClinic.Items.Add(new ODBoxItem<Clinic>("Unassigned",new Clinic{ClinicNum=0}));
				}
				foreach(Clinic clinic in _listClinics) {
					ODBoxItem<Clinic> boxClinic=new ODBoxItem<Clinic>(clinic.Abbr,clinic);
					listBoxClinic.Items.Add(boxClinic);
				}
			}
			else {
				_listClinics=new List<Clinic>();
			}
			_listProviders=Providers.GetListReports();
			_listProviders.Insert(0,Providers.GetUnearnedProv());
			checkAllProv.Checked=true;
			listBoxProv.SetItems(_listProviders,x => x.Abbr);
			_listUnearnedTypes=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).Where(x => !string.IsNullOrEmpty(x.ItemValue)).ToList();
			checkAllUnearnedTypes.Checked=true;
			listBoxUnearnedTypes.SetItems(_listUnearnedTypes,x => x.ItemName);
		}

		private void CheckAllProv_Click(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
				listBoxProv.SelectedIndices.Clear();
			}
		}

		private void CheckAllUnearnedTypes_Click(object sender,EventArgs e) {
			if(checkAllUnearnedTypes.Checked) {
				listBoxUnearnedTypes.SelectedIndices.Clear();
			}
		}

		private void CheckAllClinics_Click(object sender,EventArgs e) {
			if(checkAllClinics.Checked) {
				listBoxClinic.SelectedIndices.Clear();
			}
		}

		private bool IsValid() {
			if(!checkAllProv.Checked && listBoxProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return false;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClinics.Checked && listBoxClinic.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return false;
			}
			if(!checkAllUnearnedTypes.Checked && listBoxUnearnedTypes.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one unearned type must be selected.");
				return false;
			}
			return true;
		}

		private void ButOK_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			ReportComplex report=new ReportComplex(true,false);
			List<long> listProvNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			List<long> listUnearnedTypeDefNums=new List<long>();
			string subtitleProvs="";
			string subtitleClinics="";
			string subtitleUnearned="";
			if(checkAllProv.Checked) {
				subtitleProvs="All Providers";
				listProvNums=_listProviders.Select(x => x.ProvNum).ToList();
			}
			else {
				subtitleProvs=string.Join(", ",listBoxProv.SelectedTags<Provider>().Select(x => x.Abbr));
				listProvNums=listBoxProv.SelectedTags<Provider>().Select(x => x.ProvNum).ToList();
			}
			if(checkAllUnearnedTypes.Checked) {
				subtitleUnearned="All Hidden Unearned";
				listUnearnedTypeDefNums=_listUnearnedTypes.Select(x => x.DefNum).ToList();
			}
			else {
				subtitleUnearned=string.Join(", ",listBoxUnearnedTypes.SelectedTags<Def>().Select(x => x.ItemName));
				listUnearnedTypeDefNums=listBoxUnearnedTypes.SelectedTags<Def>().Select(x => x.DefNum).ToList();
			}
			if(PrefC.HasClinicsEnabled && checkAllClinics.Checked) {
				subtitleClinics="All Clinics";
				listClinicNums=_listClinics.Select(x => x.ClinicNum).ToList();
			}
			else if(PrefC.HasClinicsEnabled && !checkAllClinics.Checked) {
				subtitleClinics=string.Join(", ",listBoxClinic.SelectedTags<Clinic>().Select(x => x.Abbr).ToList());
				listClinicNums=listBoxClinic.SelectedTags<Clinic>().Select(x => x.ClinicNum).ToList();
			}
			DataTable table=new DataTable();
			table=RpHiddenPaySplits.GetReportData(listProvNums,listUnearnedTypeDefNums,listClinicNums);
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Hidden Payment Splits");
			report.AddTitle("Title",Lan.g(this,"Hidden Payment Splits"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			report.AddSubTitle("UnearnedTypes",subtitleUnearned,fontSubTitle);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			}
			QueryObject query=report.AddQuery(table,"Hidden PaySplits");
			query.AddColumn("Date",90,FieldValueType.Date,font);
			query.AddColumn("Patient",140,FieldValueType.String,font);
			query.AddColumn("Provider",90,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn("Clinic",50,FieldValueType.String,font);
			}
			query.AddColumn("Code",65,FieldValueType.String,font);
			query.AddColumn("Description",220,FieldValueType.String,font);
			query.AddColumn("Amount",80,FieldValueType.Number,font);
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			FormReportComplex formComplex=new FormReportComplex(report);
			formComplex.ShowDialog();
			DialogResult=DialogResult.OK;
		}
	}
}
