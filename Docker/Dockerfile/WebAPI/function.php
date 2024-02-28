<?php

include 'mysql.php';

function addLog($ip, $request, $authToken, $connection) {
   $logMessage = ' Request: ' . $request . ' | Auth-Token: ' . $authToken;
   $logMessage = $connection->real_escape_string($logMessage);
   
   $query = "INSERT INTO deploy_logs (ip, log_message, created_at, updated_at) VALUES ('$ip', '$logMessage', NOW(), NOW())";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
}

function removeLog($connection){
   $query = "DELETE FROM deploy_logs where created_at < DATE_SUB(NOW(), INTERVAL 7 DAY)";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
  
}




function generateToken($username, $password, $connection) {
   // Check if login credentials are correct with password_verify
      $hashedPassword = password_hash($password, PASSWORD_DEFAULT);
      $query = "SELECT * FROM deploy_users WHERE name = '$username'";
      $result = $connection->query($query);
      if (!$result) {
         die('Error: ' . $connection->error);
      }
      
      if ($result->num_rows > 0) {
         $row = $result->fetch_assoc();
         if (password_verify($password, $row['password'])) {
            // Generate token for 60 minutes
            $token = bin2hex(random_bytes(32));
            $query = "INSERT INTO deploy_tokens (token, expired, created_at, updated_at) VALUES ('$token', FALSE, NOW(), NOW())";
            $result = $connection->query($query);
            if (!$result) {
               die('Error: ' . $connection->error);
            }
            
            return $token;
            addLog($_SERVER['REMOTE_ADDR'], 'generateToken', $token, $connection);
         }
      } else {
         $token = false;
      addLog($_SERVER['REMOTE_ADDR'], 'generateToken', 'Invalid login credentials', $connection);
      }
   }


function verifyToken($token, $connection) {
   $query = "SELECT * FROM deploy_tokens WHERE token = '$token' AND expired = FALSE and created_at > DATE_SUB(NOW(), INTERVAL 60 MINUTE)";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }

   if ($result->num_rows > 0) {
      return TRUE;
   } else {
      return FALSE;
   }
}

function createVM($vmName, $vmHostname, $vmIP, $vmSubnet, $vmGateway, $vmDNS1, $vmDNS2, $vmDomain, $vmVLAN, $vmRole, $vmStatus, $connection, $token) {
   $query = "INSERT INTO deploy_vms (vm_name, vm_hostname, vm_ip, vm_subnet, vm_gateway, vm_dns1, vm_dns2, vm_domain, vm_vlan, vm_role, vm_status, created_at, updated_at) 
             VALUES ('$vmName', '$vmHostname', '$vmIP', '$vmSubnet', '$vmGateway', '$vmDNS1', '$vmDNS2', '$vmDomain', '$vmVLAN', '$vmRole', '$vmStatus', NOW(), NOW())";
   $result = $connection->query($query);
   if (!$result) {
function createOrUpdateVm($hostname, $connection) {
   $query = "SELECT * FROM deploy_vms WHERE vm_hostname = '$hostname'";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
   if ($result->num_rows > 0) {
      // Update existing VM
      $query = "UPDATE deploy_vms SET vm_status = 'updated', updated_at = NOW() WHERE vm_hostname = '$hostname'";
      $result = $connection->query($query);
      if (!$result) {
         die('Error: ' . $connection->error);
      }
   } else {
      // Create new VM
      $query = "INSERT INTO deploy_vms (vm_hostname, created_at, updated_at) VALUES ('$hostname', NOW(), NOW())";
      $result = $connection->query($query);
      if (!$result) {
         die('Error: ' . $connection->error);
      }
   }
}
      die('Error: ' . $connection->error);
   }
   addLog($_SERVER['REMOTE_ADDR'], 'createVM', $token, $connection);
}


function getMissions($connection) {
   $query = "SELECT *, (SELECT COUNT(*) FROM deploy_vms WHERE mission_id = deploy_missions.id) AS vm_count FROM deploy_missions";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
   $missions = array();
   while ($row = $result->fetch_assoc()) {
      $missions[] = $row;
   }
   
   return $missions;
}

function getVMs_2($connection, $missionId) {
   $query = "SELECT * FROM deploy_vms where mission_id = $missionId";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
   $vms = array();
   while ($row = $result->fetch_assoc()) {
      $vms[] = $row;
   }
   
   return $vms;
}

