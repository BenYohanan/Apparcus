function login() {
	var $submitBtn = $("#submit_btn");
	$submitBtn.prop("disabled", true).text("Please wait...");

	var email = $('#email').val().trim();
	var password = $('#password').val();

	if (!email) {
		errorAlert("Email is required.");
		$submitBtn.prop("disabled", false).text("Login");
		return;
	}

	if (!password) {
		errorAlert("Password is required.");
		$submitBtn.prop("disabled", false).text("Login");
		return;
	}

	$.ajax({
		type: 'POST',
		url: '/Account/Login',
		dataType: 'json',
		data: {
			email: email,
			password: password
		},
		success: function (result) {
			$submitBtn.prop("disabled", false).text("Login");

			if (!result.isError) {
				location.href = result.returnUrl;
			} else {
				if (result.data != null) {
					newSuccessAlert(result.msg, result.url);
				} else {
					errorAlert(result.msg || "Login failed. Please try again.");
				}
			}
		},
		error: function (ex) {
			$submitBtn.prop("disabled", false).text("Login");
			errorAlert("An error has occurred. Please try again or contact support if the issue persists.");
		}
	});
}

function registerUser() {
	var $submitBtn = $("#submit_btn");
	$submitBtn.prop("disabled", true).text("Please wait...");

	var email = $('#email').val().trim();
	var firstName = $('#firstName').val().trim();
	var password = $('#password').val();
	var confirmPassword = $('#confirmPassword').val();
	var phone = $('#phone').val().trim();
	var lastName = $('#lastName').val().trim();
	var address = $('#address').val().trim();
	var dateOfBirth = $('#dateOfBirth').val().trim();

	if (!email) {
		errorAlert("Email is required.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (!firstName) {
		errorAlert("First Name is required.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (!password) {
		errorAlert("Password is required.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (!confirmPassword) {
		errorAlert("Please confirm your password.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (password !== confirmPassword) {
		errorAlert("Passwords do not match.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}

	var data = {
		Email: email,
		PhoneNumber: phone,
		FirstName: firstName,
		LastName: lastName,
		Address: address,
		Password: password,
		DateOfBirth: dateOfBirth,
		ConfirmPassowrd: confirmPassword  
	};

	$.ajax({
		type: 'POST',
		url: '/Account/Register',
		dataType: 'json',
		data: {
			userData: JSON.stringify(data)
		},
		success: function (result) {
			if (!result.isError) {
				var url = '/Account/Login';
				newSuccessAlert(result.msg, url);
			} else {
				errorAlert(result.msg);
				$submitBtn.prop("disabled", false).text("Register");
			}
		},
		error: function (ex) {
			errorAlert("An error has occurred, try again. Please contact support if the error persists.");
			$submitBtn.prop("disabled", false).text("Register");
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

function openChangePasswordModal(userId) {
	$('#userIdForPassword').val(userId);
	$('#change_password_modal').modal('show');
}

function openMakeAdminModal(userId) {
	$('#userIdToMakeId').val(userId);
	$('#makeAdmin_modal').modal('show');
}

function changePassword() {
	var userId = $('#userIdForPassword').val();
	var newPassword = $('#newPassword').val();
	if (!newPassword) {
		infoAlert('Please enter a new password.');
		return;
	}
	var defaultBtnValue = $('#change_pass_btn').html();
	$('#change_pass_btn').html("Please wait...").attr("disabled", true);

	$.ajax({
		type: 'POST',
		url: '/Admin/ChangePassword',
		dataType: 'json',
		data: { userId: userId, newPassword: newPassword },
		success: function (result) {
			if (!result.isError) {
				newSuccessAlert(result.msg, location.href);
			} else {
				errorAlert(result.msg);
			}
			$('#change_pass_btn').html(defaultBtnValue).attr("disabled", false);
		},
		error: function () {
			errorAlert("An error has occurred, please try again.");
			$('#change_pass_btn').html(defaultBtnValue).attr("disabled", false);
		}
	});
}

function makeUserAdmin() {
	const userId = $('#userIdToMakeId').val();
	if (!userId) {
		errorAlert("Invalid user ID");
		return;
	}

	$.ajax({
		url: '/Admin/MakeUserAdmin',
		type: 'POST',
		data: { userId: userId },
		success: function (response) {
			if (!response.isError) {
				var url = location.href;
				newSuccessAlert(response.msg, url);
			} else {
				errorAlert(response.msg);
			}
		},
		error: function () {
			errorAlert("An error occurred while making user admin.");
		}
	});
}

function openDeleteUserModal(userId) {
	$('#userIdToDelete').val(userId);
	$('#delete_modal').modal('show');
}
function deleteUsers() {
	$.ajax({
		type: 'Post',
		dataType: 'json',
		url: '/Admin/Delete',
		data:
		{
			userId: $('#userIdToDelete').val(),
		},
		success: function (result) {
			if (!result.isError) {
				var url = window.location.href;
				newSuccessAlert(result.msg, url)
			}
			else {
				errorAlert(result.msg)
			}
		},
		error: function (ex) {
			errorAlert("An error has occurred, try again. Please contact support if the error persists");
		}
	});
}
$(function () {
	$(".select2").select2();
});

function addProjet() {
	var $submitBtn = $("#submit_btn");
	$submitBtn.prop("disabled", true).text("Please wait...");

	var data = {};
	data.Title = $('#projectName').val();
	data.Description = $('description').val();
	data.AmountNeeded = $('amount').val();
	data.CreatedById = $('userId').val();

	if (!email) {
		errorAlert("Email is required.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (!firstName) {
		errorAlert("First Name is required.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (!password) {
		errorAlert("Password is required.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (!confirmPassword) {
		errorAlert("Please confirm your password.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}
	if (password !== confirmPassword) {
		errorAlert("Passwords do not match.");
		$submitBtn.prop("disabled", false).text("Register");
		return;
	}

	var data = {
		Email: email,
		PhoneNumber: phone,
		FirstName: firstName,
		LastName: lastName,
		Address: address,
		Password: password,
		DateOfBirth: dateOfBirth,
		ConfirmPassowrd: confirmPassword
	};

	$.ajax({
		type: 'POST',
		url: '/Account/Register',
		dataType: 'json',
		data: {
			userData: JSON.stringify(data)
		},
		success: function (result) {
			if (!result.isError) {
				var url = '/Account/Login';
				newSuccessAlert(result.msg, url);
			} else {
				errorAlert(result.msg);
				$submitBtn.prop("disabled", false).text("Register");
			}
		},
		error: function (ex) {
			errorAlert("An error has occurred, try again. Please contact support if the error persists.");
			$submitBtn.prop("disabled", false).text("Register");
		}
	});
}