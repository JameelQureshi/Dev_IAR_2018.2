/*
 * ============================================================================== 
 * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.
 * 
 * @Author : Jitender Hooda 
 * 
 ==============================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using UnityEngine.Video;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

public class PrimaryScreenController : BaseScreen
{

    private bool processed = false;
    private bool applicationBuilt = false;
    private bool ARMode = false;
    //private string UniqueTargetId;
    BlocklyEvents eventObj;

    // Use this for initialization

    //private void Start()
    //{
    //   GlobalVariables.ContainerNames = new Dictionary<string, string>();
    //}


    public void PrimaryScreenApplication(GameObject baseCanvas, bool isARScreen)
    {

        ARMode = isARScreen;
        eventObj = new BlocklyEvents();
        // GlobalVariables.VUFORIA_UNIQUE_ID = "274a46365cb74bea9921cab4c6540a57"; //drone

        //===============================================
        //UniqueTargetId = GlobalVariables.VUFORIA_UNIQUE_ID;
        Debug.Log("<color=green> ################## GlobalVariables.VUFORIA_UNIQUE_ID: </color>" + GlobalVariables.VUFORIA_UNIQUE_ID);
        Debug.Log("<color=green> ############## GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID: </color>" + GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID);
        if (baseCanvas == null)
        {
            baseCanvas = this.gameObject;
        }

        if (applicationBuilt && !string.IsNullOrEmpty(GlobalVariables.VUFORIA_UNIQUE_ID) && GlobalVariables.VUFORIA_UNIQUE_ID.Equals(GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID))
        {
            return;
        }


        //UniqueTargetId = "2ff3ec534ffb4e879a5765984bb9e525";
        if (string.IsNullOrEmpty(GlobalVariables.VUFORIA_UNIQUE_ID))
        {
            //JituMessageBox.DisplayMessageBox("Scan Status", "No Image Has been scanned yet !!!", true, ok);
            Debug.Log("<color=red> ################## No Image Has been scanned yet !!! </color>");

        }
        else if (!GlobalVariables.VUFORIA_UNIQUE_ID.Equals(GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID))
        {
            applicationBuilt = false;
            GlobalVariables.PREVIOUS_VUFORIA_UNIQUE_ID = GlobalVariables.VUFORIA_UNIQUE_ID;
            GlobalVariables.ContainerNames = new Dictionary<string, string>();
            GlobalVariables.CanvasNames = new Dictionary<string, bool>();

            processed = false;
            GlobalVariables.ButtonList = new List<string>();

            if (baseCanvas.GetComponent<Canvas>() != null)
            {
                Transform[] transforms = baseCanvas.transform.GetComponentsInChildren<Transform>();
                foreach (Transform child in transforms)
                {
                    //Debug.Log("<color=red> @@@@@@@@@@ child.gameObject.name: </color>" + child.gameObject.name);
                    if (child.gameObject != baseCanvas)
                    {
                        //Debug.Log("<color=green> @@@@@@@@@@ Deleting </color>" + child.gameObject.name);
                        Destroy(child.gameObject);
                    }
                }


                getAppJson(baseCanvas);
            }
            else
            {
                Debug.Log("<color=red> @@@@ PrimaryScreenController is NOT attached to a Canvas GameObject, so application can not be built </color>");
            }

            applicationBuilt = true;
            //m_CommonBarScript.PrimaryScreenButton.GetComponent<RectTransform>().Rotate(0f, 0f, 0f);
            BlocklyEvents eventObj = new BlocklyEvents();
            eventObj.executeARTrackingBlocks(UIEnums.ARTrackingTypes.ApplicationLoad);

        }

    }



    private void ok()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("UIScene");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("3-CloudReco");
        applicationBuilt = false;
        this.gameController.OpenCloudScreen();
    }

    private void getAppJson(GameObject baseCanvas)
    {
        if (baseCanvas == null)
        {
            return;
        }
        UIUtility.initGlobalVariables();

        //=========================================================================
        //Debug.Log("<color=red> @@@@ GlobalVariables.VUFORIA_UNIQUE_ID </color>" + GlobalVariables.VUFORIA_UNIQUE_ID);
        //string UniqueTargetId = GlobalVariables.VUFORIA_UNIQUE_ID;
        //UniqueTargetId = "054f66d6a2334eed9a52b31d26bfd097"; //shaumik
        //UniqueTargetId = "58a04f1c17bd41e8900eedbf43d1bdd6"; //Amazon Logo 
        //UniqueTargetId = "cc1c71357dbf4bb09ffb987e3f0e26ba"; //Blue Flower Logo
        //UniqueTargetId = "2ff3ec534ffb4e879a5765984bb9e525"; //Mutimeter

        //Debug.Log("<color=red> @@@@ UniqueTargetId </color>" + GlobalVariables.VUFORIA_UNIQUE_ID);
        //Debug.Log("<color=red> @@@@ GlobalVariables.UI_API </color>" + GlobalVariables.UI_API);
        string uiApiURL = GlobalVariables.GET_APP_API + GlobalVariables.VUFORIA_UNIQUE_ID;
        //string uiApiURL = "https://dl.dropbox.com/s/mplxfba149ndd3p/SampleObject.json";
        Debug.Log("<color=green> @@@@ uiApiURL </color>" + uiApiURL);
        //=========================================================================

        UIApplication uiApplication = null;
        try
        {
            //uiApplication = UIUtility.getUIApplication(uiApiURL);	
            uiApplication = UIUtility.getUIApplicationUpdated(uiApiURL);
            if (uiApplication == null)
            {
                JituMessageBox.DisplayMessageBox("Application Status", "There is no UI Experience associated with this image .\n\nSwitching Back to Image Scanniing Mode !!!", true, ok);
            }

            //loadAppResourcesAsync(uiApplication);
            GetStringAsync(uiApplication);

            TargetImage targetImageObject = UIUtility.getFullTargetImageFromUID(GlobalVariables.VUFORIA_UNIQUE_ID);
            //Debug.Log("<color=red> @@@@ After targetImageObject </color>");
            UIUtility.initializeGlobalVariables(targetImageObject);
            //Debug.Log("<color=red> @@@@ After initializeGlobalVariables </color>");

            //Debug.Log("<color=red> @@@@ uiApplication.views.Length  </color>" + uiApplication.views.Length);

            if (uiApplication.children != null)
            {
                buildChildren(uiApplication.children, baseCanvas);
            }
            //foreach (UIView uiView in uiApplication.views)	
            //{	
            //    //TODO Jitu filetered only the first canvas, later need to be removed this filter when all canvas logic is implemented..  	
            //    if (uiView.canvas.elementId.Equals("canvas-0"))	
            //    {	
            //        buildCanvas(baseCanvas, uiView.canvas);	
            //    }	
            //    //buildCanvas(baseCanvas, uiView.canvas);	
            //    //break;	
            //}	
            //Debug.Log("<color=red> @@@@ Done iteration on Views </color>");	
            if (uiApplication.children.Count > 0)
            {
                //BlocklyEvents script = new BlocklyEvents();	
                //script.executeFirstTimeBlocky();	
                eventObj.executeFirstTimeBlocky();
            }
            Debug.Log("<color=green> @@@@ Done iteration on panels </color>");
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in getMyTestClass: </color>" + e.ToString());
        }

        //Debug.Log("<color=blue> ====================== cloud TARGET_IMAGE_DBX_URL:     </color>" + FloatingVariables.TARGET_IMAGE_DBX_URL);

    }

    private void buildChildren(List<ChildElement> children, GameObject parent)
    {
        if (children == null)
        {
            if (parent != null)
            {
                Debug.Log("<color=green> @@@@ No child to add for parent : </color>" + parent.name);
            }
            else
            {
                Debug.Log("<color=green> @@@@ No child and no parent!</color>");
            }

            return;
        }
        foreach (ChildElement ce in children)
        {
            switch (ce.elementType)
            {
                case "Canvas":
                    buildCanvas(parent, ce.canvas);
                    break;
                case "Panel":
                    buildPanel(parent, ce.panel);
                    break;
                case "Button":
                    buildButton(parent, ce.button);
                    break;
                case "Dropdown":
                    buildDropdown(parent, ce.dropdown);
                    break;
                case "InputField":
                    buildInputField(parent, ce.inputField);
                    break;
                case "Text":
                    buildText1(parent, ce.text);
                    break;
                case "Image":
                    buildImage(parent, ce.image);
                    break;
                case "Toggle":
                    buildToggle(parent, ce.toggle);
                    break;
                case "Slider":
                    buildSlider(parent, ce.slider);
                    break;
                case "PopUp":
                    buildPopup(parent, ce.popup);
                    break;
                case "Gauge":
                    //TODO LAX : Put the implementation for gauge	
                    break;
                case "Video":
                    //TODO LAX : Put the implementation for video	
                    break;
                case "Table":
                    //TODO LAX : Put the implementation for table	
                    break;
            }
        }
    }


    public void buildCanvas(GameObject baseCanvas, UICanvas uiCanvas)
    {
        Debug.Log("<color=green> @@@@  baseCanvas.name </color>" + baseCanvas.name);
        Debug.Log("<color=green> @@@@  uiCanvas.name </color>" + uiCanvas.elementId);


        //Debug.Log("<color=purple> @@@@  baseCanvas.transform.childCount </color>" + baseCanvas.transform.childCount);

        //if (baseCanvas.transform.childCount > 0)
        //{
        //    Debug.Log("<color=purple> @@@@  Child object to be deleted is: </color>" + baseCanvas.transform.GetChild(0).gameObject.name);
        //    Destroy(baseCanvas.transform.GetChild(0).gameObject);
        //}

        //Transform[] gameObjects = baseCanvas.GetComponentsInChildren<Transform>();
        //foreach (Transform transform in gameObjects)
        //{
        //    if (!transform.name.Equals(baseCanvas.name))
        //    {
        //        Debug.Log("<color=purple> @@@@ Child gameObject to be destroyed </color>" + transform.name);
        //        Destroy(transform.gameObject);
        //        Debug.Log("<color=red> @@@@ Destroyed gameObject.name </color>" + transform.name);
        //    }
        //    else
        //    {
        //        Debug.Log("<color=purple> @@@@ this gameObject.name </color>" + transform.name);
        //    }
        //}

        //Debug.Log("<color=red> @@@@  before maincanvase </color>" + baseCanvas.name);
        GameObject mainCanvas = (GameObject)Instantiate(Resources.Load("UICanvas"));
        //Debug.Log("<color=red> @@@@  before setparent maincanvase </color>" + baseCanvas.name);
        mainCanvas.transform.SetParent(baseCanvas.transform);
        mainCanvas.transform.localScale = Vector3.one;

        RectTransform canvasRectTransform = mainCanvas.GetComponent<RectTransform>();
        canvasRectTransform.anchorMin = new Vector2(0, 0);
        canvasRectTransform.anchorMax = new Vector2(1, 1);
        canvasRectTransform.pivot = new Vector2(0, 0);
        canvasRectTransform.offsetMin = new Vector2(0, 0);
        canvasRectTransform.offsetMax = new Vector2(0, 0);




        //Debug.Log("<color=red> @@@@  after setparent  maincanvase </color>" + baseCanvas.name);
        //GameObject mainCanvas = baseCanvas;

        float baseW = mainCanvas.GetComponent<RectTransform>().rect.width;
        float baseH = mainCanvas.GetComponent<RectTransform>().rect.height;
        //Canvas canvas = ((GameObject)Instantiate(Resources.Load("UICanvas"))).GetComponent<Canvas>();
        //mainCanvas.name = "Canvas_" + uiCanvas.id;
        mainCanvas.name = uiCanvas.elementId;
        //canvas.name = "Canvas_" + uiCanvas.id;
        float percentFactor = 0.01f;
        //Debug.Log("<color=purple> @@@@ buildCanvas 1: </color>");

        //float posX = uiCanvas.rectTransform.posX * base_width;
        //float posY = uiCanvas.rectTransform.posY * base_height;
        //float posZ = 0;

        //float width = uiCanvas.rectTransform.width * base_width;
        //float height = uiCanvas.rectTransform.height * base_height;

        //testPanel.transform.SetParent(parent.transform);
        //canvas.transform.position = new Vector3(0, 0, 0);
        //canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(base_width, base_height);
        //Debug.Log("<color=purple> @@@@ buildCanvas layout : </color>" + uiCanvas.layout.layoutType);
        if (uiCanvas.layout != null && uiCanvas.layout.layoutType != null)
        {
            //Debug.Log("<color=purple> @@@@ uiCanvas has layout:   : </color>" + uiCanvas.layout.layoutType);
            //if (uiCanvas.layout.layoutType.Equals("Vertical"))	
            //{	
            //    //VerticalLayoutGroup layoutGrp = canvas.gameObject.AddComponent<VerticalLayoutGroup>();	
            //    VerticalLayoutGroup layoutGrp = mainCanvas.AddComponent<VerticalLayoutGroup>();	
            //    //mainCanvas.AddComponent<VerticalLayoutGroup>();	
            //    layoutGrp.CalculateLayoutInputVertical();	
            //    layoutGrp.SetLayoutVertical();	
            //    layoutGrp.padding.left = uiCanvas.layout.leftPadding * (int)Math.Round(percentFactor * baseW);	
            //    layoutGrp.padding.right = uiCanvas.layout.rightPadding * (int)Math.Round(percentFactor * baseW);	
            //    layoutGrp.padding.top = uiCanvas.layout.topPadding * (int)Math.Round(percentFactor * baseH);	
            //    layoutGrp.padding.bottom = uiCanvas.layout.bottomPadding * (int)Math.Round(percentFactor * baseH);	
            //    layoutGrp.spacing = uiCanvas.layout.spacing * (int)Math.Round(percentFactor * baseH);	
            //    layoutGrp.childAlignment = getLayoutAlignment(uiCanvas.layout.justification + uiCanvas.layout.alignment);	
            //    layoutGrp.childControlWidth = false;//uiCanvas.layout.controlChildSizeWidth;	
            //    layoutGrp.childControlHeight = false;//uiCanvas.layout.controlChildSizeHeight;	
            //    //layoutGrp.childScaleWidth = uiCanvas.layout.userChildScaleWidth;	
            //    //layoutGrp.childScaleHeight = uiCanvas.layout.userChildScaleHeight;	
            //    layoutGrp.childForceExpandWidth = false;//uiCanvas.layout.childForceExpandWidth;	
            //    layoutGrp.childForceExpandHeight = false;//uiCanvas.layout.childForceExpandHeight;	
            //}	
            //else if (uiCanvas.layout.layoutType.Equals("Horizontal"))	
            //{	
            //    //HorizontalLayoutGroup layoutGrp = canvas.gameObject.AddComponent<HorizontalLayoutGroup>();	
            //    HorizontalLayoutGroup layoutGrp = mainCanvas.AddComponent<HorizontalLayoutGroup>();	
            //    layoutGrp.CalculateLayoutInputHorizontal();	
            //    layoutGrp.SetLayoutHorizontal();	
            //    layoutGrp.padding.left = uiCanvas.layout.leftPadding * (int)Math.Round(percentFactor * baseW);	
            //    layoutGrp.padding.right = uiCanvas.layout.rightPadding * (int)Math.Round(percentFactor * baseW);	
            //    layoutGrp.padding.top = uiCanvas.layout.topPadding * (int)Math.Round(percentFactor * baseH);	
            //    layoutGrp.padding.bottom = uiCanvas.layout.bottomPadding * (int)Math.Round(percentFactor * baseH);	
            //    layoutGrp.spacing = uiCanvas.layout.spacing * (int)Math.Round(percentFactor * baseW);	
            //    layoutGrp.childAlignment = getLayoutAlignment(uiCanvas.layout.alignment + uiCanvas.layout.justification);	
            //    layoutGrp.childControlWidth = false;//uiCanvas.layout.controlChildSizeWidth;	
            //    layoutGrp.childControlHeight = false;//uiCanvas.layout.controlChildSizeHeight;	
            //    //layoutGrp.childScaleWidth = uiCanvas.layout.userChildScaleWidth;	
            //    //layoutGrp.childScaleHeight = uiCanvas.layout.userChildScaleHeight;	
            //    layoutGrp.childForceExpandWidth = false;//uiCanvas.layout.childForceExpandWidth;	
            //    layoutGrp.childForceExpandHeight = false;//uiCanvas.layout.childForceExpandHeight;	
            //}	
            addLayOut(uiCanvas.layout, mainCanvas.gameObject, baseW, baseH);
        }
        addBorder(mainCanvas.gameObject, uiCanvas.border);

        //canvas.renderMode = getRenderMode(uiCanvas.renderMode);
        mainCanvas.GetComponent<Canvas>().renderMode = getRenderMode(uiCanvas.renderMode);
        CanvasGroup m_CanvasGroup = mainCanvas.GetComponent<CanvasGroup>();

        GlobalVariables.CanvasNames.Add(mainCanvas.name, uiCanvas.homeScreen);

        if (uiCanvas.homeScreen)
        {
            //m_CanvasGroup.alpha = 1;
            mainCanvas.transform.localScale = Vector3.one;
        }
        else
        {
            //m_CanvasGroup.alpha = 0;
            mainCanvas.transform.localScale = Vector3.zero;

        }
        //if (uiCanvas.visible)
        //{
        //    //m_CanvasGroup.alpha = 1;
        //    //m_CanvasGroup.blocksRaycasts = true;
        //    //m_CanvasGroup.interactable = true;
        //    mainCanvas.transform.localScale = Vector3.one;
        //}
        //else
        //{
        //    //m_CanvasGroup.alpha = 0;
        //    mainCanvas.transform.localScale = Vector3.zero;

        //}

        //GlobalVariables.ContainerNames.Add(mainCanvas.name, "rgba(0,0,0,0)");
        if (uiCanvas.imageAttributes != null && !string.IsNullOrEmpty(uiCanvas.imageAttributes.color))
        {
            GlobalVariables.ContainerNames.Add(mainCanvas.name, uiCanvas.imageAttributes.color);
        }
        else
        {
            GlobalVariables.ContainerNames.Add(mainCanvas.name, "rgba(0,0,0,0)");
        }

        //Image img = m_CanvasGroup.GetComponent<Image>();
        //img.color = getColor(uiCanvas.imageAttributes.color);
        //img.color = getColor("rgba(255, 255, 255, 0)");

        buildChildren(uiCanvas.children, mainCanvas);
        //Debug.Log("<color=green> @@@@ Done iteration on panels </color>");


        // Text positionimageObject
        //RectTransform rectTransform;
        //rectTransform = text.GetComponent<RectTransform>();
        //rectTransform.localPosition = new Vector3(0, 0, 0);
        //rectTransform.sizeDelta = new Vector2(400, 200);
    }

    public void buildPanel(GameObject parent, UIPanel uiPanel)
    {
        try
        {
            float baseW = parent.GetComponent<RectTransform>().rect.width;
            float baseH = parent.GetComponent<RectTransform>().rect.height;
            float percentFactor = 0.01f;

            var testPanel = (GameObject)Instantiate(Resources.Load("UIPanel"));
            //testPanel.name = "Panel_" + uiPanel.id;
            testPanel.name = uiPanel.elementId;
            //Debug.Log("<color=green> @@@@ testPanel.name:   : </color>" + testPanel.name + " , uiPanel.id " + uiPanel.id + "");
            //Debug.Log("<color=green> @@@@ testPanel.color:   : </color>" + uiPanel.imageAttributes.color);

            float posX = uiPanel.rectTransform.posX * baseW;
            float posY = uiPanel.rectTransform.posY * baseH;
            float posZ = 0;

            float width = uiPanel.rectTransform.width * baseW;
            float height = uiPanel.rectTransform.height * baseH;

            testPanel.transform.SetParent(parent.transform);
            testPanel.transform.localPosition = new Vector3(posX, posY, posZ);
            testPanel.transform.localScale = Vector3.one;
            testPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            if (!uiPanel.visible)
            {
                testPanel.transform.localScale = Vector3.zero;
            }

            if (uiPanel.layout != null && uiPanel.layout.layoutType != null)
            {
                //Debug.Log("<color=green> @@@@ uiPanel has layout:   : </color>" + uiPanel.layout.layoutType);
                addLayOut(uiPanel.layout, testPanel.gameObject, baseW, baseH);
                //if (uiPanel.layout.layoutType.Equals("Vertical"))	
                //{	
                //    VerticalLayoutGroup layoutGrp = testPanel.gameObject.AddComponent<VerticalLayoutGroup>();	
                //    layoutGrp.CalculateLayoutInputVertical();	
                //    layoutGrp.SetLayoutVertical();	
                //    layoutGrp.padding.left = uiPanel.layout.leftPadding * (int)Math.Round(percentFactor * baseW);	
                //    layoutGrp.padding.right = uiPanel.layout.rightPadding * (int)Math.Round(percentFactor * baseW);	
                //    layoutGrp.padding.top = uiPanel.layout.topPadding * (int)Math.Round(percentFactor * baseH);	
                //    layoutGrp.padding.bottom = uiPanel.layout.bottomPadding * (int)Math.Round(percentFactor * baseH);	
                //    layoutGrp.spacing = uiPanel.layout.spacing * (int)Math.Round(percentFactor * baseH);	
                //    layoutGrp.childAlignment = getLayoutAlignment(uiPanel.layout.justification + uiPanel.layout.alignment);	
                //    layoutGrp.childControlWidth = false;//uiPanel.layout.controlChildSizeWidth;	
                //    layoutGrp.childControlHeight = false;//uiPanel.layout.controlChildSizeHeight;	
                //    //layoutGrp.childScaleWidth = uiPanel.layout.userChildScaleWidth;	
                //    //layoutGrp.childScaleHeight = uiPanel.layout.userChildScaleHeight;	
                //    layoutGrp.childForceExpandWidth = false;//uiPanel.layout.childForceExpandWidth;	
                //    layoutGrp.childForceExpandHeight = false;//uiPanel.layout.childForceExpandHeight;	
                //}	
                //else if (uiPanel.layout.layoutType.Equals("Horizontal"))	
                //{	
                //    HorizontalLayoutGroup layoutGrp = testPanel.gameObject.AddComponent<HorizontalLayoutGroup>();	
                //    layoutGrp.CalculateLayoutInputHorizontal();	
                //    layoutGrp.SetLayoutHorizontal();	
                //    layoutGrp.padding.left = uiPanel.layout.leftPadding * (int)Math.Round(percentFactor * baseW);	
                //    layoutGrp.padding.right = uiPanel.layout.rightPadding * (int)Math.Round(percentFactor * baseW);	
                //    layoutGrp.padding.top = uiPanel.layout.topPadding * (int)Math.Round(percentFactor * baseH);	
                //    layoutGrp.padding.bottom = uiPanel.layout.bottomPadding * (int)Math.Round(percentFactor * baseH);	
                //    layoutGrp.spacing = uiPanel.layout.spacing * (int)Math.Round(percentFactor * baseW);	
                //    layoutGrp.childAlignment = getLayoutAlignment(uiPanel.layout.alignment + uiPanel.layout.justification);	
                //    layoutGrp.childControlWidth = false;//uiPanel.layout.controlChildSizeWidth;	
                //    layoutGrp.childControlHeight = false;//uiPanel.layout.controlChildSizeHeight;	
                //    //layoutGrp.childScaleWidth = uiPanel.layout.userChildScaleWidth;	
                //    //layoutGrp.childScaleHeight = uiPanel.layout.userChildScaleHeight;	
                //    layoutGrp.childForceExpandWidth = false;//uiPanel.layout.childForceExpandWidth;	
                //    layoutGrp.childForceExpandHeight = false;//uiPanel.layout.childForceExpandHeight;	
                //}	
            }
            addBorder(testPanel.gameObject, uiPanel.border);

            Image img = testPanel.GetComponent<Image>();
            if (ARMode)
            {
                img.color = getColor("rgba(0,0,0,0)");
            }
            else
            {
                img.color = getColor(uiPanel.imageAttributes.color);
            }
            if (uiPanel.imageAttributes != null)
            {
                if (!String.IsNullOrEmpty(uiPanel.imageAttributes.sourceImage))
                {
                    string imagePath = saveSecondaryImage(uiPanel.imageAttributes.sourceImage);
                    img.sprite = loadNewSprite(imagePath);
                    img.type = Image.Type.Simple;
                    img.preserveAspect = uiPanel.imageAttributes.preserveAspect;
                    //img.preserveAspect = false;

                    // Comment below 2 line if you like mix color with image background
                    img.color = getColor("rgba(255,255,255,255)");
                    uiPanel.imageAttributes.color = "rgba(255,255,255,255)";
                }
                else if (!String.IsNullOrEmpty(uiPanel.imageAttributes.SourceImageIcon))
                {
                    string imagePath = uiPanel.imageAttributes.SourceImageIcon;
                    Debug.Log("<color=red> @@@@@@@@@@ imagePath before : </color>" + imagePath);
                    string assetIconPrefix = "assets/images/icons/";
                    Sprite targetImageSprite = null;
                    if ((imagePath.IndexOf(assetIconPrefix) == 0))
                    {
                        imagePath = imagePath.Substring(imagePath.IndexOf(assetIconPrefix) + assetIconPrefix.Length);
                        imagePath = "JituSprites/" + imagePath;
                        targetImageSprite = Resources.Load<Sprite>(imagePath);
                    }
                    else if ((imagePath.IndexOf("http") == 0))
                    {
                        targetImageSprite = loadNewSprite(imagePath);
                    }
                    Debug.Log("<color=red> @@@@@@@@@@ imagePath after: </color>" + imagePath);
                    img.sprite = targetImageSprite;
                    img.type = Image.Type.Simple;
                    img.preserveAspect = uiPanel.imageAttributes.preserveAspect;
                    //img.preserveAspect = false;
                    // Comment below 2 line if you like mix color with image background
                    img.color = getColor("rgba(255,255,255,255)");
                    uiPanel.imageAttributes.color = "rgba(255,255,255,255)";
                }
            }

            if (uiPanel.imageAttributes != null && !string.IsNullOrEmpty(uiPanel.imageAttributes.color))
            {
                GlobalVariables.ContainerNames.Add(testPanel.name, uiPanel.imageAttributes.color);
            }
            else
            {
                GlobalVariables.ContainerNames.Add(testPanel.name, "rgba(0,0,0,0)");
            }

            buildChildren(uiPanel.children, testPanel);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildPanel: </color>" + e.ToString());
        }
    }

    public void buildButton(GameObject parent, UIButton uiButton)
    {
        try
        {

            Button button = ((GameObject)Instantiate(Resources.Load("UIButton"))).GetComponent<Button>();
            //button.name = "button-" + uiButton.elementId;
            button.name = uiButton.elementId;
            GlobalVariables.ButtonList.Add(button.name);

            float baseW = parent.GetComponent<RectTransform>().rect.width;
            float baseH = parent.GetComponent<RectTransform>().rect.height;

            float posX = uiButton.rectTransform.posX * baseW;//base_width;
            //jitu trying to set the position X=0 to the left edge of the parent panel
            //posX = posX - baseW / 2; //TODO LAX : This was intentionally done and now being removed
            float posY = uiButton.rectTransform.posY * baseH;// base_height;
            float posZ = 0;


            float width = uiButton.rectTransform.width * baseW;
            float height = uiButton.rectTransform.height * baseH;

            //string color = uiButton.imageAttributes.color;
            button.transform.SetParent(parent.transform);
            //button.transform.position = new Vector3(posX, posY, posZ);
            button.transform.localPosition = new Vector3(posX, posY, posZ);
            button.transform.localScale = Vector3.one;

            button.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            ColorBlock theColor = button.GetComponent<Button>().colors;

            if (!uiButton.visible)
            {
                button.transform.localScale = Vector3.zero;
            }


            //if (uiButton.imageAttributes != null && !String.IsNullOrEmpty(uiButton.imageAttributes.sourceImage))
            //{
            //    String imagePath = saveSecondaryImage(uiButton.imageAttributes.sourceImage);
            //    Image img = button.GetComponent<Image>();
            //    img.sprite = loadNewSprite(imagePath);
            //    img.type = Image.Type.Simple;
            //    img.preserveAspect = true;
            //}

            Image img = button.GetComponent<Image>();
            if (uiButton.imageAttributes != null)
            {
                if (!String.IsNullOrEmpty(uiButton.imageAttributes.sourceImage))
                {
                    string imagePath = saveSecondaryImage(uiButton.imageAttributes.sourceImage);
                    img.sprite = loadNewSprite(imagePath);
                    img.type = Image.Type.Simple;
                    img.preserveAspect = uiButton.imageAttributes.preserveAspect;
                    //img.preserveAspect = false;

                    // Comment below 2 line if you like mix color with image background
                    img.color = getColor("rgba(255,255,255,255)");
                    uiButton.imageAttributes.color = "rgba(255,255,255,255)";
                    uiButton.transition.normalColor = "rgba(255,255,255,255)";
                }
                else if (!String.IsNullOrEmpty(uiButton.imageAttributes.SourceImageIcon))
                {
                    string imagePath = uiButton.imageAttributes.SourceImageIcon;
                    Debug.Log("<color=red> @@@@@@@@@@ imagePath before : </color>" + imagePath);
                    string assetIconPrefix = "assets/images/icons/";
                    Sprite targetImageSprite = null;
                    if ((imagePath.IndexOf(assetIconPrefix) == 0))
                    {
                        imagePath = imagePath.Substring(imagePath.IndexOf(assetIconPrefix) + assetIconPrefix.Length);
                        imagePath = imagePath.Substring(0, imagePath.LastIndexOf("."));
                        imagePath = "JituSprites/" + imagePath;
                        targetImageSprite = Resources.Load<Sprite>(imagePath);
                    }
                    else if ((imagePath.IndexOf("http") == 0))
                    {
                        targetImageSprite = loadNewSprite(imagePath);
                    }
                    Debug.Log("<color=red> @@@@@@@@@@ imagePath after: </color>" + imagePath);
                    img.sprite = targetImageSprite;
                    img.type = Image.Type.Simple;
                    img.preserveAspect = uiButton.imageAttributes.preserveAspect;
                    //img.preserveAspect = false;
                    // Comment below 2 line if you like mix color with image background
                    img.color = getColor("rgba(255,255,255,255)");
                    uiButton.imageAttributes.color = "rgba(255,255,255,255)";
                    uiButton.transition.normalColor = "rgba(255,255,255,255)";
                }
            }

            //Debug.Log("<color=green> The normal color for " + button.name + "  is : </color>" + uiButton.transition.normalColor);
            theColor.normalColor = getColor(uiButton.transition.normalColor); //TODO Lax : Add the normal color only if there is no image
            //theColor.pressedColor = getColor(uiButton.transition.pressedColor);
            //theColor.highlightedColor = getColor(uiButton.transition.highlightedColor);
            theColor.pressedColor = theColor.normalColor;
            theColor.highlightedColor = Color.Lerp(theColor.normalColor, Color.black, .4f);
            theColor.disabledColor = Color.Lerp(theColor.normalColor, Color.white, .2f);
            button.GetComponent<Button>().colors = theColor;


            UIText uiText = uiButton.text;
            if (uiText.textAttribute != null)
            {
                button.GetComponentInChildren<Text>().text = uiText.textAttribute.text;
                button.GetComponentInChildren<Text>().font.name = uiText.textAttribute.font;
                button.GetComponentInChildren<Text>().fontStyle = getFontStyle(uiText.textAttribute.fontStyle);
                if (GlobalVariables.fontFactor > 0)
                {
                    button.GetComponentInChildren<Text>().fontSize = uiText.textAttribute.fontSize * GlobalVariables.fontFactor;
                }
                else
                {
                    button.GetComponentInChildren<Text>().fontSize = uiText.textAttribute.fontSize;
                }

                button.GetComponentInChildren<Text>().lineSpacing = uiText.textAttribute.lineSpacing;
                button.GetComponentInChildren<Text>().supportRichText = uiText.textAttribute.richText;
                button.GetComponentInChildren<Text>().alignment = getTextAnchor(uiText.textAttribute.alignmentVertical, uiText.textAttribute.alignmentHorizontal);
                button.GetComponentInChildren<Text>().horizontalOverflow = getHorizontalWrapMode(uiText.textAttribute.horizontalOverflow);
                button.GetComponentInChildren<Text>().verticalOverflow = getVerticalWrapMode(uiText.textAttribute.verticalOverflow);
                button.GetComponentInChildren<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                button.GetComponentInChildren<Text>().verticalOverflow = VerticalWrapMode.Overflow;
                button.GetComponentInChildren<Text>().resizeTextForBestFit = uiText.textAttribute.bestFit;
                button.GetComponentInChildren<Text>().color = getRGBAColor(uiText.textAttribute.color);
            }

            //if (uiButton.layout != null && uiButton.layout.layoutType != null)
            //{
            //    addLayOut(uiButton.layout, button.gameObject, baseW, baseH);
            //}

            Debug.Log(button.name + "<color=red> @@@@@@@@@@ Button Border is : </color>" + uiButton.border.width);
            if (uiButton.border.width > 0)
            {
                Debug.Log(button.name + "<color=green> @@@@@@@@@@ Button Border is : </color>" + uiButton.border.width);
                addBorder(button.gameObject, uiButton.border);
            }

            //JITU TESTING MYASSET===============

            List<EventAction> eventActions = uiButton.eventActions;
            if (eventActions != null)
            {
                buildEvent(eventActions, button.gameObject);
            }

            //UIEnums.EventTypes eventType = eventActions.eventType


            //========================


            //if(button.name.Equals("button-10", StringComparison.OrdinalIgnoreCase))	
            //{	
            //    //TODO LAX : Its purely for testing purpose, please definitely delete it	
            //    Debug.Log("<color=yellow> @@@@ Popup </color>");	
            //    buildPopup2(parent, "This is default message2");	
            //}	
            buildChildren(uiButton.children, button.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildButton: </color>" + e.ToString());
        }

    }

    private void buildEvent(List<EventAction> eventActions, GameObject sourceElement)
    {
        foreach (EventAction eventAction in eventActions)
        {
            switch (eventAction.eventType.ToString())
            {
                case "OnClick":
                    //buildCanvas(parent, ce.canvas);
                    List<ElementAsset> elementAssets = eventAction.elementAssets;
                    if (elementAssets != null)
                    {
                        showAsset(elementAssets, sourceElement);
                    }
                    break;
                case "ValueChange":
                    //buildPanel(parent, ce.panel);
                    break;
            }
        }
    }
    private void showAsset(List<ElementAsset> elementAssets, GameObject sourceElement)
    {
        AssetInvokeHandler assetInvokeHandler = sourceElement.GetComponent<AssetInvokeHandler>();
        if (assetInvokeHandler != null)
        {
            foreach (ElementAsset elementAsset in elementAssets)
            {
                assetInvokeHandler.assetType = elementAsset.assetType.ToString();
                switch (elementAsset.assetType.ToString())
                {
                    case "Image":
                        //Audio, Video, ThreeDObject, Image, Document, Icon
                        assetInvokeHandler.assetURL = elementAsset.image.url;
                        break;
                    case "Video":
                        assetInvokeHandler.assetURL = elementAsset.video.url;
                        break;
                    case "Document":
                        assetInvokeHandler.assetURL = elementAsset.document.url;
                        break;
                    case "Icon":
                        assetInvokeHandler.assetURL = elementAsset.icon.url;
                        break;
                    case "ThreeDObject":
                        assetInvokeHandler.assetURL = elementAsset.threeDObject.url;
                        break;
                }
            }
        }
    }


    public void buildImage(GameObject parent, UIImage image)
    {
        string imagePath;
        bool isPrimaryImage = image.imageAttributes.primaryImage;

        if (isPrimaryImage)
        {
            //Image img = parent.GetComponent<Image>();
            //TODO LAX : this is being done because if we remove any panel, the color of the upper and bottom of main image was not matching with canvas color. canvas color is being set in prefab
            //img.color = getColor("rgba(6,169,255,255)");
            //img.color = getColor("rgba(6,6,6,0)");

            imagePath = GlobalVariables.TARGET_IMAGE_DBX_URL;
        }
        else
        {
            imagePath = image.imageAttributes.sourceImage;
        }

        if (string.IsNullOrEmpty(imagePath))
        {
            return;
        }

        GameObject imageObject = new GameObject(); //Create the GameObject
        Image NewImage = imageObject.AddComponent<Image>(); //Add the Image Component script

        Sprite targetImageSprite = IMG2Sprite.LoadNewSprite(imagePath, 100f, SpriteMeshType.Tight);
        NewImage.sprite = targetImageSprite;

        imageObject.GetComponent<RectTransform>().SetParent(parent.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        //imageObject.name = "Image_" + image.id;
        imageObject.name = image.id;

        RectTransform rectTransform = parent.GetComponent<RectTransform>();
        float panel_width = rectTransform.rect.width;
        float panel_height = rectTransform.rect.height;

        float imageAspect = GlobalVariables.ASPECT_RATIO;
        float image_width = panel_width;
        float image_height = panel_height;
        imageObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        imageObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        imageObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(panel_width, panel_height);
        imageObject.transform.localScale = Vector3.one;
        //Debug.Log("<color=green> @@@@@@@@@ checking is the image is a primary Image .name is:   : </color>");
        if (isPrimaryImage)
        {
            GlobalVariables.PrimaryImageName = imageObject.name;
            //Debug.Log("<color=green> @@@@@@@@@ Image is a primary Image .name is:   : </color>");
            if (imageAspect > 1)
            {
                image_height = image_width / imageAspect;
            }
            else if (imageAspect < 1)
            {
                image_width = image_height * imageAspect;
            }
            else
            {
                image_width = image_height;
            }

            if (image_width > panel_width)
            {
                image_width = panel_width;
                image_height = image_width / imageAspect;
            }
            else if (image_height > panel_height)
            {
                image_height = panel_height;
                image_width = image_height * imageAspect;
            }

            imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(image_width, image_height);
            UIBuilder.infoAttachButton(imageObject);

            addBorder(imageObject.gameObject, image.border);
            buildChildren(image.children, imageObject.gameObject);
        }
        else
        {
            Debug.Log("<color=red> @@@@@@@@@ Image is not primary Image .name is:   : </color>");
        }
    }

    //===========


    public void buildDropdown(GameObject parent, UIDropDown uiDropdown)
    {
        try
        {
            Dropdown dropdown = ((GameObject)Instantiate(Resources.Load("UIDropdown"))).GetComponent<Dropdown>();
            //GameObject dropdownObj = ((GameObject)Instantiate(Resources.Load("UIDropdown"))).get;
            //dropdown.name = "dropdown-" + uiDropdown.elementId;
            dropdown.name = uiDropdown.elementId;

            float baseW = parent.GetComponent<RectTransform>().rect.width;
            float baseH = parent.GetComponent<RectTransform>().rect.height;
            float posX = uiDropdown.rectTransform.posX * baseW;// base_width;
            float posY = uiDropdown.rectTransform.posY * baseH;// base_height;
            float posZ = 0;

            float width = uiDropdown.rectTransform.width * baseW;
            float height = uiDropdown.rectTransform.height * baseH;

            string color = uiDropdown.imageAttributes.color;
            color = "#" + color;

            dropdown.transform.SetParent(parent.transform);

            dropdown.transform.localPosition = new Vector3(posX, posY, posZ);
            dropdown.transform.localScale = Vector3.one;

            if (!uiDropdown.visible)
            {
                dropdown.transform.localScale = Vector3.zero;
            }

            //dropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 120);
            dropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            Color newCol;
            if (ColorUtility.TryParseHtmlString(color, out newCol))
            {
                dropdown.GetComponent<Image>().color = newCol;
            }

            List<string> listOptions = new List<string>();
            foreach (UIText optionText in uiDropdown.options)
            {
                listOptions.Add(optionText.textAttribute.text);
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(listOptions);

            if (uiDropdown.layout != null && uiDropdown.layout.layoutType != null)
            {
                addLayOut(uiDropdown.layout, dropdown.gameObject, baseW, baseH);
            }
            addBorder(dropdown.gameObject, uiDropdown.border);
            buildChildren(uiDropdown.children, dropdown.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildButton: </color>" + e.ToString());
        }

    }

    public void buildInputField(GameObject parent, UIInputField uiInputField)
    {
        try
        {
            InputField input = ((GameObject)Instantiate(Resources.Load("UIInputField"))).GetComponent<InputField>();
            //input.name = "inputfield-" + uiInputField.elementId;
            input.name = uiInputField.elementId;

            float baseW = parent.GetComponent<RectTransform>().rect.width;
            float baseH = parent.GetComponent<RectTransform>().rect.height;
            float posX = uiInputField.rectTransform.posX * baseW;// base_width;
            float posY = uiInputField.rectTransform.posY * baseH;// base_height;
            float posZ = 0;


            float width = uiInputField.rectTransform.width * baseW;
            float height = uiInputField.rectTransform.height * baseH;

            //string color = uiButton.imageAttributes.color;
            input.transform.SetParent(parent.transform);
            input.transform.localPosition = new Vector3(posX, posY, posZ);
            input.transform.localScale = Vector3.one;

            if (!uiInputField.visible)
            {
                input.transform.localScale = Vector3.zero;
            }

            input.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            ColorBlock theColor = input.GetComponent<InputField>().colors;

            theColor.normalColor = getColor(uiInputField.transition.normalColor);
            //theColor.pressedColor = getColor(uiInputField.transition.pressedColor);
            //theColor.highlightedColor = getColor(uiInputField.transition.highlightedColor);
            theColor.pressedColor = theColor.normalColor;
            theColor.highlightedColor = Color.Lerp(theColor.normalColor, Color.black, .4f);
            theColor.disabledColor = Color.Lerp(theColor.normalColor, Color.white, .2f);
            input.GetComponent<InputField>().colors = theColor;

            UIText uiText = uiInputField.text;
            Debug.Log("<color=green> @@@@@@@@@@ buildInputField: before if </color>");
            if (uiText.textAttribute != null)
            {
                input.textComponent.text = uiText.textAttribute.text;
                input.text = uiText.textAttribute.text;
                input.textComponent.font.name = uiText.textAttribute.font;
                input.textComponent.fontStyle = getFontStyle(uiText.textAttribute.fontStyle);
                if (GlobalVariables.fontFactor > 0)
                {
                    input.textComponent.fontSize = uiText.textAttribute.fontSize * GlobalVariables.fontFactor;
                }
                else
                {
                    input.textComponent.fontSize = uiText.textAttribute.fontSize;
                }
                //input.textComponent.fontSize = uiText.textAttribute.fontSize;
                input.textComponent.lineSpacing = uiText.textAttribute.lineSpacing;
                input.textComponent.supportRichText = uiText.textAttribute.richText;
                input.textComponent.alignment = getTextAnchor(uiText.textAttribute.alignmentVertical, uiText.textAttribute.alignmentHorizontal);
                //input.textComponent.horizontalOverflow = getHorizontalWrapMode(uiText.textAttribute.horizontalOverflow);
                //input.textComponent.verticalOverflow = getVerticalWrapMode(uiText.textAttribute.verticalOverflow);
                input.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
                input.textComponent.verticalOverflow = VerticalWrapMode.Overflow;
                input.textComponent.resizeTextForBestFit = uiText.textAttribute.bestFit;
                input.textComponent.color = getRGBAColor(uiText.textAttribute.color);
            }
            else
            {
                Debug.Log("<color=green> @@@@@@@@@@ buildInputField: else </color>");
            }
            input.characterLimit = uiInputField.characterLimit;
            input.contentType = getContentType(uiInputField.contentType);
            input.lineType = getLineType(uiInputField.lineType);

            UIText placeHolder = uiInputField.placeholder;
            if (placeHolder != null && placeHolder.textAttribute != null)
            {
                Debug.Log("<color=green> @@@@@@@@@@ buildInputField: placeHolder.textAttribute.text : </color>" + placeHolder.textAttribute.text);

                input.placeholder.GetComponent<Text>().text = placeHolder.textAttribute.text;
                //input.text = placeHolder.textAttribute.text;
                input.placeholder.GetComponent<Text>().fontStyle = getFontStyle(placeHolder.textAttribute.fontStyle);
                input.placeholder.GetComponent<Text>().fontSize = placeHolder.textAttribute.fontSize;
                input.placeholder.GetComponent<Text>().lineSpacing = placeHolder.textAttribute.lineSpacing;
                input.placeholder.GetComponent<Text>().supportRichText = placeHolder.textAttribute.richText;
                input.placeholder.GetComponent<Text>().alignment = getTextAnchor(placeHolder.textAttribute.alignmentVertical, placeHolder.textAttribute.alignmentHorizontal);
                //input.placeholder.GetComponent<Text>().horizontalOverflow = getHorizontalWrapMode(placeHolder.textAttribute.horizontalOverflow);
                //input.placeholder.GetComponent<Text>().verticalOverflow = getVerticalWrapMode(placeHolder.textAttribute.verticalOverflow);
                //input.placeholder.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                //input.placeholder.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
                input.placeholder.GetComponent<Text>().resizeTextForBestFit = placeHolder.textAttribute.bestFit;
                input.placeholder.GetComponent<Text>().color = getRGBAColor(placeHolder.textAttribute.color);
            }
            input.readOnly = uiInputField.readOnly;

            if (uiInputField.layout != null && uiInputField.layout.layoutType != null)
            {
                addLayOut(uiInputField.layout, input.gameObject, baseW, baseH);
            }
            addBorder(input.gameObject, uiInputField.border);
            buildChildren(uiInputField.children, input.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildInputField: </color>" + e.ToString());
        }

    }

    public void buildToggle(GameObject parent, UIToggle uiToggle)
    {
        try
        {
            Toggle toggle = ((GameObject)Instantiate(Resources.Load("UIToggle"))).GetComponent<Toggle>();
            //toggle.name = "toggle-" + uiToggle.elementId;
            toggle.name = uiToggle.elementId;

            float baseW = parent.GetComponent<RectTransform>().rect.width;
            float baseH = parent.GetComponent<RectTransform>().rect.height;
            float posX = uiToggle.rectTransform.posX * baseW;// base_width;
            float posY = uiToggle.rectTransform.posY * baseH;// base_height;
            float posZ = 0;


            float width = uiToggle.rectTransform.width * baseW;
            float height = uiToggle.rectTransform.height * baseH;

            //string color = uiButton.imageAttributes.color;
            toggle.transform.SetParent(parent.transform);
            toggle.transform.localPosition = new Vector3(posX, posY, posZ);
            toggle.transform.localScale = Vector3.one;

            if (!uiToggle.visible)
            {
                toggle.transform.localScale = Vector3.zero;
            }

            toggle.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            ColorBlock theColor = toggle.GetComponent<Toggle>().colors;

            theColor.normalColor = getColor(uiToggle.transition.normalColor);
            //theColor.pressedColor = getColor(uiToggle.transition.pressedColor);
            //theColor.highlightedColor = getColor(uiToggle.transition.highlightedColor);
            theColor.pressedColor = theColor.normalColor;
            theColor.highlightedColor = Color.Lerp(theColor.normalColor, Color.black, .4f);
            theColor.disabledColor = Color.Lerp(theColor.normalColor, Color.white, .2f);
            toggle.GetComponent<Toggle>().colors = theColor;

            UIText label = uiToggle.label;
            Debug.Log("<color=green> @@@@@@@@@@ buildToggle: before if </color>");
            if (label.textAttribute != null)
            {
                toggle.GetComponentInChildren<Text>().text = label.textAttribute.text;
                toggle.GetComponentInChildren<Text>().font.name = label.textAttribute.font;
                toggle.GetComponentInChildren<Text>().fontSize = label.textAttribute.fontSize;
                toggle.GetComponentInChildren<Text>().lineSpacing = label.textAttribute.lineSpacing;
                toggle.GetComponentInChildren<Text>().supportRichText = label.textAttribute.richText;
                toggle.GetComponentInChildren<Text>().alignment = getTextAnchor(label.textAttribute.alignmentVertical, label.textAttribute.alignmentHorizontal);
                toggle.GetComponentInChildren<Text>().horizontalOverflow = getHorizontalWrapMode(label.textAttribute.horizontalOverflow);
                toggle.GetComponentInChildren<Text>().verticalOverflow = getVerticalWrapMode(label.textAttribute.verticalOverflow);
                toggle.GetComponentInChildren<Text>().resizeTextForBestFit = label.textAttribute.bestFit;
                toggle.GetComponentInChildren<Text>().color = getRGBAColor(label.textAttribute.color);
            }
            else
            {
                Debug.Log("<color=green> @@@@@@@@@@ buildInputField: else </color>");
            }
            toggle.isOn = uiToggle.isOn;
            toggle.toggleTransition = getToggleTransition(uiToggle.toggleTransition);

            addBorder(toggle.gameObject, uiToggle.border);
            buildChildren(uiToggle.children, toggle.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildToggle: </color>" + e.ToString());
        }

    }

    public void buildSlider(GameObject parent, UISlider uiSlider)
    {
        try
        {
            Slider slider = ((GameObject)Instantiate(Resources.Load("UISlider"))).GetComponent<Slider>();
            //slider.name = "slider-" + uiSlider.elementId;
            slider.name = uiSlider.elementId;

            float baseW = parent.GetComponent<RectTransform>().rect.width;
            float baseH = parent.GetComponent<RectTransform>().rect.height;
            float posX = uiSlider.rectTransform.posX * baseW;// base_width;
            float posY = uiSlider.rectTransform.posY * baseH;// base_height;
            float posZ = 0;


            float width = uiSlider.rectTransform.width * baseW;
            float height = uiSlider.rectTransform.height * baseH;

            //string color = uiButton.imageAttributes.color;
            slider.transform.SetParent(parent.transform);
            slider.transform.localPosition = new Vector3(posX, posY, posZ);
            slider.transform.localScale = Vector3.one;

            if (!uiSlider.visible)
            {
                slider.transform.localScale = Vector3.zero;
            }

            slider.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            ColorBlock theColor = slider.GetComponent<Slider>().colors;

            theColor.normalColor = getColor(uiSlider.transition.normalColor);
            //theColor.pressedColor = getColor(uiSlider.transition.pressedColor);
            //theColor.highlightedColor = getColor(uiSlider.transition.highlightedColor);
            theColor.pressedColor = theColor.normalColor;
            theColor.highlightedColor = Color.Lerp(theColor.normalColor, Color.black, .4f);
            theColor.disabledColor = Color.Lerp(theColor.normalColor, Color.white, .2f);
            slider.GetComponent<Slider>().colors = theColor;
            slider.direction = getDirection(uiSlider.direction);
            slider.minValue = uiSlider.minValue;
            slider.maxValue = uiSlider.maxValue;
            slider.wholeNumbers = uiSlider.wholeNumber;
            addBorder(slider.gameObject, uiSlider.border);
            buildChildren(uiSlider.children, slider.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildToggle: </color>" + e.ToString());
        }

    }

    public void buildPopup(GameObject parent, UIPopUp uiPopup)
    {
        try
        {
            GameObject popup = (GameObject)Instantiate(Resources.Load("UIPopupYesNo"));

            //IAPopupScript popupScript = new IAPopupScript(uiPopup.message.textAttribute., popup);
            //IAPopupScript popupScript = popup.AddComponent<IAPopupScript>();
            IAPopupScript popupScript = popup.GetComponent<IAPopupScript>();
            popupScript.PopWindow = popup;
            popupScript.message = uiPopup.message.textAttribute.text;
            //popup.AddComponent<popupScript>();
            popup.transform.SetParent(parent.transform);
            popup.transform.localScale = Vector3.one;
            //popupScript.CallMe();
            //buildChildren(uiPopup.children, popup.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildPopup: </color>" + e.ToString());
        }
    }

    public void buildPopup2(GameObject parent, string message)
    {
        try
        {
            GameObject gObj = PopupUtilities.makePopupYesNo(null, null, message, true, null);

        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildPopup: </color>" + e.ToString());
        }
    }

    public void buildPopup1(GameObject parent, string message, GameObject button)
    {
        try
        {
            GameObject popup = (GameObject)Instantiate(Resources.Load("PopUpCanvas"));

            //IAPopupScript popupScript = new IAPopupScript(uiPopup.message.textAttribute., popup);
            IAPopupScript popupScript = popup.GetComponent<IAPopupScript>();
            //popupScript.PopWindow = popup;
            popupScript.message = message;
            //popup.transform.SetParent(mainCanvas.transform);
            //button.transform.position
            //popup.transform.SetParent(button.transform);
            //popup.transform.localPosition = mainCanvas.transform.localPosition;
            //popup.transform.localScale = Vector3.one;
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildPopup: </color>" + e.ToString());
        }
    }

    public void buildText(GameObject parent, UIText uiText)
    {
        try
        {
            Text text = ((GameObject)Instantiate(Resources.Load("UIText"))).GetComponent<Text>();
            //GameObject dropdownObj = ((GameObject)Instantiate(Resources.Load("UIDropdown"))).get;
            //text.name = "text-" + uiText.elementId;
            text.name = uiText.elementId;
            Text innerText = text.gameObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>();
            Image backgroundPanel = text.gameObject.transform.GetChild(0).GetComponent<Image>();
            innerText.name = "ChildText";

            if (!uiText.visible)
            {
                text.transform.localScale = Vector3.zero;
            }

            if (uiText.rectTransform != null)
            {
                float baseW = parent.GetComponent<RectTransform>().rect.width;
                float baseH = parent.GetComponent<RectTransform>().rect.height;
                float posX = uiText.rectTransform.posX * baseW;// base_width;
                float posY = uiText.rectTransform.posY * baseH;// base_height;
                float posZ = 0;

                float width = uiText.rectTransform.width * baseW;
                float height = uiText.rectTransform.height * baseH;
                text.transform.localPosition = new Vector3(posX, posY, posZ);
                text.transform.localScale = Vector3.one;
                text.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            }
            else
            {
                Debug.Log("<color=red> rect transform in text is not present: </color>");
            }

            innerText.GetComponent<Text>().text = uiText.textAttribute.text;



            //Setting Colors
            string textColor = uiText.textAttribute.color;
            textColor = "#" + textColor;

            string bgColor = uiText.textAttribute.backgroundColor;
            bgColor = "#" + bgColor;
            Color newCol;


            //Setting Background Panel Color

            if (!string.IsNullOrEmpty(bgColor) && ColorUtility.TryParseHtmlString(bgColor, out newCol))
            {
                backgroundPanel.color = newCol;
            }

            //Setting Text  Color
            if (!string.IsNullOrEmpty(textColor) && ColorUtility.TryParseHtmlString(textColor, out newCol))
            {
                innerText.GetComponent<Text>().color = newCol;
            }

            //Setting Text Font and Styles

            innerText.GetComponent<Text>().fontStyle = FontStyle.Italic;


            text.transform.SetParent(parent.transform);
            buildChildren(uiText.children, text.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildText: </color>" + e.ToString());
        }

    }

    public void buildText1(GameObject parent, UIText uiText)
    {
        try
        {
            Text text = ((GameObject)Instantiate(Resources.Load("UIText"))).GetComponent<Text>();
            //text.name = "text-" + uiText.elementId;
            text.name = uiText.elementId;
            if (uiText.rectTransform != null)
            {
                float baseW = parent.GetComponent<RectTransform>().rect.width;
                float baseH = parent.GetComponent<RectTransform>().rect.height;
                float posX = uiText.rectTransform.posX * baseW;// base_width;
                float posY = uiText.rectTransform.posY * baseH;// base_height;
                float posZ = 0;

                float width = uiText.rectTransform.width * baseW;
                float height = uiText.rectTransform.height * baseH;
                text.transform.localPosition = new Vector3(posX, posY, posZ);
                text.transform.localScale = Vector3.one;
                text.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            }
            else
            {
                Debug.Log("<color=red> rect transform in text is not present: </color>");
            }
            if (uiText.textAttribute != null)
            {
                text.text = uiText.textAttribute.text;
                Debug.Log("<color=green> setting text to : </color>" + uiText.textAttribute.text);
                Debug.Log("<color=green> setting text to : </color>" + text.text);
                text.font.name = uiText.textAttribute.font;
                text.fontStyle = getFontStyle(uiText.textAttribute.fontStyle);
                text.fontSize = uiText.textAttribute.fontSize;
                text.lineSpacing = uiText.textAttribute.lineSpacing;
                text.supportRichText = uiText.textAttribute.richText;
                text.alignment = getTextAnchor(uiText.textAttribute.alignmentVertical, uiText.textAttribute.alignmentHorizontal);
                text.horizontalOverflow = getHorizontalWrapMode(uiText.textAttribute.horizontalOverflow);
                text.verticalOverflow = getVerticalWrapMode(uiText.textAttribute.verticalOverflow);
                text.resizeTextForBestFit = uiText.textAttribute.bestFit;
                text.color = getRGBAColor(uiText.textAttribute.color);
            }
            else
            {
                Debug.Log("<color=red> textAttribute in text is not present: </color>");
            }

            if (!uiText.visible)
            {
                text.transform.localScale = Vector3.zero;
            }

            //text.transform.SetParent(parent.transform);
            text.GetComponent<RectTransform>().SetParent(parent.transform);
            buildChildren(uiText.children, text.gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@ An error occurred in buildText: </color>" + e.ToString());
        }

    }

    private void addLayOut(UILayout layout, GameObject gameObject, float baseW, float baseH)
    {
        float percentFactor = 0.01f;
        //Debug.Log("<color=green> @@@@ uiPanel has layout:   : </color>" + uiPanel.layout.layoutType);	
        if (layout.layoutType.Equals("Vertical"))
        {
            VerticalLayoutGroup layoutGrp = gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGrp.CalculateLayoutInputVertical();
            layoutGrp.SetLayoutVertical();
            layoutGrp.padding.left = layout.leftPadding * (int)Math.Round(percentFactor * baseW);
            layoutGrp.padding.right = layout.rightPadding * (int)Math.Round(percentFactor * baseW);
            layoutGrp.padding.top = layout.topPadding * (int)Math.Round(percentFactor * baseH);
            layoutGrp.padding.bottom = layout.bottomPadding * (int)Math.Round(percentFactor * baseH);
            layoutGrp.spacing = layout.spacing * (int)Math.Round(percentFactor * baseH);
            layoutGrp.childAlignment = getLayoutAlignment(layout.justification + layout.alignment);
            layoutGrp.childControlWidth = false;//uiPanel.layout.controlChildSizeWidth;	
            layoutGrp.childControlHeight = false;//uiPanel.layout.controlChildSizeHeight;	
                                                 //layoutGrp.childScaleWidth = uiPanel.layout.userChildScaleWidth;	
                                                 //layoutGrp.childScaleHeight = uiPanel.layout.userChildScaleHeight;	
            layoutGrp.childForceExpandWidth = false;//uiPanel.layout.childForceExpandWidth;	
            layoutGrp.childForceExpandHeight = false;//uiPanel.layout.childForceExpandHeight;	
        }
        else if (layout.layoutType.Equals("Horizontal"))
        {
            HorizontalLayoutGroup layoutGrp = gameObject.AddComponent<HorizontalLayoutGroup>();
            layoutGrp.CalculateLayoutInputHorizontal();
            layoutGrp.SetLayoutHorizontal();
            layoutGrp.padding.left = layout.leftPadding * (int)Math.Round(percentFactor * baseW);
            layoutGrp.padding.right = layout.rightPadding * (int)Math.Round(percentFactor * baseW);
            layoutGrp.padding.top = layout.topPadding * (int)Math.Round(percentFactor * baseH);
            layoutGrp.padding.bottom = layout.bottomPadding * (int)Math.Round(percentFactor * baseH);
            layoutGrp.spacing = layout.spacing * (int)Math.Round(percentFactor * baseW);
            layoutGrp.childAlignment = getLayoutAlignment(layout.alignment + layout.justification);
            layoutGrp.childControlWidth = false;//uiPanel.layout.controlChildSizeWidth;	
            layoutGrp.childControlHeight = false;//uiPanel.layout.controlChildSizeHeight;	
                                                 //layoutGrp.childScaleWidth = uiPanel.layout.userChildScaleWidth;	
                                                 //layoutGrp.childScaleHeight = uiPanel.layout.userChildScaleHeight;	
            layoutGrp.childForceExpandWidth = false;//uiPanel.layout.childForceExpandWidth;	
            layoutGrp.childForceExpandHeight = false;//uiPanel.layout.childForceExpandHeight;	
        }
    }
    private void addBorder(GameObject gameObject, ElementBorder border)
    {
        if (border != null && border.color != null && border.width > 0)
        {
            Debug.Log("<color=green> @@@@@@@@@@ Inside adBorder,  gameObject is: </color>" + gameObject.name);
            Debug.Log("<color=green> @@@@@@@@@@ Inside adBorder,  border.color is: </color>" + border.color);
            Debug.Log("<color=green> @@@@@@@@@@ Inside adBorder,  border.isBorderPresent is: </color>" + border.borderPresent);

            GameObject uiBorder = (GameObject)Instantiate(Resources.Load("UIBorder"));
            Debug.Log("<color=green> @@@@@@@@@@ Inside adBorder,  uiBorder is: </color>" + uiBorder.name);
            uiBorder.transform.SetParent(gameObject.transform);
            BorderScript bs = uiBorder.GetComponent<BorderScript>();
            bs.borderColor = getColor(border.color);
            bs.borderWidth = border.width;
            bs.enabled = border.borderPresent;
            bs.setBorders();
            RectTransform rectTransform = uiBorder.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(0, 0);
            rectTransform.offsetMax = new Vector2(0, 0);
        }
    }

    public static Color getRGBAColor(String color)
    {
        //color = "rgba(255, 255, 255, 1)";
        color = color.Replace(" ", string.Empty);
        color = color.Replace("rgba(", string.Empty);
        color = color.Replace(")", string.Empty);

        string[] words = color.Split(',');
        float r = 1;
        float g = 1;
        float b = 1;
        float a = 1;
        try
        {
            if (words.Length == 4)
            {
                r = float.Parse(words[0]) / 255;
                g = float.Parse(words[1]) / 255;
                b = float.Parse(words[2]) / 255;
                a = float.Parse(words[3]);
            }
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@  Exception in Building RGBA color without space  is : </color>" + e.StackTrace);
            r = 1; b = 1; g = 1; a = 1;
        }

        Color newCol = new Color(r, g, b, a);

        return newCol;

    }

    public static Color getColor(String color)
    {
        if (color.IndexOf("rgba") == 0)
        {
            //Debug.Log("<color=red> @@@@@@@@@@  ITS RGBA COLOR :  </color>" + color);
            return getRGBAColor(color);
        }


        Color newCol;
        if (string.IsNullOrEmpty(color))
        {
            color = "#000000";
        }
        else
        {
            // Debug.Log("<color=red> @@@@@@@@@@  getColor Color is : </color>" + color);
            if (color.IndexOf("#") != -1)
            {
                color = color.Substring(color.LastIndexOf("#"));
            }
            else
            {
                color = "#" + color;
            }
            // Debug.Log("<color=green> @@@@@@@@@@  getColor changed Color is : </color>" + color);

        }
        if (ColorUtility.TryParseHtmlString(color, out newCol))
        {
            return newCol;
        }
        return newCol;

    }

    public static FontStyle getFontStyle(String style)
    {
        switch (style)
        {
            case "Normal":
                return FontStyle.Normal;
            case "Bold":
                return FontStyle.Bold;
            case "Italic":
                return FontStyle.Italic;
            case "BoldAndItalic":
                return FontStyle.BoldAndItalic;
            default:
                return FontStyle.Normal;
        }
    }

    private TextAnchor getTextAnchor(String vertical, String horizontal)
    {
        switch (vertical + horizontal)
        {
            case "TopLeft":
                return TextAnchor.UpperLeft;
            case "TopCenter":
                return TextAnchor.UpperCenter;
            case "TopRight":
                return TextAnchor.UpperRight;
            case "CenterLeft":
                return TextAnchor.MiddleLeft;
            case "CenterCenter":
                return TextAnchor.MiddleCenter;
            case "CenterRight":
                return TextAnchor.MiddleRight;
            case "BottomLeft":
                return TextAnchor.LowerLeft;
            case "BottomCenter":
                return TextAnchor.LowerCenter;
            case "BottomRight":
                return TextAnchor.LowerRight;
            default:
                return TextAnchor.UpperLeft;
        }
    }

    private TextAnchor getLayoutAlignment(String alignment)
    {
        switch (alignment)
        {
            case "StartStart":
                return TextAnchor.UpperLeft;
            case "StartCenter":
                return TextAnchor.UpperCenter;
            case "StartEnd":
                return TextAnchor.UpperRight;
            case "CenterStart":
                return TextAnchor.MiddleLeft;
            case "CenterCenter":
                return TextAnchor.MiddleCenter;
            case "CenterEnd":
                return TextAnchor.MiddleRight;
            case "EndStart":
                return TextAnchor.LowerLeft;
            case "EndCenter":
                return TextAnchor.LowerCenter;
            case "EndEnd":
                return TextAnchor.LowerRight;
            default:
                return TextAnchor.UpperLeft;
        }
    }

    private HorizontalWrapMode getHorizontalWrapMode(String mode)
    {
        switch (mode)
        {
            case "Wrap":
                return HorizontalWrapMode.Wrap;
            case "Overflow":
                return HorizontalWrapMode.Overflow;
            default:
                return HorizontalWrapMode.Wrap;
        }
    }

    private VerticalWrapMode getVerticalWrapMode(String mode)
    {
        switch (mode)
        {
            case "Truncate":
                return VerticalWrapMode.Truncate;
            case "Overflow":
                return VerticalWrapMode.Overflow;
            default:
                return VerticalWrapMode.Truncate;
        }
    }

    private InputField.ContentType getContentType(String content)
    {
        switch (content)
        {
            case "Standard":
                return InputField.ContentType.Standard;
            case "AutoCorrected":
                return InputField.ContentType.Autocorrected;
            case "IntegerNumber":
                return InputField.ContentType.IntegerNumber;
            case "DecimalNumber":
                return InputField.ContentType.DecimalNumber;
            case "AlphaNumeric":
                return InputField.ContentType.Alphanumeric;
            case "Name":
                return InputField.ContentType.Name;
            case "EmailAddress":
                return InputField.ContentType.EmailAddress;
            case "Password":
                return InputField.ContentType.Password;
            case "Pin":
                return InputField.ContentType.Pin;
            case "Custom":
                return InputField.ContentType.Custom;
            default:
                return InputField.ContentType.Standard;
        }
    }

    private InputField.LineType getLineType(String type)
    {
        switch (type)
        {
            case "SingleLine":
                return InputField.LineType.SingleLine;
            case "MultiLineSubmit":
                return InputField.LineType.MultiLineSubmit;
            case "MultiLineNewLine":
                return InputField.LineType.MultiLineNewline;
            default:
                return InputField.LineType.SingleLine;
        }
    }

    private Toggle.ToggleTransition getToggleTransition(String transition)
    {
        switch (transition)
        {
            case "Fade":
                return Toggle.ToggleTransition.Fade;
            case "None":
                return Toggle.ToggleTransition.None;
            default:
                return Toggle.ToggleTransition.Fade;
        }
    }

    private Slider.Direction getDirection(String direction)
    {
        switch (direction)
        {
            case "LeftToRight":
                return Slider.Direction.LeftToRight;
            case "RightToLeft":
                return Slider.Direction.RightToLeft;
            case "BottomToTop":
                return Slider.Direction.BottomToTop;
            case "TopToBottom":
                return Slider.Direction.TopToBottom;
            default:
                return Slider.Direction.LeftToRight;
        }
    }

    private RenderMode getRenderMode(String mode)
    {
        switch (mode)
        {
            case "Screen Space - Overlay":
                return RenderMode.ScreenSpaceOverlay;
            case "Screen Space - Camera":
                return RenderMode.ScreenSpaceCamera;
            case "World Space":
                return RenderMode.WorldSpace;
            default:
                return RenderMode.ScreenSpaceOverlay;
        }
    }

    public static string saveSecondaryImage(string file)
    {
        WebFunctions web = new WebFunctions();
        string secondaryImagePath = Path.Combine(Application.persistentDataPath, "secondaryImages");
        if (!Directory.Exists(secondaryImagePath))
        {
            Directory.CreateDirectory(secondaryImagePath);
        }

        String imagePath = "";
        if (file.Contains(".png") || file.Contains(".PNG"))
        {
            int position = file.LastIndexOf(".png", StringComparison.OrdinalIgnoreCase);
            String imageIndex = file.Substring(0, position + 4);
            String imageName = Path.GetFileName(imageIndex);
            imagePath = Path.Combine(secondaryImagePath, imageName);
            if (!File.Exists(imagePath))
            {
                web.downloadImage(file, imagePath);
            }
        }
        else if (file.Contains(".jpeg") || file.Contains(".JPEG"))
        {
            int position = file.LastIndexOf(".jpeg", StringComparison.OrdinalIgnoreCase);
            String imageIndex = file.Substring(0, position + 5);
            String imageName = Path.GetFileName(imageIndex);
            imagePath = Path.Combine(secondaryImagePath, imageName);
            if (!File.Exists(imagePath))
            {
                web.downloadImage(file, imagePath);
            }
        }
        else if (file.Contains(".jpg") || file.Contains(".JPG"))
        {
            int position = file.LastIndexOf(".jpg", StringComparison.OrdinalIgnoreCase);
            String imageIndex = file.Substring(0, position + 4);
            String imageName = Path.GetFileName(imageIndex);
            imagePath = Path.Combine(secondaryImagePath, imageName);
            if (!File.Exists(imagePath))
            {
                web.downloadImage(file, imagePath);
            }
        }
        return imagePath;
    }

    public static Sprite loadNewSprite(string filePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        Texture2D spriteTexture;
        if (filePath.StartsWith(Application.persistentDataPath))
        {
            WebFunctions web = new WebFunctions();
            byte[] imageBytes = web.loadImage(filePath);
            spriteTexture = new Texture2D(2, 2);
            spriteTexture.LoadImage(imageBytes);
        }
        else
        {
            WWW request = new WWW(filePath);
            while (!request.isDone) { }
            spriteTexture = request.texture;
        }
        Sprite NewSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);
        return NewSprite;
    }

    private async Task loadAppResourcesAsync(UIApplication app) //A Task return type will eventually yield a void
    {
        //await GetStringAsync(app); // we are awaiting the Async Method GetStringAsync
        //IEnumerator coroutine;
        //coroutine = WaitAndPrint(10.0f, app);
        //StartCoroutine(coroutine);
        //WaitAndPrint(1.0f, app);
        await Task.Run(() => GetStringAsync(app));
        //GetStringAsync(app); // we are awaiting the Async Method GetStringAsync
    }

    private async Task WaitAndPrint(float waitTime, UIApplication app)
    {

        while (!processed)
        {
            Debug.Log("<color=green> @@@@@@@@@@  processed is : </color>" + processed + " Sleeping again for 1 sec");
            //yield return new WaitForSeconds(1.0f);
            await Task.Delay(1000);
        }
        //yield return new WaitForSeconds(waitTime);

        GetStringAsync(app);
        Debug.Log("<color=green> @@@@@@@@@@  processed is : </color>" + processed + " all done");
    }

    private void GetStringAsync(UIApplication app)
    {
        processed = false;
        string imageId = app.imageUniqueId;
        string imagePath = Path.Combine(Application.persistentDataPath, imageId);
        //Debug.Log("<color=green> @@@@@@@@@@  GetStringAsync secondaryImagePath is : </color>" + imagePath);
        if (!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }
        string uiApiURL = GlobalVariables.UI_API + imageId;
        var directResponse = WebFunctions.Get(uiApiURL);

        //Lets add app json
        if (directResponse.error == null)
        {
            string appJson = Path.Combine(imagePath, imageId + ".json");
            File.WriteAllText(appJson, directResponse.text);
        }

        //lets add workspace xml
        string workspaceURL = GlobalVariables.UI_WorkspaceAPI + imageId;
        var workspaceResponse = WebFunctions.Get(workspaceURL);
        if (workspaceResponse.error == null)
        {
            if (workspaceResponse.text.Length > 1)
            {
                string workspace = workspaceResponse.text;
                string workspacePath = Path.Combine(imagePath, imageId + ".xml");
                File.WriteAllText(workspacePath, workspaceResponse.text);
                if (workspaceResponse.text.Contains("ARQuery:"))
                {
                    //findAndStoreQueries(workspaceResponse.text, imagePath);
                }
                else
                {
                    //Debug.Log("<color=green> @@@@@@@@@@  findAndStoreARQueries imagePath is : </color>" + imagePath);
                    //findAndStoreARQueries(workspaceResponse.text, imagePath);
                }
            }
        }
        //Debug.Log("<color=green> @@@@@@@@@@ GetStringAsync processed is : </color>" + processed);
        processed = true;
    }

    void findAndStoreQueries(string blocklyWorkspace, string imagePath)
    {
        List<string> queryList = new List<string>();
        string[] separatingStrings = { "ARQuery:" };
        string[] queries = blocklyWorkspace.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < queries.Length; i++)
        {
            string query = queries[i];
            if (query.IndexOf("</") > 1)
            {
                queryList.Add(query);
            }
        }
        foreach (string query in queryList)
        {
            if (query.Length > 1)
            {
                //query backend and get the results
                string queryString = query;
                queryString = Uri.EscapeDataString(queryString);

                var queryResponse = WebFunctions.Get(GlobalVariables.GET_Query_Results + queryString);
                if (queryResponse.error == null)
                {
                    if (queryResponse.text != null && queryResponse.text.Length > 1)
                    {
                        string queryPath = Path.Combine(imagePath, query + ".json");
                        File.WriteAllText(queryPath, queryResponse.text);
                    }
                }
            }
        }
    }

    void findAndStoreARQueries(string blocklyWorkspace, string imagePath)
    {
        List<string> queryList = new List<string>();
        XNamespace ns = "https://developers.google.com/blockly/xml";
        XElement element = XElement.Parse(blocklyWorkspace);
        IEnumerable<XElement> arQueryAllElements = element.Descendants(ns + "block").Where(child => child.Attribute("type").Value.Equals("ARQueryAll"));
        IEnumerable<XElement> arQueryElements = element.Descendants(ns + "block").Where(child => child.Attribute("type").Value.Equals("ARQuery"));
        foreach (XElement elem in arQueryAllElements)
        {
            XElement e = getXElementWithNameSpace(elem);
            //string query = e.Descendants(ns + "field").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault()?.Value;
            XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
            string query = null;
            object obj = null;
            if (queryNameValue != null)
            {
                XElement qnblock = queryNameValue.Element(BlocklyUtil.ns + "block");
                XElement shadowBlock = queryNameValue.Element(BlocklyUtil.ns + "shadow");
                if (qnblock != null && qnblock.Attribute("type") != null)
                {
                    obj = eventObj.parseBlock(qnblock);
                }
                else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
                {
                    obj = eventObj.parseBlock(shadowBlock);
                }
                if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
                {
                    query = ((BlocklyReference)obj).value.ToString();
                }
                else
                {
                    query = obj.ToString();
                }
            }

            if (!queryList.Contains(query))
            {
                queryList.Add(query);
            }
        }
        foreach (XElement elem in arQueryElements)
        {
            XElement e = getXElementWithNameSpace(elem);
            //string query = e.Descendants(ns + "field").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault()?.Value;
            XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
            string query = null;
            object obj = null;
            if (queryNameValue != null)
            {
                XElement qnblock = queryNameValue.Element(BlocklyUtil.ns + "block");
                XElement shadowBlock = queryNameValue.Element(BlocklyUtil.ns + "shadow");
                if (qnblock != null && qnblock.Attribute("type") != null)
                {
                    obj = eventObj.parseBlock(qnblock);
                }
                else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
                {
                    obj = eventObj.parseBlock(shadowBlock);
                }
                if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
                {
                    query = ((BlocklyReference)obj).value.ToString();
                }
                else
                {
                    query = obj.ToString();
                }
            }

            if (!queryList.Contains(query))
            {
                queryList.Add(query);
            }
        }
        Debug.Log("<color=green> @@@@@@@@@@  findAndStoreARQueries queryList count is : </color>" + queryList.Count);
        foreach (string query in queryList)
        {
            if (query.Length > 1)
            {
                //query backend and get the results
                string queryName = query;
                queryName = Uri.EscapeDataString(queryName);
                Debug.Log("<color=green> @@@@@@@@@@  findAndStoreARQueries queryName is : </color>" + queryName);
                //var queryResponse = WebFunctions.Get(GlobalVariables.GET_Query_Results + "ALL/" + queryName);	
                WWW www = new WWW(GlobalVariables.GET_Query_Results + "ALL/" + queryName);

                StartCoroutine(ShowProgress(www, queryName, imagePath));
                string queryPath = Path.Combine(imagePath, queryName + ".json");
                //File.WriteAllText(queryPath, queryResponse);
                //if (queryResponse.error == null)
                //{
                //    if (queryResponse.text != null && queryResponse.text.Length > 1)
                //    {
                //        string queryPath = Path.Combine(imagePath, queryName + ".json");
                //        File.WriteAllText(queryPath, queryResponse.text);
                //    }
                //} else
                //{
                //    Debug.Log("<color=red> @@@@@@@@@@  findAndStoreARQueries BAD response! queryName is : </color>" + queryName);
                //}
            }
        }
    }
    private IEnumerator ShowProgress(WWW queryResponse, string queryName, string imagePath)
    {
        Debug.Log(">>>>>>>>>>>>>>>>>>>>Inside ShowProgress ");
        while (!queryResponse.isDone)
        {
            yield return new WaitForSeconds(.1f);
        }
        if (queryResponse.error == null)
        {
            Debug.Log("<color=white>   >>>>>>success: </color>" + queryResponse.text);
            if (queryResponse.error == null)
            {
                if (queryResponse.text != null && queryResponse.text.Length > 1)
                {
                    string queryPath = Path.Combine(imagePath, queryName + ".json");
                    File.WriteAllText(queryPath, queryResponse.text);
                }
            }
            else
            {
                Debug.Log("<color=red> @@@@@@@@@@  findAndStoreARQueries BAD response! queryName is : </color>" + queryName);
            }
        }
        else
        {
            Debug.Log("<color=white>   >>>>>>something wrong:  </color>" + queryResponse.text);

        }

    }

    XElement getXElementWithNameSpace(XElement element)
    {
        string s = element.ToString();
        if (!s.StartsWith("<xml"))
        {
            s = "<xml xmlns=\"https://developers.google.com/blockly/xml\"> \n" + s + "\n</xml>";
            element = XElement.Parse(s);
        }
        return element;
    }
}