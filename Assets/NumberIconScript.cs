using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberIconScript : MonoBehaviour {

	public Image image;
	public Text text;
	public Color textColor;
	public string textInfo;
	// Use this for initialization
	private bool initialSetup;

	public void Clicked()
	{
        if (!initialSetup)
        {
			setup();
			initialSetup = true;
		}
		PopupUtilities.makePopupYesNo(this.gameObject, null, "THIS IS WORKING", true, null);
	}
	public void setup() {
		image.color= new Color(1-textColor.r, 1 - textColor.g, 1 - textColor.b);
		float darkness = textColor.r + textColor.g + textColor.b;
		if (darkness >= 1.5)
        {
			image.color = Color.black;
		}
        else
        {
			image.color = Color.white;
		}
		text.color = textColor;
		text.text = textInfo;
		this.GetComponent<Image>().color = textColor;

		
	}
	
}
