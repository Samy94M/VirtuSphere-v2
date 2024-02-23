using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static VirtuSphere.ApiService;


namespace VirtuSphere
{
    public partial class FMmain : Form
    {

        public string hostname { get; set; }
        public string Token { get; set; }
        public int missionId { get; set; }
        public string missionName { get; set; }
        public ApiService ApiService { get; set; }
        public List<VM> vms = new List<VM>();
        public List<VM> vmListToDelete = new List<VM>();
        public List<VM> vmListToCreate = new List<VM>();
        public List<VM> vmListToUpdate = new List<VM>();
        public List<MissionItem> missionItems = new List<MissionItem>();



        public object JsonConvert { get; private set; }

        public FMmain()
        {


            InitializeComponent();

            DisableInputFields();

        }


        public async void btn_loadVMsfromDB(object sender, EventArgs e)
        {
            if(missionBox.SelectedIndex != -1)
            {
                MissionItem selectedItem = missionBox.SelectedItem as MissionItem;
                if (selectedItem != null)
                {
                    //MessageBox.Show($"Die ID der ausgewählten Mission ist: {selectedItem.Id}");

                   // DialogResult result = MessageBox.Show("Möchten Sie die Liste der VMs aus der Datenbank laden?", "Bestätigung", MessageBoxButtons.YesNo);

                  //  if (result == DialogResult.Yes)
                   // {
                        // leere listView1 und fülle sie mit den VMs aus der Datenbank
                        listView1.Items.Clear();
                        ClearTextBoxes();

                        missionId = selectedItem.Id;
                        missionName = selectedItem.mission_name;

                        // enable btn_add
                        if (missionId != 0) { btn_add.Enabled = true; EnableInputFields(); }

                       // MessageBox.Show("Lade VMs aus der Datenbank für die Mission " + selectedItem.mission_name + " mit der ID " + missionId);
                        Console.WriteLine("Lade VMs aus der Datenbank für die Mission " + selectedItem.mission_name + " mit der ID " + missionId);

                        // leere vms 
                        vms.Clear();
                        vms = await ApiService.GetVMs(hostname, Token, missionId);

                        if (vms != null && vms.Count > 0)
                        {
                            UpdateListView(vms);
                            EnableInputFields();
                            Console.WriteLine("Insgesamt " + vms.Count + " VMs gefunden.");

                            // Alle VMs in der Console ausgeben
                            foreach (VM vm in vms)
                            {
                                Console.WriteLine(vm.vm_name);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Keine VMs für "+selectedItem.mission_name+" in der Datenbank gefunden.");
                        }

                   // }
                }
                else
                {
                    MessageBox.Show("Mission kann nicht geladen werden, weil die noch nicht in der Datenbank angelegt wurde.");
                }
            }
        }
        private void btnAddClick(object sender, EventArgs e)
        {
            string packages = "";
            foreach (var item in listBoxPackages.SelectedItems)
            {
                packages += item.ToString() + ";";
                Console.WriteLine("Selected Packages Item für: " + txtName.Text + " " + item.ToString());
            }

            // Create a new VM object with the entered values
            VM vm = new VM
            {
                vm_name = txtName.Text,
                vm_hostname = txtHostname.Text,
                vm_ip = txtIP.Text,
                vm_subnet = txtSubnet.Text,
                vm_gateway = txtGateway.Text,
                vm_dns1 = txtDNS1.Text,
                vm_dns2 = txtDNS2.Text,
                vm_domain = txtDomain.Text,
                vm_vlan = comboVLAN.Text,
                vm_os = listBoxOS.Text,
                vm_packages = packages,
                vm_status = "Neu - DB Sync!"

            };

            if (ValidateInputFields(vm))
            {
                // Add the VM object to the vms list
                vms.Add(vm);
                vmListToCreate.Add(vm);

                LoadVMsIntoListView(vms);
                ClearTextBoxes();
                DisableButtons();
            }
        }

        private bool ValidateInputFields(VM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.vm_name) || string.IsNullOrWhiteSpace(vm.vm_hostname))
            {
                MessageBox.Show("Name und Hostname dürfen nicht leer sein.");
                return false;
            }

