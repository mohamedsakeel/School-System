$(document).ready(function () {
    $('#datatable').DataTable();

    $(".dataTables_length select").addClass('form-select form-select-sm');
});

// Initialize DataTable when the modal is shown
$('#uploadModal').on('shown.bs.modal', function () {
    // Initialize DataTable if it hasn't been initialized yet
    if (!$.fn.DataTable.isDataTable('#previewTable')) {
        $('#previewTable').DataTable();
    }
});


document.getElementById("studentForm").addEventListener("submit", function (event) {
    let isValid = true;

    // Validate phone number (10 digits only)
    const phoneInput = document.getElementById("phonenumber");
    const phoneRegex = /^\d{10}$/;
    if (!phoneRegex.test(phoneInput.value)) {
        setError(phoneInput, "Phone Number must be 10 digits");
        isValid = false;
    } else {
        clearError(phoneInput);
    }

    // Validate date of birth (must be at least 2 years old)
    const dobInput = document.getElementById("dob");
    const dob = new Date(dobInput.value);
    const minDate = new Date();
    minDate.setFullYear(minDate.getFullYear() - 2);

    if (dob > minDate) {
        setError(dobInput, "Child must be at least 2 years old");
        isValid = false;
    } else {
        clearError(dobInput);
    }

    if (!isValid) {
        event.preventDefault();
    }
});

// Function to display error messages
function setError(inputElement, message) {
    let errorSpan = inputElement.nextElementSibling;
    if (!errorSpan || !errorSpan.classList.contains("text-danger")) {
        errorSpan = document.createElement("span");
        errorSpan.classList.add("text-danger");
        inputElement.parentNode.appendChild(errorSpan);
    }
    errorSpan.innerText = message;
}

// Function to clear error messages
function clearError(inputElement) {
    let errorSpan = inputElement.nextElementSibling;
    if (errorSpan && errorSpan.classList.contains("text-danger")) {
        errorSpan.innerText = "";
    }
}

document.addEventListener("DOMContentLoaded", function () {
    // Initialize Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// Edit Button Click Event
document.querySelectorAll(".edit-student").forEach(button => {
    button.addEventListener("click", function () {
        document.getElementById("studentId").value = this.dataset.id;
        document.getElementById("firstName").value = this.dataset.firstname;
        document.getElementById("lastName").value = this.dataset.lastname;
        document.getElementById("indexno").value = this.dataset.indexno;
        document.getElementById("phonenumber").value = this.dataset.phone;
        document.getElementById("dob").value = this.dataset.dob;
        document.getElementById("classId").value = this.dataset.classid;

        let genderValue = this.dataset.gender;
        if (genderValue === "Male") {
            document.getElementById("genderMale").checked = true;
        } else if (genderValue === "Female") {
            document.getElementById("genderFemale").checked = true;
        }
    });
});

document.querySelectorAll(".delete-btn").forEach(button => {
    button.addEventListener("click", function () {
        let Id = this.getAttribute("data-id");

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                // Send a POST request for deletion
                fetch('/Master/DeleteStudent', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ studentId: Id })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            Swal.fire("Deleted!", data.message, "success");
                            setTimeout(() => location.reload(), 1500); // Reload after 1.5s
                        } else {
                            Swal.fire("Error!", data.message, "error");
                        }
                    })
                    .catch(error => {
                        console.error("Error deleting student:", error);
                        Swal.fire("Error!", "Something went wrong!", "error");
                    });
            }
        });
    });
});


