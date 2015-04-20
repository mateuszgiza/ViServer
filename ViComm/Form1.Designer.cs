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
			this.inputBox = new System.Windows.Forms.TextBox();
			this.log = new System.Windows.Forms.Label();
			this.btn_login = new System.Windows.Forms.Button();
			this.lb_nick = new System.Windows.Forms.Label();
			this.list_contacts = new System.Windows.Forms.ListBox();
			this.lb_contacts = new System.Windows.Forms.Label();
			this.separator1 = new System.Windows.Forms.Label();
			this.chatBox1 = new ViComm.ChatBox();
			this.SuspendLayout();
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
			this.inputBox.TextChanged += new System.EventHandler(this.inputBox_TextChanged);
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
			// btn_login
			// 
			this.btn_login.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_login.Location = new System.Drawing.Point(397, 5);
			this.btn_login.Name = "btn_login";
			this.btn_login.Size = new System.Drawing.Size(75, 23);
			this.btn_login.TabIndex = 5;
			this.btn_login.Text = "Logout";
			this.btn_login.UseVisualStyleBackColor = true;
			this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
			// 
			// lb_nick
			// 
			this.lb_nick.AutoSize = true;
			this.lb_nick.BackColor = System.Drawing.Color.Transparent;
			this.lb_nick.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lb_nick.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.lb_nick.Location = new System.Drawing.Point(9, 5);
			this.lb_nick.Name = "lb_nick";
			this.lb_nick.Size = new System.Drawing.Size(46, 21);
			this.lb_nick.TabIndex = 1001;
			this.lb_nick.Text = "Nick: ";
			// 
			// list_contacts
			// 
			this.list_contacts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.list_contacts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(78)))), ((int)(((byte)(84)))));
			this.list_contacts.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.list_contacts.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.list_contacts.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.list_contacts.FormattingEnabled = true;
			this.list_contacts.ItemHeight = 20;
			this.list_contacts.Location = new System.Drawing.Point(480, 30);
			this.list_contacts.Name = "list_contacts";
			this.list_contacts.ScrollAlwaysVisible = true;
			this.list_contacts.Size = new System.Drawing.Size(150, 300);
			this.list_contacts.Sorted = true;
			this.list_contacts.TabIndex = 1002;
			// 
			// lb_contacts
			// 
			this.lb_contacts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lb_contacts.AutoSize = true;
			this.lb_contacts.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lb_contacts.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.lb_contacts.Location = new System.Drawing.Point(478, 7);
			this.lb_contacts.Name = "lb_contacts";
			this.lb_contacts.Size = new System.Drawing.Size(77, 21);
			this.lb_contacts.TabIndex = 1003;
			this.lb_contacts.Text = "Contacts: ";
			// 
			// separator1
			// 
			this.separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.separator1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.separator1.Location = new System.Drawing.Point(475, 10);
			this.separator1.Name = "separator1";
			this.separator1.Size = new System.Drawing.Size(2, 320);
			this.separator1.TabIndex = 1005;
			// 
			// chatBox1
			// 
			this.chatBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chatBox1.AutoScroll = true;
			this.chatBox1.BackColor = System.Drawing.Color.Transparent;
			this.chatBox1.Location = new System.Drawing.Point(12, 30);
			this.chatBox1.Name = "chatBox1";
			this.chatBox1.Size = new System.Drawing.Size(460, 271);
			this.chatBox1.TabIndex = 1004;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(78)))), ((int)(((byte)(84)))));
			this.ClientSize = new System.Drawing.Size(630, 362);
			this.Controls.Add(this.separator1);
			this.Controls.Add(this.chatBox1);
			this.Controls.Add(this.lb_contacts);
			this.Controls.Add(this.list_contacts);
			this.Controls.Add(this.lb_nick);
			this.Controls.Add(this.btn_login);
			this.Controls.Add(this.log);
			this.Controls.Add(this.inputBox);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ViComm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.TextBox inputBox;
		public System.Windows.Forms.Label log;
		public System.Windows.Forms.Button btn_login;
		private System.Windows.Forms.Label lb_nick;
		private System.Windows.Forms.Label lb_contacts;
		public System.Windows.Forms.ListBox list_contacts;
		public ChatBox chatBox1;
		private System.Windows.Forms.Label separator1;

	}
}

