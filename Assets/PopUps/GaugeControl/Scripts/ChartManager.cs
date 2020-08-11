using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChartManager : MonoBehaviour
{
    [Header("Display")]
    public string m_Title = "Guage";
    public Color m_TitleColor;
    public string m_Unit = "km/h";
    public float GaugeValue;            //Value the gauge is displaying
    public float GaugeSpeed;            //Speed at which the needle should move to a new value

    [Header("Background")]
    public bool m_ColorImage = false;
    public Texture m_BackgroundImage;
    public Color m_BackgroundColor = Color.white;

    [Header("Scale")]
    public bool m_AutoSplit = true;
    public float _scaleStep;
    private float _scaleMinValue;       //This is the minimum value in our Gauge
    private float _scaleMaxValue;       //This is the maximum value in our Gauge
    private float _scaleRange;          //This is the range of values in our gauge
    public Transform ScaleParent;
    public GameObject ScalePrefab;
    public Color _scaleColor;

    [Header("Text")]
    public Transform TextParent;
    public GameObject TextPrefab;
    public float m_Padding = 0;
    public Color _textColor;

    [Header("Ranges")]

    public List<ChartRange> ChartRanges;//Ranges of values to be used in the gauge

    [Header("UI")]

    public Transform GaugeParent;
    public GameObject GaugeRangePrefab;

    public RawImage GaugeNeedle;        //Image representing the gauge needle

    public RawImage m_Image;
    public RawImage m_Color;
    public Text GaugeText;              //Text used to display the current gauge value
    public Text ValueText;              //Text used to display the current gauge value
    //public Text UnitText;              //Text used to display the current gauge value

    float GaugeMinRotation = -90f;      //Maximum rotation of the needle on the gauge
    float GaugeMaxRotation = 90f;      //Minimum rotation of the needle on the gauge

    private float _needleInterpolation; //This is the variable used to facilitate smooth needle animation when it is moving

    private Slider[] _gaugeRangeSliders;
    
    // Use this for initialization
    void Start()
    {
        ReSetGauge();
    }

    public void ReSetGauge()
    {
        GaugeText.text = m_Title;
        ValueText.text = GaugeValue.ToString();
        if (!string.IsNullOrEmpty(m_Unit))
        {
            ValueText.text += " " + m_Unit;
        }
       // UnitText.text = m_Unit;

        GaugeText.color = m_TitleColor;
        ValueText.color = m_TitleColor;
       // UnitText.color = m_TitleColor;


        if (m_ColorImage)
        {
            m_Color.gameObject.SetActive(true);
            m_Image.gameObject.SetActive(false);
            m_Color.color = m_BackgroundColor;
        }
        else
        {
            m_Color.gameObject.SetActive(false);
            m_Image.gameObject.SetActive(true);
            m_Image.texture = m_BackgroundImage;
        }

        for (int i = 0; i < GaugeParent.childCount; i++)
        {
            Destroy(GaugeParent.GetChild(i).gameObject);
        }

        SortRanges();
        if(m_AutoSplit)
        {
            RedefineRanges();
        }

        ManualRanges();
        ShowScale();
        ScaleItem();

        for (int i = 0; i < ChartRanges.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(GaugeRangePrefab) as GameObject;

            obj.transform.SetParent(GaugeParent);

            obj.transform.localScale = Vector3.one;
            obj.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
            obj.transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            obj.name = "Range" + i;
        }

        //set the min value of the scale to something ridiculous (if it starts at 0 and our scale starts at e.g. 5, it will stay at 0
        _scaleMinValue = 999999f;
        _scaleMaxValue = -999999f;

        CalculateGaugeRanges(ChartRanges);
        CalibrateGaugeLegends();
    }

    public void SortRanges()
    {
        for (int i = 0; i < ChartRanges.Count; i++)
        {
            for (int j = i + 1; j < ChartRanges.Count; j++)
            {
                if(ChartRanges[i].StartValue > ChartRanges[j].StartValue)
                {
                    ChartRange tmp = ChartRanges[i];
                    ChartRanges[i] = ChartRanges[j];
                    ChartRanges[j] = tmp;
                }
            }
        }
    }

    public void ManualRanges()
    {
        int i = (int)(_scaleStep * 1000f);
        int j = (int)(ChartRanges[0].StartValue * 1000f - (float)i);

        if(j % i != 0)
        {
            j = j / i;
            ChartRanges[0].StartValue = (float)(j * _scaleStep);
        }

        j = (int)(ChartRanges[ChartRanges.Count - 1].EndValue * 1000f + (float)i);
        if (j % i != 0)
        {
            j = j / i;
            ChartRanges[ChartRanges.Count - 1].EndValue = (float)(j * _scaleStep);
        }
    }

    public void RedefineRanges()
    {
        float i = ChartRanges[ChartRanges.Count - 1].EndValue * 1000f;

        if (i == 0) _scaleStep = 0.00005f;
        else if (i <= 5)
            _scaleStep = 0.0001f;
        else if (i <= 20)
            _scaleStep = 0.001f;
        else if (i <= 50)
            _scaleStep = 0.005f;
        else if (i <= 100)
            _scaleStep = 0.01f;
        else if (i <= 200)
            _scaleStep = 0.01f;
        else if (i <= 500)
            _scaleStep = 0.05f;
        else if (i <= 1000)
            _scaleStep = 0.1f;
        else if (i <= 2000)
            _scaleStep = 0.1f;
        else if (i <= 5000)
            _scaleStep = 0.5f;
        else if (i <= 10000)
            _scaleStep = 1f;
        else if (i <= 20000)
            _scaleStep = 1f;
        else if (i <= 50000)
            _scaleStep = 5f;
        else if (i <= 100000)
            _scaleStep = 10f;
        else if (i <= 200000)
            _scaleStep = 10f;
        else if (i <= 500000)
            _scaleStep = 50f;
        else if (i <= 1000000)
            _scaleStep = 100f;
        else if (i <= 2000000)
            _scaleStep = 100f;
        else if (i <= 5000000)
            _scaleStep = 500f;
        else if (i <= 10000000)
            _scaleStep = 1000f;
        else if (i <= 20000000)
            _scaleStep = 1000f;
        else if (i <= 50000000)
            _scaleStep = 5000f;
        else if (i <= 100000000)
            _scaleStep = 10000f;
        else if (i <= 200000000)
            _scaleStep = 10000f;
        else if (i <= 500000000)
            _scaleStep = 50000f;
        else
            _scaleStep = 50000f;
    }

    public void ShowScale()
    {
        int index = 0;
        float fStepAngle = 180f / ((ChartRanges[ChartRanges.Count - 1].EndValue - ChartRanges[0].StartValue) / _scaleStep);

        for (float i = ChartRanges[0].StartValue; i <= ChartRanges[ChartRanges.Count - 1].EndValue; i+= _scaleStep)
        {
            index++;

            //Scale Text
            GameObject obj1 = GameObject.Instantiate(TextPrefab) as GameObject;

            obj1.transform.SetParent(TextParent);

            obj1.transform.localScale = Vector3.one;
            obj1.name = "ScaleText" + index;
            

            float fWidth = gameObject.GetComponent<RectTransform>().rect.width * (1f - 0.05f) + m_Padding;
            float fHeight = gameObject.GetComponent<RectTransform>().rect.height * (1f - 0.05f) * 0.97f + m_Padding;

            //Vector2 CenterPos = new Vector2(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y);

            Vector2 CenterPos = new Vector2(0, 0);

            float fAngle = (i - ChartRanges[0].StartValue) / _scaleStep * fStepAngle;

            float newX = CenterPos.x 
                + (fWidth / 2f - Mathf.Cos(Mathf.Deg2Rad * fAngle) * fWidth / 2f)
                //- obj1.GetComponent<RectTransform>().rect.width / 2f
                - fWidth / 2f;

            float newY = CenterPos.y 
                + (Mathf.Sin(Mathf.Deg2Rad * fAngle) * fHeight / 2f)
                + obj1.GetComponent<RectTransform>().rect.height / 2f;

            obj1.transform.localPosition = new Vector3(newX, newY, 0);
            obj1.GetComponent<Text>().text = i.ToString();
            obj1.GetComponent<Text>().color = _textColor;
        }
    }

    public void ScaleItem()
    {
        int index = 0;
        float fStepAngle = 180f / ((ChartRanges[ChartRanges.Count - 1].EndValue - ChartRanges[0].StartValue) / _scaleStep);

        for (float i = ChartRanges[0].StartValue; i <= ChartRanges[ChartRanges.Count - 1].EndValue; i += _scaleStep)
        {
            index++;

            float fAngle = (i - ChartRanges[0].StartValue) / _scaleStep * fStepAngle;

            GameObject obj2 = GameObject.Instantiate(ScalePrefab) as GameObject;

            obj2.transform.SetParent(ScaleParent);

            obj2.transform.localScale = Vector3.one;

            obj2.GetComponent<RectTransform>().SetLeft(0);
            obj2.GetComponent<RectTransform>().SetRight(0);
            obj2.GetComponent<RectTransform>().SetTop(0);
            obj2.GetComponent<RectTransform>().SetBottom(0);

            obj2.name = "ScaleImage" + index;
            obj2.GetComponent<Image>().color = _scaleColor;
            obj2.transform.localEulerAngles = new Vector3(0, 0, 0f - fAngle);
        }
    }

    public void SetGaugeValue(float newValue)
    {
        GaugeValue = newValue;
    }

    public void SetGaugeMinRotation(float newValue)
    {
        GaugeMinRotation = newValue;
        ReSetGauge();
    }

    public void SetGaugeMaxRotation(float newValue)
    {
        GaugeMaxRotation = newValue;
        ReSetGauge();
    }


    /// <summary>
    /// This configures and rotates the require gauge range legends to align with the configuration of our gauge
    /// It also hides the legends we are not using
    /// </summary>
    private void CalibrateGaugeLegends()
    {
        _gaugeRangeSliders = GaugeParent.GetComponentsInChildren<Slider>();

        float rangePosition;    //The position that our range ends in. Since our range is 360degrees, this represents the value the legend should have
        float gaugeRotation = GaugeMinRotation * -1;

        //if we have a negative value, we need to convert that to its equivalent value out of 360
        if (gaugeRotation < 0)
        {
            gaugeRotation = 360 + gaugeRotation;
        }

        //Calibrate ranges we are using for rotation, colour, values
        for (int i = 0; i < ChartRanges.Count; i++)
        {
            rangePosition = CalculateGaugeposition(ChartRanges[i].EndValue, _scaleRange, GaugeMinRotation, GaugeMaxRotation);

            //_gaugeRangeSliders[_gaugeRangeSliders.Length - i - 1].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, gaugeRotation));
            _gaugeRangeSliders[_gaugeRangeSliders.Length - i - 1].GetComponentInChildren<Image>().color = ChartRanges[i].Colour;
            _gaugeRangeSliders[_gaugeRangeSliders.Length - i - 1].value = rangePosition - GaugeMinRotation;
        }

        //Hide ranges we are not using (All ranges have the slider control, so count them and deduct from our ranges configured)
        for (int i = 0; i < (GetComponentsInChildren<Slider>().Length - ChartRanges.Count); i++)
        {
            _gaugeRangeSliders[i].GetComponentInChildren<Image>().color = Color.clear;
        }
    }

    /// <summary>
    /// Converts supplied Hex values to a colour object. This is used if the GaugeRange object is changed to receive hex values instead of colour types
    /// Note that if GaugeRange is changed to receive hex values, where the colour is implemented, this method should be used to convert the value to a colour object.
    /// </summary>
    /// <param name="hex">The hex value representing the desired colour</param>
    /// <returns>Color type</returns>
    private Color HexToColor(string hex)
    {
        hex = hex.Replace("0x", ""); //in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", ""); //in case the string is formatted #FFFFFF
        float a = 255f; //assume fully visible unless specified in hex
        float r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        float g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        float b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        }

        Color newColor = new Color(r, g, b, a);
        //return new Color(r, g, b, a);

        return newColor;
    }


    // Update is called once per frame
    void Update()
    {
        MoveNeedle(GaugeValue);
        //if(_needleInterpolation!= 0)
        //{
        //    Debug.Log("<color=green> @@@@@@@@@@ Calling MoveNeedle </color>");
        //    MoveNeedle(GaugeValue);
        //}
        //else
        //{
        //    Debug.Log("<color=red> @@@@@@@@@@ Done with MoveNeedle </color>");
        //}
    }

    /// <summary>
    /// Positions our gauge needle at the right location and colour the text based on the ranges we have
    /// </summary>
    /// <param name="flGaugeValue">The value to be displayed on the gauge</param>
    private void MoveNeedle(float flGaugeValue)
    {
        ValueText.text = flGaugeValue.ToString(CultureInfo.CurrentCulture);
        if (!string.IsNullOrEmpty(m_Unit))
        {
            ValueText.text += " " + m_Unit;
        }

        //Get the range we are in out of our list of ranges
        var ourRange = ChartRanges.FirstOrDefault(x => x.StartValue <= flGaugeValue && x.EndValue >= flGaugeValue);

        ChartRange gaugeRange = ourRange;
        if (gaugeRange != null)
        {
            //If we are in a range, show that range's colour
            //ValueText.color = gaugeRange.Colour;
            //UnitText.color = gaugeRange.Colour;
        }
        else
        {
            //If we are not in a range, default to white
            ValueText.color = Color.white;
            //UnitText.color = Color.white;
        }

        //calculate the gauge position based on our ranges configured for that gauge
        float gaugePosition = CalculateGaugeposition(GaugeValue, _scaleRange, GaugeMinRotation, GaugeMaxRotation);

        if (GaugeNeedle.transform.rotation != Quaternion.Euler(0f, 0f, gaugePosition))
        {
            _needleInterpolation += Time.deltaTime * GaugeSpeed;

            Vector3 newRotation = new Vector3();
            newRotation.x = 0f;
            newRotation.y = 0f;
            //newRotation.z = GaugeValue;
            newRotation.z = -Mathf.Lerp(GaugeNeedle.transform.rotation.z, gaugePosition, _needleInterpolation);

            GaugeNeedle.transform.rotation = Quaternion.Euler(newRotation);
        }
        else
        {
            _needleInterpolation = 0f;
        }
    }

    /// <summary>
    /// Used to Calculate the Min and Max values on the gauge as well as the centre position of the scale and the range of the scale
    /// </summary>
    /// <param name="gaugeRanges">A list of the ranges to be used on the gauge. This is what determines the ranges etc calculated by this method</param>
    private void CalculateGaugeRanges(List<ChartRange> gaugeRanges)
    {

        //get the smallest and largest value over our ranges
        foreach (ChartRange currentRange in gaugeRanges)
        {
            if (_scaleMinValue > currentRange.StartValue)
            {
                _scaleMinValue = currentRange.StartValue;
            }
            if (_scaleMaxValue < currentRange.EndValue)
            {
                _scaleMaxValue = currentRange.EndValue;
            }
        }

        //get the middle of our range (this will represent 0 degrees rotation for the needle)

        //get the range of values in our range
        _scaleRange = _scaleMaxValue - _scaleMinValue;
    }

    /// <summary>
    /// Used to calculate the position on the gauge for a specific value based on current known values of a gauge
    /// This is also used to determine where the scales should be drawn on the gauge
    /// </summary>
    /// <param name="gaugeValue">The value whose position is to be determined</param>
    /// <param name="scaleRange">The range of the scale on the gauge</param>
    /// <param name="gaugeMinRotation">The minimum rotation allowed on the gauge (minimum position)</param>
    /// <param name="gaugeMaxRotation">The maximum rotation allowed on the gauge (maximum position)</param>
    /// <returns></returns>
    private float CalculateGaugeposition(float gaugeValue, float scaleRange, float gaugeMinRotation, float gaugeMaxRotation)
    {
        //float gaugePosition = ((gaugeValue / scaleRange) * ((gaugeMaxRotation - gaugeMinRotation))) + gaugeMinRotation;
        float gaugePosition = (((gaugeValue - _scaleMinValue) / scaleRange) * ((gaugeMaxRotation - gaugeMinRotation))) + gaugeMinRotation;
        return gaugePosition;
    }
}

[Serializable]
public class ChartRange
{
    public Color Colour;        //Colour of the range on the gauge
    public float StartValue;    //Minimum value of the range on the gauge
    public float EndValue;      //Maximum value of the range on the gauge
}

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}