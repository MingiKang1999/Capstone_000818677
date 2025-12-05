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

	//check if the connection was successful
	if (mysqli_connect_errno()){
		echo "Connection Failed";
		exit();
	}

	// Receive username from Unity input field and sanitize the information for security
	$username = mysqli_real_escape_string($connect, $_POST["username"]);
	$usernameFiltered = filter_var($username, FILTER_SANITIZE_SPECIAL_CHARS, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	if ($username != $usernameFiltered){
		echo "The username you entered is invalid";
		exit();
	}

	// Receive score from Unity input field and sanitize the information for security
	$newScore = $_POST["score"];
	if (filter_var($newScore, FILTER_VALIDATE_INT) === false){
		echo "The score entered is invalid";
		exit();
	}

	//check if name exists
	$nameCheckSql = "SELECT username FROM players WHERE username = ?";
	$nameCheck = mysqli_prepare($connect, $nameCheckSql);
	mysqli_stmt_bind_param($nameCheck, "s", $usernameFiltered);
	mysqli_stmt_execute($nameCheck);
	mysqli_stmt_store_result($nameCheck); // Hostinger-safe

	if (mysqli_stmt_num_rows($nameCheck) != 1){
		echo "Either no user with this username or more than one username";
		exit();
	}

	// Update the username with the new one given from Unity
	$updateSql = "UPDATE players SET score = ? WHERE username = ?";
	$stmt = mysqli_prepare($connect, $updateSql);
	if (!$stmt) {
		echo "Prepare failed: " . mysqli_error($connect);
		exit();
	}

	mysqli_stmt_bind_param($stmt, "is", $newScore, $usernameFiltered);

	// If all operation was successful send 1 to indicate it
	if (mysqli_stmt_execute($stmt)) {
		echo "1"; // Success
	} else {
		echo "Save failed: " . mysqli_error($connect);
	}

	mysqli_stmt_close($stmt);
?>