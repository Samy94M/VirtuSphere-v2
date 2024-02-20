<?php

require_once 'mysql.php';
require_once 'function.php';

$token = isset($_GET['token']) ? htmlspecialchars($_GET['token']) : '';

if (!verifyToken($token, $connection)) {
    header('HTTP/1.1 403 Forbidden');
    echo 'Access Forbidden';
    exit;
}

### Ab hier ausgabe nur noch in Json
header('Content-Type: application/json');


# Wenn action = addVM dann rufe createVM($vmName, $vmHostname, $vmIP, $vmSubnet, $vmGateway, $vmDNS1, $vmDNS2, $vmDomain, $vmVLAN, $vmRole, $vmStatus, $connection) auf
if (isset($_POST['action']) && $_POST['action'] == 'addVM') {
    $vmName = isset($_POST['vmName']) ? htmlspecialchars($_POST['vmName']) : '';
    $vmHostname = isset($_POST['vmHostname']) ? htmlspecialchars($_POST['vmHostname']) : '';
    $vmIP = isset($_POST['vmIP']) ? htmlspecialchars($_POST['vmIP']) : '';
    $vmSubnet = isset($_POST['vmSubnet']) ? htmlspecialchars($_POST['vmSubnet']) : '';
    $vmGateway = isset($_POST['vmGateway']) ? htmlspecialchars($_POST['vmGateway']) : '';
    $vmDNS1 = isset($_POST['vmDNS1']) ? htmlspecialchars($_POST['vmDNS1']) : '';
    $vmDNS2 = isset($_POST['vmDNS2']) ? htmlspecialchars($_POST['vmDNS2']) : '';
    $vmDomain = isset($_POST['vmDomain']) ? htmlspecialchars($_POST['vmDomain']) : '';
    $vmVLAN = isset($_POST['vmVLAN']) ? htmlspecialchars($_POST['vmVLAN']) : '';
    $vmRole = isset($_POST['vmRole']) ? htmlspecialchars($_POST['vmRole']) : '';
    $vmStatus = isset($_POST['vmStatus']) ? htmlspecialchars($_POST['vmStatus']) : '';

    $result = createVM($vmName, $vmHostname, $vmIP, $vmSubnet, $vmGateway, $vmDNS1, $vmDNS2, $vmDomain, $vmVLAN, $vmRole, $vmStatus, $token, $connection);
    echo json_encode($result);
}

# Schreibe eine getMission Funktion die alle Missionen aus der Datenbank holt und als Json ausgibt
if (isset($_GET['action']) && $_GET['action'] == 'getMissions') {
    $result = getMissions($connection);
    echo json_encode($result);
}


# Schreibe eine getVM Funktion die alle VMs aus der Datenbank holt und als Json ausgibt
if (isset($_GET['action']) && $_GET['action'] == 'getVMs') {
    $result = getVMs($connection);
    echo json_encode($result);
}


# schreibe eine getPackage Funktion die alle Packages aus der Datenbank holt und als Json ausgibt
if (isset($_GET['action']) && $_GET['action'] == 'getPackages') {
    $result = getPackages($connection);
    echo json_encode($result);
}
?>
