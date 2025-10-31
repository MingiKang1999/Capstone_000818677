<?php
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
	
	//check if name already exists
	$nameCheckSql = "SELECT username, hash, score FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $nameCheckSql);
	if (!$stmt) {
		echo ("Preperation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "s", $usernameFiltered);
	mysqli_stmt_execute($stmt);
	$nameCheck = mysqli_stmt_get_result($stmt);
	if (mysqli_num_rows($nameCheck) > 0){
		echo "Name already exists";
		exit();
	}
	
	// Hash the password before storing in the database for security and store the hashed password
	$hash = password_hash($passwordFiltered, PASSWORD_DEFAULT);
	$inserUserSql = "INSERT INTO players (username, hash) VALUES (?, ?)";
	$stmt = mysqli_prepare($connect, $inserUserSql);
	if (!$stmt){
		echo ("Preperation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "ss", $usernameFiltered, $hash);
	// If all operation was sucessful send 1 to indicate it
	if (mysqli_stmt_execute($stmt)) {
		echo "1"; // Success
		} else {
			echo "Error creating new player: " . mysqli_error($connect);
		}
		
	mysqli_stmt_close($stmt);
?>