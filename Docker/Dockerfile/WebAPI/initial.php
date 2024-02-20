<?php
# Database:
# table: deploy_users
# id, username, password, email, created_at, updated_at

# table: deploy_vms
# id, mission_id, vm_name, vm_hostname, vm_ip, vm_subnet, vm_gateway, vm_dns1, vm_dns2, vm_domain, vm_vlan, vm_role, vm_status, created_at, updated_at

#table: deploy_packages
# id, user_id, package_name, package_version, package_status, created_at, updated_at

#table: deploy_logs
# id, user_id, log_message, created_at, updated_at

#table: deploy_tokens
# id, user_id, token, expired, created_at, updated_at

#table: deploy_missions
# id, mission_name, mission_status, created_at, updated_at

# erstelle einen user admin:admin



include 'mysql.php';
include "function.php";

// Create tables if they don't exist. Name is unique, so it will throw an error if it already exists
$createUsersTable = "CREATE TABLE IF NOT EXISTS deploy_users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$createVmsTable = "CREATE TABLE IF NOT EXISTS deploy_vms (
    id INT AUTO_INCREMENT PRIMARY KEY,
    mission_id INT NOT NULL,
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

$createMissionsTable = "CREATE TABLE IF NOT EXISTS deploy_missions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    mission_name VARCHAR(255) NOT NULL,
    mission_status VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$password = 'admin';
$hashedPassword = password_hash($password, PASSWORD_DEFAULT);
$createUser = "INSERT INTO deploy_users (name, password, email) VALUES ('admin', '$hashedPassword', 'admin@localhost')";

# Erstelle für jede Tabelle 10 Testdaten in einer schleife
for ($i = 0; $i < 10; $i++) {
    $createTestdata_missiontable = "INSERT INTO deploy_missions (mission_name, mission_status) VALUES ('mission$i', 'active')";
    mysqli_query($connection, $createTestdata_missiontable);
}


for ($i = 0; $i < 10; $i++) {
    $createTestdata_vmstable = "INSERT INTO deploy_vms (mission_id, vm_name, vm_hostname, vm_ip, vm_subnet, vm_gateway, vm_dns1, vm_dns2, vm_domain, vm_vlan, vm_role, vm_status, created_at, updated_at) VALUES ($i, 'vm$i', 'vm$i', '192.168.1.$i', '', '', '','','', '', '', 'active', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

    mysqli_query($connection, $createTestdata_vmstable);
}

for ($i = 0; $i < 10; $i++) {
    $createTestdata_packagestable = "INSERT INTO deploy_packages (package_name, package_version, package_status) VALUES ('package$i', '1.0', 'active')";
    mysqli_query($connection, $createTestdata_packagestable);
}

echo $createTestdata_vmstable;



# Erstelle eine result ausgabe für die mysqli querys
$result_user = mysqli_query($connection, $createUsersTable);
$result_vm = mysqli_query($connection, $createVmsTable);
$result_package = mysqli_query($connection, $createPackagesTable);
$result_logs = mysqli_query($connection, $createLogsTable);
$result_tokens = mysqli_query($connection, $createTokensTable);
$result_user2 = mysqli_query($connection, $createUser);
$result_mission = mysqli_query($connection, $createMissionsTable);

# zeige die results an
echo "Users Table: " . $result_user . "<br>";
echo "VMs Table: " . $result_vm . "<br>";
echo "Packages Table: " . $result_package . "<br>";
echo "Logs Table: " . $result_logs . "<br>";
echo "Tokens Table: " . $result_tokens . "<br>";
echo "User: " . $result_user2 . "<br>";
echo "Mission: " . $result_mission . "<br>";

# erstelle ein addLog mit der aufgerufenden IP, Request, Auth-Token und der connection
addLog($_SERVER['REMOTE_ADDR'], $_SERVER['REQUEST_URI'], "none", $connection);

# schließe die verbindung
$connection->close();
?>

