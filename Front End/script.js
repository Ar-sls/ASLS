// Variables to store the current page and the number of items per page
var currentPage = 1;
var itemsPerPage = 10;

function search() {
    var searchWord = encodeURIComponent(document.getElementById('searchInput').value);

    // Show loading animation
    showLoading();

    $.ajax({
        type: 'GET',
        url: './search.php',
        data: { word: searchWord },
        success: function(response) {
            if (response.noBlogs > 0) {
                var sentenceLists = response.sentenceLists;

                if (sentenceLists && sentenceLists.length > 0) {
                    var resultList = '<ul>';

                    sentenceLists.forEach(function(sentenceObj) {
                        resultList += '<li>' + sentenceObj.sentenceHTML + '</li>';
                    });

                    resultList += `
                    
                    <div id="pagination"></div>

                    <div class="buttons_box">
                    <div class="btnx">
                    <form id="cleanForm" action="./clean.php">
                        <input type="hidden" id="guidInput" name="guidInput">
                        <button type="button" onclick="clean()">تشذيب السياقات</button>
                    </form>
                    <form id="StemForm" action="./Stem.php">
                        <input type="hidden" id="StemguidInput" name="StemguidInput">
                        <button type="button" onclick="Stem()">تجزيع الكلمات</button>
                    </form>
                    </div>
                    <div>
                    <button class="Btn" id="print_btn" onclick="printContent()">
                       <svg class="svgIcon" viewBox="0 0 384 512" height="1em" xmlns="http://www.w3.org/2000/svg"><path d="M169.4 470.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 370.8 224 64c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 306.7L54.6 265.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"></path></svg>
                       <span class="icon2"></span>
                       <span class="tooltip">Download</span>
                    </button>
                    </div>
                    </div>`;


                    resultList += '</ul>';

                    // Display the results in the searchResults div
                    $('#searchResults').html('<div class="head_tag">'+
                    '<h2>سياقات الكلمة</h2>'+
                    '</div>'+
                    '<p>عدد المدونات التي تم العثور عليها: ' + response.noBlogs + '</p>' + resultList);

                    // Save the GUID to the input field
                    saveGuidToCleanInput(response.guid);
                    saveGuidToStemInput(response.guid);


                } else {
                    // Display a message if no sentences were found
                    $('#searchResults').html('<p>No sentences found for the search term: ' + decodeURIComponent(response.wordsearch) + '</p>');
                }
            } else {
                // Display a message if no blogs were found
                $('#searchResults').html('<p>No blogs found for the search term: ' + decodeURIComponent(response.wordsearch) + '</p>');
            }
        },
        error: function(error) {
            console.log('Error:', error);
        },
        complete: function() {
            // Hide loading animation after the request is complete (success or error)
            hideLoading();
        }
    });
}

