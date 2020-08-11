using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// This screen is used to handle all user actions pertaining to the login screen. It uses the Base screen superclass
/// to automatically declare various variables and methods that each screen shoul have.
/// </summary>
public class LoginScreen : BaseScreen {

	public GameObject MainCamera;
	public GameObject ARCamera;


	public InputField emailInput;
	public InputField passwordInput;

	public Text messageArea;

	private CommonBarScript m_CommonBarScript;


	public override void Awake() {
		base.Awake();
		authManager = AuthManager.Instance;
	}

	void Start()
	{
		m_CommonBarScript = FindObjectOfType<CommonBarScript>();
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
                //this.gameController.OpenLogoutScreen();
                DontDestroyOnLoad(this.gameObject);
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("3-CloudReco");
				this.gameController.OpenCloudScreen();
				m_CommonBarScript.menuButton.transform.localScale = Vector3.one;
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
        Debug.Log("<color=red> $$$$$$$ WOW>>> Inside OpenRegistrationScreen !  </color>");
        this.gameController.OpenRegistrationScreen();
	}

    public void GuestTrail()
    {
		//UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("3-CloudReco");
		//Debug.Log("<color=red> $$$$$$$ @@@@@@@@@@@@@@@ WOW>>> Inside GuestTrail !  </color>");
		m_CommonBarScript.menuButton.transform.localScale = Vector3.one;
		this.gameController.OpenCloudScreen();
	}

	public void OpenUIScreen()
	{
		Debug.Log("<color=red> $$$$$$$ WOW>>> Inside OpenRegistrationScreen !  </color>");
		this.gameController.OpenUIScreen();
	}


	public void OpenCloudScreen()
	{
		//this.gameController.cloudScreen.gameObject.SetActive(true);
		//this.gameController.jituScreen.gameObject.SetActive(false);

		MainCamera.SetActive(false);
		if (this.gameController == null)
		{
			Debug.Log("<color=red>gameController is NULL </color>");
		}
		if (this.gameController.cloudScreen == null)
		{
			Debug.Log("<color=red>cloudScreen is NULL </color>");
		}
		else
		{
			Debug.Log("<color=green>cloudScreen is NOT NULL </color>");
			this.gameController.cloudScreen.gameObject.SetActive(true);
			this.gameController.OpenCloudScreen();
		}

		MainCamera.SetActive(false);
		ARCamera.SetActive(true);
	}

	public void CloseCloudScreen()
	{
		//this.gameController.cloudScreen.gameObject.SetActive(true);
		//this.gameController.jituScreen.gameObject.SetActive(false);
		ARCamera.SetActive(false);
		
		if (this.gameController == null)
		{
			Debug.Log("<color=red>gameController is NULL </color>");
		}
		if (this.gameController.cloudScreen == null)
		{
			Debug.Log("<color=red>cloudScreen is NULL </color>");
		}
		else
		{
			Debug.Log("<color=green>cloudScreen is NOT NULL </color>");
			this.gameController.cloudScreen.gameObject.SetActive(false);
			this.gameController.OpenUIScreen();
		}

		MainCamera.SetActive(true);
	}



}
