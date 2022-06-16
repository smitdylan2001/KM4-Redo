<?php
    include 'PHPLogin.php';    

    session_start();    

    //Get values
    $gamerTag = $_GET["name"];
    $mail = $_GET["mail"];
    $pw = $_GET["pass"];
    
    //Sanitize variables
    $name = filter_var($name, FILTER_SANITIZE_STRING); 
    $gamerTag = filter_var($gamerTag, FILTER_SANITIZE_EMAIL);
    $pw = filter_var($pw, FILTER_SANITIZE_STRING); 
    
    //Check if username ise mail
    if (filter_var($mail, FILTER_VALIDATE_EMAIL)) {
        //Check if gamertag does not contain spaces
        if (strpos($pw, ' ') == false) {
            //Check if password does not contain spaces
            if (strpos($pw, ' ') == false) {
                //Check if email already exists
                $query1 = "SELECT email FROM `Users` WHERE `email` = '$mail'";
                if ($result1 = $mysqli->query($query1)) {
                    $json1 = mysqli_fetch_all ($result1, MYSQLI_ASSOC);
                    if(empty($json1)){ //No previous email
                        //Make query
                        $query2 = "INSERT INTO Users (`id`, `name`, `email`, `password`, `serverID`) VALUES (NULL, '$gamerTag', '".$mail."', '$pw', 1)";
                
                        //Try to submit query
                        if ($result2 = $mysqli->query($query2)) echo "Hey, you are added $gamerTag"; //Succes
                        else echo '0'; //Failed
                    }
                    else echo "Email already exitst";
                }
                else echo "Cannot process query";
            }
            else echo "Password has spaces";
        }
        else echo "GamerTag has spaces";
    }
    else echo "No valid email";
?>