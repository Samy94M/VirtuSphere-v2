using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static VirtuSphere.FMmain;

namespace VirtuSphere
{

    public class ApiService
    {
        private HttpClient _httpClient;
        public string Token { get; private set; }
        public DateTime TokenExpiryTime { get; private set; }

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public class ApiResponse
        {
            public bool success { get; set; }
        }


        public async Task<string> IsValidLogin(string username, string password, string hostname)
        {
            //msgbox aller parameter
            //MessageBox.Show("Username: " + username + " Password: " + password + " Hostname: " + hostname);
            Console.WriteLine("Username: " + username + " Password: " + password + " Hostname: " + hostname);

            // Beispiel-URL, an deine API anpassen

            string requestUri = $"http://{hostname}/api/login.php";
            var loginData = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };
            var content = new FormUrlEncodedContent(loginData);

            var response = await _httpClient.PostAsync(requestUri, content);


            // schreibe hier einen debug log in die Console verbindung zu hostname
            Console.WriteLine("Verbindung zu " + hostname + " hergestellt");
            // resopnse code in die Console schreiben
            Console.WriteLine(response.StatusCode);
            // response content in die Console schreiben

            // schreib result content in die Console
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseContent);
                // gib alles in result aus


                // Wenn result nicht null ist und nicht "Access Forbidden" dann gib den Token zurück
                if (result != null && result != "Access Forbidden")
                {
                    // gib token aus
                    Console.WriteLine("Token: " + result);
                    return result;
                }

