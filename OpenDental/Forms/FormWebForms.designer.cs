namespace OpenDental{
	partial class FormWebForms {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebForms));
			this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
			this.menuItemSetup = new System.Windows.Forms.MenuItem();
			this.menuWebFormsRight = new System.Windows.Forms.ContextMenu();
			this.menuItemViewAllSheets = new System.Windows.Forms.MenuItem();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.butPickClinic = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxMulti();
			this.labelClinic = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.butToday = new OpenDental.UI.Button();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelStartDate = new System.Windows.Forms.Label();
			this.labelEndDate = new System.Windows.Forms.Label();
			this.textDateEnd = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.butRetrieve = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.ODGrid();
			this.butClose = new OpenDental.UI.Button();
			this.groupFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSetup});
			// 
			// menuItemSetup
			// 
			this.menuItemSetup.Index = 0;
			this.menuItemSetup.Text = "Setup";
			this.menuItemSetup.Click += new System.EventHandler(this.menuItemSetup_Click);
			// 
			// menuWebFormsRight
			// 
			this.menuWebFormsRight.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemViewAllSheets});
			this.menuWebFormsRight.Popup += new System.EventHandler(this.menuWebFormsRight_Popup);
			// 
			// menuItemViewAllSheets
			// 
			this.menuItemViewAllSheets.Index = 0;
			this.menuItemViewAllSheets.Text = "View this patient\'s forms";
			this.menuItemViewAllSheets.Click += new System.EventHandler(this.menuItemViewAllSheets_Click);
			// 
			// groupFilters
			// 
			this.groupFilters.Controls.Add(this.butPickClinic);
			this.groupFilters.Controls.Add(this.comboClinic);
			this.groupFilters.Controls.Add(this.labelClinic);
			this.groupFilters.Controls.Add(this.butRefresh);
			this.groupFilters.Controls.Add(this.butToday);
			this.groupFilters.Controls.Add(this.textDateStart);
			this.groupFilters.Controls.Add(this.labelStartDate);
			this.groupFilters.Controls.Add(this.labelEndDate);
			this.groupFilters.Controls.Add(this.textDateEnd);
			this.groupFilters.Location = new System.Drawing.Point(12, 2);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(495, 69);
			this.groupFilters.TabIndex = 238;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Show Retrieved Forms";
			// 
			// butPickClinic
			// 
			this.butPickClinic.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPickClinic.Autosize = true;
			this.butPickClinic.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPickClinic.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPickClinic.CornerRadius = 4F;
			this.butPickClinic.Location = new System.Drawing.Point(450, 42);
			this.butPickClinic.Name = "butPickClinic";
			this.butPickClinic.Size = new System.Drawing.Size(21, 21);
			this.butPickClinic.TabIndex = 5;
			this.butPickClinic.Text = "...";
			this.butPickClinic.Visible = false;
			this.butPickClinic.Click += new System.EventHandler(this.butPickClinic_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.ArraySelectedIndices = new int[0];
			this.comboClinic.BackColor = System.Drawing.SystemColors.Window;
			this.comboClinic.ComboDelimiter = OpenDental.UI.ComboMultiDelimiter.None;
			this.comboClinic.Items = ((System.Collections.ArrayList)(resources.GetObject("comboClinic.Items")));
			this.comboClinic.Location = new System.Drawing.Point(300, 42);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.SelectedIndices = ((System.Collections.ArrayList)(resources.GetObject("comboClinic.SelectedIndices")));
			this.comboClinic.Size = new System.Drawing.Size(149, 21);
			this.comboClinic.TabIndex = 4;
			this.comboClinic.Visible = false;
			this.comboClinic.SelectionChangeCommitted += new OpenDental.UI.ComboBoxMulti.SelectionChangeCommittedHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(240, 45);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(57, 14);
			this.labelClinic.TabIndex = 255;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelClinic.Visible = false;
			// 
			// butRefresh
			// 
			this.butRefresh.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRefresh.Autosize = true;
			this.butRefresh.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRefresh.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRefresh.CornerRadius = 4F;
			this.butRefresh.Location = new System.Drawing.Point(158, 39);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(77, 24);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butToday
			// 
			this.butToday.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butToday.Autosize = true;
			this.butToday.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butToday.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butToday.CornerRadius = 4F;
			this.butToday.Location = new System.Drawing.Point(158, 14);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(77, 24);
			this.butToday.TabIndex = 1;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// textDateStart
			// 
			this.textDateStart.BackColor = System.Drawing.SystemColors.Window;
			this.textDateStart.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textDateStart.Location = new System.Drawing.Point(75, 16);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 0;
			// 
			// labelStartDate
			// 
			this.labelStartDate.Location = new System.Drawing.Point(6, 19);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new System.Drawing.Size(69, 14);
			this.labelStartDate.TabIndex = 221;
			this.labelStartDate.Text = "Start Date";
			this.labelStartDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelEndDate
			// 
			this.labelEndDate.Location = new System.Drawing.Point(6, 44);
			this.labelEndDate.Name = "labelEndDate";
			this.labelEndDate.Size = new System.Drawing.Size(69, 14);
			this.labelEndDate.TabIndex = 222;
			this.labelEndDate.Text = "End Date";
			this.labelEndDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(75, 41);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(502, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(197, 36);
			this.label1.TabIndex = 239;
			this.label1.Text = "(All retrieved forms are automatically attached to the correct patient)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butRetrieve
			// 
			this.butRetrieve.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRetrieve.Autosize = true;
			this.butRetrieve.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRetrieve.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRetrieve.CornerRadius = 4F;
			this.butRetrieve.Location = new System.Drawing.Point(536, 8);
			this.butRetrieve.Name = "butRetrieve";
			this.butRetrieve.Size = new System.Drawing.Size(123, 24);
			this.butRetrieve.TabIndex = 0;
			this.butRetrieve.Text = "&Retrieve New Forms";
			this.butRetrieve.Click += new System.EventHandler(this.butRetrieve_Click);
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
			this.gridMain.HScrollVisible = false;
			this.gridMain.Location = new System.Drawing.Point(12, 77);
			this.gridMain.Name = "gridMain";
			this.gridMain.ScrollValue = 0;
			this.gridMain.Size = new System.Drawing.Size(647, 205);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Webforms";
			this.gridMain.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridMain.TitleHeight = 18;
			this.gridMain.TranslationName = "TableWebforms";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.CornerRadius = 4F;
			this.butClose.Location = new System.Drawing.Point(668, 258);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormWebForms
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(755, 315);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butRetrieve);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.MinimumSize = new System.Drawing.Size(771, 354);
			this.Name = "FormWebForms";
			this.Text = "Web Forms";
			this.Load += new System.EventHandler(this.FormWebForms_Load);
			this.groupFilters.ResumeLayout(false);
			this.groupFilters.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.ODGrid gridMain;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItemSetup;
		private OpenDental.UI.Button butRetrieve;
		private System.Windows.Forms.GroupBox groupFilters;
		private OpenDental.UI.Button butToday;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label labelStartDate;
		private System.Windows.Forms.Label labelEndDate;
		private ValidDate textDateEnd;
		private OpenDental.UI.Button butRefresh;
		private System.Windows.Forms.ContextMenu menuWebFormsRight;
		private System.Windows.Forms.MenuItem menuItemViewAllSheets;
		private System.Windows.Forms.Label label1;
		private UI.Button butPickClinic;
		private UI.ComboBoxMulti comboClinic;
		private System.Windows.Forms.Label labelClinic;
	}
}