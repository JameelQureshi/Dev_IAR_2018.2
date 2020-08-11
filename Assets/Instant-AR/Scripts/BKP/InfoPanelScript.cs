/*hooda===============================================================================
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.
 
Copyright (c) 2010-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class InfoPanelScript : MonoBehaviour
{
    #region PRIVATE_MEMBERS

    private GameObject canvas;
    private GameObject newCanvas;
    private List<TargetInfo> targetInfoSet;
    private bool isPublic = true;
    #endregion // PRIVATE_MEMBERS

    #region PUBLIC_MEMBERS
    public GameObject sphere;
    public Button infoButton;
    public Button infoUnpressButton;
    public GameObject quad;
    public GameObject imageButton;
    public GameObject youTubeButton;
    public Button videoButton;
    public CloudRecoEventHandler m_CloudRecoEventHandler;
    #endregion // PUBLIC_MEMBERS


    void Start()
    {
        canvas = sphere.GetComponent<attachButton>().canvas;
        Debug.Log("<color=red> $$$$$$ Inside InfoPanelScript start()  </color>" + canvas.name);

    }

    public void TrackingFound()
    {
        showInfoButton();
        videoButton.transform.localScale = new Vector3(0, 0, 0);
        youTubeButton.transform.localScale = new Vector3(0, 0, 0);
        imageButton.transform.localScale = new Vector3(0, 0, 0);
        //sphere.GetComponent<attachButton>().canvas = makeCanvas();
        //canvas = sphere.GetComponent<attachButton>().canvas;
    }

    public void TrackingLost()
    {
        hideInfoButton();
        hideVideoButton();
        GlobalVariables.INFO_BUTTON_CLICKED = false;
        if (imageButton != null)
            imageButton.transform.localScale = new Vector3(1, 1, 1);
        Destroy(canvas);

    }

    GameObject makeCanvas()
    {
        GameObject canvasPrefab = (GameObject)Resources.Load("InfoCanvasPrefab");
        GameObject newCanvas = Instantiate(canvasPrefab);
        newCanvas.name = "INFO_CANVAS";
        Debug.Log("<color=red> ???????newCanvas is created  </color>" + newCanvas.name);
        newCanvas.SetActive(false);
        newCanvas.transform.localScale = new Vector3(1, 1, 1);
        addButtons(newCanvas);
        //newCanvas.transform.SetParent(sphere.transform);
        return newCanvas;

    }

    void addButtons(GameObject canvas1)
    {
        int rowCounts = 10;
        targetInfoSet = new List<TargetInfo>();

        List<string> buttonList = new List<string>();
        ButtonInfoMAP buttonInfoMAP;
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
        string buttonName;
        for (int i = 0; i < 100; i++)
        {
            buttonName = "Button_" + i;
            string targetInfoString = (string)buttonInfoMAP.GetType().GetField(buttonName).GetValue(buttonInfoMAP);
            //Debug.Log("<color=red> ??????? targetInfoString is   </color>" + targetInfoString);
            if (!string.IsNullOrEmpty(targetInfoString))
            {
                TargetInfo targetInfo = JsonUtility.FromJson<TargetInfo>(targetInfoString);
                string infovalue = targetInfo.string_value3;
                //Debug.Log("<color=red> ??????? buttonName is   </color>" + buttonName);
                //Debug.Log("<color=red> ??????? infovalue is   </color>" + infovalue);
                buttonList.Add(buttonName);
                targetInfoSet.Add(targetInfo);
                //if (!string.IsNullOrEmpty(infovalue))
                //{
                //    buttonList.Add(buttonName);
                //    targetInfoSet.Add(targetInfo);
                //}

            }

        }

        if (canvas1 != null)
        {
            foreach (var button in buttonList)
            {
                GameObject buttonPrefab = (GameObject)Resources.Load("ButtonPrefab");
                GameObject newButton = (GameObject)Instantiate(buttonPrefab);
                newButton.name = button;
                newButton.transform.SetParent(canvas1.transform);
            }
        }
    }




    public void clicked()
    {
        Debug.Log("<color=black> ??????? OKKKKKK>>> InfoPanelScript>> I am inside clicked  </color>");
        GlobalVariables.INFO_BUTTON_CLICKED = true;
        GlobalVariables.VIDEO_BUTTON_CLICKED = false;
        if (m_CloudRecoEventHandler != null)
        {
            m_CloudRecoEventHandler.TrackingLost();
        }
        sphere.GetComponent<attachButton>().canvas = makeCanvas();
        sphere.GetComponent<attachButton>().targetInfoSet = targetInfoSet;
        canvas = sphere.GetComponent<attachButton>().canvas;
        quad.transform.localScale = new Vector3(0, 0, 0);
        //canvas = sphere.GetComponent<attachButton>().canvas;
        if (canvas != null)
            canvas.SetActive(true);
        Debug.Log("<color=red> ???????canvas inside clicked  </color>");
        Debug.Log("<color=red> ???????canvas  </color>" + canvas.name);
        infoUnpressButton.transform.localScale = new Vector3(1, 1, 1);
        infoButton.transform.localScale = new Vector3(0, 0, 0);
        //GlobalVariables.INFO_BUTTON_CLICKED = true;
    }

    public void UnClicked()
    {
        Debug.Log("<color=black> ??????? OKKKKKK>>> InfoPanelScript>> I am inside UnClicked  </color>");
        GlobalVariables.INFO_BUTTON_CLICKED = false;
        GlobalVariables.VIDEO_BUTTON_CLICKED = true;
        quad.transform.localScale = new Vector3(1, 1, 1);
        if (m_CloudRecoEventHandler != null)
        {
            m_CloudRecoEventHandler.TrackingFound();
        }
        if (canvas != null)
            canvas.SetActive(false);
        infoButton.transform.localScale = new Vector3(1, 1, 1);
        infoUnpressButton.transform.localScale = new Vector3(0, 0, 0);
        //GlobalVariables.INFO_BUTTON_CLICKED = false;
        hideOtherObjects();
    }

    public void EnableInfoButton()
    {
        Debug.Log("<color=red> EnableInfoButton: </color>");
        if (infoButton != null)
        {
            //infoButton.gameObject.SetActive(false);
            infoButton.transform.localScale = new Vector3(0, 0, 0);
        }
    }
    public void showInfoButton()
    {
        Debug.Log("<color=blue>>>><<<<<<<<<<<<< Inside  showInfoButton </color>");
        if (infoButton != null)
        {
            infoButton.transform.localScale = new Vector3(1, 1, 1);
        }

    }
    public void hideInfoButton()
    {
        if (infoButton != null)
        {
            infoButton.transform.localScale = new Vector3(0, 0, 0);
            if (canvas != null)
                canvas.SetActive(false);
        }
        hideOtherObjects();
    }
    public void hideOtherObjects()
    {
        sphere.GetComponent<attachButton>().infoEditButton.transform.localScale = new Vector3(0, 0, 0);
        //sphere.GetComponent<attachButton>().heathButton.transform.localScale = new Vector3(0, 0, 0);
        sphere.GetComponent<attachButton>().sliderPanel.SetActive(false);
        sphere.GetComponent<attachButton>().sliderPanel.transform.localScale = new Vector3(0, 0, 0);
        //sphere.GetComponent<attachButton>().infoTextSizeSlider.transform.localScale = new Vector3(0, 0, 0);
        //sphere.GetComponent<attachButton>().smallFontSymbol.transform.localScale = new Vector3(0, 0, 0);
        //sphere.GetComponent<attachButton>().bigFontSymbol.transform.localScale = new Vector3(0, 0, 0);
    }
    public void hideVideoButton()
    {
        if (infoUnpressButton != null)
        {
            infoUnpressButton.transform.localScale = new Vector3(0, 0, 0);
        }


    }
}
