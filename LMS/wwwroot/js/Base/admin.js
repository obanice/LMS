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
				var url = '/Admin/Lecturers';
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
function addCourse() {
	var defaultBtnValue = $('#submit-btn').html();
	$('#submit-btn').html("Please wait...");
	$('#submit-btn').attr("disabled", true);
	var data = {};
	data.Code = $('#code').val();
	data.PhoneNumber = $('#phone').val();
	data.Name = $('#name').val();
	data.Description = $('#description').val();
	data.LecturerId = $('#lecturerId').val();
	data.LevelId = $('#levelId').val();
	data.SemesterId = $('#semesterId').val();
	$.ajax({
		type: 'Post',
		url: '/Admin/AddCourse',
		dataType: 'json',
		data:
		{
			courseDetails: JSON.stringify(data)
		},
		success: function (result) {
			if (!result.isError) {
				var url = '/Admin/Courses';
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
function addFile(courseId) {
	var defaultBtnValue = $('.submit-btn').html();
	$('.submit-btn').html("Please wait...");
	$('.submit-btn').attr("disabled", true);
	var formData = new FormData();

	var file = document.getElementById("fileUrl").files[0];
	formData.append("courseId", courseId);
	formData.append("file", file);

	$.ajax({
		type: 'post',
		url: '/Admin/AddMaterials',
		data: formData,
		contentType: false,
		processData: false,
		success: function (result) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			if (!result.isError) {
				var url = window.location.href;
				successAlertWithRedirect(result.msg, url);
			} else {
				$('.submit-btn').html(defaultBtnValue);
				$('.submit-btn').attr("disabled", false);
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			errorAlert(ex);
		}
	});
}
function addQuiz() {
	var defaultBtnValue = $('.submit-btn').html();
	$('.submit-btn').html("Please wait...");
	$('.submit-btn').attr("disabled", true);
	var formData = new FormData();
	var courseId = $('#courseId').val();
	var file = document.getElementById("fileUrl").files[0];
	formData.append("courseId", courseId);
	formData.append("file", file);

	$.ajax({
		type: 'post',
		url: '/Lecturer/Home/AddQuiz',
		data: formData,
		contentType: false,
		processData: false,
		success: function (result) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			if (!result.isError) {
				var url = window.location.href;
				successAlertWithRedirect(result.msg, url);
			} else {
				$('.submit-btn').html(defaultBtnValue);
				$('.submit-btn').attr("disabled", false);
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			errorAlert(ex);
		}
	});
}
function addAnswers() {
	var defaultBtnValue = $('.submit-btn').html();
	$('.submit-btn').html("Please wait...");
	$('.submit-btn').attr("disabled", true);
	var formData = new FormData();
	var quizId = $('#quizId').val();
	var file = document.getElementById("fileUrl").files[0];
	formData.append("quizId", quizId);
	formData.append("file", file);

	$.ajax({
		type: 'post',
		url: '/Student/Home/UploadAnswer',
		data: formData,
		contentType: false,
		processData: false,
		success: function (result) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			if (!result.isError) {
				var url = window.location.href;
				successAlertWithRedirect(result.msg, url);
			} else {
				$('.submit-btn').html(defaultBtnValue);
				$('.submit-btn').attr("disabled", false);
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			errorAlert(ex);
		}
	});
}
function getQuiz(quizId) {
	$('#quizId').val(quizId);
	$('#add_Answer, #add_score').modal('show');
}
function addmark() {
	var defaultBtnValue = $('.submit-btn').html();
	$('.submit-btn').html("Please wait...");
	$('.submit-btn').attr("disabled", true);
	var quizId = $('#quizId').val();
	var mark = $('#mark').val();
	$.ajax({
		type: 'post',
		dataType:'json',
		url: '/Lecturer/Home/AddScore',
		data: {
			quizId: quizId,
			mark: mark,
		},
		success: function (result) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			if (!result.isError) {
				var url = window.location.href;
				successAlertWithRedirect(result.msg, url);
			} else {
				$('.submit-btn').html(defaultBtnValue);
				$('.submit-btn').attr("disabled", false);
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			$('.submit-btn').html(defaultBtnValue);
			$('.submit-btn').attr("disabled", false);
			errorAlert(ex);
		}
	});
}
function deleteMaterial(materialId) {
	$.ajax({
		type: 'post',
		dataType: 'json',
		url: '/Admin/DeleteMaterial',
		data: {
			materialId: materialId
		},
		success: function (result) {
			if (!result.isError) {
				var url = window.location.href;
				successAlertWithRedirect(result.msg, url);
			} else {
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			errorAlert(ex);
		}
	});
}