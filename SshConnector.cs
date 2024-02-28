using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class SshConnector
{

    public void DeployAndRunPlaybook(string host, int port, string username, string password, string localPlaybookPath)
    {
        string remotePlaybookPath = $"/tmp/{Path.GetFileName(localPlaybookPath)}";

        // Transferiere das Playbook
        using (var sftp = new SftpClient(host, port, username, password))
        {
            try
            {
                sftp.Connect();
                Console.WriteLine("Verbunden mit SFTP-Server.");

                using (var fileStream = File.OpenRead(localPlaybookPath))
                {
                    sftp.UploadFile(fileStream, remotePlaybookPath, true);
                    Console.WriteLine($"Playbook hochgeladen nach: {remotePlaybookPath}");
                }

                sftp.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Hochladen des Playbooks: {ex.Message}");
                return; // Beende die Methode, wenn ein Fehler auftritt
            }
        }

        // Führe das Playbook aus
        using (var ssh = new SshClient(host, port, username, password))
        {
            try
            {
                ssh.Connect();
                Console.WriteLine("Verbunden mit SSH-Server.");

                var cmd = ssh.CreateCommand($"ansible-playbook {remotePlaybookPath}");
                var result = cmd.Execute();

                Console.WriteLine("Ansible Playbook Ausführungsergebnis:");
                Console.WriteLine(result);

                ssh.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der Ausführung des Playbooks: {ex.Message}");
            }
        }
    }

    public string GenerateSSHKey(string privateKeyPath)
    {
        string publicKeyPath = privateKeyPath + ".pub"; // Standardpfad für den öffentlichen Schlüssel
        try
        {
            // Generiere das Schlüsselpaar mit ssh-keygen
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "ssh-keygen",
                Arguments = $"-t rsa -b 2048 -N \"\" -f \"{privateKeyPath}\"", // Verwende Arguments anstelle von ArgumentList
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            Process process = Process.Start(psi);
            process.WaitForExit();

            // Lese den öffentlichen Schlüssel aus der Datei
            string publicKey = File.ReadAllText(publicKeyPath);
            return publicKey;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            return null;
        }
    }

    public List<string> ExecuteCommands(string host, int port, string username, string password, List<string> commands)
    {
        var output = new List<string>();

        using (var client = new SshClient(host, port, username, password))
        {
            try
            {
                client.Connect();

                if (client.IsConnected)
                {
                    foreach (var command in commands)
                    {
                        var result = client.CreateCommand(command).Execute();
                        // Fügt jede Zeile der Ausgabe als eigenen Eintrag hinzu
                        output.AddRange(result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
                    }
                }
                else
                {
                    Console.WriteLine("Verbindung fehlgeschlagen.");
                    output.Add("Verbindung fehlgeschlagen.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
                output.Add($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
            finally
            {
                client.Disconnect();

            }
        }

        return output;
    }
}
