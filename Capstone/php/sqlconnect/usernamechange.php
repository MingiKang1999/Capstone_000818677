<?php
/*
/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/

    $connect = mysqli_connect("localhost", "root", "", "unitydb");
    //check if the connection was sucessful
    if (mysqli_connect_errno()){
        echo "Connection Failed";
        exit();
    }

    $query = "SELECT username, time FROM players ORDER BY score DESC, time ASC LIMIT 20";

    $stmt = mysqli_prepare($connect, $query);
    if (!$stmt) {
        echo "Prepare failed: " . mysqli_error($connect);
        exit();
    }

    mysqli_stmt_execute($stmt);
    $result = mysqli_stmt_get_result($stmt);

    if (!$result) {
        echo "Query failed: " . mysqli_error($connect);
        exit();
    }

    // Build leaderboard array
    $leaderboard = array();

    while ($row = mysqli_fetch_assoc($result)) {
        $leaderboard[] = array(
            "username" => $row["username"],
            "time" => $row["time"]
        );
    }

    // Return JSON so Unity can decode it easily
    echo json_encode($leaderboard);

    mysqli_stmt_close($stmt);
?>