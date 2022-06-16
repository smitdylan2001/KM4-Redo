<?php
    include 'PHPLogin.php';    

    session_start();    

    
    //Query all entries from last month
    $query1 = "SELECT * FROM Score WHERE dateTime >= DATE_FORMAT( CURRENT_DATE - INTERVAL 1 MONTH, '%Y/%m/01' )";
        
    if (!($result1 = $mysqli->query($query1)))
                showerror($mysqli->errno,$mysqli->error);
    
    //Get result from query
    $json1 = mysqli_fetch_all ($result1, MYSQLI_ASSOC);
    
    //Echo text and play count this month
    echo "Total plays last month: " . count($json1) . "</br>" . "Fastest scores this month: </br>";
        

    //Query quickest scores, with time of entry being the secondary sort in case of same score
    $query = "SELECT userID, score, dateTime FROM `Score` WHERE dateTime >= DATE_FORMAT( CURRENT_DATE - INTERVAL 1 MONTH, '%Y/%m/01' ) ORDER BY score ASC, dateTime ASC LIMIT 5";
    
    //Try to process quiry
    if (!($result = $mysqli->query($query)))
                showerror($mysqli->errno,$mysqli->error);
    
    //Get result from query
    $json = mysqli_fetch_all ($result, MYSQLI_ASSOC);

    //Echo results
    echo json_encode($json);
?>