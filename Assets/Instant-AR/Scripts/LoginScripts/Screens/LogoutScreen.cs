using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// This screen is used to handle all user actions pertaining to the logout screen. It uses the Base screen superclass
/// to automatically declare various variables and methods that each screen shoul have.
/// </summary>
/// <remarks>
/// In reality this should really be called the Account Screen. But let's leave it as Logout for clarity
/// within the sample project.
/// </remarks>
/// 
public class LogoutScreen : BaseScreen {

	[Tooltip("Used to show the currently logged in user")]
	public Text usernameText;
	public Button logouButton;
	private string logouButtonText;
	private string userText;

	private CommonBarScript m_CommonBarScript;

	public override void Awake() {
		base.Awake();
		authManager = AuthManager.Instance;
	}

	void Start() {
		if (logouButton != null)
        {
			logouButtonText = logouButton.GetComponentInChildren<Text>().text;
		}
		if (usernameText != null)
		{
			userText = usernameText.text;
		}
		Setup();
		m_CommonBarScript = FindObjectOfType<CommonBarScript>();
	}

	/// <summary>
	/// Sets the screen up with any further configurations.
	/// </summary>
	public void Setup() {
		// Set the username field.
		if (authManager.IsLoggedIn) {
			usernameText.text = authManager.CurrentToken.username;
			if (!string.IsNullOrEmpty(logouButtonText))
			{
				logouButton.GetComponentInChildren<Text>().text = logouButtonText;
			}
		}
        else if(logouButton !=null)
        {
			logouButton.GetComponentInChildren<Text>().text = "Login";
			if (!string.IsNullOrEmpty(userText))
			{
				usernameText.text = userText;
			}
		}
	}

	/// <summary>
	/// Closes the logout screen and shows the login screen.
	/// </summary>
	public void CloseScreen() {
		//TODO: When hooked into a real game this should just close the screen
		Debug.Log("Tapped exit button. This is just a sample project so this doesn't do anything while a user is logged in");
		this.gameController.OpenCloudScreen();
	}

	/// <summary>
	/// Logs the user out and then navigates back to the Login screen.
	/// </summary>
	/// <remarks>
	/// This will always log the user out even if the network request fails. This is because the user
	/// should always be allowed to logout and the token expires itself anyway.
	/// </remarks>
	public void Logout() {
		if (authManager.IsLoggedIn)
		{
			SimpleRequestCallback callback = delegate (string response, UnityWebRequest obj, bool error, bool loginNeeded)
			{

				if (error)
				{
					//NOTE: Since tokens expire it is safe to disregard errors in this case.
					Debug.Log("LogoutScreen: Error logging out token. Continuing anyway.");
				}
				else
				{
					Debug.Log("LogoutScreen: Successfully logged out.");
				}
				m_CommonBarScript.cloudScanButton.transform.localScale = Vector3.zero;
				m_CommonBarScript.menuButton.transform.localScale = Vector3.zero;
				this.gameController.OpenLoginScreen();
			};

			StartCoroutine(authManager.Logout(callback));
		}
        else
        {
			this.gameController.OpenLoginScreen();
        }
	}

	public void Logout_ORG()
	{
		SimpleRequestCallback callback = delegate (string response, UnityWebRequest obj, bool error, bool loginNeeded) {

			if (error)
			{
				//NOTE: Since tokens expire it is safe to disregard errors in this case.
				Debug.Log("LogoutScreen: Error logging out token. Continuing anyway.");
			}
			else
			{
				Debug.Log("LogoutScreen: Successfully logged out.");
			}
			m_CommonBarScript.cloudScanButton.transform.localScale = Vector3.zero;
			m_CommonBarScript.menuButton.transform.localScale = Vector3.zero;
			this.gameController.OpenLoginScreen();
		};

		StartCoroutine(authManager.Logout(callback));
	}

}
