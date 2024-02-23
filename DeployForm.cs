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
    public partial class DeployForm : Form
    {
        public DeployForm()
        {
            InitializeComponent();
        }

        public void AddDeployItems(List<string> items)
        {
            foreach (var item in items)
            {
                // Erstellt einen neuen ListViewItem und fügt ihn zur ListView hinzu
                DeployListView.Items.Add(new ListViewItem(item));
            }
        }

    }
}
