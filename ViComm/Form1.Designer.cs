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
			this.lb_nick = new System.Windows.Forms.Label();
			this.txt_name = new System.Windows.Forms.TextBox();
			this.btn_login = new System.Windows.Forms.Button();
			this.lb_to = new System.Windows.Forms.Label();
			this.txt_receiver = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// outputBox
			// 
			this.outputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputBox.BackColor = System.Drawing.Color.SkyBlue;
			this.outputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.outputBox.Location = new System.Drawing.Point(12, 34);
			this.outputBox.Name = "outputBox";
			this.outputBox.ReadOnly = true;
			this.outputBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.outputBox.Size = new System.Drawing.Size(460, 267);
			this.outputBox.TabIndex = 0;
			this.outputBox.Text = "";
			// 
			// inputBox
			// 
			this.inputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.inputBox.Enabled = false;
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
			// lb_nick
			// 
			this.lb_nick.AutoSize = true;
			this.lb_nick.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lb_nick.Location = new System.Drawing.Point(12, 9);
			this.lb_nick.Name = "lb_nick";
			this.lb_nick.Size = new System.Drawing.Size(46, 18);
			this.lb_nick.TabIndex = 3;
			this.lb_nick.Text = "Nick: ";
			// 
			// txt_name
			// 
			this.txt_name.Location = new System.Drawing.Point(64, 7);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new System.Drawing.Size(119, 20);
			this.txt_name.TabIndex = 4;
			// 
			// btn_login
			// 
			this.btn_login.Location = new System.Drawing.Point(189, 5);
			this.btn_login.Name = "btn_login";
			this.btn_login.Size = new System.Drawing.Size(75, 23);
			this.btn_login.TabIndex = 5;
			this.btn_login.Text = "Login";
			this.btn_login.UseVisualStyleBackColor = true;
			this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
			// 
			// lb_to
			// 
			this.lb_to.AutoSize = true;
			this.lb_to.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lb_to.Location = new System.Drawing.Point(270, 6);
			this.lb_to.Name = "lb_to";
			this.lb_to.Size = new System.Drawing.Size(34, 18);
			this.lb_to.TabIndex = 6;
			this.lb_to.Text = "To: ";
			// 
			// txt_receiver
			// 
			this.txt_receiver.Location = new System.Drawing.Point(310, 7);
			this.txt_receiver.Name = "txt_receiver";
			this.txt_receiver.Size = new System.Drawing.Size(162, 20);
			this.txt_receiver.TabIndex = 7;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.SkyBlue;
			this.ClientSize = new System.Drawing.Size(484, 362);
			this.Controls.Add(this.txt_receiver);
			this.Controls.Add(this.lb_to);
			this.Controls.Add(this.btn_login);
			this.Controls.Add(this.txt_name);
			this.Controls.Add(this.lb_nick);
			this.Controls.Add(this.log);
			this.Controls.Add(this.inputBox);
			this.Controls.Add(this.outputBox);
			this.Name = "Form1";
			this.Text = "ViComm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.RichTextBox outputBox;
		public System.Windows.Forms.TextBox inputBox;
		public System.Windows.Forms.Label log;
		private System.Windows.Forms.Label lb_nick;
		private System.Windows.Forms.TextBox txt_name;
		private System.Windows.Forms.Button btn_login;
		private System.Windows.Forms.Label lb_to;
		public System.Windows.Forms.TextBox txt_receiver;

	}
}

