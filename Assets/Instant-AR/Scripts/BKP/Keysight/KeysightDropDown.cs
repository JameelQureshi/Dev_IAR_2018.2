using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;


public class KeysightDropDown : MonoBehaviour
{

    //Create a List of new Dropdown options
    List<string> m_DropOptions = new List<string>();
    //This is the Dropdown
    public Dropdown m_Dropdown;
    public Dropdown green_Dropdown;
    public Dropdown yellow_Dropdown;
    public Dropdown red_Dropdown;


    public Text selectedOption;
    public Canvas graphCanvas;
    public Canvas scrollBarPanel;
    public Canvas lowerBarPanel;

    public Button leftRedButton;
    public Button leftYellowButton;
    public Button greenButton;
    public Button rightYellowButton;
    public Button rightRedButton;
    public Button refereshButton;

    public Image leftMarker;
    public Image centerMarker;
    public Image rightMarker;

    public GameObject buttonsPanel;
    public GameObject healthMarkerPanel;
    public GameObject markerTextPanel;

    //lower panel buttons
    public Button healthStatusButton;
    public Button greenStatusButton;
    public Button yellowStatusButton;
    public Button redStatusButton;



    public Text healthButtonText;

    //graph varibales
    private string limit_type;
    private float min_hold;
    private string streamId;
    private string displayName;
    private float BaseValue;
    private float maxValue;
    private float num_double_digits;
    private string units;
    private string deviceName;
    private float minValue;
    private float YellowN;
    private string metric_name;
    private float YellowP;
    private float healthValue;
    private float max_hold;
    private float RedP;
    private float RedN;
    private string device;
    private float Green;
    private string status;

    private float max;
    private float Red;
    private float Yellow;

    private float range;
    private int scale_EqualBlocksCount;
    private float scale_UnitBlockSize;
    private float scale_FirstBlockSize;
    private float scale_FirstBlockMarker;
    private float scale_LastBlockSize;

    private float width_Factor;

    private HealthParameter[] healthParameterArray;

    private HealthParameter[] greenHealthParameterArray;
    private HealthParameter[] yellowHealthParameterArray;
    private HealthParameter[] redHealthParameterArray;


    void Update()
    {
        if (!GlobalVariables.KEYSIGHT_ASSET.Equals(GlobalVariables.CURRENT_KEYSIGHT_ASSET))
        {
            if (!string.IsNullOrEmpty(GlobalVariables.CURRENT_KEYSIGHT_ASSET))
            {
                processHealthData(GlobalVariables.CURRENT_KEYSIGHT_ASSET);
                GlobalVariables.KEYSIGHT_ASSET = GlobalVariables.CURRENT_KEYSIGHT_ASSET;
                scrollBarPanel.transform.gameObject.SetActive(false);
                graphCanvas.transform.gameObject.SetActive(false);
            }
        }

    }

    void Start()
    {
        //processHealthData(GlobalVariables.KEYSIGHT_ASSET);
        if (healthButtonText != null && GlobalVariables.CURRENT_KEYSIGHT_ASSET != null)
        {
            healthButtonText.text = "Health of " + GlobalVariables.CURRENT_KEYSIGHT_ASSET + " :";
            processHealthData(GlobalVariables.CURRENT_KEYSIGHT_ASSET);
        }
    }

    private void processHealthData(string assetNumber)
    {
        if (!string.IsNullOrEmpty(GlobalVariables.CURRENT_KEYSIGHT_HELATH))
        {
            Debug.Log("<color=green> WOWWWWWWW  CURRENT_KEYSIGHT_HELATH of " +
                      assetNumber + " is: </color>" +
                      GlobalVariables.CURRENT_KEYSIGHT_HELATH);

            /*
            if (GlobalVariables.CURRENT_KEYSIGHT_HELATH.ToLower().Equals("green"))
            {
                if (greenStatusButton != null)
                {
                    greenStatusButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    yellowStatusButton.transform.localScale = Vector3.one;
                    redStatusButton.transform.localScale = Vector3.one;
                }
            }
            else if (GlobalVariables.CURRENT_KEYSIGHT_HELATH.ToLower().Equals("yellow"))
            {
                if (yellowStatusButton != null)
                {
                    greenStatusButton.transform.localScale = Vector3.one;
                    yellowStatusButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    redStatusButton.transform.localScale = Vector3.one;
                }
            }
            else if (GlobalVariables.CURRENT_KEYSIGHT_HELATH.ToLower().Equals("red"))
            {
                if (redStatusButton != null)
                {
                    greenStatusButton.transform.localScale = Vector3.one;
                    yellowStatusButton.transform.localScale = Vector3.one;
                    redStatusButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                }
            }
            */
        }

        //string assetNumber = "2_E7515BMY57180022";
        //string assetNumber = GlobalVariables.KEYSIGHT_ASSET; //Its retreived in attachbutton
        Debug.Log("<color=red> GlobalVariables.KEYSIGHT_ASSET is: </color>" + GlobalVariables.KEYSIGHT_ASSET);

        healthParameterArray = null;
        greenHealthParameterArray = null;
        yellowHealthParameterArray = null;
        redHealthParameterArray = null;

        if (healthButtonText != null && assetNumber != null)
        {
            healthButtonText.text = "Health of " + GlobalVariables.CURRENT_KEYSIGHT_ASSET + " :";
        }
        else
        {
            healthButtonText.text = "Equipment Health: ";
        }
        getKeysightHealthData(assetNumber);

        if (m_Dropdown != null)
        {
            //populateList();
            populateDropDownList(healthParameterArray, m_Dropdown);
        }
        if (green_Dropdown != null)
        {
            populateDropDownList(greenHealthParameterArray, green_Dropdown);
        }
        if (yellow_Dropdown != null)
        {
            populateDropDownList(yellowHealthParameterArray, yellow_Dropdown);
        }
        if (red_Dropdown != null)
        {
            populateDropDownList(redHealthParameterArray, red_Dropdown);
        }
    }


    public void DropDown_IndexChanged(int index)
    {
        processDropDown(index, healthParameterArray);
    }
    public void GreenDropDown_IndexChanged(int index)
    {
        processDropDown(index, greenHealthParameterArray);
    }
    public void YellowDropDown_IndexChanged(int index)
    {
        processDropDown(index, yellowHealthParameterArray);
    }
    public void RedDropDown_IndexChanged(int index)
    {
        processDropDown(index, redHealthParameterArray);
    }

    public void processDropDown(int index, HealthParameter[] healthParameterArray1)
    {
        if (healthParameterArray1 == null || healthParameterArray1.Length == 0)
        {
            return;
        }
        graphCanvas.transform.gameObject.SetActive(true);
        setValues(healthParameterArray1[index]);
        string limit_type = healthParameterArray1[index].limit_type;
        displayName = healthParameterArray1[index].displayName;
        Debug.Log("<color=blue> JITU limit_type is:  </color>" + limit_type);
        if (limit_type.Equals("none") || limit_type.Equals("min"))
        {
            buttonsPanel.transform.gameObject.SetActive(false);
            healthMarkerPanel.transform.gameObject.SetActive(false);
            markerTextPanel.transform.gameObject.SetActive(false);
            selectedOption.text = displayName + " : " + healthParameterArray1[index].healthValue;
            if (limit_type.Equals("min"))
            {
                selectedOption.text = displayName + " : This parameter is of \"min\" limit_type, and yet to be tested";
            }
            return;
        }
        else
        {
            buttonsPanel.transform.gameObject.SetActive(true);
            healthMarkerPanel.transform.gameObject.SetActive(true);
            markerTextPanel.transform.gameObject.SetActive(true);

        }
        if (limit_type.Equals("max_min"))
        {
            adjustButtonsForMaxMin();
        }
        else if (limit_type.Equals("max"))
        {
            adjustButtonsForMax();
        }
        else if (limit_type.Equals("min"))
        {
            //adjustButtonsForMin();
            adjustButtonsForMax(); //need to be replace by adjustButtonsForMin();
        }
        buildTextMarkers(healthParameterArray1[index]);
    }

    void populateDropDownList(HealthParameter[] healthParameterArray1, Dropdown dropDown)
    {
        List<string> listOptions = new List<string>();
        dropDown.ClearOptions();
        //dropDown.options.Clear();
        if (healthParameterArray1 == null || healthParameterArray1.Length == 0)
        {
            listOptions.Add("NO Parameters Under This Status");
            dropDown.AddOptions(listOptions);
        }
        else
        {
            for (int i = 0; i < healthParameterArray1.Length; i++)
            {
                listOptions.Add(healthParameterArray1[i].displayName);
            }
            dropDown.AddOptions(listOptions);
        }
    }

