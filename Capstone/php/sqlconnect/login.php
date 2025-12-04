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
	
	//Check if name exists
	$nameCheckSql = "SELECT username, hash, score FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $nameCheckSql);
	if (!$stmt) {
		echo ("Preperation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "s", $usernameFiltered);
	mysqli_stmt_execute($stmt);
	$nameCheck = mysqli_stmt_get_result($stmt);
	if (mysqli_num_rows($nameCheck) != 1){
		echo "The username you entered does not exist";
		exit();
	}
	
	// Check if the hashed password from the database matches the one given from Unity
	$loginInfo = mysqli_fetch_assoc($nameCheck);
	$hash = $loginInfo["hash"];
	
	if (!password_verify($passwordFiltered, $hash)){
		echo "Incorrect Password Entered";
		exit();
	}
	// If all operation was sucessful send 1 to indicate it
	echo "1\t" . $loginInfo["score"];
	mysqli_stmt_close($stmt);
?>