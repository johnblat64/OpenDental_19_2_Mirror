﻿using System;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBackport));
			this.textPath = new System.Windows.Forms.TextBox();
			this.textIgnoreList = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelCurProj = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.ODGrid();
			this.butCommit = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.butCompileAll = new OpenDental.UI.Button();
			this.butBackport = new OpenDental.UI.Button();
			this.listBoxVersions = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.validJobNum = new OpenDental.ValidNum();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.butBackportAndCommit = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textPath
			// 
			this.textPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPath.Location = new System.Drawing.Point(392, 11);
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(541, 20);
			this.textPath.TabIndex = 4;
			this.textPath.Text = "C:\\development\\OPEN DENTAL SUBVERSION";
			// 
			// textIgnoreList
			// 
			this.textIgnoreList.Location = new System.Drawing.Point(179, 11);
			this.textIgnoreList.Name = "textIgnoreList";
			this.textIgnoreList.Size = new System.Drawing.Size(100, 20);
			this.textIgnoreList.TabIndex = 3;
			this.textIgnoreList.Text = "ignore-on-commit";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(119, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 14;
			this.label2.Text = "SVN Ignore";
			// 
			// labelCurProj
			// 
			this.labelCurProj.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCurProj.Location = new System.Drawing.Point(114, 551);
			this.labelCurProj.Name = "labelCurProj";
			this.labelCurProj.Size = new System.Drawing.Size(533, 13);
			this.labelCurProj.TabIndex = 13;
			this.labelCurProj.Text = "Current Project: Unknown";
			this.labelCurProj.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridMain.HasDistinctClickEvents = true;
			this.gridMain.HasDropDowns = false;
			this.gridMain.HasLinkDetect = false;
			this.gridMain.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridMain.HeaderHeight = 15;
			this.gridMain.HScrollVisible = false;
			this.gridMain.Location = new System.Drawing.Point(114, 37);
			this.gridMain.Name = "gridMain";
			this.gridMain.ScrollValue = 0;
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(900, 498);
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
			this.butCommit.Location = new System.Drawing.Point(858, 546);
			this.butCommit.Name = "butCommit";
			this.butCommit.Size = new System.Drawing.Size(75, 23);
			this.butCommit.TabIndex = 9;
			this.butCommit.Text = "Commit";
			this.butCommit.UseVisualStyleBackColor = true;
			this.butCommit.Click += new System.EventHandler(this.butCommit_Click);
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.CornerRadius = 4F;
			this.butClose.Location = new System.Drawing.Point(939, 546);
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
			this.butRefresh.Location = new System.Drawing.Point(939, 10);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 5;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butCompileAll
			// 
			this.butCompileAll.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCompileAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCompileAll.Autosize = true;
			this.butCompileAll.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCompileAll.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCompileAll.CornerRadius = 4F;
			this.butCompileAll.Location = new System.Drawing.Point(12, 546);
			this.butCompileAll.Name = "butCompileAll";
			this.butCompileAll.Size = new System.Drawing.Size(98, 23);
			this.butCompileAll.TabIndex = 6;
			this.butCompileAll.Text = "Compile Selected";
			this.butCompileAll.UseVisualStyleBackColor = true;
			this.butCompileAll.Click += new System.EventHandler(this.butCompileAll_Click);
			// 
			// butBackport
			// 
			this.butBackport.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butBackport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butBackport.Autosize = true;
			this.butBackport.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butBackport.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butBackport.CornerRadius = 4F;
			this.butBackport.Location = new System.Drawing.Point(777, 546);
			this.butBackport.Name = "butBackport";
			this.butBackport.Size = new System.Drawing.Size(75, 23);
			this.butBackport.TabIndex = 10;
			this.butBackport.Text = "&Backport";
			this.butBackport.UseVisualStyleBackColor = true;
			this.butBackport.Click += new System.EventHandler(this.butBackport_Click);
			// 
			// listBoxVersions
			// 
			this.listBoxVersions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxVersions.FormattingEnabled = true;
			this.listBoxVersions.Location = new System.Drawing.Point(12, 37);
			this.listBoxVersions.Name = "listBoxVersions";
			this.listBoxVersions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxVersions.Size = new System.Drawing.Size(96, 498);
			this.listBoxVersions.TabIndex = 15;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(282, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Job #";
			// 
			// validJobNum
			// 
			this.validJobNum.Location = new System.Drawing.Point(314, 11);
			this.validJobNum.MaxVal = 999999999;
			this.validJobNum.MinVal = 0;
			this.validJobNum.Name = "validJobNum";
			this.validJobNum.Size = new System.Drawing.Size(46, 20);
			this.validJobNum.TabIndex = 17;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 13);
			this.label3.TabIndex = 18;
			this.label3.Text = "Versions";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(364, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(29, 13);
			this.label4.TabIndex = 19;
			this.label4.Text = "Path";
			// 
			// butBackportAndCommit
			// 
			this.butBackportAndCommit.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butBackportAndCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butBackportAndCommit.Autosize = true;
			this.butBackportAndCommit.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butBackportAndCommit.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butBackportAndCommit.CornerRadius = 4F;
			this.butBackportAndCommit.Location = new System.Drawing.Point(653, 546);
			this.butBackportAndCommit.Name = "butBackportAndCommit";
			this.butBackportAndCommit.Size = new System.Drawing.Size(118, 23);
			this.butBackportAndCommit.TabIndex = 20;
			this.butBackportAndCommit.Text = "Backport and Commit";
			this.butBackportAndCommit.UseVisualStyleBackColor = true;
			this.butBackportAndCommit.Click += new System.EventHandler(this.butBackportAndCommit_Click);
			// 
			// FormBackport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1026, 580);
			this.Controls.Add(this.butBackportAndCommit);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.validJobNum);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBoxVersions);
			this.Controls.Add(this.butCommit);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butCompileAll);
			this.Controls.Add(this.butBackport);
			this.Controls.Add(this.labelCurProj);
			this.Controls.Add(this.textIgnoreList);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(500, 196);
			this.Name = "FormBackport";
			this.Text = "Backport";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBackport_FormClosing);
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
		private UI.Button butCompileAll;
		private UI.Button butRefresh;
		private UI.Button butClose;
		private UI.Button butCommit;
		private System.Windows.Forms.ListBox listBoxVersions;
		private System.Windows.Forms.Label label1;
		private ValidNum validJobNum;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private UI.Button butBackportAndCommit;
	}
}

