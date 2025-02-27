
// Function to generate student report
// Function to generate individual student report
function generateStudentReport(studentId, classId) {
    var url = `/operations/student/${studentId}/class/${classId}`; // Controller method path

    // Create a link to download the PDF
    var link = document.createElement("a");
    link.href = url;
    link.download = `${studentId}_Report.pdf`; // Optional: Customize file name
    link.click(); // Trigger the download
}

// Function to generate class master report (only once for the entire class)
function generateClassMasterReport(classId) {
    var url = `/operations/class/${classId}`; // Controller method path

    // Create a link to download the PDF
    var link = document.createElement("a");
    link.href = url;
    link.download = "Class_Master_Report.pdf"; // Optional: Customize file name
    link.click(); // Trigger the download
}