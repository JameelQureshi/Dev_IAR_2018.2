using System;

[Serializable]
public class GlobalVariablesPoJo
{
    public string DROPBOX_TOCKEN = "Bearer 4xBkAhkRvGAAAAAAAAAFnP-WOrtd-lnl0_UIo9N1L1qEHExWywUuWZWvbQdaMDDm";
    //==================== Dev DBX  ====================================
    public string MasterJsonURL = "https://dl.dropbox.com/s/8vkvfurw5deia89/MasterJason.json";
    public string DROPBOX_STAGING_FOLDER = "/dev/CoolARApp/StagingArea/";
    public string DROPBOX_EXPERIENCE_FOLDER_INFO = "/dev/CoolARApp/TargetExperiences/Info/";
    //public string REST_SERVER = "https://ar-dev.emachinelabs.com/";
    public string REST_SERVER = "http://dropbox-env.8xe8tpevpj.us-west-1.elasticbeanstalk.com/";
    public string GET_IMAGE_JSON_API = "2/query/imageid?uniqueTargetId=";

    //public string UI_API = "https://ar-dev.emachinelabs.com/getapp/";
    public string UI_API = "http://dropbox-env.8xe8tpevpj.us-west-1.elasticbeanstalk.com/";

    public string VUFORIA_UNIQUE_ID; // = "64b61f343fc346339ab14897dc2a9af7";

    public string INFO_DELIMETER = "INSTANT_AR_TARGET_IMAGE_INFO";
    //public string INFO_STRING = "";

    public bool INFO_BUTTON_CLICKED = false;
    public bool VIDEO_BUTTON_CLICKED = false;
    public bool INFO_PANEL_BUTTON_CLICKED = false;

    public float ASPECT_RATIO = 1.0f;

    public bool TRACKING_FOUND = false;

    public bool kickOff = false;
    public bool isPublic = true;

    public int healthCounter = 0;
    public int fontFactor = 6;
}