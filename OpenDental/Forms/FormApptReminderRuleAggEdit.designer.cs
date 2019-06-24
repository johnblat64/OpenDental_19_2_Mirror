namespace OpenDental {
	partial class FormApptReminderRuleAggEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptReminderRuleAggEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textSMSAggShared = new OpenDental.ODtextBox();
			this.groupBoxSMSAggShared = new System.Windows.Forms.GroupBox();
			this.labelSMSAggShared = new System.Windows.Forms.Label();
			this.groupBoxSMSAggPerAppt = new System.Windows.Forms.GroupBox();
			this.labelSMSAggPerAppt = new System.Windows.Forms.Label();
			this.textSMSAggPerAppt = new OpenDental.ODtextBox();
			this.groupBoxEmailSubjAggShared = new System.Windows.Forms.GroupBox();
			this.labelEmailSubjAggShared = new System.Windows.Forms.Label();
			this.textEmailSubjAggShared = new OpenDental.ODtextBox();
			this.groupBoxEmailAggShared = new System.Windows.Forms.GroupBox();
			this.butEditEmail = new OpenDental.UI.Button();
			this.browserEmailBody = new System.Windows.Forms.WebBrowser();
			this.labelEmailAggShared = new System.Windows.Forms.Label();
			this.groupBoxEmailAggPerAppt = new System.Windows.Forms.GroupBox();
			this.labelEmailAggPerAppt = new System.Windows.Forms.Label();
			this.textEmailAggPerAppt = new OpenDental.ODtextBox();
			this.groupBoxTags = new System.Windows.Forms.GroupBox();
			this.labelTags = new System.Windows.Forms.Label();
			this.groupBoxSMSAggShared.SuspendLayout();
			this.groupBoxSMSAggPerAppt.SuspendLayout();
			this.groupBoxEmailSubjAggShared.SuspendLayout();
			this.groupBoxEmailAggShared.SuspendLayout();
			this.groupBoxEmailAggPerAppt.SuspendLayout();
			this.groupBoxTags.SuspendLayout();
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
			this.butOK.Location = new System.Drawing.Point(500, 750);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 6;
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
			this.butCancel.Location = new System.Drawing.Point(581, 750);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textSMSAggShared
			// 
			this.textSMSAggShared.AcceptsTab = true;
			this.textSMSAggShared.BackColor = System.Drawing.SystemColors.Window;
			this.textSMSAggShared.DetectLinksEnabled = false;
			this.textSMSAggShared.DetectUrls = false;
			this.textSMSAggShared.Location = new System.Drawing.Point(6, 39);
			this.textSMSAggShared.Name = "textSMSAggShared";
			this.textSMSAggShared.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textSMSAggShared.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSMSAggShared.Size = new System.Drawing.Size(615, 66);
			this.textSMSAggShared.TabIndex = 1;
			this.textSMSAggShared.Text = "";
			// 
			// groupBoxSMSAggShared
			// 
			this.groupBoxSMSAggShared.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSMSAggShared.Controls.Add(this.labelSMSAggShared);
			this.groupBoxSMSAggShared.Controls.Add(this.textSMSAggShared);
			this.groupBoxSMSAggShared.Location = new System.Drawing.Point(12, 87);
			this.groupBoxSMSAggShared.Name = "groupBoxSMSAggShared";
			this.groupBoxSMSAggShared.Size = new System.Drawing.Size(627, 110);
			this.groupBoxSMSAggShared.TabIndex = 8;
			this.groupBoxSMSAggShared.TabStop = false;
			this.groupBoxSMSAggShared.Text = "SMS Template";
			// 
			// labelSMSAggShared
			// 
			this.labelSMSAggShared.Location = new System.Drawing.Point(7, 20);
			this.labelSMSAggShared.Name = "labelSMSAggShared";
			this.labelSMSAggShared.Size = new System.Drawing.Size(471, 13);
			this.labelSMSAggShared.TabIndex = 9;
			this.labelSMSAggShared.Text = "The message body template. Used once per aggregate message.";
			// 
			// groupBoxSMSAggPerAppt
			// 
			this.groupBoxSMSAggPerAppt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSMSAggPerAppt.Controls.Add(this.labelSMSAggPerAppt);
			this.groupBoxSMSAggPerAppt.Controls.Add(this.textSMSAggPerAppt);
			this.groupBoxSMSAggPerAppt.Location = new System.Drawing.Point(12, 199);
			this.groupBoxSMSAggPerAppt.Name = "groupBoxSMSAggPerAppt";
			this.groupBoxSMSAggPerAppt.Size = new System.Drawing.Size(627, 110);
			this.groupBoxSMSAggPerAppt.TabIndex = 10;
			this.groupBoxSMSAggPerAppt.TabStop = false;
			this.groupBoxSMSAggPerAppt.Text = "SMS Template Per Appointment";
			// 
			// labelSMSAggPerAppt
			// 
			this.labelSMSAggPerAppt.Location = new System.Drawing.Point(7, 20);
			this.labelSMSAggPerAppt.Name = "labelSMSAggPerAppt";
			this.labelSMSAggPerAppt.Size = new System.Drawing.Size(471, 13);
			this.labelSMSAggPerAppt.TabIndex = 11;
			this.labelSMSAggPerAppt.Text = "A single appointment template. Formats each appointment listed in the aggregate m" +
    "essage.";
			// 
			// textSMSAggPerAppt
			// 
			this.textSMSAggPerAppt.AcceptsTab = true;
			this.textSMSAggPerAppt.BackColor = System.Drawing.SystemColors.Window;
			this.textSMSAggPerAppt.DetectLinksEnabled = false;
			this.textSMSAggPerAppt.DetectUrls = false;
			this.textSMSAggPerAppt.Location = new System.Drawing.Point(6, 39);
			this.textSMSAggPerAppt.Name = "textSMSAggPerAppt";
			this.textSMSAggPerAppt.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textSMSAggPerAppt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSMSAggPerAppt.Size = new System.Drawing.Size(615, 66);
			this.textSMSAggPerAppt.TabIndex = 2;
			this.textSMSAggPerAppt.Text = "";
			// 
			// groupBoxEmailSubjAggShared
			// 
			this.groupBoxEmailSubjAggShared.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxEmailSubjAggShared.Controls.Add(this.labelEmailSubjAggShared);
			this.groupBoxEmailSubjAggShared.Controls.Add(this.textEmailSubjAggShared);
			this.groupBoxEmailSubjAggShared.Location = new System.Drawing.Point(12, 311);
			this.groupBoxEmailSubjAggShared.Name = "groupBoxEmailSubjAggShared";
			this.groupBoxEmailSubjAggShared.Size = new System.Drawing.Size(627, 66);
			this.groupBoxEmailSubjAggShared.TabIndex = 12;
			this.groupBoxEmailSubjAggShared.TabStop = false;
			this.groupBoxEmailSubjAggShared.Text = "E-mail Subject";
			// 
			// labelEmailSubjAggShared
			// 
			this.labelEmailSubjAggShared.Location = new System.Drawing.Point(7, 20);
			this.labelEmailSubjAggShared.Name = "labelEmailSubjAggShared";
			this.labelEmailSubjAggShared.Size = new System.Drawing.Size(471, 13);
			this.labelEmailSubjAggShared.TabIndex = 13;
			this.labelEmailSubjAggShared.Text = "The subject heading template.";
			// 
			// textEmailSubjAggShared
			// 
			this.textEmailSubjAggShared.AcceptsTab = true;
			this.textEmailSubjAggShared.BackColor = System.Drawing.SystemColors.Window;
			this.textEmailSubjAggShared.DetectLinksEnabled = false;
			this.textEmailSubjAggShared.DetectUrls = false;
			this.textEmailSubjAggShared.Location = new System.Drawing.Point(6, 39);
			this.textEmailSubjAggShared.Name = "textEmailSubjAggShared";
			this.textEmailSubjAggShared.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textEmailSubjAggShared.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textEmailSubjAggShared.Size = new System.Drawing.Size(615, 20);
			this.textEmailSubjAggShared.TabIndex = 3;
			this.textEmailSubjAggShared.Text = "";
			// 
			// groupBoxEmailAggShared
			// 
			this.groupBoxEmailAggShared.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxEmailAggShared.Controls.Add(this.butEditEmail);
			this.groupBoxEmailAggShared.Controls.Add(this.browserEmailBody);
			this.groupBoxEmailAggShared.Controls.Add(this.labelEmailAggShared);
			this.groupBoxEmailAggShared.Location = new System.Drawing.Point(12, 383);
			this.groupBoxEmailAggShared.Name = "groupBoxEmailAggShared";
			this.groupBoxEmailAggShared.Size = new System.Drawing.Size(627, 245);
			this.groupBoxEmailAggShared.TabIndex = 14;
			this.groupBoxEmailAggShared.TabStop = false;
			this.groupBoxEmailAggShared.Text = "E-mail Template";
			// 
			// butEditEmail
			// 
			this.butEditEmail.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butEditEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditEmail.Autosize = true;
			this.butEditEmail.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butEditEmail.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butEditEmail.CornerRadius = 4F;
			this.butEditEmail.Location = new System.Drawing.Point(546, 214);
			this.butEditEmail.Name = "butEditEmail";
			this.butEditEmail.Size = new System.Drawing.Size(75, 26);
			this.butEditEmail.TabIndex = 127;
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
			this.browserEmailBody.Location = new System.Drawing.Point(10, 36);
			this.browserEmailBody.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmailBody.Name = "browserEmailBody";
			this.browserEmailBody.Size = new System.Drawing.Size(611, 174);
			this.browserEmailBody.TabIndex = 115;
			this.browserEmailBody.WebBrowserShortcutsEnabled = false;
			// 
			// labelEmailAggShared
			// 
			this.labelEmailAggShared.Location = new System.Drawing.Point(7, 20);
			this.labelEmailAggShared.Name = "labelEmailAggShared";
			this.labelEmailAggShared.Size = new System.Drawing.Size(471, 13);
			this.labelEmailAggShared.TabIndex = 16;
			this.labelEmailAggShared.Text = "The message body template. Used once per aggregate message.";
			// 
			// groupBoxEmailAggPerAppt
			// 
			this.groupBoxEmailAggPerAppt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxEmailAggPerAppt.Controls.Add(this.labelEmailAggPerAppt);
			this.groupBoxEmailAggPerAppt.Controls.Add(this.textEmailAggPerAppt);
			this.groupBoxEmailAggPerAppt.Location = new System.Drawing.Point(12, 631);
			this.groupBoxEmailAggPerAppt.Name = "groupBoxEmailAggPerAppt";
			this.groupBoxEmailAggPerAppt.Size = new System.Drawing.Size(627, 110);
			this.groupBoxEmailAggPerAppt.TabIndex = 16;
			this.groupBoxEmailAggPerAppt.TabStop = false;
			this.groupBoxEmailAggPerAppt.Text = "E-mail Template Per Appointment";
			// 
			// labelEmailAggPerAppt
			// 
			this.labelEmailAggPerAppt.Location = new System.Drawing.Point(7, 13);
			this.labelEmailAggPerAppt.Name = "labelEmailAggPerAppt";
			this.labelEmailAggPerAppt.Size = new System.Drawing.Size(471, 26);
			this.labelEmailAggPerAppt.TabIndex = 17;
			this.labelEmailAggPerAppt.Text = "A single appointment template. Formats each appointment listed in the aggregate m" +
    "essage.";
			this.labelEmailAggPerAppt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textEmailAggPerAppt
			// 
			this.textEmailAggPerAppt.AcceptsTab = true;
			this.textEmailAggPerAppt.BackColor = System.Drawing.SystemColors.Window;
			this.textEmailAggPerAppt.DetectLinksEnabled = false;
			this.textEmailAggPerAppt.DetectUrls = false;
			this.textEmailAggPerAppt.Location = new System.Drawing.Point(6, 39);
			this.textEmailAggPerAppt.Name = "textEmailAggPerAppt";
			this.textEmailAggPerAppt.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textEmailAggPerAppt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textEmailAggPerAppt.Size = new System.Drawing.Size(615, 66);
			this.textEmailAggPerAppt.TabIndex = 5;
			this.textEmailAggPerAppt.Text = "";
			// 
			// groupBoxTags
			// 
			this.groupBoxTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxTags.Controls.Add(this.labelTags);
			this.groupBoxTags.Location = new System.Drawing.Point(12, 12);
			this.groupBoxTags.Name = "groupBoxTags";
			this.groupBoxTags.Size = new System.Drawing.Size(628, 73);
			this.groupBoxTags.TabIndex = 18;
			this.groupBoxTags.TabStop = false;
			this.groupBoxTags.Text = "Template Replacement Tags";
			// 
			// labelTags
			// 
			this.labelTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTags.Location = new System.Drawing.Point(3, 16);
			this.labelTags.Name = "labelTags";
			this.labelTags.Size = new System.Drawing.Size(622, 54);
			this.labelTags.TabIndex = 19;
			this.labelTags.Text = "Use template tags to create dynamic messages.";
			// 
			// FormApptReminderRuleAggEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(668, 786);
			this.Controls.Add(this.groupBoxTags);
			this.Controls.Add(this.groupBoxEmailAggPerAppt);
			this.Controls.Add(this.groupBoxEmailAggShared);
			this.Controls.Add(this.groupBoxEmailSubjAggShared);
			this.Controls.Add(this.groupBoxSMSAggPerAppt);
			this.Controls.Add(this.groupBoxSMSAggShared);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptReminderRuleAggEdit";
			this.Text = "Aggregate Messages Edit";
			this.Load += new System.EventHandler(this.FormApptReminderRuleEdit_Load);
			this.groupBoxSMSAggShared.ResumeLayout(false);
			this.groupBoxSMSAggPerAppt.ResumeLayout(false);
			this.groupBoxEmailSubjAggShared.ResumeLayout(false);
			this.groupBoxEmailAggShared.ResumeLayout(false);
			this.groupBoxEmailAggPerAppt.ResumeLayout(false);
			this.groupBoxTags.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ODtextBox textSMSAggShared;
		private System.Windows.Forms.GroupBox groupBoxSMSAggShared;
		private System.Windows.Forms.GroupBox groupBoxSMSAggPerAppt;
		private ODtextBox textSMSAggPerAppt;
		private System.Windows.Forms.GroupBox groupBoxEmailSubjAggShared;
		private ODtextBox textEmailSubjAggShared;
		private System.Windows.Forms.GroupBox groupBoxEmailAggShared;
		private System.Windows.Forms.GroupBox groupBoxEmailAggPerAppt;
		private ODtextBox textEmailAggPerAppt;
		private System.Windows.Forms.Label labelSMSAggPerAppt;
		private System.Windows.Forms.Label labelEmailSubjAggShared;
		private System.Windows.Forms.Label labelEmailAggShared;
		private System.Windows.Forms.Label labelEmailAggPerAppt;
		private System.Windows.Forms.Label labelSMSAggShared;
		private System.Windows.Forms.GroupBox groupBoxTags;
		private System.Windows.Forms.Label labelTags;
		private System.Windows.Forms.WebBrowser browserEmailBody;
		private UI.Button butEditEmail;
	}
}