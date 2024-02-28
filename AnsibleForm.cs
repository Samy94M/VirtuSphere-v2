using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static VirtuSphere.ApiService;

namespace VirtuSphere
{
    

    public partial class AnsibleForm : Form
    {
        private List<VM> vms;


        public AnsibleForm(List<VM> vms)
        {
            InitializeComponent();

            // comboAction
            combo_action.Items.Add("unmodifiziert");
            combo_action.SelectedItem = 0;
            this.vms = vms;

        }

        public bool modifiziert = false;
        public bool view_modifiziert = false;


        private void generateConfigs(object sender, EventArgs e)
        {
            modifiziert = true;
            string serverlist = "vm_configurations:\n";
            string interfaces = "";
            string packages = "";



            //vm_configurations:
            //    -vm_name: "TestDaniel"
            //    memory: 4096
            //    vcpus: 2
            //    disk_size: 50
            //    network: "SIDS_SRV_3_Data"
            //    datastore_name: "DatenSSD"
            //    datacenter_name: "ha-datacenter"
            //    guest_id: "windows2019srv_64Guest"
            //    packages: "Testgruppe;role-install-dhcp;role-install-fs"
            //    deployment: "ws2k19-standard"


            // liste vms in console auf
            foreach (var vm in vms)
            {

                // Netzwerk und Packages sind eine Liste und die Werte werden durch ein Semikolon getrennt
                foreach (Package package in vm.packages)
                {
                    packages += package.package_name + ";";
                }

                foreach (Interface network in vm.interfaces)
                {
                    interfaces += network.vlan + ";";
                }



                Console.WriteLine("Füge zur Serverliste hinzu: " + vm.vm_name);
                serverlist += "  - vm_name: " + vm.vm_name + "\n";
                serverlist += "    memory: " + vm.vm_ram + "\n";
                serverlist += "    vcpus: " + vm.vm_cpu + "\n";
                serverlist += "    disk_size: " + vm.vm_disk + "\n";
                serverlist += "    network: " + interfaces + "\n";
                serverlist += "    datastore_name: " + vm.vm_datastore + "\n";
                serverlist += "    datacenter_name: " + vm.vm_datacenter + "\n";
                serverlist += "    guest_id: " + vm.vm_guest_id + "\n";
                serverlist += "    packages: " + packages + "\n";
                serverlist += "    os   : " + vm.vm_os + "\n";
            }

            // erstelle eine Datei (serverlist.yml) mit dem Inhalt von serverlist unter tmp
            string path = Path.GetTempPath();
            // wenn datei serverlist_modi.yml existiert, lösche diese
            if (File.Exists(Path.Combine(path, "serverlist_modi.yml")))
            {
                File.Delete(Path.Combine(path, "serverlist_modi.yml"));
            }

            File.WriteAllText(Path.Combine(path, "serverlist_modi.yml"), serverlist);

            // füge dn Pfad der Datei in die Liste hinzu
            listFiles.Items.Add("serverlist_modi.yml");



            combo_action.Items.Add("modifiziert");
        }


        private void loadConfig(object sender, EventArgs e)
        {
            // Sicherstellen, dass die Auswahl gültig ist
            if (listFiles.SelectedItems.Count > 0)
            {
                string selectedItem = listFiles.SelectedItems[0].Text;
                // Anzeigen der Auswahl in einer MessageBox
                //MessageBox.Show("Ausgewählter Eintrag: " + selectedItem, "Auswahl", MessageBoxButtons.OK, MessageBoxIcon.Information);

                String path = Path.Combine(Path.GetTempPath(), selectedItem);
                // Lese den Inhalt der Datei
                string content = File.ReadAllText(path);

                // Ersetzt Unix- und Mac-Zeilenumbrüche durch Windows-Zeilenumbrüche
                content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");


                // Schreibe den Inhalt in txtAnsible
                txtAnsible.Text = content;
            }
        }

        public void viewConfigs(object sender, EventArgs e)
        {
            view_modifiziert = true;
            combo_action.SelectedItem = "modifiziert";


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //txtAnsible.toggle
            if (checkBox1.Checked)
            {
                txtAnsible.Enabled = true;
            }
            else
            {
                txtAnsible.Enabled = false;
            }
        }
    }
}
