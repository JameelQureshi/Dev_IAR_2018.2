
/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderScript : MonoBehaviour {


    public Color borderColor;
    public float borderWidth = 10;
	//public GameObject BorderObject;

	private GameObject topBorder;
	private GameObject bottomBorder;
	private GameObject leftBorder;
	private GameObject rightBorder;



	// Use this for initialization
	public void setBorders()
    {
		Start();

	}
	void Start () {
		Transform[] transforms = this.gameObject.transform.GetComponentsInChildren<Transform>();
		foreach (Transform child in transforms)
		{
            if (child.gameObject.name.Equals("BorderTop"))
            {
				topBorder = child.gameObject;

			}
            else if (child.gameObject.name.Equals("BorderBottom"))
			{
				bottomBorder = child.gameObject;
			}
			else if (child.gameObject.name.Equals("BorderLeft"))
			{
				leftBorder = child.gameObject;
			}
			else if (child.gameObject.name.Equals("BorderRight"))
			{
				rightBorder = child.gameObject;
			}
		}

		setDimensions();
		setColor();
	}
	
	private void setDimensions()
    {
		Vector2 temp = new Vector2(-borderWidth / 2f, -0.5f - (borderWidth / 2f));
		topBorder.GetComponent<RectTransform>().offsetMin = temp;
		topBorder.GetComponent<RectTransform>().offsetMax = new Vector2(borderWidth / 2f, temp.y + borderWidth);


		

		temp = new Vector2(-borderWidth / 2f, 0.5f - (borderWidth / 2f));
		bottomBorder.GetComponent<RectTransform>().offsetMin = temp;
		bottomBorder.GetComponent<RectTransform>().offsetMax = new Vector2(borderWidth / 2f, temp.y + borderWidth);

		temp = new Vector2(0.5f - (borderWidth / 2f), 0f);
		leftBorder.GetComponent<RectTransform>().offsetMin = temp;
		leftBorder.GetComponent<RectTransform>().offsetMax = new Vector2(temp.x + borderWidth, 0);


		temp = new Vector2(-0.5f - (borderWidth / 2f), 0f);
		rightBorder.GetComponent<RectTransform>().offsetMin = temp;
		rightBorder.GetComponent<RectTransform>().offsetMax = new Vector2(temp.x + borderWidth, 0);
	}
    private void setColor()
    {
        if(borderColor != null)
        {
			topBorder.GetComponent<RawImage>().color = borderColor;
			bottomBorder.GetComponent<RawImage>().color = borderColor;
			leftBorder.GetComponent<RawImage>().color = borderColor;
			rightBorder.GetComponent<RawImage>().color = borderColor;
		}
        

	}
}
