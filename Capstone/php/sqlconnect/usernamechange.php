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
	
	// Receive new username from Unity input field and sanitize the information for security
	$newUsername = mysqli_real_escape_string($connect, $_POST["newUsername"]);
	$newUsernameFiltered = filter_var($newUsername, FILTER_SANITIZE_SPECIAL_CHARS, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	if ($newUsername != $newUsernameFiltered){
		echo "The new username you entered is invalid";
		exit();
	}
	
	// Change the username into the new username given from unity
	$updateSql = "UPDATE players SET username = ? WHERE username = ?";
	$stmt = mysqli_prepare($connect, $updateSql);
	if (!$stmt) {
		echo "Prepare failed: " . mysqli_error($connect);
		exit();
	}
	mysqli_stmt_bind_param($stmt, "ss", $newUsernameFiltered, $usernameFiltered);
	// If all operation was sucessful send 1 to indicate it
	if (mysqli_stmt_execute($stmt)) {
		echo "1"; // Sucess
	} else {
		echo "Save failed: " . mysqli_error($connect);
	}
	
	mysqli_stmt_close($stmt);
?>