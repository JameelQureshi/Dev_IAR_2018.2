using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AccountController : MonoBehaviour {

	/// <summary>
	/// Enum that states all the screens within the app. This helps express the screens that should be seen and also
	/// helps clarity when dealing with screen management.
	/// </summary>
	private enum Screens {
		LoginScreen,
		RegisterScreen,
		LogoutScreen,
		ForgotPasswordScreen
	}
	
	public CanvasGroup loginScreen;
	public CanvasGroup registerScreen;
	public CanvasGroup logoutScreen;
	public CanvasGroup forgotPasswordScreen;

	public AccountLoginScreen loginScreenScript;
	public AccountRegisterScreen registerScreenScript;
	public AccountLogoutScreen logoutScreenScript;
	public AccountForgotPasswordScreen forgotPasswordScreenScript;
	
	private AuthManager _authManager;
	
	void Awake() {
		
		_authManager = AuthManager.Instance;
		
		// Make sure all screens are set
		ValidateScreenReferences(); 
		EnableAllScreens();
	}

	void Start() {
		
		if (_authManager.IsLoggedIn) {
			ShowScreen(Screens.LogoutScreen);
		}
		else {
			//ShowScreen(Screens.LoginScreen);
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
        }
		
	}

    #region Screen Management
    /// <summary>
    /// We need to make sure that all screens are set within the Unity Editor. Do this within the Awake method to
    /// throw any errors if one isn't set.
    ///
    /// This will throw a runtime error if any are not set
    /// </summary>
    ///
    /// <exception cref="MissingReferenceException">When any screen references are not set on the AccountController Object</exception>
    private void ValidateScreenReferences() {
		if (loginScreen == null
		    || registerScreen == null
		    || logoutScreen == null
		    || forgotPasswordScreen == null) {
			
			throw new MissingReferenceException("Please set all screen GameObject references on the @AccountController Editor object");
			
		}
	}

	/// <summary>
	/// The UnityEditor may have some things disabled/enabled. This method makes sure that all screens are in the correct
	/// state to start the game.
	/// </summary>
	private void EnableAllScreens() {
		loginScreen.gameObject.SetActive(true);
		registerScreen.gameObject.SetActive(true);
		logoutScreen.gameObject.SetActive(true);
		forgotPasswordScreen.gameObject.SetActive(true);
	}

	/// <summary>
	/// Hides all of the GUI screens within this game. Use this immediately before showing a screen to avoid having
	/// to manually check which screen is currently on screen that needs to be closed out.
	/// </summary>
	/// <remarks>
	/// We are not disabling the screens. This is due to Unity's event system and that disabled objects don't receive
	/// events. While disabling/enabling screens would be less code it may not be scalable in the future if some screens
	/// rely on obersving events within the game.
	/// </remarks>
	private void HideAllScreens() {
		loginScreen.alpha = 0;
		loginScreen.blocksRaycasts = false;
		loginScreen.interactable = false;
		
		registerScreen.alpha = 0;
		registerScreen.blocksRaycasts = false;
		registerScreen.interactable = false;
		
		logoutScreen.alpha = 0;
		logoutScreen.blocksRaycasts = false;
		logoutScreen.interactable = false;
		
		forgotPasswordScreen.alpha = 0;
		forgotPasswordScreen.blocksRaycasts = false;
		forgotPasswordScreen.interactable = false;
	}

	/// <summary>
	/// Shows the given screen and hides all other screens.
	/// </summary>
	/// <param name="screenToShow">The screen to show next</param>
	private void ShowScreen(Screens screenToShow) {
		HideAllScreens();

		switch (screenToShow) {
			case Screens.LoginScreen:
				loginScreen.alpha = 1;
				loginScreen.blocksRaycasts = true;
				loginScreen.interactable = true;
				break;
			
			case Screens.LogoutScreen:
				logoutScreen.alpha = 1;
				logoutScreen.blocksRaycasts = true;
				logoutScreen.interactable = true;
				logoutScreenScript.Setup();
				break;
			
			case Screens.RegisterScreen:
				registerScreen.alpha = 1;
				registerScreen.blocksRaycasts = true;
				registerScreen.interactable = true;
				break;
			
			case Screens.ForgotPasswordScreen:
				forgotPasswordScreen.alpha = 1;
				forgotPasswordScreen.blocksRaycasts = true;
				forgotPasswordScreen.interactable = true;
				break;
		}
	}
	#endregion
	
	
	#region Public API (used from other scripts)

	public void OpenLoginScreen() {
		ShowScreen(Screens.LoginScreen);
	}

	public void OpenLogoutScreen() {
		ShowScreen(Screens.LogoutScreen);
	}

	public void OpenRegistrationScreen() {
		ShowScreen(Screens.RegisterScreen);
	}

	public void OpenForgotPasswordScreen() {
		ShowScreen(Screens.ForgotPasswordScreen);
	}
	#endregion
	
}
