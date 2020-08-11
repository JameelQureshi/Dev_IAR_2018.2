
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System;
using Vuforia;

using System.Collections.Generic;



public class RestServerDelegate : MonoBehaviour
{

    public void refreshImageDataOnServer(String uniqueTargetId)
    {
        StartCoroutine(refreshCallOnServer(uniqueTargetId));
    }
    private IEnumerator refreshCallOnServer(String uniqueTargetId)
    {

        Debug.Log("<color=green> =================== REFERESHING DATA ================================== </color>");

        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        string url = GlobalVariables.REST_SERVER + "query/user/refereshimage";
        WWWForm form = new WWWForm();
        form.AddField("uniqueTargetID", uniqueTargetId);
        WWW www = new WWW(url, form);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(.1f);
        }

        if (www.error == null)
        {
            Debug.Log("<color=white>   >>>>>>success: </color>" + www.text);
        }
        else
        {
            Debug.Log("<color=white>   >>>>>>something wrong:  </color>" + www.text);
        }

        Debug.Log("<color=green> =================== REFERESHING DONE ================================== </color>");

    }

    }
