/*============================================================================== 
Copyright (c) 2017-2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;

public static class GlobalVariables
{
    public static string DROPBOX_TOCKEN = "Bearer 4xBkAhkRvGAAAAAAAAAFnP-WOrtd-lnl0_UIo9N1L1qEHExWywUuWZWvbQdaMDDm";
    public static string GOOGLE_MAP_API_KEY = "AIzaSyCYhQrjBnkiAeHmW_IHZNMaXtAkI24qX6k";

    //==================== Dev DBX  ====================================
    public static string MasterJsonURL = "https://dl.dropbox.com/s/8vkvfurw5deia89/MasterJason.json";
    //public static string DROPBOX_STAGING_FOLDER = "/dev/CoolARApp/StagingArea/";
    //public static string DROPBOX_EXPERIENCE_FOLDER_INFO = "/dev/CoolARApp/TargetExperiences/Info/";

    public static string DROPBOX_STAGING_FOLDER = "/local/CoolARApp/StagingArea/";
    public static string DROPBOX_EXPERIENCE_FOLDER_INFO = "/local/CoolARApp/TargetExperiences/Info/";

    //public static string REST_SERVER = "https://ar-dev.emachinelabs.com/";	
    public static string REST_SERVER = "http://dropbox-env.8xe8tpevpj.us-west-1.elasticbeanstalk.com/";
    public static string GET_IMAGE_JSON_API = "2/query/imageid?uniqueTargetId=";
    public static string UI_API = REST_SERVER + "getapp/";
    public static string UI_WorkspaceAPI = REST_SERVER + "getworkspace/";
    public static string GET_APP_API = REST_SERVER + "getapp/";
    public static string GET_Query_Results = REST_SERVER + "getqueryresult/";
    public static string GET_Key_Query_Results = REST_SERVER + "keyqueryresult/";
    public static string TEST_Query = REST_SERVER + "2/testquery";
    public static string floatingVariable_Query = REST_SERVER + "getapp/9db3b24ac41f44da934829cc3e9ed8a5";
    public static string Send_Mail = REST_SERVER + "sendemail";
    public static string Cell_Properties = REST_SERVER + "getCellProperties";

    public static string VUFORIA_UNIQUE_ID;
    public static string PREVIOUS_VUFORIA_UNIQUE_ID;
    public static string AR_PREVIOUS_VUFORIA_UNIQUE_ID;

    public static string INFO_DELIMETER = "INSTANT_AR_TARGET_IMAGE_INFO";
    public static int fontFactor = 6;
    public static string INFO_STRING = "";

    public static bool INFO_BUTTON_CLICKED = false;
    public static bool VIDEO_BUTTON_CLICKED = false;
    public static bool AR_VIDEO_RESET = true;
    public static bool INFO_PANEL_BUTTON_CLICKED = false;

    public static float ASPECT_RATIO = 1.0f;

    public static bool TRACKING_FOUND = false;

    public static bool kickOff = false;
    public static bool isPublic = true;

    public static string initialMobileImagePath;

    public static int healthCounter = 0;

    public static string KEYSIGHT_ASSET;
    public static string CURRENT_KEYSIGHT_ASSET;
    public static string CURRENT_KEYSIGHT_HELATH;

    public static string INFO_JSON_URL;
    public static string INFO_PUBLIC_JSON_URL;
    public static TargetImage targetImageObject;
    public static ArrayList availableButtonNames;
    public static string TARGET_IMAGE_DBX_URL;
    public static AsyncOperation uiScene;
    public static string CURRENT_USER = "jitenderh@gmail.com";
    //public static string CURRENT_USER = "laxsharma79@gmail.com";

    public static List<TargetInfo> Target_Info_Set;
    public static ButtonInfoMAP buttonInfoMAP;
    public static ButtonInfoMAP buttonPublicInfoMAP;
    //public static Dictionary<GameObject, string> popupresponse;
    public static string popupresponse;

    public static List<string> ButtonList;

    public static Dictionary<string, string> ContainerNames;
    public static Dictionary<string, bool> CanvasNames;
    public static string PrimaryImageName;
    public static bool ARScreen_Enable = true;

    public static bool UIScreen_CLICKED = false;

    public static bool LocationMap_CLICKED = false;

}