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

	// Receive current username from Unity input field and sanitize the information for security
	$username = mysqli_real_escape_string($connect, $_POST["username"]);
	$usernameFiltered = filter_var($username, FILTER_SANITIZE_SPECIAL_CHARS, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	if ($username != $usernameFiltered){
		echo "The username you entered is invalid";
		exit();
	}

	// Receive new username from Unity input field and sanitize the information for security
	$newUsername = mysqli_real_escape_string($connect, $_POST["newUsername"]);
	$newUsernameFiltered = filter_var($newUsername, FILTER_SANITIZE_SPECIAL_CHARS, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	if ($newUsername != $newUsernameFiltered){
		echo "The new username you entered is invalid";
		exit();
	}

	// Optional: enforce minimum length for the new username
	if (strlen($newUsernameFiltered) < 8){
		echo "The new username must be at least 8 characters long";
		exit();
	}

	// Check if current username exists
	$nameCheckSql = "SELECT username FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $nameCheckSql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "s", $usernameFiltered);
	mysqli_stmt_execute($stmt);
	mysqli_stmt_store_result($stmt); // Hostinger-safe

	if (mysqli_stmt_num_rows($stmt) != 1){
		echo "The username you entered does not exist";
		mysqli_stmt_close($stmt);
		exit();
	}
	mysqli_stmt_close($stmt);

	// Check if the new username is already taken
	$newNameCheckSql = "SELECT username FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $newNameCheckSql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "s", $newUsernameFiltered);
	mysqli_stmt_execute($stmt);
	mysqli_stmt_store_result($stmt); // Hostinger-safe

	if (mysqli_stmt_num_rows($stmt) > 0){
		echo "The new username is already taken";
		mysqli_stmt_close($stmt);
		exit();
	}
	mysqli_stmt_close($stmt);

	// Update the username from old to new
	$updateSql = "UPDATE players SET username = ? WHERE username = ?";
	$stmt = mysqli_prepare($connect, $updateSql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "ss", $newUsernameFiltered, $usernameFiltered);

	// If all operation was successful send 1 to indicate it
	if (mysqli_stmt_execute($stmt)) {
		echo "1";
	} else {
		echo "User name update failed: " . mysqli_error($connect);
	}

	mysqli_stmt_close($stmt);
?>