function clean() {
    var guidInput = encodeURIComponent(document.getElementById('guidInput').value);
    var searchWord = document.getElementById('searchInput').value;

    // Show loading animation
    showLoading();

    $.ajax({
        type: 'GET',
        url: './clean.php',
        data: { guidData: guidInput },
        success: function(response) {
            if (response.noBlogs > 0) {
                var sentenceLists = response.sentenceLists;

                if (sentenceLists && sentenceLists.length > 0) {
                    var resultList = '<ul>';

                    sentenceLists.forEach(function(sentenceObj) {
                        resultList += '<li>' + sentenceObj + '</li>';
                    });

                    resultList += `
                    <div class="buttons_box">
                        <div class="btnx">
                            <form id="cleanForm" action="./clean.php">
                                <input type="hidden" id="guidInput" name="guidInput">
                                <button type="button" onclick="clean()">تشذيب السياقات</button>
                            </form>
                            <form id="StemForm" action="./Stem.php">
                                <input type="hidden" id="StemguidInput" name="StemguidInput">
                                <button type="button" onclick="Stem()">تجزيع الكلمات</button>
                            </form>
                        </div>
                        <div>
                        <button class="Btn" id="print_btn" onclick="printContent()">
                          <svg class="svgIcon" viewBox="0 0 384 512" height="1em" xmlns="http://www.w3.org/2000/svg"><path d="M169.4 470.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 370.8 224 64c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 306.7L54.6 265.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"></path></svg>
                          <span class="icon2"></span>
                          <span class="tooltip">Download</span>
                        </button>
                        </div>
                    </div>
                    <div class="ex_buttonBox">
                    <table id="tbl_exporttable_to_xls" border="1" style="display: none;">
                        <thead>
                            <th>Word</th>
                            <th>Contexts</th>
                            <th>Group</th>
                        </thead>
                        <tbody>
                        ${sentenceLists.map(function (sentenceObj) {
                            return `
                                <tr>
                                    <td>${searchWord}</td>
                                    <td>${sentenceObj}</td>
                                    <td></td>
                                </tr>`;
                        }).join('')}
                        </tbody>
                    </table>
                    <button onclick="ExportToExcel('xlsx')">Export to excel</button>
                    </div>
                    `;


                    resultList += '</ul>';

                    // Display the results in the searchResults div
                    $('#searchResults').html('<div class="head_tag">'+
                    '<h2>سياقات الكلمة</h2>'+
                    '</div>'+
                    '<p>عدد المدونات التي تم العثور عليها: ' + response.noBlogs + '</p>' + resultList);

                    // Save the GUID to the input field
                    saveGuidToCleanInput(response.guid);
                    saveGuidToStemInput(response.guid);
                } else {
                    // Display a message if no sentences were found
                    $('#searchResults').html('<p>No sentences found for the search term: ' + decodeURIComponent(response.wordsearch) + '</p>');
                }
            } else {
                // Display a message if no blogs were found
                $('#searchResults').html('<p>No blogs found for the search term: ' + decodeURIComponent(response.wordsearch) + '</p>');
            }
        },
        error: function(error) {
            console.log('Error:', error);
        },
        complete: function() {
            // Hide loading animation after the request is complete (success or error)
            hideLoading();
        }
    });
}

function Stem() {
    var StemguidInput = encodeURIComponent(document.getElementById('StemguidInput').value);

    // Show loading animation
    showLoading();

    $.ajax({
        type: 'GET',
        url: './Stem.php',
        data: { stemguidData: StemguidInput },
        success: function(response) {
            if (response.noBlogs > 0) {
                var sentenceLists = response.sentenceLists;

                if (sentenceLists && sentenceLists.length > 0) {
                    var resultList = '<ul>';

                    sentenceLists.forEach(function(sentenceObj) {
                        resultList += '<li>' + sentenceObj + '</li>';
                    });

                    resultList += `
                    <div class="buttons_box">
                    <div class="btnx">
                    <form id="cleanForm" action="./clean.php">
                        <input type="hidden" id="guidInput" name="guidInput">
                        <button type="button" onclick="clean()">تشذيب السياقات</button>
                    </form>
                    <form id="StemForm" action="./Stem.php">
                        <input type="hidden" id="StemguidInput" name="StemguidInput">
                        <button type="button" onclick="Stem()">تجزيع الكلمات</button>
                    </form>
                    </div>
                    <div>
                    <button class="Btn" id="print_btn" onclick="printContent()">
                       <svg class="svgIcon" viewBox="0 0 384 512" height="1em" xmlns="http://www.w3.org/2000/svg"><path d="M169.4 470.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 370.8 224 64c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 306.7L54.6 265.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"></path></svg>
                       <span class="icon2"></span>
                       <span class="tooltip">Download</span>
                    </button>
                    </div>
                    </div>`;

                    resultList += '</ul>';

                    // Display the results in the searchResults div
                    $('#searchResults').html('<div class="head_tag">'+
                    '<h2>سياقات الكلمة</h2>'+
                    '</div>'+
                    '<p>عدد المدونات التي تم العثور عليها: ' + response.noBlogs + '</p>' + resultList);

                    // Save the GUID to the input field
                    saveGuidToCleanInput(response.guid);
                    saveGuidToStemInput(response.guid);
                } else {
                    // Display a message if no sentences were found
                    $('#searchResults').html('<p>No sentences found for the search term: ' + decodeURIComponent(response.wordsearch) + '</p>');
                }
            } else {
                // Display a message if no blogs were found
                $('#searchResults').html('<p>No blogs found for the search term: ' + decodeURIComponent(response.wordsearch) + '</p>');
            }
        },
        error: function(error) {
            console.log('Error:', error);
        },
        complete: function() {
            // Hide loading animation after the request is complete (success or error)
            hideLoading();
        }
    });
}

