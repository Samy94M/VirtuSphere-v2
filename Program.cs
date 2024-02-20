using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace VirtuSphere
{
    static class Program
    {
        /// <summary>
        /// Der Haupt-Einstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Erstelle eine Instanz deines LoginForms
            LoginForm loginForm = new LoginForm();

            // Zeige das LoginForm an und überprüfe das DialogResult
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                //gib token an das Hauptformular weiter
                MessageBox.Show("Login an "+loginForm.hostname+" erfolgreich. Token: " + loginForm.Token);

                // loginForm.Token an mainForm übergeben
                String Token = loginForm.Token;


                // Wenn die Anmeldung erfolgreich war, zeige das Hauptformular an
                FMmain mainForm = new FMmain(); // Erstelle eine Instanz deines Hauptformulars


                Application.Run(mainForm);
            }
            else
            {
                // Beende die Anwendung, wenn das LoginForm geschlossen wird, ohne sich erfolgreich anzumelden
                Application.Exit();
            }
        }



    }
}
