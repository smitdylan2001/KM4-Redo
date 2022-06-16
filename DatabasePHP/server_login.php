<?php
    include 'PHPLogin.php';

    session_start();

    //Get values
    $id = $_GET["id"];
    $pw = $_GET["pw"];

    //Check if id is int
    if (filter_var($id, FILTER_VALIDATE_INT)) {
        //Sanitize password string
        $pw = filter_var($pw, FILTER_SANITIZE_STRING); 

        //Check if password does not contain spaces
        if (strpos($pw, ' ') == false) {
            //Make query
            $query = "SELECT id FROM Servers WHERE id = '". $id ."' and password = '$pw'";
            
            //Try to precess query
            if (!($result = $mysqli->query($query)))
                    showerror($mysqli->errno,$mysqli->error);

            $row = $result->fetch_assoc();

            if(mysqli_num_rows($result) == 1){
                //check if row id == id from url
                if($row["id"] == $id)
                {
                    $_SESSION['server_id'] = $row["id"]; //instead of GET get variable from row id

                    echo session_id();
                }
                
        
            }else{
                echo "0";
            }
        }else{
            echo "0";
        }
    }else {
      echo "0";
    }
?>