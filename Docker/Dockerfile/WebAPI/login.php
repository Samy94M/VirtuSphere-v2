<?php

require_once 'mysql.php';
require_once 'function.php';



# bei action = login soll die funktion generateToken($username, $password, $connection) aufgerufen werden. die POST Parameter sollen vorher auf sicherheit geprüft werden
if (isset($_POST['action']) && $_POST['action'] == 'login') {
   $username = isset($_POST['username']) ? htmlspecialchars($_POST['username']) : '';
   $password = isset($_POST['password']) ? htmlspecialchars($_POST['password']) : '';
   $token = generateToken($username, $password, $connection);
   echo json_encode($token);
}