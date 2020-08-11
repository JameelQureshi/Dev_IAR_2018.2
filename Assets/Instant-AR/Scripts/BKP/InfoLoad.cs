﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class InfoLoad : MonoBehaviour {
    public GameObject quad;
    public GameObject canvas;
    public Button infoButton;
    public Button videoButton;
    // Use this for initialization
    public VideoPlayer videoPlayer;
    void Start () {
		
	}
    public void clicked(){
        Debug.Log("<color=black> ??????? OKKKKKK>>>  I am inside clicked  </color>");
        videoPlayer = quad.GetComponent<VideoPlayer>();
        if (videoPlayer != null){
            Debug.Log("<color=red> PAUSING VIDEO </color>" + videoPlayer.url);
            videoPlayer.Pause();
            quad.transform.localScale = new Vector3(0, 0, 0);

        }

        canvas.SetActive(true);
        videoButton.transform.localScale = new Vector3(1, 1, 1);
        infoButton.transform.localScale = new Vector3(0, 0, 0);
    }
    public void UnClicked()
    {
        Debug.Log("<color=black> ??????? OKKKKKK>>>  InfoLoad>>I am inside clicked  </color>");
        //quad.SetActive(true);
        videoPlayer = quad.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            quad.transform.localScale = new Vector3(1, 1, 1);
            videoPlayer.Play();

        }


        canvas.SetActive(false);

        infoButton.transform.localScale = new Vector3(1, 1, 1);
        videoButton.transform.localScale = new Vector3(0, 0, 0);
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
        infoButton.transform.localScale = new Vector3(1, 1, 1);


    }
    public void hideInfoButton()
    {
        infoButton.transform.localScale = new Vector3(0, 0, 0);
        canvas.SetActive(false);


    }
    public void hideVideoButton()
    {
        videoButton.transform.localScale = new Vector3(0, 0, 0);


    }
}
