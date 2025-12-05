<?php
/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/

	// CORS headers for WebGL on itch.io
	header("Access-Control-Allow-Origin: *"); // or restrict to your itch.io URL
	header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
	header("Access-Control-Allow-Headers: Content-Type");

	// Handle preflight OPTIONS request
	if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
		http_response_code(200);
		exit();
	}

	// Connect to DB
	$connect = mysqli_connect("", "", "", "");
	if (mysqli_connect_errno()) {
		echo json_encode(["error" => "Connection failed: " . mysqli_connect_error()]);
		exit();
	}

	// Get top players (order by score DESC, tie-break by time ASC)
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
			"time"     => $row["time"]      // TIME field as string
		];
	}

	// Send JSON back to Unity
	echo json_encode($players);

	mysqli_free_result($result);
	mysqli_close($connect);
?>