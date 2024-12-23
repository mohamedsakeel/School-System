$(document).ready(function () {
    $('#classModal').on('shown.bs.modal', function () {
        $('.select2').select2({
            dropdownParent: $('#classModal')
        });
    });
});

$(document).ready(function () {

    // Edit button click event
    $(document).on('click', '#btnEdit', function () {
        // Get user data from data attributes
        var classId = $(this).data('class-id');
        var className = $(this).data('class-name');
        var classStatus = $(this).data('class-status');
        const subjectid = this.getAttribute('data-subjectid').split(',');

        const subjectselect = $('#selectedSubjects');

        const subjectMap = {};
        subjectselect.find('option').each(function () {
            const option = $(this);
            const subjectName = option.text().trim();
            const subjectId = option.val();
            subjectMap[subjectName] = subjectId;
        });

        const selectedRoleIds = subjectid.map(subjectName => subjectMap[subjectName]);

        subjectselect.val(null).trigger('change');

        subjectselect.val(selectedRoleIds).trigger('change.select2');

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