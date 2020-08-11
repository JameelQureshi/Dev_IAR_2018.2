using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// This screen is used to handle all user actions pertaining to the registration screen. It uses the Base screen superclass
/// to automatically declare various variables and methods that each screen shoul have.
/// </summary>
public class AccountRegisterScreen : AccountBaseScreen
{

	[Tooltip("Text area to show any registration messages to the user")]
	public Text messageArea;

	public InputField emailInput;
	public InputField passwordInput;
	public InputField confirmPasswordInput;
	
	/// <summary>
	/// Navigates back to the login screen when the exit button is tapped
	/// </summary>
	public void CloseScreen() {
		this.gameController.OpenLoginScreen();
	}

	/// <summary>
	/// Registers a user with the given username and password.
	/// </summary>
	/// <remarks>
	/// Please note that email and password validation is occuring on the server/backend. Any errors or
	/// password strength messages will be returned within the response.
	/// </remarks>
	public void Register() {
		string email = emailInput.text ?? "";
		string password = passwordInput.text ?? "";
		string confirmPassword = confirmPasswordInput.text ?? "";

		if (email.Equals("")
		    || password.Equals("")
		    || confirmPassword.Equals("")) {

			messageArea.text = "Please fill out all input areas";
			return;
		}
		
		SimpleRequestCallback callback = delegate(string message, UnityWebRequest obj, bool error, bool loginNeeded) {

			messageArea.text = message;
			
			if (error) {
				Debug.Log("Error registering user.");
				return;
			}

			emailInput.text = "";
			passwordInput.text = "";
			confirmPasswordInput.text = "";
		};

		StartCoroutine(authManager.Register(email, password, confirmPassword, callback));
	}
}
