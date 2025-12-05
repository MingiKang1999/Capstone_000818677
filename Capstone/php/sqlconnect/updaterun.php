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

	$connect = mysqli_connect("", "", "", "");

	// Check if the connection was successful
	if (mysqli_connect_errno()){
		echo "Connection Failed";
		exit();
	}

	// Receive username from Unity input field and sanitize the information for security
	$username = mysqli_real_escape_string($connect, $_POST["username"]);
	$usernameFiltered = filter_var(
		$username,
		FILTER_SANITIZE_SPECIAL_CHARS,
		FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH
	);

	if ($username != $usernameFiltered){
		echo "The username you entered is invalid";
		exit();
	}

	// Receive runtime from Unity (format: hh:mm:ss) and validate
	$runTime = $_POST["runtime"];

	if (!preg_match('/^\d{2}:\d{2}:\d{2}$/', $runTime)) {
		echo "The run time entered is invalid";
		exit();
	}

	// Receive tokens from Unity and validate
	$tokens = $_POST["tokens"];
	if (filter_var($tokens, FILTER_VALIDATE_INT) === false){
		echo "The token count entered is invalid";
		exit();
	}

	//  Get current time + score
	$nameCheckSql = "SELECT time, score FROM players WHERE username = ?";
	$nameCheck = mysqli_prepare($connect, $nameCheckSql);
	if (!$nameCheck) {
		echo "Prepare failed: " . mysqli_error($connect);
		exit();
	}

	mysqli_stmt_bind_param($nameCheck, "s", $usernameFiltered);
	mysqli_stmt_execute($nameCheck);
	mysqli_stmt_store_result($nameCheck);

	if (mysqli_stmt_num_rows($nameCheck) != 1){
		echo "Either no user with this username or more than one username";
		exit();
	}

	mysqli_stmt_bind_result($nameCheck, $currentTime, $currentScore);
	mysqli_stmt_fetch($nameCheck);
	mysqli_stmt_close($nameCheck);

	// Default if null
	if ($currentTime === null) {
		$currentTime = "00:00:00";
	}
	if ($currentScore === null) {
		$currentScore = 0;
	}

	//  Compute new score (add tokens)
	$newScore = $currentScore + (int)$tokens;

	// Helper to convert "hh:mm:ss" to seconds
	function timeToSeconds($t) {
		list($h, $m, $s) = explode(':', $t);
		return $h * 3600 + $m * 60 + $s;
	}

	$finalTime = $currentTime;

	// If currentTime is 00:00:00 (no record yet) OR new run is faster
	if ($currentTime === "00:00:00" || timeToSeconds($runTime) < timeToSeconds($currentTime)) {
		$finalTime = $runTime;
	}

	//  UPDATE players row
	$updateSql = "UPDATE players 
				  SET time = ?, score = ? 
				  WHERE username = ?";
	$stmt = mysqli_prepare($connect, $updateSql);

	if (!$stmt) {
		echo "Prepare failed: " . mysqli_error($connect);
		exit();
	}

	// s = time string, i = int score, s = username
	mysqli_stmt_bind_param($stmt, "sis", $finalTime, $newScore, $usernameFiltered);

	if (mysqli_stmt_execute($stmt)) {
		echo "1"; // Success
	} else {
		echo "Save failed: " . mysqli_error($connect);
	}

	mysqli_stmt_close($stmt);
?>