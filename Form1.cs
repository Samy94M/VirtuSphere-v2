using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static VirtuSphere.ApiService;
using System.Security.Cryptography;



namespace VirtuSphere
{
    public partial class FMmain : Form
    {

        public ApiService ApiService = new ApiService();

        public string hostname { get; set; }
        public string Token { get; set; }
        public int missionId { get; set; }
        public string missionName { get; set; }
        public List<VM> vms = new List<VM>();
        public List<VM> vmListToDelete = new List<VM>();
        public List<VM> vmListToCreate = new List<VM>();
        public List<VM> vmListToUpdate = new List<VM>();
        public List<MissionItem> missionItems = new List<MissionItem>();
        public List<Package> packageItems = new List<Package>();



        public object JsonConvert { get; private set; }

        public FMmain()
        {

            InitializeComponent();
            DisableInputFields();

            string LocalUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string LocalAnsiblePath = LocalUserProfile + "\\ansible-playbooks";
            txtAnsibleLocal.Text = LocalAnsiblePath;
            comboAnsibleRemote.SelectedIndex = 0;

            // befülle mit Properties.Settings, sofern vorhanden
            if(Properties.Settings.Default.txt_ssh_ip != "") { txt_ssh_ip.Text = Properties.Settings.Default.txt_ssh_ip; }
            if (Properties.Settings.Default.txt_ssh_port != "") { txt_ssh_port.Text = Properties.Settings.Default.txt_ssh_port; }
            if (Properties.Settings.Default.txt_ssh_user != "") { txt_ssh_user.Text = Properties.Settings.Default.txt_ssh_user; }
            if (Properties.Settings.Default.txt_ssh_password != "") { txt_ssh_password.Text = Properties.Settings.Default.txt_ssh_password; }
            if (Properties.Settings.Default.txt_hv_ip != "") { txt_hv_ip.Text = Properties.Settings.Default.txt_hv_ip; }
            if (Properties.Settings.Default.txt_hv_loginname != "") { txt_hv_loginname.Text = Properties.Settings.Default.txt_hv_loginname; }
            if (Properties.Settings.Default.txt_hv_loginpassword != "") { txt_hv_loginpassword.Text = Properties.Settings.Default.txt_hv_loginpassword; }
            if (Properties.Settings.Default.comboHypervisor != "") { comboHypervisor.Text = Properties.Settings.Default.comboHypervisor; }

            if (Properties.Settings.Default.chk_ansible_credssave) { chk_ansible_credssave.Checked = true; } else { chk_ansible_credssave.Checked = false; }
            if (Properties.Settings.Default.chk_hypervisor_credssave) { chk_hypervisor_credssave.Checked = true; } else { chk_hypervisor_credssave.Checked = false; }
        }


        public async void btn_loadVMsfromDB(object sender, EventArgs e)
        {
            if (missionBox.SelectedIndex != -1)
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
                        MessageBox.Show("Keine VMs für " + selectedItem.mission_name + " in der Datenbank gefunden.");
                    }