function getVMs($connection, $missionId) {
   // Zuerst die VMs für die gegebene mission_id abrufen
   $vmQuery = "SELECT * FROM deploy_vms WHERE mission_id = ?";
   $stmt = $connection->prepare($vmQuery);
   $stmt->bind_param("i", $missionId);
   $stmt->execute();
   $result = $stmt->get_result();
   if (!$result) {
       die('Error: ' . $connection->error);
   }
   
   $vms = array();
   while ($row = $result->fetch_assoc()) {
       // Für jede VM die zugehörigen Pakete abrufen
       $vmId = $row['id'];
       $packagesQuery = "SELECT dp.* FROM deploy_packages dp 
                         INNER JOIN deploy_vm_packages dvp ON dp.id = dvp.package_id 
                         WHERE dvp.vm_id = ?";
       $packageStmt = $connection->prepare($packagesQuery);
       $packageStmt->bind_param("i", $vmId);
       $packageStmt->execute();
       $packagesResult = $packageStmt->get_result();
       
       $packages = array();
       while ($packageRow = $packagesResult->fetch_assoc()) {
           $packages[] = $packageRow;
       }
       
       // Die Pakete zum VM-Array hinzufügen
       $row['packages'] = $packages;

      // Für jede VM die zugehörigen Netzwerk-Interfaces abrufen
      $interfacesQuery = "SELECT * FROM deploy_interfaces WHERE vm_id = ?";
      $interfaceStmt = $connection->prepare($interfacesQuery);
      $interfaceStmt->bind_param("i", $vmId);
      $interfaceStmt->execute();
      $interfacesResult = $interfaceStmt->get_result();

       $interfaces = array();
       while ($interfaceRow = $interfacesResult->fetch_assoc()) {
           $interfaces[] = $interfaceRow;
       }
       
       // Die Interfaces zum VM-Array hinzufügen
       $row['interfaces'] = $interfaces;

       $vms[] = $row;
   }
   
   return $vms;
}


function getPackages($connection) {
   $query = "SELECT * FROM deploy_packages";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
   $packages = array();
   while ($row = $result->fetch_assoc()) {
      $packages[] = $row;
   }
   
   return $packages;
}

function deleteMission($id, $connection){
   $query = "DELETE FROM deploy_missions WHERE id = $id";
   $result = $connection->query($query);

   if($result){
      return true;
      addLog($_SERVER['REMOTE_ADDR'], 'deleteMission', $id, $connection);
   } else {
      return false;
      addLog($_SERVER['REMOTE_ADDR'], 'deleteMission Failed', $id, $connection);
   }
}


function createMission($missionName, $connection){
   $query = "INSERT INTO deploy_missions (mission_name, mission_status) VALUES ('$missionName', 'active')";
   $result = $connection->query($query);
   if($result){
      return true;
      addLog($_SERVER['REMOTE_ADDR'], 'createMission', $missionName, $connection);
   } else {
      return false;
      addLog($_SERVER['REMOTE_ADDR'], 'createMission Failed', $missionName, $connection);
   }
}

function getOS($connection){
   $query = "SELECT * FROM deploy_os";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
   $os = array();
   while ($row = $result->fetch_assoc()) {
      $os[] = $row;
   }
   
   return $os;
}

function sendVMList($missionId, $vmList, $connection){
   $json = file_get_contents('php://input');
   $vmList = json_decode($json);
}

function getVLAN($connection){
   $query = "SELECT * FROM deploy_vlan";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
   $vlans = array();
   while ($row = $result->fetch_assoc()) {
      $vlans[] = $row;
   }
   
   return $vlans;
}

function deleteVM($vmList, $connection){
   if (!empty($vmList)) {
      foreach ($vmList as $vm) {
         $query = "DELETE FROM deploy_vms WHERE id = '{$vm->Id}'";
         $result = $connection->query($query);
         if (!$result) {
            die('Error: ' . $connection->error);
         }
      }
      return true;
   } else {
      return false;
   }
}

