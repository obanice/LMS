function addDepartment() {
	var defaultBtnValue = $('#submit-btn').html();
	$('#submit-btn').html("Please wait...");
	$('#submit-btn').attr("disabled", true);
	var data = {};
	data.Email = $('#email').val();
	data.Name = $('#departmentName').val();
	data.GenderId = $('#genderId').val();
	data.FirstName = $('#firstName').val();
	data.LastName = $('#lastName').val();
	data.PhoneNumber = $('#phone').val();
	$.ajax({
		type: 'Post',
		url: '/SuperAdmin/Home/Add',
		dataType: 'json',
		data:
		{
			departmentDetails: JSON.stringify(data)
		},
		success: function (result) {
			if (!result.isError) {
				var url = '/SuperAdmin/Home';
				successAlertWithRedirect(result.msg, url);
			}
			else {
				$('#submit-btn').html(defaultBtnValue);
				$('#submit-btn').attr("disabled", false);
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			$('#submit-btn').html(defaultBtnValue);
			$('#submit-btn').attr("disabled", false);
			errorAlert("An error has occurred, try again. Please contact support if the error persists");
		}
	});
}

function departmentIdToDelete(id) {
	$("#deleteId").val(id);
}
function deleteDepartmentById() {
	var id = $("#deleteId").val();
	$.ajax({
		type: 'Post',
		dataType: 'json',
		url: '/SuperAdmin/Home/DeleteDepartment',
		data:
		{
			departmentId: id,
		},
		success: function (result) {
			if (!result.isError) {
				var url = '/SuperAdmin/Home';
				successAlertWithRedirect(result.msg, url)
			}
			else {
				errorAlert(result.msg)
			}
		},

	});

}




function adminIdToDelete(id) {
	debugger
	$('#delete_adminId').val(id);
}
function deleteAdminById() {
	debugger;
	var userId = $("#delete_adminId").val();
	$.ajax({
		type: 'Post',
		dataType: 'json',
		url: '/SuperAdmin/Home/DeleteAdmin',
		data:
		{
			userId: userId,
		},
		success: function (result) {
			debugger;
			if (!result.isError) {
				var url = '/SuperAdmin/Home';
				successAlertWithRedirect(result.msg, url)
			}
			else {
				errorAlert(result.msg)
			}
		},

	});

}


