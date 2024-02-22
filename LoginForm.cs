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
using System.Windows.Forms.Design;


namespace VirtuSphere
{
    public partial class LoginForm : Form
    {

        public string hostname { get; set; }
        public string Token { get; set; }

        public LoginForm()
        {
            InitializeComponent();

            // ließ serverlist.ini, wenn vorhanden, ansonsten füge localhost:8021 ein und füge die Servernamen in die ComboBox ein
            string[] serverList = new string[] { "localhost:8021" };
            if (System.IO.File.Exists("serverlist.ini"))
            {
                serverList = System.IO.File.ReadAllLines("serverlist.ini");
            }
            comboBox1.Items.AddRange(serverList);



            comboBox1.SelectedIndex = 0;
        }


        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text; // Stelle sicher, dass die Feldnamen korrekt sind
            string password = txtPassword.Text;
            string hostname = comboBox1.SelectedItem.ToString();

            // ApiService-Instanz sollte bereits verfügbar sein, z.B. über Dependency Injection
            ApiService apiService = new ApiService(); // Erstellen Sie eine Instanz der ApiService-Klasse
            string token = await apiService.IsValidLogin(username, password, hostname);


            if (!string.IsNullOrEmpty(token))
            {
                this.Token = token; // Speichere den Token
                this.hostname = hostname; // Speichere den Hostnamen
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Login fehlgeschlagen. Bitte versuche es erneut.");
                this.DialogResult = DialogResult.None;
            }
        }



    }
}
