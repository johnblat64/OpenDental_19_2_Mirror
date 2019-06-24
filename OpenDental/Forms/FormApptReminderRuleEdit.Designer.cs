namespace OpenDental {
	partial class FormApptReminderRuleEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptReminderRuleEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.textTemplateSms = new System.Windows.Forms.RichTextBox();
			this.labelLeadTime = new System.Windows.Forms.Label();
			this.butOk = new OpenDental.UI.Button();
			this.gridPriorities = new OpenDental.UI.ODGrid();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.checkSendAll = new System.Windows.Forms.CheckBox();
			this.labelTags = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textTemplateSubject = new System.Windows.Forms.RichTextBox();
			this.groupBox12 = new System.Windows.Forms.GroupBox();
			this.textDaysWithin = new OpenDental.ValidNumber();
			this.labelDaysWithin = new System.Windows.Forms.Label();
			this.labelDoNotSendWithin = new System.Windows.Forms.Label();
			this.radioAfterAppt = new System.Windows.Forms.RadioButton();
			this.radioBeforeAppt = new System.Windows.Forms.RadioButton();
			this.textDays = new OpenDental.ValidNum();
			this.textHoursWithin = new OpenDental.ValidNumber();
			this.label1 = new System.Windows.Forms.Label();
			this.textHours = new OpenDental.ValidNum();
			this.labelHoursWithin = new System.Windows.Forms.Label();
			this.groupSendOrder = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.butEditEmail = new OpenDental.UI.Button();
			this.browserEmailBody = new System.Windows.Forms.WebBrowser();
			this.labelRuleType = new System.Windows.Forms.Label();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.butAdvanced = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox12.SuspendLayout();
			this.groupSendOrder.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Autosize = true;
			this.butCancel.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCancel.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCancel.CornerRadius = 4F;
			this.butCancel.Location = new System.Drawing.Point(656, 661);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textTemplateSms
			// 
			this.textTemplateSms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTemplateSms.Location = new System.Drawing.Point(6, 17);
			this.textTemplateSms.Name = "textTemplateSms";
			this.textTemplateSms.Size = new System.Drawing.Size(695, 54);
			this.textTemplateSms.TabIndex = 69;
			this.textTemplateSms.Text = "";
			// 
			// labelLeadTime
			// 
			this.labelLeadTime.Location = new System.Drawing.Point(71, 48);
			this.labelLeadTime.Name = "labelLeadTime";
			this.labelLeadTime.Size = new System.Drawing.Size(43, 21);
			this.labelLeadTime.TabIndex = 15;
			this.labelLeadTime.Text = "Hours";
			this.labelLeadTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butOk
			// 
			this.butOk.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Autosize = true;
			this.butOk.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butOk.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butOk.CornerRadius = 4F;
			this.butOk.Location = new System.Drawing.Point(574, 661);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(76, 26);
			this.butOk.TabIndex = 124;
			this.butOk.Text = "&OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// gridPriorities
			// 
			this.gridPriorities.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridPriorities.HasAddButton = false;
			this.gridPriorities.HasDropDowns = false;
			this.gridPriorities.HasMultilineHeaders = false;
			this.gridPriorities.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridPriorities.HeaderHeight = 15;
			this.gridPriorities.HScrollVisible = false;
			this.gridPriorities.Location = new System.Drawing.Point(42, 19);
			this.gridPriorities.Name = "gridPriorities";
			this.gridPriorities.ScrollValue = 0;
			this.gridPriorities.Size = new System.Drawing.Size(359, 96);
			this.gridPriorities.TabIndex = 106;
			this.gridPriorities.Title = "Contact Methods";
			this.gridPriorities.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridPriorities.TitleHeight = 18;
			this.gridPriorities.TranslationName = "TableContactMethods";
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butUp.Autosize = false;
			this.butUp.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butUp.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butUp.CornerRadius = 4F;
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.Location = new System.Drawing.Point(6, 19);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(30, 30);
			this.butUp.TabIndex = 103;
			this.butUp.UseVisualStyleBackColor = true;
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDown.Autosize = false;
			this.butDown.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butDown.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butDown.CornerRadius = 4F;
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.Location = new System.Drawing.Point(6, 55);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(30, 30);
			this.butDown.TabIndex = 104;
			this.butDown.UseVisualStyleBackColor = true;
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// checkSendAll
			// 
			this.checkSendAll.Location = new System.Drawing.Point(42, 119);
			this.checkSendAll.Name = "checkSendAll";
			this.checkSendAll.Size = new System.Drawing.Size(359, 18);
			this.checkSendAll.TabIndex = 105;
			this.checkSendAll.Text = "Send All - If available, send text AND email.";
			this.checkSendAll.UseVisualStyleBackColor = true;
			this.checkSendAll.CheckedChanged += new System.EventHandler(this.checkSendAll_CheckedChanged);
			// 
			// labelTags
			// 
			this.labelTags.Location = new System.Drawing.Point(3, 16);
			this.labelTags.Name = "labelTags";
			this.labelTags.Size = new System.Drawing.Size(698, 37);
			this.labelTags.TabIndex = 110;
			this.labelTags.Text = "Use template tags to create dynamic messages.";
			// 
			// butDelete
			// 
			this.butDelete.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Autosize = true;
			this.butDelete.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butDelete.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butDelete.CornerRadius = 4F;
			this.butDelete.Image = global::OpenDental.Properties.Resources.deleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 661);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 26);
			this.butDelete.TabIndex = 111;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textTemplateSubject
			// 
			this.textTemplateSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTemplateSubject.Location = new System.Drawing.Point(6, 17);
			this.textTemplateSubject.Name = "textTemplateSubject";
			this.textTemplateSubject.Size = new System.Drawing.Size(701, 22);
			this.textTemplateSubject.TabIndex = 113;
			this.textTemplateSubject.Text = "";
			// 
			// groupBox12
			// 
			this.groupBox12.Controls.Add(this.textDaysWithin);
			this.groupBox12.Controls.Add(this.labelDaysWithin);
			this.groupBox12.Controls.Add(this.labelDoNotSendWithin);
			this.groupBox12.Controls.Add(this.radioAfterAppt);
			this.groupBox12.Controls.Add(this.radioBeforeAppt);
			this.groupBox12.Controls.Add(this.textDays);
			this.groupBox12.Controls.Add(this.textHoursWithin);
			this.groupBox12.Controls.Add(this.label1);
			this.groupBox12.Controls.Add(this.textHours);
			this.groupBox12.Controls.Add(this.labelLeadTime);
			this.groupBox12.Controls.Add(this.labelHoursWithin);
			this.groupBox12.Location = new System.Drawing.Point(18, 28);
			this.groupBox12.Name = "groupBox12";
			this.groupBox12.Size = new System.Drawing.Size(496, 77);
			this.groupBox12.TabIndex = 101;
			this.groupBox12.TabStop = false;
			this.groupBox12.Text = "Send Time";
			// 
			// textDaysWithin
			// 
			this.textDaysWithin.Location = new System.Drawing.Point(279, 46);
			this.textDaysWithin.MaxVal = 366;
			this.textDaysWithin.MinVal = 0;
			this.textDaysWithin.Name = "textDaysWithin";
			this.textDaysWithin.Size = new System.Drawing.Size(51, 20);
			this.textDaysWithin.TabIndex = 24;
			this.textDaysWithin.TextChanged += new System.EventHandler(this.textDoNotSendWithin_TextChanged);
			// 
			// labelDaysWithin
			// 
			this.labelDaysWithin.Location = new System.Drawing.Point(332, 45);
			this.labelDaysWithin.Name = "labelDaysWithin";
			this.labelDaysWithin.Size = new System.Drawing.Size(43, 21);
			this.labelDaysWithin.TabIndex = 23;
			this.labelDaysWithin.Text = "Days";
			this.labelDaysWithin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDoNotSendWithin
			// 
			this.labelDoNotSendWithin.Location = new System.Drawing.Point(261, 13);
			this.labelDoNotSendWithin.Name = "labelDoNotSendWithin";
			this.labelDoNotSendWithin.Size = new System.Drawing.Size(221, 28);
			this.labelDoNotSendWithin.TabIndex = 17;
			this.labelDoNotSendWithin.Text = "Do not send within _____________ of appointment";
			// 
			// radioAfterAppt
			// 
			this.radioAfterAppt.Location = new System.Drawing.Point(120, 47);
			this.radioAfterAppt.Name = "radioAfterAppt";
			this.radioAfterAppt.Size = new System.Drawing.Size(127, 20);
			this.radioAfterAppt.TabIndex = 20;
			this.radioAfterAppt.TabStop = true;
			this.radioAfterAppt.Text = "After appointment";
			this.radioAfterAppt.CheckedChanged += new System.EventHandler(this.radioBeforeAfterAppt_CheckedChanged);
			// 
			// radioBeforeAppt
			// 
			this.radioBeforeAppt.Location = new System.Drawing.Point(120, 25);
			this.radioBeforeAppt.Name = "radioBeforeAppt";
			this.radioBeforeAppt.Size = new System.Drawing.Size(127, 20);
			this.radioBeforeAppt.TabIndex = 19;
			this.radioBeforeAppt.TabStop = true;
			this.radioBeforeAppt.Text = "Before appointment";
			this.radioBeforeAppt.CheckedChanged += new System.EventHandler(this.radioBeforeAfterAppt_CheckedChanged);
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(18, 23);
			this.textDays.MaxVal = 366;
			this.textDays.MinVal = 0;
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(51, 20);
			this.textDays.TabIndex = 18;
			// 
			// textHoursWithin
			// 
			this.textHoursWithin.Location = new System.Drawing.Point(375, 47);
			this.textHoursWithin.MaxVal = 23;
			this.textHoursWithin.MinVal = 0;
			this.textHoursWithin.Name = "textHoursWithin";
			this.textHoursWithin.Size = new System.Drawing.Size(51, 20);
			this.textHoursWithin.TabIndex = 22;
			this.textHoursWithin.TextChanged += new System.EventHandler(this.textDoNotSendWithin_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(71, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 21);
			this.label1.TabIndex = 17;
			this.label1.Text = "Days";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textHours
			// 
			this.textHours.Location = new System.Drawing.Point(18, 49);
			this.textHours.MaxVal = 23;
			this.textHours.MinVal = 0;
			this.textHours.Name = "textHours";
			this.textHours.Size = new System.Drawing.Size(51, 20);
			this.textHours.TabIndex = 16;
			// 
			// labelHoursWithin
			// 
			this.labelHoursWithin.Location = new System.Drawing.Point(428, 46);
			this.labelHoursWithin.Name = "labelHoursWithin";
			this.labelHoursWithin.Size = new System.Drawing.Size(43, 21);
			this.labelHoursWithin.TabIndex = 21;
			this.labelHoursWithin.Text = "Hours";
			this.labelHoursWithin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupSendOrder
			// 
			this.groupSendOrder.Controls.Add(this.gridPriorities);
			this.groupSendOrder.Controls.Add(this.checkSendAll);
			this.groupSendOrder.Controls.Add(this.butDown);
			this.groupSendOrder.Controls.Add(this.butUp);
			this.groupSendOrder.Location = new System.Drawing.Point(18, 110);
			this.groupSendOrder.Name = "groupSendOrder";
			this.groupSendOrder.Size = new System.Drawing.Size(415, 140);
			this.groupSendOrder.TabIndex = 114;
			this.groupSendOrder.TabStop = false;
			this.groupSendOrder.Text = "Send Order";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this.labelTags);
			this.groupBox2.Location = new System.Drawing.Point(18, 599);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(713, 56);
			this.groupBox2.TabIndex = 115;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Template Replacement Tags";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.textTemplateSms);
			this.groupBox3.Location = new System.Drawing.Point(18, 259);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(707, 77);
			this.groupBox3.TabIndex = 119;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Text Message";
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.butEditEmail);
			this.groupBox4.Controls.Add(this.browserEmailBody);
			this.groupBox4.Controls.Add(this.textTemplateSubject);
			this.groupBox4.Location = new System.Drawing.Point(18, 336);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(713, 257);
			this.groupBox4.TabIndex = 120;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Email Subject and Body";
			// 
			// butEditEmail
			// 
			this.butEditEmail.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butEditEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditEmail.Autosize = true;
			this.butEditEmail.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butEditEmail.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butEditEmail.CornerRadius = 4F;
			this.butEditEmail.Location = new System.Drawing.Point(632, 225);
			this.butEditEmail.Name = "butEditEmail";
			this.butEditEmail.Size = new System.Drawing.Size(75, 26);
			this.butEditEmail.TabIndex = 126;
			this.butEditEmail.Text = "&Edit";
			this.butEditEmail.UseVisualStyleBackColor = true;
			this.butEditEmail.Click += new System.EventHandler(this.butEditEmail_Click);
			// 
			// browserEmailBody
			// 
			this.browserEmailBody.AllowWebBrowserDrop = false;
			this.browserEmailBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.browserEmailBody.Location = new System.Drawing.Point(6, 42);
			this.browserEmailBody.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmailBody.Name = "browserEmailBody";
			this.browserEmailBody.Size = new System.Drawing.Size(701, 179);
			this.browserEmailBody.TabIndex = 114;
			this.browserEmailBody.WebBrowserShortcutsEnabled = false;
			// 
			// labelRuleType
			// 
			this.labelRuleType.Location = new System.Drawing.Point(297, 5);
			this.labelRuleType.Name = "labelRuleType";
			this.labelRuleType.Size = new System.Drawing.Size(282, 21);
			this.labelRuleType.TabIndex = 16;
			this.labelRuleType.Text = "labelRuleType";
			this.labelRuleType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkEnabled
			// 
			this.checkEnabled.Location = new System.Drawing.Point(28, 7);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(90, 18);
			this.checkEnabled.TabIndex = 107;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// butAdvanced
			// 
			this.butAdvanced.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdvanced.Autosize = true;
			this.butAdvanced.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAdvanced.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAdvanced.CornerRadius = 4F;
			this.butAdvanced.Location = new System.Drawing.Point(287, 661);
			this.butAdvanced.Name = "butAdvanced";
			this.butAdvanced.Size = new System.Drawing.Size(75, 26);
			this.butAdvanced.TabIndex = 125;
			this.butAdvanced.Text = "&Advanced";
			this.butAdvanced.UseVisualStyleBackColor = true;
			this.butAdvanced.Click += new System.EventHandler(this.butAdvanced_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(163, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 21);
			this.label2.TabIndex = 126;
			this.label2.Text = "Reminder Rule Type:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormApptReminderRuleEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 696);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelRuleType);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butAdvanced);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupSendOrder);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.groupBox12);
			this.Controls.Add(this.groupBox4);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptReminderRuleEdit";
			this.Text = "Appointment Reminder Rule";
			this.Load += new System.EventHandler(this.FormApptReminderRuleEdit_Load);
			this.groupBox12.ResumeLayout(false);
			this.groupBox12.PerformLayout();
			this.groupSendOrder.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butCancel;
		private System.Windows.Forms.RichTextBox textTemplateSms;
		private System.Windows.Forms.Label labelLeadTime;
		private UI.Button butOk;
		private UI.ODGrid gridPriorities;
		private UI.Button butUp;
		private UI.Button butDown;
		private System.Windows.Forms.CheckBox checkSendAll;
		private System.Windows.Forms.Label labelTags;
		private UI.Button butDelete;
		private System.Windows.Forms.RichTextBox textTemplateSubject;
		private System.Windows.Forms.GroupBox groupBox12;
		private System.Windows.Forms.GroupBox groupSendOrder;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label labelRuleType;
		private ValidNum textHours;
		private ValidNum textDays;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkEnabled;
		private UI.Button butAdvanced;
		private System.Windows.Forms.RadioButton radioAfterAppt;
		private System.Windows.Forms.RadioButton radioBeforeAppt;
		private ValidNumber textDaysWithin;
		private System.Windows.Forms.Label labelDaysWithin;
		private System.Windows.Forms.Label labelDoNotSendWithin;
		private ValidNumber textHoursWithin;
		private System.Windows.Forms.Label labelHoursWithin;
		private System.Windows.Forms.WebBrowser browserEmailBody;
		private UI.Button butEditEmail;
		private System.Windows.Forms.Label label2;
	}
}