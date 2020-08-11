using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Xml.XPath;
using System.Web;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Net;
using System.IO;
using System.Text;

public class PopupScript : MonoBehaviour
{
    public GameObject thisPopup;
    public float widthFactor = -1f;
    public float hightFactor = -1f;
    public string message;

    private float width;
    private float height;


    // Use this for initialization
    void Start()
    {
        Debug.Log("<color=blue>@@@@@@@ INSIDE PopupScript </color>");
        Debug.Log("<color=blue>@@@@@@@ widthFactor </color>" + widthFactor);
        Debug.Log("<color=blue>@@@@@@@ hightFactor </color>" + hightFactor);
        GameObject parentObject = thisPopup.transform.parent.gameObject;
        thisPopup.transform.localPosition = parentObject.transform.localPosition;
        Debug.Log("<color=blue>@@@@@@@ THE  parentObject  of POPUP Window is </color>"+ parentObject.name);
        RectTransform rectTransform = parentObject.GetComponent<RectTransform>();
        //RectTransform thisrectTransform = thisPopup.GetComponent<RectTransform>();
        //thisrectTransform.anchorMin = rectTransform.anchorMin;
        //thisrectTransform.anchorMax = rectTransform.anchorMax;
        //thisrectTransform.anchoredPosition = rectTransform.anchoredPosition;
        //thisrectTransform.sizeDelta = rectTransform.sizeDelta;
        Debug.Log("<color=blue>@@@@@@@ THE  parentObject Width is </color>" + rectTransform.rect.width);
        if (widthFactor <  0.001)
        {
            width = rectTransform.rect.width * 0.8f;
        }
        else if (widthFactor < 1.001)
        {
            width = rectTransform.rect.width;
        }
        else
        {
            width = rectTransform.rect.width * widthFactor;
        }
        if (hightFactor < 0.001)
        {
            height = rectTransform.rect.height * 0.3f;
        }
        else
        {
            height = rectTransform.rect.height * hightFactor;
        }

        Debug.Log("<color=blue>@@@@@@@ The NEW Popup Width is </color>" + width);
        Debug.Log("<color=blue>@@@@@@@ The NEW Popup Height is </color>" + height);
        //thisPopup.GetComponent<RectTransform>().sizeDelta = new Vector2(width- rectTransform.rect.width, height- rectTransform.rect.height);
        thisPopup.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        Debug.Log("<color=blue>@@@@@@@ The REVISED Popup Width is </color>" + thisPopup.GetComponent<RectTransform>().rect.width);
        Debug.Log("<color=blue>@@@@@@@ The REVISED Popup Height is </color>" + thisPopup.GetComponent<RectTransform>().rect.height);

        Transform[] transforms = thisPopup.GetComponentsInChildren<Transform>();
        foreach (Transform child in transforms)
        {
            //child is your child transform
            Debug.Log("<color=red> POPUP Child in ARRAY is </color>" + child.name);
            if (child.name.ToLower().Equals("top")|| child.name.ToLower().Equals("bottom"))
            {
                adjustChildPanel(child);
            }

        }

        Debug.Log("<color=blue>@@@@@@@ OUTSIDE PopupScript </color>");

    }
    void adjustChildPanel(Transform transform)
    {
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        if (transform.name.ToLower().Equals("top")){
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, height / 2);
            Transform[] transforms = rectTransform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                //child is your child transform
                Debug.Log("<color=blue>>>><<<<<<<<<<<<<  Child in TOP Panel is </color>" + child.name);
                if (child.name.ToLower().Equals("text"))
                {
                    Text msgText= child.GetComponent<RectTransform>().GetComponent<Text>();
                    RectTransform rectTransformText = msgText.GetComponent<RectTransform>();
                    float parentHeight = rectTransform.rect.height;
                    float parentWidth = rectTransform.rect.width;
                    rectTransformText.offsetMin = new Vector2(parentWidth*0.05f, parentHeight*0.15f);
                    rectTransformText.offsetMax= new Vector2(parentWidth * -0.05f, parentHeight * -0.15f);
                    if (!string.IsNullOrEmpty(message))
                    {
                        msgText.text = message;
                    }
                }

            }
        }
        else if (transform.name.ToLower().Equals("bottom"))
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, height / -2);
            Transform[] transforms = rectTransform.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                //child is your child transform
                Debug.Log("<color=red>>>><<<<<<<<<<<<< GRAND Child in Bottom Panel is </color>" + child.name);
                if (child.name.ToLower().Equals("no"))
                {
                    adjustChildButtons(rectTransform, child);
                }
                else if (child.name.ToLower().Equals("yes"))
                {
                    adjustChildButtons(rectTransform, child);
                }

            }

        }

    }

    void adjustChildButtons(RectTransform parentRectTransform, Transform transform)
    {
        Debug.Log("<color=green> Parent of Button is  </color>" + parentRectTransform.name);
        //RectTransform parentRectTransform = ChildGameObject2.GetComponent<RectTransform>();
        float height = parentRectTransform.rect.height;
        Debug.Log("<color=green> Parnet Height is </color>" + height);
        height = height / 2;
        Debug.Log("<color=green> New Buttton Height is </color>" + height);
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(height, height);

    }

    public void Refresh()
    {
        Start();
    }

}