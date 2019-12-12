namespace OpenDental {
	partial class FormClaimAttachment {
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
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.labelNarrative = new System.Windows.Forms.Label();
			this.groupBoxAttachment = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textClaimStatus = new OpenDental.ODtextBox();
			this.gridAttachedImages = new OpenDental.UI.ODGrid();
			this.butSnipTool = new OpenDental.UI.Button();
			this.butAddImage = new OpenDental.UI.Button();
			this.textNarrative = new OpenDental.ODtextBox();
			this.labelClaimAttachWarning = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelPasteImage = new System.Windows.Forms.Label();
			this.butPasteImage = new OpenDental.UI.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.groupBoxAttachment.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
			this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ContextMenu_ItemClicked);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			// 
			// labelNarrative
			// 
			this.labelNarrative.Location = new System.Drawing.Point(7, 257);
			this.labelNarrative.Name = "labelNarrative";
			this.labelNarrative.Size = new System.Drawing.Size(121, 13);
			this.labelNarrative.TabIndex = 12;
			this.labelNarrative.Text = "Narrative (optional)";
			// 
			// groupBoxAttachment
			// 
			this.groupBoxAttachment.Controls.Add(this.labelPasteImage);
			this.groupBoxAttachment.Controls.Add(this.butPasteImage);
			this.groupBoxAttachment.Controls.Add(this.label1);
			this.groupBoxAttachment.Controls.Add(this.textClaimStatus);
			this.groupBoxAttachment.Controls.Add(this.gridAttachedImages);
			this.groupBoxAttachment.Controls.Add(this.butSnipTool);
			this.groupBoxAttachment.Controls.Add(this.labelNarrative);
			this.groupBoxAttachment.Controls.Add(this.butAddImage);
			this.groupBoxAttachment.Controls.Add(this.textNarrative);
			this.groupBoxAttachment.Location = new System.Drawing.Point(7, 7);
			this.groupBoxAttachment.Name = "groupBoxAttachment";
			this.groupBoxAttachment.Size = new System.Drawing.Size(593, 374);
			this.groupBoxAttachment.TabIndex = 13;
			this.groupBoxAttachment.TabStop = false;
			this.groupBoxAttachment.Text = "Create Attachment";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(283, 257);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "Claim Validation Status";
			// 
			// textClaimStatus
			// 
			this.textClaimStatus.AcceptsTab = true;
			this.textClaimStatus.BackColor = System.Drawing.SystemColors.Control;
			this.textClaimStatus.DetectLinksEnabled = false;
			this.textClaimStatus.DetectUrls = false;
			this.textClaimStatus.Location = new System.Drawing.Point(286, 273);
			this.textClaimStatus.Name = "textClaimStatus";
			this.textClaimStatus.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textClaimStatus.ReadOnly = true;
			this.textClaimStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textClaimStatus.Size = new System.Drawing.Size(301, 91);
			this.textClaimStatus.TabIndex = 6;
			this.textClaimStatus.Text = "";
			// 
			// gridAttachedImages
			// 
			this.gridAttachedImages.CellFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
			this.gridAttachedImages.ContextMenuStrip = this.contextMenuStrip1;
			this.gridAttachedImages.HasDropDowns = false;
			this.gridAttachedImages.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
			this.gridAttachedImages.HeaderHeight = 15;
			this.gridAttachedImages.HScrollVisible = false;
			this.gridAttachedImages.Location = new System.Drawing.Point(6, 21);
			this.gridAttachedImages.Name = "gridAttachedImages";
			this.gridAttachedImages.ScrollValue = 0;
			this.gridAttachedImages.Size = new System.Drawing.Size(460, 231);
			this.gridAttachedImages.TabIndex = 10;
			this.gridAttachedImages.Title = "Images to Send";
			this.gridAttachedImages.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
			this.gridAttachedImages.TitleHeight = 18;
			this.gridAttachedImages.TranslationName = "TableImagesToSend";
			this.gridAttachedImages.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.CellDoubleClick_EditImage);
			// 
			// butSnipTool
			// 
			this.butSnipTool.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butSnipTool.Autosize = true;
			this.butSnipTool.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butSnipTool.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butSnipTool.CornerRadius = 4F;
			this.butSnipTool.Location = new System.Drawing.Point(472, 50);
			this.butSnipTool.Name = "butSnipTool";
			this.butSnipTool.Size = new System.Drawing.Size(115, 24);
			this.butSnipTool.TabIndex = 0;
			this.butSnipTool.Text = "Snipping Tool";
			this.butSnipTool.UseVisualStyleBackColor = true;
			this.butSnipTool.Click += new System.EventHandler(this.buttonSnipTool_Click);
			// 
			// butAddImage
			// 
			this.butAddImage.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAddImage.Autosize = true;
			this.butAddImage.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddImage.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddImage.CornerRadius = 4F;
			this.butAddImage.Location = new System.Drawing.Point(472, 21);
			this.butAddImage.Name = "butAddImage";
			this.butAddImage.Size = new System.Drawing.Size(115, 24);
			this.butAddImage.TabIndex = 2;
			this.butAddImage.Text = "Add Image";
			this.butAddImage.UseVisualStyleBackColor = true;
			this.butAddImage.Click += new System.EventHandler(this.buttonAddImage_Click);
			// 
			// textNarrative
			// 
			this.textNarrative.AcceptsTab = true;
			this.textNarrative.BackColor = System.Drawing.SystemColors.Window;
			this.textNarrative.DetectLinksEnabled = false;
			this.textNarrative.DetectUrls = false;
			this.textNarrative.Location = new System.Drawing.Point(6, 273);
			this.textNarrative.Name = "textNarrative";
			this.textNarrative.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textNarrative.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNarrative.Size = new System.Drawing.Size(261, 91);
			this.textNarrative.TabIndex = 11;
			this.textNarrative.Text = "";
			// 
			// labelClaimAttachWarning
			// 
			this.labelClaimAttachWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimAttachWarning.ForeColor = System.Drawing.Color.DarkRed;
			this.labelClaimAttachWarning.Location = new System.Drawing.Point(7, 397);
			this.labelClaimAttachWarning.Name = "labelClaimAttachWarning";
			this.labelClaimAttachWarning.Size = new System.Drawing.Size(411, 27);
			this.labelClaimAttachWarning.TabIndex = 15;
			this.labelClaimAttachWarning.Text = "No claim attachment image category definition found.  Images will be saved using " +
    "the first image category.";
			this.labelClaimAttachWarning.Visible = false;
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Autosize = true;
			this.butCancel.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCancel.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCancel.CornerRadius = 4F;
			this.butCancel.Location = new System.Drawing.Point(519, 398);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// butOK
			// 
			this.butOK.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Autosize = true;
			this.butOK.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butOK.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butOK.CornerRadius = 4F;
			this.butOK.Location = new System.Drawing.Point(438, 398);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// labelPasteImage
			// 
			this.labelPasteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPasteImage.Location = new System.Drawing.Point(472, 107);
			this.labelPasteImage.Name = "labelPasteImage";
			this.labelPasteImage.Size = new System.Drawing.Size(115, 35);
			this.labelPasteImage.TabIndex = 20;
			this.labelPasteImage.Text = "(image may be pasted from your clipboard)";
			this.labelPasteImage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butPasteImage
			// 
			this.butPasteImage.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butPasteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPasteImage.Autosize = true;
			this.butPasteImage.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butPasteImage.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butPasteImage.CornerRadius = 4F;
			this.butPasteImage.Location = new System.Drawing.Point(472, 80);
			this.butPasteImage.Name = "butPasteImage";
			this.butPasteImage.Size = new System.Drawing.Size(115, 24);
			this.butPasteImage.TabIndex = 19;
			this.butPasteImage.Text = "Paste Image";
			this.butPasteImage.UseVisualStyleBackColor = true;
			this.butPasteImage.Click += new System.EventHandler(this.ButPasteImage_Click);
			// 
			// FormClaimAttachment
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(606, 431);
			this.Controls.Add(this.labelClaimAttachWarning);
			this.Controls.Add(this.groupBoxAttachment);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Name = "FormClaimAttachment";
			this.Text = "Claim Attachments";
			this.Load += new System.EventHandler(this.FormClaimAttachment_Load);
			this.Shown += new System.EventHandler(this.FormClaimAttachment_Shown);
			this.contextMenuStrip1.ResumeLayout(false);
			this.groupBoxAttachment.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butSnipTool;
		private UI.Button butAddImage;
		private UI.Button butOK;
		private UI.Button butCancel;
		private ODtextBox textClaimStatus;
		private UI.ODGrid gridAttachedImages;
		private ODtextBox textNarrative;
		private System.Windows.Forms.Label labelNarrative;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBoxAttachment;
		private System.Windows.Forms.Label labelClaimAttachWarning;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelPasteImage;
		private UI.Button butPasteImage;
	}
}