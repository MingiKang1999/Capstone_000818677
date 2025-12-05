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

	// Receive petId and cost
	$petId = $_POST["petId"];
	$cost  = $_POST["cost"];

	if (filter_var($petId, FILTER_VALIDATE_INT) === false){
		echo "Invalid petId";
		exit();
	}
	if (filter_var($cost, FILTER_VALIDATE_INT) === false){
		echo "Invalid cost";
		exit();
	}

	// Check if user exists and get current score + pet
	$sql = "SELECT score, pet FROM players WHERE username = ?";
	$stmt = mysqli_prepare($connect, $sql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "s", $usernameFiltered);
	mysqli_stmt_execute($stmt);

	// Hostinger-safe: use store_result + bind_result instead of mysqli_stmt_get_result
	mysqli_stmt_store_result($stmt);

	if (mysqli_stmt_num_rows($stmt) != 1){
		echo "The username you entered does not exist";
		mysqli_stmt_close($stmt);
		exit();
	}

	// Fetch current score and pet
	mysqli_stmt_bind_result($stmt, $currentScore, $currentPet);
	mysqli_stmt_fetch($stmt);

	// Ensure numeric
	$currentScore = (int)$currentScore;

	mysqli_stmt_close($stmt);

	// Check if player can afford the pet
	if ($currentScore < (int)$cost){
		echo "Not enough tokens to buy this pet";
		exit();
	}

	$newScore = $currentScore - (int)$cost;
	$newPet   = (int)$petId;

	// Update score and pet
	$updateSql = "UPDATE players SET score = ?, pet = ? WHERE username = ?";
	$stmt = mysqli_prepare($connect, $updateSql);
	if (!$stmt) {
		echo ("Preparation Failed: " . mysqli_error($connect));
		exit();
	}
	mysqli_stmt_bind_param($stmt, "iis", $newScore, $newPet, $usernameFiltered);

	if (mysqli_stmt_execute($stmt)) {
		// success: send 1, newScore, petId
		echo "1\t" . $newScore . "\t" . $newPet;
	} else {
		echo "Pet purchase failed: " . mysqli_error($connect);
	}

	mysqli_stmt_close($stmt);
?>