                    // }
                }
                else
                {
                    MessageBox.Show("Mission kann nicht geladen werden, weil die noch nicht in der Datenbank angelegt wurde.");
                }
            }
        }
        // check status 
        public void checkStatus()
        {
            if (vmListToCreate.Count == 0 && vmListToDelete.Count == 0 && vmListToUpdate.Count == 0)
            {

                txtStatus.Text = "Status: OK";

            }
            else
            {
                txtStatus.Text = "Status: Datenbank update notwendig!";
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



            // ermittel das erste VLAN in der Liste
            string erstesVLAN = comboWDSVlan.SelectedItem.ToString();

            // erstelle Inteface und weise es vm zu
            Interface newInterface = new Interface
            {
                ip = "",
                subnet = "",
                gateway = "",
                dns1 = "",
                dns2 = "",
                vlan = erstesVLAN,
                mode = "DHCP"
            };

            // Create a new VM object with the entered values
            VM vm = new VM
            {
                vm_name = txtName.Text,
                vm_hostname = txtHostname.Text,
                vm_domain = txtDomain.Text,
                vm_os = listBoxOS.Text,
                vm_status = "Neu - DB Sync!",
                vm_cpu = txtCPU.Text,
                vm_disk = txtHDD.Text,
                vm_ram = txtRAM.Text,
                created_at = "",
                updated_at = "",
                interfaces = new List<Interface> { newInterface }
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

            checkStatus();
        }

        private bool ValidateInputFields(VM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.vm_name) || string.IsNullOrWhiteSpace(vm.vm_hostname))
            {
                MessageBox.Show("Name und Hostname dürfen nicht leer sein.");
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
                string PackagesList = "";
                string InterfaceList = "";


                // prüfe ob vm.packages gefüllt ist und gebe die namen der Packages aus
                if (vm.packages != null)
                {
                    foreach (var package in vm.packages)
                    {
                        Console.WriteLine("Packages für " + vm.vm_name + ": " + vm.packages);
                        PackagesList += package.package_name + "; ";
                    }


                    Console.WriteLine("Packages für " + vm.vm_name + ": " + vm.packages);
                }

                if (vm.interfaces != null)
                {
                    foreach (var interface1 in vm.interfaces)
                    {
                        Console.WriteLine("Interface für " + vm.vm_name + ": " + interface1.ip);
                        InterfaceList += interface1.ip + "; ";
                    }

                    Console.WriteLine("Interfaces für " + vm.vm_name + ": " + InterfaceList);
                }

                ListViewItem lvi = new ListViewItem(new[] {
                    vm.vm_name,
                    vm.vm_hostname,
                    InterfaceList,
                    vm.vm_domain,
                    vm.vm_os,
                    PackagesList,
                    vm.vm_status
                });

                listView1.Items.Add(lvi);


                lvi.Tag = vm;
            }

            return Task.CompletedTask;
        }

        private async void btnEditClick(object sender, EventArgs e)
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
                    selectedVM.vm_domain = txtDomain.Text;
                    selectedVM.vm_os = listBoxOS.Text;
                    selectedVM.vm_status = "geändert - DB Sync!";
                    selectedVM.vm_ram = txtRAM.Text;
                    selectedVM.vm_disk = txtHDD.Text;
                    selectedVM.vm_cpu = txtCPU.Text;

                    // Ausgewählte listBoxPackages Objekte sollen mit Semikolon getrennt in packages gespeichert werden

                    //selectedVM.packages = packages;

                    selectedVM.packages = await GetSelectedPackages(ApiService); // Warten auf das Task-Ergebnis

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

        private async Task<List<Package>> GetSelectedPackages(ApiService apiService)
        {

            List<Package> selectedPackages = new List<Package>();
            var allPackageItems = await apiService.GetPackages(hostname, Token); // Warten auf das Task-Ergebnis

            if (allPackageItems != null) // Prüfen, ob das Ergebnis nicht null ist
            {
                foreach (var selectedItem in listBoxPackages.SelectedItems)
                {
                    // Annahme: 'selectedItem' ist der Name des Pakets, der in der ListBox angezeigt wird.
                    var packageItem = allPackageItems.FirstOrDefault(p => p.package_name == selectedItem.ToString());
                    if (packageItem != null)
                    {
                        Package package = new Package
                        {
                            id = packageItem.id,
                            package_name = packageItem.package_name,
                            package_version = packageItem.package_version,
                            package_status = packageItem.package_status
                        };
                        selectedPackages.Add(package);
                        Console.WriteLine("GetSelectedPackages Selected Package: " + package.package_name);
                    }
                }
            }
            return selectedPackages;
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
                    txtDomain.Text = selectedVM.vm_domain;
                    listBoxOS.Text = selectedVM.vm_os;
                    txtRAM.Text = selectedVM.vm_ram;
                    txtHDD.Text = selectedVM.vm_disk;
                    txtCPU.Text = selectedVM.vm_cpu;
                    listBoxPackages.ClearSelected();

                    MarkSelectedPackagesInListBox(selectedVM.packages);



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

        private void MarkSelectedPackagesInListBox(List<Package> selectedPackages)
        {

            // Sicherstellen, dass listBoxPackages2 nicht null ist
            if (listBoxPackages == null || selectedPackages == null) return;

            // Durchlaufen aller Items in listBoxPackages2
            for (int i = 0; i < listBoxPackages.Items.Count; i++)
            {

                Console.WriteLine("2MarkSelectedPackagesInListBox is: " + listBoxPackages.Items[i]);
                // Nehmen wir an, dass jedes Item in listBoxPackages2 eine Repräsentation eines Package-Objekts ist
                // und dass es eine ToString-Überschreibung gibt, die package_name zurückgibt oder
                // dass das Item direkt das Package-Objekt ist.
                var item = listBoxPackages.Items[i];

                // Überprüfen, ob das aktuelle Item in der Liste der ausgewählten Pakete vorhanden ist
                bool isSelected = selectedPackages.Any(package =>
                    package.package_name == item.ToString() || // Wenn die ListBox die Namen der Pakete direkt speichert
                    (item is Package && ((Package)item).id == package.id)); // Wenn die ListBox Package-Objekte speichert

                // Setzen der Selected-Eigenschaft basierend auf der Übereinstimmung
                listBoxPackages.SetSelected(i, isSelected);

                if (isSelected) { Console.Write(item.ToString() + " is selected"); }
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
                    DialogResult result = MessageBox.Show("Möchten Sie die VM " + (selectedVM.vm_name) + " #" + (selectedVM.Id) + " löschen?", "Bestätigung", MessageBoxButtons.YesNo);

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
                    DialogResult result = MessageBox.Show("Möchten Sie die Mission " + missionName + " wirklich löschen?", "Bestätigung", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No) { return; }

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
            txtDomain.Enabled = false;
            listBoxOS.Enabled = false;
            listBoxPackages.Enabled = false;
            btn_add.Enabled = false;
            txtCPU.Enabled = false;
            txtHDD.Enabled = false;
            txtRAM.Enabled = false;
        }

        public void EnableInputFields()
        {
            txtName.Enabled = true;
            txtHostname.Enabled = true;
            txtDomain.Enabled = true;
            listBoxOS.Enabled = true;
            listBoxPackages.Enabled = true;
            btn_add.Enabled = true;
            txtCPU.Enabled = true;
            txtHDD.Enabled = true;
            txtRAM.Enabled = true;
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
            txtDomain.Text = "";
            listBoxPackages.SelectedItems.Clear();
            txtDomain.Text = "";

        }
        public void ShowPackages(List<Package> packagesList)
        {
            // Stelle sicher, dass listBoxPackages die ListBox ist, die du in deiner Form hast.
            listBoxPackages.Items.Clear(); // Bestehende Einträge löschen

            if (packagesList != null && packagesList.Any())
            {
                foreach (var package in packagesList)
                {
                    listBoxPackages.Items.Add(package.package_name); // Füge jedes Package zur ListBox hinzu

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

                // wähle den ersten wert aus
                listBoxOS.SelectedIndex = 0;

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
            //comboVLAN.Items.Clear();

            if (vlanList != null && vlanList.Any())
            {
                foreach (var vlan in vlanList)
                {
                    //comboVLAN.Items.Add(vlan); // Fügt das VLANItem direkt hinzu
                    comboWDSVlan.Items.Add(vlan);
                }
            }
            else
            {
                //comboVLAN.Items.Add("Keine VLANs verfügbar.");
            }

            comboWDSVlan.SelectedIndex = 0;
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
                if (vmListToDelete.Count > 0) { isSuccess2 = await ApiService.VmListToWebAPI("vmListToDelete", hostname, Token, missionId, vmListToDelete); }
                if (vmListToUpdate.Count > 0) { isSuccess3 = await ApiService.VmListToWebAPI("vmListToUpdate", hostname, Token, missionId, vmListToUpdate); }


                if (isSuccess && isSuccess2 && isSuccess3)
                {
                    MessageBox.Show("Neue VMs: " + vmListToCreate.Count + " - Updated " + vmListToUpdate.Count + " Übertragen und " + vmListToDelete.Count + " gelöscht");


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

                    checkStatus();
                }
                else
                {
                    MessageBox.Show("Fehlgeschlagen");
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
                string PackagesList = "";
                string InterfaceList = "";


                // prüfe ob vm.packages gefüllt ist und gebe die namen der Packages aus
                if (vm.packages != null)
                {
                    foreach (var package in vm.packages)
                    {
                        Console.WriteLine("Packages für " + vm.vm_name + ": " + vm.packages);
                        PackagesList += package.package_name + "; ";
                    }


                    Console.WriteLine("Packages für " + vm.vm_name + ": " + vm.packages);
                }

                if (vm.interfaces != null)
                {
                    foreach (var interface1 in vm.interfaces)
                    {
                        Console.WriteLine("Interface für " + vm.vm_name + ": " + interface1.ip);
                        InterfaceList += interface1.ip + "; ";
                    }

                    Console.WriteLine("Interfaces für " + vm.vm_name + ": " + InterfaceList);
                }

                ListViewItem lvi = new ListViewItem(new[] {
                    vm.vm_name,
                    vm.vm_hostname,
                    InterfaceList,
                    vm.vm_domain,
                    vm.vm_os,
                    PackagesList,
                    vm.vm_status
                });

                lvi.Tag = vm;

                listView1.Items.Add(lvi);
                listView1.Show();
                checkStatus();
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
                        vm.vm_domain == item.SubItems[6].Text &&
                        vm.vm_os == item.SubItems[8].Text)
                    {
                        found = true;
                        break;
                    }
                }

                string vmname = vm.vm_name;

                // Wenn das VM-Objekt nicht in der Liste vms gefunden wurde, gib es in der Konsole aus
                if (!found)
                {
                    Console.WriteLine($"Das VM-Objekt mit den Eigenschaften: {vm.vm_name}  fehlt in der Liste listView1.");
                }
                else { Console.WriteLine(vm.vm_name + " passt."); }
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

        private void btnDeploy(object sender, EventArgs e)
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


                if (useSSHKey.Checked)
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

        private void generatePlaybooks(object sender, EventArgs e)
        {
            Console.WriteLine("MissionID: " + missionId);
            Console.WriteLine("MissionName: " + missionName);

            // prüfe ob notwendige Felder befüllt sind:
            if (txt_hv_ip.Text == "" || txt_hv_loginname.Text == "" || txt_hv_loginpassword.Text == "" || txt_ssh_user.Text == "")
            {
                MessageBox.Show("Bitte füllen Sie alle Felder unter Umgebung aus.");
                // öffne Tab Umgebung
                tabControl2.SelectedTab = tabControl2.TabPages["Umgebung"];

                return;
            }

            // Fehlermeldung wenn MissionName leer ist
            if (missionName == null)
            {
                MessageBox.Show("Bitte wählen Sie eine Mission aus.");
                return;
            }


            // erstelle missionName Ordner unter temp, wenn nooch nicht existiert
            string ProjecttempPath = Path.Combine(Path.GetTempPath(), missionName);
            if (!Directory.Exists(ProjecttempPath))
            {
                Directory.CreateDirectory(ProjecttempPath);
            }


            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "Ansible", "createVMs-ESXi.yml");

            if (File.Exists(filePath))
            {
                try
                {
                    AnsibleForm ansibleForm = new AnsibleForm(vms);

                    // createVMs-ESXi
                    String TargetFile = Path.Combine(ProjecttempPath, "createVMs-ESXi.yml");
                    ansibleForm.listFiles.Items.Add(Path.GetFileName(Path.Combine(ProjecttempPath, "createVMs-ESXi.yml")));
                    string serverlist = "vm_configurations:\n";
                    string interfaces = "";
                    string packages = ""; 
                    //File.Copy(filePath, TargetFile, true);

                    // lade inhalt von filePath in einen String
                    string createVMsContent = File.ReadAllText(filePath);

                    // ersetze in createVMsContent die Variablen
                    createVMsContent = createVMsContent.Replace("{hypervisor_esxiip}", txt_hv_ip.Text);
                    createVMsContent = createVMsContent.Replace("{hypervisor_username}", txt_hv_loginname.Text);
                    createVMsContent = createVMsContent.Replace("{hypervisor_password}", txt_hv_loginpassword.Text);
                    createVMsContent = createVMsContent.Replace("{ansible_username}", txt_ssh_user.Text);

                    // speichere createVMsContent in TargetFile
                    File.WriteAllText(TargetFile, createVMsContent);

                    // Erstelle serverlist.yml

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

                    if (File.Exists(Path.Combine(ProjecttempPath, "serverlist.yml")))
                    {
                        File.Delete(Path.Combine(ProjecttempPath, "serverlist.yml"));
                    }

                    File.WriteAllText(Path.Combine(ProjecttempPath, "serverlist.yml"), serverlist);
                    ansibleForm.listFiles.Items.Add(Path.GetFileName(Path.Combine(ProjecttempPath, "serverlist.yml")));


                    // Liest den Inhalt der Datei
                    string result = File.ReadAllText(filePath);

                    // Ersetzt Unix- und Mac-Zeilenumbrüche durch Windows-Zeilenumbrüche
                    result = result.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");



                    // Konfiguration für bessere Formatierung
                    ansibleForm.txtAnsible.Multiline = true;
                    ansibleForm.txtAnsible.ScrollBars = ScrollBars.Both;
                    ansibleForm.txtAnsible.Font = new Font("Consolas", 10);

                    // erfasse Variablen aus Textbox
                    string esxi_host = txt_hv_ip.Text;
                    string esxi_user = txt_hv_loginname.Text;
                    string esxi_password = txt_hv_loginpassword.Text;


                    // Ersetze Variablen in der Datei
                    result = result.Replace("{{esxi_host}}", txt_hv_ip.Text);
                    result = result.Replace("{{esxi_user}}", txt_hv_loginname.Text);
                    result = result.Replace("{{esxi_password}}", txt_hv_loginpassword.Text);
                    result = result.Replace("{{vm_name}}", txtName.Text);


                    // Ausgabe in Textbox
                    ansibleForm.txtAnsible.Text = result;

                    // fülle comboPlaybooks mit den werten aus der Liste

                    foreach (ListViewItem item in ansibleForm.listFiles.Items)
                    {
                        ansibleForm.comboPlaybooks.Items.Add(item.Text); 
                    }

                    ansibleForm.comboPlaybooks.SelectedIndex = 0;



                    ansibleForm.SetMissionName(missionName);
                    ansibleForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Lesen der Datei: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"Datei nicht gefunden: {filePath}");
            }
        }

        private void OpenVmeditForm(object sender, EventArgs e)
        {
            // Öffne GUI vmedit.cs
            //vmeditForm vmeditForm = new vmeditForm();
            VM selectedVM = listView1.SelectedItems[0].Tag as VM; // Cast das Tag zurück zum VM-Objekt

            if (selectedVM != null)
            {
                vmeditForm editForm = new vmeditForm(this, selectedVM);
                editForm.FillListBoxPackages2(this.packageItems);

                // Zeige das Formular an
                editForm.ShowDialog();
            }

        }

        private void btn_savecredsHypervisor(object sender, EventArgs e)
        {

            // Speicher die eingetragenden Werte bis zum Computerneustart
            if (chk_hypervisor_credssave.Checked)
            {
                Console.WriteLine("Speichere Hypervisor Zugangsdaten.");
                Properties.Settings.Default.txt_hv_ip = txt_hv_ip.Text;
                Properties.Settings.Default.txt_hv_loginname = txt_hv_loginname.Text;
                Properties.Settings.Default.txt_hv_loginpassword = txt_hv_loginpassword.Text;
                Properties.Settings.Default.comboHypervisor = comboHypervisor.Text;

                //// Verschlüsseln
                //byte[] toEncrypt = Encoding.UTF8.GetBytes(txt_hv_loginpassword.Text);
                //byte[] encrypted = ProtectedData.Protect(toEncrypt, null, DataProtectionScope.CurrentUser);

                //// Speichern der verschlüsselten Daten
                //Properties.Settings.Default.hv_loginpassword = Convert.ToBase64String(encrypted);

            }
            else
            {
                Console.WriteLine("lösche Hypervisor Zugangsdaten.");
                Properties.Settings.Default.txt_hv_ip = "";
                Properties.Settings.Default.txt_hv_loginname = "";
                Properties.Settings.Default.txt_hv_loginpassword = "";
                Properties.Settings.Default.comboHypervisor = "";
            }

            Properties.Settings.Default.Save();

        }

        private void btn_savecredsAnsible(object sender, EventArgs e)
        {
            if (chk_ansible_credssave.Checked)
            {
                Console.WriteLine("Speichere Ansible Zugangsdaten.");
                Properties.Settings.Default.txt_ssh_ip = txt_hv_ip.Text;
                Properties.Settings.Default.txt_ssh_port = txt_hv_loginname.Text;
                Properties.Settings.Default.txt_ssh_user = txt_hv_loginpassword.Text;
                Properties.Settings.Default.txt_ssh_password = txt_ssh_user.Text;

            }
            else
            {
                Console.WriteLine("lösche Ansible Zugangsdaten.");
                Properties.Settings.Default.txt_ssh_ip = "";
                Properties.Settings.Default.txt_ssh_port = "";
                Properties.Settings.Default.txt_ssh_user = "";
                Properties.Settings.Default.txt_ssh_password = "";
            }

            Properties.Settings.Default.Save();
        }
    }
}