function vmListToCreate($missionId, $vmList, $mysqli){
   if (!empty($vmList)) {
      $successCount = 0;
      foreach ($vmList as $vm) {
         // Angenommen, $vm repräsentiert das VM-Objekt, das Sie einfügen möchten, und $mysqli ist Ihre Datenbankverbindung
         //$missionId = $vm->mission_id; // Stellen Sie sicher, dass diese Eigenschaft existiert und korrekt gesetzt ist
         $vmStatus = ''; // Beispielstatus, passen Sie dies entsprechend an

         // VM in die deploy_vms Tabelle einfügen
         $query = "INSERT INTO deploy_vms (mission_id, vm_name, vm_hostname, vm_domain, vm_os, vm_status, vm_notes) VALUES ('{$missionId}', '{$vm->vm_name}', '{$vm->vm_hostname}', '{$vm->vm_domain}', '{$vm->vm_os}', '{$vmStatus}', '{$vm->vm_notes}')";

         if ($mysqli->query($query) === TRUE) {
            $vmId = $mysqli->insert_id; // ID der gerade eingefügten VM
            $successCount++;

            // Jetzt können Sie die Interfaces und Pakete der VM einfügen
            // Für jedes Interface in $vm->vm_interfaces
            foreach ($vm->interfaces as $interface) {
               // Stellen Sie sicher, dass die notwendigen Interface-Daten vorhanden sind
               $query = "INSERT INTO deploy_interfaces (vm_id, ip, subnet, gateway, dns1, dns2, vlan, mac) VALUES ('{$vmId}', '{$interface->ip}', '{$interface->subnet}', '{$interface->gateway}', '{$interface->dns1}', '{$interface->dns2}', '{$interface->vlan}', '{$interface->mac}')";
               $mysqli->query($query);
               // Prüfen Sie hier auf Fehler mit $mysqli->error und behandeln Sie diese entsprechend
            }

            // Für jedes Paket in $vm->vm_packages
            foreach ($vm->packages as $package) {
               // Stellen Sie sicher, dass die notwendigen Paketdaten vorhanden sind
               $query = "INSERT INTO deploy_packages (vm_id, package_name, package_version, package_status) VALUES ('{$vmId}', '{$package->package_name}', '{$package->package_version}', '{$package->package_status}')";
               $mysqli->query($query);
               // Prüfen Sie hier auf Fehler mit $mysqli->error und behandeln Sie diese entsprechend
            }
         } else {
            // Fehlerbehandlung, wenn das Einfügen der VM fehlschlägt
            file_put_contents('fail.log', json_encode($vmList));
            file_put_contents('fail.log', $query);
            file_put_contents('fail.log', $mysqli->error);
            echo "Error: " . $query . "<br>" . $mysqli->error;
         }
      }
      return $successCount;
   }else { return 0; }
}

function vmListToUpdate($vmList, $connection) {
   $successCount = 0;
   foreach ($vmList as $vm) {
       if (isset($vm->Id)) {
           $query = "UPDATE deploy_vms SET mission_id = ?, vm_name = ?, vm_hostname = ?, vm_domain = ?, vm_os = ?, vm_status = ?, vm_notes = ? WHERE id = ?";
           if ($stmt = $connection->prepare($query)) {
               $stmt->bind_param("issssssi", $vm->mission_id, $vm->vm_name, $vm->vm_hostname, $vm->vm_domain, $vm->vm_os, $vm->vm_status, $vm->vm_notes, $vm->Id);
               if (!$stmt->execute()) {
                   file_put_contents('fail.log', "Fehler beim Aktualisieren der VM mit ID " . $vm->Id . ": " . $stmt->error, FILE_APPEND);
                   echo "Fehler beim Aktualisieren der VM mit ID " . $vm->Id . ": " . $stmt->error;
               } else {
                   $successCount++;
                    
                   // Lösche vorhandene Interfaces der VM
                    $deleteInterfacesQuery = "DELETE FROM deploy_interfaces WHERE vm_id = ?";
                    if ($deleteInterfacesStmt = $connection->prepare($deleteInterfacesQuery)) {
                        $deleteInterfacesStmt->bind_param("i", $vm->Id);
                        $deleteInterfacesStmt->execute();
                        $deleteInterfacesStmt->close();
                    }

                    // Füge neue Interfaces ein
                    $insertInterfaceQuery = "INSERT INTO deploy_interfaces (vm_id, ip, subnet, gateway, dns1, dns2, vlan, mac) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                    foreach ($vm->interfaces as $interface) {
                        if ($insertInterfaceStmt = $connection->prepare($insertInterfaceQuery)) {
                            $insertInterfaceStmt->bind_param("isssssss", $vm->Id, $interface->ip, $interface->subnet, $interface->gateway, $interface->dns1, $interface->dns2, $interface->vlan, $interface->mac);
                            if (!$insertInterfaceStmt->execute()) {
                                file_put_contents('fail.log', "Fehler beim Einfügen des Interfaces für VM mit ID " . $vm->Id . ": " . $insertInterfaceStmt->error, FILE_APPEND);
                            }
                            $insertInterfaceStmt->close();
                        }
                    }


                   // Pakete aktualisieren
                   $deleteQuery = "DELETE FROM deploy_vm_packages WHERE vm_id = ?";
                   if ($deleteStmt = $connection->prepare($deleteQuery)) {
                       $deleteStmt->bind_param("i", $vm->Id);
                       $deleteStmt->execute(); // Vorhandene Zuordnungen löschen
                       $deleteStmt->close();

                       // Neue Paketzuordnungen einfügen
                       $insertQuery = "INSERT INTO deploy_vm_packages (vm_id, package_id) VALUES (?, ?)";
                       foreach ($vm->packages as $package) {
                           if ($insertStmt = $connection->prepare($insertQuery)) {
                               $insertStmt->bind_param("ii", $vm->Id, $package->id);
                               if (!$insertStmt->execute()) {
                                   file_put_contents('fail.log', "Fehler beim Zuordnen des Pakets mit ID " . $package->id . " zur VM mit ID " . $vm->Id . ": " . $insertStmt->error, FILE_APPEND);
                                   echo "Fehler beim Zuordnen des Pakets mit ID " . $package->id . " zur VM mit ID " . $vm->Id . ": " . $insertStmt->error;
                               }
                               $insertStmt->close();
                           }
                       }
                   }
               }
               $stmt->close();
           } else {
               echo "Fehler beim Vorbereiten des Update-Statements: " . $connection->error;
           }
       } else {
           file_put_contents('fail.log', json_encode($vm), FILE_APPEND);
           echo "Nicht alle erforderlichen Daten für das Update sind vorhanden.";
       }
   }
   return $successCount;
}



