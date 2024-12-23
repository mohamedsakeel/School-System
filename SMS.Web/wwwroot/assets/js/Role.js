$(document).ready(function () {
    // Modal handling
    $('#roleModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var action = button.data('action'); // Extract info from data-* attributes
        var roleId = button.data('role-id');
        var roleName = button.data('role-name');
        var modal = $(this);

        var districtId = $(this).data('action');
        var districtName = $(this).data('role-id');
        var districtStatus = $(this).data('role-name');

        if (action === 'edit') {
            modal.find('.modal-title').text('Edit Role');
            modal.find('#roleId').val(roleId);
            modal.find('#roleName').val(roleName);
            //modal.find('form').attr('action', 'AddOrEditRole');
        } else {
            modal.find('.modal-title').text('Add New Role');
            modal.find('#roleId').val(''); // Clear roleId for new role
            modal.find('#roleName').val('');
            //modal.find('form').attr('action', 'AddOrEditRole');
        }
    });
});