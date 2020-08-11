/*
 * ============================================================================== 
 * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.
 * 
 * @Author : Jitender Hooda 
 * 
 ==============================================================================
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public static class FloatingVariables1
{
    //public static string UI_API = "https://ar-dev.emachinelabs.com/2/testquery";
    //public static string UI_API = "https://ar-dev.emachinelabs.com/getapp/9db3b24ac41f44da934829cc3e9ed8a5";
    public static string UI_API = GlobalVariables.REST_SERVER + "getapp/9db3b24ac41f44da934829cc3e9ed8a5";

    public static string REST_SERVER = GlobalVariables.REST_SERVER;
    public static string GET_IMAGE_JSON_API = "2/query/imageid?uniqueTargetId=";
    public static string GET_UI_JSON_API = "2/testquery";

    public static string VUFORIA_UNIQUE_ID;
    public static TargetImage targetImageObject;
      public static string TARGET_IMAGE_DBX_URL;
    public static float ASPECT_RATIO = 1.0f;


}