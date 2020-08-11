/*============================================================================== 
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ImageAndVideoPicker;
using System.IO;
using System.Xml;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using UnityEngine.SceneManagement;

public class DialogueBox : MonoBehaviour
{
    #region PRIVATE_MEMBERS
    Text[] textComponents;
    delegate void DelegateMessageBoxButtonAction();
    DelegateMessageBoxButtonAction m_DelegateMessageBoxAction;

    private static String myToken = GlobalVariables.DROPBOX_TOCKEN;
    private String targetFolder = GlobalVariables.DROPBOX_STAGING_FOLDER;

    private static GameObject youTubeButton;
    private static GameObject videoButton;
    private static GameObject imageButton;
    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_METHODS

    //public static void DisplayMessageBox(string title,
    //string body, bool dismissable, Action dismissAction)
    //{
    //    Debug.Log("<color=red> $$$$$$$ Inside DisplayMessageBox  </color>");
    //    GameObject prefab = (GameObject)Resources.Load("DialogueBox");
    //    Debug.Log("<color=red> $$$$$$$ Got the  prefab  </color>" + prefab);
    //    if (prefab)
    //    {
    //        Debug.Log("<color=red> $$$$$$$ Instantiating messageBox  </color>");
    //        DialogueBox messageBox = Instantiate(prefab.GetComponent<DialogueBox>());
    //        Debug.Log("<color=red> $$$$$$$ Instantiate  messageBox  </color>");
    //        messageBox.Setup(title, body, dismissable, dismissAction);
    //        Debug.Log("<color=red> $$$$$$$ Instantiate  messageBox  </color>");
    //    }
    //}

    public static void UploadYouTubeLinkBox(string title,
    string body, bool dismissable, Action dismissAction, GameObject button1, GameObject button2, GameObject button3)
    {
        Debug.Log("<color=red> $$$$$$$ Inside DisplayMessageBox  </color>");

        videoButton = button1;
        youTubeButton = button2;
        imageButton = button3;

        GameObject prefab = (GameObject)Resources.Load("DialogueBox");
        Debug.Log("<color=red> $$$$$$$ Got the  prefab  </color>" + prefab);
        if (prefab)
        {
            Debug.Log("<color=red> $$$$$$$ Instantiating messageBox  </color>");
            DialogueBox messageBox = Instantiate(prefab.GetComponent<DialogueBox>());
            Debug.Log("<color=red> $$$$$$$ Instantiate  messageBox  </color>");
            messageBox.Setup(title, body, dismissable, dismissAction);
            Debug.Log("<color=red> $$$$$$$ Instantiate  messageBox  </color>");
        }
    }

    public void CancelActionButton()
    {
        // This method will close the window
        if (youTubeButton != null)
            youTubeButton.transform.localScale = new Vector3(1, 1, 1);
        if (videoButton != null)
            videoButton.transform.localScale = new Vector3(1, 1, 1);
        Destroy(gameObject);
    }

    public void MessageBoxButton()
    {
        // This method called by the UI Canvas Button

        // If there's a custom method, run it first
        if (m_DelegateMessageBoxAction != null)
        {
            m_DelegateMessageBoxAction();
            textComponents = GetComponentsInChildren<Text>();
            Debug.Log("<color=red> $$$$$$$ Final Inside m_DelegateMessageBoxAction  </color>" + textComponents[2].text);
            StartCoroutine(uploadYouTubeLink(textComponents[2].text));
        }

        // Destroy MessageBox
        //Destroy(gameObject);
    }

    #endregion // PUBLIC_METHODS


    #region PRIVATE_METHODS

    void Setup(string title, string body, bool dismissable = true, Action closeButton = null)
    {
        textComponents = GetComponentsInChildren<Text>();
        if (textComponents.Length >= 2)
        {
            //textComponents[0].text = title;
            //textComponents[1].text = body;
        }
        Debug.Log("<color=red> $$$$$$$ Inside Setup,textComponents[1].text is  </color>" + textComponents[1].text);

        if (closeButton != null)
            m_DelegateMessageBoxAction = new DelegateMessageBoxButtonAction(closeButton);

        Button button = GetComponentInChildren<Button>();
        if (button != null)
        {
            button.gameObject.SetActive(dismissable);
        }
    }



    //JITU added 
    private IEnumerator uploadYouTubeLink(String link)
    {
        Debug.Log("<color=red> Inside uploadYouTubeLink   </color>" + link);
        String filename = "";
        if (!string.IsNullOrEmpty(GlobalVariables.VUFORIA_UNIQUE_ID))
        {
            filename = GlobalVariables.VUFORIA_UNIQUE_ID + ".ytb";
            //Debug.Log("<color=green> $$$$$$$ WOW>>>  testFileName is:  </color>" + filename);
        }
        else if (!string.IsNullOrEmpty(GlobalVariables.initialMobileImagePath))
        {
            string imageName = GlobalVariables.initialMobileImagePath.Substring(GlobalVariables.initialMobileImagePath.LastIndexOf("/") + 1);
            filename = "INIT_VIDEO" + imageName + ".ytb";
            //Debug.Log("<color=green> $$$$$$$ WOW>>>  testFileName is:  </color>" + prefixPath + testFileName);
        }
        else
        {
            filename = "youtubelink.ytb";
        }

        String targetPath;
        String uid = SystemInfo.deviceUniqueIdentifier;
        uid = uid + "/";
        AuthManager _authManager = AuthManager.Instance;
        if (_authManager.IsLoggedIn)
        {
            string subFolder = _authManager.CurrentToken.username;
            subFolder = subFolder.Substring(0, subFolder.LastIndexOf("."));
            //Debug.Log(">>>>>>>>> Inside UploadImage..the subFolder is:  " + subFolder);
            targetPath = targetFolder + subFolder + "/" + filename;
        }
        else
        {
            targetPath = targetFolder + uid + filename;
        }
        Debug.Log(">>>>>>>>>>>>>>>>>>>>targetPath is" + targetPath);
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        String param = "{\"path\": \"" + targetPath + "\",\"mode\": \"overwrite\",\"autorename\": false,\"mute\": false}";
        postHeader.Add("Authorization", myToken);
        postHeader.Add("Dropbox-API-Arg", param);
        postHeader.Add("Content-Type", "application/octet-stream");
        //Debug.Log(">>>>>>>>>>>>>>>>>>>>filename is" + filename);
        byte[] myData = Encoding.ASCII.GetBytes(link);
        int sourceSize = myData.Length;
        //Debug.Log(">>>>>>>>>>>>>>>>>>>>myData is" + myData.ToString());
        WWW www = new WWW("https://content.dropboxapi.com/2/files/upload", myData, postHeader);


        //StartCoroutine(ShowProgress(www, true));
        yield return www;
        if (www.error == null)
        {
            Debug.Log("<color=white>   >>>>>>success: </color>" + www.text);
        }
        else
        {
            Debug.Log("<color=white>   >>>>>>something wrong:  </color>" + www.text);
        }
        //JituMessageBox.DisplayMessageBox("Video Upload Status", "The Video Link is Succesfully Uploaded", true, CloseDialog);
        JituMessageBox.DisplayMessageBox("AR Video Upload Status", "Awesome!\nYour AR Experience Video would be active shortly.\n", true, CloseDialog);
        if (youTubeButton != null)
            youTubeButton.transform.localScale = new Vector3(0, 0, 0);
        if (videoButton != null)
            videoButton.transform.localScale = new Vector3(0, 0, 0);
        if (imageButton != null)
            imageButton.transform.localScale = new Vector3(1, 1, 1);

        Destroy(this.gameObject);
        GlobalVariables.INFO_BUTTON_CLICKED = false;
        GlobalVariables.INFO_PANEL_BUTTON_CLICKED = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("3-CloudReco");

    }
    public void CloseDialog()
    {
        Debug.Log("<color=red> $$$$$$$ JABARDAST KAMAAL HO GAYA   </color>");
    }




    /////

    #endregion // PRIVATE_METHODS
}
