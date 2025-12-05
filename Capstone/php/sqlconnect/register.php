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

	// Receive password from Unity input field and sanitize the information for security
	$password = $_POST["password"];
	$passwordFiltered = filter_var($password, FILTER_SANITIZE_SPECIAL_CHARS, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	if ($password != $passwordFiltered){
		echo "The password you entered is invalid";
		exit();
	}

	if (!preg_match('/[0-9]/', $password)) {
		echo "Password must contain at least one number.";
		exit();
	}

	if (!preg_match('/[A-Z]/', $password)) {
		echo "Password must contain at least one uppercase letter.";
		exit();
	}

	if (!preg_match('/[^A-Za-z0-9]/', $password)) {
		echo "Password must contain at least one special character.";
		exit();
	}

	//check if name already exists
	$nameCheckSql = "SELECT username, hash, score FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $nameCheckSql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}

	mysqli_stmt_bind_param($stmt, "s", $usernameFiltered);
	mysqli_stmt_execute($stmt);

	// use store_result + num_rows instead of mysqli_stmt_get_result
	mysqli_stmt_store_result($stmt); // buffer results for num_rows

	if (mysqli_stmt_num_rows($stmt) > 0){
		echo "Name already exists";
		exit();
	}

	// Hash the password before storing in the database for security and store the hashed password
	$hash = password_hash($passwordFiltered, PASSWORD_DEFAULT);

	// Default starting time = 10 hours (10:00:00)
	$defaultTime = "10:00:00";

	// Insert new user with default time
	$insertUserSql = "INSERT INTO players (username, hash, time) VALUES (?, ?, ?)";
	$stmt = mysqli_prepare($connect, $insertUserSql);
	if (!$stmt){
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}

	mysqli_stmt_bind_param($stmt, "sss", $usernameFiltered, $hash, $defaultTime);

	// If all operation was successful send 1 to indicate it
	if (mysqli_stmt_execute($stmt)) {
		echo "1"; // Success
	} else {
		echo "Error creating new player: " . mysqli_error($connect);
	}

	mysqli_stmt_close($stmt);
?>