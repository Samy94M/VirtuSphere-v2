<?php
# Database:
# table: deploy_users
# id, name, password, email, created_at, updated_at

# table: deploy_vms
# id, mission_id, vm_name, vm_hostname, vm_ip, vm_subnet, vm_gateway, vm_dns1, vm_dns2, vm_domain, vm_vlan, vm_role, vm_status, os_id, created_at, updated_at

#table: deploy_packages
# id, user_id, package_name, package_version, package_status, created_at, updated_at

#table: deploy_logs
# id, user_id, log_message, created_at, updated_at

#table: deploy_tokens
# id, user_id, token, expired, created_at, updated_at

#table: deploy_missions
# id, mission_name, mission_status, created_at, updated_at

# erstelle einen user admin:admin

#table: deploy_os
# id, os_name

#table: deploy_vlan
# id, vlan_name



include 'mysql.php';
//include "function.php";

// Create tables if they don't exist. Name is unique, so it will throw an error if it already exists
$createUsersTable = "CREATE TABLE IF NOT EXISTS deploy_users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";


$createOS = "CREATE TABLE IF NOT EXISTS deploy_os (
    id INT AUTO_INCREMENT PRIMARY KEY unique,
    os_name VARCHAR(255) NOT NULL UNIQUE,
    os_status VARCHAR(255) NOT NULL
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
    mission_name VARCHAR(255) NOT NULL UNIQUE,
    mission_status VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$createVLAN = "CREATE TABLE IF NOT EXISTS deploy_vlan (
    id INT AUTO_INCREMENT PRIMARY KEY,
    vlan_name VARCHAR(255) NOT NULL UNIQUE
)";

$sql = file_get_contents('./struktur.sql');

$sqlStatements = array_filter(array_map('trim', explode(';', $sql)));

// Jede Anweisung ausführen
foreach ($sqlStatements as $statement) {
    if ($statement) {
        if (!mysqli_query($connection, $statement)) {
            echo "Fehler beim Ausführen von: $statement\n";
            echo 'MySQL-Fehler: ' . mysqli_error($connection);
        }
    }
}


# Erstelle eine result ausgabe für die mysqli querys
$result_user = mysqli_query($connection, $createUsersTable);
$result_logs = mysqli_query($connection, $createLogsTable);
$result_tokens = mysqli_query($connection, $createTokensTable);
$result_mission = mysqli_query($connection, $createMissionsTable);
$result_os = mysqli_query($connection, $createOS);
$result_vlan = mysqli_query($connection, $createVLAN);


## Besonderheiten

$vmTable = "ALTER TABLE deploy_vms ADD UNIQUE INDEX mission_vm_unique (mission_id, vm_name);";
$osTable = "ALTER TABLE deploy_os ADD UNIQUE INDEX os_name_unique (os_name);";
$missionTable = "ALTER TABLE deploy_missions ADD UNIQUE INDEX mission_name_unique (mission_name);";
$packageTable = "ALTER TABLE deploy_packages ADD UNIQUE INDEX package_name_unique (package_name);";
$usersTable = "ALTER TABLE deploy_users ADD UNIQUE INDEX user_name_unique (name);";

mysqli_query($connection, $vmTable);
mysqli_query($connection, $osTable);
mysqli_query($connection, $missionTable);
mysqli_query($connection, $usersTable);





?>

