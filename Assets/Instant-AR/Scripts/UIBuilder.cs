/*
 * ============================================================================== 
 * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.
 * 
 * @Author : Jitender Hooda 
 * 
 ==============================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;

public class UIBuilder : MonoBehaviour
{
    public static void infoAttachButton(GameObject targetImage)
    {
        //Debug.Log("<color=green> ################## infoAttachButton is starting </color>");
        ButtonDetails[] buttons = null;
        TargetImage targetImageObject = GlobalVariables.targetImageObject;


        if (targetImageObject != null)
        {
            //Debug.Log("<color=green> ################## infoAttachButton targetImageObject != null </color>");
            buttons = targetImageObject.buttons;
            //Debug.Log("<color=green> ################## infoAttachButton targetImageObject != null </color>" + buttons.Length);
        }

        if (buttons == null || buttons.Length == 0)
        {
            Debug.Log("<color=red> ################## buttonInfoMAP is null, so NO INFO to show  </color>");
            return;
        }

        RectTransform rectTransform = targetImage.GetComponent<RectTransform>();
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        foreach (ButtonDetails button in buttons)
        {
            float sizeRatio = 1;
            GameObject buttonPrefab = (GameObject)Resources.Load("InfoButton");
            GameObject newButton = (GameObject)Instantiate(buttonPrefab);
            newButton.name = button.buttonID;
            newButton.transform.SetParent(targetImage.transform);
            float xFactor = float.Parse(button.buttonPosition_x);
            float yFactor = float.Parse(button.buttonPosition_y) * -1;
            newButton.transform.localPosition = new Vector3(-width / 2, height / 2, 0) + new Vector3(width * xFactor, height * yFactor, 0);
            float widPos = (-width / 2) + (width * xFactor);
            changeSprite(newButton, button.button_sprite);

            //Change the size of the button based on the sizeRatio set from the web page
            if (!string.IsNullOrEmpty(button.sizeRatio))
            {
                sizeRatio = float.Parse(button.sizeRatio);
                sizeRatio = sizeRatio / 50;
            }
            newButton.transform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);

            IconButtonScript iconScript = newButton.GetComponent<IconButtonScript>();
            if (iconScript != null)
            {
                iconScript.info = button.string_value3;
                iconScript.prefix = button.prefix;
                iconScript.posX = xFactor;
                iconScript.posY = yFactor;
                iconScript.sizeRatio = sizeRatio;
            }


            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (!string.IsNullOrEmpty(button.string_value3) && !button.string_value3.Equals("null"))
            {
                //IconButtonScript iconScript = newButton.GetComponent<IconButtonScript>();
                //if (iconScript != null)
                //{
                //    iconScript.info = button.string_value3;
                //    iconScript.prefix = button.prefix;
                //    iconScript.posX = xFactor;
                //    iconScript.posY = yFactor;
                //    iconScript.sizeRatio = sizeRatio;
                //}
                //prefix.Equals("info", System.StringComparison.OrdinalIgnoreCase)
                //if (string.IsNullOrEmpty(button.prefix) || button.prefix.Equals("null"))
                //{
                //    handleInfoText(thisButton, message);
                //}
                //else if (button.prefix != null &&
                //!button.prefix.Equals("table", System.StringComparison.OrdinalIgnoreCase)&&
                //!button.prefix.Equals("prompt", System.StringComparison.OrdinalIgnoreCase))

                if (string.IsNullOrEmpty(button.prefix) ||
                    button.prefix.Equals("null") ||
                    ((button.prefix != null &&
                    !button.prefix.Equals("table", System.StringComparison.OrdinalIgnoreCase) &&
                    !button.prefix.Equals("prompt", System.StringComparison.OrdinalIgnoreCase))))
                {
                    //buttonText.text = button.string_value3;
                    Text innerText = buttonText.gameObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>();
                    innerText.name = "ChildText";
                    innerText.text = button.string_value3;
                    //adjustText(buttonText, targetImage, sizeRatio);
                }
                else if (buttonText != null)
                {
                    buttonText.transform.localScale = Vector3.zero;

                }
            }
            else if (buttonText != null)
            {
                buttonText.transform.localScale = Vector3.zero;
            }
        }

    }
    private static void changeSprite(GameObject go, string spritePath)
    {
        spritePath = "JituSprites/" + spritePath;
        Button thisButton = go.GetComponent<Button>();
        Texture2D texture;
        texture = Resources.Load<Texture2D>(spritePath);
        if (texture == null)
        {
            texture = Resources.Load<Texture2D>("JituSprites/Button-Info-icon");
        }
        Rect rect = new Rect();
        rect.center = new Vector2(0, 0);
        rect.height = texture.height;
        rect.width = texture.width;
        Sprite tempSprite = UnityEngine.Sprite.Create(texture, rect, new Vector2(1, 1), 100f);
        thisButton.GetComponent<Image>().sprite = tempSprite;
    }

    private static void adjustText(Text buttonText, GameObject targetImage, float sizeRatio)
    {
        float xPosText = buttonText.rectTransform.position.x / sizeRatio;
        float textWidth = buttonText.rectTransform.rect.width;

        float panelWidth = targetImage.transform.parent.gameObject.GetComponent<RectTransform>().rect.width;
        panelWidth = panelWidth / sizeRatio;
        if (xPosText < textWidth / 2)
        {
            float xDisplacement = textWidth / 2 - xPosText + 5;
            buttonText.transform.localPosition = buttonText.transform.localPosition + new Vector3(xDisplacement, 0, 0);
        }
        else if (xPosText > (panelWidth - textWidth / 2))
        {
            float xDisplacement = textWidth / 2 - (panelWidth - xPosText) + 5;
            buttonText.transform.localPosition = buttonText.transform.localPosition - new Vector3(xDisplacement, 0, 0);
        }

    }

}
