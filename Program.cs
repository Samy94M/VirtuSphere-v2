using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static VirtuSphere.FMmain;




namespace VirtuSphere
{
    static class Program
    {

        [STAThread]
        static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApiService apiService = new ApiService(); // Erstellen Sie eine Instanz der ApiService-Klasse

            // Erstelle eine Instanz deines LoginForms
            LoginForm loginForm = new LoginForm();

            // Zeige das LoginForm an und überprüfe das DialogResult
            if (loginForm.ShowDialog() != DialogResult.OK)
            {
                // Beende die Anwendung, wenn das LoginForm geschlossen wird, ohne sich erfolgreich anzumelden
                Application.Exit();
            }
            else
            {
                //gib token an das Hauptformular weiter
                //MessageBox.Show("Login an " + loginForm.hostname + " erfolgreich. `nToken: " + loginForm.Token);

                // Hole die Daten, die du benötigst
                List<OSItem> osList = await apiService.GetOS(loginForm.hostname, loginForm.Token);
                List<MissionItem> missionsList = await apiService.GetMissions(loginForm.hostname, loginForm.Token);
                List<PackageItem> PackagesList = await apiService.GetPackages(loginForm.hostname, loginForm.Token);
                List<VLANItem> VlanList = await apiService.GetVLANs(loginForm.hostname, loginForm.Token);


                // Erstelle eine Instanz deines MainForm
                FMmain mainForm = new FMmain(); 
                                                
                //Variablen übergaben
                String Token = loginForm.Token;
                String hostname = loginForm.hostname;
                mainForm.hostname = hostname;
                mainForm.Token = Token;
                mainForm.ShowOS(osList);
                mainForm.ShowMissions(missionsList);
                mainForm.ShowPackages(PackagesList);
                mainForm.ShowVLANs(VlanList);
                mainForm.ApiService = new ApiService(); // oder übergebe eine existierende Instanz
                Application.Run(mainForm);

                
            }
        }



    }
}
