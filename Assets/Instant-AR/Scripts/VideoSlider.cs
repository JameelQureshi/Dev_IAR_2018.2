/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSlider : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    public Slider slider;

    void Start()
    {
        //videoPlayer = GetComponent<VideoPlayer>();
        //slider = GetComponent<Slider>();
        slider.minValue = 0;
        Debug.Log("<color=red> @@@@@@@@@@ CalculateLengh: </color>" + CalculateLengh());
        slider.maxValue = (float)CalculateLengh();

    }

    void Update()
    {
        //slider.value = (float)videoPlayer.time;
    }

    public void MoveSlider()
    {
        videoPlayer.time = slider.value;
    }

    double CalculateLengh()
    {
        return videoPlayer.frameCount / videoPlayer.frameRate;
    }
}