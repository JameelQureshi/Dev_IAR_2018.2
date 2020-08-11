/*
 * ============================================================================== 
 * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.
 * 
 * @Author : Jitender Hooda 
 * 
 ==============================================================================
 */

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class WebFunctions : MonoBehaviour
{
    public static WWW Get(string url)
    {

        WWW www = new WWW(url);

        WaitForSeconds w;
        while (!www.isDone)
            w = new WaitForSeconds(0.1f);
        return www;
    }

    public static WWW Post(string url, Dictionary<string, string> post)
    {
        WWWForm form = new WWWForm();
        foreach (var pair in post)
            form.AddField(pair.Key, pair.Value);

        WWW www = new WWW(url, form);
        WaitForSeconds w;
        while (!www.isDone)
            w = new WaitForSeconds(0.1f);

        return www;
    }
    public static WWW PostHeader(string url, byte[] post, Dictionary<string, string> header)
    {

        WWW www = new WWW(url, post, header);
        WaitForSeconds w;
        while (!www.isDone)
            w = new WaitForSeconds(0.1f);

        return www;
    }

    public void downloadImage(string url, string pathToSaveImage)
    {
        WWW www = new WWW(url);

        WaitForSeconds w;
        while (!www.isDone)
            w = new WaitForSeconds(0.1f);

        //Check if we failed to send
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("<color=orange> @@@@ downloadImage : Success");
            saveImage(pathToSaveImage, www.bytes);
        }
        else
        {
            Debug.Log("<color=orange> @@@@ downloadImage : Error: " + www.error);
        }
    }

    //private IEnumerator _downloadImage(WWW www, string savePath)
    //{
    //    yield return www;

    //    //Check if we failed to send
    //    if (string.IsNullOrEmpty(www.error))
    //    {
    //        Debug.Log("Success");

    //        //Save Image
    //        saveImage(savePath, www.bytes);
    //    }
    //    else
    //    {
    //        Debug.Log("Error: " + www.error);
    //    }
    //}

    void saveImage(string path, byte[] imageBytes)
    {
        Debug.Log("<color=orange> @@@@ saveImage path : </color>" + path);
        try
        {
            File.WriteAllBytes(path, imageBytes);
            Debug.Log("<color=orange> @@@@ saveImage saved image at path : </color>" + path);
        }
        catch (Exception e)
        {
            Debug.LogWarning("<color=orange> @@@@ saveImage : Failed To Save Data to: " + path);
            Debug.LogWarning("<color=orange> @@@@ loadImage : Error: " + e.Message);
        }
    }

    public byte[] loadImage(string path)
    {
        byte[] dataByte = null;

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Debug.LogWarning("<color=orange> @@@@ loadImage : Directory does not exist");
            return null;
        }

        if (!File.Exists(path))
        {
            Debug.Log("<color=orange> @@@@ loadImage : File does not exist");
            return null;
        }

        try
        {
            dataByte = File.ReadAllBytes(path);
            Debug.Log("<color=orange> @@@@ loadImage : Loaded Data from: " + path);
        }
        catch (Exception e)
        {
            Debug.LogWarning("<color=orange> @@@@ loadImage : Failed To Load Data from: " + path);
            Debug.LogWarning("<color=orange> @@@@ loadImage : Error: " + e.Message);
        }

        return dataByte;
    }
}