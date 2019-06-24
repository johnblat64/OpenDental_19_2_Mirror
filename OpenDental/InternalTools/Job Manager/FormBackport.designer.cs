using System;

namespace OpenDental {
	partial class FormBackport {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBackport));
			this.textPath = new System.Windows.Forms.TextBox();
			this.textIgnoreList = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelCurProj = new System.Windows.Forms.Label();
			this.checkBeta = new System.Windows.Forms.CheckBox();
			this.checkStable = new System.Windows.Forms.CheckBox();
			this.checkPrevStable = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.ODGrid();
			this.butCommit = new OpenDental.UI.Button();
			this.butCompilePrevStable = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.butCompileBeta = new OpenDental.UI.Button();
			this.butCompileStable = new OpenDental.UI.Button();
			this.butBackport = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textPath
			// 
			this.textPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPath.Location = new System.Drawing.Point(482, 10);
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(345, 20);
			this.textPath.TabIndex = 4;
			this.textPath.Text = "C:\\development\\OPEN DENTAL SUBVERSION";
			// 
			// textIgnoreList
			// 
			this.textIgnoreList.Location = new System.Drawing.Point(376, 10);
			this.textIgnoreList.Name = "textIgnoreList";
			this.textIgnoreList.Size = new System.Drawing.Size(100, 20);
			this.textIgnoreList.TabIndex = 3;
			this.textIgnoreList.Text = "ignore-on-commit";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(316, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 14;
			this.label2.Text = "SVN Ignore";
			// 
			// labelCurProj
			// 
			this.labelCurProj.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCurProj.Location = new System.Drawing.Point(276, 521);
			this.labelCurProj.Name = "labelCurProj";
			this.labelCurProj.Size = new System.Drawing.Size(348, 13);
			this.labelCurProj.TabIndex = 13;
			this.labelCurProj.Text = "Current Project: Unknown";
			this.labelCurProj.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBeta
			// 
			this.checkBeta.AutoSize = true;
			this.checkBeta.Checked = true;
			this.checkBeta.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBeta.Location = new System.Drawing.Point(12, 12);
			this.checkBeta.Name = "checkBeta";
			this.checkBeta.Size = new System.Drawing.Size(54, 17);
			this.checkBeta.TabIndex = 0;
			this.checkBeta.Text = "Beta: ";
			this.checkBeta.UseVisualStyleBackColor = true;
			// 
			// checkStable
			// 
			this.checkStable.AutoSize = true;
			this.checkStable.Location = new System.Drawing.Point(88, 12);
			this.checkStable.Name = "checkStable";
			this.checkStable.Size = new System.Drawing.Size(62, 17);
			this.checkStable.TabIndex = 1;
			this.checkStable.Text = "Stable: ";
			this.checkStable.UseVisualStyleBackColor = true;
			// 
			// checkPrevStable
			// 
			this.checkPrevStable.AutoSize = true;
			this.checkPrevStable.Location = new System.Drawing.Point(172, 12);
			this.checkPrevStable.Name = "checkPrevStable";
			this.checkPrevStable.Size = new System.Drawing.Size(106, 17);
			this.checkPrevStable.TabIndex = 2;
			this.checkPrevStable.Text = "Previous Stable: ";
			this.checkPrevStable.UseVisualStyleBackColor = true;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridMain.HasAddButton = false;
			this.gridMain.HasDistinctClickEvents = true;
			this.gridMain.HasDropDowns = false;
			this.gridMain.HasLinkDetect = false;
			this.gridMain.HasMultilineHeaders = false;
			this.gridMain.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridMain.HeaderHeight = 15;
			this.gridMain.HScrollVisible = false;
			this.gridMain.Location = new System.Drawing.Point(12, 36);
			this.gridMain.Name = "gridMain";
			this.gridMain.ScrollValue = 0;
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(896, 473);
			this.gridMain.TabIndex = 12;
			this.gridMain.Title = "Head Differences";
			this.gridMain.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridMain.TitleHeight = 18;
			this.gridMain.TranslationName = "gridMain";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butCommit
			// 
			this.butCommit.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCommit.Autosize = true;
			this.butCommit.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCommit.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCommit.CornerRadius = 4F;
			this.butCommit.Location = new System.Drawing.Point(671, 516);
			this.butCommit.Name = "butCommit";
			this.butCommit.Size = new System.Drawing.Size(75, 23);
			this.butCommit.TabIndex = 9;
			this.butCommit.Text = "Commit";
			this.butCommit.UseVisualStyleBackColor = true;
			this.butCommit.Click += new System.EventHandler(this.butCommit_Click);
			// 
			// butCompilePrevStable
			// 
			this.butCompilePrevStable.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCompilePrevStable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCompilePrevStable.Autosize = true;
			this.butCompilePrevStable.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCompilePrevStable.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCompilePrevStable.CornerRadius = 4F;
			this.butCompilePrevStable.Location = new System.Drawing.Point(188, 516);
			this.butCompilePrevStable.Name = "butCompilePrevStable";
			this.butCompilePrevStable.Size = new System.Drawing.Size(82, 23);
			this.butCompilePrevStable.TabIndex = 8;
			this.butCompilePrevStable.Text = "Compile ";
			this.butCompilePrevStable.UseVisualStyleBackColor = true;
			this.butCompilePrevStable.Click += new System.EventHandler(this.butCompilePrevStable_Click);
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.CornerRadius = 4F;
			this.butClose.Location = new System.Drawing.Point(833, 516);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 11;
			this.butClose.Text = "&Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Autosize = true;
			this.butRefresh.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butRefresh.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butRefresh.CornerRadius = 4F;
			this.butRefresh.Location = new System.Drawing.Point(833, 8);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 5;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butCompileBeta
			// 
			this.butCompileBeta.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCompileBeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCompileBeta.Autosize = true;
			this.butCompileBeta.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCompileBeta.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCompileBeta.CornerRadius = 4F;
			this.butCompileBeta.Location = new System.Drawing.Point(12, 516);
			this.butCompileBeta.Name = "butCompileBeta";
			this.butCompileBeta.Size = new System.Drawing.Size(82, 23);
			this.butCompileBeta.TabIndex = 6;
			this.butCompileBeta.Text = "Compile ";
			this.butCompileBeta.UseVisualStyleBackColor = true;
			this.butCompileBeta.Click += new System.EventHandler(this.butCompileBeta_Click);
			// 
			// butCompileStable
			// 
			this.butCompileStable.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCompileStable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCompileStable.Autosize = true;
			this.butCompileStable.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCompileStable.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCompileStable.CornerRadius = 4F;
			this.butCompileStable.Location = new System.Drawing.Point(100, 516);
			this.butCompileStable.Name = "butCompileStable";
			this.butCompileStable.Size = new System.Drawing.Size(82, 23);
			this.butCompileStable.TabIndex = 7;
			this.butCompileStable.Text = "Compile ";
			this.butCompileStable.UseVisualStyleBackColor = true;
			this.butCompileStable.Click += new System.EventHandler(this.butCompileStable_Click);
			// 
			// butBackport
			// 
			this.butBackport.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butBackport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butBackport.Autosize = true;
			this.butBackport.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butBackport.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butBackport.CornerRadius = 4F;
			this.butBackport.Location = new System.Drawing.Point(752, 516);
			this.butBackport.Name = "butBackport";
			this.butBackport.Size = new System.Drawing.Size(75, 23);
			this.butBackport.TabIndex = 10;
			this.butBackport.Text = "&Backport";
			this.butBackport.UseVisualStyleBackColor = true;
			this.butBackport.Click += new System.EventHandler(this.butBackport_Click);
			// 
			// FormBackport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(920, 548);
			this.Controls.Add(this.butCommit);
			this.Controls.Add(this.checkPrevStable);
			this.Controls.Add(this.butCompilePrevStable);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butCompileBeta);
			this.Controls.Add(this.butCompileStable);
			this.Controls.Add(this.butBackport);
			this.Controls.Add(this.labelCurProj);
			this.Controls.Add(this.textIgnoreList);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.checkStable);
			this.Controls.Add(this.checkBeta);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(500, 196);
			this.Name = "FormBackport";
			this.Text = "Backport";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.ODGrid gridMain;
		private System.Windows.Forms.TextBox textPath;
		private System.Windows.Forms.TextBox textIgnoreList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelCurProj;
		private UI.Button butBackport;
		private UI.Button butCompileStable;
		private UI.Button butCompileBeta;
		private UI.Button butRefresh;
		private UI.Button butClose;
		private UI.Button butCompilePrevStable;
		private System.Windows.Forms.CheckBox checkBeta;
		private System.Windows.Forms.CheckBox checkStable;
		private System.Windows.Forms.CheckBox checkPrevStable;
		private UI.Button butCommit;
	}
}

