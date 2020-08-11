/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using Mapbox.Unity.Location;
/// <summary>
/// This screen is used to handle all user actions pertaining to the registration screen. It uses the Base screen superclass
/// to automatically declare various variables and methods that each screen shoul have.
/// </summary>
public class UIScreen : BaseScreen {

	public CanvasGroup[] uiScreens;

	private PrimaryScreenController m_PrimaryScreenController;
	private CommonBarScript m_CommonBarScript;
	private float rotateSpeed = 200f;
	private bool applicationBuilt;
	private Location currentLocation;
	private void Start()
    {
		//TODO Build All UI Canvases, attach to UIScreen, and add into CanvasGroup[] uiScreens

		m_CommonBarScript = FindObjectOfType<CommonBarScript>();

		//currentLocation = LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation;
		//string location = currentLocation.LatitudeLongitude.ToString();
		//Debug.Log("<color=red> @@@@  Before Location    : </color>" + location);
		//string[] latlong = location.Split(',');
		//Debug.Log("<color=red> @@@@ Lat   : </color>" + latlong[0]);
		//Debug.Log("<color=red> @@@@ Long   : </color>" + latlong[1]);
		//location = latlong[1] + "," + latlong[0];
		//Debug.Log("<color=red> @@@@  After Location    : </color>" + location);


		//Debug.Log("<color=red> @@@@@@@@@@ Start of UIScreen: </color>" );

		//string locatioApiURL = GlobalVariables.REST_SERVER + "setlocation";
		//WWW www = ARUtilityTools.setLocation(locatioApiURL, "d14cb1720bdf40d1a4466e6659026f6e", "68.724039,-102.944422");

		//StartCoroutine(postURL(RestServerPath));


		if (!GlobalVariables.LocationMap_CLICKED)
        {
            GlobalVariables.VUFORIA_UNIQUE_ID = "7379692a4f00410fbb182ef74cb6dcee";
        }
		m_PrimaryScreenController = FindObjectOfType<PrimaryScreenController>();
		//LoadPrimaryScreen();
	}

	public void LoadPrimaryScreen(bool isARScreen)
	{
		//this.gameController.OpenJituScreen();
		//PrimaryScreenController primaryScreenController = PrimaryScreen.GetComponent<PrimaryScreenController>();
		if (m_PrimaryScreenController != null)
		{
			this.gameObject.transform.localScale = Vector3.one;
			m_PrimaryScreenController.PrimaryScreenApplication(this.gameObject,isARScreen);
		}
		//Debug.Log("<color=purple> $$$$$$$$$$$$$$$$$$$$$$$ OnAPPLOAD</color>");
		//BlocklyEvents eventObj = new BlocklyEvents();
		//eventObj.executeARTrackingBlocks(UIEnums.ARTrackingTypes.ApplicationLoad);
	}


    /// <summary>
    /// This Method will go into each of the Canas Prefab 
    /// </summary>
    /// <param name="screenToShow"></param>
    public void LoadScreen(CanvasGroup screenToShow)
    {
		ShowScreen(screenToShow);
	}



	private void HideAllScreens()
	{
        foreach(CanvasGroup screenToHide in uiScreens)
        {
            screenToHide.alpha = 0;
            screenToHide.blocksRaycasts = false;
            screenToHide.interactable = false;
        }

	}

	private void ShowScreen(CanvasGroup screenToShow)
	{
		HideAllScreens();
		screenToShow.alpha = 1;
		screenToShow.blocksRaycasts = true;
		screenToShow.interactable = true;
	}



}
