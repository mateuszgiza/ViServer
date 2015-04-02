namespace ViComm
{
	partial class FormRegister
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
			this.label1 = new System.Windows.Forms.Label();
			this.tb_login = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tb_mail = new System.Windows.Forms.TextBox();
			this.tb_repwd = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tb_pwd = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label1.ForeColor = System.Drawing.SystemColors.Control;
			this.label1.Location = new System.Drawing.Point(79, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "Login";
			// 
			// tb_login
			// 
			this.tb_login.Location = new System.Drawing.Point(24, 45);
			this.tb_login.Name = "tb_login";
			this.tb_login.Size = new System.Drawing.Size(150, 20);
			this.tb_login.TabIndex = 1;
			this.tb_login.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_login.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_KeyDown);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label2.ForeColor = System.Drawing.SystemColors.Control;
			this.label2.Location = new System.Drawing.Point(241, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "E-mail";
			// 
			// tb_mail
			// 
			this.tb_mail.Location = new System.Drawing.Point(188, 45);
			this.tb_mail.Name = "tb_mail";
			this.tb_mail.Size = new System.Drawing.Size(150, 20);
			this.tb_mail.TabIndex = 3;
			this.tb_mail.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_mail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_KeyDown);
			// 
			// tb_repwd
			// 
			this.tb_repwd.Location = new System.Drawing.Point(188, 89);
			this.tb_repwd.Name = "tb_repwd";
			this.tb_repwd.Size = new System.Drawing.Size(150, 20);
			this.tb_repwd.TabIndex = 7;
			this.tb_repwd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_repwd.UseSystemPasswordChar = true;
			this.tb_repwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_KeyDown);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label3.ForeColor = System.Drawing.SystemColors.Control;
			this.label3.Location = new System.Drawing.Point(212, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(103, 18);
			this.label3.TabIndex = 6;
			this.label3.Text = "Verify Password";
			// 
			// tb_pwd
			// 
			this.tb_pwd.Location = new System.Drawing.Point(24, 89);
			this.tb_pwd.Name = "tb_pwd";
			this.tb_pwd.Size = new System.Drawing.Size(150, 20);
			this.tb_pwd.TabIndex = 5;
			this.tb_pwd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_pwd.UseSystemPasswordChar = true;
			this.tb_pwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_KeyDown);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label4.ForeColor = System.Drawing.SystemColors.Control;
			this.label4.Location = new System.Drawing.Point(67, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 18);
			this.label4.TabIndex = 4;
			this.label4.Text = "Password";
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button1.Location = new System.Drawing.Point(24, 115);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(314, 30);
			this.button1.TabIndex = 8;
			this.button1.Text = "Register";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// FormRegister
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DarkSlateGray;
			this.ClientSize = new System.Drawing.Size(362, 169);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tb_repwd);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tb_pwd);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tb_mail);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tb_login);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRegister";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Register";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRegister_FormClosing);
			this.Load += new System.EventHandler(this.FormRegister_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tb_login;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tb_mail;
		private System.Windows.Forms.TextBox tb_repwd;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tb_pwd;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button1;
	}
}