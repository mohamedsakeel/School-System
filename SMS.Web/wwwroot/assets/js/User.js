

function openEditUserModal(userId, firstName, lastName, email, phoneNumber, RoleName) {


    document.getElementById('userId').value = userId;
    document.getElementById('firstName').value = firstName;
    document.getElementById('lastName').value = lastName;
    document.getElementById('email').value = email;
    document.getElementById('phoneNumber').value = phoneNumber;
    document.getElementById('role').value = RoleName;



    $('#userModal').modal('show');
}
