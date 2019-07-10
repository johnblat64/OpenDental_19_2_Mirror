namespace OpenDental.InternalTools.Phones {
	partial class UserControlMapDetails {
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
			this.labelUserName = new System.Windows.Forms.Label();
			this.labelExtension = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.labelTime = new System.Windows.Forms.Label();
			this.odPictureBoxEmployee = new OpenDental.UI.ODPictureBox();
			this.SuspendLayout();
			// 
			// labelUserName
			// 
			this.labelUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUserName.Location = new System.Drawing.Point(179, 3);
			this.labelUserName.Name = "labelUserName";
			this.labelUserName.Size = new System.Drawing.Size(167, 37);
			this.labelUserName.TabIndex = 1;
			this.labelUserName.Text = "Employee";
			this.labelUserName.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// labelExtension
			// 
			this.labelExtension.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelExtension.Location = new System.Drawing.Point(180, 43);
			this.labelExtension.Name = "labelExtension";
			this.labelExtension.Size = new System.Drawing.Size(167, 34);
			this.labelExtension.TabIndex = 2;
			this.labelExtension.Text = "x0000";
			this.labelExtension.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// labelStatus
			// 
			this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatus.Location = new System.Drawing.Point(180, 77);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(167, 33);
			this.labelStatus.TabIndex = 3;
			this.labelStatus.Text = "Available";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// labelTime
			// 
			this.labelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTime.Location = new System.Drawing.Point(180, 110);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(167, 33);
			this.labelTime.TabIndex = 4;
			this.labelTime.Text = "0:00:00";
			this.labelTime.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// odPictureBoxEmployee
			// 
			this.odPictureBoxEmployee.HasBorder = false;
			this.odPictureBoxEmployee.Location = new System.Drawing.Point(3, 2);
			this.odPictureBoxEmployee.Name = "odPictureBoxEmployee";
			this.odPictureBoxEmployee.Size = new System.Drawing.Size(170, 146);
			this.odPictureBoxEmployee.TabIndex = 0;
			this.odPictureBoxEmployee.Text = "Employee Picture";
			this.odPictureBoxEmployee.TextNullImage = "No Image Available";
			// 
			// UserControlMapDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelTime);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.labelExtension);
			this.Controls.Add(this.labelUserName);
			this.Controls.Add(this.odPictureBoxEmployee);
			this.Name = "UserControlMapDetails";
			this.Size = new System.Drawing.Size(350, 255);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.ODPictureBox odPictureBoxEmployee;
		private System.Windows.Forms.Label labelUserName;
		private System.Windows.Forms.Label labelExtension;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.Label labelTime;
	}
}