$(document).ready(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/uploadProgressHub").build();

    connection.on("ReceiveProgress", function (message, percentage) {
        $("#uploadProgress").css("width", percentage + "%").text(message);
    });

    connection.start().catch(err => console.error(err));

    $("#btnUpload").click(function () {
        var fileInput = $("#fileUpload")[0].files[0];
        if (!fileInput) {
            alert("Please select a file.");
            return;
        }

        var formData = new FormData();
        formData.append("file", fileInput);

        // Show loading spinner on button
        $("#btnUpload").prop("disabled", true); // Disable the button to prevent multiple clicks
        $("#btnUpload").html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Importing...'); // Show spinner and change tex

        $.ajax({
            url: "/Master/UploadExcel",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            beforeSend: function () {
                $("#progressContainer").show();
                $("#uploadProgress").css("width", "0%").text("Uploading...");
            },
            success: function (students) {
                var tableBody = $("#previewTable tbody");
                tableBody.empty();

                // Clear existing rows
                var table = $('#previewTable').DataTable();
                table.clear();

                students.forEach(function (student) {
                    var row = [
                        student.indexNo,
                        student.firstName + " " + student.lastName,
                        student.gender,
                        student.classId,
                        student.phoneNumber,
                        student.dateOfBirth
                    ];

                    // Add the new row
                    table.row.add(row);
                });

                table.draw();

                $("#previewSection").show();
            },
            error: function () {
                alert("File upload failed.");
            },
            complete: function () {
                // Hide spinner and reset button after the upload is complete
                $("#btnUpload").prop("disabled", false); // Enable the button
                $("#btnUpload").html('Upload Students'); // Reset button text
            }
        });
    });

    $("#btnConfirmUpload").click(async function () {
        var students = [];

        var table = $('#previewTable').DataTable();
        var data = table.rows().data(); // Get all data from the table

        data.each(function (student) {
            var studentData = {
                indexNo: student[0], // Assuming the first column is indexNo
                firstName: student[1].split(" ")[0],
                lastName: student[1].split(" ")[1] || "",
                gender: student[2],
                classId: student[3], // Assuming classId is in the 4th column
                phoneNumber: student[4],
                dateOfBirth: new Date(student[5]).toISOString(), // Ensure proper format
                enteredDate: new Date().toISOString(), // Current date and time
                enteredBy: "Admin", // Set enteredBy to "Admin" (could be dynamic)
                status: true // Assuming you want the status to be true by default
            };
            students.push(studentData);
        });

        try {
            // Make the AJAX call asynchronously using fetch
            const response = await fetch("/Master/ConfirmUpload", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(students)
            });

            // Check if the response is OK (status 200-299)
            if (response.ok) {
                // SweetAlert2 success message
                Swal.fire({
                    title: "Success!",
                    text: "Students uploaded successfully.",
                    icon: "success",
                    confirmButtonText: "OK"
                }).then(() => {
                    location.reload(); // Reload the page after the alert
                });
            } else {
                throw new Error("Error saving students.");
            }
        } catch (error) {
            // SweetAlert2 error message
            Swal.fire({
                title: "Error!",
                text: error.message,
                icon: "error",
                confirmButtonText: "Try Again"
            });
        }
    });
});

$(document).ready(function () {
    var table = $('#datatable').DataTable();

    // Populate the class filter dropdown with unique class names
    var uniqueClasses = [];

    // Get all data from all pages (not just the visible ones)
    var data = table.rows().data();

    // Loop through the rows to get unique class names
    data.each(function (rowData) {
        var className = rowData[4]; // Get class name from 5th column (index 4)

        // Check if the class is already in the uniqueClasses array
        if (uniqueClasses.indexOf(className) === -1) {
            uniqueClasses.push(className);
        }
    });

    // Populate the dropdown with options
    uniqueClasses.forEach(function (className) {
        $('#classFilter').append(new Option(className, className));
    });

    // Add event listener to filter the table
    $('#classFilter').on('change', function () {
        var selectedClass = $(this).val();

        // Filter the table by the selected class
        table.rows().every(function () {
            var row = this.node();
            var rowClass = $(row).find('td').eq(4).text().trim(); // Get class name from 5th column

            // Show or hide the row based on the selected class filter
            if (selectedClass === "" || rowClass === selectedClass) {
                $(row).show(); // Show the row
            } else {
                $(row).hide(); // Hide the row
            }
        });

        // Redraw the table to apply the filtering effect
        table.draw();
    });
});



