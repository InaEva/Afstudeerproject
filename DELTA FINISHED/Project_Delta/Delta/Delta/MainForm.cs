using System;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace Delta
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private Delta delta;
		private Thread delta_thread;

		public MainForm()
		{
			InitializeComponent();
			delta_thread = new Thread(() =>
			                      {
			                      	delta = new Delta(this,console);	
			                      }
			                     );
			delta_thread.IsBackground = true;
			delta_thread.Start();
		}

		public void ExitButtonClick(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}
	}
}
