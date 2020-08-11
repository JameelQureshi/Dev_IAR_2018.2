using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ImageAndVideoPicker;
using System.IO;
using System.Xml;
using System.Net;
using System.Collections.Specialized;
using System;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{

    public Slider infoTextSizeSlider;
    public Text smallFontSymbol;
    public Text bigFontSymbol;
    public Button infoEditButton;
    public Button heathButton;

    private AuthManager _authManager;

    public void openCamCapture()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CameraCapture");

    }

    public void openAccountPage()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");

    }
    public void openInstantARPage()
    {
        //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("3-CloudReco");
        UnityEngine.SceneManagement.SceneManager.LoadScene("3-CloudReco");

    }
    public void openAccountPage_Org()
    {
        Debug.Log("<color=red> AccountLogin Screen is Called !!  </color>");
        _authManager = AuthManager.Instance;
        if (_authManager.IsLoggedIn)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("AccountLogin");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
        }
    }

    public void openMapImagePage()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UIScene");
        /*
        infoTextSizeSlider.transform.localScale = new Vector3(0, 0, 0);
        smallFontSymbol.transform.localScale = new Vector3(0, 0, 0);
        bigFontSymbol.transform.localScale = new Vector3(0, 0, 0);
        infoEditButton.transform.localScale = new Vector3(0, 0, 0);
        GlobalVariables.INFO_BUTTON_CLICKED = false;
        GlobalVariables.INFO_PANEL_BUTTON_CLICKED = true;
        Debug.Log("<color=green> ####### openMapImagePage clicked  </color>");
        //DestroyDontDestroyOnLoadGameObjects();
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MapTargetImage");
        */

    }
    private void DestroyDontDestroyOnLoadGameObjects()
    {
        var dontDestoyOnLoadScene = SceneManager.GetSceneByName("3-CloudReco");
        var dontDestroyOnLoadGameObjects = dontDestoyOnLoadScene.GetRootGameObjects();
        Debug.Log("<color=green> ####### dontDestroyOnLoadGameObjects length ---  </color>" + dontDestroyOnLoadGameObjects.Length);
        foreach (var dontdestroyGameObject in dontDestroyOnLoadGameObjects)
        {
            Debug.Log("<color=green> ####### Destroying  ---  </color>"+gameObject.name);
            Destroy(gameObject);
        }
    }

    public void openHealthScreen()
    {
        heathButton.transform.localScale = new Vector3(0, 0, 0);
        UnityEngine.SceneManagement.SceneManager.LoadScene("StaticTargetImage");
    }

}
