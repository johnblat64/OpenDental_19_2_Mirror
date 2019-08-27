namespace OpenDental {
	partial class FormRecallList {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecallList));
			this.butClose = new OpenDental.UI.Button();
			this.menuRightClick = new System.Windows.Forms.ContextMenu();
			this.menuItemSeeFamily = new System.Windows.Forms.MenuItem();
			this.menuItemSeeAccount = new System.Windows.Forms.MenuItem();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageRecalls = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboEmailFrom = new System.Windows.Forms.ComboBox();
			this.panelWebSched = new System.Windows.Forms.Panel();
			this.butUndo = new OpenDental.UI.Button();
			this.butGotoFamily = new OpenDental.UI.Button();
			this.butCommlog = new OpenDental.UI.Button();
			this.butGotoAccount = new OpenDental.UI.Button();
			this.butSchedFam = new OpenDental.UI.Button();
			this.butLabelOne = new OpenDental.UI.Button();
			this.butSchedPat = new OpenDental.UI.Button();
			this.labelPatientCount = new System.Windows.Forms.Label();
			this.butECards = new OpenDental.UI.Button();
			this.butEmail = new OpenDental.UI.Button();
			this.butPostcards = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.ODGrid();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.butSetStatus = new OpenDental.UI.Button();
			this.butLabels = new OpenDental.UI.Button();
			this.butReport = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkShowReminded = new System.Windows.Forms.CheckBox();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.checkConflictingTypes = new System.Windows.Forms.CheckBox();
			this.comboNumberReminders = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboSort = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboSite = new System.Windows.Forms.ComboBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.label4 = new System.Windows.Forms.Label();
			this.checkGroupFamilies = new System.Windows.Forms.CheckBox();
			this.textDateEnd = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.tabPageRecentlyContacted = new System.Windows.Forms.TabPage();
			this.gridRecentlyContacted = new OpenDental.UI.ODGrid();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.butRefreshRecent = new OpenDental.UI.Button();
			this.comboClinicRecent = new OpenDental.UI.ComboBoxClinicPicker();
			this.datePickerRecent = new OpenDental.UI.ODDateRangePicker();
			this.tabPageReactivations = new System.Windows.Forms.TabPage();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.comboReactEmailFrom = new System.Windows.Forms.ComboBox();
			this.butReactGoToFam = new OpenDental.UI.Button();
			this.butReactComm = new OpenDental.UI.Button();
			this.butReactGoToAcct = new OpenDental.UI.Button();
			this.butReactSchedFam = new OpenDental.UI.Button();
			this.butReactSingleLabels = new OpenDental.UI.Button();
			this.butReactSchedPat = new OpenDental.UI.Button();
			this.butReactEmail = new OpenDental.UI.Button();
			this.butReactPostcardPreview = new OpenDental.UI.Button();
			this.butReactPrint = new OpenDental.UI.Button();
			this.butReactLabelPreview = new OpenDental.UI.Button();
			this.labelReactPatCount = new System.Windows.Forms.Label();
			this.gridReactivations = new OpenDental.UI.ODGrid();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.comboReactStatus = new System.Windows.Forms.ComboBox();
			this.butReactSetStatus = new OpenDental.UI.Button();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.checkReactGroupFamilies = new System.Windows.Forms.CheckBox();
			this.comboReactProv = new System.Windows.Forms.ComboBox();
			this.checkReactShowDNC = new System.Windows.Forms.CheckBox();
			this.comboBillingTypes = new System.Windows.Forms.ComboBox();
			this.labelBillingType = new System.Windows.Forms.Label();
			this.comboShowReactivate = new System.Windows.Forms.ComboBox();
			this.labelShowReactivate = new System.Windows.Forms.Label();
			this.comboReactSortBy = new System.Windows.Forms.ComboBox();
			this.labelReactSortBy = new System.Windows.Forms.Label();
			this.comboReactSite = new System.Windows.Forms.ComboBox();
			this.labelReactSite = new System.Windows.Forms.Label();
			this.comboReactClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelReactProv = new System.Windows.Forms.Label();
			this.validDateSince = new OpenDental.ValidDate();
			this.labelDateSince = new System.Windows.Forms.Label();
			this.butReactRefresh = new OpenDental.UI.Button();
			this.tabControl.SuspendLayout();
			this.tabPageRecalls.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPageRecentlyContacted.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.tabPageReactivations.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.CornerRadius = 4F;
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(906, 733);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// menuRightClick
			// 
			this.menuRightClick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSeeFamily,
            this.menuItemSeeAccount});
			// 
			// menuItemSeeFamily
			// 
			this.menuItemSeeFamily.Index = 0;
			this.menuItemSeeFamily.Text = "See Family";
			this.menuItemSeeFamily.Click += new System.EventHandler(this.butGotoFamily_Click);
			// 
			// menuItemSeeAccount
			// 
			this.menuItemSeeAccount.Index = 1;
			this.menuItemSeeAccount.Text = "See Account";
			this.menuItemSeeAccount.Click += new System.EventHandler(this.butGotoAccount_Click);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPageRecalls);
			this.tabControl.Controls.Add(this.tabPageRecentlyContacted);
			this.tabControl.Controls.Add(this.tabPageReactivations);
			this.tabControl.Location = new System.Drawing.Point(2, 3);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(979, 725);
			this.tabControl.TabIndex = 3;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tabPageRecalls
			// 
			this.tabPageRecalls.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageRecalls.Controls.Add(this.groupBox2);
			this.tabPageRecalls.Controls.Add(this.panelWebSched);
			this.tabPageRecalls.Controls.Add(this.butUndo);
			this.tabPageRecalls.Controls.Add(this.butGotoFamily);
			this.tabPageRecalls.Controls.Add(this.butCommlog);
			this.tabPageRecalls.Controls.Add(this.butGotoAccount);
			this.tabPageRecalls.Controls.Add(this.butSchedFam);
			this.tabPageRecalls.Controls.Add(this.butLabelOne);
			this.tabPageRecalls.Controls.Add(this.butSchedPat);
			this.tabPageRecalls.Controls.Add(this.labelPatientCount);
			this.tabPageRecalls.Controls.Add(this.butECards);
			this.tabPageRecalls.Controls.Add(this.butEmail);
			this.tabPageRecalls.Controls.Add(this.butPostcards);
			this.tabPageRecalls.Controls.Add(this.butPrint);
			this.tabPageRecalls.Controls.Add(this.gridMain);
			this.tabPageRecalls.Controls.Add(this.groupBox3);
			this.tabPageRecalls.Controls.Add(this.butLabels);
			this.tabPageRecalls.Controls.Add(this.butReport);
			this.tabPageRecalls.Controls.Add(this.groupBox1);
			this.tabPageRecalls.Location = new System.Drawing.Point(4, 22);
			this.tabPageRecalls.Name = "tabPageRecalls";
			this.tabPageRecalls.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRecalls.Size = new System.Drawing.Size(971, 699);
			this.tabPageRecalls.TabIndex = 0;
			this.tabPageRecalls.Tag = this.gridMain;
			this.tabPageRecalls.Text = "Recalls";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboEmailFrom);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(704, 58);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(263, 46);
			this.groupBox2.TabIndex = 125;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Email From";
			// 
			// comboEmailFrom
			// 
			this.comboEmailFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEmailFrom.Location = new System.Drawing.Point(17, 17);
			this.comboEmailFrom.MaxDropDownItems = 40;
			this.comboEmailFrom.Name = "comboEmailFrom";
			this.comboEmailFrom.Size = new System.Drawing.Size(233, 21);
			this.comboEmailFrom.TabIndex = 65;
			// 
			// panelWebSched
			// 
			this.panelWebSched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panelWebSched.BackgroundImage = global::OpenDental.Properties.Resources.webSched_PV_Button;
			this.panelWebSched.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.panelWebSched.Location = new System.Drawing.Point(759, 668);
			this.panelWebSched.Name = "panelWebSched";
			this.panelWebSched.Size = new System.Drawing.Size(120, 24);
			this.panelWebSched.TabIndex = 138;
			this.panelWebSched.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelWebSched_MouseClick);
			// 
			// butUndo
			// 
			this.butUndo.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUndo.Autosize = true;
			this.butUndo.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butUndo.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butUndo.CornerRadius = 4F;
			this.butUndo.Location = new System.Drawing.Point(3, 668);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(119, 24);
			this.butUndo.TabIndex = 137;
			this.butUndo.Text = "Undo";
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			// 
			// butGotoFamily
			// 
			this.butGotoFamily.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butGotoFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGotoFamily.Autosize = true;
			this.butGotoFamily.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butGotoFamily.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butGotoFamily.CornerRadius = 4F;
			this.butGotoFamily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGotoFamily.Location = new System.Drawing.Point(443, 642);
			this.butGotoFamily.Name = "butGotoFamily";
			this.butGotoFamily.Size = new System.Drawing.Size(96, 24);
			this.butGotoFamily.TabIndex = 136;
			this.butGotoFamily.Text = "Go to Family";
			this.butGotoFamily.Click += new System.EventHandler(this.butGotoFamily_Click);
			// 
			// butCommlog
			// 
			this.butCommlog.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCommlog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCommlog.Autosize = true;
			this.butCommlog.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCommlog.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCommlog.CornerRadius = 4F;
			this.butCommlog.Image = global::OpenDental.Properties.Resources.commlog;
			this.butCommlog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCommlog.Location = new System.Drawing.Point(545, 668);
			this.butCommlog.Name = "butCommlog";
			this.butCommlog.Size = new System.Drawing.Size(88, 24);
			this.butCommlog.TabIndex = 135;
			this.butCommlog.Text = "Comm";
			this.butCommlog.Click += new System.EventHandler(this.butCommlog_Click);
			// 
			// butGotoAccount
			// 
			this.butGotoAccount.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butGotoAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGotoAccount.Autosize = true;
			this.butGotoAccount.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butGotoAccount.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butGotoAccount.CornerRadius = 4F;
			this.butGotoAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGotoAccount.Location = new System.Drawing.Point(443, 668);
			this.butGotoAccount.Name = "butGotoAccount";
			this.butGotoAccount.Size = new System.Drawing.Size(96, 24);
			this.butGotoAccount.TabIndex = 134;
			this.butGotoAccount.Text = "Go to Account";
			this.butGotoAccount.Click += new System.EventHandler(this.butGotoAccount_Click);
			// 
			// butSchedFam
			// 
			this.butSchedFam.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butSchedFam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSchedFam.Autosize = true;
			this.butSchedFam.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butSchedFam.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butSchedFam.CornerRadius = 4F;
			this.butSchedFam.Image = global::OpenDental.Properties.Resources.butPin;
			this.butSchedFam.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSchedFam.Location = new System.Drawing.Point(639, 668);
			this.butSchedFam.Name = "butSchedFam";
			this.butSchedFam.Size = new System.Drawing.Size(114, 24);
			this.butSchedFam.TabIndex = 129;
			this.butSchedFam.Tag = "SchedFamRecall";
			this.butSchedFam.Text = "Sched Family";
			this.butSchedFam.Click += new System.EventHandler(this.butSched_Click);
			// 
			// butLabelOne
			// 
			this.butLabelOne.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butLabelOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabelOne.Autosize = true;
			this.butLabelOne.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butLabelOne.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butLabelOne.CornerRadius = 4F;
			this.butLabelOne.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabelOne.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabelOne.Location = new System.Drawing.Point(128, 642);
			this.butLabelOne.Name = "butLabelOne";
			this.butLabelOne.Size = new System.Drawing.Size(119, 24);
			this.butLabelOne.TabIndex = 133;
			this.butLabelOne.Text = "Single Labels";
			this.butLabelOne.Click += new System.EventHandler(this.butLabelOne_Click);
			// 
			// butSchedPat
			// 
			this.butSchedPat.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butSchedPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSchedPat.Autosize = true;
			this.butSchedPat.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butSchedPat.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butSchedPat.CornerRadius = 4F;
			this.butSchedPat.Image = global::OpenDental.Properties.Resources.butPin;
			this.butSchedPat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSchedPat.Location = new System.Drawing.Point(639, 642);
			this.butSchedPat.Name = "butSchedPat";
			this.butSchedPat.Size = new System.Drawing.Size(114, 24);
			this.butSchedPat.TabIndex = 128;
			this.butSchedPat.Tag = "SchedPatRecall";
			this.butSchedPat.Text = "Sched Patient";
			this.butSchedPat.Click += new System.EventHandler(this.butSched_Click);
			// 
			// labelPatientCount
			// 
			this.labelPatientCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPatientCount.Location = new System.Drawing.Point(854, 640);
			this.labelPatientCount.Name = "labelPatientCount";
			this.labelPatientCount.Size = new System.Drawing.Size(114, 14);
			this.labelPatientCount.TabIndex = 132;
			this.labelPatientCount.Text = "Patient Count:";
			this.labelPatientCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butECards
			// 
			this.butECards.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butECards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butECards.Autosize = true;
			this.butECards.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butECards.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butECards.CornerRadius = 4F;
			this.butECards.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butECards.Location = new System.Drawing.Point(253, 642);
			this.butECards.Name = "butECards";
			this.butECards.Size = new System.Drawing.Size(91, 24);
			this.butECards.TabIndex = 130;
			this.butECards.Text = "eCards";
			this.butECards.Visible = false;
			this.butECards.Click += new System.EventHandler(this.butECards_Click);
			// 
			// butEmail
			// 
			this.butEmail.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEmail.Autosize = true;
			this.butEmail.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butEmail.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butEmail.CornerRadius = 4F;
			this.butEmail.Image = global::OpenDental.Properties.Resources.email1;
			this.butEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEmail.Location = new System.Drawing.Point(253, 668);
			this.butEmail.Name = "butEmail";
			this.butEmail.Size = new System.Drawing.Size(91, 24);
			this.butEmail.TabIndex = 131;
			this.butEmail.Text = "E-Mail";
			this.butEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butPostcards
			// 
			this.butPostcards.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPostcards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPostcards.Autosize = true;
			this.butPostcards.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPostcards.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPostcards.CornerRadius = 4F;
			this.butPostcards.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPostcards.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPostcards.Location = new System.Drawing.Point(3, 642);
			this.butPostcards.Name = "butPostcards";
			this.butPostcards.Size = new System.Drawing.Size(119, 24);
			this.butPostcards.TabIndex = 124;
			this.butPostcards.Text = "Postcard Preview";
			this.butPostcards.Click += new System.EventHandler(this.butPostcards_Click);
			// 
			// butPrint
			// 
			this.butPrint.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Autosize = true;
			this.butPrint.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPrint.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPrint.CornerRadius = 4F;
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(350, 668);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 127;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridMain.HasDropDowns = false;
			this.gridMain.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridMain.HeaderHeight = 15;
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(3, 110);
			this.gridMain.Name = "gridMain";
			this.gridMain.ScrollValue = 0;
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(965, 527);
			this.gridMain.TabIndex = 126;
			this.gridMain.Title = "Recall List";
			this.gridMain.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridMain.TitleHeight = 18;
			this.gridMain.TranslationName = "TableRecallList";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.comboStatus);
			this.groupBox3.Controls.Add(this.butSetStatus);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox3.Location = new System.Drawing.Point(704, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(263, 46);
			this.groupBox3.TabIndex = 123;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Set Status";
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.Location = new System.Drawing.Point(17, 17);
			this.comboStatus.MaxDropDownItems = 40;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(160, 21);
			this.comboStatus.TabIndex = 15;
			// 
			// butSetStatus
			// 
			this.butSetStatus.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butSetStatus.Autosize = true;
			this.butSetStatus.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butSetStatus.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butSetStatus.CornerRadius = 4F;
			this.butSetStatus.Location = new System.Drawing.Point(183, 14);
			this.butSetStatus.Name = "butSetStatus";
			this.butSetStatus.Size = new System.Drawing.Size(67, 24);
			this.butSetStatus.TabIndex = 14;
			this.butSetStatus.Text = "Set";
			this.butSetStatus.Click += new System.EventHandler(this.butSetStatus_Click);
			// 
			// butLabels
			// 
			this.butLabels.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabels.Autosize = true;
			this.butLabels.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butLabels.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butLabels.CornerRadius = 4F;
			this.butLabels.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabels.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabels.Location = new System.Drawing.Point(128, 668);
			this.butLabels.Name = "butLabels";
			this.butLabels.Size = new System.Drawing.Size(119, 24);
			this.butLabels.TabIndex = 122;
			this.butLabels.Text = "Label Preview";
			this.butLabels.Click += new System.EventHandler(this.butLabels_Click);
			// 
			// butReport
			// 
			this.butReport.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReport.Autosize = true;
			this.butReport.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReport.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReport.CornerRadius = 4F;
			this.butReport.Location = new System.Drawing.Point(350, 642);
			this.butReport.Name = "butReport";
			this.butReport.Size = new System.Drawing.Size(87, 24);
			this.butReport.TabIndex = 121;
			this.butReport.Text = "R&un Report";
			this.butReport.Click += new System.EventHandler(this.butReport_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkShowReminded);
			this.groupBox1.Controls.Add(this.comboProv);
			this.groupBox1.Controls.Add(this.checkConflictingTypes);
			this.groupBox1.Controls.Add(this.comboNumberReminders);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.comboSort);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.comboSite);
			this.groupBox1.Controls.Add(this.labelSite);
			this.groupBox1.Controls.Add(this.comboClinic);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.checkGroupFamilies);
			this.groupBox1.Controls.Add(this.textDateEnd);
			this.groupBox1.Controls.Add(this.textDateStart);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.butRefresh);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(3, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(695, 98);
			this.groupBox1.TabIndex = 120;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "View";
			// 
			// checkShowReminded
			// 
			this.checkShowReminded.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowReminded.Location = new System.Drawing.Point(26, 48);
			this.checkShowReminded.Name = "checkShowReminded";
			this.checkShowReminded.Size = new System.Drawing.Size(159, 18);
			this.checkShowReminded.TabIndex = 42;
			this.checkShowReminded.Text = "Include Reminded";
			this.checkShowReminded.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowReminded.UseVisualStyleBackColor = true;
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.FormattingEnabled = true;
			this.comboProv.Location = new System.Drawing.Point(437, 16);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(160, 21);
			this.comboProv.TabIndex = 41;
			// 
			// checkConflictingTypes
			// 
			this.checkConflictingTypes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkConflictingTypes.Location = new System.Drawing.Point(26, 30);
			this.checkConflictingTypes.Name = "checkConflictingTypes";
			this.checkConflictingTypes.Size = new System.Drawing.Size(159, 18);
			this.checkConflictingTypes.TabIndex = 40;
			this.checkConflictingTypes.Text = "Show Conflicting Types";
			this.checkConflictingTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkConflictingTypes.UseVisualStyleBackColor = true;
			this.checkConflictingTypes.Click += new System.EventHandler(this.checkRecallTypes_Click);
			// 
			// comboNumberReminders
			// 
			this.comboNumberReminders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboNumberReminders.Location = new System.Drawing.Point(103, 68);
			this.comboNumberReminders.MaxDropDownItems = 40;
			this.comboNumberReminders.Name = "comboNumberReminders";
			this.comboNumberReminders.Size = new System.Drawing.Size(82, 21);
			this.comboNumberReminders.TabIndex = 39;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 71);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 14);
			this.label3.TabIndex = 38;
			this.label3.Text = "Show Reminders";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSort
			// 
			this.comboSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSort.Location = new System.Drawing.Point(242, 67);
			this.comboSort.MaxDropDownItems = 40;
			this.comboSort.Name = "comboSort";
			this.comboSort.Size = new System.Drawing.Size(118, 21);
			this.comboSort.TabIndex = 37;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(186, 70);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 14);
			this.label5.TabIndex = 36;
			this.label5.Text = "Sort";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSite
			// 
			this.comboSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSite.Location = new System.Drawing.Point(437, 68);
			this.comboSite.MaxDropDownItems = 40;
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(160, 21);
			this.comboSite.TabIndex = 25;
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(365, 71);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(70, 14);
			this.labelSite.TabIndex = 24;
			this.labelSite.Text = "Site";
			this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.DoIncludeAll = true;
			this.comboClinic.DoIncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(402, 42);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(195, 21);
			this.comboClinic.TabIndex = 23;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(365, 19);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 14);
			this.label4.TabIndex = 20;
			this.label4.Text = "Provider";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkGroupFamilies
			// 
			this.checkGroupFamilies.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamilies.Location = new System.Drawing.Point(77, 12);
			this.checkGroupFamilies.Name = "checkGroupFamilies";
			this.checkGroupFamilies.Size = new System.Drawing.Size(108, 18);
			this.checkGroupFamilies.TabIndex = 19;
			this.checkGroupFamilies.Text = "Group Families";
			this.checkGroupFamilies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamilies.UseVisualStyleBackColor = true;
			this.checkGroupFamilies.Click += new System.EventHandler(this.checkGroupFamilies_Click);
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(283, 42);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 18;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(283, 18);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 17;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(198, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 14);
			this.label2.TabIndex = 12;
			this.label2.Text = "End Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(198, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 14);
			this.label1.TabIndex = 11;
			this.label1.Text = "Start Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butRefresh
			// 
			this.butRefresh.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRefresh.Autosize = true;
			this.butRefresh.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRefresh.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRefresh.CornerRadius = 4F;
			this.butRefresh.Location = new System.Drawing.Point(609, 64);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(80, 24);
			this.butRefresh.TabIndex = 2;
			this.butRefresh.Text = "&Refresh List";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// tabPageRecentlyContacted
			// 
			this.tabPageRecentlyContacted.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageRecentlyContacted.Controls.Add(this.gridRecentlyContacted);
			this.tabPageRecentlyContacted.Controls.Add(this.groupBox4);
			this.tabPageRecentlyContacted.Location = new System.Drawing.Point(4, 22);
			this.tabPageRecentlyContacted.Name = "tabPageRecentlyContacted";
			this.tabPageRecentlyContacted.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRecentlyContacted.Size = new System.Drawing.Size(971, 699);
			this.tabPageRecentlyContacted.TabIndex = 1;
			this.tabPageRecentlyContacted.Tag = this.gridRecentlyContacted;
			this.tabPageRecentlyContacted.Text = "Reminders";
			// 
			// gridRecentlyContacted
			// 
			this.gridRecentlyContacted.AllowSortingByColumn = true;
			this.gridRecentlyContacted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRecentlyContacted.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridRecentlyContacted.HasDropDowns = false;
			this.gridRecentlyContacted.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridRecentlyContacted.HeaderHeight = 15;
			this.gridRecentlyContacted.HScrollVisible = true;
			this.gridRecentlyContacted.Location = new System.Drawing.Point(3, 86);
			this.gridRecentlyContacted.Name = "gridRecentlyContacted";
			this.gridRecentlyContacted.ScrollValue = 0;
			this.gridRecentlyContacted.Size = new System.Drawing.Size(965, 607);
			this.gridRecentlyContacted.TabIndex = 127;
			this.gridRecentlyContacted.Title = "Reminder List";
			this.gridRecentlyContacted.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridRecentlyContacted.TitleHeight = 18;
			this.gridRecentlyContacted.TranslationName = "";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.butRefreshRecent);
			this.groupBox4.Controls.Add(this.comboClinicRecent);
			this.groupBox4.Controls.Add(this.datePickerRecent);
			this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox4.Location = new System.Drawing.Point(35, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(395, 74);
			this.groupBox4.TabIndex = 121;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "View";
			// 
			// butRefreshRecent
			// 
			this.butRefreshRecent.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRefreshRecent.Autosize = true;
			this.butRefreshRecent.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRefreshRecent.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRefreshRecent.CornerRadius = 4F;
			this.butRefreshRecent.Location = new System.Drawing.Point(307, 40);
			this.butRefreshRecent.Name = "butRefreshRecent";
			this.butRefreshRecent.Size = new System.Drawing.Size(80, 24);
			this.butRefreshRecent.TabIndex = 2;
			this.butRefreshRecent.Text = "&Refresh List";
			this.butRefreshRecent.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboClinicRecent
			// 
			this.comboClinicRecent.DoIncludeAll = true;
			this.comboClinicRecent.DoIncludeUnassigned = true;
			this.comboClinicRecent.Location = new System.Drawing.Point(189, 14);
			this.comboClinicRecent.Name = "comboClinicRecent";
			this.comboClinicRecent.Size = new System.Drawing.Size(198, 21);
			this.comboClinicRecent.TabIndex = 23;
			// 
			// datePickerRecent
			// 
			this.datePickerRecent.BackColor = System.Drawing.Color.Transparent;
			this.datePickerRecent.DefaultDateTimeFrom = new System.DateTime(2018, 1, 1, 0, 0, 0, 0);
			this.datePickerRecent.DefaultDateTimeTo = new System.DateTime(2018, 4, 16, 0, 0, 0, 0);
			this.datePickerRecent.EnableWeekButtons = false;
			this.datePickerRecent.IsVertical = true;
			this.datePickerRecent.Location = new System.Drawing.Point(6, 13);
			this.datePickerRecent.MaximumSize = new System.Drawing.Size(0, 185);
			this.datePickerRecent.MinimumSize = new System.Drawing.Size(100, 22);
			this.datePickerRecent.Name = "datePickerRecent";
			this.datePickerRecent.Size = new System.Drawing.Size(181, 57);
			this.datePickerRecent.TabIndex = 24;
			// 
			// tabPageReactivations
			// 
			this.tabPageReactivations.BackColor = System.Drawing.Color.Transparent;
			this.tabPageReactivations.Controls.Add(this.groupBox5);
			this.tabPageReactivations.Controls.Add(this.butReactGoToFam);
			this.tabPageReactivations.Controls.Add(this.butReactComm);
			this.tabPageReactivations.Controls.Add(this.butReactGoToAcct);
			this.tabPageReactivations.Controls.Add(this.butReactSchedFam);
			this.tabPageReactivations.Controls.Add(this.butReactSingleLabels);
			this.tabPageReactivations.Controls.Add(this.butReactSchedPat);
			this.tabPageReactivations.Controls.Add(this.butReactEmail);
			this.tabPageReactivations.Controls.Add(this.butReactPostcardPreview);
			this.tabPageReactivations.Controls.Add(this.butReactPrint);
			this.tabPageReactivations.Controls.Add(this.butReactLabelPreview);
			this.tabPageReactivations.Controls.Add(this.labelReactPatCount);
			this.tabPageReactivations.Controls.Add(this.gridReactivations);
			this.tabPageReactivations.Controls.Add(this.groupBox6);
			this.tabPageReactivations.Controls.Add(this.groupBox7);
			this.tabPageReactivations.Location = new System.Drawing.Point(4, 22);
			this.tabPageReactivations.Name = "tabPageReactivations";
			this.tabPageReactivations.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageReactivations.Size = new System.Drawing.Size(971, 699);
			this.tabPageReactivations.TabIndex = 2;
			this.tabPageReactivations.Tag = this.gridReactivations;
			this.tabPageReactivations.Text = "Reactivations";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.comboReactEmailFrom);
			this.groupBox5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox5.Location = new System.Drawing.Point(704, 49);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(263, 40);
			this.groupBox5.TabIndex = 163;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Email From";
			// 
			// comboReactEmailFrom
			// 
			this.comboReactEmailFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReactEmailFrom.Location = new System.Drawing.Point(17, 14);
			this.comboReactEmailFrom.MaxDropDownItems = 40;
			this.comboReactEmailFrom.Name = "comboReactEmailFrom";
			this.comboReactEmailFrom.Size = new System.Drawing.Size(233, 21);
			this.comboReactEmailFrom.TabIndex = 65;
			// 
			// butReactGoToFam
			// 
			this.butReactGoToFam.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactGoToFam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactGoToFam.Autosize = true;
			this.butReactGoToFam.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactGoToFam.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactGoToFam.CornerRadius = 4F;
			this.butReactGoToFam.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactGoToFam.Location = new System.Drawing.Point(443, 642);
			this.butReactGoToFam.Name = "butReactGoToFam";
			this.butReactGoToFam.Size = new System.Drawing.Size(96, 24);
			this.butReactGoToFam.TabIndex = 162;
			this.butReactGoToFam.Text = "Go to Family";
			this.butReactGoToFam.Click += new System.EventHandler(this.butGotoFamily_Click);
			// 
			// butReactComm
			// 
			this.butReactComm.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactComm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactComm.Autosize = true;
			this.butReactComm.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactComm.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactComm.CornerRadius = 4F;
			this.butReactComm.Image = global::OpenDental.Properties.Resources.commlog;
			this.butReactComm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactComm.Location = new System.Drawing.Point(545, 668);
			this.butReactComm.Name = "butReactComm";
			this.butReactComm.Size = new System.Drawing.Size(88, 24);
			this.butReactComm.TabIndex = 161;
			this.butReactComm.Text = "Comm";
			this.butReactComm.Click += new System.EventHandler(this.butCommlog_Click);
			// 
			// butReactGoToAcct
			// 
			this.butReactGoToAcct.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactGoToAcct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactGoToAcct.Autosize = true;
			this.butReactGoToAcct.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactGoToAcct.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactGoToAcct.CornerRadius = 4F;
			this.butReactGoToAcct.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactGoToAcct.Location = new System.Drawing.Point(443, 668);
			this.butReactGoToAcct.Name = "butReactGoToAcct";
			this.butReactGoToAcct.Size = new System.Drawing.Size(96, 24);
			this.butReactGoToAcct.TabIndex = 160;
			this.butReactGoToAcct.Text = "Go to Account";
			this.butReactGoToAcct.Click += new System.EventHandler(this.butGotoAccount_Click);
			// 
			// butReactSchedFam
			// 
			this.butReactSchedFam.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactSchedFam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactSchedFam.Autosize = true;
			this.butReactSchedFam.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactSchedFam.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactSchedFam.CornerRadius = 4F;
			this.butReactSchedFam.Image = global::OpenDental.Properties.Resources.butPin;
			this.butReactSchedFam.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactSchedFam.Location = new System.Drawing.Point(639, 668);
			this.butReactSchedFam.Name = "butReactSchedFam";
			this.butReactSchedFam.Size = new System.Drawing.Size(114, 24);
			this.butReactSchedFam.TabIndex = 156;
			this.butReactSchedFam.Tag = "SchedFamReact";
			this.butReactSchedFam.Text = "Sched Family";
			this.butReactSchedFam.Click += new System.EventHandler(this.butSched_Click);
			// 
			// butReactSingleLabels
			// 
			this.butReactSingleLabels.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactSingleLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactSingleLabels.Autosize = true;
			this.butReactSingleLabels.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactSingleLabels.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactSingleLabels.CornerRadius = 4F;
			this.butReactSingleLabels.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butReactSingleLabels.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactSingleLabels.Location = new System.Drawing.Point(128, 642);
			this.butReactSingleLabels.Name = "butReactSingleLabels";
			this.butReactSingleLabels.Size = new System.Drawing.Size(119, 24);
			this.butReactSingleLabels.TabIndex = 159;
			this.butReactSingleLabels.Text = "Single Labels";
			this.butReactSingleLabels.Click += new System.EventHandler(this.butLabelOne_Click);
			// 
			// butReactSchedPat
			// 
			this.butReactSchedPat.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactSchedPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactSchedPat.Autosize = true;
			this.butReactSchedPat.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactSchedPat.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactSchedPat.CornerRadius = 4F;
			this.butReactSchedPat.Image = global::OpenDental.Properties.Resources.butPin;
			this.butReactSchedPat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactSchedPat.Location = new System.Drawing.Point(639, 642);
			this.butReactSchedPat.Name = "butReactSchedPat";
			this.butReactSchedPat.Size = new System.Drawing.Size(114, 24);
			this.butReactSchedPat.TabIndex = 155;
			this.butReactSchedPat.Tag = "SchedPatReact";
			this.butReactSchedPat.Text = "Sched Patient";
			this.butReactSchedPat.Click += new System.EventHandler(this.butSched_Click);
			// 
			// butReactEmail
			// 
			this.butReactEmail.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactEmail.Autosize = true;
			this.butReactEmail.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactEmail.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactEmail.CornerRadius = 4F;
			this.butReactEmail.Image = global::OpenDental.Properties.Resources.email1;
			this.butReactEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactEmail.Location = new System.Drawing.Point(253, 668);
			this.butReactEmail.Name = "butReactEmail";
			this.butReactEmail.Size = new System.Drawing.Size(91, 24);
			this.butReactEmail.TabIndex = 158;
			this.butReactEmail.Text = "E-Mail";
			this.butReactEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butReactPostcardPreview
			// 
			this.butReactPostcardPreview.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactPostcardPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactPostcardPreview.Autosize = true;
			this.butReactPostcardPreview.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactPostcardPreview.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactPostcardPreview.CornerRadius = 4F;
			this.butReactPostcardPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butReactPostcardPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactPostcardPreview.Location = new System.Drawing.Point(3, 642);
			this.butReactPostcardPreview.Name = "butReactPostcardPreview";
			this.butReactPostcardPreview.Size = new System.Drawing.Size(119, 24);
			this.butReactPostcardPreview.TabIndex = 153;
			this.butReactPostcardPreview.Text = "Postcard Preview";
			this.butReactPostcardPreview.Click += new System.EventHandler(this.butPostcards_Click);
			// 
			// butReactPrint
			// 
			this.butReactPrint.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactPrint.Autosize = true;
			this.butReactPrint.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactPrint.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactPrint.CornerRadius = 4F;
			this.butReactPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butReactPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactPrint.Location = new System.Drawing.Point(350, 668);
			this.butReactPrint.Name = "butReactPrint";
			this.butReactPrint.Size = new System.Drawing.Size(87, 24);
			this.butReactPrint.TabIndex = 154;
			this.butReactPrint.Text = "Print List";
			this.butReactPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butReactLabelPreview
			// 
			this.butReactLabelPreview.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactLabelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactLabelPreview.Autosize = true;
			this.butReactLabelPreview.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactLabelPreview.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactLabelPreview.CornerRadius = 4F;
			this.butReactLabelPreview.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butReactLabelPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactLabelPreview.Location = new System.Drawing.Point(128, 668);
			this.butReactLabelPreview.Name = "butReactLabelPreview";
			this.butReactLabelPreview.Size = new System.Drawing.Size(119, 24);
			this.butReactLabelPreview.TabIndex = 152;
			this.butReactLabelPreview.Text = "Label Preview";
			this.butReactLabelPreview.Click += new System.EventHandler(this.butLabels_Click);
			// 
			// labelReactPatCount
			// 
			this.labelReactPatCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelReactPatCount.Location = new System.Drawing.Point(854, 640);
			this.labelReactPatCount.Name = "labelReactPatCount";
			this.labelReactPatCount.Size = new System.Drawing.Size(114, 14);
			this.labelReactPatCount.TabIndex = 151;
			this.labelReactPatCount.Text = "Patient Count:";
			this.labelReactPatCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gridReactivations
			// 
			this.gridReactivations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridReactivations.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridReactivations.HasDropDowns = false;
			this.gridReactivations.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridReactivations.HeaderHeight = 15;
			this.gridReactivations.HScrollVisible = true;
			this.gridReactivations.Location = new System.Drawing.Point(3, 93);
			this.gridReactivations.Name = "gridReactivations";
			this.gridReactivations.ScrollValue = 0;
			this.gridReactivations.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridReactivations.Size = new System.Drawing.Size(965, 544);
			this.gridReactivations.TabIndex = 145;
			this.gridReactivations.Title = "Reactivation List";
			this.gridReactivations.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridReactivations.TitleHeight = 18;
			this.gridReactivations.TranslationName = "TableReactivationList";
			this.gridReactivations.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridReactivations_CellDoubleClick);
			this.gridReactivations.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.comboReactStatus);
			this.groupBox6.Controls.Add(this.butReactSetStatus);
			this.groupBox6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox6.Location = new System.Drawing.Point(704, 6);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(263, 40);
			this.groupBox6.TabIndex = 142;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Set Status";
			// 
			// comboReactStatus
			// 
			this.comboReactStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReactStatus.Location = new System.Drawing.Point(17, 14);
			this.comboReactStatus.MaxDropDownItems = 40;
			this.comboReactStatus.Name = "comboReactStatus";
			this.comboReactStatus.Size = new System.Drawing.Size(160, 21);
			this.comboReactStatus.TabIndex = 15;
			// 
			// butReactSetStatus
			// 
			this.butReactSetStatus.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactSetStatus.Autosize = true;
			this.butReactSetStatus.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactSetStatus.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactSetStatus.CornerRadius = 4F;
			this.butReactSetStatus.Location = new System.Drawing.Point(183, 11);
			this.butReactSetStatus.Name = "butReactSetStatus";
			this.butReactSetStatus.Size = new System.Drawing.Size(67, 24);
			this.butReactSetStatus.TabIndex = 14;
			this.butReactSetStatus.Text = "Set";
			this.butReactSetStatus.Click += new System.EventHandler(this.butReactSetStatus_Click);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.checkReactGroupFamilies);
			this.groupBox7.Controls.Add(this.comboReactProv);
			this.groupBox7.Controls.Add(this.checkReactShowDNC);
			this.groupBox7.Controls.Add(this.comboBillingTypes);
			this.groupBox7.Controls.Add(this.labelBillingType);
			this.groupBox7.Controls.Add(this.comboShowReactivate);
			this.groupBox7.Controls.Add(this.labelShowReactivate);
			this.groupBox7.Controls.Add(this.comboReactSortBy);
			this.groupBox7.Controls.Add(this.labelReactSortBy);
			this.groupBox7.Controls.Add(this.comboReactSite);
			this.groupBox7.Controls.Add(this.labelReactSite);
			this.groupBox7.Controls.Add(this.comboReactClinic);
			this.groupBox7.Controls.Add(this.labelReactProv);
			this.groupBox7.Controls.Add(this.validDateSince);
			this.groupBox7.Controls.Add(this.labelDateSince);
			this.groupBox7.Controls.Add(this.butReactRefresh);
			this.groupBox7.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox7.Location = new System.Drawing.Point(3, 6);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(695, 83);
			this.groupBox7.TabIndex = 139;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "View";
			// 
			// checkReactGroupFamilies
			// 
			this.checkReactGroupFamilies.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReactGroupFamilies.Location = new System.Drawing.Point(26, 14);
			this.checkReactGroupFamilies.Name = "checkReactGroupFamilies";
			this.checkReactGroupFamilies.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkReactGroupFamilies.Size = new System.Drawing.Size(159, 18);
			this.checkReactGroupFamilies.TabIndex = 44;
			this.checkReactGroupFamilies.Text = "Group Families";
			this.checkReactGroupFamilies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReactGroupFamilies.UseVisualStyleBackColor = true;
			this.checkReactGroupFamilies.MouseClick += new System.Windows.Forms.MouseEventHandler(this.checkGroupFamilies_Click);
			// 
			// comboReactProv
			// 
			this.comboReactProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReactProv.FormattingEnabled = true;
			this.comboReactProv.Location = new System.Drawing.Point(437, 11);
			this.comboReactProv.Name = "comboReactProv";
			this.comboReactProv.Size = new System.Drawing.Size(160, 21);
			this.comboReactProv.TabIndex = 43;
			// 
			// checkReactShowDNC
			// 
			this.checkReactShowDNC.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReactShowDNC.Location = new System.Drawing.Point(26, 33);
			this.checkReactShowDNC.Name = "checkReactShowDNC";
			this.checkReactShowDNC.Size = new System.Drawing.Size(159, 18);
			this.checkReactShowDNC.TabIndex = 42;
			this.checkReactShowDNC.Text = "Show \"Do Not Contact\"";
			this.checkReactShowDNC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReactShowDNC.UseVisualStyleBackColor = true;
			// 
			// comboBillingTypes
			// 
			this.comboBillingTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBillingTypes.Location = new System.Drawing.Point(255, 34);
			this.comboBillingTypes.MaxDropDownItems = 40;
			this.comboBillingTypes.Name = "comboBillingTypes";
			this.comboBillingTypes.Size = new System.Drawing.Size(118, 21);
			this.comboBillingTypes.TabIndex = 41;
			// 
			// labelBillingType
			// 
			this.labelBillingType.Location = new System.Drawing.Point(192, 37);
			this.labelBillingType.Name = "labelBillingType";
			this.labelBillingType.Size = new System.Drawing.Size(62, 14);
			this.labelBillingType.TabIndex = 40;
			this.labelBillingType.Text = "Billing Type";
			this.labelBillingType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboShowReactivate
			// 
			this.comboShowReactivate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboShowReactivate.Location = new System.Drawing.Point(103, 57);
			this.comboShowReactivate.MaxDropDownItems = 40;
			this.comboShowReactivate.Name = "comboShowReactivate";
			this.comboShowReactivate.Size = new System.Drawing.Size(82, 21);
			this.comboShowReactivate.TabIndex = 39;
			// 
			// labelShowReactivate
			// 
			this.labelShowReactivate.Location = new System.Drawing.Point(3, 60);
			this.labelShowReactivate.Name = "labelShowReactivate";
			this.labelShowReactivate.Size = new System.Drawing.Size(99, 14);
			this.labelShowReactivate.TabIndex = 38;
			this.labelShowReactivate.Text = "Show Reactivate";
			this.labelShowReactivate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboReactSortBy
			// 
			this.comboReactSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReactSortBy.Location = new System.Drawing.Point(255, 56);
			this.comboReactSortBy.MaxDropDownItems = 40;
			this.comboReactSortBy.Name = "comboReactSortBy";
			this.comboReactSortBy.Size = new System.Drawing.Size(118, 21);
			this.comboReactSortBy.TabIndex = 37;
			// 
			// labelReactSortBy
			// 
			this.labelReactSortBy.Location = new System.Drawing.Point(199, 59);
			this.labelReactSortBy.Name = "labelReactSortBy";
			this.labelReactSortBy.Size = new System.Drawing.Size(55, 14);
			this.labelReactSortBy.TabIndex = 36;
			this.labelReactSortBy.Text = "Sort";
			this.labelReactSortBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboReactSite
			// 
			this.comboReactSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReactSite.Location = new System.Drawing.Point(437, 57);
			this.comboReactSite.MaxDropDownItems = 40;
			this.comboReactSite.Name = "comboReactSite";
			this.comboReactSite.Size = new System.Drawing.Size(160, 21);
			this.comboReactSite.TabIndex = 25;
			// 
			// labelReactSite
			// 
			this.labelReactSite.Location = new System.Drawing.Point(379, 60);
			this.labelReactSite.Name = "labelReactSite";
			this.labelReactSite.Size = new System.Drawing.Size(56, 14);
			this.labelReactSite.TabIndex = 24;
			this.labelReactSite.Text = "Site";
			this.labelReactSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboReactClinic
			// 
			this.comboReactClinic.DoIncludeAll = true;
			this.comboReactClinic.DoIncludeUnassigned = true;
			this.comboReactClinic.Location = new System.Drawing.Point(402, 34);
			this.comboReactClinic.Name = "comboReactClinic";
			this.comboReactClinic.Size = new System.Drawing.Size(195, 21);
			this.comboReactClinic.TabIndex = 23;
			// 
			// labelReactProv
			// 
			this.labelReactProv.Location = new System.Drawing.Point(365, 14);
			this.labelReactProv.Name = "labelReactProv";
			this.labelReactProv.Size = new System.Drawing.Size(70, 14);
			this.labelReactProv.TabIndex = 20;
			this.labelReactProv.Text = "Provider";
			this.labelReactProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// validDateSince
			// 
			this.validDateSince.Location = new System.Drawing.Point(255, 13);
			this.validDateSince.Name = "validDateSince";
			this.validDateSince.Size = new System.Drawing.Size(77, 20);
			this.validDateSince.TabIndex = 17;
			// 
			// labelDateSince
			// 
			this.labelDateSince.Location = new System.Drawing.Point(189, 16);
			this.labelDateSince.Name = "labelDateSince";
			this.labelDateSince.Size = new System.Drawing.Size(65, 14);
			this.labelDateSince.TabIndex = 11;
			this.labelDateSince.Text = "Date Since";
			this.labelDateSince.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butReactRefresh
			// 
			this.butReactRefresh.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butReactRefresh.Autosize = true;
			this.butReactRefresh.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butReactRefresh.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butReactRefresh.CornerRadius = 4F;
			this.butReactRefresh.Location = new System.Drawing.Point(609, 53);
			this.butReactRefresh.Name = "butReactRefresh";
			this.butReactRefresh.Size = new System.Drawing.Size(80, 24);
			this.butReactRefresh.TabIndex = 2;
			this.butReactRefresh.Text = "&Refresh List";
			this.butReactRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// FormRecallList
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(985, 761);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(991, 385);
			this.Name = "FormRecallList";
			this.Text = "Recall List";
			this.Load += new System.EventHandler(this.FormRecallList_Load);
			this.Shown+=new System.EventHandler(this.FormRecallList_Shown);
			this.tabControl.ResumeLayout(false);
			this.tabPageRecalls.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPageRecentlyContacted.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.tabPageReactivations.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		#region UI Variables
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.ContextMenu menuRightClick;
		private System.Windows.Forms.MenuItem menuItemSeeFamily;
		private System.Windows.Forms.MenuItem menuItemSeeAccount;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageRecalls;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboEmailFrom;
		private System.Windows.Forms.Panel panelWebSched;
		private OpenDental.UI.Button butUndo;
		private OpenDental.UI.Button butGotoFamily;
		private OpenDental.UI.Button butCommlog;
		private OpenDental.UI.Button butGotoAccount;
		private OpenDental.UI.Button butSchedFam;
		private OpenDental.UI.Button butLabelOne;
		private OpenDental.UI.Button butSchedPat;
		private System.Windows.Forms.Label labelPatientCount;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butECards;
		private OpenDental.UI.Button butEmail;
		private OpenDental.UI.Button butPostcards;
		private OpenDental.UI.ODGrid gridMain;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ComboBox comboStatus;
		private OpenDental.UI.Button butSetStatus;
		private OpenDental.UI.Button butLabels;
		private OpenDental.UI.Button butReport;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkConflictingTypes;
		private System.Windows.Forms.ComboBox comboNumberReminders;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboSort;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboSite;
		private System.Windows.Forms.Label labelSite;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox checkGroupFamilies;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butRefresh;
		private System.Windows.Forms.TabPage tabPageRecentlyContacted;
		private OpenDental.UI.ODGrid gridRecentlyContacted;
		private System.Windows.Forms.GroupBox groupBox4;
		private OpenDental.UI.ComboBoxClinicPicker comboClinicRecent;
		private OpenDental.UI.Button butRefreshRecent;
		private OpenDental.UI.ODDateRangePicker datePickerRecent;
		private System.Windows.Forms.TabPage tabPageReactivations;
		private System.Windows.Forms.Label labelReactPatCount;
		private System.Windows.Forms.GroupBox groupBox6;
		private OpenDental.UI.Button butReactSetStatus;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.ComboBox comboShowReactivate;
		private System.Windows.Forms.Label labelShowReactivate;
		private System.Windows.Forms.ComboBox comboReactSortBy;
		private System.Windows.Forms.Label labelReactSortBy;
		private System.Windows.Forms.ComboBox comboReactSite;
		private System.Windows.Forms.Label labelReactSite;
		private OpenDental.UI.ComboBoxClinicPicker comboReactClinic;
		private System.Windows.Forms.Label labelReactProv;
		private ValidDate validDateSince;
		private System.Windows.Forms.Label labelDateSince;
		private OpenDental.UI.Button butReactRefresh;
		private System.Windows.Forms.ComboBox comboBillingTypes;
		private System.Windows.Forms.Label labelBillingType;
		private System.Windows.Forms.CheckBox checkReactShowDNC;
		private System.Windows.Forms.ComboBox comboReactStatus;
		private System.Windows.Forms.ComboBox comboReactProv;
		private System.Windows.Forms.ComboBox comboProv;
		private System.Windows.Forms.CheckBox checkReactGroupFamilies;
		private OpenDental.UI.Button butReactGoToFam;
		private OpenDental.UI.Button butReactComm;
		private OpenDental.UI.Button butReactGoToAcct;
		private OpenDental.UI.Button butReactSchedFam;
		private OpenDental.UI.Button butReactSingleLabels;
		private OpenDental.UI.Button butReactSchedPat;
		private OpenDental.UI.Button butReactEmail;
		private OpenDental.UI.Button butReactPostcardPreview;
		private OpenDental.UI.Button butReactPrint;
		private OpenDental.UI.Button butReactLabelPreview;
		#endregion UI Variables

		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.ComboBox comboReactEmailFrom;
		private UI.ODGrid gridReactivations;
		private System.Windows.Forms.CheckBox checkShowReminded;
	}
}
