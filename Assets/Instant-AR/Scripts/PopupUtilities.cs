/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */  using System.Collections; using System.Collections.Generic; using UnityEngine; using System.IO; using UnityEditor; using System;
//using Vuforia;
using UnityEngine.Video; using UnityEngine.Networking; using UnityEngine.UI; using System.Text.RegularExpressions;  using Newtonsoft.Json.Converters; using Newtonsoft.Json; using Newtonsoft; using Newtonsoft.Json.Linq; using System.Reflection;

public class PopupUtilities : MonoBehaviour {     public static GameObject callingGameObject = null;
    delegate void DelegateMessageBoxButtonAction();
    /// <summary>     /// Makes the floating popup text Window in a separate tarnsparent canvas.     /// </summary>     /// <returns>The popuText object back to the calling GameObject, if needed there for further processing.</returns>     /// <param name="callingObject">Calling object is the GameObject which calls this popupText.</param>     /// <param name="message">Message which needs to go into the popupText.</param>     /// <param name="flushOthers">If set to <c>true</c> flush all others popTexts from the scene if exist.</param>     /// <param name="parameters">Any other Parameters required e.g. width, height , color etc.</param> 
    public static GameObject makePopupText(GameObject callingObject, GameObject parentContainer, string message, bool flushOthers, Dictionary<string,object> parameters)     {         if (message == null || message.Equals("null"))         {             return null;         }         callingGameObject = null; 
        if (parentContainer == null)         {             parentContainer = GameObject.Find("FloatingCanvas");
            if (parentContainer == null)
            {
                return null;
            }
        }         parentContainer.transform.SetAsLastSibling();           if (flushOthers)         {
            Transform[] transforms = parentContainer.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                Debug.Log("<color=red> @@@@@@@@@@ child.gameObject.name: </color>" + child.gameObject.name);
                if (child.gameObject != parentContainer)
                {
                    Debug.Log("<color=green> @@@@@@@@@@ Deleting </color>" + child.gameObject.name);
                    Destroy(child.gameObject);
                }
            }
        }


        GameObject popupPrefab = (GameObject)Resources.Load("UIPopupText");
        GameObject popupObject = (GameObject)Instantiate(popupPrefab);
        RectTransform rectTransform = popupObject.GetComponent<RectTransform>();


