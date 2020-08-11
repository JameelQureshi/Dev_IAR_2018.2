/*==============================================================================
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.

Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
==============================================================================*/
using UnityEngine;
using System;
using Vuforia;
using UnityEngine.Video;
using UnityEngine.Networking;
using ImageAndVideoPicker;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class JasonTest : MonoBehaviour
{
    private static String myToken = "Bearer 4xBkAhkRvGAAAAAAAAAFnP-WOrtd-lnl0_UIo9N1L1qEHExWywUuWZWvbQdaMDDm";
    private String targetFolder = "/sush/CoolARApp/StagingArea/";

    void Start()
    {
        Debug.Log("<color=red>  $$$$$$$$$$$$$$$$ I am inside JasonTest:   </color>");
        //getTagetImageInfo();
        //getDBServer();
        string dbxJasonUrl = "https://dl.dropbox.com/s/aher54w2hel1nj1/TestJason.json";
        //string dbxJasonUrl2 = "https://dl.dropbox.com/s/80upx2o1opbn5yt/TestJason2.json";
        string dbxJasonUrl3 = "https://dl.dropbox.com/s/6sk6bz3igbj8bl7/2e69a19906c8430e9fac9a931a0aab41.json";

        //getTargetInfo(dbxJasonUrl3);
        //newtonTest(dbxJasonUrl3);
        //testRestApi();

        //string UniqueTargetId = "3b5f7f8d9ee94d728f426a47a8f53cfb";
        //string RestServerPath = "http://localhost:8025/query/imageJsonURL";
        //string jsonURL = ARUtilityTools.getTargetImageDbxURL(RestServerPath, UniqueTargetId);
        //Debug.Log("<color=red> $$$$$$ AGAIN jsonURL IS :   </color>" + jsonURL);

        //testUserRestApi();
        /*
        string userId = "63FAEB13-8B67-4FAA-8178-1BF2ED5CFE7D";
        string RestServerPath = "http://localhost:8025/query/user/imageid";
        List<TargetImage> list = ARUtilityTools.getTargetImagesFromUserID(RestServerPath, userId);
        Debug.Log("<color=red> ===============================  </color>");
        foreach (var obj in list)
        {
            Debug.Log("<color=green> @@@@@@@@@@ obj.targetID is :   </color>" + obj.fileName);
        }
        */
        String filePath = targetFolder + "JituTestInfo.json";
        TargetInfo targetInfo = new TargetInfo();
        targetInfo.buttonID = "Superb DONE";
        targetInfo.string_value3 = "AWESOME , ITS WORKING";
        updateInfo("Button_1x1", targetInfo, filePath);
    }

    private void updateInfo(string buttonName, TargetInfo targetInfo, string filePath)
    {
        //String filePath = targetFolder + fileName;
        //TargetInfo targetInfo = new TargetInfo();
        //targetInfo.buttonID = "kkkk";
        //targetInfo.string_value3 = "jitu just added this.OKKK";
        //string fileName ="testinfo.json";
        ButtonInfoMAP buttonInfoMAP = new ButtonInfoMAP();
        buttonInfoMAP.GetType().GetField(buttonName).SetValue(buttonInfoMAP, JsonUtility.ToJson(targetInfo));

        //buttonInfoMAP.Button_10x1 = JsonUtility.ToJson(targetInfo);
        //Debug.Log("<color=red>  Finally the revised targetImage   </color>" + JsonUtility.ToJson(targetImage));
        StartCoroutine(uploadTargetImageJson(JsonUtility.ToJson(buttonInfoMAP), filePath));

    }


    public void newtonTest(string targetImageURL)
    {
        targetImageURL = "file:///Users/jitenderhooda/Downloads/1Jason1.json";
        string originalJasonString = WebFunctions.Get(targetImageURL).text;
        TargetImage targetImage = JsonUtility.FromJson<TargetImage>(originalJasonString);

        var data = (JObject)JsonConvert.DeserializeObject(originalJasonString);


        string uid = data["targetImage_UniqueID"].Value<string>();
        Debug.Log("<color=red> @@@@@@@@@@ UID IS :   </color>" + uid);

        /*
        Debug.Log("<color=red>  dbxVideoURLPath IS :   </color>" + targetImage.dbxVideoURLPath);

        string infoSetString = data["AR_INFO"].Value<string>();
        if (!string.IsNullOrEmpty(infoSetString))
        {
            JArray obj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(infoSetString);
            foreach (var result in obj)
            {
                string value1 = result["string_value1"].Value<string>();
                Debug.Log("<color=red>  value1 IS :   </color>" + value1);
            }
        }
        else{
            Debug.Log("<color=red>  infoSetString IS NULL :   </color>");
        }
        if(string.IsNullOrEmpty(targetImage.AR_INFO)){
            Debug.Log("<color=red>  YES..infoSetString IS NULL :   </color>");
        }
        */


    }




    public void getTargetInfo(string targetImageURL)
    {
        //String targetImageURL = "file:///Users/jitenderhooda/Downloads/1Jason1.json";
        string originalJasonString = WebFunctions.Get(targetImageURL).text;
        // Debug.Log("<color=red>  serverResponse:   </color>" + serverResponse.text);
        TargetImage targetImage = JsonUtility.FromJson<TargetImage>(originalJasonString);
        //Debug.Log("<color=red>  targetImage.AR_INFO:   </color>" + targetImage.AR_INFO);

        List<TargetInfo> infoObjectList = new List<TargetInfo>();

        List<string> infoStringList = new List<string>(targetImage.infoDataPublicKey.Split(new string[] { GlobalVariables.INFO_DELIMETER }, StringSplitOptions.RemoveEmptyEntries));

        TargetInfo targetInfo;

        foreach (string element in infoStringList)
        {
            //Debug.Log("<color=red>  for loop value, element :   </color>" + element);
            try
            {
                targetInfo = JsonUtility.FromJson<TargetInfo>(element);
                infoObjectList.Add(targetInfo);
            }
            catch (Exception e)
            {
                Debug.Log("<color=red> @@@@@@@@@@ An error occurred: </color>" + e.ToString());
            }
        }
        add_or_delete(infoObjectList);


        string newTargetInfoString = string.Empty;
        int counter = 100;
        Debug.Log("<color=red>  The number of Info Objects are :   </color>" + infoObjectList.Count);
        foreach (var target in infoObjectList)
        {
            counter = counter + 100;
            target.string_value3 = target.string_value3 + "__" + counter;
            Debug.Log("<color=red>  string_value3:   </color>" + target.string_value3);
            newTargetInfoString = newTargetInfoString + JsonUtility.ToJson(target) + GlobalVariables.INFO_DELIMETER;
        }
        string filePath = targetImage.uniqueID + ".json";
        targetImage.infoDataPublicKey = newTargetInfoString;
        //Debug.Log("<color=red>  Finally the revised targetImage   </color>" + JsonUtility.ToJson(targetImage));
        StartCoroutine(uploadTargetImageJson(JsonUtility.ToJson(targetImage), filePath));

    }

    private void add_or_delete(List<TargetInfo> infoObjectList)
    {
        TargetInfo targetInfo = new TargetInfo();
        targetInfo.buttonID = "kkkk";
        targetInfo.string_value3 = "jitu just added this.OKKK";
        infoObjectList.Add(targetInfo);

    }

    private IEnumerator uploadTargetImageJson(String newTargetImage, String filepath)
    {


        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        String param = "{\"path\": \"" + filepath + "\",\"mode\": \"overwrite\",\"autorename\": false,\"mute\": false}";
        postHeader.Add("Authorization", myToken);
        postHeader.Add("Dropbox-API-Arg", param);
        postHeader.Add("Content-Type", "application/octet-stream");
        byte[] myData = Encoding.ASCII.GetBytes(newTargetImage);
        //byte[] myData = File.ReadAllBytes(sourcePath);
        WWW www = new WWW("https://content.dropboxapi.com/2/files/upload", myData, postHeader);
        yield return www;
    }


    public void getTagetImageInfo()
    {
        //String serverJsonURL = "https://dl.dropbox.com/s/g5evp81o3o9qb7v/DBServerJason.json";//sush dbx
        //string dbxJasonUrl = "https://dl.dropbox.com/s/aher54w2hel1nj1/TestJason.json";

        String targetImageURL = "file:///Users/jitenderhooda/Downloads/1Jason.json";
        var serverResponse = WebFunctions.Get(targetImageURL);
        //Debug.Log("<color=red>  serverResponse:   </color>" + serverResponse.text);

        //string[] targetImages = serverResponse.text.Split(',');
        //foreach (string value in values)
        //{
        //}

        TargetImage targetImage = JsonUtility.FromJson<TargetImage>(serverResponse.text);

        TargetInfo targetInfo = JsonUtility.FromJson<TargetInfo>(targetImage.infoDataPublicKey);

        //string output = JsonUtility.ToJson(targetImage);
        Debug.Log("<color=red>  button_sprite:   </color>" + targetInfo.button_sprite);

        string output = JsonUtility.ToJson(targetInfo);
        Debug.Log("<color=red>  output:   </color>" + output);

        List<TargetInfo> infoList = new List<TargetInfo>();
        infoList.Add(targetInfo);
        foreach (var target in infoList)
        {
            Debug.Log("<color=red>  string_value3:   </color>" + target.string_value3);

        }

    }


    public void getDBServer()
    {
        //String serverJsonURL = "https://dl.dropbox.com/s/g5evp81o3o9qb7v/DBServerJason.json";//sush dbx
        String targetImageURL = "file:///Users/jitenderhooda/Downloads/1Jason1.json";
        var serverResponse = WebFunctions.Get(targetImageURL);
        // Debug.Log("<color=red>  serverResponse:   </color>" + serverResponse.text);

        TargetImage targetImage = JsonUtility.FromJson<TargetImage>(serverResponse.text);
        //Debug.Log("<color=red>  targetImage.AR_INFO:   </color>" + targetImage.AR_INFO);

        List<TargetInfo> infoObjectList = new List<TargetInfo>();
        List<string> infoStringList = new List<string>(targetImage.infoDataPublicKey.Split(new string[] { GlobalVariables.INFO_DELIMETER }, StringSplitOptions.RemoveEmptyEntries));
        TargetInfo targetInfo;
        foreach (string element in infoStringList)
        {
            Debug.Log("<color=red>  for loop value, element :   </color>" + element);
            try
            {
                targetInfo = JsonUtility.FromJson<TargetInfo>(element);
                infoObjectList.Add(targetInfo);
            }
            catch (Exception e)
            {
                Debug.Log("<color=red> @@@@@@@@@@ An error occurred: </color>" + e.ToString());
            }
        }
        string newTargetInfoString = string.Empty;
        foreach (var target in infoObjectList)
        {
            Debug.Log("<color=red>  string_value3:   </color>" + target.string_value3);
            newTargetInfoString = newTargetInfoString + JsonUtility.ToJson(target) + GlobalVariables.INFO_DELIMETER;
        }

        targetImage.infoDataPublicKey = newTargetInfoString;
        Debug.Log("<color=red>  Finally the revised targetImage   </color>" + JsonUtility.ToJson(targetImage));

    }


    ////main class end
}


public class TargetImageTest
{
    public string targetImage_dbxPath { get; set; }
    public string targetImage_serverPath { get; set; }
    public string targetImage_height { get; set; }
    public string targetImage_aspectRatio { get; set; }
    public string targetImage_portrait { get; set; }
    public string targetImage_UniqueID { get; set; }
    public string defaultView { get; set; }
    public string experience_type { get; set; }
    public string video_portrait { get; set; }
    public string targetImage_FileName { get; set; }
    public string dbxVideoURL { get; set; }
    public string targetImage_Width { get; set; }
    public string AR_INFO { get; set; }

}







