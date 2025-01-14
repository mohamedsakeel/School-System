$(document).ready(function () {
    $('#teacherModal').on('shown.bs.modal', function () {
        $('.select2').select2({
            dropdownParent: $('#teacherModal')
        });
    });
});

$(document).ready(function () {

    // Edit button click event
    $(document).on('click', '#btnEdit', function () {
        // Get user data from data attributes
        var teacherId = $(this).data('teacher-id');
        var classNames = (this).getAttribute('data-class-names').split(',');
        var subjectNames = (this).getAttribute('data-subject-names').split(',');

        const subjectselect = $('#SelectedSubjects');
        const classselect = $('#SelectedClasses');

        const subjectMap = {};
        subjectselect.find('option').each(function () {
            const option = $(this);
            const subjectName = option.text().trim();
            const subjectId = option.val();
            subjectMap[subjectName] = subjectId;
        });

        const selectedsubjectIds = subjectNames.map(subjectName => subjectMap[subjectName]);

        subjectselect.val(null).trigger('change');

        subjectselect.val(selectedsubjectIds).trigger('change.select2');

        const classMap = {};
        classselect.find('option').each(function () {
            const option = $(this);
            const className = option.text().trim();
            const classId = option.val();
            classMap[className] = classId;
        });

        const selectedclassIds = classNames.map(className => classMap[className]);

        classselect.val(null).trigger('change');

        classselect.val(selectedclassIds).trigger('change.select2');

        $('#teacherid').val(teacherId).trigger('change');

        // Open the modal
        $('#teacherModal').modal('show');
    });
});

document.getElementById('teacherModal').addEventListener('hidden.bs.modal', function (event) {
    // Get the form inside the modal
    var form = document.getElementById('classForm');

    // Reset the form fields
    form.reset();
});

