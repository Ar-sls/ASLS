<?php
if (isset($_GET['word'])) {
    $searchWord = $_GET['word'];
    
    // Construct API URL
    $apiUrl = "https://api.ar-sls.com/Blogs/GetBlogs?word=$searchWord";

    // Make API request
    $response = file_get_contents($apiUrl);

    // Process API response
    $result = json_decode($response, true);

    /* $_SESSION['search_guid'] = $result['guid']; */

    // Return the results as JSON
    header('Content-Type: application/json');
    echo json_encode($result);
}
?>
