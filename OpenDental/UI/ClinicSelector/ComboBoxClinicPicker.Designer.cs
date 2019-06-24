namespace OpenDental.UI {
	partial class ComboBoxClinicPicker {
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
			this.labelClinic = new System.Windows.Forms.Label();
			this.comboFake = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(-2, 2);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(37, 18);
			this.labelClinic.TabIndex = 265;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFake
			// 
			this.comboFake.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboFake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFake.FormattingEnabled = true;
			this.comboFake.Location = new System.Drawing.Point(35, 0);
			this.comboFake.Name = "comboFake";
			this.comboFake.Size = new System.Drawing.Size(165, 21);
			this.comboFake.TabIndex = 263;
			this.comboFake.Visible = false;
			// 
			// ComboBoxClinicPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboFake);
			this.Controls.Add(this.labelClinic);
			this.Name = "ComboBoxClinicPicker";
			this.Size = new System.Drawing.Size(200, 21);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ComboBoxClinicPicker_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ComboBoxClinicPicker_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ComboBox comboFake;
		private System.Windows.Forms.Label labelClinic;
	}
}
