
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System;
using Vuforia;
using UnityEngine.Video;
using UnityEngine.Networking;
using ImageAndVideoPicker;
using System.Collections.Generic;

public partial class MainMenuScripts : MonoBehaviour
{

    private static AuthManager _authManager;
    private TargetImageMapping targetImageMapping;


    void Start()
    {

    }
    public void switch2InfoScreen(){
        Debug.Log("<color=red> $$$$$$$$$$  inside MainMenu switch2InfoScreen is  </color>" );
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MapTargetImage");

    }

    public void switch2UIScreen()
    {
        Debug.Log("<color=red> $$$$$$$$$$  inside MainMenu switch2InfoScreen is  </color>");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UIScene");

    }

    public void switch2MainScreen()
    {
        Debug.Log("<color=red> $$$$$$$$$$  inside MainMenu switch2InfoScreen is  </color>");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");

    }


}
