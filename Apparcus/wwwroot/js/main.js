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
function openRemoveAdminModal(id) {
	$('#userIdToRemoveId').val(id);
	$('#removeAdmin_modal').modal('show');
}
function removeUserAdmin() {
	const userId = $('#userIdToRemoveId').val();
	if (!userId) {
		errorAlert("Invalid user ID");
		return;
	}

	$.ajax({
		url: '/Admin/RemoveUserAdmin',
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
			errorAlert("An error occurred while removing admin role.");
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

function saveProject() {
	var $submitBtn = $("#submit_btn");
	var originalText = $submitBtn.text();
	$submitBtn.prop("disabled", true).text("Please wait...");
	function restoreButton() {
		$submitBtn.prop("disabled", false).text(originalText);
	}
	var data = {};
	data.Name = $('#name').val();
	data.Description = $('#description').val();
	data.AmountNeeded = parseFloat($('#amountNeeded').val());
	if (!data.Name) {
		errorAlert("Name  is required.");
		restoreButton();
		return;
	}
	if (!data.AmountNeeded) {
		errorAlert("Amount  is required.");
		restoreButton();
		return;
	}

	$.ajax({
		type: 'POST',
		url: '/Project/Create',
		dataType: 'json',
		data:
		{
			projectDetails: JSON.stringify(data)
		},
		success: function (result) {
			if (!result.isError) {
				var url = window.location.href;
				successAlertWithRedirect(result.msg, url);
			} else {
				errorAlert(result.msg);
				restoreButton();
			}
		},
		error: function (ex) {
			errorAlert("An error has occurred, try again. Please contact support if the error persists.");
			restoreButton();
		}
	});
}
function getProjectById(id) {
	$.ajax({
		type: "GET",
		url: "/Project/GetProjectById?id=" + id,
		dataType: "json",
		success: function (project) {
			if (!project) {
				errorAlert("Project not found.");
				return;
			}

			$('#edit_projectId').val(project.id);
			$('#edit_name').val(project.name);          
			$('#edit_description').summernote('code', project.description);
			$('#edit_amountNeeded').val(project.amountNeeded);

			$('#edit_project').modal('show');
		},
		error: function () {
			errorAlert("Failed to load project.");
		}
	});
}


function updateProject() {
	var $submitBtn = $("#edit_submit_btn");
	var originalText = $submitBtn.text();
	$submitBtn.prop("disabled", true).text("Please wait...");

	function restoreButton() {
		$submitBtn.prop("disabled", false).text(originalText);
	}

	var data = {};
	data.Id = parseInt($('#edit_projectId').val());        
	data.Name = $('#edit_name').val();
	data.Description = $('#edit_description').val();
	data.AmountNeeded = parseFloat($('#edit_amountNeeded').val()); 

	if (!data.Name) {
		errorAlert("Name is required.");
		restoreButton();
		return;
	}

	if (!data.AmountNeeded) {
		errorAlert("Amount Needed is required.");
		restoreButton();
		return;
	}

	$.ajax({
		type: "POST",
		url: "/Project/Edit",
		dataType: "json",
		data: {
			projectDetails: JSON.stringify(data)
		},
		success: function (result) {
			if (result.success) {
				var url = window.location.href;
				successAlertWithRedirect(result.message, url);
			} else {
				errorAlert(result.message);
				restoreButton();
			}
		},
		error: function () {
			errorAlert("An error occurred. Please try again.");
			restoreButton();
		}
	});
}

function confirmDeleteProject(id) {
	$('#delete_projectId').val(id);
	$('#delete_project_modal').modal('show');
}

function deleteProject() {
	var $submitBtn = $("#delete_submit_btn");
	var originalText = $submitBtn.text();
	$submitBtn.prop("disabled", true).text("Deleting...");

	function restoreButton() {
		$submitBtn.prop("disabled", false).text(originalText);
	}

	var projectId = parseInt($('#delete_projectId').val());

	if (!projectId) {
		errorAlert("Invalid project ID.");
		restoreButton();
		return;
	}

	$.ajax({
		type: "POST",
		url: "/Project/Delete",
		dataType: "json",
		data: { id: projectId },
		success: function (result) {
			if (result.success) {
				var url = window.location.href;
				$('#delete_project_modal').modal('hide');
				successAlertWithRedirect(result.message, url);
			} else {
				errorAlert(result.message);
				restoreButton();
			}
		},
		error: function () {
			errorAlert("An error occurred. Please try again.");
			restoreButton();
		}
	});
}
