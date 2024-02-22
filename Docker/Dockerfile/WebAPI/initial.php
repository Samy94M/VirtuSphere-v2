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
    os_id INT NOT NULL,
    vm_status VARCHAR(255) NOT NULL,
    vm_packages VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$createOS = "CREATE TABLE IF NOT EXISTS deploy_os (
    id INT AUTO_INCREMENT PRIMARY KEY unique,
    os_name VARCHAR(255) NOT NULL UNIQUE,
    os_status VARCHAR(255) NOT NULL
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
    mission_name VARCHAR(255) NOT NULL UNIQUE,
    mission_status VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
)";

$createVLAN = "CREATE TABLE IF NOT EXISTS deploy_vlan (
    id INT AUTO_INCREMENT PRIMARY KEY,
    vlan_name VARCHAR(255) NOT NULL UNIQUE
)";


# Erstelle eine result ausgabe für die mysqli querys
$result_user = mysqli_query($connection, $createUsersTable);
$result_vm = mysqli_query($connection, $createVmsTable);
$result_package = mysqli_query($connection, $createPackagesTable);
$result_logs = mysqli_query($connection, $createLogsTable);
$result_tokens = mysqli_query($connection, $createTokensTable);
$result_user2 = mysqli_query($connection, $createUser);
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
mysqli_query($connection, $packageTable);
mysqli_query($connection, $usersTable);






############# Testdaten

$password = 'admin';
$hashedPassword = password_hash($password, PASSWORD_DEFAULT);
$createUser = "INSERT INTO deploy_users (name, password, email) VALUES ('admin', '$hashedPassword', 'admin@localhost')";
mysqli_query($connection, $createUser);

$softwarePackages = [
    ['id' => 1, 'Notepad++', '7.9.5', 'active'],
    ['id' => 2, 'VLC Media Player', '3.0.12', 'active'],
    ['id' => 3, 'GIMP', '2.10.22', 'active'],
    ['id' => 4, 'Firefox', '85.0.2', 'active'],
    ['id' => 5, 'Google Chrome', '88.0.4324.146', 'active'],
    ['id' => 6, 'LibreOffice', '7.1', 'active'],
    ['id' => 7, '7-Zip', '19.00', 'active'],
    ['id' => 8, 'Audacity', '2.4.2', 'active'],
    ['id' => 9, 'FileZilla', '3.52.2', 'active'],
    ['id' => 10, 'Thunderbird', '78.7.1', 'active']
];

foreach ($softwarePackages as $package) {
    $createTestdata_packagestable = "INSERT INTO deploy_packages (id, package_name, package_version, package_status) VALUES ($package[id], '$package[0]', '$package[1]', '$package[2]')";
    mysqli_query($connection, $createTestdata_packagestable);
}


$osData = [
    ['id' => 1, 'Windows Server 2012 R2', 'active'],
    ['id' => 2, 'Windows Server 2016', 'active'],
    ['id' => 3, 'Windows Server 2019', 'active'],
    ['id' => 4, 'Windows 10', 'active'],
    ['id' => 5, 'Ubuntu 18.04', 'active'],
    ['id' => 6, 'Ubuntu 20.04', 'active'],
    ['id' => 7, 'Debian 9', 'active'],
    ['id' => 8, 'Debian 10', 'active'],
    ['id' => 9, 'CentOS 7', 'active'],
    ['id' => 10, 'CentOS 8', 'active']
];

foreach($osData as $os) {
    $createTestdata_ostable = "INSERT INTO deploy_os (id, os_name, os_status) VALUES ($os[id], '$os[0]', '$os[1]')";
    echo $createTestdata_ostable;
    mysqli_query($connection, $createTestdata_ostable);
}

