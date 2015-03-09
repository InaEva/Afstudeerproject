/*
 * Created by SharpDevelop.
 * User: DG
 * Date: 26-10-2014
 * Time: 22:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Delta
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.ExitButton = new System.Windows.Forms.Button();
			this.console = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// ExitButton
			// 
			this.ExitButton.BackColor = System.Drawing.Color.White;
			this.ExitButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.ExitButton.Location = new System.Drawing.Point(170, 261);
			this.ExitButton.Name = "ExitButton";
			this.ExitButton.Size = new System.Drawing.Size(75, 23);
			this.ExitButton.TabIndex = 0;
			this.ExitButton.Text = "Exit";
			this.ExitButton.UseVisualStyleBackColor = false;
			this.ExitButton.Click += new System.EventHandler(this.ExitButtonClick);
			// 
			// console
			// 
			this.console.BackColor = System.Drawing.Color.Black;
			this.console.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.console.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.console.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.console.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.console.Location = new System.Drawing.Point(45, 12);
			this.console.Multiline = true;
			this.console.Name = "console";
			this.console.ReadOnly = true;
			this.console.Size = new System.Drawing.Size(345, 63);
			this.console.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(434, 312);
			this.Controls.Add(this.console);
			this.Controls.Add(this.ExitButton);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Delta";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox console;
		private System.Windows.Forms.Button ExitButton;
	}
}
