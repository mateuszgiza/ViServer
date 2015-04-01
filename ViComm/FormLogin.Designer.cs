namespace ViComm
{
	partial class FormLogin
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
			this.button1 = new System.Windows.Forms.Button();
			this.tb_pwd = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tb_login = new System.Windows.Forms.TextBox();
			this.lb_register = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label1.ForeColor = System.Drawing.SystemColors.Control;
			this.label1.Location = new System.Drawing.Point(99, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 18);
			this.label1.TabIndex = 9;
			this.label1.Text = "Login";
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.button1.Location = new System.Drawing.Point(19, 115);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(200, 30);
			this.button1.TabIndex = 13;
			this.button1.Text = "Login";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tb_pwd
			// 
			this.tb_pwd.Location = new System.Drawing.Point(19, 89);
			this.tb_pwd.Name = "tb_pwd";
			this.tb_pwd.Size = new System.Drawing.Size(200, 20);
			this.tb_pwd.TabIndex = 12;
			this.tb_pwd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_pwd.UseSystemPasswordChar = true;
			this.tb_pwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_pwd_KeyDown);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label4.ForeColor = System.Drawing.SystemColors.Control;
			this.label4.Location = new System.Drawing.Point(87, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 18);
			this.label4.TabIndex = 11;
			this.label4.Text = "Password";
			// 
			// tb_login
			// 
			this.tb_login.Location = new System.Drawing.Point(19, 45);
			this.tb_login.Name = "tb_login";
			this.tb_login.Size = new System.Drawing.Size(200, 20);
			this.tb_login.TabIndex = 10;
			this.tb_login.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lb_register
			// 
			this.lb_register.AutoSize = true;
			this.lb_register.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lb_register.Font = new System.Drawing.Font("Sylfaen", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lb_register.ForeColor = System.Drawing.SystemColors.ButtonShadow;
			this.lb_register.Location = new System.Drawing.Point(16, 148);
			this.lb_register.Name = "lb_register";
			this.lb_register.Size = new System.Drawing.Size(87, 18);
			this.lb_register.TabIndex = 9;
			this.lb_register.Text = "Register now!";
			this.lb_register.Click += new System.EventHandler(this.lb_register_Click);
			this.lb_register.MouseEnter += new System.EventHandler(this.lb_register_MouseEnter);
			this.lb_register.MouseLeave += new System.EventHandler(this.lb_register_MouseLeave);
			// 
			// FormLogin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DarkSlateGray;
			this.ClientSize = new System.Drawing.Size(234, 191);
			this.Controls.Add(this.lb_register);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tb_pwd);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tb_login);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FormLogin";
			this.Text = "FormLogin";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
			this.Load += new System.EventHandler(this.FormLogin_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox tb_pwd;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tb_login;
		private System.Windows.Forms.Label lb_register;
	}
}