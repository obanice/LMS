$(function () {
	$(".makeFilter").select2();
});
function addLecturer() {
	var defaultBtnValue = $('#submit-btn').html();
	$('#submit-btn').html("Please wait...");
	$('#submit-btn').attr("disabled", true);
	var data = {};
	data.Email = $('#email').val();
	data.PhoneNumber = $('#phone').val();
	data.GenderId = $('#genderId').val();
	data.FirstName = $('#firstName').val();
	data.LastName = $('#lastName').val();
	$.ajax({
		type: 'Post',
		url: '/Admin/AddLecturer',
		dataType: 'json',
		data:
		{
			lecturerDetails: JSON.stringify(data)
		},
		success: function (result) {
			if (!result.isError) {
				var url = '/Adimin/Lecturers';
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