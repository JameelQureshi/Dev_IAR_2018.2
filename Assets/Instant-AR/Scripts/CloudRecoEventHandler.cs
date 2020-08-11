/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */


using UnityEngine;
using System;
using Vuforia;
using UnityEngine.Video;
using UnityEngine.Networking;
using ImageAndVideoPicker;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Mapbox.Unity.Location;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CloudRecoEventHandler : MonoBehaviour, IObjectRecoEventHandler
{
    #region PRIVATE_MEMBERS
    PrimaryScreenController m_PrimaryScreenController;
    CommonBarScript m_CommonBarScript;
    CloudRecoBehaviour m_CloudRecoBehaviour;
    ObjectTracker m_ObjectTracker;
    TargetFinder m_TargetFinder;

    bool isTargetFinderScanning;
    // private VideoPlayer videoPlayer;
    private VideoPlayer videoPlayer;

    private float aspectRatio = 1.0f;
    GameObject targetImage = null;
    private String MasterJsonURL = GlobalVariables.MasterJsonURL;

    private bool isPortrait = false;
    private bool URL_FROM_DBX = false;
    //private RestServerDelegate serverDelegate;


    #endregion // PRIVATE_MEMBERS
    #region PUBLIC_MEMBERS
    /// <summary>
    /// Can be set in the Unity inspector to reference a ImageTargetBehaviour 
    /// that is used for augmentations of new cloud reco results.
    /// </summary>
    [Tooltip("Here you can set the ImageTargetBehaviour from the scene that will be used to " +
             "augment new cloud reco search results.")]
    public ImageTargetBehaviour m_ImageTargetBehaviour;
    public UnityEngine.UI.Image m_CloudActivityIcon;
    public UnityEngine.UI.Image m_CloudIdleIcon;
    public GameObject ParentCanvas;
    //public ScanLine m_ScanLine;

    private bool trackingFound=false;
    private bool locationSet = false;

    private string UniqueTargetId;

    GameObject sphere1;
    GameObject sphere2;
    GameObject newButton;
    GameObject infoPanel;
    GameObject videoPanel;
    GameObject mycanvas;


    private TargetImage targetImageObject;

    #endregion // PUBLIC_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    /// <summary>
    /// Register for events at the CloudRecoBehaviour
    /// </summary>
    void Start()
    {
        m_CommonBarScript = FindObjectOfType<CommonBarScript>();
        //serverDelegate = new RestServerDelegate();
        // m_ScanLine = FindObjectOfType<ScanLine>();
        //m_CloudRecoContentManager = FindObjectOfType<CloudRecoContentManager>();
        //m_TrackableSettings = FindObjectOfType<TrackableSettings>();

        // Register this event handler at the CloudRecoBehaviour
        m_PrimaryScreenController = FindObjectOfType<PrimaryScreenController>();
        m_CloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (m_CloudRecoBehaviour)
        {
            m_CloudRecoBehaviour.RegisterEventHandler(this);
        }

        if (m_CloudActivityIcon)
        {
            m_CloudActivityIcon.enabled = false;
        }
    }

    void Update()
    {

        if (m_CloudRecoBehaviour.CloudRecoInitialized && m_TargetFinder != null)
        {
            SetCloudActivityIconVisible(m_TargetFinder.IsRequesting());
        }

        if (m_CloudIdleIcon)
        {
            m_CloudIdleIcon.color = m_CloudRecoBehaviour.CloudRecoEnabled ? Color.white : Color.gray;
        }

        //if (trackingFound && GlobalVariables.INFO_BUTTON_CLICKED)
        if (trackingFound)
        {
            showButton();

            if (videoPanel != null && videoPlayer != null)
            {
                if (GlobalVariables.VIDEO_BUTTON_CLICKED)
                {
                    videoPanel.transform.localScale = Vector3.one;
                    videoPlayer.Play();
                }
                else
                {
                    videoPanel.transform.localScale = Vector3.zero;
                    videoPlayer.Pause();
                }
            }
        }
    }

    #endregion // MONOBEHAVIOUR_METHODS


    #region INTERFACE_IMPLEMENTATION_ICloudRecoEventHandler
    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>

    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized successfully.");

        m_ObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        m_TargetFinder = m_ObjectTracker.GetTargetFinder<ImageTargetFinder>();
    }

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized successfully.");

        m_ObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        m_TargetFinder = targetFinder;
    }


    void SetCloudActivityIconVisible(bool visible)
    {
        if (!m_CloudActivityIcon) return;

        m_CloudActivityIcon.enabled = visible;
    }

    // Error callback methods implemented in CloudErrorHandler
    public void OnInitError(TargetFinder.InitState initError) { }
    public void OnUpdateError(TargetFinder.UpdateState updateError) { }


    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetBehaviour, 
    /// then delete all trackables
    /// </summary>
    public void OnStateChanged(bool scanning)
    {
        //Debug.Log("<color=blue>OnStateChanged(): </color>" + scanning);

        // Changing CloudRecoBehaviour.CloudRecoEnabled to false will call:
        // 1. TargetFinder.Stop()
        // 2. All registered ICloudRecoEventHandler.OnStateChanged() with false.

        // Changing CloudRecoBehaviour.CloudRecoEnabled to true will call:
        // 1. TargetFinder.StartRecognition()
        // 2. All registered ICloudRecoEventHandler.OnStateChanged() with true.
    }


    public void TrackingLost()
    {
        GlobalVariables.TRACKING_FOUND = false;
        m_CommonBarScript.ARVideo.transform.localScale = Vector3.zero;
        trackingFound = false;
        if (infoPanel != null)
        {
            //Destroy(infoPanel);
            //infoPanel = null;
            infoPanel.transform.localScale = Vector3.zero;
            if (videoPanel != null)
            {
                if(videoPlayer != null)
                {
                    videoPlayer.Pause();
                }
            }
        }



        Debug.Log("<color=red> TrackingLost  </color>");
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
        }

        //set new location
        if (locationSet)
        {
            Location currentLocation = LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation;
            string location = currentLocation.LatitudeLongitude.ToString();
            string[] latlong = location.Split(',');
            if (latlong != null && latlong.Length == 2)
            {
                location = latlong[1] + "," + latlong[0];
                Debug.Log("<color=red> >>>>>>>>>>>>  currentLocation.LatitudeLongitude: </color>" + location);
                Debug.Log("<color=red> >>>>>>>>>>>> UniqueTargetId: </color>" + UniqueTargetId);
                WWW www = ARUtilityTools.setLocation(UniqueTargetId, location);
            }
        }

        //============



    }
    public void TrackingFound()
    {
        Debug.Log("<color=green> $$$$$$$$$$$$$$ TrackingFound() </color>");
        Debug.Log("<color=green> $$$$$$$$$$$$$$ GlobalVariables.AR_VIDEO_RESET </color>"+ GlobalVariables.AR_VIDEO_RESET);
        GlobalVariables.TRACKING_FOUND = true;
        if (GlobalVariables.AR_VIDEO_RESET)
        {
            if(videoPanel != null)
            {
                Destroy(videoPanel);
                videoPanel = null;
            }
            GlobalVariables.AR_VIDEO_RESET = false;
        }
       
        //m_CommonBarScript.PrimaryScreenButton.transform.localScale = Vector3.zero;
        trackingFound = true;
        if (infoPanel == null) {
            infoPanel = (GameObject)Instantiate(Resources.Load("InfoPanel"));
            //testPanel.name = "Panel_" + uiPanel.id;
            infoPanel.name = "AugmentPanel";
            infoPanel.transform.SetParent(ParentCanvas.transform);

            UIBuilder.infoAttachButton(infoPanel);
            
        }
        else
        {
            infoPanel.transform.localScale = Vector3.one;
        }

        if (infoPanel != null && videoPanel == null)
        {
            Debug.Log("<color=green> Creating VideoPanel</color>");
            videoPanel = PopupUtilities.makePopupVideo(null, infoPanel, false, false, targetImageObject.dbxVideoURL, false, Color.red, 8);
            if (videoPanel != null)
            {
                videoPanel.transform.localScale = Vector3.zero;
                StreamVideo streamVideo = videoPanel.GetComponent<StreamVideo>();
                if (streamVideo != null)
                {
                    videoPlayer = streamVideo.getVideoPlayer();
                }
            }
            else
            {
                Debug.Log("<color=green> There is no Video linked with this Image </color>");

            }
        }

        if (videoPanel != null)
        {
            m_CommonBarScript.ARVideo.transform.localScale = Vector3.one;
        }


        //Debug.Log("<color=red> TrackingFound: .... videoPlayer</color>" + videoPlayer);
        //Debug.Log("<color=red> TrackingFound:GlobalVariables.INFO_BUTTON_CLICKED </color>" + GlobalVariables.INFO_BUTTON_CLICKED);
        //if (videoPlayer != null && GlobalVariables.VIDEO_BUTTON_CLICKED)
        //{
        //    Debug.Log("<color=red> TrackingFound: TRYING TO PLAY VIDEO </color>");
        //    setVideoScale(aspectRatio, isPortrait);
        //    videoPlayer.Play();
        //}
    }


        public void refreshScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("3-CloudReco");

    }


    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetImageSearchResult)
    {
        
        Debug.Log("<color=green> =================================================================== </color>");
        Debug.Log("<color=green> >>>>>>>>>>>> OnNewSearchResult(): </color>" + targetImageSearchResult.UniqueTargetId);
        Debug.Log("<color=green> =================================================================== </color>");

        locationSet = true;

        videoPlayer = null;
        m_CommonBarScript.PrimaryScreenButton.transform.localScale = Vector3.one;
        GlobalVariables.VIDEO_BUTTON_CLICKED = false;
        if (infoPanel != null)
        {
            Destroy(infoPanel);
            infoPanel = null;
        }
        if (videoPanel != null)
        {
            Destroy(videoPanel);
            videoPanel = null;
        }


        TargetFinder.CloudRecoSearchResult targetSearchResult = (TargetFinder.CloudRecoSearchResult)targetImageSearchResult;


        string model_name = targetSearchResult.MetaData;
        UniqueTargetId = targetSearchResult.UniqueTargetId;
        

        targetImageObject = ARUtilityTools.getFullTargetImageFromUID(UniqueTargetId);
        if (targetImageObject == null)
        {
            targetImageObject = ARUtilityTools.getTargetImageFromUID(UniqueTargetId);
        }

        if (targetImageObject == null)
        {
            JituMessageBox.DisplayMessageBox("AR Experience Missing", "No AR Experience is found for this Object !", true, refreshScene);
        }
        //TargetImage targetImageObject = ARUtilityTools.getFullTargetImageFromUID(UniqueTargetId);


        //Assign Global Variables
        ARUtilityTools.initializeGlobalVariables(targetImageObject);

        //GlobalVariables.uiScene = SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Single);
        //GlobalVariables.uiScene.allowSceneActivation = false;

        //Check if the metadata isn't null
        if (targetSearchResult.MetaData == null)
        {
            Debug.Log("Target metadata not available.");
        }
        else
        {
            Debug.Log("MetaData: " + targetSearchResult.MetaData);
            Debug.Log("TargetName: " + targetSearchResult.TargetName);
            Debug.Log("Pointer: " + targetSearchResult.TargetSearchResultPtr);
            Debug.Log("TargetSize: " + targetSearchResult.TargetSize);
            Debug.Log("TrackingRating: " + targetSearchResult.TrackingRating);
            Debug.Log("<color=blue>  @@@@@@@@@@@@@@@@@@@@@@  TrackingRating: : </color>  " + targetSearchResult.TrackingRating);

            Debug.Log("UniqueTargetId: " + targetSearchResult.UniqueTargetId);
            Debug.Log("Type: " + targetSearchResult.GetType());
            Debug.Log("Hashcode: " + targetSearchResult.GetHashCode());
            Debug.Log("targetSearchResult.ToString: " + targetSearchResult.ToString());

        }
        // First clear all trackables
        m_TargetFinder.ClearTrackables(false);
        // enable the new result with the same ImageTargetBehaviour:
        m_TargetFinder.EnableTracking(targetSearchResult, m_ImageTargetBehaviour.gameObject);
        Renderer[] rendererComponents = m_ImageTargetBehaviour.gameObject.GetComponentsInChildren<Renderer>();
        string targetURL = targetImageObject.dbxVideoURL;
        string aspectR = targetImageObject.aspectRatio;
        string portrait = targetImageObject.videoPortrait;
        if (string.IsNullOrEmpty(portrait))
        {
            portrait = "no";
        }



        aspectRatio = 1.0f;
        aspectRatio = float.Parse(aspectR);

        targetURL = "https://dl.dropbox.com/s/pz73u6nuuyb0j0f/Cummins_AR_Use_Case.MP4";
        targetURL = "";
        foreach (Renderer component in rendererComponents)
        {
            GameObject myObject = (GameObject)component.gameObject;
            Debug.Log(">>>>>>>>>>>>> The myObject is:  " + myObject.name);

            if (myObject.name.Equals("Quad"))
            {
                myObject.transform.localScale = Vector3.zero;
            }
            else if (myObject.name.Equals("mySphere"))
            {
                float xPosition = -0.5f;
                float zPosition = 0.5f;
                if (aspectRatio < 1)
                {
                    xPosition = (aspectRatio / 2) * -1;
                }
                else if (aspectRatio > 1)
                {
                    zPosition = (1 / aspectRatio) / 2;
                }

                //Debug.Log(">>><<<<<<<<<<<<< The xPosition is:  " + xPosition);
                //Debug.Log(">>><<<<<<<<<<<<< The zPosition is:  " + zPosition);
                myObject.transform.localPosition = new Vector3(xPosition, 0.1f, zPosition);

                sphere1 = myObject;



            }
            else if (myObject.name.Equals("mySphere2"))
            {
                float xPosition = 0.5f;
                float zPosition = -0.5f;
                if (aspectRatio < 1)
                {
                    xPosition = (aspectRatio / 2);
                }
                else if (aspectRatio > 1)
                {
                    zPosition = ((1 / aspectRatio) / 2) * -1;
                }
                //Debug.Log(">>><<<<<<<<<<<<< The xPosition is:  " + xPosition);
                //Debug.Log(">>><<<<<<<<<<<<< The zPosition is:  " + zPosition);
                myObject.transform.localPosition = new Vector3(xPosition, 0.1f, zPosition);
                sphere2 = myObject;

            }
        }
        Debug.Log("<color=white> =================================================================== </color>");
        Debug.Log("<color=green> =================================================================== </color>");
    }

    void showButton()
    {
        Vector3 sphere1Pos = Camera.main.WorldToScreenPoint(sphere1.transform.position);
        Vector3 sphere2Pos = Camera.main.WorldToScreenPoint(sphere2.transform.position);
        float image_width = sphere2Pos.x - sphere1Pos.x;
        float image_height = sphere1Pos.y - sphere2Pos.y;
        //Debug.Log("<color=green> >>>>>>>>>>>> image_width: </color>" + image_width);
        //Debug.Log("<color=green> >>>>>>>>>>>> image_heigh: </color>" + image_height);

        if (infoPanel != null)
        {
            infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(image_width, image_height);
            infoPanel.transform.position = sphere1Pos + new Vector3(image_width * 0.5f, (image_height * 0.5f) * -1, 0.0f);
            Button[] buttons = infoPanel.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                IconButtonScript iconScript = button.GetComponent<IconButtonScript>();
                if (iconScript != null)
                {
                    float xFactor = iconScript.posX;
                    float yFactor = iconScript.posY;
                    float sizeRatio = iconScript.sizeRatio;

                    button.transform.position = sphere1Pos + new Vector3(image_width * xFactor, (image_height * yFactor), 0.0f);
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(image_width / 10, image_width / 10);
                    button.transform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
                }

            }

        }
    }

    

    //void setVideoScale(float aspect, bool portrait)
    //{
    //    float localAspect = aspect;
    //    Debug.Log("<color=blue>  >>>>Inside  setVideoScale with aspectRatio: : </color> " + localAspect);
    //    if (localAspect > 1 && portrait && URL_FROM_DBX)
    //    {
    //        Debug.Log("<color=red>  >>>>Rotating the video by 90 degree </color> ");
    //        localAspect = 1 / localAspect;
    //        var rot = videoPlayer.transform.rotation;
    //        rot.x = 90;
    //        rot.y = 90;
    //        rot.z = 0;
    //        Vector3 view = new Vector3(90, 0, -90);
    //        videoPlayer.transform.rotation = Quaternion.Euler(view.x, view.y, view.z);

    //    }
    //    else
    //    {
    //        Debug.Log("<color=green>  >>>>No Video Rotation is required  </color> ");
    //    }
    //    if (localAspect < 1)
    //    {
    //        videoPlayer.transform.localScale = new Vector3(localAspect, 1, 1);
    //    }
    //    else if (localAspect > 1)
    //    {
    //        localAspect = 1 / localAspect;
    //        Debug.Log(">>>>aspectRatio is now :  " + localAspect);
    //        videoPlayer.transform.localScale = new Vector3(1, localAspect, 1);
    //    }
    //    else
    //    {
    //        videoPlayer.transform.localScale = new Vector3(1, 1, 1);
    //    }

    //}
    #endregion // INTERFACE_IMPLEMENTATION_ICloudRecoEventHandler


    //Get Jason
    public Dictionary<string, string> getDbxPathFromJson(String UniqueTargetId)
    {
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        var masterResponse = WebFunctions.Get(MasterJsonURL);
        var data = (JObject)JsonConvert.DeserializeObject(masterResponse.text);
        string jsonURL = data[UniqueTargetId].Value<string>();


        var jsonResponse = WebFunctions.Get(jsonURL);
        var jsonData = (JObject)JsonConvert.DeserializeObject(jsonResponse.text);

        TargetImage targetImageObject = JsonUtility.FromJson<TargetImage>(jsonResponse.text);

        keyValuePairs.Add("dbxurl", targetImageObject.dbxVideoURL);
        keyValuePairs.Add("aspectRatio", targetImageObject.aspectRatio);
        keyValuePairs.Add("portrait", targetImageObject.videoPortrait);

        Debug.Log("<color=blue> !!!!!!!!!!!! DONE WITH targetImageObject : </color>" + jsonURL);

        return keyValuePairs;
    }


}

public class ServerJsonObject
{
    public string DBSERVER;
    public string MASTERJSON;

}

public class VuforiaJsonObject
{
    public string aspectRatio;
    public string dbxURL;
    public string status;
    public string portrait;

}
