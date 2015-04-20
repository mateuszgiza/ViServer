using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ViComm
{
	public partial class ChatBox : UserControl
	{
		private List<Label> _messages;
		private Point lastPosition;
		private int lastHeight = 0;
		private int maxWidth;

		public Font CurrentFont;
		public Color color = SystemColors.ScrollBar;
		public Color backColor = Color.Transparent;

		public ChatBox()
		{
			InitializeComponent();

			this.VerticalScroll.Enabled = true;
			this.VerticalScroll.Visible = true;

			_messages = new List<Label>();
			lastPosition = new Point(10, 0);
			maxWidth = this.Width - 2*10 - SystemInformation.VerticalScrollBarWidth;
		}

		public Font SetFont
		{
			set
			{
				this.CurrentFont = value;
			}
		}

		public bool IsNotEmpty
		{
			get
			{
				return _messages.Count > 0 ? true : false;
			}
		}

		public void ScrollToCarret()
		{
			this.VerticalScroll.Value = this.VerticalScroll.Maximum;

			this.HorizontalScroll.Enabled = false;
			this.HorizontalScroll.Visible = false;
			panel1.HorizontalScroll.Enabled = false;
			panel1.HorizontalScroll.Visible = false;
		}

		private Label GetLast()
		{
			return this._messages[_messages.Count - 1];
		}

		public void Add(string text)
		{
			panel1.Width = this.Width - 5 - SystemInformation.VerticalScrollBarWidth;
			maxWidth = this.Width - 2 * lastPosition.X;

			Label l = new Label();
			l.Text = text;
			l.Location = new Point(lastPosition.X, lastPosition.Y + lastHeight);
			l.MaximumSize = new System.Drawing.Size(maxWidth, 0);
			l.AutoSize = true;
			l.BackColor = backColor;
			l.ForeColor = color;
			l.Font = CurrentFont;
			l.Margin = new System.Windows.Forms.Padding(5);

			lastPosition = l.Location;

			using (Graphics g = CreateGraphics()) {
				lastHeight = (int) g.MeasureString(l.Text, l.Font, maxWidth).Height;
			}

			_messages.Add(l);

			panel1.Controls.Add(_messages[_messages.Count - 1]);
			panel1.Height = lastPosition.Y + lastHeight;
		}
	}
}
