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


namespace VirtuSphere
{
    public partial class LoginForm : Form
    {

        public string hostname { get; set; }
        public string Token { get; set; }

        public LoginForm()
        {
            InitializeComponent();

            comboBox1.Items.Add("Item 1");
            comboBox1.Items.Add("Item 2");
            comboBox1.Items.Add("Item 3");

            comboBox1.SelectedIndex = 0;
        }


        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtLoginname.Text;
            string password = txtPassword.Text;
            string hostname = comboBox1.SelectedItem.ToString();

            if (await IsValidLogin(username, password, hostname))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Login fehlgeschlagen. Bitte versuche es erneut.");
                this.DialogResult = DialogResult.None;
            }
        }

        private async Task<bool> IsValidLogin(string username, string password, string hostname)
        {
            using (HttpClient client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
                    {
                        { "action", "login" },
                        { "username", username },
                        { "password", password }
                    };

                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync("http://"+hostname+"/login.php", content);
                var result = await response.Content.ReadAsStringAsync();
                this.hostname = hostname;

                // wenn nicht "Invalid login credentials" dann speicher die rückgabe in einer variable token
                // und gib true zurück
                if (!result.Contains("Invalid login credentials"))
                {
                    // Token soll an Program.cs übergeben 
                    Token = result;
                    return true;
                }
                else
                {
                    return false;
                }


            }
        }
    }
}
