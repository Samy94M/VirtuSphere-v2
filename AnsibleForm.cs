using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static VirtuSphere.ApiService;

namespace VirtuSphere
{


    public partial class AnsibleForm : Form
    {
        private List<VM> vms;

        // mission dir missionName tmp

        private string PathTmp = Path.GetTempPath();
        private string ProjectPathTmp = "";

        public void SetMissionName(string missionName)
        {
            // Verwende missionName hier, z.B. um einen Label-Text zu setzen
            this.labelMissionName.Text = missionName;
            ProjectPathTmp = Path.Combine(PathTmp, missionName);

            // selectiere erstes item in listFiles
            if (listFiles.Items.Count > 0)
            {
                listFiles.Items[0].Selected = true;
            }
        }


        public AnsibleForm(List<VM> vms)
        {
            InitializeComponent();

            // comboAction
            this.vms = vms;



        }

        public bool modifiziert = false;
        public bool view_modifiziert = false;
        private string selectedItem;


        private void loadConfig(object sender, EventArgs e)
        {
            // Sicherstellen, dass die Auswahl gültig ist
            if (listFiles.SelectedItems.Count > 0)
            {
                string selectedItem = listFiles.SelectedItems[0].Text;
                // Anzeigen der Auswahl in einer MessageBox
                //MessageBox.Show("Ausgewählter Eintrag: " + selectedItem, "Auswahl", MessageBoxButtons.OK, MessageBoxIcon.Information);

                String path = Path.Combine(ProjectPathTmp, selectedItem);
                // prüfe ob Filepath existiert
                if (!File.Exists(path))
                {
                    MessageBox.Show("Datei existiert nicht", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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
                btnSave.Visible = true;
            }
            else
            {
                txtAnsible.Enabled = false;
                btnSave.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // combind temp und mission
            // öffne den Explorer im Ordner Temp
            System.Diagnostics.Process.Start(ProjectPathTmp);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (listFiles.SelectedItems.Count > 0)
            { 
                string selectedItem = listFiles.SelectedItems[0].Text;

                Console.WriteLine("Reload Datei: " + selectedItem);

                String path = Path.Combine(ProjectPathTmp, selectedItem);
                // prüfe ob Filepath existiert
                if (File.Exists(path))
                {
                    string content = File.ReadAllText(path);
                    content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                    txtAnsible.Text = content;
                    Console.WriteLine("Datei reloaded.");
                }
        }

            // leere listFiles
            listFiles.Items.Clear();
            // fülle listFiles mit Dateien aus dem Ordner
            foreach (string file in Directory.GetFiles(ProjectPathTmp))
            {
                listFiles.Items.Add(Path.GetFileName(file));
            }

            // wähle in listFiles test.yml aus
            foreach (ListViewItem item in listFiles.Items)
            {
                if (item.Text == selectedItem)
                {
                    item.Selected = true;
                    break;
                }
            }
            

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems.Count > 0)
            {
                Console.WriteLine("Speichere Datei");

                string selectedItem = listFiles.SelectedItems[0].Text;
                String path = Path.Combine(ProjectPathTmp, selectedItem);
                // prüfe ob Filepath existiert
                if (File.Exists(path))
                {
                    File.WriteAllText(path, txtAnsible.Text);
                }   
                MessageBox.Show("Datei gespeichert", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Keine Datei ausgewählt", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
