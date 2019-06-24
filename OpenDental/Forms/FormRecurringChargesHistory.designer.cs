namespace OpenDental{
	partial class FormRecurringChargesHistory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecurringChargesHistory));
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.ODGrid();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemGoTo = new System.Windows.Forms.MenuItem();
			this.menuItemOpenPayment = new System.Windows.Forms.MenuItem();
			this.menuItemViewError = new System.Windows.Forms.MenuItem();
			this.menuItemDeletePending = new System.Windows.Forms.MenuItem();
			this.butRefresh = new OpenDental.UI.Button();
			this.datePicker = new OpenDental.UI.ODDateRangePicker();
			this.groupView = new System.Windows.Forms.GroupBox();
			this.comboAutomated = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboStatuses = new OpenDental.UI.ComboBoxMulti();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupView.SuspendLayout();
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
			this.butClose.Location = new System.Drawing.Point(950, 680);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.AutoScroll = true;
			this.gridMain.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridMain.HasDropDowns = false;
			this.gridMain.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridMain.HeaderHeight = 15;
			this.gridMain.HScrollVisible = false;
			this.gridMain.Location = new System.Drawing.Point(20, 88);
			this.gridMain.Name = "gridMain";
			this.gridMain.ScrollValue = 0;
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1005, 584);
			this.gridMain.TabIndex = 30;
			this.gridMain.Title = "Recurring Charges";
			this.gridMain.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridMain.TitleHeight = 18;
			this.gridMain.TranslationName = "TableRecurring";
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemGoTo,
            this.menuItemOpenPayment,
            this.menuItemViewError,
            this.menuItemDeletePending});
			this.contextMenu.Popup += new System.EventHandler(this.contextMenu_Popup);
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Index = 0;
			this.menuItemGoTo.Text = "Go To Account";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// menuItemOpenPayment
			// 
			this.menuItemOpenPayment.Index = 1;
			this.menuItemOpenPayment.Text = "Open Payment";
			this.menuItemOpenPayment.Click += new System.EventHandler(this.menuItemOpenPayment_Click);
			// 
			// menuItemViewError
			// 
			this.menuItemViewError.Index = 2;
			this.menuItemViewError.Text = "View Error Message";
			this.menuItemViewError.Click += new System.EventHandler(this.menuItemViewError_Click);
			// 
			// menuItemDeletePending
			// 
			this.menuItemDeletePending.Index = 3;
			this.menuItemDeletePending.Text = "Delete Pending Charge";
			this.menuItemDeletePending.Click += new System.EventHandler(this.menuItemDeletePending_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRefresh.Autosize = true;
			this.butRefresh.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRefresh.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRefresh.CornerRadius = 4F;
			this.butRefresh.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butRefresh.Location = new System.Drawing.Point(694, 18);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 32;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.Color.Transparent;
			this.datePicker.DefaultDateTimeFrom = new System.DateTime(2018, 1, 1, 0, 0, 0, 0);
			this.datePicker.DefaultDateTimeTo = new System.DateTime(2018, 7, 9, 0, 0, 0, 0);
			this.datePicker.EnableWeekButtons = false;
			this.datePicker.IsVertical = true;
			this.datePicker.Location = new System.Drawing.Point(6, 18);
			this.datePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.datePicker.MinimumSize = new System.Drawing.Size(0, 22);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(180, 46);
			this.datePicker.TabIndex = 33;
			this.datePicker.CalendarClosed += new OpenDental.UI.CalendarClosedHandler(this.FilterChanged);
			this.datePicker.Leave += new System.EventHandler(this.FilterChanged);
			// 
			// groupView
			// 
			this.groupView.Controls.Add(this.comboAutomated);
			this.groupView.Controls.Add(this.label2);
			this.groupView.Controls.Add(this.label1);
			this.groupView.Controls.Add(this.comboStatuses);
			this.groupView.Controls.Add(this.comboClinics);
			this.groupView.Controls.Add(this.butRefresh);
			this.groupView.Controls.Add(this.datePicker);
			this.groupView.Location = new System.Drawing.Point(20, 12);
			this.groupView.Name = "groupView";
			this.groupView.Size = new System.Drawing.Size(776, 70);
			this.groupView.TabIndex = 36;
			this.groupView.TabStop = false;
			this.groupView.Text = "View";
			// 
			// comboAutomated
			// 
			this.comboAutomated.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAutomated.FormattingEnabled = true;
			this.comboAutomated.Location = new System.Drawing.Point(506, 20);
			this.comboAutomated.MaxDropDownItems = 30;
			this.comboAutomated.Name = "comboAutomated";
			this.comboAutomated.Size = new System.Drawing.Size(182, 21);
			this.comboAutomated.TabIndex = 42;
			this.comboAutomated.SelectionChangeCommitted += new System.EventHandler(this.FilterChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(439, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 16);
			this.label2.TabIndex = 41;
			this.label2.Text = "Automated";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(192, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 16);
			this.label1.TabIndex = 39;
			this.label1.Text = "Status";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatuses
			// 
			this.comboStatuses.ArraySelectedIndices = new int[0];
			this.comboStatuses.BackColor = System.Drawing.SystemColors.Window;
			this.comboStatuses.Items = ((System.Collections.ArrayList)(resources.GetObject("comboStatuses.Items")));
			this.comboStatuses.Location = new System.Drawing.Point(245, 19);
			this.comboStatuses.Name = "comboStatuses";
			this.comboStatuses.SelectedIndices = ((System.Collections.ArrayList)(resources.GetObject("comboStatuses.SelectedIndices")));
			this.comboStatuses.Size = new System.Drawing.Size(160, 21);
			this.comboStatuses.TabIndex = 38;
			this.comboStatuses.SelectionChangeCommitted += new OpenDental.UI.ComboBoxMulti.SelectionChangeCommittedHandler(this.FilterChanged);
			// 
			// comboClinics
			// 
			this.comboClinics.DoIncludeAll = true;
			this.comboClinics.DoIncludeUnassigned = true;
			this.comboClinics.IsMultiSelect = true;
			this.comboClinics.Location = new System.Drawing.Point(210, 43);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(195, 21);
			this.comboClinics.TabIndex = 34;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.FilterChanged);
			// 
			// FormRecurringChargesHistory
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1044, 714);
			this.Controls.Add(this.groupView);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRecurringChargesHistory";
			this.Text = "Recurring Charge History";
			this.Load += new System.EventHandler(this.FormRecurringChargesHistory_Load);
			this.groupView.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.ODGrid gridMain;
		private UI.Button butRefresh;
		private UI.ODDateRangePicker datePicker;
		private System.Windows.Forms.GroupBox groupView;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItemGoTo;
		private System.Windows.Forms.MenuItem menuItemOpenPayment;
		private System.Windows.Forms.MenuItem menuItemViewError;
		private System.Windows.Forms.MenuItem menuItemDeletePending;
		private UI.ComboBoxClinicPicker comboClinics;
		private System.Windows.Forms.Label label1;
		private UI.ComboBoxMulti comboStatuses;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboAutomated;
	}
}