    public void mainscreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("3-CloudReco");
    }


    //==============
    public void showScrollBar()
    {
        if (scrollBarPanel != null)
        {
            if (!m_Dropdown.transform.gameObject.activeSelf)
            {
                graphCanvas.transform.gameObject.SetActive(false);
            }

            green_Dropdown.transform.gameObject.SetActive(false);
            yellow_Dropdown.transform.gameObject.SetActive(false);
            red_Dropdown.transform.gameObject.SetActive(false);
            m_Dropdown.transform.gameObject.SetActive(true);

            if (scrollBarPanel.transform.gameObject.activeSelf)
            {
                scrollBarPanel.transform.gameObject.SetActive(false);
                graphCanvas.transform.gameObject.SetActive(false);
            }
            else
            {
                scrollBarPanel.transform.gameObject.SetActive(true);
            }
        }
    }
    public void showGreenScrollBar()
    {
        if (green_Dropdown != null)
        {

            greenStatusButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            yellowStatusButton.transform.localScale = Vector3.one;
            redStatusButton.transform.localScale = Vector3.one;


            // healthParameterArray = greenHealthParameterArray;
            if (!green_Dropdown.transform.gameObject.activeSelf)
            {
                graphCanvas.transform.gameObject.SetActive(false);
            }

            m_Dropdown.transform.gameObject.SetActive(false);
            yellow_Dropdown.transform.gameObject.SetActive(false);
            red_Dropdown.transform.gameObject.SetActive(false);
            green_Dropdown.transform.gameObject.SetActive(true);
            scrollBarPanel.transform.gameObject.SetActive(true);

            GreenDropDown_IndexChanged(0);

        }

    }
    public void showYellowScrollBar()
    {
        if (yellow_Dropdown != null)
        {

            greenStatusButton.transform.localScale = Vector3.one;
            yellowStatusButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            redStatusButton.transform.localScale = Vector3.one;
            //healthParameterArray = yellowHealthParameterArray;
            if (!yellow_Dropdown.transform.gameObject.activeSelf)
            {
                graphCanvas.transform.gameObject.SetActive(false);
            }
            m_Dropdown.transform.gameObject.SetActive(false);
            green_Dropdown.transform.gameObject.SetActive(false);
            red_Dropdown.transform.gameObject.SetActive(false);
            yellow_Dropdown.transform.gameObject.SetActive(true);
            scrollBarPanel.transform.gameObject.SetActive(true);

            YellowDropDown_IndexChanged(0);
        }

    }
    public void showRedScrollBar()
    {
        if (red_Dropdown != null)
        {
            greenStatusButton.transform.localScale = Vector3.one;
            yellowStatusButton.transform.localScale = Vector3.one;
            redStatusButton.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            //healthParameterArray = redHealthParameterArray;
            if (!red_Dropdown.transform.gameObject.activeSelf)
            {
                graphCanvas.transform.gameObject.SetActive(false);
            }

            m_Dropdown.transform.gameObject.SetActive(false);
            green_Dropdown.transform.gameObject.SetActive(false);
            yellow_Dropdown.transform.gameObject.SetActive(false);
            red_Dropdown.transform.gameObject.SetActive(true);
            scrollBarPanel.transform.gameObject.SetActive(true);

            RedDropDown_IndexChanged(0);
        }
    }


    //====================

    void setValues(HealthParameter healthParameter)
    {
        Debug.Log("<color=blue> JITU Selected Option is: </color>" + healthParameter.deviceName);

        try
        {
            this.limit_type = healthParameter.limit_type;

            if (this.limit_type.Equals("none"))
            {
                this.healthValue = float.Parse(healthParameter.healthValue);
                return;
            }
            this.min_hold = float.Parse(healthParameter.min_hold);
            this.streamId = healthParameter.streamId;
            this.displayName = healthParameter.displayName;
            this.BaseValue = float.Parse(healthParameter.BaseValue);
            this.maxValue = float.Parse(healthParameter.maxValue);
            this.num_double_digits = float.Parse(healthParameter.num_double_digits);
            this.units = healthParameter.units;
            this.deviceName = healthParameter.deviceName;
            this.minValue = float.Parse(healthParameter.minValue);

            this.metric_name = healthParameter.metric_name;

            this.healthValue = float.Parse(healthParameter.healthValue);
            this.max_hold = float.Parse(healthParameter.max_hold);

            this.device = healthParameter.device;
            this.Green = float.Parse(healthParameter.Green);
            this.status = healthParameter.status;




            this.range = float.Parse(healthParameter.range);
            this.scale_EqualBlocksCount = int.Parse(healthParameter.scale_EqualBlocksCount);
            this.scale_UnitBlockSize = float.Parse(healthParameter.scale_UnitBlockSize);
            this.scale_FirstBlockSize = float.Parse(healthParameter.scale_FirstBlockSize);
            this.scale_LastBlockSize = float.Parse(healthParameter.scale_LastBlockSize);
            this.scale_FirstBlockMarker = float.Parse(healthParameter.scale_FirstBlockMarker);


            if (healthParameter.YellowN != null)
                this.YellowN = float.Parse(healthParameter.YellowN);
            if (healthParameter.RedP != null)
                this.RedP = float.Parse(healthParameter.RedP);
            if (healthParameter.RedN != null)
                this.RedN = float.Parse(healthParameter.RedN);
            if (healthParameter.YellowP != null)
                this.YellowP = float.Parse(healthParameter.YellowP);


            if (healthParameter.max != null)
                this.max = float.Parse(healthParameter.max);
            if (healthParameter.Red != null)
                this.Red = float.Parse(healthParameter.Red);
            if (healthParameter.Yellow != null)
                this.Yellow = float.Parse(healthParameter.Yellow);

        }
        catch (Exception e)
        {
            this.range = 1;
            this.scale_EqualBlocksCount = 10;
            this.scale_UnitBlockSize = 0.02f;
            this.scale_FirstBlockSize = 0;
            this.scale_LastBlockSize = 0;
            this.scale_FirstBlockMarker = -0.1f;
            Debug.Log("<color=blue> Exception in getting marker parameters </color>" + e.StackTrace);
        }

    }
    void adjustButtonsForMaxMin()
    {
        selectedOption.text = displayName;

        float leftRedWidth = 0;
        float leftYellowWidth = 0;
        float greenWidth = 0;
        float rightYellowWidth = 0;
        float rightRedWidth = 0;

        float leftMarkerPosition = 0;
        float centerMarkerPosition = 0;
        float rightMarkerPosition = 0;


        float panelWidth = 700;
        float panelHeight = 30;
        float markerStartingPoint = (panelWidth / 2) * -1;

        float widthFactor = panelWidth / (maxValue - minValue);
        width_Factor = widthFactor;
        //float widthFactor = panelWidth /120;


        leftRedWidth = (RedN - minValue) * widthFactor;
        leftYellowWidth = (YellowN - RedN) * widthFactor;
        greenWidth = (Green - YellowN) * widthFactor;
        rightYellowWidth = (YellowP - Green) * widthFactor;
        rightRedWidth = (RedP - YellowP) * widthFactor;

        //Debug.Log("<color=red> selectedOption.text  </color>" + selectedOption.text);
        //Debug.Log("<color=red> widthFactor  </color>" + widthFactor);

        //Debug.Log("<color=red> leftRedWidth  </color>" + leftRedWidth);
        //Debug.Log("<color=red> leftYellowWidth  </color>" + leftYellowWidth);
        //Debug.Log("<color=red> greenWidth  </color>" + greenWidth);
        //Debug.Log("<color=red> rightYellowWidth  </color>" + rightYellowWidth);
        //Debug.Log("<color=red> rightRedWidth  </color>" + rightRedWidth);


        leftMarkerPosition = markerStartingPoint + (min_hold - minValue) * widthFactor;
        centerMarkerPosition = markerStartingPoint + (healthValue - minValue) * widthFactor;
        rightMarkerPosition = markerStartingPoint + (max_hold - minValue) * widthFactor;
        //rightMarkerPosition = markerStartingPoint + (maxValue - minValue) * widthFactor;


        leftRedButton.GetComponent<RectTransform>().sizeDelta = new Vector2(leftRedWidth, panelHeight);
        leftYellowButton.GetComponent<RectTransform>().sizeDelta = new Vector2(leftYellowWidth, panelHeight);
        greenButton.GetComponent<RectTransform>().sizeDelta = new Vector2(greenWidth, panelHeight);
        rightYellowButton.GetComponent<RectTransform>().sizeDelta = new Vector2(rightYellowWidth, panelHeight);
        rightRedButton.GetComponent<RectTransform>().sizeDelta = new Vector2(rightRedWidth, panelHeight);


        leftMarker.transform.localPosition = new Vector3(leftMarkerPosition, 0, 0);
        centerMarker.transform.localPosition = new Vector3(centerMarkerPosition, 0, 0);
        rightMarker.transform.localPosition = new Vector3(rightMarkerPosition, 0, 0);
    }

    void adjustButtonsForMax()
    {
        Debug.Log("<color=blue> JITU Inside adjustButtonsForMax </color>");

        selectedOption.text = displayName;

        //float leftRedWidth = 0;
        //float leftYellowWidth = 0;
        float greenWidth = 0;
        float rightYellowWidth = 0;
        float rightRedWidth = 0;

        float leftMarkerPosition = 0;
        float centerMarkerPosition = 0;
        float rightMarkerPosition = 0;


        float panelWidth = 700;
        float panelHeight = 30;
        float markerStartingPoint = (panelWidth / 2) * -1;

        float widthFactor = panelWidth / (maxValue - minValue);
        width_Factor = widthFactor;
        //float widthFactor = panelWidth /120;


        //leftRedWidth = (RedN - minValue) * widthFactor;
        //leftYellowWidth = (YellowN - RedN) * widthFactor;
        greenWidth = (Green - minValue) * widthFactor;
        rightYellowWidth = (Yellow - Green) * widthFactor;
        rightRedWidth = (maxValue - Yellow) * widthFactor;

        Debug.Log("<color=red> selectedOption.text  </color>" + selectedOption.text);
        Debug.Log("<color=red> widthFactor  </color>" + widthFactor);

        Debug.Log("<color=red> greenWidth  </color>" + greenWidth);
        Debug.Log("<color=red> rightYellowWidth  </color>" + rightYellowWidth);
        Debug.Log("<color=red> rightRedWidth  </color>" + rightRedWidth);


        leftMarkerPosition = markerStartingPoint + (min_hold - minValue) * widthFactor;
        centerMarkerPosition = markerStartingPoint + (healthValue - minValue) * widthFactor;
        rightMarkerPosition = markerStartingPoint + (max_hold - minValue) * widthFactor;
        //rightMarkerPosition = markerStartingPoint + (maxValue - minValue) * widthFactor;


        leftRedButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0, panelHeight);
        leftYellowButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0, panelHeight);
        greenButton.GetComponent<RectTransform>().sizeDelta = new Vector2(greenWidth, panelHeight);
        rightYellowButton.GetComponent<RectTransform>().sizeDelta = new Vector2(rightYellowWidth, panelHeight);
        rightRedButton.GetComponent<RectTransform>().sizeDelta = new Vector2(rightRedWidth, panelHeight);


        leftMarker.transform.localPosition = new Vector3(leftMarkerPosition, 0, 0);
        centerMarker.transform.localPosition = new Vector3(centerMarkerPosition, 0, 0);
        rightMarker.transform.localPosition = new Vector3(rightMarkerPosition, 0, 0);
    }


    void buildTextMarkers(HealthParameter healthParameter)
    {
        Button[] buttons = markerTextPanel.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            DestroyImmediate(button.gameObject);
        }
        int repeatButtonCount = scale_EqualBlocksCount;
        float nextMarker = 0;
        if (scale_FirstBlockSize > 0)
        {
            nextMarker = nextMarker + scale_FirstBlockMarker;
            addMarkerButton(scale_FirstBlockSize, nextMarker.ToString("n2"), false);
        }
        else
        {
            nextMarker = minValue;

        }
        for (int i = 0; i < repeatButtonCount; i++)
        {
            nextMarker = nextMarker + scale_UnitBlockSize;
            addMarkerButton(scale_UnitBlockSize, nextMarker.ToString("n2"), false);
        }
        if (scale_LastBlockSize > 0)
        {
            addMarkerButton(scale_LastBlockSize, "", false);
        }

    }
    void addMarkerButton(float buttonWidth, string text, bool lastButton)
    {
        buttonWidth = buttonWidth * width_Factor;
        GameObject buttonPrefab = (GameObject)Resources.Load("KeysightMarkerButton");
        GameObject newButton = (GameObject)Instantiate(buttonPrefab);
        newButton.GetComponent<RectTransform>().sizeDelta =
                   new Vector2(buttonWidth, newButton.GetComponent<RectTransform>().rect.height);
        text = text.Replace(".00", "");
        newButton.GetComponentInChildren<Text>().text = text;
        newButton.transform.SetParent(markerTextPanel.transform);
    }

    //======================================================

    void getKeysightHealthData(string assetNumber)
    {
        string url = GlobalVariables.REST_SERVER + "keysight/health/" + assetNumber;
        HealthDataList healthDataList = null;
        var jsonResponse = WebFunctions.Get(url);
        try
        {
            string result = jsonResponse.text;
            // Debug.Log("<color=blue> @@@@@@@@@@@@@@@@@@@@@@@@  result:   \n  </color>" + result);

            int startIndex = result.IndexOf("GreenHealthData") + "GreenHealthData".Length + 3;
            String greenResult = result.Substring(startIndex);
            greenResult = greenResult.Substring(0, greenResult.IndexOf("]"));
            greenResult = greenResult.Replace("},", "}},");

            startIndex = result.IndexOf("YellowHealthData") + "YellowHealthData".Length + 3;
            String yellowResult = result.Substring(startIndex);
            yellowResult = yellowResult.Substring(0, yellowResult.IndexOf("]"));
            yellowResult = yellowResult.Replace("},", "}},");


            startIndex = result.IndexOf("RedHealthData") + "RedHealthData".Length + 3;
            String redResult = result.Substring(startIndex);
            redResult = redResult.Substring(0, redResult.IndexOf("]"));
            redResult = redResult.Replace("},", "}},");

            //greenHealthParameterArray

            greenHealthParameterArray = populateHealthArray(greenResult);
            yellowHealthParameterArray = populateHealthArray(yellowResult);
            redHealthParameterArray = populateHealthArray(redResult);

            healthParameterArray = null;


            //for (int i = 0; i < greenHealthParameterArray.Length; i++)
            //{
            //    green_DropOptions.Add(greenHealthParameterArray[i].displayName);
            //}
            //green_Dropdown.AddOptions(green_DropOptions);

            //for (int i = 0; i < yellowHealthParameterArray.Length; i++)
            //{
            //    yellow_DropOptions.Add(yellowHealthParameterArray[i].displayName);
            //}
            //yellow_Dropdown.AddOptions(yellow_DropOptions);

            //for (int i = 0; i < redHealthParameterArray.Length; i++)
            //{
            //    red_DropOptions.Add(redHealthParameterArray[i].displayName);
            //}
            //red_Dropdown.AddOptions(red_DropOptions);


            //call here=========

            //Debug.Log("<color=blue> healthParameterArray.Length:   \n  </color>" + healthParameterArray.Length);
        }
        catch (Exception e)
        {
            Debug.Log("<color=blue> First Try Failed because of Exception  </color>" + e.Message);
            getHealthParameterArray();
        }

        if (healthParameterArray == null)
        {
            Debug.Log("<color=blue> POUPLATING DEMO DATA </color>");
            getHealthParameterArray();
        }
        if (healthDataList != null)
        {
            //Debug.Log("<color=blue> healthDataList.Asset : </color>" + healthDataList.Asset);
            //Debug.Log("<color=blue> healthDataList.DataAge : </color>" + healthDataList.DataAge);
        }
        //Debug.Log("<color=blue> healthParameterArray length : </color>" + healthParameterArray.Length);
        //Debug.Log("<color=blue> healthParameterArray displayName : </color>" + healthParameterArray[0]);

    }

    HealthParameter[] populateHealthArray(String result)
    {
        if (String.IsNullOrEmpty(result))
        {
            return null;
        }
        //Debug.Log("<color=blue> populateHealthArray WORDS STRING :   \n  </color>" + result);
        string[] words = result.Split(new string[] { "}," }, StringSplitOptions.None);
        //Debug.Log("<color=blue> words.Length:   \n  </color>" + words.Length);
        HealthParameter[] healthArray = new HealthParameter[words.Length];
        int count = 0;
        foreach (var word in words)
        {
            //Debug.Log("<color=blue> The JSON WORD is: :   \n  </color>" + word);
            healthArray[count] = JsonUtility.FromJson<HealthParameter>(word);
            count++;
            //Debug.Log("<color=blue> The count is: :   \n  </color>" + count);
        }
        return healthArray;
    }


    void getHealthParameterArray()
    {

        try
        {
            string result = "{\"limit_type\":\"max_min\",\"min_hold\":\"37\",\"streamId\":\"532980902\",\"displayName\":\"NE_1>AMC10>AUXM DS75 Temp(degree C)\",\"BaseValue\":\"32.5\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC10>AUXM DS75 Temp\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"37\",\"max_hold\":\"37\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC10>AUXM DS75 Temp\",\"Green\":\"65.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"10.825\",\"streamId\":\"532980903\",\"displayName\":\"NE_1>AMC11>MSB +10V7(volts)\",\"BaseValue\":\"10.8\",\"maxValue\":\"11.419\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +10V7\",\"minValue\":\"9.881\",\"YellowN\":\"10.151\",\"metric_name\":\"\",\"YellowP\":\"11.349\",\"healthValue\":\"10.825\",\"max_hold\":\"10.825\",\"RedP\":\"11.419\",\"RedN\":\"9.951\",\"device\":\"NE_1>AMC11>MSB +10V7\",\"Green\":\"11.449\",\"status\":\"GREEN\",\"range\":\"1.538\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.081\",\"scale_FirstBlockMarker\":\"9.8\",\"scale_LastBlockSize\":\"0.219\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"11.747\",\"streamId\":\"532980904\",\"displayName\":\"NE_1>AMC11>MSB +12V(volts)\",\"BaseValue\":\"12\",\"maxValue\":\"13.374\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +12V\",\"minValue\":\"10.626\",\"YellowN\":\"11.351\",\"metric_name\":\"\",\"YellowP\":\"13.249\",\"healthValue\":\"11.747\",\"max_hold\":\"11.747\",\"RedP\":\"13.374\",\"RedN\":\"10.751\",\"device\":\"NE_1>AMC11>MSB +12V\",\"Green\":\"12.649\",\"status\":\"GREEN\",\"range\":\"2.748\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"-0.126\",\"scale_FirstBlockMarker\":\"10.5\",\"scale_LastBlockSize\":\"0.174\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"3.344\",\"streamId\":\"532980905\",\"displayName\":\"NE_1>AMC11>MSB +3.3V(volts)\",\"BaseValue\":\"3.295\",\"maxValue\":\"3.689\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +3.3V\",\"minValue\":\"2.811\",\"YellowN\":\"3.081\",\"metric_name\":\"\",\"YellowP\":\"3.649\",\"healthValue\":\"3.344\",\"max_hold\":\"3.344\",\"RedP\":\"3.689\",\"RedN\":\"2.851\",\"device\":\"NE_1>AMC11>MSB +3.3V\",\"Green\":\"3.509\",\"status\":\"GREEN\",\"range\":\"0.878\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.09\",\"scale_FirstBlockSize\":\"-0.021\",\"scale_FirstBlockMarker\":\"2.79\",\"scale_LastBlockSize\":\"0.089\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"3.667\",\"streamId\":\"532980906\",\"displayName\":\"NE_1>AMC11>MSB +3V7(volts)\",\"BaseValue\":\"3.7\",\"maxValue\":\"4.084\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +3V7\",\"minValue\":\"3.316\",\"YellowN\":\"3.451\",\"metric_name\":\"\",\"YellowP\":\"4.049\",\"healthValue\":\"3.667\",\"max_hold\":\"3.667\",\"RedP\":\"4.084\",\"RedN\":\"3.351\",\"device\":\"NE_1>AMC11>MSB +3V7\",\"Green\":\"3.949\",\"status\":\"GREEN\",\"range\":\"0.768\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.08\",\"scale_FirstBlockSize\":\"-0.036\",\"scale_FirstBlockMarker\":\"3.28\",\"scale_LastBlockSize\":\"0.084\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"5.5\",\"streamId\":\"532980907\",\"displayName\":\"NE_1>AMC11>MSB +5V5(volts)\",\"BaseValue\":\"5.5\",\"maxValue\":\"6.104\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +5V5\",\"minValue\":\"4.896\",\"YellowN\":\"5.151\",\"metric_name\":\"\",\"YellowP\":\"6.049\",\"healthValue\":\"5.5\",\"max_hold\":\"5.5\",\"RedP\":\"6.104\",\"RedN\":\"4.951\",\"device\":\"NE_1>AMC11>MSB +5V5\",\"Green\":\"5.849\",\"status\":\"GREEN\",\"range\":\"1.208\",\"scale_EqualBlocksCount\":\"12\",\"scale_UnitBlockSize\":\"0.1\",\"scale_FirstBlockSize\":\"-0.096\",\"scale_FirstBlockMarker\":\"4.8\",\"scale_LastBlockSize\":\"0.104\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"8.478\",\"streamId\":\"532980908\",\"displayName\":\"NE_1>AMC11>MSB +8V5(volts)\",\"BaseValue\":\"8.5\",\"maxValue\":\"9.324\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +8V5\",\"minValue\":\"7.676\",\"YellowN\":\"7.951\",\"metric_name\":\"\",\"YellowP\":\"9.249\",\"healthValue\":\"8.478\",\"max_hold\":\"8.478\",\"RedP\":\"9.324\",\"RedN\":\"7.751\",\"device\":\"NE_1>AMC11>MSB +8V5\",\"Green\":\"9.049\",\"status\":\"GREEN\",\"range\":\"1.648\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.076\",\"scale_FirstBlockMarker\":\"7.6\",\"scale_LastBlockSize\":\"0.124\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980909\",\"displayName\":\"NE_1>AMC11>MSB Hotswap\",\"BaseValue\":\"0.5\",\"maxValue\":\"2.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC11>MSB Hotswap\",\"minValue\":\"-1.204\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"2.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"2.204\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC11>MSB Hotswap\",\"Green\":\"2.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"11\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.004\",\"scale_FirstBlockMarker\":\"-1.2\",\"scale_LastBlockSize\":\"0.104\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"5.667\",\"streamId\":\"532980910\",\"displayName\":\"NE_1>AMC11>MSB Isense(amps)\",\"BaseValue\":\"7.75\",\"maxValue\":\"17.904\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC11>MSB Isense\",\"minValue\":\"-0.904\",\"YellowN\":\"0.451\",\"metric_name\":\"\",\"YellowP\":\"17.049\",\"healthValue\":\"5.667\",\"max_hold\":\"5.667\",\"RedP\":\"17.904\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB Isense\",\"Green\":\"15.049\",\"status\":\"GREEN\",\"range\":\"18.808\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.904\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"1.904\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"37\",\"streamId\":\"532980911\",\"displayName\":\"NE_1>AMC11>MSB PCB Temp 1(degree C)\",\"BaseValue\":\"30\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB PCB Temp 1\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"37\",\"max_hold\":\"37\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB PCB Temp 1\",\"Green\":\"60.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"31\",\"streamId\":\"532980912\",\"displayName\":\"NE_1>AMC11>MSB PCB Temp 2(degree C)\",\"BaseValue\":\"30\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB PCB Temp 2\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"31\",\"max_hold\":\"31\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB PCB Temp 2\",\"Green\":\"60.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"2\",\"streamId\":\"532980913\",\"displayName\":\"NE_1>AMC11>MSB Power Good\",\"BaseValue\":\"1.5\",\"maxValue\":\"3.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC11>MSB Power Good\",\"minValue\":\"-0.204\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"3.049\",\"healthValue\":\"2\",\"max_hold\":\"2\",\"RedP\":\"3.204\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB Power Good\",\"Green\":\"3.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"10\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.204\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"0.204\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"51.813\",\"streamId\":\"532980914\",\"displayName\":\"NE_1>AMC11>MSB RFB_TEMP_RX1_RX2(degree C)\",\"BaseValue\":\"35\",\"maxValue\":\"84.054\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB RFB_TEMP_RX1_RX2\",\"minValue\":\"-4.054\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"80.049\",\"healthValue\":\"51.813\",\"max_hold\":\"51.813\",\"RedP\":\"84.054\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB RFB_TEMP_RX1_RX2\",\"Green\":\"70.049\",\"status\":\"GREEN\",\"range\":\"88.108\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"4.054\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"4.054\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"47.063\",\"streamId\":\"532980915\",\"displayName\":\"NE_1>AMC11>MSB RFB_TEMP_TX1_TX2(degree C)\",\"BaseValue\":\"35\",\"maxValue\":\"84.054\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB RFB_TEMP_TX1_TX2\",\"minValue\":\"-4.054\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"80.049\",\"healthValue\":\"47.063\",\"max_hold\":\"47.063\",\"RedP\":\"84.054\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB RFB_TEMP_TX1_TX2\",\"Green\":\"70.049\",\"status\":\"GREEN\",\"range\":\"88.108\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"4.054\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"4.054\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"50.375\",\"streamId\":\"532980916\",\"displayName\":\"NE_1>AMC11>MSB RFB_TEMP_TX3_TX4(degree C)\",\"BaseValue\":\"35\",\"maxValue\":\"84.054\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB RFB_TEMP_TX3_TX4\",\"minValue\":\"-4.054\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"80.049\",\"healthValue\":\"50.375\",\"max_hold\":\"50.375\",\"RedP\":\"84.054\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB RFB_TEMP_TX3_TX4\",\"Green\":\"70.049\",\"status\":\"GREEN\",\"range\":\"88.108\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"4.054\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"4.054\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"36.375\",\"streamId\":\"532980917\",\"displayName\":\"NE_1>AMC11>MSB RFIO_TEMP_RX(degree C)\",\"BaseValue\":\"30\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB RFIO_TEMP_RX\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"36.375\",\"max_hold\":\"36.375\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB RFIO_TEMP_RX\",\"Green\":\"60.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"36.625\",\"streamId\":\"532980918\",\"displayName\":\"NE_1>AMC11>MSB RFIO_TEMP_TCT(degree C)\",\"BaseValue\":\"30\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB RFIO_TEMP_TCT\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"36.625\",\"max_hold\":\"36.625\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB RFIO_TEMP_TCT\",\"Green\":\"60.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"36.75\",\"streamId\":\"532980919\",\"displayName\":\"NE_1>AMC11>MSB RFIO_TEMP_TX(degree C)\",\"BaseValue\":\"30\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC11>MSB RFIO_TEMP_TX\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"36.75\",\"max_hold\":\"36.75\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB RFIO_TEMP_TX\",\"Green\":\"60.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980920\",\"displayName\":\"NE_1>AMC11>MSB Watchdog\",\"BaseValue\":\"1.5\",\"maxValue\":\"3.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC11>MSB Watchdog\",\"minValue\":\"-0.204\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"3.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"3.204\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC11>MSB Watchdog\",\"Green\":\"3.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"10\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.204\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"0.204\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"10.825\",\"streamId\":\"532980921\",\"displayName\":\"NE_1>AMC12>MSB +10V7(volts)\",\"BaseValue\":\"10.8\",\"maxValue\":\"11.419\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC12>MSB +10V7\",\"minValue\":\"9.881\",\"YellowN\":\"10.151\",\"metric_name\":\"\",\"YellowP\":\"11.349\",\"healthValue\":\"10.825\",\"max_hold\":\"10.825\",\"RedP\":\"11.419\",\"RedN\":\"9.951\",\"device\":\"NE_1>AMC12>MSB +10V7\",\"Green\":\"11.449\",\"status\":\"GREEN\",\"range\":\"1.538\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.081\",\"scale_FirstBlockMarker\":\"9.8\",\"scale_LastBlockSize\":\"0.219\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"11.747\",\"streamId\":\"532980922\",\"displayName\":\"NE_1>AMC12>MSB +12V(volts)\",\"BaseValue\":\"12\",\"maxValue\":\"13.374\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC12>MSB +12V\",\"minValue\":\"10.626\",\"YellowN\":\"11.351\",\"metric_name\":\"\",\"YellowP\":\"13.249\",\"healthValue\":\"11.747\",\"max_hold\":\"11.747\",\"RedP\":\"13.374\",\"RedN\":\"10.751\",\"device\":\"NE_1>AMC12>MSB +12V\",\"Green\":\"12.649\",\"status\":\"GREEN\",\"range\":\"2.748\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"-0.126\",\"scale_FirstBlockMarker\":\"10.5\",\"scale_LastBlockSize\":\"0.174\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"3.33\",\"streamId\":\"532980923\",\"displayName\":\"NE_1>AMC12>MSB +3.3V(volts)\",\"BaseValue\":\"3.295\",\"maxValue\":\"3.689\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC12>MSB +3.3V\",\"minValue\":\"2.811\",\"YellowN\":\"3.081\",\"metric_name\":\"\",\"YellowP\":\"3.649\",\"healthValue\":\"3.33\",\"max_hold\":\"3.33\",\"RedP\":\"3.689\",\"RedN\":\"2.851\",\"device\":\"NE_1>AMC12>MSB +3.3V\",\"Green\":\"3.509\",\"status\":\"GREEN\",\"range\":\"0.878\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.09\",\"scale_FirstBlockSize\":\"-0.021\",\"scale_FirstBlockMarker\":\"2.79\",\"scale_LastBlockSize\":\"0.089\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"3.686\",\"streamId\":\"532980924\",\"displayName\":\"NE_1>AMC12>MSB +3V7(volts)\",\"BaseValue\":\"3.7\",\"maxValue\":\"4.084\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC12>MSB +3V7\",\"minValue\":\"3.316\",\"YellowN\":\"3.451\",\"metric_name\":\"\",\"YellowP\":\"4.049\",\"healthValue\":\"3.686\",\"max_hold\":\"3.686\",\"RedP\":\"4.084\",\"RedN\":\"3.351\",\"device\":\"NE_1>AMC12>MSB +3V7\",\"Green\":\"3.949\",\"status\":\"GREEN\",\"range\":\"0.768\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.08\",\"scale_FirstBlockSize\":\"-0.036\",\"scale_FirstBlockMarker\":\"3.28\",\"scale_LastBlockSize\":\"0.084\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"5.441\",\"streamId\":\"532980925\",\"displayName\":\"NE_1>AMC12>MSB +5V5(volts)\",\"BaseValue\":\"5.5\",\"maxValue\":\"6.104\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC12>MSB +5V5\",\"minValue\":\"4.896\",\"YellowN\":\"5.151\",\"metric_name\":\"\",\"YellowP\":\"6.049\",\"healthValue\":\"5.441\",\"max_hold\":\"5.441\",\"RedP\":\"6.104\",\"RedN\":\"4.951\",\"device\":\"NE_1>AMC12>MSB +5V5\",\"Green\":\"5.849\",\"status\":\"GREEN\",\"range\":\"1.208\",\"scale_EqualBlocksCount\":\"12\",\"scale_UnitBlockSize\":\"0.1\",\"scale_FirstBlockSize\":\"-0.096\",\"scale_FirstBlockMarker\":\"4.8\",\"scale_LastBlockSize\":\"0.104\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"8.433\",\"streamId\":\"532980926\",\"displayName\":\"NE_1>AMC12>MSB +8V5(volts)\",\"BaseValue\":\"8.5\",\"maxValue\":\"9.324\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC12>MSB +8V5\",\"minValue\":\"7.676\",\"YellowN\":\"7.951\",\"metric_name\":\"\",\"YellowP\":\"9.249\",\"healthValue\":\"8.433\",\"max_hold\":\"8.433\",\"RedP\":\"9.324\",\"RedN\":\"7.751\",\"device\":\"NE_1>AMC12>MSB +8V5\",\"Green\":\"9.049\",\"status\":\"GREEN\",\"range\":\"1.648\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.076\",\"scale_FirstBlockMarker\":\"7.6\",\"scale_LastBlockSize\":\"0.124\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980927\",\"displayName\":\"NE_1>AMC12>MSB Hotswap\",\"BaseValue\":\"0.5\",\"maxValue\":\"2.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC12>MSB Hotswap\",\"minValue\":\"-1.204\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"2.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"2.204\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC12>MSB Hotswap\",\"Green\":\"2.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"11\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.004\",\"scale_FirstBlockMarker\":\"-1.2\",\"scale_LastBlockSize\":\"0.104\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"4.404\",\"streamId\":\"532980928\",\"displayName\":\"NE_1>AMC12>MSB Isense(amps)\",\"BaseValue\":\"7.75\",\"maxValue\":\"17.904\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC12>MSB Isense\",\"minValue\":\"-0.904\",\"YellowN\":\"0.451\",\"metric_name\":\"\",\"YellowP\":\"17.049\",\"healthValue\":\"4.404\",\"max_hold\":\"4.404\",\"RedP\":\"17.904\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB Isense\",\"Green\":\"15.049\",\"status\":\"GREEN\",\"range\":\"18.808\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.904\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"1.904\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"46\",\"streamId\":\"532980929\",\"displayName\":\"NE_1>AMC12>MSB PCB Temp 1(degree C)\",\"BaseValue\":\"30\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC12>MSB PCB Temp 1\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"46\",\"max_hold\":\"46\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB PCB Temp 1\",\"Green\":\"60.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"31\",\"streamId\":\"532980930\",\"displayName\":\"NE_1>AMC12>MSB PCB Temp 2(degree C)\",\"BaseValue\":\"30\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC12>MSB PCB Temp 2\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"31\",\"max_hold\":\"31\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB PCB Temp 2\",\"Green\":\"60.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"2\",\"streamId\":\"532980931\",\"displayName\":\"NE_1>AMC12>MSB Power Good\",\"BaseValue\":\"1.5\",\"maxValue\":\"3.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC12>MSB Power Good\",\"minValue\":\"-0.204\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"3.049\",\"healthValue\":\"2\",\"max_hold\":\"2\",\"RedP\":\"3.204\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB Power Good\",\"Green\":\"3.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"10\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.204\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"0.204\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"53.125\",\"streamId\":\"532980932\",\"displayName\":\"NE_1>AMC12>MSB RFB_TEMP_RX1_RX2(degree C)\",\"BaseValue\":\"35\",\"maxValue\":\"84.054\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC12>MSB RFB_TEMP_RX1_RX2\",\"minValue\":\"-4.054\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"80.049\",\"healthValue\":\"53.125\",\"max_hold\":\"53.125\",\"RedP\":\"84.054\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB RFB_TEMP_RX1_RX2\",\"Green\":\"70.049\",\"status\":\"GREEN\",\"range\":\"88.108\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"4.054\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"4.054\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"50.813\",\"streamId\":\"532980933\",\"displayName\":\"NE_1>AMC12>MSB RFB_TEMP_TX1_TX2(degree C)\",\"BaseValue\":\"35\",\"maxValue\":\"84.054\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC12>MSB RFB_TEMP_TX1_TX2\",\"minValue\":\"-4.054\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"80.049\",\"healthValue\":\"50.813\",\"max_hold\":\"50.813\",\"RedP\":\"84.054\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB RFB_TEMP_TX1_TX2\",\"Green\":\"70.049\",\"status\":\"GREEN\",\"range\":\"88.108\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"4.054\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"4.054\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"50.438\",\"streamId\":\"532980934\",\"displayName\":\"NE_1>AMC12>MSB RFB_TEMP_TX3_TX4(degree C)\",\"BaseValue\":\"35\",\"maxValue\":\"84.054\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC12>MSB RFB_TEMP_TX3_TX4\",\"minValue\":\"-4.054\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"80.049\",\"healthValue\":\"50.438\",\"max_hold\":\"50.438\",\"RedP\":\"84.054\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB RFB_TEMP_TX3_TX4\",\"Green\":\"70.049\",\"status\":\"GREEN\",\"range\":\"88.108\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"4.054\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"4.054\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980935\",\"displayName\":\"NE_1>AMC12>MSB Watchdog\",\"BaseValue\":\"1.5\",\"maxValue\":\"3.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC12>MSB Watchdog\",\"minValue\":\"-0.204\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"3.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"3.204\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC12>MSB Watchdog\",\"Green\":\"3.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"10\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.204\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"0.204\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0.137\",\"streamId\":\"532980616\",\"displayName\":\"NE_1>AMC1>ISense0(amps)\",\"BaseValue\":\"-1.9999999999999996\",\"maxValue\":\"8.954\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC1>ISense0\",\"minValue\":\"-10.954\",\"YellowN\":\"-10.049\",\"metric_name\":\"\",\"YellowP\":\"8.049\",\"healthValue\":\"0.137\",\"max_hold\":\"0.137\",\"RedP\":\"8.954\",\"RedN\":\"-10.049\",\"device\":\"NE_1>AMC1>ISense0\",\"Green\":\"6.049\",\"status\":\"GREEN\",\"range\":\"19.908\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.954\",\"scale_FirstBlockMarker\":\"-10.0\",\"scale_LastBlockSize\":\"0.954\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0.576\",\"streamId\":\"532980617\",\"displayName\":\"NE_1>AMC1>ISense2(amps)\",\"BaseValue\":\"-4\",\"maxValue\":\"4.754\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC1>ISense2\",\"minValue\":\"-10.754\",\"YellowN\":\"-10.049\",\"metric_name\":\"\",\"YellowP\":\"4.049\",\"healthValue\":\"0.576\",\"max_hold\":\"0.576\",\"RedP\":\"4.754\",\"RedN\":\"-10.049\",\"device\":\"NE_1>AMC1>ISense2\",\"Green\":\"2.049\",\"status\":\"GREEN\",\"range\":\"15.508\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.754\",\"scale_FirstBlockMarker\":\"-10.0\",\"scale_LastBlockSize\":\"0.754\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0.012\",\"streamId\":\"532980618\",\"displayName\":\"NE_1>AMC1>ISense3(amps)\",\"BaseValue\":\"-3.75\",\"maxValue\":\"4.754\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC1>ISense3\",\"minValue\":\"-10.754\",\"YellowN\":\"-10.049\",\"metric_name\":\"\",\"YellowP\":\"4.049\",\"healthValue\":\"0.012\",\"max_hold\":\"0.012\",\"RedP\":\"4.754\",\"RedN\":\"-10.049\",\"device\":\"NE_1>AMC1>ISense3\",\"Green\":\"2.549\",\"status\":\"GREEN\",\"range\":\"15.508\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.754\",\"scale_FirstBlockMarker\":\"-10.0\",\"scale_LastBlockSize\":\"0.754\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"1.361\",\"streamId\":\"532980619\",\"displayName\":\"NE_1>AMC1>ISense4(amps)\",\"BaseValue\":\"-2.7499999999999996\",\"maxValue\":\"5.804\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC1>ISense4\",\"minValue\":\"-10.804\",\"YellowN\":\"-10.049\",\"metric_name\":\"\",\"YellowP\":\"5.049\",\"healthValue\":\"1.361\",\"max_hold\":\"1.361\",\"RedP\":\"5.804\",\"RedN\":\"-10.049\",\"device\":\"NE_1>AMC1>ISense4\",\"Green\":\"4.549\",\"status\":\"GREEN\",\"range\":\"16.608\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.804\",\"scale_FirstBlockMarker\":\"-10.0\",\"scale_LastBlockSize\":\"1.804\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0.725\",\"streamId\":\"532980620\",\"displayName\":\"NE_1>AMC1>ISense5(amps)\",\"BaseValue\":\"-1.9999999999999996\",\"maxValue\":\"8.954\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC1>ISense5\",\"minValue\":\"-10.954\",\"YellowN\":\"-10.049\",\"metric_name\":\"\",\"YellowP\":\"8.049\",\"healthValue\":\"0.725\",\"max_hold\":\"0.725\",\"RedP\":\"8.954\",\"RedN\":\"-10.049\",\"device\":\"NE_1>AMC1>ISense5\",\"Green\":\"6.049\",\"status\":\"GREEN\",\"range\":\"19.908\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.954\",\"scale_FirstBlockMarker\":\"-10.0\",\"scale_LastBlockSize\":\"0.954\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0.098\",\"streamId\":\"532980621\",\"displayName\":\"NE_1>AMC1>ISense6(amps)\",\"BaseValue\":\"-1.4999999999999996\",\"maxValue\":\"8.954\",\"num_double_digits\":\"3\",\"units\":\"amps\",\"deviceName\":\"NE_1>AMC1>ISense6\",\"minValue\":\"-10.954\",\"YellowN\":\"-10.049\",\"metric_name\":\"\",\"YellowP\":\"8.049\",\"healthValue\":\"0.098\",\"max_hold\":\"0.098\",\"RedP\":\"8.954\",\"RedN\":\"-10.049\",\"device\":\"NE_1>AMC1>ISense6\",\"Green\":\"7.049\",\"status\":\"GREEN\",\"range\":\"19.908\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"2.0\",\"scale_FirstBlockSize\":\"0.954\",\"scale_FirstBlockMarker\":\"-10.0\",\"scale_LastBlockSize\":\"0.954\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980622\",\"displayName\":\"NE_1>AMC1>KU Fault_0\",\"BaseValue\":\"127.49999999999999\",\"maxValue\":\"268.904\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC1>KU Fault_0\",\"minValue\":\"-13.904\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"256.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"268.904\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC1>KU Fault_0\",\"Green\":\"256.049\",\"status\":\"GREEN\",\"range\":\"282.808\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"30.0\",\"scale_FirstBlockSize\":\"13.904\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"28.904\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980623\",\"displayName\":\"NE_1>AMC1>KU Fault_1\",\"BaseValue\":\"127.49999999999999\",\"maxValue\":\"268.904\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC1>KU Fault_1\",\"minValue\":\"-13.904\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"256.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"268.904\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC1>KU Fault_1\",\"Green\":\"256.049\",\"status\":\"GREEN\",\"range\":\"282.808\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"30.0\",\"scale_FirstBlockSize\":\"13.904\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"28.904\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980624\",\"displayName\":\"NE_1>AMC1>KU Fault_2\",\"BaseValue\":\"127.49999999999999\",\"maxValue\":\"268.904\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC1>KU Fault_2\",\"minValue\":\"-13.904\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"256.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"268.904\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC1>KU Fault_2\",\"Green\":\"256.049\",\"status\":\"GREEN\",\"range\":\"282.808\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"30.0\",\"scale_FirstBlockSize\":\"13.904\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"28.904\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"37\",\"streamId\":\"532980625\",\"displayName\":\"NE_1>AMC1>KU Tsense_Int(degree C)\",\"BaseValue\":\"40\",\"maxValue\":\"100.054\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC1>KU Tsense_Int\",\"minValue\":\"-10.054\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"95.049\",\"healthValue\":\"37\",\"max_hold\":\"37\",\"RedP\":\"100.054\",\"RedN\":\"-5.049\",\"device\":\"NE_1>AMC1>KU Tsense_Int\",\"Green\":\"80.049\",\"status\":\"GREEN\",\"range\":\"110.108\",\"scale_EqualBlocksCount\":\"11\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"0.054\",\"scale_FirstBlockMarker\":\"-10.0\",\"scale_LastBlockSize\":\"0.054\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"11.8\",\"streamId\":\"532980626\",\"displayName\":\"NE_1>AMC1>KU Vin_sns(volts)\",\"BaseValue\":\"11.85\",\"maxValue\":\"13.374\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC1>KU Vin_sns\",\"minValue\":\"10.626\",\"YellowN\":\"11.051\",\"metric_name\":\"\",\"YellowP\":\"13.249\",\"healthValue\":\"11.8\",\"max_hold\":\"11.8\",\"RedP\":\"13.374\",\"RedN\":\"10.751\",\"device\":\"NE_1>AMC1>KU Vin_sns\",\"Green\":\"12.649\",\"status\":\"GREEN\",\"range\":\"2.748\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"-0.126\",\"scale_FirstBlockMarker\":\"10.5\",\"scale_LastBlockSize\":\"0.174\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980627\",\"displayName\":\"NE_1>AMC1>PWR_CTL_PGOOD\",\"BaseValue\":\"127.49999999999999\",\"maxValue\":\"268.904\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC1>PWR_CTL_PGOOD\",\"minValue\":\"-13.904\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"256.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"268.904\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC1>PWR_CTL_PGOOD\",\"Green\":\"256.049\",\"status\":\"GREEN\",\"range\":\"282.808\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"30.0\",\"scale_FirstBlockSize\":\"13.904\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"28.904\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"11.747\",\"streamId\":\"532980628\",\"displayName\":\"NE_1>AMC1>SPB Plus +12V(volts)\",\"BaseValue\":\"12\",\"maxValue\":\"13.374\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC1>SPB Plus +12V\",\"minValue\":\"10.626\",\"YellowN\":\"11.351\",\"metric_name\":\"\",\"YellowP\":\"13.249\",\"healthValue\":\"11.747\",\"max_hold\":\"11.747\",\"RedP\":\"13.374\",\"RedN\":\"10.751\",\"device\":\"NE_1>AMC1>SPB Plus +12V\",\"Green\":\"12.649\",\"status\":\"GREEN\",\"range\":\"2.748\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"-0.126\",\"scale_FirstBlockMarker\":\"10.5\",\"scale_LastBlockSize\":\"0.174\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"1.333\",\"streamId\":\"532980629\",\"displayName\":\"NE_1>AMC1>SPB Plus +1V35 ZQ DDR(volts)\",\"BaseValue\":\"1.35\",\"maxValue\":\"1.569\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC1>SPB Plus +1V35 ZQ DDR\",\"minValue\":\"1.131\",\"YellowN\":\"1.251\",\"metric_name\":\"\",\"YellowP\":\"1.549\",\"healthValue\":\"1.333\",\"max_hold\":\"1.333\",\"RedP\":\"1.569\",\"RedN\":\"1.151\",\"device\":\"NE_1>AMC1>SPB Plus +1V35 ZQ DDR\",\"Green\":\"1.449\",\"status\":\"GREEN\",\"range\":\"0.438\",\"scale_EqualBlocksCount\":\"10\",\"scale_UnitBlockSize\":\"0.04\",\"scale_FirstBlockSize\":\"-0.011\",\"scale_FirstBlockMarker\":\"1.12\",\"scale_LastBlockSize\":\"0.049\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"1.784\",\"streamId\":\"532980630\",\"displayName\":\"NE_1>AMC1>SPB Plus +1V8 KU AUX(volts)\",\"BaseValue\":\"1.8\",\"maxValue\":\"2.074\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC1>SPB Plus +1V8 KU AUX\",\"minValue\":\"1.526\",\"YellowN\":\"1.651\",\"metric_name\":\"\",\"YellowP\":\"2.049\",\"healthValue\":\"1.784\",\"max_hold\":\"1.784\",\"RedP\":\"2.074\",\"RedN\":\"1.551\",\"device\":\"NE_1>AMC1>SPB Plus +1V8 KU AUX\",\"Green\":\"1.949\",\"status\":\"GREEN\",\"range\":\"0.548\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.06\",\"scale_FirstBlockSize\":\"-0.026\",\"scale_FirstBlockMarker\":\"1.5\",\"scale_LastBlockSize\":\"0.034\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"2.49\",\"streamId\":\"532980631\",\"displayName\":\"NE_1>AMC1>SPB Plus +2V5(volts)\",\"BaseValue\":\"2.5\",\"maxValue\":\"2.774\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC1>SPB Plus +2V5\",\"minValue\":\"2.226\",\"YellowN\":\"2.351\",\"metric_name\":\"\",\"YellowP\":\"2.749\",\"healthValue\":\"2.49\",\"max_hold\":\"2.49\",\"RedP\":\"2.774\",\"RedN\":\"2.251\",\"device\":\"NE_1>AMC1>SPB Plus +2V5\",\"Green\":\"2.649\",\"status\":\"GREEN\",\"range\":\"0.548\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.06\",\"scale_FirstBlockSize\":\"-0.006\",\"scale_FirstBlockMarker\":\"2.22\",\"scale_LastBlockSize\":\"0.014\"}},{\"limit_type\":\"max_min\",\"min_hold\":\"3.344\",\"streamId\":\"532980632\",\"displayName\":\"NE_1>AMC1>SPB Plus +3.3V(volts)\",\"BaseValue\":\"3.3\",\"maxValue\":\"3.689\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC1>SPB Plus +3.3V\",\"minValue\":\"2.811\",\"YellowN\":\"3.051\",\"metric_name\":\"\",\"YellowP\":\"3.649\",\"healthValue\":\"3.344\",\"max_hold\":\"3.344\",\"RedP\":\"3.689\",\"RedN\":\"2.851\",\"device\":\"NE_1>AMC1>SPB Plus +3.3V\",\"Green\":\"3.549\",\"status\":\"GREEN\",\"range\":\"0.878\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.09\",\"scale_FirstBlockSize\":\"-0.021\",\"scale_FirstBlockMarker\":\"2.79\",\"scale_LastBlockSize\":\"0.089\"}";
            string[] words = result.Split(new string[] { "}," }, StringSplitOptions.None);
            //Debug.Log("<color=blue> words.Length:   \n  </color>" + words.Length);
            healthParameterArray = new HealthParameter[words.Length];
            int count = 0;
            foreach (var word in words)
            {
                healthParameterArray[count] = JsonUtility.FromJson<HealthParameter>(word);
                count++;
            }
        }
        catch (Exception e)
        {
            Debug.Log("<color=blue> Second Try also Failed .SORRY !! </color>" + e.Message);
        }
    }

    //===================== backup Newton Version below ==============
    /*
    void getKeysightHealthData(string assetNumber)
    {
        string url = GlobalVariables.REST_SERVER + "keysight/health/" + assetNumber;
        HealthDataList healthDataList = null;
        var jsonResponse = WebFunctions.Get(url);
        Debug.Log("<color=blue> Newton jsonResponse is: \n</color>" + jsonResponse.text.Substring(0,100));
        try
        {
            Debug.Log("<color=blue> Newton Before JsonConvert.DeserializeObject \n</color>");
            var healthDataList1 = JsonConvert.DeserializeObject<HealthDataList>(jsonResponse.text);
            Debug.Log("<color=blue> Newton After JsonConvert.DeserializeObject \n</color>");

            if (healthDataList1.GetType() == typeof(HealthDataList)){
                healthDataList = (HealthDataList)healthDataList1;
                Debug.Log("<color=blue> Newton CASTTYPE MATCHED  </color>");
            }
            else{
                Debug.Log("<color=red> Newton CASTTYPE DID NOT MATCH  </color>");
            }
            //healthDataList = (HealthDataList)JsonConvert.DeserializeObject(jsonResponse.text);
            healthParameterArray = healthDataList1.HealthData;
        }
        catch (Exception e)
        {
            Debug.Log("<color=blue> Newton First Try Failed because of Exception  </color>" + e.Message);
            healthParameterArray = getHealthParameterArray(assetNumber);
        }

        if (healthParameterArray == null)
        {
            Debug.Log("<color=blue> First Try Failed because it was null , with jsonResponse.text </color>" );
            healthParameterArray = getHealthParameterArray(assetNumber);
        }
        if (healthDataList != null)
        {
            Debug.Log("<color=blue> Newton healthDataList.Asset : </color>" + healthDataList.Asset);
            Debug.Log("<color=blue> Newton healthDataList.DataAge : </color>" + healthDataList.DataAge);
        }
        Debug.Log("<color=blue> Newton healthParameterArray length : </color>" + healthParameterArray.Length);
        Debug.Log("<color=blue> Newton healthParameterArray displayName : </color>" + healthParameterArray[0].displayName);


    }
    HealthParameter[] getHealthParameterArray(string assetNumber)
    {
        HealthDataList healthDataList = null;
        HealthParameter[] healthParameters = null;
        string url = GlobalVariables.BACKUP_SERVER + "keysight/health/" + assetNumber;
        //string url = "http://localhost:8025/keysight/health/" + assetNumber;
        var jsonResponse = WebFunctions.Get(url);
        Debug.Log("<color=blue> Second ATTEMPT with BACKUP_SERVER , with jsonResponse.text </color>" + jsonResponse.text.Substring(0,100));
        try
        {
            var healthDataList1 = JsonConvert.DeserializeObject<HealthDataList>(jsonResponse.text);
            if (healthDataList1.GetType() == typeof(HealthDataList))
            {
                healthDataList = (HealthDataList)healthDataList1;
                Debug.Log("<color=blue> Newton CAST TYPE MATCHED  </color>");
            }

            //healthDataList = (HealthDataList)JsonConvert.DeserializeObject(jsonResponse.text);
            Debug.Log("<color=blue> Second Try  , with jsonResponse.text </color>") ;
            healthParameters = healthDataList.HealthData;
            if (healthParameters == null)
            {
                Debug.Log("<color=blue> >>>> Second Try also failed  ,so dispatching hardcoded data </color>");
                string jsonString = "{\"HealthData\":[{\"limit_type\":\"max_min\",\"min_hold\":\"37\",\"streamId\":\"532980902\",\"displayName\":\"NE_1>AMC10>AUXM DS75 Temp(degree C)\",\"BaseValue\":\"32.5\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC10>AUXM DS75 Temp\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"37\",\"max_hold\":\"37\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC10>AUXM DS75 Temp\",\"Green\":\"65.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"},{\"limit_type\":\"max_min\",\"min_hold\":\"10.825\",\"streamId\":\"532980903\",\"displayName\":\"NE_1>AMC11>MSB +10V7(volts)\",\"BaseValue\":\"10.8\",\"maxValue\":\"11.419\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +10V7\",\"minValue\":\"9.881\",\"YellowN\":\"10.151\",\"metric_name\":\"\",\"YellowP\":\"11.349\",\"healthValue\":\"10.825\",\"max_hold\":\"10.825\",\"RedP\":\"11.419\",\"RedN\":\"9.951\",\"device\":\"NE_1>AMC11>MSB +10V7\",\"Green\":\"11.449\",\"status\":\"GREEN\",\"range\":\"1.538\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.081\",\"scale_FirstBlockMarker\":\"9.8\",\"scale_LastBlockSize\":\"0.219\"},{\"limit_type\":\"max_min\",\"min_hold\":\"11.747\",\"streamId\":\"532980904\",\"displayName\":\"NE_1>AMC11>MSB +12V(volts)\",\"BaseValue\":\"12\",\"maxValue\":\"13.374\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +12V\",\"minValue\":\"10.626\",\"YellowN\":\"11.351\",\"metric_name\":\"\",\"YellowP\":\"13.249\",\"healthValue\":\"11.747\",\"max_hold\":\"11.747\",\"RedP\":\"13.374\",\"RedN\":\"10.751\",\"device\":\"NE_1>AMC11>MSB +12V\",\"Green\":\"12.649\",\"status\":\"GREEN\",\"range\":\"2.748\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"-0.126\",\"scale_FirstBlockMarker\":\"10.5\",\"scale_LastBlockSize\":\"0.174\"},{\"limit_type\":\"max_min\",\"min_hold\":\"3.344\",\"streamId\":\"532980905\",\"displayName\":\"NE_1>AMC11>MSB +3.3V(volts)\",\"BaseValue\":\"3.295\",\"maxValue\":\"3.689\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +3.3V\",\"minValue\":\"2.811\",\"YellowN\":\"3.081\",\"metric_name\":\"\",\"YellowP\":\"3.649\",\"healthValue\":\"3.344\",\"max_hold\":\"3.344\",\"RedP\":\"3.689\",\"RedN\":\"2.851\",\"device\":\"NE_1>AMC11>MSB +3.3V\",\"Green\":\"3.509\",\"status\":\"GREEN\",\"range\":\"0.878\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.09\",\"scale_FirstBlockSize\":\"-0.021\",\"scale_FirstBlockMarker\":\"2.79\",\"scale_LastBlockSize\":\"0.089\"},{\"limit_type\":\"max_min\",\"min_hold\":\"3.667\",\"streamId\":\"532980906\",\"displayName\":\"NE_1>AMC11>MSB +3V7(volts)\",\"BaseValue\":\"3.7\",\"maxValue\":\"4.084\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +3V7\",\"minValue\":\"3.316\",\"YellowN\":\"3.451\",\"metric_name\":\"\",\"YellowP\":\"4.049\",\"healthValue\":\"3.667\",\"max_hold\":\"3.667\",\"RedP\":\"4.084\",\"RedN\":\"3.351\",\"device\":\"NE_1>AMC11>MSB +3V7\",\"Green\":\"3.949\",\"status\":\"GREEN\",\"range\":\"0.768\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.08\",\"scale_FirstBlockSize\":\"-0.036\",\"scale_FirstBlockMarker\":\"3.28\",\"scale_LastBlockSize\":\"0.084\"},{\"limit_type\":\"max_min\",\"min_hold\":\"5.5\",\"streamId\":\"532980907\",\"displayName\":\"NE_1>AMC11>MSB +5V5(volts)\",\"BaseValue\":\"5.5\",\"maxValue\":\"6.104\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +5V5\",\"minValue\":\"4.896\",\"YellowN\":\"5.151\",\"metric_name\":\"\",\"YellowP\":\"6.049\",\"healthValue\":\"5.5\",\"max_hold\":\"5.5\",\"RedP\":\"6.104\",\"RedN\":\"4.951\",\"device\":\"NE_1>AMC11>MSB +5V5\",\"Green\":\"5.849\",\"status\":\"GREEN\",\"range\":\"1.208\",\"scale_EqualBlocksCount\":\"12\",\"scale_UnitBlockSize\":\"0.1\",\"scale_FirstBlockSize\":\"-0.096\",\"scale_FirstBlockMarker\":\"4.8\",\"scale_LastBlockSize\":\"0.104\"},{\"limit_type\":\"max_min\",\"min_hold\":\"8.478\",\"streamId\":\"532980908\",\"displayName\":\"NE_1>AMC11>MSB +8V5(volts)\",\"BaseValue\":\"8.5\",\"maxValue\":\"9.324\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +8V5\",\"minValue\":\"7.676\",\"YellowN\":\"7.951\",\"metric_name\":\"\",\"YellowP\":\"9.249\",\"healthValue\":\"8.478\",\"max_hold\":\"8.478\",\"RedP\":\"9.324\",\"RedN\":\"7.751\",\"device\":\"NE_1>AMC11>MSB +8V5\",\"Green\":\"9.049\",\"status\":\"GREEN\",\"range\":\"1.648\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.076\",\"scale_FirstBlockMarker\":\"7.6\",\"scale_LastBlockSize\":\"0.124\"},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980909\",\"displayName\":\"NE_1>AMC11>MSB Hotswap\",\"BaseValue\":\"0.5\",\"maxValue\":\"2.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC11>MSB Hotswap\",\"minValue\":\"-1.204\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"2.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"2.204\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC11>MSB Hotswap\",\"Green\":\"2.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"11\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.004\",\"scale_FirstBlockMarker\":\"-1.2\",\"scale_LastBlockSize\":\"0.104\"}]}";
                healthDataList1 = JsonConvert.DeserializeObject<HealthDataList>(jsonString);
                if (healthDataList1.GetType() == typeof(HealthDataList))
                {
                    healthDataList = (HealthDataList)healthDataList1;
                    Debug.Log("<color=blue> Newton CAST TYPE MATCHED  </color>");
                }

                //healthDataList = (HealthDataList)JsonConvert.DeserializeObject(jsonString);
                healthParameters = healthDataList.HealthData;

            }
        }
        catch (Exception e)
        {
            Debug.Log("<color=blue> >>>> so dispatching hardcoded data </color>" + e.Message);
            string jsonString = "{\"HealthData\":[{\"limit_type\":\"max_min\",\"min_hold\":\"37\",\"streamId\":\"532980902\",\"displayName\":\"NE_1>AMC10>AUXM DS75 Temp(degree C)\",\"BaseValue\":\"32.5\",\"maxValue\":\"73.554\",\"num_double_digits\":\"3\",\"units\":\"degree C\",\"deviceName\":\"NE_1>AMC10>AUXM DS75 Temp\",\"minValue\":\"-3.554\",\"YellowN\":\"-0.049\",\"metric_name\":\"\",\"YellowP\":\"70.049\",\"healthValue\":\"37\",\"max_hold\":\"37\",\"RedP\":\"73.554\",\"RedN\":\"-0.049\",\"device\":\"NE_1>AMC10>AUXM DS75 Temp\",\"Green\":\"65.049\",\"status\":\"GREEN\",\"range\":\"77.108\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"10.0\",\"scale_FirstBlockSize\":\"3.554\",\"scale_FirstBlockMarker\":\"0.0\",\"scale_LastBlockSize\":\"3.554\"},{\"limit_type\":\"max_min\",\"min_hold\":\"10.825\",\"streamId\":\"532980903\",\"displayName\":\"NE_1>AMC11>MSB +10V7(volts)\",\"BaseValue\":\"10.8\",\"maxValue\":\"11.419\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +10V7\",\"minValue\":\"9.881\",\"YellowN\":\"10.151\",\"metric_name\":\"\",\"YellowP\":\"11.349\",\"healthValue\":\"10.825\",\"max_hold\":\"10.825\",\"RedP\":\"11.419\",\"RedN\":\"9.951\",\"device\":\"NE_1>AMC11>MSB +10V7\",\"Green\":\"11.449\",\"status\":\"GREEN\",\"range\":\"1.538\",\"scale_EqualBlocksCount\":\"7\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.081\",\"scale_FirstBlockMarker\":\"9.8\",\"scale_LastBlockSize\":\"0.219\"},{\"limit_type\":\"max_min\",\"min_hold\":\"11.747\",\"streamId\":\"532980904\",\"displayName\":\"NE_1>AMC11>MSB +12V(volts)\",\"BaseValue\":\"12\",\"maxValue\":\"13.374\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +12V\",\"minValue\":\"10.626\",\"YellowN\":\"11.351\",\"metric_name\":\"\",\"YellowP\":\"13.249\",\"healthValue\":\"11.747\",\"max_hold\":\"11.747\",\"RedP\":\"13.374\",\"RedN\":\"10.751\",\"device\":\"NE_1>AMC11>MSB +12V\",\"Green\":\"12.649\",\"status\":\"GREEN\",\"range\":\"2.748\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"-0.126\",\"scale_FirstBlockMarker\":\"10.5\",\"scale_LastBlockSize\":\"0.174\"},{\"limit_type\":\"max_min\",\"min_hold\":\"3.344\",\"streamId\":\"532980905\",\"displayName\":\"NE_1>AMC11>MSB +3.3V(volts)\",\"BaseValue\":\"3.295\",\"maxValue\":\"3.689\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +3.3V\",\"minValue\":\"2.811\",\"YellowN\":\"3.081\",\"metric_name\":\"\",\"YellowP\":\"3.649\",\"healthValue\":\"3.344\",\"max_hold\":\"3.344\",\"RedP\":\"3.689\",\"RedN\":\"2.851\",\"device\":\"NE_1>AMC11>MSB +3.3V\",\"Green\":\"3.509\",\"status\":\"GREEN\",\"range\":\"0.878\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.09\",\"scale_FirstBlockSize\":\"-0.021\",\"scale_FirstBlockMarker\":\"2.79\",\"scale_LastBlockSize\":\"0.089\"},{\"limit_type\":\"max_min\",\"min_hold\":\"3.667\",\"streamId\":\"532980906\",\"displayName\":\"NE_1>AMC11>MSB +3V7(volts)\",\"BaseValue\":\"3.7\",\"maxValue\":\"4.084\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +3V7\",\"minValue\":\"3.316\",\"YellowN\":\"3.451\",\"metric_name\":\"\",\"YellowP\":\"4.049\",\"healthValue\":\"3.667\",\"max_hold\":\"3.667\",\"RedP\":\"4.084\",\"RedN\":\"3.351\",\"device\":\"NE_1>AMC11>MSB +3V7\",\"Green\":\"3.949\",\"status\":\"GREEN\",\"range\":\"0.768\",\"scale_EqualBlocksCount\":\"9\",\"scale_UnitBlockSize\":\"0.08\",\"scale_FirstBlockSize\":\"-0.036\",\"scale_FirstBlockMarker\":\"3.28\",\"scale_LastBlockSize\":\"0.084\"},{\"limit_type\":\"max_min\",\"min_hold\":\"5.5\",\"streamId\":\"532980907\",\"displayName\":\"NE_1>AMC11>MSB +5V5(volts)\",\"BaseValue\":\"5.5\",\"maxValue\":\"6.104\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +5V5\",\"minValue\":\"4.896\",\"YellowN\":\"5.151\",\"metric_name\":\"\",\"YellowP\":\"6.049\",\"healthValue\":\"5.5\",\"max_hold\":\"5.5\",\"RedP\":\"6.104\",\"RedN\":\"4.951\",\"device\":\"NE_1>AMC11>MSB +5V5\",\"Green\":\"5.849\",\"status\":\"GREEN\",\"range\":\"1.208\",\"scale_EqualBlocksCount\":\"12\",\"scale_UnitBlockSize\":\"0.1\",\"scale_FirstBlockSize\":\"-0.096\",\"scale_FirstBlockMarker\":\"4.8\",\"scale_LastBlockSize\":\"0.104\"},{\"limit_type\":\"max_min\",\"min_hold\":\"8.478\",\"streamId\":\"532980908\",\"displayName\":\"NE_1>AMC11>MSB +8V5(volts)\",\"BaseValue\":\"8.5\",\"maxValue\":\"9.324\",\"num_double_digits\":\"3\",\"units\":\"volts\",\"deviceName\":\"NE_1>AMC11>MSB +8V5\",\"minValue\":\"7.676\",\"YellowN\":\"7.951\",\"metric_name\":\"\",\"YellowP\":\"9.249\",\"healthValue\":\"8.478\",\"max_hold\":\"8.478\",\"RedP\":\"9.324\",\"RedN\":\"7.751\",\"device\":\"NE_1>AMC11>MSB +8V5\",\"Green\":\"9.049\",\"status\":\"GREEN\",\"range\":\"1.648\",\"scale_EqualBlocksCount\":\"8\",\"scale_UnitBlockSize\":\"0.2\",\"scale_FirstBlockSize\":\"-0.076\",\"scale_FirstBlockMarker\":\"7.6\",\"scale_LastBlockSize\":\"0.124\"},{\"limit_type\":\"max_min\",\"min_hold\":\"0\",\"streamId\":\"532980909\",\"displayName\":\"NE_1>AMC11>MSB Hotswap\",\"BaseValue\":\"0.5\",\"maxValue\":\"2.204\",\"num_double_digits\":\"3\",\"units\":\"\",\"deviceName\":\"NE_1>AMC11>MSB Hotswap\",\"minValue\":\"-1.204\",\"YellowN\":\"-1.049\",\"metric_name\":\"\",\"YellowP\":\"2.049\",\"healthValue\":\"0\",\"max_hold\":\"0\",\"RedP\":\"2.204\",\"RedN\":\"-1.049\",\"device\":\"NE_1>AMC11>MSB Hotswap\",\"Green\":\"2.049\",\"status\":\"GREEN\",\"range\":\"3.408\",\"scale_EqualBlocksCount\":\"11\",\"scale_UnitBlockSize\":\"0.3\",\"scale_FirstBlockSize\":\"0.004\",\"scale_FirstBlockMarker\":\"-1.2\",\"scale_LastBlockSize\":\"0.104\"}]}";
            var healthDataList1 = JsonConvert.DeserializeObject<HealthDataList>(jsonString);
            if (healthDataList1.GetType() == typeof(HealthDataList))
            {
                healthDataList = (HealthDataList)healthDataList1;
                Debug.Log("<color=blue> Newton CAST TYPE MATCHED  </color>");
            }
            //healthDataList = (HealthDataList)JsonConvert.DeserializeObject(jsonString);
            healthParameters = healthDataList.HealthData;

        }
        return healthParameters;
    }
    */


    public void refereshKeysighData()
    {
        //string url = GlobalVariables.REST_SERVER + "keysight/health/referesh/" + GlobalVariables.KEYSIGHT_ASSET;
        //var jsonResponse = WebFunctions.Get(url);
        //Debug.Log("<color=blue> >>>> jsonResponse response is: </color>" + jsonResponse.text);
        JituMessageBox.DisplayMessageBox(GlobalVariables.KEYSIGHT_ASSET + " Referesh Status",
                                         "Health Data of \"" + GlobalVariables.KEYSIGHT_ASSET + "\" is being refereshed !!!", true, null);

        StartCoroutine(refreshAssetFromServer());
    }

    public IEnumerator refreshAssetFromServer()
    {
        Debug.Log("<color=green> ===  REFERESHING KEYSIGHT_ASSET : </color>" + GlobalVariables.KEYSIGHT_ASSET);
        if (!string.IsNullOrEmpty(GlobalVariables.KEYSIGHT_ASSET))
        {
            string url = GlobalVariables.REST_SERVER + "/keysight/health/referesh";
            WWWForm form = new WWWForm();
            form.AddField("equipmentNo", GlobalVariables.KEYSIGHT_ASSET);
            UnityWebRequest www1 = UnityWebRequest.Post(url, form);
            www1.SetRequestHeader("equipmentNo", GlobalVariables.KEYSIGHT_ASSET);
            yield return www1.SendWebRequest();
        }
        else
        {
            Debug.Log("<color=red> ==== NO KEYSIGHT_ASSET so no referesh====== </color>");

        }

    }

    public void showRefereshButton()
    {
        if (refereshButton != null)
        {
            if (refereshButton.transform.gameObject.activeSelf)
            {
                refereshButton.transform.gameObject.SetActive(false);
            }
            else
            {
                refereshButton.transform.gameObject.SetActive(true);
            }
        }
    }



}

public class HealthDataList
{
    //public string Asset;
    //public string DataAge;
    public HealthParameter[] HealthData;

    //public HealthParameter[] GreenHealthData;
    //public HealthParameter[] YellowHealthData;
    //public HealthParameter[] RedHealthData;
}
public class HealthParameter
{
    public string limit_type;
    public string min_hold;
    public string streamId;
    public string displayName;
    public string BaseValue;
    public string maxValue;
    public string num_double_digits;
    public string units;
    public string deviceName;
    public string minValue;
    public string YellowN;
    public string metric_name;
    public string YellowP;
    public string healthValue;
    public string max_hold;
    public string RedP;
    public string RedN;
    public string device;
    public string Green;
    public string status;

    public string max;
    public string Red;
    public string Yellow;

    public string range;
    public string scale_EqualBlocksCount;
    public string scale_UnitBlockSize;
    public string scale_FirstBlockSize;
    public string scale_FirstBlockMarker;
    public string scale_LastBlockSize;

}
