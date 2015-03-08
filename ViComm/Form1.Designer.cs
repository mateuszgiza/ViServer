namespace ViComm
{
	partial class Form1
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
			if ( disposing && ( components != null ) ) {
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
			this.outputBox = new System.Windows.Forms.RichTextBox();
			this.inputBox = new System.Windows.Forms.TextBox();
			this.log = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// outputBox
			// 
			this.outputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputBox.BackColor = System.Drawing.Color.SkyBlue;
			this.outputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.outputBox.Location = new System.Drawing.Point(12, 12);
			this.outputBox.Name = "outputBox";
			this.outputBox.ReadOnly = true;
			this.outputBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.outputBox.Size = new System.Drawing.Size(460, 289);
			this.outputBox.TabIndex = 0;
			this.outputBox.Text = "";
			// 
			// inputBox
			// 
			this.inputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.inputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.inputBox.Location = new System.Drawing.Point(12, 307);
			this.inputBox.Name = "inputBox";
			this.inputBox.Size = new System.Drawing.Size(460, 23);
			this.inputBox.TabIndex = 1;
			this.inputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputBox_KeyDown);
			// 
			// log
			// 
			this.log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.log.Location = new System.Drawing.Point(12, 333);
			this.log.Name = "log";
			this.log.Size = new System.Drawing.Size(460, 20);
			this.log.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.SkyBlue;
			this.ClientSize = new System.Drawing.Size(484, 362);
			this.Controls.Add(this.log);
			this.Controls.Add(this.inputBox);
			this.Controls.Add(this.outputBox);
			this.Name = "Form1";
			this.Text = "ViComm";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.RichTextBox outputBox;
		public System.Windows.Forms.TextBox inputBox;
		public System.Windows.Forms.Label log;

	}
}

