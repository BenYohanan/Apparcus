function login() {
	$("#submit_btn").prop("disabled", true).text("Please wait...");
	var email = $('#email').val();
	var password = $('#password').val();
	$.ajax({
		type: 'Post',
		url: '/Account/Login',
		dataType: 'json',
		data:
		{
			emailorphone: email,
			password: password
		},
		success: function (result) {
			$("#submit_btn").prop("disabled", false).text("Login");
			if (!result.isError) {
				location.href = result.dashboard;
			}
			else {
				if (result.data != null) {
					newSuccessAlert(result.msg, result.url);
				} else {
					errorAlert(result.msg);
				}

			}
		},
		error: function (ex) {
			errorAlert("An error has occurred, try again. Please contact support if the error persists");
		}
	});
}
function registerUser() {
	var data = {};
	$("#submit_btn").prop("disabled", true).text("Please wait...");
	data.Email = $('#email').val();
	data.PhoneNumber = $('#phone').val();
	data.FirstName = $('#firstName').val();
	data.LastName = $('#lastName').val();
	data.Address = $('#address').val();
	data.Password = $('#password').val();
	data.DateOfBirth = $('#dateOfBirth').val();
	data.ConfirmPassowrd = $('#confirmPassword').val();
	$.ajax({
		type: 'Post',
		url: '/Account/Register',
		dataType: 'json',
		data:
		{
			userData: JSON.stringify(data)
		},
		success: function (result) {
			$("#submit_btn").prop("disabled", true).text("Register");
			if (!result.isError) {
				var url = location.href;
				newSuccessAlert(result.msg, url);
			}
			else {
				errorAlert(result.msg);
			}
		},
		error: function (ex) {
			errorAlert("An error has occurred, try again. Please contact support if the error persists");
		}
	});
}