using System;
using System.Windows.Forms;

namespace VirtuSphere
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void AddLog(string message)
        {
            // Stelle sicher, dass dieser Aufruf im UI-Thread erfolgt
            if (lsLog.InvokeRequired)
            {
                lsLog.Invoke(new Action(() => AddLog(message)));
                return;
            }

            lsLog.Items.Add(message);
        }
    }


}