        if (parentContainer != null)
        {
            popupObject.transform.SetParent(parentContainer.transform);
            popupObject.transform.localPosition = Vector3.zero;
        }
        IAPopupScript popupScript = (IAPopupScript)popupObject.GetComponent<IAPopupScript>();         popupScript.message = message;          //DrawLine(callingObject, popupObject);         callingGameObject = callingObject;         return popupObject;     }      static void DrawLine(GameObject startObject, GameObject endObject)
    {

        //Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 0), Color.white, 2.5f);
        //Debug.DrawLine(startObject.transform.position, endObject.transform.position, Color.white, 2.5f);
        Debug.Log("<color=red> @@@@@@@@@ startObject: </color>"+ startObject.name);
        Debug.Log("<color=red> @@@@@@@@@ endObject: </color>" + endObject.name);
        Debug.Log("<color=red> @@@@@@@@@ Trying to attached to targetobject, so making new one  : </color>");
            LineRenderer line = endObject.AddComponent<LineRenderer>();             line.startColor = Color.black;             line.endColor = Color.black;             line.startWidth = 5f;             line.endWidth = 5f;             line.positionCount = 2;             line.useWorldSpace = true;             //Material mat = new Material(Shader.Find("Default-Line"));             line.material = Resources.Load<Material>("Default-Line");         line.SetPosition(0, startObject.transform.position);         line.SetPosition(1, endObject.transform.position);      } 

  
    /// <summary>     /// Makes the floating popup Window in a separate tarnsparent canvas.     /// </summary>     /// <returns>The popup object back to the calling GameObject, if needed there for further processing.</returns>     /// <param name="callingObject">Calling object is the GameObject which calls this popupText.</param>     /// <param name="message">Message which needs to go into the Top Portion of Popup Window.</param>     /// <param name="flushOthers">If set to <c>true</c> flush all others popTexts from the scene if exist.</param>     /// <param name="parameters">Any other Parameters required e.g. width, height , color etc.</param>         public static GameObject makePopupYesNo(GameObject callingObject, GameObject parentContainer, string message, bool flushOthers, Dictionary<string, object> parameters)     {         if(message==null || message.Equals("null")){             return null;         }          callingGameObject = null;                  GameObject popupPrefab = (GameObject)Resources.Load("UIPopupYesNo");         GameObject popupObject = (GameObject)Instantiate(popupPrefab);         RectTransform rectTransform = popupObject.GetComponent<RectTransform>();          //GameObject floatingSpace = GameObject.Find("FloatingCanvas");         //floatingSpace.transform.SetAsLastSibling();          if (parentContainer == null)         {             parentContainer = GameObject.Find("FloatingCanvas");
            if (parentContainer == null)
            {
                return null;
            }
        }         parentContainer.transform.SetAsLastSibling();           if (flushOthers)         {
            Transform[] transforms = parentContainer.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                Debug.Log("<color=red> @@@@@@@@@@ child.gameObject.name: </color>" + child.gameObject.name);
                if (child.gameObject != parentContainer)
                {
                    Debug.Log("<color=green> @@@@@@@@@@ Deleting </color>" + child.gameObject.name);
                    Destroy(child.gameObject);
                }
            }
        }            if (parentContainer != null)         {             popupObject.transform.SetParent(parentContainer.transform);             popupObject.transform.localPosition = Vector3.zero;         }         IAPopupScript popupScript = (IAPopupScript)popupObject.GetComponent<IAPopupScript>();         popupScript.message = message;         popupScript.callingObject = callingObject;         callingGameObject = callingObject;         return popupObject;     }
 
    /// <summary>     /// Makes the floating popup Window in a separate tarnsparent canvas.     /// </summary>     /// <returns>The popup object back to the calling GameObject, if needed there for further processing.</returns>     /// <param name="callingObject">Calling object is the GameObject which calls this popupText.</param>     /// <param name="message">Message which needs to go into the Top Portion of Popup Window.</param>     /// <param name="flushOthers">If set to <c>true</c> flush all others popTexts from the scene if exist.</param>     /// <param name="parameters">Any other Parameters required e.g. width, height , color etc.</param>     public static GameObject makePopupTable(GameObject callingObject, GameObject parentContainer, string jsonString, bool flushOthers, Dictionary<string, object> parameters)     {
        if (jsonString == null || jsonString.Equals("null"))         {             return null;         }

        callingGameObject = null;
        GameObject popupObject;           if (parentContainer == null)         {             parentContainer = GameObject.Find("FloatingCanvas");
            if (parentContainer == null)
            {
                return null;
            }
        }         parentContainer.transform.SetAsLastSibling();           if (flushOthers)         {
            Transform[] transforms = parentContainer.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                Debug.Log("<color=red> @@@@@@@@@@ child.gameObject.name: </color>" + child.gameObject.name);
                if (child.gameObject != parentContainer)
                {
                    Debug.Log("<color=green> @@@@@@@@@@ Deleting </color>" + child.gameObject.name);
                    Destroy(child.gameObject);
                }
            }
        }         GameObject popupPrefab = (GameObject)Resources.Load("UIPopupTable");         popupObject = (GameObject)Instantiate(popupPrefab);         RectTransform rectTransform = popupObject.GetComponent<RectTransform>();         //GameObject floatingSpace = GameObject.Find("FloatingCanvas");         //floatingSpace.transform.SetAsLastSibling();          if (parentContainer != null)         {             popupObject.transform.SetParent(parentContainer.transform);             popupObject.transform.localPosition = Vector3.zero;         } 
        TableManager popupScript = popupObject.GetComponent<TableManager>();         popupScript.m_JsonText = jsonString;         popupScript.MakeTable();         callingGameObject = callingObject;         return popupObject;     }


    public static string getTableStirngFromDropBox(string url)     {         var directResponse = WebFunctions.Get(url);         if (directResponse.error != null)         {             Debug.Log("<color=red> $$$$$$$$$$$$$$$$$ directResponse.error  : </color>");             return null;         }         else         {             return directResponse.text;         }     }



    /// <summary>     /// Makes the floating popup Window in a separate tarnsparent canvas.     /// </summary>     /// <returns>The popup object back to the calling GameObject, if needed there for further processing.</returns>     /// <param name="callingObject">Calling object is the GameObject which calls this popupText.</param>     /// <param name="message">Message which needs to go into the Top Portion of Popup Window.</param>     /// <param name="flushOthers">If set to <c>true</c> flush all others popTexts from the scene if exist.</param>     /// <param name="parameters">Any other Parameters required e.g. width, height , color etc.</param> 
    public static GameObject makePopupVideo(GameObject callingObject, GameObject parentContainer, bool flushOthers,bool preserveAspect,         string videoURL,bool playOnStart, Color borderColor, float borderWidth)     {         if (string.IsNullOrEmpty(videoURL) || videoURL.Equals("null"))
        {
            return null;
        }          if (parentContainer == null)         {             parentContainer = GameObject.Find("FloatingCanvas");
            if (parentContainer == null)
            {
                return null;
            }
        }         parentContainer.transform.SetAsLastSibling();                            if (borderColor == null)
        {
            borderColor = Color.red;
        }           if (flushOthers)         {
            Transform[] transforms = parentContainer.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                Debug.Log("<color=red> @@@@@@@@@@ child.gameObject.name: </color>" + child.gameObject.name);
                if (child.gameObject != parentContainer)
                {
                    Debug.Log("<color=green> @@@@@@@@@@ Deleting </color>" + child.gameObject.name);
                    Destroy(child.gameObject);
                }
            }
        }          GameObject popupPrefab = (GameObject)Resources.Load("UIPopupVideo");         GameObject popupObject = (GameObject)Instantiate(popupPrefab);         popupObject.transform.SetParent(parentContainer.transform);         popupObject.transform.localPosition = Vector3.zero;         popupObject.transform.localScale = Vector3.one;

        //==========
        float parent_width = parentContainer.GetComponent<RectTransform>().rect.width;          //==========


        StreamVideo streamVideoScript = (StreamVideo)popupObject.GetComponent<StreamVideo>();         streamVideoScript.parentContainer = parentContainer;         streamVideoScript.preserveAspect = preserveAspect;
        //streamVideoScript.url = "https://dl.dropbox.com/s/dsmrpxm5fomedfd/earth.mov"; 
        // streamVideoScript.url = "https://dl.dropbox.com/s/t17ie1xn33wqw0u/InstantAR_F02.mp4";
        streamVideoScript.url = videoURL;
        streamVideoScript.playVideo = playOnStart;


        ScreenTouchControl screenTouchControl = popupObject.GetComponent<ScreenTouchControl>();         BorderScript borderScript = screenTouchControl.Border.GetComponent<BorderScript>();         borderScript.borderColor = borderColor;         borderScript.borderWidth = borderWidth;          streamVideoScript.VideoKickoff();          return popupObject;     }




    /// <summary>     /// Makes the floating popup Window in a separate tarnsparent canvas.     /// </summary>     /// <returns>The popup object back to the calling GameObject, if needed there for further processing.</returns>     /// <param name="callingObject">Calling object is the GameObject which calls this popupText.</param>     /// <param name="message">Message which needs to go into the Top Portion of Popup Window.</param>     /// <param name="flushOthers">If set to <c>true</c> flush all others popTexts from the scene if exist.</param>     /// <param name="parameters">Any other Parameters required e.g. width, height , color etc.</param>     ///     ///      /// public static GameObject makePopupGauge(GameObject callingObject, GameObject parentContainer, bool flushOthers,bool preserveAspect,        /// float GaugeRadius, string m_Title, Color m_TitleColor, string m_Unit, float GaugeValue, List<ChartRange> ChartRanges)


    public static GameObject makePopupGauge(GaugePOCO gaugeInfo)     {         if (gaugeInfo.parentContainer == null)         {             gaugeInfo.parentContainer = GameObject.Find("FloatingCanvas");
            if (gaugeInfo.parentContainer == null)
            {
                return null;
            }
        }
        gaugeInfo.parentContainer.transform.SetAsLastSibling();         if (gaugeInfo.flushOthers)         {
            Transform[] transforms = gaugeInfo.parentContainer.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                //Debug.Log("<color=red> @@@@@@@@@@ child.gameObject.name: </color>" + child.gameObject.name);
                if (child.gameObject != gaugeInfo.parentContainer)
                {
                    //Debug.Log("<color=green> @@@@@@@@@@ Deleting </color>" + child.gameObject.name);
                    Destroy(child.gameObject);
                }
            }
        }


       
         GameObject popupPrefab = (GameObject)Resources.Load("UIPopupGauge");         GameObject popupObject = (GameObject)Instantiate(popupPrefab);         popupObject.transform.SetParent(gaugeInfo.parentContainer.transform);         popupObject.transform.localPosition = Vector3.zero;         popupObject.transform.localScale = Vector3.one;


        ChartManager chartManager = (ChartManager)popupObject.GetComponent<ChartManager>();
        chartManager.m_Title = gaugeInfo.m_Title;         chartManager.m_TitleColor = gaugeInfo.m_TitleColor;         chartManager.m_Unit = gaugeInfo.m_Unit;         chartManager.GaugeValue = gaugeInfo.GaugeValue;         //chartManager.ChartRanges = gaugeInfo.ChartRanges;
        chartManager.ChartRanges = gaugeInfo.ChartRanges;

        //float parent_width = gaugeInfo.parentContainer.GetComponent<RectTransform>().rect.width;
        if (gaugeInfo.GaugeRadius >= 0)
        {
            gaugeInfo.GaugeRadius = gaugeInfo.GaugeRadius * gaugeInfo.parentContainer.GetComponent<RectTransform>().rect.width;
        }
        else
        {
            gaugeInfo.GaugeRadius = gaugeInfo.parentContainer.GetComponent<RectTransform>().rect.width;
        }
        if (!gaugeInfo.preserveAspect)
        {
            popupObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gaugeInfo.GaugeRadius, gaugeInfo.GaugeRadius);
        }
        else
        {
            MatchSizeByScale(gaugeInfo.parentContainer, popupObject);
        }

        return popupObject;     }






     public static void MatchSize(GameObject parentObject, GameObject childObject)
    {


        if (parentObject == null || childObject == null)
        {
            return;
        }

        RectTransform parentTransform = parentObject.gameObject.GetComponent<RectTransform>();
        RectTransform childTransform = childObject.gameObject.GetComponent<RectTransform>();

        childTransform.anchorMin = new Vector2(0, 0);
        childTransform.anchorMax = new Vector2(1, 1);
        childTransform.offsetMin = new Vector2(0, 0); // new Vector2(left, bottom);
        childTransform.offsetMax = new Vector2(0, 0); // new Vector2(-right, -top);
    }      public static void MatchSizeByScale(GameObject parentObject, GameObject childObject)
    {

        if (parentObject == null || childObject == null)
        {
            return;
        }

        RectTransform parentTransform = parentObject.gameObject.GetComponent<RectTransform>();
        RectTransform childTransform = childObject.gameObject.GetComponent<RectTransform>();


        float widthFactor = parentTransform.rect.width / childTransform.rect.width;
        float heightFactor = parentTransform.rect.height / childTransform.rect.height;
        childTransform.localScale = new Vector3(widthFactor, heightFactor, 0);
    }
 
}  