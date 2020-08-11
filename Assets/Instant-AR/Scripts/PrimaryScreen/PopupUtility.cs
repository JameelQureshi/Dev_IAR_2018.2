/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */  using System.Collections; using UnityEngine; using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupUtility : MonoBehaviour {      public static void MatchSize(RectTransform parentObject, RectTransform childObject)
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
    }      public static void MatchSizeByScale(RectTransform parentObject, RectTransform childObject)
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


        //panelTransform.sizeDelta = new Vector2(panelWidth, panelHeight);
    }

}  