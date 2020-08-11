
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System;
using System.Collections.Generic;
using Vuforia;
using UnityEngine.Video;
using UnityEngine.Networking;
using ImageAndVideoPicker;
using UnityEngine.UI;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Text;


public partial class ARUtilityTools
{

    private static AuthManager _authManager;

    void Start()
    {
        Debug.Log("<color=red>>>><<<<<<<<<<<<< Inside  start of  ARUtilityTools </color>");
        //nameLabelText = "jjjjjj";
        //showInfoData();
        //addButtons();


    }

    public static string getCurrentUser()
    {
        string userID;
        _authManager = AuthManager.Instance;
        if (_authManager.IsLoggedIn)
        {
            userID = _authManager.CurrentToken.username;
        }
        else
        {
            userID = "Guest User";
        }
        return userID;
    }


    public static void TestExecutionTime()
    {
        string json = "";
        var data = (JObject)JsonConvert.DeserializeObject(json);
        string timeZone = data["Atlantic/Canary"].Value<string>();
        //Jitu calculating time for Rest API
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // CAll some method here

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        Debug.Log("<color=red>  ################### Time Taken for JSON  CALL IS: : </color> " + elapsedMs);

    }

    public static string getTempFromNest()
    {
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Authorization", "Bearer c.xgtAz1T37QadHHMoh6NU1mdMkdaXnc8nyT0jBktQ9JQsCaFjnKiWlgugLhlEh9N2qVCX9Nc8NGXKJ8TrHBFGX8oDRuovQWpu8zoeJDtddYKYJXxcq5L4rRJSnVF290eZeaybh4usyXhL9SHK");
        postHeader.Add("Content-Type", "application/octet-stream");
        WWW www = new WWW("https://developer-api.nest.com/devices/thermostats/8n5Q5wa2RWbRMit8pG28WI9kD-rl8TA7/ambient_temperature_f", null, postHeader);
        while (!www.isDone) { }
        //yield return www;
        string temp = www.text;
        return temp;
    }

    public static List<TargetImage> getTargetImagesFromUserID(string RestServerPath, string userId)
    {
        //string userId = "jitenderh@gmail";
        //string userId = "63FAEB13-8B67-4FAA-8178-1BF2ED5CFE7D";
        //string RestServerPath = "http://localhost:8025/query/user/images";
        var content = "{ \"userID\" : \"" + userId + "\"}";
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");
        var RestResponse = WebFunctions.PostHeader(RestServerPath, encoding.GetBytes(content), headers);
        var targetImageObjects = JsonConvert.DeserializeObject<List<TargetImage>>(RestResponse.text);
        //var targetImageObjects = JsonConvert.DeserializeObject(tagetImages, typeof(List<TargetImage>)) as List<TargetImage>;
        foreach (var obj in targetImageObjects)
        {
            Debug.Log("<color=red> $$$$$$$$$$ obj.targetID is :   </color>" + obj.targetID);
            Debug.Log("<color=red> @@@@@@@@@@ obj.image is :   </color>" + obj.dbxImageURL);
            //Debug.Log("<color=red> @@@@@@@@@@ targetImages[1] is :   </color>" + targetImageObjects[1].targetID);
        }
        return targetImageObjects;

    }

    public static TargetImage getLatestTargetImagesFromUserID(string RestServerPath, string userId, string imageCounter)
    {
        //string userId = "jitenderh@gmail";
        //string userId = "63FAEB13-8B67-4FAA-8178-1BF2ED5CFE7D";
        //string RestServerPath = "http://localhost:8025/query/user/images";
        Debug.Log("<color=red>>>><<<imageCounter in getLatestTargetImagesFromUserID  </color>" + imageCounter);
        var content = "{ \"userID\" : \"" + userId + "\"," +
            "\"latestCounter\" : \"" + imageCounter + "\"}";
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");
        var RestResponse = WebFunctions.PostHeader(RestServerPath, encoding.GetBytes(content), headers);
        var targetImageObject = JsonConvert.DeserializeObject<TargetImage>(RestResponse.text);
        Debug.Log("<color=red>>>><<<targetImageObject in ARUtilities  </color>" + targetImageObject);
        return targetImageObject;
    }