function vmListToCreate_old($missionId, $vmList, $connection){
   if (!empty($vmList)) {
      $successCount = 0;
      foreach ($vmList as $vm) {
         $query = "INSERT INTO deploy_vms (mission_id, vm_name, vm_hostname, vm_ip, vm_subnet, vm_gateway, vm_dns1, vm_dns2, vm_domain, vm_vlan, vm_role, vm_status, os_id, vm_os, vm_packages) VALUES ('{$missionId}','{$vm->vm_name}', '{$vm->vm_hostname}', '{$vm->vm_ip}', '{$vm->vm_subnet}', '{$vm->vm_gateway}', '{$vm->vm_dns1}', '{$vm->vm_dns2}', '{$vm->vm_domain}', '{$vm->vm_vlan}', 'aktiv', '', '{$vm->os_id}', '{$vm->vm_os}', '{$vm->vm_packages}')";
         $result = $connection->query($query);
         if ($result) {
            $successCount++;
         } else {
           
            file_put_contents('fail.log', json_encode($vmList));
            die('Error: ' . $connection->error);
         }
      }
      return $successCount;
   } else {
      return 0;
      // Speicher $vmList in Fail.log
   // Speicher $vmList in Fail.log
      // erstelle 

   file_put_contents('fail.log', json_encode($vmList));
      
   }
}
      


function vmListToUpdate_old($vmList, $connection){
   if (!empty($vmList)) {
      foreach ($vmList as $vm) {
         if($vm->Id != '' or $vm->Id != null){

            if($vm->vm_status == 'geändert - DB Sync!'){
               $status = "aktiv";
            }else{
               $status = $vm->vm_status;
            }

            $query = "UPDATE deploy_vms SET vm_name = '{$vm->vm_name}', vm_ip = '{$vm->vm_ip}', vm_subnet = '{$vm->vm_subnet}', vm_gateway = '{$vm->vm_gateway}', vm_dns1 = '{$vm->vm_dns1}', vm_dns2 = '{$vm->vm_dns2}', vm_domain = '{$vm->vm_domain}', vm_vlan = '{$vm->vm_vlan}', vm_role = '{$vm->vm_role}', vm_status = '$status', os_id = '{$vm->os_id}', vm_os = '{$vm->vm_os}', vm_packages = '{$vm->vm_packages}', updated_at = NOW() WHERE id = '{$vm->Id}'";
            $result = $connection->query($query);
            if (!$result) {
               die('Error: ' . $connection->error);
            }
         }
      }
      return true;
   } else {
      return false;
   }
}

function vmListToDelete($vmList, $connection){
   if (!empty($vmList)) {
      foreach ($vmList as $vm) {
         if($vm->Id != '' or $vm->Id != null){
            $query = "DELETE FROM deploy_vms WHERE id = '{$vm->Id}'";
            $result = $connection->query($query);
            if (!$result) {
               die('Error: ' . $connection->error);
            }
         }
      }
      return true;
   } else {
      return false;
   }
}



removeLog($connection);