            if (!IPAddress.TryParse(vm.vm_ip, out _) || !IPAddress.TryParse(vm.vm_subnet, out _) ||
                !IPAddress.TryParse(vm.vm_gateway, out _) || !IPAddress.TryParse(vm.vm_dns1, out _) ||
                !IPAddress.TryParse(vm.vm_dns2, out _))
            {
                MessageBox.Show("Eine oder mehrere IP-Adressen sind ungültig.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(vm.vm_os))
            {
                MessageBox.Show("Bitte wähle ein Betriebssystem aus.");
                return false;
            }

            return true;
        }

        private bool ValidateDoubleItems(ListView listView, string newItem)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text == newItem)
                {
                    MessageBox.Show("Dieser Eintrag existiert bereits.");
                    return false;
                }
            }
            return true;
        }

        public Task LoadVMsIntoListView(List<VM> vms)
        {
            // Clear the listView1
            listView1.Items.Clear();

            // Iterate through the vms list and add each VM to the listView1
            foreach (var vm in vms)
            {
                ListViewItem lvi = new ListViewItem(new[] {
                    vm.vm_name,
                    vm.vm_hostname,
                    vm.vm_ip,
                    vm.vm_subnet,
                    vm.vm_gateway,
                    vm.vm_dns1,
                    vm.vm_dns2,
                    vm.vm_domain,
                    vm.vm_vlan,
                    vm.vm_os,
                    vm.vm_role,
                    vm.vm_status
                });

                listView1.Items.Add(lvi);


                lvi.Tag = vm;
            }

            return Task.CompletedTask;
        }

        private void btnEditClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {

                VM selectedVM = listView1.SelectedItems[0].Tag as VM; // Cast das Tag zurück zum VM-Objekt
                if (selectedVM != null)
                {
                    // Zeige jetzt Informationen aus dem VM-Objekt an, z.B.:
                   // MessageBox.Show($"ID: {selectedVM.Id}\nName: {selectedVM.vm_name}\nIP: {selectedVM.vm_ip}\nOS: {selectedVM.vm_os}");

                    // Bearbeite hier das Objekt in der Klasse vms
                    selectedVM.vm_name = txtName.Text;
                    selectedVM.vm_hostname = txtHostname.Text;
                    selectedVM.vm_ip = txtIP.Text;
                    selectedVM.vm_subnet = txtSubnet.Text;
                    selectedVM.vm_gateway = txtGateway.Text;
                    selectedVM.vm_dns1 = txtDNS1.Text;
                    selectedVM.vm_dns2 = txtDNS2.Text;
                    selectedVM.vm_domain = txtDomain.Text;
                    selectedVM.vm_vlan = comboVLAN.Text;
                    selectedVM.vm_os = listBoxOS.Text;
                    selectedVM.vm_status = "geändert - DB Sync!";

                    // Ausgewählte listBoxPackages Objekte sollen mit Semikolon getrennt in vm_packages gespeichert werden
                    string packages = "";
                    foreach (var item in listBoxPackages.SelectedItems)
                    {
                        packages += item.ToString() + ";";
                        Console.WriteLine("Selected Packages Item für: "+selectedVM.vm_name+" " + item.ToString());
                    }
                    selectedVM.vm_packages = packages;

                    if (selectedVM.Id != 0)
                    {
                        vmListToUpdate.Add(selectedVM);
                    }

                    // Update the listView1
                    UpdateListView(vms);
                }


                ClearTextBoxes();
                DisableButtons();
            }

        }
        void VMList_Click(object sender, EventArgs e)
        {
            // gib id aus
            if (listView1.SelectedItems.Count > 0)
            {
                VM selectedVM = listView1.SelectedItems[0].Tag as VM; // Cast das Tag zurück zum VM-Objekt
                if (selectedVM != null)
                {
                    // Zeige jetzt Informationen aus dem VM-Objekt an, z.B.:
                    //MessageBox.Show($"ID: {selectedVM.Id}\nName: {selectedVM.vm_name}\nIP: {selectedVM.vm_ip}\nOS: {selectedVM.vm_os}");
                    txtName.Text = selectedVM.vm_name;
                    txtHostname.Text = selectedVM.vm_hostname;
                    txtIP.Text = selectedVM.vm_ip;
                    txtSubnet.Text = selectedVM.vm_subnet;
                    txtGateway.Text = selectedVM.vm_gateway;
                    txtDNS1.Text = selectedVM.vm_dns1;
                    txtDNS2.Text = selectedVM.vm_dns2;
                    txtDomain.Text = selectedVM.vm_domain;
                    comboVLAN.Text = selectedVM.vm_vlan;
                    listBoxOS.Text = selectedVM.vm_os;
                    listBoxPackages.ClearSelected();

                    // selectedVM.packages in listBoxPackages eintragen
                    string[] packages = selectedVM.vm_packages.Split(';');

                    List<object> itemsToAdd = new List<object>();
                    foreach (var item in listBoxPackages.Items)
                    {
                        if (packages.Contains(item.ToString()))
                        {
                            itemsToAdd.Add(item);
                        }
                    }

                    foreach (var item in itemsToAdd)
                    {
                        listBoxPackages.SelectedItems.Add(item);
                    }

                        EnableButtons();
                }
                else
                {
                    // liste mir hier alle Objekte in vms auf
                    MessageBox.Show("Fehler beim Laden der VM-Daten.");

                    foreach (var vm in vms)
                    {
                        Console.WriteLine("VM-Objekt in der Klasse vms: " + vm.vm_name);
                        //console zeige tag
                        Console.WriteLine("Selected Tag: " + listView1.SelectedItems[0].Tag);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine VM aus.");
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // lösche das ausgewählte Item aus der ListView und macht die Textboxen leer
            if (listView1.SelectedItems.Count > 0)
            {
                //listView1.Items.Remove(listView1.SelectedItems[0]);
                VM selectedVM = listView1.SelectedItems[0].Tag as VM; // Cast das Tag zurück zum VM-Objekt
                if (selectedVM != null)
                {
                    // Frage ob wirklich gelöscht werden soll
                    DialogResult result = MessageBox.Show("Möchten Sie die VM "+(selectedVM.vm_name)+" #"+(selectedVM.Id)+" löschen?", "Bestätigung", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        // Entferne das VM-Objekt aus der Liste vms
                        vms.Remove(selectedVM);
                        vmListToDelete.Add(selectedVM);

                        // Update the listView1
                        UpdateListView(vms);
                    }
                }
                ClearTextBoxes();
                DisableButtons();
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            DisableButtons();

        }
        private void btnCSVExportClick(object sender, EventArgs e)
        {
            // exportiere die ListView in eine CSV-Datei
            ExportToCSV();
        }
        private void btnCSVImportClick(object sender, EventArgs e)
        {
            // importiere eine CSV-Datei in die ListView
            ImportFromCSV();
        }
        private void btnOpenLogsWindowsClick(object sender, EventArgs e)
        {
            // öffne das LogForm und füge eine Log-Nachricht hinzu (dies ist nur ein Beispiel) 
            LogForm logForm = new LogForm();
            logForm.AddLog("Dies ist eine Log-Nachricht.");
            logForm.ShowDialog();


        }
        private async void btnDeleteMissionClick(object sender, EventArgs e)
        {

            if (missionBox.SelectedIndex != -1)
            {
                MissionItem selectedItem = missionBox.SelectedItem as MissionItem;
                if (selectedItem != null)
                {
                    //MessageBox.Show($"Die ID der ausgewählten Mission ist: {selectedItem.Id}");

                    // Bestätigungsdialog anzeigen
                    DialogResult result = MessageBox.Show("Möchten Sie die Mission "+missionName+" wirklich löschen?", "Bestätigung", MessageBoxButtons.YesNo);
                    if(result == DialogResult.No) { return; }

                    DialogResult result2 = MessageBox.Show("Ganz Sicher?", "Bestätigung", MessageBoxButtons.YesNo);
                    if (result2 == DialogResult.No) { return; }

                    DialogResult result3 = MessageBox.Show("Gannnnnz Sicher?", "Bestätigung", MessageBoxButtons.YesNo);


                    if (result == DialogResult.Yes && result2 == DialogResult.Yes && result3 == DialogResult.Yes)
                    {
                        bool isSuccess = await ApiService.DeleteMission(hostname, Token, missionId);

                        if (isSuccess)
                        {
                            MessageBox.Show("Mission erfolgreich gelöscht.");

                            // missionBox leeren und neu laden
                            missionBox.Items.Clear();
                            missionBox.Text = "";
                            List<MissionItem> missionsList = await ApiService.GetMissions(hostname, Token);
                            ShowMissions(missionsList);

                            //listView1.Clear();
                            //vms.Clear();

                        }
                        else
                        {
                            MessageBox.Show("Fehler beim Löschen der Mission.");
                        }
                    }
                }
            }
        }


        public void DisableInputFields()
        {
            txtName.Enabled = false;
            txtHostname.Enabled = false;
            txtIP.Enabled = false;
            txtSubnet.Enabled = false;
            txtGateway.Enabled = false;
            txtDNS1.Enabled = false;
            txtDNS2.Enabled = false;
            txtDomain.Enabled = false;
            comboVLAN.Enabled = false;
            listBoxOS.Enabled = false;
            listBoxPackages.Enabled = false;
            btn_add.Enabled = false;
        }

        public void EnableInputFields()
        {
            txtName.Enabled = true;
            txtHostname.Enabled = true;
            txtIP.Enabled = true;
            txtSubnet.Enabled = true;
            txtGateway.Enabled = true;
            txtDNS1.Enabled = true;
            txtDNS2.Enabled = true;
            txtDomain.Enabled = true;
            comboVLAN.Enabled = true;
            listBoxOS.Enabled = true;
            listBoxPackages.Enabled = true;
            btn_add.Enabled = true;
        }

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
        private void DisableButtons()
        {
            btn_delete.Enabled = false;
            btn_edit.Enabled = false;
            btn_clear.Enabled = false;
        }
        private void EnableButtons()
        {
            btn_delete.Enabled = true;
            btn_edit.Enabled = true;
            btn_clear.Enabled = true;
        }
        private void ClearTextBoxes()
        {
            txtName.Text = "";
            txtHostname.Text = "";
            txtIP.Text = "";
            txtSubnet.Text = "";
            txtGateway.Text = "";
            txtDNS1.Text = "";
            txtDNS2.Text = "";
            txtDomain.Text = "";
            listBoxPackages.SelectedItems.Clear();
            txtDomain.Text = "";
            comboVLAN.Text = "";
            listBoxOS.Text = "";

        }
        public void ShowPackages(List<PackageItem> packagesList)
        {
            // Stelle sicher, dass listBoxPackages die ListBox ist, die du in deiner Form hast.
            listBoxPackages.Items.Clear(); // Bestehende Einträge löschen

            if (packagesList != null && packagesList.Any())
            {
                foreach (var package in packagesList)
                {
                    listBoxPackages.Items.Add(package); // Füge jedes Package zur ListBox hinzu
                }
            }
            else
            {
                listBoxPackages.Items.Add("Keine Pakete verfügbar.");
            }
        }
        public void ShowOS(List<OSItem> osList)
        {
            listBoxOS.Items.Clear();

            if (osList != null && osList.Any())
            {
                foreach (var os in osList)
                {

                    listBoxOS.Items.Add(os); // Fügt das OSItem direkt hinzu
                }
            }
            else
            {
                listBoxOS.Items.Add("Keine OS verfügbar.");
            }
        }
        public void ShowMissions(List<MissionItem> missionsList)
        {
            // Stelle sicher, dass listBoxMissions die ListBox ist, die du in deiner Form hast.
            missionBox.Items.Clear(); // Bestehende Einträge löschen

            if (missionsList != null && missionsList.Any())
            {
                foreach (var mission in missionsList)
                {
                    missionBox.Items.Add(new MissionItem(mission.Id, mission.mission_name, mission.vm_count));
                }
            }
            else
            {
                missionBox.Items.Add("Keine Missionen verfügbar.");
            }
        }
        public void ShowVLANs(List<VLANItem> vlanList)
        {
            comboVLAN.Items.Clear();

            if (vlanList != null && vlanList.Any())
            {
                foreach (var vlan in vlanList)
                {
                    comboVLAN.Items.Add(vlan); // Fügt das VLANItem direkt hinzu
                }
            }
            else
            {
                comboVLAN.Items.Add("Keine VLANs verfügbar.");
            }
        }
        private async void SaveVMsinMission_Click(object sender, EventArgs e)
        {
            MissionItem selectedItem = missionBox.SelectedItem as MissionItem;
            if (selectedItem != null)
            {
                
                foreach (var vm in vmListToCreate)
                {
                    Console.WriteLine("Mission ID: " + missionId);
                    Console.WriteLine("Neue VM: " + vm.vm_name);
                }

                foreach (var vm in vmListToDelete)
                {
                    Console.WriteLine("Mission ID: " + missionId);
                    Console.WriteLine("VM zum Löschen: " + vm.vm_name);
                }

                foreach (var vm in vmListToUpdate)
                {
                    Console.WriteLine("Mission ID: " + missionId);
                    Console.WriteLine("VM zum Updaten: " + vm.vm_name);
                }
                bool isSuccess = true;
                bool isSuccess2 = true;
                bool isSuccess3 = true;

                if (vmListToCreate.Count > 0) { isSuccess = await ApiService.VmListToWebAPI("vmListToCreate", hostname, Token, missionId, vmListToCreate); }
                if(vmListToDelete.Count > 0) { isSuccess2 = await ApiService.VmListToWebAPI("vmListToDelete", hostname, Token, missionId, vmListToDelete); }
                if(vmListToUpdate.Count > 0) { isSuccess3 = await ApiService.VmListToWebAPI("vmListToUpdate", hostname, Token, missionId, vmListToUpdate); }


                if (isSuccess && isSuccess2 && isSuccess3)
                {
                    MessageBox.Show("Neue VMs: " + vmListToCreate.Count + " - Updated " + vmListToUpdate.Count + " Übertragen und " + vmListToDelete.Count + " gelöscht");
                }
                else
                {
                    MessageBox.Show("Fehlgeschlagen");
                }

                // Leere vmListToCreate, vmListToDelete und vmListToUpdate
                vmListToCreate.Clear();
                vmListToDelete.Clear();
                vmListToUpdate.Clear();


                // lad missionbox neu
                missionBox.Text = "";
                missionBox.Items.Clear();
                List<MissionItem> missionsList = await ApiService.GetMissions(hostname, Token);
                ShowMissions(missionsList);

                // entferne aktuelle auswahl und wähle neu aus
                selectMission(missionName + " (" + vms.Count + ")");

                // Lade ListView1 neu
                vms.Clear();
                vms = await ApiService.GetVMs(hostname, Token, missionId);

                if (vms != null && vms.Count > 0)
                {
                    UpdateListView(vms);
                    EnableInputFields();
                }
            }
            else if (missionBox.Text != "")
            {
                string missionName2 = missionBox.Text;
                CreateMission(missionName2);

                // lade die Missionen neu und wähle neu erstellte Mission aus
                missionBox.Items.Clear();
                List<MissionItem> missionsList = await ApiService.GetMissions(hostname, Token);

                // wähle die neu erstellte Mission aus
                ShowMissions(missionsList);
                selectMission(missionName2 + " (0)");
                EnableInputFields();
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Mission aus oder geben Sie einen Namen ein.");
            }
        }

        private void selectMission(string SucheWert)
        {
            var obj = missionBox.Items.Cast<object>().FirstOrDefault(item => item.ToString() == SucheWert);

            // Wenn ein entsprechendes Objekt gefunden wurde, wähle es aus
            if (obj != null)
            {
                missionBox.SelectedItem = obj;
            }
            else
            {
                Console.WriteLine("Kein passendes Objekt für MissionSelect gefunden für: " + SucheWert);

                // gib hier alle werte von missionBox aus
                Console.WriteLine("Alle Werte von missionBox:");

                foreach (var item in missionBox.Items)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine("Ende der Werte von missionBox.");

            }
        }
        private async void CreateMission(string missionName)
        {
            bool isSuccess = await ApiService.CreateMission(hostname, Token, missionName);

            if (isSuccess)
            {
                MessageBox.Show("Mission erfolgreich erstellt.");
                // missionBox leeren und neu laden
                missionBox.Items.Clear();
                List<MissionItem> missionsList = await ApiService.GetMissions(hostname, Token);

                ShowMissions(missionsList);

                selectMission(missionName + " (0)");


            }
            else
            {
                MessageBox.Show("Fehler beim Speichern der Mission.");
                
            }
        }

        private void UpdateListView(List<VM> vms)
        {
            listView1.Items.Clear();
            foreach (var vm in vms)
            {
                Console.WriteLine("Funktion UpdateListView: " + vm.vm_name);    

                ListViewItem lvi = new ListViewItem(new[] {
                                vm.vm_name,
                                vm.vm_hostname,
                                vm.vm_ip,
                                vm.vm_subnet,
                                vm.vm_gateway,
                                vm.vm_dns1,
                                vm.vm_dns2,
                                vm.vm_domain,
                                vm.vm_vlan,
                                vm.vm_os,
                                vm.vm_packages,
                                vm.vm_status
                                    });

                lvi.Tag = vm;

                listView1.Items.Add(lvi);
                listView1.Show(); 
            }
        }

        public class ListBoxItem
        {
            public string Name { get; set; }
            public int Id { get; set; }

            // Konstruktor
            public ListBoxItem(string name, int id)
            {
                Name = name;
                Id = id;
            }

            // Die ToString()-Methode zurückgibt den Namen, der in der ListBox angezeigt wird
            public override string ToString()
            {
                return Name;
            }
        }

        public class OSItem
        {
            public string os_name { get; set; }
            public int Id { get; set; }

            public OSItem(string name, int id)
            {
                os_name = name;
                Id = id;
            }

            // Die ListBox verwendet ToString(), um den anzuzeigenden Text zu bestimmen.
            public override string ToString()
            {
                return $"{os_name}"; // Format: "OS-Name (ID)"
            }
        }

        public class MissionItem
        {
            public int Id { get; set; }
            public string mission_name { get; set; }
            public int vm_count { get; set; } // Angenommen, du möchtest auch die Anzahl der VMs speichern

            // Konstruktor
            public MissionItem(int id, string missionName, int vmCount)
            {
                Id = id;
                mission_name = missionName;
                vm_count = vmCount;
            }

            // Überschreibe ToString(), um den Text im ListBox anzuzeigen
            public override string ToString()
            {
                return $"{mission_name} ({vm_count})";
            }
        }

        public class PackageItem
        {
            public string package_name { get; set; }
            public int Id { get; set; }

            public PackageItem(string name, int id)
            {
                package_name = name;
                Id = id;
            }

            // Die ListBox verwendet ToString(), um den anzuzeigenden Text zu bestimmen.
            public override string ToString()
            {
                return $"{package_name}"; // Format: "OS-Name (ID)"
            }
        }

        public class VLANItem
        {
            public string vlan_name { get; set; }
            public int Id { get; set; }

            public VLANItem(string name, int id)
            {
                vlan_name = name;
                Id = id;
            }

            // Die ListBox verwendet ToString(), um den anzuzeigenden Text zu bestimmen.
            public override string ToString()
            {
                return $"{vlan_name}"; // Format: "OS-Name (ID)"
            }
        }

        private void btnVergleich_Click(object sender, EventArgs e)
        {


            // messagebox
            MessageBox.Show("Vergleiche die VM-Objekte mit der Liste vms.");


            // Vergleiche die VM-Objekte mit der Liste vms
            foreach (VM vm in vms)
            {
                bool found = false;

                // Überprüfe, ob das VM-Objekt in der Liste vms vorhanden ist
                foreach (ListViewItem item in listView1.Items)
                {
                    if (vm.vm_name == item.SubItems[0].Text &&
                        vm.vm_ip == item.SubItems[1].Text &&
                        vm.vm_subnet == item.SubItems[2].Text &&
                        vm.vm_gateway == item.SubItems[3].Text &&
                        vm.vm_dns1 == item.SubItems[4].Text &&
                        vm.vm_dns2 == item.SubItems[5].Text &&
                        vm.vm_domain == item.SubItems[6].Text &&
                        vm.vm_vlan == item.SubItems[7].Text &&
                        vm.vm_os == item.SubItems[8].Text &&
                        vm.vm_role == item.SubItems[9].Text &&
                        vm.vm_status == item.SubItems[10].Text)
                    {
                        found = true;
                        break;
                    }
                }

                string vmname = vm.vm_name;

                // Wenn das VM-Objekt nicht in der Liste vms gefunden wurde, gib es in der Konsole aus
                if (!found)
                {
                    Console.WriteLine($"Das VM-Objekt mit den Eigenschaften: {vm.vm_name}, {vm.vm_ip}, {vm.vm_subnet}, {vm.vm_gateway}, {vm.vm_dns1}, {vm.vm_dns2}, {vm.vm_domain}, {vm.vm_vlan}, {vm.vm_os}, {vm.vm_role}, {vm.vm_status} fehlt in der Liste listView1.");
                }
                else { Console.WriteLine(vm.vm_name+" passt."); }
            }
        }

        private void MissionChange(object sender, EventArgs e)
        {
            var aktuelleAuswahl = missionBox.Text;

            if (aktuelleAuswahl == "")
            {
                btn_loadVMsfromDB(sender, e);
            }
            else
            {
                DialogResult result = MessageBox.Show("Möchten Sie die Mission wechseln?", "Bestätigung", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    btn_loadVMsfromDB(sender, e);
                }
                else
                {
                    selectMission(missionName + " (" + vms.Count + ")");
                }
            }
        }

        private async void btnDeploy(object sender, EventArgs e)
        {
            var sshConnector = new SshConnector();




            // suche private Key im Windows User profile
            string privateKeyPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.ssh\\id_rsa";
            string publicKeyPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.ssh\\id_rsa.pub";
            string publicKey = "";
            string checkAndAddPublicKeyCommand = "";

            string ssh_password = txt_ssh_password.Text;
            string ssh_ip = txt_ssh_ip.Text;
            string ssh_port = txt_ssh_port.Text;
            string ssh_user = txt_ssh_user.Text;



            if (!int.TryParse(ssh_port, out int sshport))
            {
                MessageBox.Show("SSH-Port ist ungültig.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // prüfe ob datei existiert
            if (System.IO.File.Exists(privateKeyPath))
            {
                Console.WriteLine("Private Key gefunden: " + privateKeyPath);

                publicKey = File.ReadAllText(publicKeyPath);
                publicKey = publicKey.Replace("\n", "").Replace("\r", ""); // Entferne mögliche Newline-Zeichen
                Console.WriteLine("Öffentlicher Schlüssel aus Datei: " + publicKey);
            }
            else
            {
                Console.WriteLine("Private Key nicht gefunden: " + privateKeyPath);

                if (checkSSHKey.Checked)
                {
                    // ssh key erzeugen
                    publicKey = sshConnector.GenerateSSHKey(privateKeyPath);

                    if (!string.IsNullOrEmpty(publicKey))
                    {
                        Console.WriteLine("Öffentlicher Schlüssel generiert: ");
                        Console.WriteLine(publicKey);
                    }
                    else
                    {
                        Console.WriteLine("Fehler beim Generieren des SSH-Schlüssels.");

                        MessageBox.Show("Fehler beim Generieren des SSH-Schlüssels. Bitte prüfen Sie die Konsole.");
                        // abbruch
                        return;
                    }

                }
               

                if(useSSHKey.Checked)
                {

                    // Erstellt einen Befehl, der prüft, ob der öffentliche Schlüssel bereits in authorized_keys vorhanden ist, und fügt ihn hinzu, falls nicht
                    checkAndAddPublicKeyCommand = $"grep -q -F '{publicKey}' ~/.ssh/authorized_keys || echo '{publicKey}' >> ~/.ssh/authorized_keys";

                    // Füge den geänderten Befehl zur Liste der Befehle hinzu
                    //commands.Add(checkAndAddPublicKeyCommand);

                    MessageBox.Show("Verwende Key Authentifizierung");

                }
            }

            List<string> commands = new List<string>
            {
                $"grep -q -F '{publicKey}' ~/.ssh/authorized_keys || echo '{publicKey}' >> ~/.ssh/authorized_keys"
            };

            // führe folgende Befehle remote aus in console
            foreach (var command in commands)
            {
                Console.WriteLine("Befehl für remote: " + command);
            }

            // Angenommen, ExecuteCommands ist nun asynchron und du hast den Code entsprechend angepasst.
            commands.Add("ls -la");



            List<string> deployItems = new List<string>();

            if (ssh_password != "")
            {
                deployItems = sshConnector.ExecuteCommands(ssh_ip, sshport, ssh_user, ssh_password, commands);
                Console.WriteLine("Authentification with password");
            }
            else
            {
                deployItems = sshConnector.ExecuteCommands(ssh_ip, sshport, ssh_user, privateKeyPath, commands);
                Console.WriteLine("Authentification with private key");
            }
            

            // Erstelle eine Instanz von DeployForm
            DeployForm deployForm = new DeployForm();

            // Füge die Ausgaben zur DeployListView in DeployForm hinzu
            deployForm.AddDeployItems(deployItems);

            // Zeige DeployForm an
            deployForm.Show();

        }

        private async void btn_connectionHypervisor(object sender, EventArgs e)
        {
            Console.WriteLine("Auswahl: " + comboHypervisor.SelectedText);

            if (comboHypervisor.SelectedText != "Hyper-V")
            {
                button6.Text = "Verbinden";
                lbl_hypervisor.Text = "Status: Verbinde...";
                button6.Enabled = false;

                Console.WriteLine("Verbinde mit ESXi");
                string esxiHost = txt_hv_ip.Text;
                string username = txt_hv_loginname.Text;
                string password = txt_hv_loginpassword.Text;

                bool credentialsValid = await EsxiApiHelper.VerifyEsxiCredentialsAsync(esxiHost, username, password);
                if (credentialsValid)
                {
                    Console.WriteLine("Zugangsdaten sind korrekt.");
                    lbl_hypervisor.Text = "Status: Verbindung war erfolgreich!";
                    button6.Text = "Verbinden";

                }
                else
                {
                    Console.WriteLine("Zugangsdaten sind ungültig oder es gab einen Fehler.");
                    lbl_hypervisor.Text = "Status: Zugangsdaten sind ungültig oder es gab einen Fehler.";
                    button6.Text = "Verbinden";
                    button6.Enabled = true;
                }


            }
            else
            {
                MessageBox.Show("Hyper-V wird aktuell noch nicht unterstützt.");
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
