namespace OpenDental.UI {
	partial class UserControlApptsPanelJ {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Location = new System.Drawing.Point(620, 74);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 560);
			this.vScrollBar1.TabIndex = 0;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// UserControlApptsPanelJ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.vScrollBar1);
			this.DoubleBuffered = true;
			this.Name = "UserControlApptsPanelJ";
			this.Size = new System.Drawing.Size(774, 737);
			this.Load += new System.EventHandler(this.UserControlApptsPanel_Load);
			this.Resize += new System.EventHandler(this.UserControlApptsPanel_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.VScrollBar vScrollBar1;
	}
}
