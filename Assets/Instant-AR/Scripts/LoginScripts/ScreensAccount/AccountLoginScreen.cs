using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// This screen is used to handle all user actions pertaining to the login screen. It uses the Base screen superclass
/// to automatically declare various variables and methods that each screen shoul have.
/// </summary>
public class AccountLoginScreen : AccountBaseScreen
{

	public InputField emailInput;
	public InputField passwordInput;

	public Text messageArea;
	
	public override void Awake() {
		base.Awake();
		authManager = AuthManager.Instance;
	}

	public void Login() {
		string email = emailInput.text ?? "";
		string password = passwordInput.text ?? "";

		if (email.Equals("") || password.Equals("")) {
			Debug.Log("Missing fields");
			return;
		}

		SimpleRequestCallback callback = delegate(string response, UnityWebRequest obj, bool error, bool loginNeeded) {

			if (!error) {
				Debug.Log(response);
				emailInput.text = "";
				passwordInput.text = "";
				messageArea.text = "";
				this.gameController.OpenLogoutScreen();
			}
			else {
				Debug.Log(response);
				messageArea.text = response;
			}
		};

		StartCoroutine(authManager.Login(email, password, callback));
	}

	/// <summary>
	/// Opens the forgot password screen using the base class's game controller object
	/// </summary>
	public void OpenForgotPasswordScreen() {
		this.gameController.OpenForgotPasswordScreen();
	}

	/// <summary>
	/// Opens the registration screen using the base class's game controller object
	/// </summary>
	public void OpenRegistrationScreen() {
		this.gameController.OpenRegistrationScreen();
	}



}
