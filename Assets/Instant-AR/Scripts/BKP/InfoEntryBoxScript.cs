/*============================================================================== 
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using System;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using UnityEngine.Video;
using UnityEngine.Networking;
using ImageAndVideoPicker;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;

public class InfoEntryBoxScript : MonoBehaviour
{
    public Text infoText;
    private static GameObject parentButton;
    private string userID;
    public InputField mainInputField;

    void Start()
    {
        AuthManager _authManager = AuthManager.Instance;
        if (_authManager.IsLoggedIn)
        {
            userID = _authManager.CurrentToken.username;
            userID = userID.Substring(0, userID.LastIndexOf("."));

        }
        else
        {
            userID = "public";
        }
    }


    public static void DisplayInfoEntryBox(GameObject button)
    {
        Debug.Log("<color=red>>>><<<Inside InfoEntryBox  </color>" + button.name);
        parentButton = button;

        GameObject prefab = (GameObject)Resources.Load("InfoEntryBox");
        if (prefab)
        {
            InfoEntryBoxScript messageBox = Instantiate(prefab.GetComponent<InfoEntryBoxScript>());
            Debug.Log("<color=red>>>><<< AGAIN Inside InfoEntryBox  </color>" + button.name);
            messageBox.Setup(button);
            //Text[] textComponents = GetComponentsInChildren<Text>();
            //infoText.text = "jituuuuuu";

        }
    }

    public void Setup(GameObject button)
    {
        string inputText = getInputText(button);
        mainInputField.text = inputText;

    }

    public string getInputText(GameObject newButton)
    {
        Debug.Log("<color=red>>>><<<Inside  getInputText, newButton name is :  </color>" + newButton.name);
        string inputText = "";
        //ButtonInfoMAP buttonInfoMAP = GlobalVariables.buttonInfoMAP;
        ButtonInfoMAP buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
        if (buttonInfoMAP == null)
        {
            return "";
        }
        string infoJsonString = (string)buttonInfoMAP.GetType().GetField(newButton.name).GetValue(buttonInfoMAP);
        Debug.Log("<color=red>>>><<<The infoJsonString is :  </color>" + infoJsonString);
        if (!string.IsNullOrEmpty(infoJsonString))
        {
            TargetInfo targetInfo = JsonUtility.FromJson<TargetInfo>(infoJsonString);
            inputText = targetInfo.string_value3;
        }
        else
        {
            inputText = "";
        }
        return inputText;


    }


    public void SubmitButton()
    {
        //getSpotIdentity();
        TargetInfo targetInfo = ARUtilityTools.getTargetInfoByButtonName(parentButton.name, true);
        if(targetInfo == null){
            targetInfo = captureInfo(parentButton);
        }
        targetInfo.string_value3 = infoText.text;

        //String filePath = GlobalVariables.DROPBOX_EXPERIENCE_FOLDER_INFO + userID + "/" + GlobalVariables.VUFORIA_UNIQUE_ID + ".json";
        //Debug.Log("<color=red>>>><<<The filePath  is :  </color>" + filePath);
        //string infoJsonUrl = "https://dl.dropbox.com/s/xfcezdxuujlw261/bc3ca485bf9448dc9fecc8c2e987e459.json";
        string infoJsonUrl = GlobalVariables.INFO_JSON_URL;
        var infoUrlResponse = WebFunctions.Get(infoJsonUrl);
        //ButtonInfoMAP buttonInfoMAP=GlobalVariables.buttonInfoMAP;
        ButtonInfoMAP buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
        if (buttonInfoMAP == null)
        {
            buttonInfoMAP = new ButtonInfoMAP();
        }
        updateInfo(buttonInfoMAP, targetInfo);
        //ChangeSprite();
        Destroy(gameObject);
    }

    public void CloseButton()
    {

        Destroy(gameObject);
    }

    public TargetInfo captureInfo(GameObject infoButton)
    {
        Debug.Log("<color=red> $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$Inside captureInfo");
        TargetInfo targetInfo = new TargetInfo();
        InfoValueParam valueParam = new InfoValueParam();
        targetInfo.buttonID = infoButton.name;
        //targetInfo.string_value3 = infoText.text;

        DragDrop buttonScript = infoButton.GetComponent<DragDrop>();
        targetInfo.button_sprite = buttonScript.button_sprite;
        targetInfo.buttonPosition_x = buttonScript.button_position_x;
        targetInfo.buttonPosition_y = buttonScript.button_position_y;
        targetInfo.string_value1 = "JITU TESTING STRING1";
        targetInfo.string_value2 = "JITU TESTING STRING2";
        targetInfo.prefix = "test_prefix";
        targetInfo.suffix = "test_suffix";
        targetInfo.label = "test_label";

        valueParam.url = "https://developer-api.nest.com";
        valueParam.method = "POST";
        valueParam.iconHeaders = "";
        valueParam.iconBody = "";


        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");
        postHeader.Add("Authorization", "Bearer c.T38gGm7ankaRBhJTAriQr7LHrjbAWj7W8R0TGd0nE8qq66g3Ewd9H2s39GNrfAoelqSfPSTmxTXedrktj2VCaTxyHQR4m0aiaQ7ocKI5pIEAME4c9puDVmAs8LaPmrnSWbsu3PAfm6rKWlG2");
        valueParam.iconHeaders = getJsonStringFromDictionary(postHeader);

        Dictionary<string, string> postBody = new Dictionary<string, string>();
        postBody.Add("bodykey1", "part1");
        postBody.Add("bodykey2", "part2");
        valueParam.iconBody = getJsonStringFromDictionary(postBody);

        targetInfo.valueParameters = JsonUtility.ToJson(valueParam);
        Debug.Log("<color=red> $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$The valueParameters is :  </color>" + JsonUtility.ToJson(valueParam));


        return targetInfo;

    }
    public string getJsonStringFromDictionary(Dictionary<string, string> param)
    {
        string jsonString = "{";
        string endString = "}";
        foreach (var item in param)
        {
            jsonString = jsonString + "\"" + item.Key + "\":\"" + item.Value + "\",";
        }
        jsonString = jsonString.Substring(0, jsonString.Length - 1);
        jsonString = jsonString + endString;

        return jsonString;
    }



    public void ChangeSprite()
    {
        Button button = parentButton.GetComponent<Button>();
        var grandChild = parentButton.transform.GetChild(1).gameObject;
        Debug.Log("<color=red>>>><<<The grandChild name is :  </color>" + grandChild);
        grandChild.transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
        //button.GetComponent<Button>().interactable = false;

    }


    private void updateInfo(ButtonInfoMAP buttonInfoMAP, TargetInfo targetInfo)
    {

        Debug.Log("<color=red>>>><<<The parentButton.name  is :  </color>" + parentButton.name);
        Debug.Log("<color=red>>>><<<The targetInfo.string_value3  is :  </color>" + targetInfo.string_value3);
        buttonInfoMAP.GetType().GetField(parentButton.name).SetValue(buttonInfoMAP, JsonUtility.ToJson(targetInfo));
        Debug.Log("<color=red>>>><<<The JsonUtility.ToJson(targetInfo)  is :  </color>" + JsonUtility.ToJson(targetInfo));

        //GlobalVariables.buttonInfoMAP = buttonInfoMAP;
        GlobalVariables.buttonPublicInfoMAP = buttonInfoMAP;
        //StartCoroutine(uploadTargetImageJson(JsonUtility.ToJson(buttonInfoMAP), filePath));

    }

    private IEnumerator uploadTargetImageJson(String newTargetImage, String filepath)
    {


        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        String param = "{\"path\": \"" + filepath + "\",\"mode\": \"overwrite\",\"autorename\": false,\"mute\": false}";
        postHeader.Add("Authorization", GlobalVariables.DROPBOX_TOCKEN);
        postHeader.Add("Dropbox-API-Arg", param);
        postHeader.Add("Content-Type", "application/octet-stream");
        byte[] myData = Encoding.ASCII.GetBytes(newTargetImage);
        //byte[] myData = File.ReadAllBytes(sourcePath);
        WWW www = new WWW("https://content.dropboxapi.com/2/files/upload", myData, postHeader);
        yield return www;
    }





    //==========

    public void getSpotIdentity()
    {
        ImageMapButtonScript buttonScript = parentButton.GetComponent<ImageMapButtonScript>();

        float xPos = buttonScript.xPos;
        float yPos = buttonScript.yPos;
        float rowCount = 10;
        float xFact = xPos / rowCount;
        float yFact = yPos / rowCount;
        string buttonPosition = xFact + " x " + yFact;
        Debug.Log("<color=red>>>><<<INFO PART1: Buton position is :  </color>" + buttonPosition);
        Debug.Log("<color=red>>>><<<INFO PART2 Text Entered is position is :  </color>" + infoText.text);
        Debug.Log("<color=red>>>><<<INFO PART3 Image ID:  </color>" + GlobalVariables.VUFORIA_UNIQUE_ID);
        Debug.Log("<color=red> ======================================>>>  </color>");

    }




}
