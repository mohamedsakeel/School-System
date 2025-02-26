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

let index = 1;

// Add new class-subject dropdown set
$("#addClassSubject").click(function () {
    const newPair = `
                <div class="class-subject-pair row mb-3">
                <div class="col-md-5">
                    <label>Class</label>
                    <select class="form-select class-dropdown" name="ClassSubjectSelections[${index}].ClassId">
                        <option value="">Select Class</option>
                        @foreach (var cls in Model.Classes)
                        {
                            <option value="@cls.Id">@cls.Name</option>
                        }
                    </select>
                    </div>
                    <div class="col-md-6">
                    <label>Subjects</label>
                    <select class="select2 form-control select2-multiple subject-dropdown" name="ClassSubjectSelections[${index}].SubjectIds" multiple="multiple">
                        <option value="">Select Subject</option>
                    </select>
                    </div>

                    <div class="col-md-1 d-flex align-items-end">
            <button type="button" class="btn btn-danger remove-pair">X</button>
        </div>
                </div>`;

    $("#classSubjectContainer").append(newPair);
    index++;

    $('.select2').select2({
        dropdownParent: $('#teacherModal')
    });
});

// Remove class-subject dropdown set
$(document).on("click", ".remove-pair", function () {
    $(this).closest(".class-subject-pair").remove();
});

// Load subjects when class is selected
$(document).on("change", ".class-dropdown", function () {
    const classId = $(this).val();
    const subjectDropdown = $(this).closest(".class-subject-pair").find(".subject-dropdown");

    $.ajax({
        url: '/Master/GetSubjectsByClass',
        type: 'GET',
        data: { classId: classId },
        success: function (data) {
            subjectDropdown.empty();
            $.each(data, function (index, subject) {
                subjectDropdown.append(`<option value="${subject.id}">${subject.subjectName}</option>`);
            });
        }
    });
});

