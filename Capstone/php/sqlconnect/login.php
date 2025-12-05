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

	//Check if name exists
	$nameCheckSql = "SELECT username, hash, score, pet FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $nameCheckSql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "s", $usernameFiltered);
	mysqli_stmt_execute($stmt);

	// use store_result + num_rows instead of mysqli_stmt_get_result
	mysqli_stmt_store_result($stmt); // buffer the result for num_rows

	if (mysqli_stmt_num_rows($stmt) != 1){
		echo "The username you entered does not exist";
		exit();
	}

	// Bind output columns to variables
	mysqli_stmt_bind_result($stmt, $dbUsername, $dbHash, $dbScore, $dbPet);
	mysqli_stmt_fetch($stmt);

	// Check if the hashed password from the database matches the one given from Unity
	$hash = $dbHash;

	if (!password_verify($passwordFiltered, $hash)){
		echo "Incorrect Password Entered";
		exit();
	}

	// If all operation was sucessful send 1 to indicate it
	echo "1\t" . $dbScore . "\t" . $dbPet;

	mysqli_stmt_close($stmt);
?>