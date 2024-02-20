using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace VirtuSphere
{
    public partial class FMmain : Form
    {
        public object JsonConvert { get; private set; }
        public string Token { get; private set; }

        public FMmain()
        {
            InitializeComponent();


            comboVLAN.Items.Add("Item 1");
            comboVLAN.Items.Add("Item 2");
            comboVLAN.Items.Add("Item 3");

            comboVLAN.SelectedIndex = 0;


            MessageBox.Show("Token: " + Token);



            // fülle listBox1 mit 50 testdaten
            for (int i = 0; i < 50; i++)
            {
                listBox1.Items.Add("Item " + i);
            }
        }

  

        // erstelle eine methode zum leeren der textfelder
        private void ClearTextBoxes()
        {
            txtHostname.Text = "";
            txtIP.Text = "";
            txtSubnet.Text = "";
            txtGateway.Text = "";
            txtDNS1.Text = "";
            txtDNS2.Text = "";
            txtDomain.Text = "";
            comboVLAN.SelectedIndex = 0;
            listBox1.SelectedItems.Clear();
        }

        // erstelle eine methode zum aktivieren /deaktivieren der buttons
        private void EnableButtons()
        {
            btn_delete.Enabled = true;
            btn_edit.Enabled = true;
            btn_clear.Enabled = true;
        }

        // erstelle eine methode zum deaktivieren der buttons
        private void DisableButtons()
        {
            btn_delete.Enabled = false;
            btn_edit.Enabled = false;
            btn_clear.Enabled = false;
        }

        // erstelle eine methode zum export der listView1 in eine CSV-Datei
        private void ExportToCSV()
        {
            // erstelle einen neuen SaveFileDialog
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV-Datei|*.csv";
            sfd.Title = "Speichern als CSV-Datei";
            sfd.FileName = "export.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // erstelle eine neue Instanz von StringBuilder
                StringBuilder sb = new StringBuilder();
                // füge die Spaltennamen hinzu
                sb.AppendLine("Hostname;IP;Subnet;Gateway;DNS1;DNS2;Domain;VLAN;Tags");
                // füge die Daten der ListView hinzu
                foreach (ListViewItem lvi in listView1.Items)
                {

                    sb.AppendLine(string.Join(";", lvi.Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text, lvi.SubItems[3].Text, lvi.SubItems[4].Text, lvi.SubItems[5].Text, lvi.SubItems[6].Text, lvi.SubItems[7].Text, lvi.SubItems[8].Text));
                }
                // speichere die CSV-Datei
                System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
            }
        }

        // erstelle eine methode zum import einer CSV-Datei in die ListView. Überspringe die Erste Line
        private void ImportFromCSV()
        {
            // erstelle einen neuen OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV-Datei|*.csv";
            ofd.Title = "Öffnen einer CSV-Datei";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // lösche alle Einträge in der ListView
                listView1.Items.Clear();
                // lese die CSV-Datei
                string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                // füge die Daten in die ListView ein
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] items = lines[i].Split(';');
                    ListViewItem lvi = new ListViewItem(items[0]);
                    lvi.SubItems.Add(items[1]);
                    lvi.SubItems.Add(items[2]);
                    lvi.SubItems.Add(items[3]);
                    lvi.SubItems.Add(items[4]);
                    lvi.SubItems.Add(items[5]);
                    lvi.SubItems.Add(items[6]);
                    lvi.SubItems.Add(items[7]);
                    lvi.SubItems.Add(items[8]);
                    listView1.Items.Add(lvi);
                }
            }
        }

        // ...

        // schreibe eine methode die http://localhost:8080/access.php?action=getMissions&token=your_token aufruft (json) und die daten in comboBox2 lädt
        private async void LoadMissions(string Token, string Hostname)
        {
            MessageBox.Show("LoadMissions: " + Token);

            using (HttpClient client = new HttpClient())
            {
                // Füge den Token hinzu
                client.DefaultRequestHeaders.Add("token", Token);

                try
                {
                    // Rufe die URL asynchron auf
                    string response = await client.GetStringAsync("http://" + Hostname + "/access.php?action=getMissions");

                    // Deserialisiere die JSON-Daten
                    dynamic missions = JsonConvert.DeserializeObject(response);

                    // Füge die Daten in comboBox2 ein
                    foreach (var mission in missions)
                    {
                        comboBox2.Items.Add((string)mission.name);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden der Missionen: " + ex.Message);
                }
            }
        }


        // schreibe eine methode die http://localhost:8080/access.php?action=getPackages&token=your_token aufruft (json) und in listBox1 lädt
        private void LoadPackages(String Token, String Hostname)
        {
            MessageBox.Show("LoadPackages" + Token);


            // erstelle eine neue Instanz von HttpClient
            HttpClient client = new HttpClient();
            // füge den token hinzu
            client.DefaultRequestHeaders.Add("token", Token);
            // rufe die URL auf
            string response = client.GetStringAsync("http://"+Hostname+"/access.php?action=getPackages").Result;
            // deserialisiere die JSON-Daten
            dynamic packages = JsonConvert.DeserializeObject(response);
            // füge die Daten in listBox1 ein
            foreach (var package in packages)
            {
                listBox1.Items.Add(package.name);
            }
        }

        // schreibe eine methode die http://localhost:8080/access.php?action=getVMS&mission=(comboBox2.text)&token=your_token aufruft (json) und in listView1 lädt
        private void LoadVMS(String Token, String Hostname)
        {
            // erstelle eine neue Instanz von HttpClient
            HttpClient client = new HttpClient();
            // füge den token hinzu
            client.DefaultRequestHeaders.Add("token", Token);
            // rufe die URL auf
            string response = client.GetStringAsync("http://"+Hostname+"/access.php?action=getVMS&mission=" + comboBox2.Text).Result;
            // deserialisiere die JSON-Daten
            dynamic vms = JsonConvert.DeserializeObject(response);
            // füge die Daten in listView1 ein
            foreach (var vm in vms)
            {
                ListViewItem lvi = new ListViewItem(vm.hostname);
                lvi.SubItems.Add(vm.ip);
                lvi.SubItems.Add(vm.subnet);
                lvi.SubItems.Add(vm.gateway);
                lvi.SubItems.Add(vm.dns1);
                lvi.SubItems.Add(vm.dns2);
                lvi.SubItems.Add(vm.domain);
                lvi.SubItems.Add(vm.vlan);
                lvi.SubItems.Add(vm.tags);
                listView1.Items.Add(lvi);
            }
        }



        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // füge txtHostname in ListView ein
            ListViewItem lvi = new ListViewItem(txtHostname.Text);
            lvi.SubItems.Add(txtIP.Text);
            lvi.SubItems.Add(txtSubnet.Text);
            lvi.SubItems.Add(txtGateway.Text);
            lvi.SubItems.Add(txtDNS1.Text);
            lvi.SubItems.Add(txtDNS2.Text);
            lvi.SubItems.Add(txtDomain.Text);
            lvi.SubItems.Add("unbekannt");
            // füge VLAN hinzu
            lvi.SubItems.Add(comboVLAN.Text);
            // listBox1 (MultiSelect) mit Semikolon getrennt hinzufügen
            string selectedItems = "";
            foreach (var item in listBox1.SelectedItems)
            {
                selectedItems += item.ToString() + ";";
            }
            lvi.SubItems.Add(selectedItems);

            LogForm logForm = new LogForm();
            logForm.AddLog("Dies ist eine Log-Nachricht.");


            listView1.Items.Add(lvi);
            ClearTextBoxes();
            DisableButtons();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            // bearbeitet das ausgewählte Item in der ListView mit den werten aus den textfeldern
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView1.SelectedItems[0];
                lvi.Text = txtHostname.Text;
                lvi.SubItems[1].Text = txtIP.Text;
                lvi.SubItems[2].Text = txtSubnet.Text;
                lvi.SubItems[3].Text = txtGateway.Text;
                lvi.SubItems[4].Text = txtDNS1.Text;
                lvi.SubItems[5].Text = txtDNS2.Text;
                lvi.SubItems[6].Text = txtDomain.Text;
                lvi.SubItems[7].Text = comboVLAN.Text;
                // listBox1 (MultiSelect) mit Semikolon getrennt hinzufügen
                string selectedItems = "";
                foreach (var item in listBox1.SelectedItems)
                {
                    selectedItems += item.ToString() + ";";
                }
                lvi.SubItems[8].Text = selectedItems;
                ClearTextBoxes();
                DisableButtons();
            }

        }


        // bei doppelklick auf ein item in der listView1, werden die werte in die textfelder geschrieben
        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView1.SelectedItems[0];
                txtHostname.Text = lvi.Text;
                txtIP.Text = lvi.SubItems[1].Text;
                txtSubnet.Text = lvi.SubItems[2].Text;
                txtGateway.Text = lvi.SubItems[3].Text;
                txtDNS1.Text = lvi.SubItems[4].Text;
                txtDNS2.Text = lvi.SubItems[5].Text;
                txtDomain.Text = lvi.SubItems[6].Text;
                comboVLAN.Text = lvi.SubItems[7].Text;
                // listBox1 (MultiSelect) mit Semikolon getrennt hinzufügen
                string[] selectedItems = lvi.SubItems[8].Text.Split(';');
                foreach (var item in selectedItems)
                {
                    listBox1.SelectedItems.Add(item);
                }

                EnableButtons();
            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            // lösche das ausgewählte Item aus der ListView und macht die Textboxen leer
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.Items.Remove(listView1.SelectedItems[0]);
                ClearTextBoxes();
                DisableButtons();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            DisableButtons();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // exportiere die ListView in eine CSV-Datei
            ExportToCSV();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            // importiere eine CSV-Datei in die ListView
            ImportFromCSV();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            // öffne das LogForm und füge eine Log-Nachricht hinzu (dies ist nur ein Beispiel) 
            LogForm logForm = new LogForm();
            logForm.AddLog("Dies ist eine Log-Nachricht.");
            logForm.ShowDialog();


        }
    }
}