                return null;

            }
            return null; // Bei Fehlschlag
        }
        public async Task<List<Package>> GetPackages(string hostname, string token)
        {
            string requestUri = $"http://{hostname}/access.php?action=getPackages&token={token}";
            var response = await _httpClient.GetAsync(requestUri);

            // wenn responsecode 418 ist, dann gib 418 zurück
            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var packageList = JsonConvert.DeserializeObject<List<Package>>(responseContent);
                return packageList;
            }
            return null; // Bei Fehlschlag oder "Access Forbidden"
        }
        public async Task<List<MissionItem>> GetMissions(string hostname, string token)
        {
            string requestUri = $"http://{hostname}/access.php?action=getMissions&token={token}";
            var response = await _httpClient.GetAsync(requestUri);

            // wenn responsecode 418 ist, dann gib 418 zurück
            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var missionsList = JsonConvert.DeserializeObject<List<MissionItem>>(responseContent);

                    return missionsList;
                }
                catch (JsonException)
                {
                    MessageBox.Show("Ungültiges JSON: " + responseContent);
                    return null;
                }
            }
            return null; // Bei Fehlschlag oder "Access Forbidden"
        }
        public async Task<List<OSItem>> GetOS(string hostname, string token)
        {
            string requestUri = $"http://{hostname}/access.php?action=getOS&token={token}";
            var response = await _httpClient.GetAsync(requestUri);

            // wenn responsecode 418 ist, dann gib 418 zurück
            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var osList = JsonConvert.DeserializeObject<List<OSItem>>(responseContent);
                return osList;
            }
            return null; // Bei Fehlschlag oder "Access Forbidden"
        }
        public async Task<List<VLANItem>> GetVLANs(string hostname, string token)
        {
            string requestUri = $"http://{hostname}/access.php?action=getVLANs&token={token}";
            var response = await _httpClient.GetAsync(requestUri);

            // wenn responsecode 418 ist, dann gib 418 zurück
            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                return null;
            }


            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    Console.WriteLine(responseContent);
                    try
                    {
                        var vlanList = JsonConvert.DeserializeObject<List<VLANItem>>(responseContent);
                        return vlanList;
                    }
                    catch (JsonException)
                    {
                        MessageBox.Show("Ungültiges JSON: " + responseContent);
                        return null;
                    }
                }
            }


            return null; // Bei Fehlschlag oder "Access Forbidden"
        }
        public async Task<bool> DeleteMission(string hostname, string token, int missionId)
        {
            if (hostname == null || token == null || _httpClient == null)
            {
                Console.WriteLine("Hostname, Token, MissionId oder HttpClient sind nicht verfügbar");
                return false;
            }
            else
            {
                Console.WriteLine("DeleteMission aufgerufen: Mission: " + missionId.ToString());
                Console.WriteLine("DeleteMission aufgerufen: Token: " + token);
                Console.WriteLine("DeleteMission aufgerufen: Hostname: " + hostname);

                // Die Mission ID ist in der klammer hinter dem Namen, also trenne sie

                Console.WriteLine("DeleteMission aufgerufen: Mission ID: " + missionId.ToString());


                string requestUri = $"http://{hostname}/access.php?action=deleteMission&token={token}&missionId={missionId}";
                var response = await _httpClient.DeleteAsync(requestUri);

                // ausgabe response content
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                // wenn responsecode 418 ist, dann gib 418 zurück
                if ((int)response.StatusCode == 418)
                {
                    Console.WriteLine("Token abgelaufen");
                    MessageBox.Show("Token abgelaufen");
                    return false;
                }
                if(response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Mission gelöscht");
                    // Wenn im Response-Code 200 steht, dann gib true zurück
                    return response.IsSuccessStatusCode;
                }
                return false;
                
            }
        }
        public async Task<bool> CreateMission(string hostname, string token, string missionName)
        {
            if (hostname == null || token == null || missionName == null || _httpClient == null)
            {
                Console.WriteLine("Hostname, Token, MissionName or HttpClient is not available");
                return false;
            }
            else
            {
                Console.WriteLine("CreateMission called: MissionName: " + missionName);
                Console.WriteLine("CreateMission called: Token: " + token);
                Console.WriteLine("CreateMission called: Hostname: " + hostname);

                string requestUri = $"http://{hostname}/access.php?action=createMission&token={token}&missionName={missionName}";
                var response = await _httpClient.PostAsync(requestUri, null);

                // wenn responsecode 418 ist, dann gib 418 zurück
                if ((int)response.StatusCode == 418)
                {
                    Console.WriteLine("Token abgelaufen");
                    MessageBox.Show("Token abgelaufen");
                    return false;
                }

                Console.WriteLine(response.StatusCode);

                return response.IsSuccessStatusCode;
            }
        }
        public async Task<List<VM>> GetVMs(string hostname, string token, int missionId)
        {
            Console.WriteLine("----------------------");
            Console.WriteLine("Aktion GetVMs für VM-Liste wird durchgeführt");
            string requestUri = $"http://{hostname}/access.php?action=getVMs&token={token}&missionId={missionId}";
            var response = await _httpClient.GetAsync(requestUri);

            Console.WriteLine($"RequestUri: {requestUri}");
            Console.WriteLine($"Request: {response}");
            Console.WriteLine($"Status Code:{response.StatusCode}");

            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                Console.WriteLine("----------------------");
                return null; // Beachte, dass du vielleicht eine leere Liste zurückgeben möchtest statt null, um NullReferenceExceptions zu vermeiden.
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Direkt deserialisieren zu List<VM> statt zu List<string>
                try
                {
                    var vmList = JsonConvert.DeserializeObject<List<VM>>(responseContent);
                    Console.WriteLine("Response Content: " + responseContent);
                    Console.WriteLine("----------------------");
                    return vmList; // Diese Liste enthält VM-Objekte, nicht nur VM-Namen
                }
                catch (JsonException)
                {
                    MessageBox.Show("Ungültiges JSON: " + responseContent);
                    // responseContent in console
                    Console.WriteLine("Response Content: " + responseContent);
                    Console.WriteLine("----------------------");
                    return null;
                }
                
            }
            Console.WriteLine("----------------------");
            return null; // Bei Fehlschlag oder "Access Forbidden"
        }
        public async Task<bool> SendVMList(string hostname, string token, int missionId, List<VM> vmList)
        {
            if (vmList != null)
            {

                // Zeige die zu sendende VM-Liste in der Konsole an
                foreach (var vm in vmList)
                {
                    Console.WriteLine($"VM: {vm.vm_name}");
                }

                string requestUri = $"http://{hostname}/access.php?action=sendVMList&token={token}&missionId={missionId}";

                var json = JsonConvert.SerializeObject(vmList);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(requestUri, content);

                if ((int)response.StatusCode == 418)
                {
                    Console.WriteLine("Token abgelaufen");
                    MessageBox.Show("Token abgelaufen");
                    return false;
                }

                Console.WriteLine($"Status Code: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                return response.IsSuccessStatusCode;
            }
            else
            {
                Console.WriteLine("VM-Liste ist null");
                return false; // Füge diese Zeile hinzu
            }
        }
        public async Task<bool> VmListToWebAPI(string action, string hostname, string token, int missionId, List<VM> vmList)
        {
            if (vmList == null)
            {
                Console.WriteLine("VM-Liste ist null");
                return false;
            }
            Console.WriteLine("----------------------");
            Console.WriteLine($"Aktion {action} für VM-Liste wird durchgeführt");
            foreach (var vm in vmList)
            {
                Console.WriteLine($"VM: {vm.vm_name}");
            }

            string requestUri = $"http://{hostname}/access.php?action={action}&token={token}&missionId={missionId}";
            var json = JsonConvert.SerializeObject(vmList);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, content);

            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                return false;
            }

            Console.WriteLine("RequestUri: " + requestUri);
            Console.WriteLine("Request: " + json);
            Console.WriteLine($"Status Code: {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");

            ApiResponse data = null;
            try
            {
                data = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
            }
            catch (JsonException)
            {
                MessageBox.Show("Ungültiges JSON: " + responseContent);
                return false;
            }

            Console.WriteLine("----------------------");

            if (data != null && data.success)
            {
                return response.IsSuccessStatusCode;
            }
            else
            {
                MessageBox.Show("Operation fehlgeschlagen.");
                return false;
            }
        }

        public async Task<bool> VmListToUpdate(string hostname, string token, int missionId, List<VM> vmList)
        {
            if (vmList != null)
            {

                // Zeige die zu sendende VM-Liste in der Konsole an
                foreach (var vm in vmList)
                {
                    Console.WriteLine($"VM: {vm.vm_name}");
                }

                string requestUri = $"http://{hostname}/access.php?action=vmListToUpdate&token={token}&missionId={missionId}";

                var json = JsonConvert.SerializeObject(vmList);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(requestUri, content);

                if ((int)response.StatusCode == 418)
                {
                    Console.WriteLine("Token abgelaufen");
                    MessageBox.Show("Token abgelaufen");
                    return false;
                }

                Console.WriteLine($"Status Code: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");
                ApiResponse data = null; // Deklariere data hier
                try
                {
                    data = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                }
                catch (JsonException)
                {
                    MessageBox.Show("Ungültiges JSON: " + responseContent);
                    return false;
                }

                if (data != null && data.success)
                {
                    return response.IsSuccessStatusCode;
                }
                else
                {
                    MessageBox.Show("Operation fehlgeschlagen.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("VM-Liste ist null");
                return false; // Füge diese Zeile hinzu
            }
        }
        public async Task<string> GetVLANName(string hostname, string token, int vlanId)
        {
            string requestUri = $"http://{hostname}/access.php?action=getVLANName&token={token}&vlanId={vlanId}";
            var response = await _httpClient.GetAsync(requestUri);

            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            return null; // Bei Fehlschlag oder "Access Forbidden"
        }
        public async Task<string> GetOSName(string hostname, string token, int osId)
        {
            string requestUri = $"http://{hostname}/access.php?action=getOSName&token={token}&osId={osId}";
            var response = await _httpClient.GetAsync(requestUri);

            if ((int)response.StatusCode == 418)
            {
                Console.WriteLine("Token abgelaufen");
                MessageBox.Show("Token abgelaufen");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            return null; // Bei Fehlschlag oder "Access Forbidden"
        }




        public class Missions
        {
            public string id { get; set; }
            public string mission_name { get; set; }
            public string mission_status { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class VM
        {
            public int Id { get; set; }
            public int mission_id { get; set; }
            public string vm_name { get; set; }
            public string vm_hostname { get; set; }
            public string vm_domain { get; set; }
            public string vm_os { get; set; } // Betriebssystem is German for Operating System
            public string vm_ram { get; set; }
            public string vm_disk { get; set; }
            public string vm_cpu { get; set; }
            public string vm_datastore { get; set; }
            public string vm_datacenter { get; set; }
            public string vm_guest_id { get; set; } // Assuming this is a unique ID for the VM guest
            public string vm_creator { get; set; } // Ersteller is German for Creator
            public string vm_status { get; set; }
            public string created_at { get; set; } // Erstellt am is German for Created on
            public string updated_at { get; set; } // Modifiziert am is German for Modified on
            public string vm_notes { get; set; } // Notizen is German for Notes

            public List<Interface> interfaces { get; set; }
            public List<Package> packages { get; set; }

            public VM()
            {
                interfaces = new List<Interface>();
                packages = new List<Package>();
            }
        }

        public class Package
        {
            public string id { get; set; }
            public string package_name { get; set; }
            public string package_version { get; set; }
            public string package_status { get; set; }

        }

        public class PackageItem
        {
            public string id { get; set; }
            public string package_name { get; set; }
            public string package_version { get; set; }
            public string package_status { get; set; }
        }

        public class Interface
        {
            public int id { get; set; }
            public int vm_id { get; set; }

            public string ip { get; set; }
            public string subnet { get; set; }
            public string gateway { get; set; }
            public string dns1 { get; set; }
            public string dns2 { get; set; }
            public string vlan { get; set; }
            public string mac { get; set; }
            public string mode { get; set; }

            public string DisplayText
            {
                get
                {
                    if (mode == "DHCP")
                    {
                        return $"Mode: {mode}, VLAN: {vlan}";
                    }
                    else // Für "Static" oder andere Modi
                    {
                        return $"IP: {ip}, Mode: {mode}, VLAN: {vlan}";
                    }
                }
            }
        }


        public class VLAN
        {
            public string id { get; set; }
            public string vlan_name { get; set; }

        }



    }
}
