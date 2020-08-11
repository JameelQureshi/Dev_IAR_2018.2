using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Michsky.UI.ModernUIPack;

public class LaxScript : MonoBehaviour
{

    private bool clicked;
    private Sprite videoSprite;

    void Start()
    {
        videoSprite = Resources.Load<Sprite>("JituSprites/play"); //Button-Info-icon

    }

   

    public void ShowPopup()
    {

        test();


        //Invoke("test", 5);
        //if (clicked)
        //{
        //    ShowVideo();
        //}
        //else
        //{
        //    ShowGauge();
        //    //GlobalVariables.VIDEO_BUTTON_CLICKED = true;
        //}


    }

    void ShowVideo()
    {
        PopupUtilities.makePopupVideo(null, null, true, true, "https://dl.dropbox.com/s/t17ie1xn33wqw0u/InstantAR_F02.mp4", true, Color.green, 5);
        clicked = false;
    }

    void ShowGauge()
    {
        List<ChartRange> ChartRanges = new List<ChartRange>();

        ChartRange chartRange = new ChartRange();


        chartRange.Colour = Color.red;
        chartRange.StartValue = 10;
        chartRange.EndValue = 50;
        ChartRanges.Add(chartRange);

        chartRange = new ChartRange();
        chartRange.Colour = Color.yellow;
        chartRange.StartValue = 50;
        chartRange.EndValue = 90;
        ChartRanges.Add(chartRange);

        chartRange = new ChartRange();
        chartRange.Colour = Color.green;
        chartRange.StartValue = 90;
        chartRange.EndValue = 150;
        ChartRanges.Add(chartRange);

        chartRange = new ChartRange();
        chartRange.Colour = Color.yellow;
        chartRange.StartValue = 150;
        chartRange.EndValue = 170;
        ChartRanges.Add(chartRange);

        chartRange = new ChartRange();
        chartRange.Colour = Color.red;
        chartRange.StartValue = 170;
        chartRange.EndValue = 190;
        ChartRanges.Add(chartRange);


        GaugePOCO gaugeInfo = new GaugePOCO(null, null, true, false, 1000, "AMC Voltage", Color.white, "Volt", 165, ChartRanges);

        //GameObject popup= PopupUtilities.makePopupGauge(null, null, true, false, 300, "AMC Voltage", Color.red, "Volt", 165, ChartRanges);

        GameObject popup = PopupUtilities.makePopupGauge(gaugeInfo);
        Debug.Log("<color=red> @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ popup name  : </color>"+ popup.name);
        clicked = true;
    }




    void test()
    {
        Debug.Log("<color=red> @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ INSIDE TEST() name  : </color>");
    }

}
