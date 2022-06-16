<?php
    include 'PHPLogin.php';    

   
        if (isset($_GET['PHPSESSID'])) { //staat de sessie id in de url?
            $sid=htmlspecialchars($_GET['PHPSESSID']); //sessie id uit url sanitizen
            session_id($sid); //sessie id voor deze sessie instellen naar wat uit url kwam
        }

        session_start();

        if (isset($_SESSION["server_id"]) && $_SESSION["server_id"]!=0) {
            //Get values
            $score = $_GET["score"];
            $userID = $_GET["id"];

            //Verify if values are int and check if player is current player
            if(filter_var($score, FILTER_VALIDATE_INT)
               && filter_var($userID, FILTER_VALIDATE_INT)
               && $userID == $_SESSION['speler'])
            {
                //Make query
                $query = "INSERT INTO Score (`id`, `userID`, `score`, `dateTime`) VALUES (NULL, $userID, $score, current_timestamp())";

                //Try to submit query
                if ($result = $mysqli->query($query)) echo '1'; //Succes
                else echo '0'; //Failed
            }
            else echo '0'; //Not properly setup
        }
        else  echo '0'; //foutmelding aan unity server dat ie opnieuw moet inloggen
?>