<?php
    include 'PHPLogin.php';

    if (isset($_GET['PHPSESSID'])) { //staat de sessie id in de url?
        $sid=htmlspecialchars($_GET['PHPSESSID']); //sessie id uit url sanitizen
        session_id($sid); //sessie id voor deze sessie instellen naar wat uit url kwam
    }

    session_start();

    if (isset($_SESSION["server_id"]) && $_SESSION["server_id"]!=0) {
        //Get values
        $un = $_GET["username"];
        $pw = $_GET["pw"];

        //Sanitize variables
        $un = filter_var($un, FILTER_SANITIZE_EMAIL);
        $pw = filter_var($pw, FILTER_SANITIZE_STRING); 

        if($_SESSION['server_id'] != 0){
            //Check if username uses mail format
            if (filter_var($un, FILTER_VALIDATE_EMAIL)) {
                //Check if password does not contain spaces
                if (strpos($pw, ' ') == false) {
                    //Make query
                    $query = "SELECT id FROM Users WHERE email = '". $un ."' and password = '$pw' LIMIT 1";

                    //Try to process quiry
                    if (!($result = $mysqli->query($query)))
                                showerror($mysqli->errno,$mysqli->error);

                    //Get result from query
                    $row = $result->fetch_assoc();

                    //Check if everything is fetched
                    if(mysqli_num_rows($result) == 1){
                        //check if row username == username from url
                         if($row["Username"] == $Username)
                         {
                            //Add player id to session
                            $_SESSION['speler']=$row["id"];
                            echo $_SESSION['speler'];
                         }
                        else echo '0'; //Failed
                    }
                    else echo '0'; //Failed
                }
                else echo '0'; //Failed
            }
            else echo '0'; //Failed
        }
        else echo '0'; //Failed
    }
    else echo '0'; //Failed
?>