    public static string getTargetImageDbxURL(string RestServerPath, string UniqueTargetId)
    {
        // string UniqueTargetId = "3b5f7f8d9ee94d728f426a47a8f53cfb";
        //string RestServerPath = "http://localhost:8025/query/imageJsonURL";
        var content = "{ \"uniqueTargetID\" : \"" + UniqueTargetId + "\"}";
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");
        var RestResponse = WebFunctions.PostHeader(RestServerPath, encoding.GetBytes(content), headers);
        string jsonURL = RestResponse.text;
        return jsonURL;
    }

    public static Dictionary<string, string> getDbxPathFromDB(String UniqueTargetId, String model_name)
    {
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        String dbxpath;
        String aspect;
        String portrait = "no";
        VuforiaJsonObject vuforiaObject = null;
        vuforiaObject = new VuforiaJsonObject();
        var content = "{ \"uniqueTargetID\" : \"" + UniqueTargetId + "\"}";
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");

        String serverURL = getDBServer(); //JITU: LATER NEED TO BE RETRIEVED BY A QUICKER WAY
        Debug.Log("<color=red> $$$$$$$$$$$$$$$$$$ The Server URL is: </color>" + serverURL);
        string databaseURL = serverURL + "query/imageid";
        Debug.Log("<color=red> $$$$$$$$$$$$$$$$$$ The databaseURL  is: </color>" + databaseURL);
        //var RestResponse = WebFunctions.PostHeader("http://eb7b3804.ngrok.io/query/imageid", encoding.GetBytes(content), headers);
        var RestResponse = WebFunctions.PostHeader(databaseURL, encoding.GetBytes(content), headers);
        string response = RestResponse.text;

        if (string.IsNullOrEmpty(response))
        {
            Debug.Log("<color=blue> $$$$$$$$$$$$$$$$$ Response is NULL  : </color>");
            Debug.Log("<color=blue> @@@@@@@@@@ MYSQL REST API URL WAS NOT FOUND, SO getting values from JSON at DropBox </color>");
            keyValuePairs = getDbxPathFromJson(UniqueTargetId);
        }
        else
        {
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ Response is NOT NULL  : </color>");
            string status500 = null;
            Debug.Log("<color=blue> $$$$$$$$$$$$$$$$$ MYSQL RestResponse.text is  : </color>" + RestResponse.text);
            try
            {
                vuforiaObject = JsonUtility.FromJson<VuforiaJsonObject>(RestResponse.text);
                status500 = (string)vuforiaObject.GetType().GetField("status").GetValue(vuforiaObject);
                if (status500 == null)
                    status500 = "";
                Debug.Log("<color=red> @@@@@@@@@@ status500: </color>" + status500);

                if (vuforiaObject != null && !status500.Contains("500"))
                {
                    Debug.Log("<color=green> @@@@@@@@@@ MYSQL REST API URL WAS  FOUND, SO getting values from Mysql DB </color>");
                    dbxpath = (string)vuforiaObject.GetType().GetField("dbxURL").GetValue(vuforiaObject);
                    aspect = (string)vuforiaObject.GetType().GetField("aspectRatio").GetValue(vuforiaObject);
                    portrait = (string)vuforiaObject.GetType().GetField("portrait").GetValue(vuforiaObject);
                    Debug.Log("<color=blue> >>>>>>>>>> MYSql aspectRatio is  : </color>" + aspect);
                    Debug.Log("<color=blue> @@@@@@>>>>@@@@@@@@@@@@ MYSql dbxpath is  : </color>" + dbxpath);
                    Debug.Log("<color=red> @@@@@@@@@@@@@@@@@@ MYSql portrait is  : </color>" + portrait);
                    if (string.IsNullOrEmpty(dbxpath) || string.IsNullOrEmpty(aspect))
                    {
                        Debug.Log("<color=blue> @@@@@@@@@@ MYSQL REST API could not get values so trying now from DropBox </color>");
                        keyValuePairs = getDbxPathFromJson(UniqueTargetId);
                    }
                    else
                    {
                        keyValuePairs.Add("dbxurl", dbxpath);
                        keyValuePairs.Add("aspectRatio", aspect);
                        keyValuePairs.Add("portrait", portrait);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("<color=red> @@@@@@@@@@ An error occurred: </color>" + e.ToString());
                keyValuePairs = getDbxPathFromJson(UniqueTargetId);
            }
        }
        vuforiaObject = null;
        return keyValuePairs;
    }


    public static String getDBServer()
    {
        //String serverJsonURL = "https://dl.dropbox.com/s/nmti5ri2z9wo3z2/DBServerJason.json";//jitu dbx
        String serverJsonURL = "https://dl.dropbox.com/s/g5evp81o3o9qb7v/DBServerJason.json";//sush dbx

        var serverResponse = WebFunctions.Get(serverJsonURL);
        ServerJsonObject serverObject = JsonUtility.FromJson<ServerJsonObject>(serverResponse.text);
        String serverURL = (string)serverObject.GetType().GetField("DBSERVER").GetValue(serverObject);
        string MasterJsonURL = (string)serverObject.GetType().GetField("MASTERJSON").GetValue(serverObject);
        Debug.Log("<color=blue> @@@@@@@@@@@@@@@@@@ MasterJsonURL is  : </color>" + MasterJsonURL);
        return serverURL;
    }

    public static Dictionary<string, string> getDbxPathFromJson(String UniqueTargetId)
    {
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        var masterResponse = WebFunctions.Get(GlobalVariables.MasterJsonURL);
        var data = (JObject)JsonConvert.DeserializeObject(masterResponse.text);
        string jsonURL = data[UniqueTargetId].Value<string>();


        var jsonResponse = WebFunctions.Get(jsonURL);
        var jsonData = (JObject)JsonConvert.DeserializeObject(jsonResponse.text);

        TargetImage targetImageObject = JsonUtility.FromJson<TargetImage>(jsonResponse.text);

        keyValuePairs.Add("dbxurl", targetImageObject.dbxVideoURL);
        keyValuePairs.Add("aspectRatio", targetImageObject.aspectRatio);
        keyValuePairs.Add("portrait", targetImageObject.videoPortrait);
        return keyValuePairs;

    }

    public static TargetImage getFullTargetImageFromUID(String UniqueTargetId)
    {
        TargetImage targetImageObject = null;
        try
        {
            //string directURL = "https://ar-dev.emachinelabs.com/2/query/imageid?uniqueTargetId=" + UniqueTargetId;

            string directURL = GlobalVariables.REST_SERVER + GlobalVariables.GET_IMAGE_JSON_API + UniqueTargetId;
            var directResponse = WebFunctions.Get(directURL);
            Debug.Log("<color=red> $$$$$$$$$$$$$$$$$ directResponse.text is  : </color>" + directResponse.text);
            targetImageObject = JsonUtility.FromJson<TargetImage>(directResponse.text);

            //Get Buttons =====================================
            string[] buttonStrings = GetJsonObjectArray(directResponse.text, "buttons");
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ buttonStrings.length is  : </color>" + buttonStrings.Length);
            ButtonDetails[] buttonDetails = new ButtonDetails[buttonStrings.Length];
            int i = 0;
            foreach (string jsonObj in buttonStrings)
            {
                buttonDetails[i] = JsonUtility.FromJson<ButtonDetails>(jsonObj);
                String iconValueString = GetJsonObject(jsonObj, "valueParameters");
                IconValueParameters iconValueParameters = null;
                if (!string.IsNullOrEmpty(iconValueString))
                {
                    iconValueParameters = JsonUtility.FromJson<IconValueParameters>(iconValueString);
                    iconValueParameters.iconHeaders = GetJsonObject(iconValueString, "iconHeaders");
                    iconValueParameters.iconBody = GetJsonObject(iconValueString, "iconBody");
                    iconValueParameters.parameters = GetJsonObject(iconValueString, "params");
                }
                buttonDetails[i].valueParameters = iconValueParameters;

                i++;
            }
            targetImageObject.buttons = buttonDetails;
            //===================================================


            //Get Videos =====================================
            string[] VideoStrings = GetJsonObjectArray(directResponse.text, "videos");
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ VideoStrings.length is  : </color>" + VideoStrings.Length);
            VideoData[] videos = new VideoData[VideoStrings.Length];
            i = 0;
            foreach (string jsonObj in VideoStrings)
            {
                videos[i] = JsonUtility.FromJson<VideoData>(jsonObj);
                i++;
            }
            targetImageObject.videos = videos;
            //===================================================

            //Get Documents =====================================
            string[] DocStrings = GetJsonObjectArray(directResponse.text, "docs");
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ DocStrings.length is  : </color>" + DocStrings.Length);
            DocumentData[] docs = new DocumentData[DocStrings.Length];
            i = 0;
            foreach (string jsonObj in DocStrings)
            {
                docs[i] = JsonUtility.FromJson<DocumentData>(jsonObj);
                i++;
            }
            targetImageObject.docs = docs;
            //===================================================


            //Get Notes =====================================
            string[] NotesStrings = GetJsonObjectArray(directResponse.text, "notes");
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ NotesStrings.length is  : </color>" + NotesStrings.Length);
            NotesData[] notes = new NotesData[NotesStrings.Length];
            i = 0;
            foreach (string jsonObj in NotesStrings)
            {
                notes[i] = JsonUtility.FromJson<NotesData>(jsonObj);
                i++;
            }
            targetImageObject.notes = notes;
            //===================================================


            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ targetImageObject.dbxImageURL is  : </color>" + targetImageObject.dbxImageURL);
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ buttons.length is  : </color>" + buttonDetails.Length);
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ videos.length is  : </color>" + videos.Length);
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ docs.length is  : </color>" + docs.Length);
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ notes.length is  : </color>" + notes.Length);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in getFullTargetImageFromUID: </color>" + e.ToString());
        }

        return targetImageObject;
    }

    //JITU-- TO BE DEPRECATED
    public static TargetImage getTargetImageFromUID(String UniqueTargetId)
    {
        TargetImage targetImageObject = null;
        try
        {
            var masterResponse = WebFunctions.Get(GlobalVariables.MasterJsonURL);
            var data = (JObject)JsonConvert.DeserializeObject(masterResponse.text);
            string jsonURL = data[UniqueTargetId].Value<string>();
            var jsonResponse = WebFunctions.Get(jsonURL);
            Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ jsonResponse.text from  is  : </color>" + jsonResponse.text);
            targetImageObject = JsonUtility.FromJson<TargetImage>(jsonResponse.text);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in getTargetImageFromUID: </color>" + e.ToString());
        }
        return targetImageObject;
    }

    public static string getLatestImage(int imageCounter)
    {
        string userID;
        _authManager = AuthManager.Instance;
        if (_authManager.IsLoggedIn)
        {
            userID = _authManager.CurrentToken.username;
            userID = userID.Substring(0, userID.LastIndexOf("."));

        }
        else
        {
            return null;
        }
        string RestServerPath = GlobalVariables.REST_SERVER + "query/user/latestimage";

        TargetImage targetImageObject = getLatestTargetImagesFromUserID(RestServerPath, userID, imageCounter.ToString());
        if (targetImageObject != null)
        {
            //GlobalVariables.targetImageObject = targetImageObject;
            return targetImageObject.dbxImageURL;
        }
        else
        {
            return null;
        }
    }

    public static void initializeLimitedVariables()
    {
        GlobalVariables.buttonInfoMAP = new ButtonInfoMAP();
        GlobalVariables.buttonPublicInfoMAP = new ButtonInfoMAP();
        GlobalVariables.availableButtonNames = getavailableButtonNames(GlobalVariables.buttonPublicInfoMAP);
    }

    public static void initializeGlobalVariables(TargetImage targetImageObject)
    {
        GlobalVariables.targetImageObject = targetImageObject;
        GlobalVariables.VUFORIA_UNIQUE_ID = targetImageObject.uniqueID;
        GlobalVariables.TARGET_IMAGE_DBX_URL = targetImageObject.dbxImageURL;
        GlobalVariables.ASPECT_RATIO = float.Parse(targetImageObject.aspectRatio);
        GlobalVariables.buttonInfoMAP = getButtonInfoMap(targetImageObject, false);
        GlobalVariables.buttonPublicInfoMAP = getButtonInfoMap(targetImageObject, true);
        GlobalVariables.INFO_JSON_URL = targetImageObject.infoDataKey;
        GlobalVariables.INFO_PUBLIC_JSON_URL = targetImageObject.infoDataPublicKey;
        GlobalVariables.UI_API=GlobalVariables.REST_SERVER + "getapp/" + GlobalVariables.VUFORIA_UNIQUE_ID;

        GlobalVariables.availableButtonNames = getavailableButtonNames(GlobalVariables.buttonPublicInfoMAP);

        Debug.Log("<color=red> $$$$$$$$$$  initializeGlobalVariables  </color>");
        Debug.Log("<color=red> ##### GlobalVariables.VUFORIA_UNIQUE_ID  </color>" + GlobalVariables.VUFORIA_UNIQUE_ID);
        Debug.Log("<color=red> ##### GlobalVariables.TARGET_IMAGE_DBX_URL  </color>" + GlobalVariables.TARGET_IMAGE_DBX_URL);
        Debug.Log("<color=red> ####### GlobalVariables.ASPECT_RATIO  </color>" + GlobalVariables.ASPECT_RATIO);
        Debug.Log("<color=red> ##### GlobalVariables.INFO_JSON_URL  </color>" + GlobalVariables.INFO_JSON_URL);

    }
    public static ArrayList getavailableButtonNames(ButtonInfoMAP buttonPublicInfoMAP)
    {
        ArrayList nameList = new ArrayList();
        string button_from_json = "";
        for (int i = 0; i < 100; i++)
        {
            string tempName = "Button_" + i.ToString();
            //Debug.Log("<color=red>>>><<<The temp Button Name is :  </color>" + tempName);
            if (buttonPublicInfoMAP != null)
            {
                button_from_json = (string)buttonPublicInfoMAP.GetType().GetField(tempName).GetValue(buttonPublicInfoMAP);
            }
            //Debug.Log("<color=red>>>><<<The infoJsonString is :  </color>" + button_from_json);
            if (string.IsNullOrEmpty(button_from_json))
            {
                nameList.Add(tempName);
                //break;
            }
        }
        //nameList.Sort();
        return nameList;


    }
    public static ButtonInfoMAP getButtonInfoMap(TargetImage targetImageObject, bool isPublic)
    {
        ButtonInfoMAP buttonInfoMAP;
        string infoJsonUrl = "";
        if (isPublic)
        {
            infoJsonUrl = targetImageObject.infoDataPublicKey;
        }
        else
        {
            infoJsonUrl = targetImageObject.infoDataKey;
        }

        var infoUrlResponse = WebFunctions.Get(infoJsonUrl);
        Debug.Log("<color=red>>>><<<infoJsonUrl :  </color>" + infoJsonUrl);
        Debug.Log("<color=red>>>><<<The ARUtility->getButtonInfoMap->infoUrlResponse is :  </color>" + infoUrlResponse.text);

        if (infoUrlResponse.text.Length < 10) // Its the first time its built so it comes as empty
        {
            //buttonInfoMAP = null;
            buttonInfoMAP = new ButtonInfoMAP();
        }
        else
        {
            buttonInfoMAP = JsonUtility.FromJson<ButtonInfoMAP>(infoUrlResponse.text);

        }
        return buttonInfoMAP;
    }

    public static ButtonInfoMAP getInitialButtonInfoMap()
    {
        ButtonInfoMAP buttonInfoMAP;
        string infoJsonUrl = "";

        var infoUrlResponse = WebFunctions.Get(infoJsonUrl);
        Debug.Log("<color=red>>>><<<The ARUtility->getButtonInfoMap->infoUrlResponse is :  </color>" + infoUrlResponse.text);
        if (infoUrlResponse.text.Length < 10) // Its the first time its built so it comes as empty
        {
            //buttonInfoMAP = null;
            buttonInfoMAP = new ButtonInfoMAP();
        }
        else
        {
            buttonInfoMAP = JsonUtility.FromJson<ButtonInfoMAP>(infoUrlResponse.text);

        }
        return buttonInfoMAP;
    }


    public static void updateInfoButtonPoition(GameObject infoButton, string button_position_x, string button_position_y, bool isPublic)
    {
        ButtonInfoMAP buttonInfoMAP;
        string tempButtonInfo = "";
        if (isPublic)
        {
            buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
        }
        else
        {
            buttonInfoMAP = GlobalVariables.buttonInfoMAP;
        }
        if (buttonInfoMAP == null)
        {
            return;
        }

        tempButtonInfo = (string)buttonInfoMAP.GetType().GetField(infoButton.name).GetValue(buttonInfoMAP);

        if (!string.IsNullOrEmpty(tempButtonInfo))
        {
            TargetInfo targetInfo = JsonUtility.FromJson<TargetInfo>(tempButtonInfo);
            targetInfo.buttonPosition_x = button_position_x;
            targetInfo.buttonPosition_y = button_position_y;
            buttonInfoMAP.GetType().GetField(infoButton.name).SetValue(buttonInfoMAP, JsonUtility.ToJson(targetInfo));
            if (isPublic)
            {
                GlobalVariables.buttonPublicInfoMAP = buttonInfoMAP;
            }
            else
            {
                GlobalVariables.buttonInfoMAP = buttonInfoMAP;
            }
        }
    }


    public static TargetInfo getTargetInfoByButtonName(string buttonName, bool isPublic)
    {
        ButtonInfoMAP buttonInfoMAP;
        TargetInfo targetInfo = null;
        string tempButtonInfo = "";
        if (isPublic)
        {
            buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
        }
        else
        {
            buttonInfoMAP = GlobalVariables.buttonInfoMAP;
        }
        if (buttonInfoMAP == null)
        {
            return null;
        }

        tempButtonInfo = (string)buttonInfoMAP.GetType().GetField(buttonName).GetValue(buttonInfoMAP);

        if (!string.IsNullOrEmpty(tempButtonInfo))
        {
            targetInfo = JsonUtility.FromJson<TargetInfo>(tempButtonInfo);
        }
        return targetInfo;
    }

    /*
    public void refreshButtons()
    {
        StartCoroutine(refreshButtonsFromServer());
    }

        public  IEnumerator refreshButtonsFromServer()
    {

        Debug.Log("<color=green> =================== REFERESHING DATA ================================== </color>");
        if (!string.IsNullOrEmpty(GlobalVariables.VUFORIA_UNIQUE_ID))
        {
            string url = GlobalVariables.REST_SERVER + "query/user/refereshbuttons";
            WWWForm form = new WWWForm();
            form.AddField("UniqueID", GlobalVariables.VUFORIA_UNIQUE_ID);
            UnityWebRequest www1 = UnityWebRequest.Post(url, form);
            www1.SetRequestHeader("UniqueID", GlobalVariables.VUFORIA_UNIQUE_ID);
            yield return www1.SendWebRequest();

            if (www1.isNetworkError || www1.isHttpError)
            {
                Debug.Log("<color=red> ========= ISSUE IN REFERESHING DATA ====== </color>" + www1.error);
            }
            else
            {
                Debug.Log("<color=green> ========= REFRESHED DATA SUCCESSFULLY ====== </color>");
            }

            Debug.Log("<color=green> =================== REFERESHING DONE ================================== </color>");
        }
        else
        {
            Debug.Log("<color=red> ========= NO uniqueTargetId so no referesh====== </color>");

        }

    }
    */


    public static string GetJsonObject(string jsonString, string handle)
    {
        string pattern = "\"" + handle + "\"\\s*:\\s*\\{";

        Regex regx = new Regex(pattern);

        Match match = regx.Match(jsonString);

        if (match.Success)
        {
            int bracketCount = 1;
            int i;
            int startOfObj = match.Index + match.Length;
            for (i = startOfObj; bracketCount > 0; i++)
            {
                if (jsonString[i] == '{') bracketCount++;
                else if (jsonString[i] == '}') bracketCount--;
            }
            return "{" + jsonString.Substring(startOfObj, i - startOfObj);
        }

        //no match, return null
        return null;
    }

    public static string[] GetJsonObjects(string jsonString, string handle)
    {
        string pattern = "\"" + handle + "\"\\s*:\\s*\\{";

        Regex regx = new Regex(pattern);

        //check if there's a match at all, return null if not
        if (!regx.IsMatch(jsonString)) return null;

        List<string> jsonObjList = new List<string>();

        //find each regex match
        foreach (Match match in regx.Matches(jsonString))
        {
            int bracketCount = 1;
            int i;
            int startOfObj = match.Index + match.Length;
            for (i = startOfObj; bracketCount > 0; i++)
            {
                if (jsonString[i] == '{') bracketCount++;
                else if (jsonString[i] == '}') bracketCount--;
            }
            jsonObjList.Add("{" + jsonString.Substring(startOfObj, i - startOfObj));
        }

        return jsonObjList.ToArray();
    }

    public static string[] GetJsonObjectArray(string jsonString, string handle)
    {
        string pattern = "\"" + handle + "\"\\s*:\\s*\\[\\s*{";

        Regex regx = new Regex(pattern);

        List<string> jsonObjList = new List<string>();

        Match match = regx.Match(jsonString);

        if (match.Success)
        {
            int squareBracketCount = 1;
            int curlyBracketCount = 1;
            int startOfObjArray = match.Index + match.Length;
            int i = startOfObjArray;
            while (true)
            {
                if (jsonString[i] == '[') squareBracketCount++;
                else if (jsonString[i] == ']') squareBracketCount--;

                int startOfObj = i;
                for (i = startOfObj; curlyBracketCount > 0; i++)
                {
                    if (jsonString[i] == '{') curlyBracketCount++;
                    else if (jsonString[i] == '}') curlyBracketCount--;
                }
                jsonObjList.Add("{" + jsonString.Substring(startOfObj, i - startOfObj));

                // continue with the next array element or return object array if there is no more left
                while (jsonString[i] != '{')
                {
                    if (jsonString[i] == ']' && squareBracketCount == 1)
                    {
                        return jsonObjList.ToArray();
                    }
                    i++;
                }
                curlyBracketCount = 1;
                i++;
            }
        }

        //no match, return null
        return jsonObjList.ToArray();
    }



    public static UnityEngine.WWW setLocation(string objectID, string location)
    {
        try
        {
            string url = GlobalVariables.REST_SERVER + "setlocation";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            string param = "{\"objectID\":\"" + objectID + "\",\"location\":\"" + location + "\"}";
            byte[] body = Encoding.UTF8.GetBytes(param);
            UnityEngine.WWW www = new UnityEngine.WWW(url, body, headers);
            return www;
        }
        catch (Exception)
        {
            return null;
        }
    }


}
