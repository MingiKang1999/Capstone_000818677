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

	// Receive old password from Unity input field and sanitize the information for security
	$oldPassword = $_POST["oldPassword"];
	$oldPasswordFiltered = filter_var($oldPassword, FILTER_SANITIZE_SPECIAL_CHARS, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	if ($oldPassword != $oldPasswordFiltered){
		echo "The old password you entered is invalid";
		exit();
	}

	// Receive new password from Unity input field and sanitize the information for security
	$newPassword = $_POST["newPassword"];
	$newPasswordFiltered = filter_var($newPassword, FILTER_SANITIZE_SPECIAL_CHARS, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	if ($newPassword != $newPasswordFiltered){
		echo "The new password you entered is invalid";
		exit();
	}

	// Apply the same password rules as registration

	// Must contain at least one number
	if (!preg_match('/[0-9]/', $newPassword)) {
		echo "Password must contain at least one number.";
		exit();
	}

	// Must contain at least one uppercase letter
	if (!preg_match('/[A-Z]/', $newPassword)) {
		echo "Password must contain at least one uppercase letter.";
		exit();
	}

	// Must contain at least one special character
	if (!preg_match('/[^A-Za-z0-9]/', $newPassword)) {
		echo "Password must contain at least one special character.";
		exit();
	}

	// Optional: if you also want a minimum length (you can remove this if not needed)
	if (strlen($newPasswordFiltered) < 8){
		echo "Password must be at least 8 characters long.";
		exit();
	}

	//Check if name exists and get current hash
	$nameCheckSql = "SELECT username, hash FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $nameCheckSql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "s", $usernameFiltered);
	mysqli_stmt_execute($stmt);

	// use store_result + num_rows instead of mysqli_stmt_get_result
	mysqli_stmt_store_result($stmt);

	if (mysqli_stmt_num_rows($stmt) != 1){
		echo "The username you entered does not exist";
		mysqli_stmt_close($stmt);
		exit();
	}

	// Bind and fetch row
	mysqli_stmt_bind_result($stmt, $dbUsername, $dbHash);
	mysqli_stmt_fetch($stmt);

	// Check if the hashed password from the database matches the old one given from Unity
	if (!password_verify($oldPasswordFiltered, $dbHash)){
		echo "Incorrect old password entered";
		mysqli_stmt_close($stmt);
		exit();
	}

	mysqli_stmt_close($stmt);

	// Hash the new password before storing in the database
	$newHash = password_hash($newPasswordFiltered, PASSWORD_DEFAULT);

	// Update the stored hash with the new hashed password
	$updateSql = "UPDATE players SET hash = ? WHERE username = ?";
	$updateStmt = mysqli_prepare($connect, $updateSql);
	if (!$updateStmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($updateStmt, "ss", $newHash, $usernameFiltered);

	// If all operation was successful send 1 to indicate it
	if (mysqli_stmt_execute($updateStmt)) {
		echo "1";
	} else {
		echo "Password update failed: " . mysqli_error($connect);
	}

	mysqli_stmt_close($updateStmt);
?>