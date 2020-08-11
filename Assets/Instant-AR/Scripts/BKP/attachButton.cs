using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class attachButton : MonoBehaviour
{
    public GameObject canvas;
    public GameObject sphere2;
    public List<TargetInfo> targetInfoSet;
    public Slider infoTextSizeSlider;
    public Text smallFontSymbol;
    public Text bigFontSymbol;
    public Button infoEditButton;
    public Button heathButton;
    public Toggle showIconToggle;
    public Toggle dataIconToggle;
    public GameObject sliderPanel;

    private Button[] liveButtons;
    private int fontSize = 20;
    private Color whiteColor = Color.white;
    private bool buttonCollected = false;
    //private GameObject newButton;


    // bool isPortrait = false;

    //string nameLabelText;
    // Use this for initialization
    void Start()
    {
        if (heathButton != null)
        {
            heathButton.transform.localScale = new Vector3(0, 0, 0);
        }

        //Debug.Log("<color=blue>>>><<<<<<<<<<<<< Inside  start of  attachButton </color>");
        fontSize = (int)(Screen.width * 0.03f);
        Debug.Log("<color=green> >>><<<<<<<<<<<<< Inside  start of  attachButton,The font size is  </color>" + fontSize);
        //nameLabelText = "jjjjjj";
        //showInfoData();
        //addButtons();
        if (infoTextSizeSlider != null)
        {
            infoTextSizeSlider.value = 0.5f;
        }

        //liveButtons = colletButtons();
       // Debug.Log("<color=red> liveButtons size is </color>" + liveButtons.Length);



    }


    Button[] colletButtons()
    {
        Button[] buttons;
       
        if (canvas != null)
        {
            Text[] textComponents = canvas.GetComponentsInChildren<Text>();
            buttons = canvas.GetComponentsInChildren<Button>();
            int counter = 0;
            TargetInfo targetInfo;
            foreach (Button button in buttons)
            {
                if (!button.name.Contains("Button_"))
                {
                    continue;
                }

                targetInfo = targetInfoSet[counter];
                float sizeRatio = 1;
                if (!string.IsNullOrEmpty(targetInfo.sizeRatio))
                {
                    sizeRatio = float.Parse(targetInfo.sizeRatio);
                    sizeRatio = sizeRatio / 100;
                }
                if (button.name.Equals(targetInfo.buttonID))
                {
                    button.transform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
                    changeSprite(button, targetInfo);
                    if (!string.IsNullOrEmpty(targetInfo.string_value1) && targetInfo.string_value1.ToLower().Equals("equipmentno"))
                    {
                        GlobalVariables.KEYSIGHT_ASSET = targetInfo.string_value2;
                        GlobalVariables.CURRENT_KEYSIGHT_ASSET = targetInfo.string_value2;
                        if (heathButton != null)
                        {
                            heathButton.transform.localScale = new Vector3(1, 1, 1);
                        }
                        string string_value3 = targetInfo.string_value3;
                        if (!string.IsNullOrEmpty(string_value3))
                        {
                            if (string_value3.ToLower().Equals("green"))
                            {
                                GlobalVariables.CURRENT_KEYSIGHT_HELATH = "green";
                            }
                            else if (string_value3.ToLower().Equals("yellow"))
                            {
                                GlobalVariables.CURRENT_KEYSIGHT_HELATH = "yellow";
                            }
                            else if (string_value3.ToLower().Equals("red"))
                            {
                                GlobalVariables.CURRENT_KEYSIGHT_HELATH = "red";
                            }
                        }
                    }

                }
            }
        }
        else{
            buttons = null;
        }
        return buttons;
    }


    // Update is called once per frame
    void Update()
    {
        if (GlobalVariables.INFO_PANEL_BUTTON_CLICKED)
        {
            if (canvas != null)
            {
                Destroy(canvas);
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapTargetImage");
        }

        if (canvas != null && GlobalVariables.INFO_BUTTON_CLICKED)
        {
            //showInfoData();
            showInfoDataORG();
        }

    }

    void showInfoData()
    {

        if(liveButtons==null || liveButtons.Length==0)
        {
            liveButtons = colletButtons();
            Debug.Log("<color=red> liveButtons size is </color>" + liveButtons.Length);
            return;
        }


        if (infoEditButton != null)
        {
            infoEditButton.transform.localScale = new Vector3(1, 1, 1);
        }
        if (sliderPanel != null)
        {
            sliderPanel.transform.localScale = new Vector3(1, 1, 1);
            sliderPanel.SetActive(true);
        }

        Vector3 sphere1Pos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        Vector3 sphere2Pos = Camera.main.WorldToScreenPoint(sphere2.transform.position);
        float width = sphere2Pos.x - sphere1Pos.x;
        float height = sphere1Pos.y - sphere2Pos.y;
        float aspect = height / width;
        if (canvas != null)
        {
            Button[] buttons = liveButtons;
            int counter = 0;
            TargetInfo targetInfo;
            foreach (Button button in buttons)
            {
                if (!button.name.Contains("Button_"))
                {
                    continue;
                }

                targetInfo = targetInfoSet[counter];
                counter++;
                float xAxis = float.Parse(targetInfo.buttonPosition_x);
                float yAxis = float.Parse(targetInfo.buttonPosition_y);

                float sizeRatio = 1;
                if (!string.IsNullOrEmpty(targetInfo.sizeRatio))
                {
                    sizeRatio = float.Parse(targetInfo.sizeRatio);
                    sizeRatio = sizeRatio / 100;
                }
                if (button.name.Equals(targetInfo.buttonID))
                {
                    button.transform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(width / 10, width / 10);
                    button.transform.position = sphere1Pos + new Vector3(width * xAxis, (height * yAxis) * -1, 0.0f); ;
                    //changeSprite(button, targetInfo);
                }
            }
        }
    }


    void showInfoDataORG()
    {

        if (infoEditButton != null)
        {
            infoEditButton.transform.localScale = new Vector3(1, 1, 1);
        }
        if (sliderPanel != null)
        {
            sliderPanel.transform.localScale = new Vector3(1, 1, 1);
            sliderPanel.SetActive(true);
        }

        Vector3 sphere1Pos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        Vector3 sphere2Pos = Camera.main.WorldToScreenPoint(sphere2.transform.position);
        float width = sphere2Pos.x - sphere1Pos.x;
        float height = sphere1Pos.y - sphere2Pos.y;
        float aspect = height / width;
        if (canvas != null)
        {
            Text[] textComponents = canvas.GetComponentsInChildren<Text>();
            Button[] buttons = canvas.GetComponentsInChildren<Button>();
            int counter = 0;
            TargetInfo targetInfo;
            foreach (Button button in buttons)
            {
                if (!button.name.Contains("Button_"))
                {
                    continue;
                }

                targetInfo = targetInfoSet[counter];
                counter++;
                float xAxis = float.Parse(targetInfo.buttonPosition_x);
                float yAxis = float.Parse(targetInfo.buttonPosition_y);

                float sizeRatio = 1;
                if (!string.IsNullOrEmpty(targetInfo.sizeRatio))
                {
                    sizeRatio = float.Parse(targetInfo.sizeRatio);
                    sizeRatio = sizeRatio / 100;
                }
                if (button.name.Equals(targetInfo.buttonID))
                {
                    button.transform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(width / 10, width / 10);
                    button.transform.position = sphere1Pos + new Vector3(width * xAxis, (height * yAxis) * -1, 0.0f);
                    changeSprite(button, targetInfo);
                    if (!string.IsNullOrEmpty(targetInfo.string_value1) && targetInfo.string_value1.ToLower().Equals("equipmentno"))
                    {
                        GlobalVariables.KEYSIGHT_ASSET = targetInfo.string_value2;
                        GlobalVariables.CURRENT_KEYSIGHT_ASSET = targetInfo.string_value2;
                        if (heathButton != null)
                        {
                            heathButton.transform.localScale = new Vector3(1, 1, 1);
                        }
                        string string_value3 = targetInfo.string_value3;
                        if (!string.IsNullOrEmpty(string_value3))
                        {
                            if (string_value3.ToLower().Equals("green"))
                            {
                                GlobalVariables.CURRENT_KEYSIGHT_HELATH = "green";
                            }
                            else if (string_value3.ToLower().Equals("yellow"))
                            {
                                GlobalVariables.CURRENT_KEYSIGHT_HELATH = "yellow";
                            }
                            else if (string_value3.ToLower().Equals("red"))
                            {
                                GlobalVariables.CURRENT_KEYSIGHT_HELATH = "red";
                            }
                        }
                    }

                }
            }
        }
    }

    public void changeSprite(Button thisButton, TargetInfo targetInfo)
    {
        string spritePath = targetInfo.button_sprite;
        if(string.IsNullOrEmpty(spritePath)){
            spritePath = "Button-Info-Unpress-icon";
        }
       // Debug.Log("<color=green> @@@@attachButton@@@@@ targetInfo.button_sprite </color>" + targetInfo.button_sprite);

        if (spritePath.IndexOf("JituSprites") == -1)
        {
            spritePath = "JituSprites/" + spritePath;
        }


        string message = targetInfo.string_value3;

       Debug.Log("<color=green> @@@@attachButton@@@@@ targetInfo.string_value3 </color>" + message);

        //try
        //{
        //    if (!string.IsNullOrEmpty(targetInfo.prefix) && !targetInfo.prefix.Equals("null"))
        //        message = targetInfo.prefix + " " + targetInfo.string_value3;
        //    if (!string.IsNullOrEmpty(targetInfo.suffix) && !targetInfo.suffix.Equals("null"))
        //        message = targetInfo.prefix + " " + targetInfo.string_value3 + " " + targetInfo.suffix;

        //} catch (Exception e)
        //{
        //    Debug.Log("<color=red> $$$$$$ An error occurred: </color>" + e.ToString());
        //}



        if (!string.IsNullOrEmpty(targetInfo.string_value1) && targetInfo.string_value1.ToLower().Equals("equipmentno"))
        {
            if (!string.IsNullOrEmpty(targetInfo.string_value3))
            {
                if (targetInfo.string_value3.ToLower().Equals("green"))
                {
                    spritePath = "JituSprites/GreenFlag";
                    message = targetInfo.string_value2;
                }
                else if (targetInfo.string_value3.ToLower().Equals("yellow"))
                {
                    spritePath = "JituSprites/YellowFlag";
                    message = targetInfo.string_value2;
                }
                else if (targetInfo.string_value3.ToLower().Equals("red"))
                {
                    spritePath = "JituSprites/RedFlag";
                    message = targetInfo.string_value2;
                }
            }
        }
        //Debug.Log("<color=blue> Inside attachButton Button Clicked is </color>" + thisButton.name);
        Texture2D texture;
        texture = Resources.Load<Texture2D>(spritePath);
        if (texture != null)
        {
            Rect rect = new Rect();
            rect.center = new Vector2(0, 0);
            rect.height = texture.height;
            rect.width = texture.width;
            Sprite tempSprite = UnityEngine.Sprite.Create(texture, rect, new Vector2(1, 1), 100f);
            thisButton.GetComponent<Image>().sprite = tempSprite;
            thisButton.GetComponent<Image>().sprite.name = tempSprite.name;
        }

        //IconButtonScript iconScript = thisButton.GetComponent<IconButtonScript>();
        //if (iconScript != null)
        //{
        //    iconScript.info = message;
        //    iconScript.prefix = targetInfo.prefix;
        //}
        //PopupUtilities.makePopupText(null, message, true, null);


        // Handle Slider Factor
        float sizeSlideFactor = infoTextSizeSlider.value - 0.5f;
        sizeSlideFactor = sizeSlideFactor * 2;
        thisButton.transform.localScale += new Vector3(sizeSlideFactor, sizeSlideFactor, 0);


        //===========


        if (targetInfo.string_value3 != null && !targetInfo.string_value3.Equals("null"))
        {
            IconButtonScript iconScript = thisButton.GetComponent<IconButtonScript>();
            if (iconScript != null)
            {
                iconScript.info = message;
                iconScript.prefix = targetInfo.prefix;
            }
            //prefix.Equals("info", System.StringComparison.OrdinalIgnoreCase)
            if (string.IsNullOrEmpty(targetInfo.prefix) ||
                targetInfo.prefix.Equals("null") ||
                ((targetInfo.prefix != null &&
                !targetInfo.prefix.Equals("table", System.StringComparison.OrdinalIgnoreCase) &&
                !targetInfo.prefix.Equals("prompt", System.StringComparison.OrdinalIgnoreCase)))
               )
            {
                handleInfoText(thisButton, message);
            }
            else
            {
                handleInfoText(thisButton, null);

            }
        }
        else
        {
            handleInfoText(thisButton, null);
        }


        //if (targetInfo.prefix != null &&
        //            !targetInfo.prefix.Equals("table", System.StringComparison.OrdinalIgnoreCase) &&
        //            !targetInfo.prefix.Equals("prompt", System.StringComparison.OrdinalIgnoreCase))
        //{
        //    handleInfoText(thisButton, message);
        //}
        //else{
        //    handleInfoText(thisButton, null);
        //}



    }

    void handleInfoText(Button button,string infoData)
    {
        Text[] textComponents = button.GetComponentsInChildren<Text>();
        if (string.IsNullOrEmpty(infoData) || infoData.Equals("null"))
        {
            //infoData = "";
            if(textComponents[0] !=null){
                textComponents[0].transform.localScale = Vector3.zero;
            }
            return;
        }

        float textWidth = 100 + infoData.Length * 25;
        float textHeight = 90;
        int textSize = 50;
        float sizeSlideFactor = infoTextSizeSlider.value - 0.5f;
        sizeSlideFactor = sizeSlideFactor * 2;
        //float widthSlideFactor = sizeSlideFactor * 5;
        //float hightSlideFactor = sizeSlideFactor * 2;
        //textWidth = textWidth + textWidth * widthSlideFactor;
        //textHeight = textHeight + textHeight * hightSlideFactor;
        //textSize = textSize + (int)(textSize * widthSlideFactor);

        //add slider value factor to increase/decrease text box size

        RectTransform[] panel = textComponents[0].GetComponentsInChildren<RectTransform>();
        //Debug.Log("<color=green> >>><<<<<<<<<<<<< Inside  handleInfoText of  attachButton,panel RectTransform name is  </color>" + panel[1].name);
        float top = button.GetComponent<RectTransform>().rect.height / 2 + textComponents[0].GetComponent<RectTransform>().rect.height;
        //Debug.Log("<color=green> >>><<<<<<<<<<<<< Inside  handleInfoText button hight is  </color>" + top);
        //top = top * 1.5f;
        //Debug.Log("<color=green> >>><<<<<<<<<<<<< Now  button hight is  </color>" + top);
        if (!showIconToggle.isOn && dataIconToggle.isOn)
        {
            panel[1].transform.localScale = Vector3.one;
            whiteColor.a = 0f;
            button.GetComponent<Image>().color = whiteColor;
            panel[1].offsetMax = new Vector2(0, 0);
            panel[1].offsetMin = new Vector2(0, 0);
        }
        else if (showIconToggle.isOn && !dataIconToggle.isOn)
        {

            whiteColor.a = 1f;
            button.GetComponent<Image>().color = whiteColor;
            panel[1].transform.localScale = Vector3.zero;
            //textComponents[0].transform.localScale = Vector3.zero;
        }

        else if (showIconToggle.isOn && dataIconToggle.isOn)
        {
            panel[1].transform.localScale = Vector3.one;
            whiteColor.a = 1f;
            button.GetComponent<Image>().color = whiteColor;
            panel[1].offsetMax = new Vector2(panel[1].offsetMax.x, top);
            panel[1].offsetMin = new Vector2(panel[1].offsetMin.x, top);

        }
        else if (!showIconToggle.isOn && !dataIconToggle.isOn)
        {
            panel[1].transform.localScale = Vector3.one;
            dataIconToggle.isOn = true;
        }

        textComponents[0].GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, textHeight);
        textComponents[1].fontSize = textSize;
        textComponents[1].text = infoData;
        if (string.IsNullOrEmpty(infoData))
        {
            textComponents[0].transform.localScale = Vector3.zero;
        }

        button.transform.localScale += new Vector3(sizeSlideFactor, sizeSlideFactor, 0);

    }


}
