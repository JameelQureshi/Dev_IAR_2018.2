/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */

using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;
public class CloudRecoTrackableEventHandler : DefaultTrackableEventHandler
{
    #region PRIVATE_MEMBERS
    private CloudRecoEventHandler m_CloudRecoEventHandler;
    private CloudRecoBehaviour m_CloudRecoBehaviour;

    private CommonBarScript m_CommonBarScript;
    //private InfoPanelScript infoPanelScript;

    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_MEMBERS
    public GameObject UIScreen;
    //public Button cloudScanButton;
    //public Button PrimaryScreenButton;

    //declare any public variable here(if required)
    #endregion // PUBLIC_MEMBERS



    #region PROTECTED_METHODS
    protected override void Start()
    {
        base.Start();
        m_CloudRecoBehaviour = FindObjectOfType<CloudRecoBehaviour>();
        m_CloudRecoEventHandler = FindObjectOfType<CloudRecoEventHandler>();

        m_CommonBarScript = FindObjectOfType<CommonBarScript>();


        //infoPanelScript = FindObjectOfType<InfoPanelScript>();
        //infoPanelScript.m_CloudRecoEventHandler = m_CloudRecoEventHandler;

    }
   
   
    protected override void OnTrackingFound()
    {
        Debug.Log("<color=blue>OnTrackingFound()</color>");
        GlobalVariables.TRACKING_FOUND = true;

        base.OnTrackingFound();

        //if (infoPanelScript != null)
        //{
        //    if(GlobalVariables.VIDEO_BUTTON_CLICKED){
        //        infoPanelScript.UnClicked();
        //    }
        //    else{
        //        infoPanelScript.TrackingFound();
        //        infoPanelScript.clicked();
        //    }

        //}

        if (m_CloudRecoBehaviour)
        {
            m_CloudRecoBehaviour.CloudRecoEnabled = false;
        }

        if (m_CloudRecoEventHandler != null)
        {
            m_CloudRecoEventHandler.TrackingFound();
        }

        if (m_CommonBarScript != null && GlobalVariables.ARScreen_Enable)
        {
            m_CommonBarScript.OpenARScreen();

            //===============

            foreach (string containerName in GlobalVariables.ContainerNames.Keys)
            {
                string containerColor = GlobalVariables.ContainerNames[containerName];

                Debug.Log("<color=green> @@@@ containerName   : </color>" + containerName);
                Debug.Log("<color=green> @@@@ containerColor   : </color>" + containerColor);


                //Now you can access the key and value both separately from this attachStat as:
               
                GameObject containerObject = GameObject.Find(containerName);
                UnityEngine.UI.Image img = containerObject.GetComponent<UnityEngine.UI.Image>();
                //img.color = PrimaryScreenController.getColor(containerColor);
                img.color = PrimaryScreenController.getColor("rgba(0,0,0,0)");
            }
            Debug.Log("<color=green> @@@@ GlobalVariables.PrimaryImageName   : </color>" + GlobalVariables.PrimaryImageName);
            GameObject primaryImage = GameObject.Find(GlobalVariables.PrimaryImageName);
            primaryImage.gameObject.transform.localScale = Vector3.zero;


            //Transform[] transforms = UIScreen.GetComponentsInChildren<Transform>();
            //foreach (Transform child in transforms)
            //{
            //    //child is your child transform
            //    if (child.name.Contains("panel") || child.name.Contains("Canvas"))
            //    {
            //        UnityEngine.UI.Image img = child.gameObject.GetComponent<UnityEngine.UI.Image>();
            //        img.color = PrimaryScreenController.getColor("rgba(255,255,255,0)");
            //    }
            //    if (child.name.Contains("Image"))
            //    {
            //        child.gameObject.transform.localScale = Vector3.zero;
            //    }

            //}


            //========================



        }

        //if (ARScreen != null)
        //{
        //    ARScreen.transform.localScale = Vector3.one;
        //}


        //Debug.Log("<color=purple> $$$$$$$$$$$$$$$$$$$$$$$ OnTrakcingFOUND</color>");
        BlocklyEvents eventObj = new BlocklyEvents();
        eventObj.executeARTrackingBlocks(UIEnums.ARTrackingTypes.ARTrackingFound);
    }

    protected override void OnTrackingLost()
    {
        //Debug.Log("<color=blue>OnTrackingLost()</color>");
        GlobalVariables.TRACKING_FOUND = false;
        base.OnTrackingLost();
        if (m_CloudRecoBehaviour)
        {
            m_CloudRecoBehaviour.CloudRecoEnabled = true;
        }

        if (m_CloudRecoEventHandler != null)
        {
            m_CloudRecoEventHandler.TrackingLost();
        }

        if (m_CommonBarScript != null && !GlobalVariables.UIScreen_CLICKED)
        {
            UIScreen.transform.localScale = Vector3.zero;
        }


        //if (ARScreen != null)
        //{
        //    ARScreen.transform.localScale = Vector3.zero;
        //}
        //if (infoPanelScript != null)
        //{
        //    infoPanelScript.TrackingLost();
        //}
        //Debug.Log("<color=purple> $$$$$$$$$$$$$$$$$$$$$$$ OnTrakcingLOST</color>");
        BlocklyEvents eventObj = new BlocklyEvents();
        eventObj.executeARTrackingBlocks(UIEnums.ARTrackingTypes.ARTrackingLost);

    }
    #endregion //PROTECTED_METHODS
   
}
