namespace OpenDental {
	partial class FormApptEdit {
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptEdit));
			this.comboConfirmed = new System.Windows.Forms.ComboBox();
			this.comboUnschedStatus = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.checkIsHygiene = new System.Windows.Forms.CheckBox();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.comboAssistant = new System.Windows.Forms.ComboBox();
			this.comboProvHyg = new System.Windows.Forms.ComboBox();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.checkIsNewPatient = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelApptNote = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textTime = new System.Windows.Forms.TextBox();
			this.butSlider = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.checkTimeLocked = new System.Windows.Forms.CheckBox();
			this.contextMenuTimeArrived = new System.Windows.Forms.ContextMenu();
			this.menuItemArrivedNow = new System.Windows.Forms.MenuItem();
			this.contextMenuTimeSeated = new System.Windows.Forms.ContextMenu();
			this.menuItemSeatedNow = new System.Windows.Forms.MenuItem();
			this.contextMenuTimeDismissed = new System.Windows.Forms.ContextMenu();
			this.menuItemDismissedNow = new System.Windows.Forms.MenuItem();
			this.textTimeAskedToArrive = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.listQuickAdd = new System.Windows.Forms.ListBox();
			this.labelQuickAdd = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkASAP = new System.Windows.Forms.CheckBox();
			this.comboApptType = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.labelSyndromicObservations = new System.Windows.Forms.Label();
			this.butSyndromicObservations = new OpenDental.UI.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.butColorClear = new OpenDental.UI.Button();
			this.butColor = new System.Windows.Forms.Button();
			this.textRequirement = new System.Windows.Forms.TextBox();
			this.butRequirement = new OpenDental.UI.Button();
			this.butInsPlan2 = new OpenDental.UI.Button();
			this.butInsPlan1 = new OpenDental.UI.Button();
			this.textInsPlan2 = new System.Windows.Forms.TextBox();
			this.labelInsPlan2 = new System.Windows.Forms.Label();
			this.textInsPlan1 = new System.Windows.Forms.TextBox();
			this.labelInsPlan1 = new System.Windows.Forms.Label();
			this.textTimeDismissed = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textTimeSeated = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textTimeArrived = new System.Windows.Forms.TextBox();
			this.labelTimeArrived = new System.Windows.Forms.Label();
			this.textLabCase = new System.Windows.Forms.TextBox();
			this.butLab = new OpenDental.UI.Button();
			this.butPickHyg = new OpenDental.UI.Button();
			this.butPickDentist = new OpenDental.UI.Button();
			this.gridFields = new OpenDental.UI.ODGrid();
			this.gridPatient = new OpenDental.UI.ODGrid();
			this.gridComm = new OpenDental.UI.ODGrid();
			this.gridProc = new OpenDental.UI.ODGrid();
			this.labelPlannedComplete = new System.Windows.Forms.Label();
			this.butPDF = new OpenDental.UI.Button();
			this.butComplete = new OpenDental.UI.Button();
			this.butDeleteProc = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.textNote = new OpenDental.ODtextBox();
			this.butText = new OpenDental.UI.Button();
			this.butAddComm = new OpenDental.UI.Button();
			this.tbTime = new OpenDental.TableTimeBar();
			this.butAudit = new OpenDental.UI.Button();
			this.butTask = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butPin = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAttachAll = new OpenDental.UI.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboConfirmed
			// 
			this.comboConfirmed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboConfirmed.Location = new System.Drawing.Point(114, 65);
			this.comboConfirmed.MaxDropDownItems = 30;
			this.comboConfirmed.Name = "comboConfirmed";
			this.comboConfirmed.Size = new System.Drawing.Size(126, 21);
			this.comboConfirmed.TabIndex = 84;
			this.comboConfirmed.SelectionChangeCommitted += new System.EventHandler(this.comboConfirmed_SelectionChangeCommitted);
			// 
			// comboUnschedStatus
			// 
			this.comboUnschedStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnschedStatus.Location = new System.Drawing.Point(114, 44);
			this.comboUnschedStatus.MaxDropDownItems = 30;
			this.comboUnschedStatus.Name = "comboUnschedStatus";
			this.comboUnschedStatus.Size = new System.Drawing.Size(126, 21);
			this.comboUnschedStatus.TabIndex = 83;
			// 
			// label4
			// 
			this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label4.Location = new System.Drawing.Point(6, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 16);
			this.label4.TabIndex = 82;
			this.label4.Text = "Unscheduled Status";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.Location = new System.Drawing.Point(114, 4);
			this.comboStatus.MaxDropDownItems = 30;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(126, 21);
			this.comboStatus.TabIndex = 81;
			this.comboStatus.SelectionChangeCommitted += new System.EventHandler(this.comboStatus_SelectionChangeCommitted);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 67);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(106, 16);
			this.label5.TabIndex = 80;
			this.label5.Text = "Confirmed";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStatus
			// 
			this.labelStatus.Location = new System.Drawing.Point(6, 6);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(106, 16);
			this.labelStatus.TabIndex = 79;
			this.labelStatus.Text = "Status";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(128, 170);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(113, 16);
			this.label24.TabIndex = 138;
			this.label24.Text = "(use hyg color)";
			// 
			// checkIsHygiene
			// 
			this.checkIsHygiene.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHygiene.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHygiene.Location = new System.Drawing.Point(6, 170);
			this.checkIsHygiene.Name = "checkIsHygiene";
			this.checkIsHygiene.Size = new System.Drawing.Size(121, 16);
			this.checkIsHygiene.TabIndex = 137;
			this.checkIsHygiene.Text = "Is Hygiene";
			this.checkIsHygiene.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.Location = new System.Drawing.Point(114, 105);
			this.comboClinic.MaxDropDownItems = 30;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(126, 21);
			this.comboClinic.TabIndex = 136;
			this.comboClinic.SelectedIndexChanged += new System.EventHandler(this.comboClinic_SelectedIndexChanged);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(6, 107);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(106, 16);
			this.labelClinic.TabIndex = 135;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboAssistant
			// 
			this.comboAssistant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAssistant.Location = new System.Drawing.Point(114, 187);
			this.comboAssistant.MaxDropDownItems = 30;
			this.comboAssistant.Name = "comboAssistant";
			this.comboAssistant.Size = new System.Drawing.Size(126, 21);
			this.comboAssistant.TabIndex = 133;
			// 
			// comboProvHyg
			// 
			this.comboProvHyg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvHyg.Location = new System.Drawing.Point(114, 147);
			this.comboProvHyg.MaxDropDownItems = 30;
			this.comboProvHyg.Name = "comboProvHyg";
			this.comboProvHyg.Size = new System.Drawing.Size(106, 21);
			this.comboProvHyg.TabIndex = 132;
			this.comboProvHyg.SelectedIndexChanged += new System.EventHandler(this.comboProvHyg_SelectedIndexChanged);
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.Location = new System.Drawing.Point(114, 126);
			this.comboProv.MaxDropDownItems = 30;
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(106, 21);
			this.comboProv.TabIndex = 131;
			this.comboProv.SelectedIndexChanged += new System.EventHandler(this.comboProv_SelectedIndexChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(6, 189);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(106, 16);
			this.label12.TabIndex = 129;
			this.label12.Text = "Assistant";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsNewPatient
			// 
			this.checkIsNewPatient.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsNewPatient.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsNewPatient.Location = new System.Drawing.Point(6, 88);
			this.checkIsNewPatient.Name = "checkIsNewPatient";
			this.checkIsNewPatient.Size = new System.Drawing.Size(121, 16);
			this.checkIsNewPatient.TabIndex = 128;
			this.checkIsNewPatient.Text = "New Patient";
			this.checkIsNewPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 149);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 16);
			this.label3.TabIndex = 127;
			this.label3.Text = "Hygienist";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 128);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 16);
			this.label2.TabIndex = 126;
			this.label2.Text = "Provider";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelApptNote
			// 
			this.labelApptNote.Location = new System.Drawing.Point(20, 451);
			this.labelApptNote.Name = "labelApptNote";
			this.labelApptNote.Size = new System.Drawing.Size(197, 16);
			this.labelApptNote.TabIndex = 141;
			this.labelApptNote.Text = "Appointment Note";
			this.labelApptNote.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 212);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(106, 16);
			this.label6.TabIndex = 65;
			this.label6.Text = "Time Length";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTime
			// 
			this.textTime.Location = new System.Drawing.Point(114, 210);
			this.textTime.Name = "textTime";
			this.textTime.ReadOnly = true;
			this.textTime.Size = new System.Drawing.Size(66, 20);
			this.textTime.TabIndex = 62;
			// 
			// butSlider
			// 
			this.butSlider.BackColor = System.Drawing.SystemColors.ControlDark;
			this.butSlider.Location = new System.Drawing.Point(6, 90);
			this.butSlider.Name = "butSlider";
			this.butSlider.Size = new System.Drawing.Size(12, 15);
			this.butSlider.TabIndex = 60;
			this.butSlider.UseVisualStyleBackColor = false;
			this.butSlider.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseDown);
			this.butSlider.MouseMove += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseMove);
			this.butSlider.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseUp);
			// 
			// checkTimeLocked
			// 
			this.checkTimeLocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeLocked.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTimeLocked.Location = new System.Drawing.Point(6, 232);
			this.checkTimeLocked.Name = "checkTimeLocked";
			this.checkTimeLocked.Size = new System.Drawing.Size(121, 16);
			this.checkTimeLocked.TabIndex = 148;
			this.checkTimeLocked.Text = "Time Locked";
			this.checkTimeLocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeLocked.Click += new System.EventHandler(this.checkTimeLocked_Click);
			// 
			// contextMenuTimeArrived
			// 
			this.contextMenuTimeArrived.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemArrivedNow});
			// 
			// menuItemArrivedNow
			// 
			this.menuItemArrivedNow.Index = 0;
			this.menuItemArrivedNow.Text = "Now";
			this.menuItemArrivedNow.Click += new System.EventHandler(this.menuItemArrivedNow_Click);
			// 
			// contextMenuTimeSeated
			// 
			this.contextMenuTimeSeated.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSeatedNow});
			// 
			// menuItemSeatedNow
			// 
			this.menuItemSeatedNow.Index = 0;
			this.menuItemSeatedNow.Text = "Now";
			this.menuItemSeatedNow.Click += new System.EventHandler(this.menuItemSeatedNow_Click);
			// 
			// contextMenuTimeDismissed
			// 
			this.contextMenuTimeDismissed.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDismissedNow});
			// 
			// menuItemDismissedNow
			// 
			this.menuItemDismissedNow.Index = 0;
			this.menuItemDismissedNow.Text = "Now";
			this.menuItemDismissedNow.Click += new System.EventHandler(this.menuItemDismissedNow_Click);
			// 
			// textTimeAskedToArrive
			// 
			this.textTimeAskedToArrive.Location = new System.Drawing.Point(114, 293);
			this.textTimeAskedToArrive.Name = "textTimeAskedToArrive";
			this.textTimeAskedToArrive.Size = new System.Drawing.Size(126, 20);
			this.textTimeAskedToArrive.TabIndex = 146;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 295);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(106, 16);
			this.label8.TabIndex = 160;
			this.label8.Text = "Time Ask To Arrive";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listQuickAdd
			// 
			this.listQuickAdd.IntegralHeight = false;
			this.listQuickAdd.Location = new System.Drawing.Point(282, 48);
			this.listQuickAdd.Name = "listQuickAdd";
			this.listQuickAdd.Size = new System.Drawing.Size(150, 355);
			this.listQuickAdd.TabIndex = 163;
			this.listQuickAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listQuickAdd_MouseDown);
			// 
			// labelQuickAdd
			// 
			this.labelQuickAdd.Location = new System.Drawing.Point(282, 7);
			this.labelQuickAdd.Name = "labelQuickAdd";
			this.labelQuickAdd.Size = new System.Drawing.Size(143, 39);
			this.labelQuickAdd.TabIndex = 162;
			this.labelQuickAdd.Text = "Single click on items in the list below to add them to this appointment.";
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.checkASAP);
			this.panel1.Controls.Add(this.comboApptType);
			this.panel1.Controls.Add(this.label10);
			this.panel1.Controls.Add(this.labelSyndromicObservations);
			this.panel1.Controls.Add(this.butSyndromicObservations);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.butColorClear);
			this.panel1.Controls.Add(this.butColor);
			this.panel1.Controls.Add(this.textRequirement);
			this.panel1.Controls.Add(this.butRequirement);
			this.panel1.Controls.Add(this.butInsPlan2);
			this.panel1.Controls.Add(this.butInsPlan1);
			this.panel1.Controls.Add(this.textInsPlan2);
			this.panel1.Controls.Add(this.labelInsPlan2);
			this.panel1.Controls.Add(this.textInsPlan1);
			this.panel1.Controls.Add(this.labelInsPlan1);
			this.panel1.Controls.Add(this.textTimeDismissed);
			this.panel1.Controls.Add(this.label7);
			this.panel1.Controls.Add(this.textTimeSeated);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.textTimeArrived);
			this.panel1.Controls.Add(this.labelTimeArrived);
			this.panel1.Controls.Add(this.textLabCase);
			this.panel1.Controls.Add(this.butLab);
			this.panel1.Controls.Add(this.comboStatus);
			this.panel1.Controls.Add(this.checkIsNewPatient);
			this.panel1.Controls.Add(this.comboConfirmed);
			this.panel1.Controls.Add(this.label24);
			this.panel1.Controls.Add(this.textTimeAskedToArrive);
			this.panel1.Controls.Add(this.comboUnschedStatus);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.checkIsHygiene);
			this.panel1.Controls.Add(this.comboClinic);
			this.panel1.Controls.Add(this.labelClinic);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.comboAssistant);
			this.panel1.Controls.Add(this.butPickHyg);
			this.panel1.Controls.Add(this.comboProvHyg);
			this.panel1.Controls.Add(this.comboProv);
			this.panel1.Controls.Add(this.butPickDentist);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label12);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.labelStatus);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.textTime);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.checkTimeLocked);
			this.panel1.Location = new System.Drawing.Point(21, 2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(260, 446);
			this.panel1.TabIndex = 164;
			// 
			// checkASAP
			// 
			this.checkASAP.BackColor = System.Drawing.SystemColors.Control;
			this.checkASAP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkASAP.ForeColor = System.Drawing.SystemColors.ControlText;
			this.checkASAP.Location = new System.Drawing.Point(6, 27);
			this.checkASAP.Name = "checkASAP";
			this.checkASAP.Size = new System.Drawing.Size(121, 16);
			this.checkASAP.TabIndex = 184;
			this.checkASAP.Text = "ASAP";
			this.checkASAP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkASAP.UseVisualStyleBackColor = false;
			this.checkASAP.CheckedChanged += new System.EventHandler(this.checkASAP_CheckedChanged);
			// 
			// comboApptType
			// 
			this.comboApptType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboApptType.Location = new System.Drawing.Point(114, 270);
			this.comboApptType.MaxDropDownItems = 30;
			this.comboApptType.Name = "comboApptType";
			this.comboApptType.Size = new System.Drawing.Size(126, 21);
			this.comboApptType.TabIndex = 183;
			this.comboApptType.SelectionChangeCommitted += new System.EventHandler(this.comboApptType_SelectionChangeCommitted);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 272);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(106, 16);
			this.label10.TabIndex = 182;
			this.label10.Text = "Appointment Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSyndromicObservations
			// 
			this.labelSyndromicObservations.Location = new System.Drawing.Point(63, 508);
			this.labelSyndromicObservations.Name = "labelSyndromicObservations";
			this.labelSyndromicObservations.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelSyndromicObservations.Size = new System.Drawing.Size(174, 16);
			this.labelSyndromicObservations.TabIndex = 181;
			this.labelSyndromicObservations.Text = "(Syndromic Observations)";
			this.labelSyndromicObservations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelSyndromicObservations.Visible = false;
			// 
			// butSyndromicObservations
			// 
			this.butSyndromicObservations.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butSyndromicObservations.Autosize = true;
			this.butSyndromicObservations.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butSyndromicObservations.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butSyndromicObservations.CornerRadius = 4F;
			this.butSyndromicObservations.Location = new System.Drawing.Point(15, 506);
			this.butSyndromicObservations.Name = "butSyndromicObservations";
			this.butSyndromicObservations.Size = new System.Drawing.Size(46, 20);
			this.butSyndromicObservations.TabIndex = 180;
			this.butSyndromicObservations.Text = "Obs";
			this.butSyndromicObservations.Visible = false;
			this.butSyndromicObservations.Click += new System.EventHandler(this.butSyndromicObservations_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 251);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(106, 16);
			this.label9.TabIndex = 179;
			this.label9.Text = "Color";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorClear
			// 
			this.butColorClear.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butColorClear.Autosize = true;
			this.butColorClear.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butColorClear.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butColorClear.CornerRadius = 4F;
			this.butColorClear.Location = new System.Drawing.Point(137, 248);
			this.butColorClear.Name = "butColorClear";
			this.butColorClear.Size = new System.Drawing.Size(39, 20);
			this.butColorClear.TabIndex = 178;
			this.butColorClear.Text = "none";
			this.butColorClear.Click += new System.EventHandler(this.butColorClear_Click);
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(114, 249);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(21, 19);
			this.butColor.TabIndex = 177;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// textRequirement
			// 
			this.textRequirement.Location = new System.Drawing.Point(63, 454);
			this.textRequirement.Multiline = true;
			this.textRequirement.Name = "textRequirement";
			this.textRequirement.ReadOnly = true;
			this.textRequirement.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textRequirement.Size = new System.Drawing.Size(177, 53);
			this.textRequirement.TabIndex = 164;
			// 
			// butRequirement
			// 
			this.butRequirement.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRequirement.Autosize = true;
			this.butRequirement.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRequirement.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRequirement.CornerRadius = 4F;
			this.butRequirement.Location = new System.Drawing.Point(15, 454);
			this.butRequirement.Name = "butRequirement";
			this.butRequirement.Size = new System.Drawing.Size(46, 20);
			this.butRequirement.TabIndex = 163;
			this.butRequirement.Text = "Req";
			this.butRequirement.Click += new System.EventHandler(this.butRequirement_Click);
			// 
			// butInsPlan2
			// 
			this.butInsPlan2.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butInsPlan2.Autosize = false;
			this.butInsPlan2.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butInsPlan2.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butInsPlan2.CornerRadius = 2F;
			this.butInsPlan2.Location = new System.Drawing.Point(222, 433);
			this.butInsPlan2.Name = "butInsPlan2";
			this.butInsPlan2.Size = new System.Drawing.Size(18, 20);
			this.butInsPlan2.TabIndex = 176;
			this.butInsPlan2.Text = "...";
			this.butInsPlan2.Click += new System.EventHandler(this.butInsPlan2_Click);
			// 
			// butInsPlan1
			// 
			this.butInsPlan1.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butInsPlan1.Autosize = false;
			this.butInsPlan1.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butInsPlan1.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butInsPlan1.CornerRadius = 2F;
			this.butInsPlan1.Location = new System.Drawing.Point(222, 412);
			this.butInsPlan1.Name = "butInsPlan1";
			this.butInsPlan1.Size = new System.Drawing.Size(18, 20);
			this.butInsPlan1.TabIndex = 175;
			this.butInsPlan1.Text = "...";
			this.butInsPlan1.Click += new System.EventHandler(this.butInsPlan1_Click);
			// 
			// textInsPlan2
			// 
			this.textInsPlan2.Location = new System.Drawing.Point(63, 433);
			this.textInsPlan2.Name = "textInsPlan2";
			this.textInsPlan2.ReadOnly = true;
			this.textInsPlan2.Size = new System.Drawing.Size(158, 20);
			this.textInsPlan2.TabIndex = 174;
			// 
			// labelInsPlan2
			// 
			this.labelInsPlan2.Location = new System.Drawing.Point(6, 435);
			this.labelInsPlan2.Name = "labelInsPlan2";
			this.labelInsPlan2.Size = new System.Drawing.Size(55, 16);
			this.labelInsPlan2.TabIndex = 173;
			this.labelInsPlan2.Text = "InsPlan 2";
			this.labelInsPlan2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsPlan1
			// 
			this.textInsPlan1.Location = new System.Drawing.Point(63, 412);
			this.textInsPlan1.Name = "textInsPlan1";
			this.textInsPlan1.ReadOnly = true;
			this.textInsPlan1.Size = new System.Drawing.Size(158, 20);
			this.textInsPlan1.TabIndex = 172;
			// 
			// labelInsPlan1
			// 
			this.labelInsPlan1.Location = new System.Drawing.Point(6, 414);
			this.labelInsPlan1.Name = "labelInsPlan1";
			this.labelInsPlan1.Size = new System.Drawing.Size(55, 16);
			this.labelInsPlan1.TabIndex = 171;
			this.labelInsPlan1.Text = "InsPlan 1";
			this.labelInsPlan1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeDismissed
			// 
			this.textTimeDismissed.Location = new System.Drawing.Point(114, 353);
			this.textTimeDismissed.Name = "textTimeDismissed";
			this.textTimeDismissed.Size = new System.Drawing.Size(126, 20);
			this.textTimeDismissed.TabIndex = 170;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 355);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(106, 16);
			this.label7.TabIndex = 169;
			this.label7.Text = "Time Dismissed";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeSeated
			// 
			this.textTimeSeated.Location = new System.Drawing.Point(114, 333);
			this.textTimeSeated.Name = "textTimeSeated";
			this.textTimeSeated.Size = new System.Drawing.Size(126, 20);
			this.textTimeSeated.TabIndex = 168;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 335);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 16);
			this.label1.TabIndex = 166;
			this.label1.Text = "Time Seated";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeArrived
			// 
			this.textTimeArrived.Location = new System.Drawing.Point(114, 313);
			this.textTimeArrived.Name = "textTimeArrived";
			this.textTimeArrived.Size = new System.Drawing.Size(126, 20);
			this.textTimeArrived.TabIndex = 167;
			// 
			// labelTimeArrived
			// 
			this.labelTimeArrived.Location = new System.Drawing.Point(6, 315);
			this.labelTimeArrived.Name = "labelTimeArrived";
			this.labelTimeArrived.Size = new System.Drawing.Size(106, 16);
			this.labelTimeArrived.TabIndex = 165;
			this.labelTimeArrived.Text = "Time Arrived";
			this.labelTimeArrived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLabCase
			// 
			this.textLabCase.AcceptsReturn = true;
			this.textLabCase.Location = new System.Drawing.Point(63, 376);
			this.textLabCase.Multiline = true;
			this.textLabCase.Name = "textLabCase";
			this.textLabCase.ReadOnly = true;
			this.textLabCase.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textLabCase.Size = new System.Drawing.Size(177, 34);
			this.textLabCase.TabIndex = 162;
			// 
			// butLab
			// 
			this.butLab.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butLab.Autosize = true;
			this.butLab.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butLab.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butLab.CornerRadius = 4F;
			this.butLab.Location = new System.Drawing.Point(15, 376);
			this.butLab.Name = "butLab";
			this.butLab.Size = new System.Drawing.Size(46, 20);
			this.butLab.TabIndex = 161;
			this.butLab.Text = "Lab";
			this.butLab.Click += new System.EventHandler(this.butLab_Click);
			// 
			// butPickHyg
			// 
			this.butPickHyg.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPickHyg.Autosize = false;
			this.butPickHyg.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPickHyg.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPickHyg.CornerRadius = 2F;
			this.butPickHyg.Location = new System.Drawing.Point(222, 148);
			this.butPickHyg.Name = "butPickHyg";
			this.butPickHyg.Size = new System.Drawing.Size(18, 20);
			this.butPickHyg.TabIndex = 158;
			this.butPickHyg.Text = "...";
			this.butPickHyg.Click += new System.EventHandler(this.butPickHyg_Click);
			// 
			// butPickDentist
			// 
			this.butPickDentist.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPickDentist.Autosize = false;
			this.butPickDentist.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPickDentist.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPickDentist.CornerRadius = 2F;
			this.butPickDentist.Location = new System.Drawing.Point(222, 127);
			this.butPickDentist.Name = "butPickDentist";
			this.butPickDentist.Size = new System.Drawing.Size(18, 20);
			this.butPickDentist.TabIndex = 157;
			this.butPickDentist.Text = "...";
			this.butPickDentist.Click += new System.EventHandler(this.butPickDentist_Click);
			// 
			// gridFields
			// 
			this.gridFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridFields.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridFields.HasAddButton = false;
			this.gridFields.HasDropDowns = false;
			this.gridFields.HasMultilineHeaders = false;
			this.gridFields.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridFields.HeaderHeight = 15;
			this.gridFields.HScrollVisible = false;
			this.gridFields.Location = new System.Drawing.Point(21, 578);
			this.gridFields.Name = "gridFields";
			this.gridFields.ScrollValue = 0;
			this.gridFields.Size = new System.Drawing.Size(259, 118);
			this.gridFields.TabIndex = 159;
			this.gridFields.Title = "Appt Fields";
			this.gridFields.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridFields.TitleHeight = 18;
			this.gridFields.TranslationName = "FormApptEdit";
			this.gridFields.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFields_CellDoubleClick);
			// 
			// gridPatient
			// 
			this.gridPatient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridPatient.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridPatient.HasAddButton = false;
			this.gridPatient.HasDropDowns = false;
			this.gridPatient.HasMultilineHeaders = false;
			this.gridPatient.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridPatient.HeaderHeight = 15;
			this.gridPatient.HScrollVisible = false;
			this.gridPatient.Location = new System.Drawing.Point(282, 405);
			this.gridPatient.Name = "gridPatient";
			this.gridPatient.ScrollValue = 0;
			this.gridPatient.Size = new System.Drawing.Size(258, 291);
			this.gridPatient.TabIndex = 0;
			this.gridPatient.Title = "Patient Info";
			this.gridPatient.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridPatient.TitleHeight = 18;
			this.gridPatient.TranslationName = "TableApptPtInfo";
			this.gridPatient.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPatient_CellClick);
			this.gridPatient.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridPatient_MouseMove);
			// 
			// gridComm
			// 
			this.gridComm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridComm.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridComm.HasAddButton = false;
			this.gridComm.HasDropDowns = false;
			this.gridComm.HasMultilineHeaders = false;
			this.gridComm.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridComm.HeaderHeight = 15;
			this.gridComm.HScrollVisible = false;
			this.gridComm.Location = new System.Drawing.Point(542, 405);
			this.gridComm.Name = "gridComm";
			this.gridComm.ScrollValue = 0;
			this.gridComm.Size = new System.Drawing.Size(335, 291);
			this.gridComm.TabIndex = 1;
			this.gridComm.Title = "Communications Log - Appointment Scheduling";
			this.gridComm.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridComm.TitleHeight = 18;
			this.gridComm.TranslationName = "TableCommLog";
			this.gridComm.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridComm_CellDoubleClick);
			this.gridComm.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridComm_MouseMove);
			// 
			// gridProc
			// 
			this.gridProc.AllowSelection = false;
			this.gridProc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProc.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridProc.HasAddButton = false;
			this.gridProc.HasDropDowns = false;
			this.gridProc.HasMultilineHeaders = false;
			this.gridProc.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridProc.HeaderHeight = 15;
			this.gridProc.HScrollVisible = false;
			this.gridProc.Location = new System.Drawing.Point(434, 28);
			this.gridProc.Name = "gridProc";
			this.gridProc.ScrollValue = 0;
			this.gridProc.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProc.Size = new System.Drawing.Size(538, 375);
			this.gridProc.TabIndex = 139;
			this.gridProc.Title = "Procedures on this Appointment";
			this.gridProc.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridProc.TitleHeight = 18;
			this.gridProc.TranslationName = "TableApptProcs";
			this.gridProc.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProc_CellDoubleClick);
			this.gridProc.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProc_CellClick);
			// 
			// labelPlannedComplete
			// 
			this.labelPlannedComplete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPlannedComplete.Location = new System.Drawing.Point(664, 1);
			this.labelPlannedComplete.Name = "labelPlannedComplete";
			this.labelPlannedComplete.Size = new System.Drawing.Size(227, 26);
			this.labelPlannedComplete.TabIndex = 184;
			this.labelPlannedComplete.Text = "This planned appointment is attached\r\nto a completed appointment.";
			this.labelPlannedComplete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelPlannedComplete.Visible = false;
			// 
			// butPDF
			// 
			this.butPDF.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPDF.Autosize = true;
			this.butPDF.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPDF.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPDF.CornerRadius = 4F;
			this.butPDF.Location = new System.Drawing.Point(880, 457);
			this.butPDF.Name = "butPDF";
			this.butPDF.Size = new System.Drawing.Size(92, 24);
			this.butPDF.TabIndex = 161;
			this.butPDF.Text = "Notes PDF";
			this.butPDF.Visible = false;
			this.butPDF.Click += new System.EventHandler(this.butPDF_Click);
			// 
			// butComplete
			// 
			this.butComplete.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butComplete.Autosize = true;
			this.butComplete.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butComplete.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butComplete.CornerRadius = 4F;
			this.butComplete.Location = new System.Drawing.Point(880, 483);
			this.butComplete.Name = "butComplete";
			this.butComplete.Size = new System.Drawing.Size(92, 24);
			this.butComplete.TabIndex = 155;
			this.butComplete.Text = "Finish && Send";
			this.butComplete.Visible = false;
			this.butComplete.Click += new System.EventHandler(this.butComplete_Click);
			// 
			// butDeleteProc
			// 
			this.butDeleteProc.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDeleteProc.Autosize = true;
			this.butDeleteProc.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butDeleteProc.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butDeleteProc.CornerRadius = 4F;
			this.butDeleteProc.Image = global::OpenDental.Properties.Resources.deleteX;
			this.butDeleteProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteProc.Location = new System.Drawing.Point(434, 2);
			this.butDeleteProc.Name = "butDeleteProc";
			this.butDeleteProc.Size = new System.Drawing.Size(75, 24);
			this.butDeleteProc.TabIndex = 154;
			this.butDeleteProc.Text = "Delete";
			this.butDeleteProc.Click += new System.EventHandler(this.butDeleteProc_Click);
			// 
			// butAdd
			// 
			this.butAdd.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAdd.Autosize = true;
			this.butAdd.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAdd.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAdd.CornerRadius = 4F;
			this.butAdd.Image = global::OpenDental.Properties.Resources.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(510, 2);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 152;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(21, 469);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Appointment;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(260, 106);
			this.textNote.TabIndex = 142;
			this.textNote.Text = "";
			// 
			// butText
			// 
			this.butText.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butText.Autosize = true;
			this.butText.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butText.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butText.CornerRadius = 4F;
			this.butText.Image = global::OpenDental.Properties.Resources.Text;
			this.butText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butText.Location = new System.Drawing.Point(880, 431);
			this.butText.Name = "butText";
			this.butText.Size = new System.Drawing.Size(92, 24);
			this.butText.TabIndex = 143;
			this.butText.Text = "Text";
			this.butText.Click += new System.EventHandler(this.butText_Click);
			// 
			// butAddComm
			// 
			this.butAddComm.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAddComm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddComm.Autosize = true;
			this.butAddComm.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddComm.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddComm.CornerRadius = 4F;
			this.butAddComm.Image = global::OpenDental.Properties.Resources.commlog;
			this.butAddComm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddComm.Location = new System.Drawing.Point(880, 405);
			this.butAddComm.Name = "butAddComm";
			this.butAddComm.Size = new System.Drawing.Size(92, 24);
			this.butAddComm.TabIndex = 143;
			this.butAddComm.Text = "Co&mm";
			this.butAddComm.Click += new System.EventHandler(this.butAddComm_Click);
			// 
			// tbTime
			// 
			this.tbTime.BackColor = System.Drawing.SystemColors.Window;
			this.tbTime.Location = new System.Drawing.Point(4, 6);
			this.tbTime.Name = "tbTime";
			this.tbTime.ScrollValue = 150;
			this.tbTime.SelectedIndices = new int[0];
			this.tbTime.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.tbTime.Size = new System.Drawing.Size(15, 561);
			this.tbTime.TabIndex = 59;
			// 
			// butAudit
			// 
			this.butAudit.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAudit.Autosize = true;
			this.butAudit.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAudit.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAudit.CornerRadius = 4F;
			this.butAudit.Location = new System.Drawing.Point(880, 509);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(92, 24);
			this.butAudit.TabIndex = 125;
			this.butAudit.Text = "Audit Trail";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// butTask
			// 
			this.butTask.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTask.Autosize = true;
			this.butTask.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butTask.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butTask.CornerRadius = 4F;
			this.butTask.Location = new System.Drawing.Point(880, 535);
			this.butTask.Name = "butTask";
			this.butTask.Size = new System.Drawing.Size(92, 24);
			this.butTask.TabIndex = 124;
			this.butTask.Text = "To Task List";
			this.butTask.Click += new System.EventHandler(this.butTask_Click);
			// 
			// butDelete
			// 
			this.butDelete.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Autosize = true;
			this.butDelete.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butDelete.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butDelete.CornerRadius = 4F;
			this.butDelete.Image = global::OpenDental.Properties.Resources.deleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(880, 587);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(92, 24);
			this.butDelete.TabIndex = 123;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butPin
			// 
			this.butPin.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPin.Autosize = true;
			this.butPin.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPin.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPin.CornerRadius = 4F;
			this.butPin.Image = ((System.Drawing.Image)(resources.GetObject("butPin.Image")));
			this.butPin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPin.Location = new System.Drawing.Point(880, 561);
			this.butPin.Name = "butPin";
			this.butPin.Size = new System.Drawing.Size(92, 24);
			this.butPin.TabIndex = 122;
			this.butPin.Text = "&Pinboard";
			this.butPin.Click += new System.EventHandler(this.butPin_Click);
			// 
			// butOK
			// 
			this.butOK.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Autosize = true;
			this.butOK.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butOK.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butOK.CornerRadius = 4F;
			this.butOK.Location = new System.Drawing.Point(880, 640);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(92, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
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
			this.butCancel.Location = new System.Drawing.Point(880, 666);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(92, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAttachAll
			// 
			this.butAttachAll.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAttachAll.Autosize = true;
			this.butAttachAll.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAttachAll.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAttachAll.CornerRadius = 4F;
			this.butAttachAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAttachAll.Location = new System.Drawing.Point(586, 2);
			this.butAttachAll.Name = "butAttachAll";
			this.butAttachAll.Size = new System.Drawing.Size(75, 24);
			this.butAttachAll.TabIndex = 185;
			this.butAttachAll.Text = "Attach All";
			this.butAttachAll.Click += new System.EventHandler(this.butAttachAll_Click);
			// 
			// FormApptEdit
			// 
			this.ClientSize = new System.Drawing.Size(974, 698);
			this.Controls.Add(this.butAttachAll);
			this.Controls.Add(this.labelPlannedComplete);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.listQuickAdd);
			this.Controls.Add(this.labelQuickAdd);
			this.Controls.Add(this.butPDF);
			this.Controls.Add(this.gridFields);
			this.Controls.Add(this.butComplete);
			this.Controls.Add(this.butDeleteProc);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelApptNote);
			this.Controls.Add(this.butText);
			this.Controls.Add(this.butAddComm);
			this.Controls.Add(this.butSlider);
			this.Controls.Add(this.tbTime);
			this.Controls.Add(this.gridPatient);
			this.Controls.Add(this.gridComm);
			this.Controls.Add(this.gridProc);
			this.Controls.Add(this.butAudit);
			this.Controls.Add(this.butTask);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butPin);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Appointment";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormApptEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormApptEdit_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ODGrid gridPatient;
		private OpenDental.UI.ODGrid gridComm;
		private System.Windows.Forms.ComboBox comboConfirmed;
		private System.Windows.Forms.ComboBox comboUnschedStatus;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboStatus;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelStatus;
		private OpenDental.UI.Button butAudit;
		private OpenDental.UI.Button butTask;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butPin;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.CheckBox checkIsHygiene;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.ComboBox comboAssistant;
		private System.Windows.Forms.ComboBox comboProvHyg;
		private System.Windows.Forms.ComboBox comboProv;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox checkIsNewPatient;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ODGrid gridProc;
		private System.Windows.Forms.Button butSlider;
		private TableTimeBar tbTime;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textTime;
		private ODtextBox textNote;
		private System.Windows.Forms.Label labelApptNote;
		private OpenDental.UI.Button butAddComm;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ContextMenu contextMenuTimeArrived;
		private System.Windows.Forms.MenuItem menuItemArrivedNow;
		private System.Windows.Forms.ContextMenu contextMenuTimeSeated;
		private System.Windows.Forms.MenuItem menuItemSeatedNow;
		private System.Windows.Forms.ContextMenu contextMenuTimeDismissed;
		private System.Windows.Forms.MenuItem menuItemDismissedNow;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butDeleteProc;
		private OpenDental.UI.Button butComplete;
		private System.Windows.Forms.CheckBox checkTimeLocked;
		private OpenDental.UI.Button butPickHyg;
		private OpenDental.UI.Button butPickDentist;
		private OpenDental.UI.ODGrid gridFields;
		private System.Windows.Forms.TextBox textTimeAskedToArrive;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.Button butPDF;
		private System.Windows.Forms.ListBox listQuickAdd;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textRequirement;
		private UI.Button butRequirement;
		private UI.Button butInsPlan2;
		private UI.Button butInsPlan1;
		private System.Windows.Forms.TextBox textInsPlan2;
		private System.Windows.Forms.Label labelInsPlan2;
		private System.Windows.Forms.TextBox textInsPlan1;
		private System.Windows.Forms.Label labelInsPlan1;
		private System.Windows.Forms.TextBox textTimeDismissed;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textTimeSeated;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textTimeArrived;
		private System.Windows.Forms.Label labelTimeArrived;
		private System.Windows.Forms.TextBox textLabCase;
		private UI.Button butLab;
		private System.Windows.Forms.Label label9;
		private UI.Button butColorClear;
		private System.Windows.Forms.Button butColor;
		private UI.Button butText;
		private System.Windows.Forms.Label labelQuickAdd;
		private UI.Button butSyndromicObservations;
		private System.Windows.Forms.Label labelSyndromicObservations;
		private System.Windows.Forms.CheckBox checkASAP;
		private System.Windows.Forms.ComboBox comboApptType;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label labelPlannedComplete;
		private UI.Button butAttachAll;
	}
}
