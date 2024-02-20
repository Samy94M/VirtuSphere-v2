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
   
   echo "Log entry added successfully";
}

function removeLog($connection){
   $query = "DELETE FROM deploy_logs where created_at < DATE_SUB(NOW(), INTERVAL 7 DAY)";
   $result = $connection->query($query);
   if (!$result) {
      die('Error: ' . $connection->error);
   }
   
}
// Create tables if they don't exist
$createUsersTable = "CREATE TABLE IF NOT EXISTS deploy_users (
   id INT AUTO_INCREMENT PRIMARY KEY,
   name VARCHAR(255) NOT NULL,
   password VARCHAR(255) NOT NULL,
   email VARCHAR(255) NOT NULL,
   created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$createVmsTable = "CREATE TABLE IF NOT EXISTS deploy_vms (
   id INT AUTO_INCREMENT PRIMARY KEY,
   vm_name VARCHAR(255) NOT NULL,
   vm_hostname VARCHAR(255) NOT NULL,
   vm_ip VARCHAR(255) NOT NULL,
   vm_subnet VARCHAR(255) NOT NULL,
   vm_gateway VARCHAR(255) NOT NULL,
   vm_dns1 VARCHAR(255) NOT NULL,
   vm_dns2 VARCHAR(255) NOT NULL,
   vm_domain VARCHAR(255) NOT NULL,
   vm_vlan VARCHAR(255) NOT NULL,
   vm_role VARCHAR(255) NOT NULL,
   vm_status VARCHAR(255) NOT NULL,
   created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$createPackagesTable = "CREATE TABLE IF NOT EXISTS deploy_packages (
   id INT AUTO_INCREMENT PRIMARY KEY,
   package_name VARCHAR(255) NOT NULL,
   package_version VARCHAR(255) NOT NULL,
   package_status VARCHAR(255) NOT NULL,
   created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$createLogsTable = "CREATE TABLE IF NOT EXISTS deploy_logs (
   id INT AUTO_INCREMENT PRIMARY KEY,
   ip VARCHAR(255) NOT NULL,
   log_message TEXT NOT NULL,
   created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

# erstelle $createTokensTable
$createTokensTable = "CREATE TABLE IF NOT EXISTS deploy_tokens (
   id INT AUTO_INCREMENT PRIMARY KEY,
   token VARCHAR(255) NOT NULL,
   expired BOOLEAN NOT NULL,
   created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

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
      die('Invalid login credentials');
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
   $query = "SELECT * FROM deploy_missions";
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

function getVMs($connection) {
   $query = "SELECT * FROM deploy_vms";
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




removeLog($connection);


