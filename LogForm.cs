using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
