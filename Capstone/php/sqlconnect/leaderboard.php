<?php
/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/

	// Connect to DB
	$connect = mysqli_connect("localhost", "root", "", "unitydb");
	if (mysqli_connect_errno()) {
		echo json_encode(["error" => "Connection failed: " . mysqli_connect_error()]);
		exit();
	}

	// Get top 20 players (change ORDER BY if you want a different rule)
	$sql = "
		SELECT username, time 
		FROM players
		ORDER BY score DESC, time ASC
	";

	$result = mysqli_query($connect, $sql);
	if (!$result) {
		echo json_encode(["error" => "Query failed: " . mysqli_error($connect)]);
		mysqli_close($connect);
		exit();
	}

	// Build array for JSON
	$players = [];
	while ($row = mysqli_fetch_assoc($result)) {
		$players[] = [
			"username" => $row["username"],
			"time"     => $row["time"]      // TIME field as string, e.g. "00:01:23"
		];
	}

	// Send JSON back to Unity
	echo json_encode($players);

	mysqli_free_result($result);
	mysqli_close($connect);

?>