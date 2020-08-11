using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// This screen is used to handle all user actions pertaining to the forgot password screen. It uses the Base screen superclass
/// to automatically declare various variables and methods that each screen shoul have.
/// </summary>
public class AccountForgotPasswordScreen : AccountBaseScreen
{

	public InputField emailInput;
	
	[Tooltip("The area where error/notice messages are showed to the user")]
	public Text messageText;

	/// <summary>
	/// Closes the current screen and navigates to the Login screen.
	/// </summary>
	public void CloseScreen() {
		this.gameController.OpenLoginScreen();
	}

	/// <summary>
	/// Send the request to the backend. This will in turn send a password recovery email to the user.
	/// </summary>
	public void Submit() {
		string email = emailInput.text ?? "";

		if (email.Equals("")) {
			messageText.text = "Please fill out all fields";
			Debug.Log("Missing fields");
			return;
		}
		
		SimpleRequestCallback callback = delegate(string response, UnityWebRequest obj, bool error, bool loginNeeded) {

			if (error) {
				messageText.text = response;
			}
			else {
				messageText.text = "A recovery email has been sent to " + email;
				emailInput.text = "";
			}
		};

		StartCoroutine(authManager.ForgotPassword(email, callback));
	}

}
