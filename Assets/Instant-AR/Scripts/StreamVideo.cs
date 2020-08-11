/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class StreamVideo : MonoBehaviour
{
    public GameObject parentContainer;
    public GameObject videoPrefab;
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public GameObject PlayPause;
    public string url;


    public Slider slider;
    public bool playVideo;
    public bool pauseVideo;
    public bool stopVideo;

    public bool preserveAspect = true;


    private bool videoPrepared = false;
    private bool videoLengthCalculated = false;
    private bool knobClicked;
    //public AudioSource audioSource;
    // Use this for initialization

    private Sprite playSprite;
    private Sprite pauseSprite;

    public Text VideoFailureText;
    private string VideoFailureMessage;


    void Start()
    {
        if (!playVideo)
        {
            PlayPause.GetComponent<Button>().image.sprite = playSprite;
        }
        //VideoKickoff();
    }

    public void VideoKickoff()
    {
        VideoFailureMessage = "Video Could not be loaded.Please check your network or try again !";
        VideoFailureText.transform.localScale = Vector3.zero;
        playSprite = Resources.Load<Sprite>("JituSprites/play");
        pauseSprite = Resources.Load<Sprite>("JituSprites/icons8-pause-button-100");
        //StartCoroutine(PlayVideo());
        //PlayVideo();
        if (!string.IsNullOrEmpty(url))
        {
            StartCoroutine(PrepareVideo());
        }
        slider.minValue = 0;
    }
    double CalculateLengh()
    {
        return videoPlayer.frameCount / videoPlayer.frameRate;
    }

    private void Update()
    {
        if (videoPrepared)
        {
            if (!videoLengthCalculated)
            {
                slider.maxValue = (float)CalculateLengh();
                videoLengthCalculated = true;
            }
            if (!knobClicked)
            {
                slider.value = (float)videoPlayer.time;
            }
        }

    }

    public VideoPlayer getVideoPlayer()
    {
        return videoPlayer;
    }

    public void PlayVideo()
    {
        // videoPlayer.Play();
        Debug.Log("<color=red> @@@@@@@@@ PlayPause.GetComponent<Button>().image.sprite.name)  : </color>" + PlayPause.GetComponent<Button>().image.sprite.name);

        if (PlayPause.GetComponent<Button>().image.sprite.name.Equals(pauseSprite.name))
        {
            //playVideo = false;
            videoPlayer.Pause();
            PlayPause.GetComponent<Button>().image.sprite = playSprite;
        }
        else
        {
            //playVideo = true;
            videoPlayer.Play();
            PlayPause.GetComponent<Button>().image.sprite = pauseSprite;
        }




    }
    public void PauseVideo()
    {
        playVideo = false;
        videoPlayer.Pause();
    }
    public void StopVideo()
    {
        playVideo = false;
        videoPlayer.Pause();
        //Destroy(videoPrefab);
        GlobalVariables.VIDEO_BUTTON_CLICKED = false;
        StartCoroutine(DecayDestroy(videoPrefab));
    }

    public void RestartVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.time = 0;
        }
    }

    public void Forward()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.time = videoPlayer.time + 10;
        }
    }
    public void Backward()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.time = videoPlayer.time - 10;
        }
    }

    IEnumerator DecayDestroy(GameObject decayingObject)
    {
        float increment = 0.05f;
        float rotateAngle = 0f;

        while (decayingObject.transform.localScale.x > 0)
        {
            decayingObject.transform.localScale = decayingObject.transform.localScale - new Vector3(increment, increment, increment);
            //PrimaryScreenButton.GetComponent<RectTransform>().Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
            //decayingObject.transform.Rotate(0f, 0f, rotateAngle);
            yield return new WaitForSeconds(0.01f);
            //rotateAngle = rotateAngle + 5f;
        }
        Destroy(decayingObject);

    }



    public void KnobClicked()
    {
        if (!knobClicked)
        {
            videoPlayer.Pause();
            knobClicked = true;
            PlayPause.GetComponent<Button>().image.sprite = playSprite;
        }
        else
        {
            videoPlayer.Play();
            knobClicked = false;
            PlayPause.GetComponent<Button>().image.sprite = pauseSprite;
        }
    }
    public void KnobDragBegin()
    {
        videoPlayer.Pause();
        PlayPause.GetComponent<Button>().image.sprite = playSprite;
    }
    public void KnobDragEnd()
    {
        videoPlayer.time = slider.value;
        videoPlayer.Play();
        PlayPause.GetComponent<Button>().image.sprite = pauseSprite;
    }

    IEnumerator PrepareVideo()
    {
        Debug.Log("<color=red> @@@@@@@@@ The Video URL is : </color>" + url);
        videoPlayer.url = url;
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        int waitLimit = 15;
        int counter = 0;
        while (true)
        {
            counter++;
            if (counter > waitLimit)
            {
                //Texture2D errorTexture = Resources.Load<Texture2D>("JituSprites/Bug");
                //rawImage.texture = errorTexture;
                Debug.Log("<color=red> @@@@@@@@@ Could not get the Video so returning NOTHING   : </color>");
                VideoFailureText.transform.localScale = Vector3.one;
                yield break;
            }
            yield return waitForSeconds;
            if (videoPlayer.isPrepared && videoPlayer.texture != null)
            {
                videoPrepared = true;
                break;
            }
        }
        rawImage.texture = videoPlayer.texture;

        adjustSize(preserveAspect);

        if (playVideo)
        {
            videoPlayer.Play();
        }
    }

    void adjustSize(bool alignAspect)
    {
        if(parentContainer == null)
        {
            return;
        }

        //RectTransform panelTransform = rawImage.gameObject.GetComponent<RectTransform>();
        RectTransform panelTransform = this.gameObject.GetComponent<RectTransform>();
        RectTransform parentTransform = parentContainer.gameObject.GetComponent<RectTransform>();

        if (!alignAspect)
        {
            panelTransform.anchorMin = new Vector2(0, 0);
            panelTransform.anchorMax = new Vector2(1, 1);
            panelTransform.offsetMin = new Vector2(0, 0); // new Vector2(left, bottom);
            panelTransform.offsetMax = new Vector2(0, 0); // new Vector2(-right, -top);
        }
        else
        {
            Texture vidTex = videoPlayer.texture;
            float videoWidth = vidTex.width;
            float videoHeight = vidTex.height;
            float videoAspect = videoWidth / videoHeight;

            float panelWidth = parentTransform.rect.width;
            float panelHeight = parentTransform.rect.height;
            float panelRatio = panelWidth / panelHeight;

            panelWidth = panelHeight * videoAspect;
            if (panelWidth > parentTransform.rect.width)
            {
                panelWidth = parentTransform.rect.width;
                panelHeight = panelWidth / videoAspect;
                if (panelHeight > parentTransform.rect.height)
                {
                    panelHeight = parentTransform.rect.height;
                    panelWidth = panelHeight * videoAspect;
                }
            }
            panelTransform.sizeDelta = new Vector2(panelWidth, panelHeight);
        }

    }
}