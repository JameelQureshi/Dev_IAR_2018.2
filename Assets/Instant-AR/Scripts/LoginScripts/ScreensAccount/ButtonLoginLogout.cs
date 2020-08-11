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
public class ButtonLoginLogout : AccountBaseScreen
{

	[Tooltip("Used to show the currently logged in user")]
	public Text userLabel;
    public Text usernameText;

    public override void Awake() {
		//base.Awake();
		authManager = AuthManager.Instance;
	}

	void Start() {
		Setup();
	}

	/// <summary>
	/// Sets the screen up with any further configurations.
	/// </summary>
	public void Setup() {
        // Set the username field.

        GameObject go = this.gameObject;
        Button button = go.GetComponent<Button>();
        Text[] text = button.GetComponentsInChildren<Text>();
        //image[0].transform.localScale = new Vector3(1, 1, 1);




        if (authManager.IsLoggedIn) {
			usernameText.text = authManager.CurrentToken.username;
            text[0].text = "Logout";
        }
        else{
            userLabel.text = "";
            usernameText.text = "";
            text[0].text = "Login";
            button.onClick.AddListener(LoginScreen);
        }
	}

	/// <summary>
	/// Closes the logout screen and shows the login screen.
	/// </summary>
	public void CloseScreen() {
		//TODO: When hooked into a real game this should just close the screen
		//Debug.Log("Tapped exit button. This is just a sample project so this doesn't do anything while a user is logged in");
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("3-CloudReco");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
    }

    public void LoginScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    /// <summary>
    /// Logs the user out and then navigates back to the Login screen.
    /// </summary>
    /// <remarks>
    /// This will always log the user out even if the network request fails. This is because the user
    /// should always be allowed to logout and the token expires itself anyway.
    /// </remarks>
    public void Logout()
    {
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

                //this.gameController.OpenLoginScreen();
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            };

            StartCoroutine(authManager.Logout(callback));
        }
        else
        {
            Debug.Log(">>>>>> Already Logged Out ");
        }
    }

}
