using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicSlider : MonoBehaviour
{

    private Slider slider;
    public static int MaxValue
    {
        set
        {
            PlayerPrefs.SetInt("MaxValue", value);
        }
        get
        {
            return PlayerPrefs.GetInt("MaxValue", 20);
        }
    }

    // Use this for initialization
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = MaxValue;
        slider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    // Update is called once per frame
    private void OnValueChanged()
    {
        StopAllCoroutines();
        StartCoroutine(AdjustSlider());
    }
    IEnumerator AdjustSlider()
    {
        yield return new WaitForSeconds(0.1f);
        float value = slider.value;
        float percentage = value / MaxValue * 100;
        Debug.Log(percentage);
        if (percentage >= 90)
        {
            if (MaxValue <= 10000)
            {
                MaxValue = MaxValue * 5;
                slider.maxValue = MaxValue;
            }
        }
        else if (percentage <= 10)
        {
            if (MaxValue > 20)
            {
                MaxValue = MaxValue / 5;
                slider.maxValue = MaxValue;
            }

        }
    }

    void OnDisabled()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}