$missionData = [
    ['id' => 1, 'mission_name' => 'Sauerland', 'mission_status' => 'active'],
    ['id' => 2, 'mission_name' => 'Mosel', 'mission_status' => 'active'],
    ['id' => 3, 'mission_name' => 'Eifel', 'mission_status' => 'pending'],
    ['id' => 4, 'mission_name' => 'Hunsrück', 'mission_status' => 'active'],
    ['id' => 5, 'mission_name' => 'Westerwald', 'mission_status' => 'active'],
    ['id' => 6, 'mission_name' => 'Bergisches Land', 'mission_status' => 'active'],
    ['id' => 7, 'mission_name' => 'Siegerland', 'mission_status' => 'active'],
    ['id' => 8, 'mission_name' => 'Ruhrgebiet', 'mission_status' => 'active'],
    ['id' => 9, 'mission_name' => 'Münsterland', 'mission_status' => 'active'],
    ['id' => 10, 'mission_name' => 'Rheinland', 'mission_status' => 'active'],
    ['id' => 11, 'mission_name' => 'Schwaben', 'mission_status' => 'active'],
    ['id' => 12, 'mission_name' => 'Oberbayern', 'mission_status' => 'active'],
    ['id' => 13, 'mission_name' => 'Niederbayern', 'mission_status' => 'active'],
    ['id' => 14, 'mission_name' => 'Franken', 'mission_status' => 'active'],
    ['id' => 15, 'mission_name' => 'Ostfriesland', 'mission_status' => 'active'],
    ['id' => 16, 'mission_name' => 'Harz', 'mission_status' => 'active'],
    ['id' => 17, 'mission_name' => 'Thüringer Wald', 'mission_status' => 'active'],
    ['id' => 18, 'mission_name' => 'Sächsische Schweiz', 'mission_status' => 'active'],
    ['id' => 19, 'mission_name' => 'Lüneburger Heide', 'mission_status' => 'active'],
    ['id' => 20, 'mission_name' => 'Bodensee', 'mission_status' => 'active']
];

foreach ($missionData as $mission) {
    $createTestdata_missionstable = "INSERT INTO deploy_missions (id, mission_name, mission_status, created_at, updated_at) VALUES ($mission[id], '$mission[mission_name]', '$mission[mission_status]', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) ON DUPLICATE KEY UPDATE mission_name = VALUES(mission_name), mission_status = VALUES(mission_status), updated_at = VALUES(updated_at)";
    mysqli_query($connection, $createTestdata_missionstable);
}

$vmData = [
    ['mission_id' => 1, 'vm_name' => 'SL_DC_01', 'vm_hostname' => 'dc01.sauerland.local', 'vm_ip' => '10.0.1.11', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.1.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'sauerland.local', 'vm_vlan' => '100', 'vm_role' => 'Domain Controller', 'vm_status' => 'active', 'os_id' => 3],
    ['mission_id' => 1, 'vm_name' => 'SL_FS_01', 'vm_hostname' => 'fs01.sauerland.local', 'vm_ip' => '10.0.1.12', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.1.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'sauerland.local', 'vm_vlan' => '100', 'vm_role' => 'File Server', 'vm_status' => 'active', 'os_id' => 5],
    ['mission_id' => 2, 'vm_name' => 'MZ_DB_02', 'vm_hostname' => 'db02.mosel.local', 'vm_ip' => '10.0.2.13', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.2.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'mosel.local', 'vm_vlan' => '200', 'vm_role' => 'Database Server', 'vm_status' => 'active', 'os_id' => 6],
    ['mission_id' => 3, 'vm_name' => 'EF_APP_03', 'vm_hostname' => 'app03.eifel.local', 'vm_ip' => '10.0.3.14', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.3.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'eifel.local', 'vm_vlan' => '300', 'vm_role' => 'Application Server', 'vm_status' => 'pending', 'os_id' => 1],
    ['mission_id' => 20, 'vm_name' => 'BD_FS_05', 'vm_hostname' => 'fs05.bodensee.local', 'vm_ip' => '10.0.20.15', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.20.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'bodensee.local', 'vm_vlan' => '200', 'vm_role' => 'File Server', 'vm_status' => 'active', 'os_id' => 10],
    ['mission_id' => 2, 'vm_name' => 'MZ_DB_01', 'vm_hostname' => 'db01.mosel.local', 'vm_ip' => '10.0.2.13', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.2.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'mosel.local', 'vm_vlan' => '200', 'vm_role' => 'Database Server', 'vm_status' => 'active', 'os_id' => 6],
    ['mission_id' => 3, 'vm_name' => 'EF_APP_01', 'vm_hostname' => 'app01.eifel.local', 'vm_ip' => '10.0.3.14', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.3.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'eifel.local', 'vm_vlan' => '300', 'vm_role' => 'Application Server', 'vm_status' => 'active', 'os_id' => 7],
    ['mission_id' => 10, 'vm_name' => 'RH_WEB_03', 'vm_hostname' => 'web03.rheinland.local', 'vm_ip' => '10.0.10.19', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.10.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'rheinland.local', 'vm_vlan' => '1000', 'vm_role' => 'Web Server', 'vm_status' => 'active', 'os_id' => 3],
    ['mission_id' => 15, 'vm_name' => 'OSF_DB_02', 'vm_hostname' => 'db02.ostfriesland.local', 'vm_ip' => '10.0.15.20', 'vm_subnet' => '255.255.255.0', 'vm_gateway' => '10.0.15.1', 'vm_dns1' => '8.8.8.8', 'vm_dns2' => '8.8.4.4', 'vm_domain' => 'ostfriesland.local', 'vm_vlan' => '1500', 'vm_role' => 'Database Server', 'vm_status' => 'active', 'os_id' => 5]
];


