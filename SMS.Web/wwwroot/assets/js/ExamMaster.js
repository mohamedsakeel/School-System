$(document).ready(function () {

    // Edit button click event
    $(document).on('click', '#btnEdit', function () {
        // Get user data from data attributes
        var examId = $(this).data('exam-id');
        var examName = $(this).data('exam-name');
        var examStatus = $(this).data('exam-status');
        var examyear = $(this).data('exam-year');

        // Set the data into the modal fields
        $('#examId').val(examId);
        $('#examName').val(examName);
        $('#examyear').val(examyear);
        

        $('#statuselect').val(examStatus.toString());

        // Open the modal
        $('#examModal').modal('show');
    });
});

document.getElementById('examModal').addEventListener('hidden.bs.modal', function (event) {
    // Get the form inside the modal
    var form = document.getElementById('examForm');

    // Reset the form fields
    form.reset();
});