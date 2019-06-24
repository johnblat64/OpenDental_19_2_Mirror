namespace OpenDental{
	partial class FormDashboardWidgetSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDashboardWidgetSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridCustom = new OpenDental.UI.ODGrid();
			this.comboUserGroup = new System.Windows.Forms.ComboBox();
			this.labelUserGroup = new System.Windows.Forms.Label();
			this.butSetAll = new OpenDental.UI.Button();
			this.gridInternal = new OpenDental.UI.ODGrid();
			this.butCopy = new OpenDental.UI.Button();
			this.butDuplicate = new OpenDental.UI.Button();
			this.butTools = new OpenDental.UI.Button();
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
			this.butOK.Location = new System.Drawing.Point(701, 626);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
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
			this.butCancel.Location = new System.Drawing.Point(782, 626);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridCustom
			// 
			this.gridCustom.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridCustom.EditableEnterMovesDown = false;
			this.gridCustom.HasAddButton = true;
			this.gridCustom.HasDropDowns = false;
			this.gridCustom.HasMultilineHeaders = true;
			this.gridCustom.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridCustom.HeaderHeight = 15;
			this.gridCustom.HScrollVisible = false;
			this.gridCustom.Location = new System.Drawing.Point(487, 35);
			this.gridCustom.Name = "gridCustom";
			this.gridCustom.ScrollValue = 0;
			this.gridCustom.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridCustom.Size = new System.Drawing.Size(370, 585);
			this.gridCustom.TabIndex = 233;
			this.gridCustom.Title = "Custom";
			this.gridCustom.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridCustom.TitleHeight = 18;
			this.gridCustom.TranslationName = "TablePermissions";
			this.gridCustom.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCustom_CellDoubleClick);
			this.gridCustom.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCustom_CellClick);
			this.gridCustom.TitleAddClick += new System.EventHandler(this.gridCustom_TitleAddClick);
			// 
			// comboUserGroup
			// 
			this.comboUserGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUserGroup.Location = new System.Drawing.Point(590, 9);
			this.comboUserGroup.Name = "comboUserGroup";
			this.comboUserGroup.Size = new System.Drawing.Size(182, 21);
			this.comboUserGroup.TabIndex = 232;
			// 
			// labelUserGroup
			// 
			this.labelUserGroup.Location = new System.Drawing.Point(484, 8);
			this.labelUserGroup.Name = "labelUserGroup";
			this.labelUserGroup.Size = new System.Drawing.Size(100, 23);
			this.labelUserGroup.TabIndex = 231;
			this.labelUserGroup.Text = "User Group";
			this.labelUserGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSetAll
			// 
			this.butSetAll.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butSetAll.Autosize = true;
			this.butSetAll.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butSetAll.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butSetAll.CornerRadius = 4F;
			this.butSetAll.Location = new System.Drawing.Point(778, 7);
			this.butSetAll.Name = "butSetAll";
			this.butSetAll.Size = new System.Drawing.Size(79, 24);
			this.butSetAll.TabIndex = 230;
			this.butSetAll.Text = "Set All";
			this.butSetAll.Click += new System.EventHandler(this.butSetAll_Click);
			// 
			// gridInternal
			// 
			this.gridInternal.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridInternal.EditableEnterMovesDown = false;
			this.gridInternal.HasAddButton = false;
			this.gridInternal.HasDropDowns = false;
			this.gridInternal.HasMultilineHeaders = true;
			this.gridInternal.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridInternal.HeaderHeight = 15;
			this.gridInternal.HScrollVisible = false;
			this.gridInternal.Location = new System.Drawing.Point(12, 35);
			this.gridInternal.Name = "gridInternal";
			this.gridInternal.ScrollValue = 0;
			this.gridInternal.Size = new System.Drawing.Size(370, 585);
			this.gridInternal.TabIndex = 234;
			this.gridInternal.Title = "Internal";
			this.gridInternal.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridInternal.TitleHeight = 18;
			this.gridInternal.TranslationName = "TableInternal";
			this.gridInternal.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInternal_CellDoubleClick);
			// 
			// butCopy
			// 
			this.butCopy.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCopy.Autosize = true;
			this.butCopy.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCopy.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCopy.CornerRadius = 4F;
			this.butCopy.Image = global::OpenDental.Properties.Resources.Right;
			this.butCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy.Location = new System.Drawing.Point(397, 301);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 24);
			this.butCopy.TabIndex = 235;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// butDuplicate
			// 
			this.butDuplicate.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDuplicate.Autosize = true;
			this.butDuplicate.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butDuplicate.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butDuplicate.CornerRadius = 4F;
			this.butDuplicate.Image = global::OpenDental.Properties.Resources.Add;
			this.butDuplicate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDuplicate.Location = new System.Drawing.Point(606, 626);
			this.butDuplicate.Name = "butDuplicate";
			this.butDuplicate.Size = new System.Drawing.Size(89, 24);
			this.butDuplicate.TabIndex = 236;
			this.butDuplicate.Text = "Duplicate";
			this.butDuplicate.Click += new System.EventHandler(this.butDuplicate_Click);
			// 
			// butTools
			// 
			this.butTools.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butTools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTools.Autosize = true;
			this.butTools.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butTools.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butTools.CornerRadius = 4F;
			this.butTools.Location = new System.Drawing.Point(397, 626);
			this.butTools.Name = "butTools";
			this.butTools.Size = new System.Drawing.Size(75, 24);
			this.butTools.TabIndex = 238;
			this.butTools.Text = "Tools";
			this.butTools.Click += new System.EventHandler(this.butTools_Click);
			// 
			// FormDashboardWidgetSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(869, 658);
			this.Controls.Add(this.butTools);
			this.Controls.Add(this.butDuplicate);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.gridInternal);
			this.Controls.Add(this.gridCustom);
			this.Controls.Add(this.comboUserGroup);
			this.Controls.Add(this.labelUserGroup);
			this.Controls.Add(this.butSetAll);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDashboardWidgetSetup";
			this.Text = "Dashboard Setup";
			this.Load += new System.EventHandler(this.FormDashboardSetup_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.ODGrid gridCustom;
		private System.Windows.Forms.ComboBox comboUserGroup;
		private System.Windows.Forms.Label labelUserGroup;
		private UI.Button butSetAll;
		private UI.ODGrid gridInternal;
		private UI.Button butCopy;
		private UI.Button butDuplicate;
		private UI.Button butTools;
	}
}