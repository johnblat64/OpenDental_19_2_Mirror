namespace CentralManager {
	partial class FormCentralPatientTransfer {
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
			this.gridPatients = new OpenDental.UI.ODGrid();
			this.gridDatabasesTo = new OpenDental.UI.ODGrid();
			this.labelSourceDb = new System.Windows.Forms.Label();
			this.butTransfer = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butAddPatients = new OpenDental.UI.Button();
			this.butAddDatabases = new OpenDental.UI.Button();
			this.butRemovePats = new OpenDental.UI.Button();
			this.butRemoveDb = new OpenDental.UI.Button();
			this.labelExplanation = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridPatients
			// 
			this.gridPatients.AllowSortingByColumn = true;
			this.gridPatients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridPatients.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridPatients.EditableEnterMovesDown = false;
			this.gridPatients.HasAddButton = false;
			this.gridPatients.HasDropDowns = false;
			this.gridPatients.HasMultilineHeaders = false;
			this.gridPatients.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridPatients.HeaderHeight = 15;
			this.gridPatients.HScrollVisible = false;
			this.gridPatients.Location = new System.Drawing.Point(12, 59);
			this.gridPatients.Name = "gridPatients";
			this.gridPatients.ScrollValue = 0;
			this.gridPatients.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridPatients.Size = new System.Drawing.Size(450, 369);
			this.gridPatients.TabIndex = 6;
			this.gridPatients.Title = "Patients to transfer";
			this.gridPatients.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridPatients.TitleHeight = 18;
			this.gridPatients.TranslationName = "";
			// 
			// gridDatabasesTo
			// 
			this.gridDatabasesTo.AllowSortingByColumn = true;
			this.gridDatabasesTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridDatabasesTo.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridDatabasesTo.EditableEnterMovesDown = false;
			this.gridDatabasesTo.HasAddButton = false;
			this.gridDatabasesTo.HasDropDowns = false;
			this.gridDatabasesTo.HasMultilineHeaders = false;
			this.gridDatabasesTo.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridDatabasesTo.HeaderHeight = 15;
			this.gridDatabasesTo.HScrollVisible = false;
			this.gridDatabasesTo.Location = new System.Drawing.Point(485, 59);
			this.gridDatabasesTo.Name = "gridDatabasesTo";
			this.gridDatabasesTo.ScrollValue = 0;
			this.gridDatabasesTo.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridDatabasesTo.Size = new System.Drawing.Size(450, 369);
			this.gridDatabasesTo.TabIndex = 7;
			this.gridDatabasesTo.Title = "Databases to transfer patients to";
			this.gridDatabasesTo.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridDatabasesTo.TitleHeight = 18;
			this.gridDatabasesTo.TranslationName = "";
			// 
			// labelSourceDb
			// 
			this.labelSourceDb.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelSourceDb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSourceDb.Location = new System.Drawing.Point(12, 36);
			this.labelSourceDb.Name = "labelSourceDb";
			this.labelSourceDb.Size = new System.Drawing.Size(453, 12);
			this.labelSourceDb.TabIndex = 228;
			this.labelSourceDb.Text = "Source database:   ";
			this.labelSourceDb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butTransfer
			// 
			this.butTransfer.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTransfer.Autosize = true;
			this.butTransfer.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butTransfer.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butTransfer.CornerRadius = 4F;
			this.butTransfer.Location = new System.Drawing.Point(953, 388);
			this.butTransfer.Name = "butTransfer";
			this.butTransfer.Size = new System.Drawing.Size(75, 23);
			this.butTransfer.TabIndex = 229;
			this.butTransfer.Text = "Transfer";
			this.butTransfer.UseVisualStyleBackColor = true;
			this.butTransfer.Click += new System.EventHandler(this.butTransfer_Click);
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
			this.butClose.Location = new System.Drawing.Point(953, 448);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 230;
			this.butClose.Text = "&Close";
			// 
			// butAddPatients
			// 
			this.butAddPatients.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAddPatients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddPatients.Autosize = true;
			this.butAddPatients.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddPatients.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddPatients.CornerRadius = 4F;
			this.butAddPatients.Location = new System.Drawing.Point(12, 447);
			this.butAddPatients.Name = "butAddPatients";
			this.butAddPatients.Size = new System.Drawing.Size(75, 23);
			this.butAddPatients.TabIndex = 231;
			this.butAddPatients.Text = "Add";
			this.butAddPatients.UseVisualStyleBackColor = true;
			this.butAddPatients.Click += new System.EventHandler(this.butAddPatients_Click);
			// 
			// butAddDatabases
			// 
			this.butAddDatabases.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAddDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddDatabases.Autosize = true;
			this.butAddDatabases.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddDatabases.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddDatabases.CornerRadius = 4F;
			this.butAddDatabases.Location = new System.Drawing.Point(485, 447);
			this.butAddDatabases.Name = "butAddDatabases";
			this.butAddDatabases.Size = new System.Drawing.Size(75, 23);
			this.butAddDatabases.TabIndex = 232;
			this.butAddDatabases.Text = "Add";
			this.butAddDatabases.UseVisualStyleBackColor = true;
			this.butAddDatabases.Click += new System.EventHandler(this.butAddDatabases_Click);
			// 
			// butRemovePats
			// 
			this.butRemovePats.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRemovePats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRemovePats.Autosize = true;
			this.butRemovePats.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRemovePats.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRemovePats.CornerRadius = 4F;
			this.butRemovePats.Location = new System.Drawing.Point(93, 448);
			this.butRemovePats.Name = "butRemovePats";
			this.butRemovePats.Size = new System.Drawing.Size(75, 23);
			this.butRemovePats.TabIndex = 233;
			this.butRemovePats.Text = "Remove";
			this.butRemovePats.UseVisualStyleBackColor = true;
			this.butRemovePats.Click += new System.EventHandler(this.butRemovePats_Click);
			// 
			// butRemoveDb
			// 
			this.butRemoveDb.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRemoveDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRemoveDb.Autosize = true;
			this.butRemoveDb.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRemoveDb.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRemoveDb.CornerRadius = 4F;
			this.butRemoveDb.Location = new System.Drawing.Point(566, 447);
			this.butRemoveDb.Name = "butRemoveDb";
			this.butRemoveDb.Size = new System.Drawing.Size(75, 23);
			this.butRemoveDb.TabIndex = 234;
			this.butRemoveDb.Text = "Remove";
			this.butRemoveDb.UseVisualStyleBackColor = true;
			this.butRemoveDb.Click += new System.EventHandler(this.butRemoveDb_Click);
			// 
			// labelExplanation
			// 
			this.labelExplanation.AutoSize = true;
			this.labelExplanation.Location = new System.Drawing.Point(12, 6);
			this.labelExplanation.Name = "labelExplanation";
			this.labelExplanation.Size = new System.Drawing.Size(422, 13);
			this.labelExplanation.TabIndex = 235;
			this.labelExplanation.Text = "To transfer a patient, first add patients to transfer. Then add the databases to " +
    "transfer to.";
			// 
			// FormCentralPatientTransfer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1050, 489);
			this.Controls.Add(this.labelExplanation);
			this.Controls.Add(this.butRemoveDb);
			this.Controls.Add(this.butRemovePats);
			this.Controls.Add(this.butAddDatabases);
			this.Controls.Add(this.butAddPatients);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butTransfer);
			this.Controls.Add(this.labelSourceDb);
			this.Controls.Add(this.gridDatabasesTo);
			this.Controls.Add(this.gridPatients);
			this.Name = "FormCentralPatientTransfer";
			this.Text = "Patient Transfer";
			this.Load += new System.EventHandler(this.FormCentralConnectionPatientTransfer_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.ODGrid gridPatients;
		private OpenDental.UI.ODGrid gridDatabasesTo;
		private System.Windows.Forms.Label labelSourceDb;
		private OpenDental.UI.Button butTransfer;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAddPatients;
		private OpenDental.UI.Button butAddDatabases;
		private OpenDental.UI.Button butRemovePats;
		private OpenDental.UI.Button butRemoveDb;
		private System.Windows.Forms.Label labelExplanation;
	}
}