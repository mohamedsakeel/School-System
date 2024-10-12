$(document).ready(function () {

    // Edit button click event
    $(document).on('click', '#btnEdit', function () {
        // Get user data from data attributes
        var classId = $(this).data('class-id');
        var className = $(this).data('class-name');
        var classStatus = $(this).data('class-status');

        // Set the data into the modal fields
        $('#classId').val(classId);
        $('#className').val(className);
        $('#statuselect').val(classStatus.toString());

        // Open the modal
        $('#classModal').modal('show');
    });
});

document.getElementById('classModal').addEventListener('hidden.bs.modal', function (event) {
    // Get the form inside the modal
    var form = document.getElementById('classForm');

    // Reset the form fields
    form.reset();
});