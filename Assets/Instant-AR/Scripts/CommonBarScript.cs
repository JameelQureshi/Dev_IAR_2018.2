/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CommonBarScript : BaseScreen
{
    public Button menuButton;
    public Button cloudScanButton;
    public Button RefreshButton;
    public Button PrimaryScreenButton;
    public Button ARScreenButton;
    public Button ARVideo;
    public Button FindOnMap;

    private GameObject MapPanel;
    private GameObject floatingSpace;
    private GameObject scanLine;

    private Sprite mapSprite;
    private Sprite scanSprite;

    private FindNearbyObjects m_FindNearbyObjects;

    public GameObject UIScreen;

    public override void Awake()
    {
        base.Awake();
        authManager = AuthManager.Instance;
    }


    void Start()
    {
        floatingSpace = GameObject.Find("FloatingCanvas");
        scanLine = GameObject.Find("ScanLine");
        m_FindNearbyObjects = FindObjectOfType<FindNearbyObjects>();
        mapSprite = Resources.Load<Sprite>("JituSprites/CenterMarker");
        scanSprite = Resources.Load<Sprite>("JituSprites/scan");

        if (authManager.IsLoggedIn)
        {
            menuButton.transform.localScale = Vector3.one;
        }
        else
        {
            menuButton.transform.localScale = Vector3.zero;
        }

        Debug.Log("<color=red> @@@@@@@@@@@@@@@@@@@@@@@@@@@ GlobalVariables.LocationMap_CLICKED   : </color>" + GlobalVariables.LocationMap_CLICKED);
        Debug.Log("<color=red> ################## GlobalVariables.VUFORIA_UNIQUE_ID: </color>" + GlobalVariables.VUFORIA_UNIQUE_ID);
        Debug.Log("<color=red> ############## GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID: </color>" + GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID);

        if (GlobalVariables.LocationMap_CLICKED)
        {
            PrimaryScreen();
        }
    }


    //public void OpenCloudScreen()
    //{
    //    m_FindNearbyObjects.unClick();
    //    this.gameController.OpenCloudScreen();
    //}

    public void OpenARScreen() {
        //cloudScanButton.transform.localScale = Vector3.one;
        //Debug.Log("<color=red> @@@@@@@@@ This is  PrimaryScreenButton)  : </color>");
        //CloseARActivities();
        //cloudScanButton.image.sprite = scanSprite;
        this.gameController.OpenARScreen();
    }
    public void RestoreContainerColor()
    {

        foreach (string containerName in GlobalVariables.ContainerNames.Keys)
        {
            string containerColor = GlobalVariables.ContainerNames[containerName];

            Debug.Log("<color=green> @@@@ containerName   : </color>" + containerName);
            Debug.Log("<color=green> @@@@ containerColor   : </color>" + containerColor);

            GameObject containerObject = GameObject.Find(containerName);
            UnityEngine.UI.Image img = containerObject.GetComponent<UnityEngine.UI.Image>();

            if (containerName.Contains("Canvas"))
            {
                img.color = PrimaryScreenController.getColor(containerColor);
                img.color = PrimaryScreenController.getColor("rgba(0,0,0,255)");
            }
            else
            {
                img.color = PrimaryScreenController.getColor(containerColor);
            }

            
            //img.color = PrimaryScreenController.getColor("rgba(0,0,0,0)");
        }
        Debug.Log("<color=green> @@@@ GlobalVariables.PrimaryImageName   : </color>" + GlobalVariables.PrimaryImageName);
        GameObject primaryImage = GameObject.Find(GlobalVariables.PrimaryImageName);
        primaryImage.gameObject.transform.localScale = Vector3.one;

    }

    public void Clicked(Button thisButton)
    {
        if (thisButton == cloudScanButton)
        {
            GlobalVariables.UIScreen_CLICKED = false;
            thisButton.transform.localScale = Vector3.zero;
            Debug.Log("<color=green> @@@@@@@@@ YES This is cloudScanButton)  : </color>");
            this.gameController.OpenCloudScreen();
            CloseARActivities();
            if (scanLine != null)
            {
                scanLine.SetActive(true);
                //scanLine.GetComponent<CloudRecoScanLine>().m_Camera = null;
            }
        }
        else if (thisButton == FindOnMap)
        {
            GlobalVariables.UIScreen_CLICKED = false;
            cloudScanButton.transform.localScale = Vector3.one;
            //Debug.Log("<color=red> @@@@@@@@@ This is  PrimaryScreenButton)  : </color>");
            CloseARActivities();
            this.gameController.OpenCloudScreen();
            MapScanClicked();
        }
        else if (thisButton == PrimaryScreenButton)
        {
            PrimaryScreen();
        }
        else if (thisButton == ARScreenButton)
        {
            GlobalVariables.UIScreen_CLICKED = false;
            //this.gameController.OpenARScreen();
            if (GlobalVariables.ARScreen_Enable)
            {
                GlobalVariables.ARScreen_Enable = false;
            }
            else
            {
                GlobalVariables.ARScreen_Enable = true;
            }

            //Test Map Screen================

            MapScreen();

            //====================

        }
        else if (thisButton == menuButton)
        {
            GlobalVariables.UIScreen_CLICKED = false;
            cloudScanButton.transform.localScale = Vector3.one;
            //Debug.Log("<color=red> @@@@@@@@@ This is  menuButton)  : </color>");
            CloseARActivities();
            cloudScanButton.image.sprite = scanSprite;
            this.gameController.OpenLogoutScreen();
        }
        else if (thisButton == RefreshButton)
        {
            GlobalVariables.UIScreen_CLICKED = false;
            RestartApplication();
        }
        else if (thisButton == ARVideo)
        {
            GlobalVariables.UIScreen_CLICKED = false;
            if (GlobalVariables.VIDEO_BUTTON_CLICKED)
            {
                GlobalVariables.VIDEO_BUTTON_CLICKED = false;
                thisButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("JituSprites/play");
                UIScreen.transform.localScale = Vector3.one;
            }
            else
            {
                GlobalVariables.VIDEO_BUTTON_CLICKED = true;
                thisButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("JituSprites/Button-Info-icon");
                UIScreen.transform.localScale = Vector3.zero;
            }
        }
    }

    private void MapScreen()
    {
        if (authManager.IsLoggedIn)
        {
            GlobalVariables.CURRENT_USER = authManager.CurrentToken.username;
            string getLocatioApiURL = GlobalVariables.REST_SERVER + "getlocation/";
            string userName = GlobalVariables.CURRENT_USER;
            userName = userName.Substring(0, userName.LastIndexOf("."));
            Debug.Log("<color=red> @@@@########$$$$$$$$$ userName  : </color>" + userName);
            string JsonURL = getLocatioApiURL + userName;
            Debug.Log("<color=red> @@@@########$$$$$$$$$ JsonURL  : </color>" + JsonURL);

            StartCoroutine(ProcessMapScreen(JsonURL));
        }
        else
        {
            JituMessageBox.DisplayMessageBox("User Login Status", "Please login to see the Objects on the Map !", true, null);
        }
    }

    IEnumerator ProcessMapScreen(string JsonURL)
    {
        UnityWebRequest www = UnityWebRequest.Get(JsonURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            LocationData locationData = JsonUtility.FromJson<LocationData>(www.downloadHandler.text);
            Debug.Log("<color=red> @@@@########$$$$$$$$$ www.downloadHandler.text  : </color>" + www.downloadHandler.text);
            if (locationData.objectLocations != null && locationData.objectLocations.Count > 0)
            {
                Debug.Log("<color=red> @@@@########$$$$$$$$$ locationData : </color>" + locationData.userID);
                GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID = "";
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
            else
            {
                JituMessageBox.DisplayMessageBox("Objects Missing", "No Objects is accessible to be displayed on the map !", true, null);

            }
        }
    }



    private void PrimaryScreen()
    {
        GlobalVariables.UIScreen_CLICKED = true;
        UIScreen.transform.localScale = Vector3.one;
        cloudScanButton.transform.localScale = Vector3.one;
        //Debug.Log("<color=red> @@@@@@@@@ This is  PrimaryScreenButton)  : </color>");
        CloseARActivities();
        cloudScanButton.image.sprite = scanSprite;
        this.gameController.OpenUIScreen();
        RestoreContainerColor();
    }

    private void CloseARActivities()
    {
        GlobalVariables.VIDEO_BUTTON_CLICKED = false;
        GlobalVariables.AR_VIDEO_RESET = true;
        ARVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>("JituSprites/play");
        CloseMapPanel();
    }

    private void MapScanClicked()
    {
        if (floatingSpace != null)
        {
            if (MapPanel == null)
            {
                MapPanel = (GameObject)Instantiate(Resources.Load("ShowMapPanel"));
                MapPanel.transform.SetParent(floatingSpace.transform);
                MapPanel.transform.localScale = Vector3.one;

                RectTransform mapRectTransform = MapPanel.GetComponent<RectTransform>();
                mapRectTransform.anchorMin = new Vector2(0, 0);
                mapRectTransform.anchorMax = new Vector2(1, 1);
                mapRectTransform.pivot = new Vector2(0, 0);
                mapRectTransform.offsetMin = new Vector2(0, 0);
                mapRectTransform.offsetMax = new Vector2(0, 0);
            }
            else
            {
                MapPanel.transform.localScale = Vector3.one;
            }
            if (scanLine != null)
            {
                scanLine.SetActive(false);
            }
        }

    }

    private void CloseMapPanel()
    {
        if (MapPanel != null)
        {
            MapPanel.transform.localScale = Vector3.zero;
        }
    }

    void RestartApplication()
    {
        //if (PrimaryScreenButton != null)
        //{
        //    PrimaryScreenButton.transform.localScale = Vector3.zero;
        //}
        GlobalVariables.VUFORIA_UNIQUE_ID = null;
        GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID = null;
        GlobalVariables.ButtonList = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        
    }

}