// Function to show loading animation
function showLoading() {
    $('#loading').show(); // Assuming you have an element with id 'loading' to display the loading animation
}

// Function to hide loading animation
function hideLoading() {
    $('#loading').hide(); // Assuming you have an element with id 'loading' to display the loading animation
}

// Function to save the GUID to the input field
function saveGuidToCleanInput(guid) {
    // Add the GUID to the input field
    $('#guidInput').val(guid);
}
function saveGuidToStemInput(guid) {
    // Add the GUID to the input field
    $('#StemguidInput').val(guid);
}

/* pdf print function */
function printContent() {
    // Open a new window
    let mywindow = window.open("", "PRINT", "height=650,width=900,top=100,left=150");

    // Retrieve the content of the searchResults div
    var searchResultsContent = document.getElementById('searchResults').innerHTML;

    // Define the styles
    var style = "<style>";
    style += "#cleanForm {display: none;}";
    style += "#StemForm {display: none;}";
    style += "#print_btn {display: none;}";
    style += ".ex_buttonBox {display: none;}";
    style += "</style>";

    // Write the HTML content to the new window
    mywindow.document.write("<html><head>" + style + "</head><body>");
    
    // Copy the content of the searchResults div into the new window
    mywindow.document.write(searchResultsContent);

    // Close the HTML document
    mywindow.document.write("</body></html>");

    // Focus on the new window
    mywindow.focus();

    // Print the content
    mywindow.print();

    // Close the new window after printing
    mywindow.close();
}

/* excle print function */
function ExportToExcel(type, fn, dl) {
    var elt = document.getElementById('tbl_exporttable_to_xls');
    var wb = XLSX.utils.table_to_book(elt, { sheet: "sheet1" });
    return dl ?
        XLSX.write(wb, { bookType: type, bookSST: true, type: 'base64' }) :
        XLSX.writeFile(wb, fn || ('MySheetName.' + (type || 'xlsx')));
}







// Function to update resultList based on the current page
/* function updateResultList(sentenceLists, totalPages) {
    var startIndex = (currentPage - 1) * itemsPerPage;
    var endIndex = startIndex + itemsPerPage;

    var resultList = '<ul class="list-group list-group-flush">';

    for (var i = startIndex; i < endIndex && i < sentenceLists.length; i++) {
        resultList += '<li class="list-group-item">' + sentenceLists[i].sentenceHTML + '</li>';
    }

    resultList += '</ul>';

    // Display the results in the searchResults div
    $('#searchResults').html('<div class="card-header">Number of blogs found: ' + sentenceLists.length + '</div>' + resultList);

    // Update the pagination controls
    updatePagination(totalPages);
} */

// Function to update pagination controls
/* function updatePagination(totalPages) {
    // Clear previous pagination controls
    $('#pagination').empty();

    // Add "Previous" button
    if (currentPage > 1) {
        $('#pagination').append('<li class="page-item disabled">' +
            '<a class="page-link" onclick="changePage(' + (currentPage - 1) + ')" tabindex="-1" aria-disabled="true">Previous</a>' +
            '</li>');
    }

    // Add numbered buttons
    for (var i = 1; i <= totalPages; i++) {
        $('#pagination').append('<li class="page-item">' +
            '<a class="page-link" onclick="changePage(' + i + ')">' + i + '</a>' + '</li>'
        );
    }

    // Add "Next" button
    if (currentPage < totalPages) {
        $('#pagination').append('<li class="page-item">' +
            '<a class="page-link" onclick="changePage(' + (currentPage + 1) + ')">Next</a>' +
            '</li>');

    }
} */

// Function to change the current page and update the resultList
/* function changePage(newPage) {
    currentPage = newPage;
    search();
} */


