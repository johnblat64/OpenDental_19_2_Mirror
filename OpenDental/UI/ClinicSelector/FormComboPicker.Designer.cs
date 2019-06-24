namespace OpenDental.UI
{
	partial class FormComboPicker
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listBoxMain = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// listBoxMain
			// 
			this.listBoxMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listBoxMain.FormattingEnabled = true;
			this.listBoxMain.IntegralHeight = false;
			this.listBoxMain.Location = new System.Drawing.Point(0, 21);
			this.listBoxMain.Name = "listBoxMain";
			this.listBoxMain.Size = new System.Drawing.Size(119, 30);
			this.listBoxMain.TabIndex = 16;
			this.listBoxMain.Click += new System.EventHandler(this.ListBoxMain_Click);
			// 
			// FormComboPicker
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(176, 73);
			this.Controls.Add(this.listBoxMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.Name = "FormComboPicker";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.Deactivate += new System.EventHandler(this.FormComboPicker_Deactivate);
			this.Load += new System.EventHandler(this.FormComboPicker_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormComboPicker_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormComboPicker_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormComboPicker_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ListBox listBoxMain;
	}
}