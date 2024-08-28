
function registerUser() {
	var defaultBtnValue = $('#submit_btn').html();
	$('#submit_btn').html("Please wait...");
	$('#submit_btn').attr("disabled", true);
	var data = {};
	data.Email = $('#email').val();
	data.PassWord = $('#password').val();
	data.ConfirmPassword = $('#confirmPassword').val();
	data.FirstName = $('#firstName').val();
	data.LastName = $('#lastName').val();
	$.ajax({
		type: 'Post',
		url: '/Security/Account/Register',
		dataType: 'json',
		data:
		{
			userDetails: JSON.stringify(data)
		},
		success: function (result) {
			if (!result.isError) {
				var url = '/Security/Account/Login';
				successAlertWithRedirect(result.msg, url);
			}
			else {
				$('#submit_btn').html(defaultBtnValue);
				$('#submit_btn').attr("disabled", false);
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			$('#submit_btn').html(defaultBtnValue);
			$('#submit_btn').attr("disabled", false);
			errorAlert("An error has occurred, try again. Please contact support if the error persists");
		}
	});
}
function login() {
	var defaultBtnValue = $('#submit_btn').html();
	$('#submit_btn').html("Please wait...");
	$('#submit_btn').attr("disabled", true);
	var email = $('#email').val();
	var password = $('#password').val();
	$.ajax({
		type: 'Post',
		url: '/Security/Account/Login',
		dataType: 'json',
		data:
		{
			emailorphone: email,
			password: password
		},
		success: function (result) {
			if (!result.isError) {
				location.href = result.dashboard;
			}
			else {
				if (result.data != null) {
					$('#submit_btn').html(defaultBtnValue);
					$('#submit_btn').attr("disabled", false);
					newInfoAlert(result.msg, result.url);
				} else {
					$('#submit_btn').html(defaultBtnValue);
					$('#submit_btn').attr("disabled", false);
					errorAlert(result.msg);
				}
			}
		},
		error: function (ex) {
			$('#submit_btn').html(defaultBtnValue);
			$('#submit_btn').attr("disabled", false);
			errorAlert("An error has occurred, try again. Please contact support if the error persists");
		}
	});
}
function PasswordShow() {
	$("#passwordHide").removeClass("d-none");
	$("#passwordShow").addClass("d-none");
	$("#password").attr("type", "text");
}
function PasswordHide() {
	$("#passwordShow").removeClass("d-none");
	$("#passwordHide").addClass("d-none");
	$("#password").attr("type", "password");
}