foreach ($vmData as $vm) {
    $createTestdata_vmstable = "INSERT INTO deploy_vms (mission_id, vm_name, vm_hostname, vm_ip, vm_subnet, vm_gateway, vm_dns1, vm_dns2, vm_domain, vm_vlan, vm_role, vm_status, os_id, created_at, updated_at) VALUES ($vm[mission_id], '$vm[vm_name]', '$vm[vm_hostname]', '$vm[vm_ip]', '$vm[vm_subnet]', '$vm[vm_gateway]', '$vm[vm_dns1]', '$vm[vm_dns2]', '$vm[vm_domain]', '$vm[vm_vlan]', '$vm[vm_role]', '$vm[vm_status]', $vm[os_id], CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) ON DUPLICATE KEY UPDATE vm_name = VALUES(vm_name), vm_hostname = VALUES(vm_hostname), vm_ip = VALUES(vm_ip), vm_subnet = VALUES(vm_subnet), vm_gateway = VALUES(vm_gateway), vm_dns1 = VALUES(vm_dns1), vm_dns2 = VALUES(vm_dns2), vm_domain = VALUES(vm_domain), vm_vlan = VALUES(vm_vlan), vm_role = VALUES(vm_role), vm_status = VALUES(vm_status), os_id = VALUES(os_id), updated_at = VALUES(updated_at)";
    mysqli_query($connection, $createTestdata_vmstable);
}

$vlanData = [
    ['id' => 1, 'vlan_name' => '100'],
    ['id' => 2, 'vlan_name' => '200'],
    ['id' => 3, 'vlan_name' => '300'],
    ['id' => 4, 'vlan_name' => '400'],
    ['id' => 5, 'vlan_name' => '500'],
    ['id' => 6, 'vlan_name' => '600'],
    ['id' => 7, 'vlan_name' => '700'],
    ['id' => 8, 'vlan_name' => '800'],
    ['id' => 9, 'vlan_name' => '900'],
    ['id' => 10, 'vlan_name' => '1000'],
    ['id' => 11, 'vlan_name' => '1100'],
    ['id' => 12, 'vlan_name' => '1200'],
    ['id' => 13, 'vlan_name' => '1300'],
    ['id' => 14, 'vlan_name' => '1400'],
    ['id' => 15, 'vlan_name' => '1500'],
    ['id' => 16, 'vlan_name' => '1600'],
    ['id' => 17, 'vlan_name' => '1700'],
    ['id' => 18, 'vlan_name' => '1800'],
    ['id' => 19, 'vlan_name' => '1900'],
    ['id' => 20, 'vlan_name' => '2000']
];

foreach ($vlanData as $vlan) {
    $createTestdata_vlantable = "INSERT INTO deploy_vlan (id, vlan_name) VALUES ($vlan[id], '$vlan[vlan_name]')";
    mysqli_query($connection, $createTestdata_vlantable);
}



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

