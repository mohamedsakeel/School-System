$(document).ready(function () {

    // Edit button click event
    $(document).on('click', '#btnEdit', function () {
        // Get user data from data attributes
        var classId = $(this).data('subject-id');
        var className = $(this).data('subject-name');
        var isElecive = $(this).data('iselective');
        var classStatus = $(this).data('subject-status');

        // Set the data into the modal fields
        $('#subjectId').val(classId);
        $('#subjectName').val(className);
        if (isElecive === 'True') {
            $('#iselective').prop('checked', isElecive);
        }
        
        $('#statuselect').val(classStatus.toString());

        // Open the modal
        $('#subjectModal').modal('show');
    });
});

document.getElementById('subjectModal').addEventListener('hidden.bs.modal', function (event) {
    // Get the form inside the modal
    var form = document.getElementById('classForm');

    // Reset the form fields
    form.reset();
});