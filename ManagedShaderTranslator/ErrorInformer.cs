using System;

namespace System.Diagnostics {
	public sealed class ErrorInformer : System.Windows.Forms.Form {
		private System.Windows.Forms.RichTextBox richTextBox1;

		public ErrorInformer(Exception ex) : this(ex, null) {
		}

		public ErrorInformer(Exception ex, string message) {
			InitializeComponent();
			if (message != null)
				richTextBox1.Text += message + "\n";
			for (int stack = 0; ex != null; stack++) {
				if (stack == 11) {
					richTextBox1.Text += "\n(More inner exceptions exist.)";
					return;
				}
				richTextBox1.Text += string.Concat("\nInner exception level ", stack.ToString(), ":\n\nMessage:\n\n   ", (ex.Message == null ? "Not specified." : ex.Message), "\n\nType:\n\n   ", ex.GetType().ToString(), "\n\nTarget Site:\n\n   ", ex.TargetSite.ToString(), "\n\nSource:\n\n   ", ex.Source, "\n\nStack Trace:\n\n", ex.StackTrace.ToString(), "\n\n\n");
				ex = ex.InnerException;
			}
		}

		private void InitializeComponent() {
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(553, 438);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "An error occurred. Exception details:";
			this.richTextBox1.WordWrap = false;
			// 
			// ErrorInformer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Menu;
			this.ClientSize = new System.Drawing.Size(553, 438);
			this.Controls.Add(this.richTextBox1);
			this.Font = new System.Drawing.Font("Calibri Light", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorInformer";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Error Informer";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
	}
}