using System.Collections.Generic;
using UnityEngine;

public class TestChartChange : MonoBehaviour
{
    private ChartManager _myGauge;

    // Use this for initialization
    void Start()
    {
        //_myGauge = GameObject.Find("Gauge").GetComponent<GaugeController>();

        _myGauge = GetComponent<ChartManager>();
    }


    public void SetGaugeValuesRed()
    {
        List<ChartRange> newValues = new List<ChartRange>();

        ChartRange newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 10;
        newSensorLimit.EndValue = 20;
        newSensorLimit.Colour = Color.red;

        newValues.Add(newSensorLimit);


        _myGauge.ChartRanges = newValues;
        _myGauge.ReSetGauge();
    }

    public void SetGaugeValuesRedGreen()
    {
        List<ChartRange> newValues = new List<ChartRange>();

        //Red
        ChartRange newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 10;
        newSensorLimit.EndValue = 20;
        newSensorLimit.Colour = Color.red;
        newValues.Add(newSensorLimit);

        //Green
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 20;
        newSensorLimit.EndValue = 30;
        newSensorLimit.Colour = Color.green;
        newValues.Add(newSensorLimit);


        _myGauge.ChartRanges = newValues;
        _myGauge.ReSetGauge();
    }

    public void SetGaugeValuesRedGreenBlue()
    {
        List<ChartRange> newValues = new List<ChartRange>();

        //Red
        ChartRange newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 10;
        newSensorLimit.EndValue = 20;
        newSensorLimit.Colour = Color.red;
        newValues.Add(newSensorLimit);

        //Green
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 20;
        newSensorLimit.EndValue = 30;
        newSensorLimit.Colour = Color.green;
        newValues.Add(newSensorLimit);

        //Blue
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 30;
        newSensorLimit.EndValue = 40;
        newSensorLimit.Colour = Color.blue;
        newValues.Add(newSensorLimit);

        _myGauge.ChartRanges = newValues;
        _myGauge.ReSetGauge();
    }

    public void SetGaugeValuesTempSensor()
    {
        List<ChartRange> newValues = new List<ChartRange>();
        ChartRange newSensorLimit;

        //Range 1
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 5;
        newSensorLimit.EndValue = 22;
        newSensorLimit.Colour = Color.red;
        newValues.Add(newSensorLimit);

        //Range 2
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 22;
        newSensorLimit.EndValue = 23;
        newSensorLimit.Colour = Color.yellow;
        newValues.Add(newSensorLimit);

        //Range 3
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 23;
        newSensorLimit.EndValue = 27;
        newSensorLimit.Colour = Color.green;
        newValues.Add(newSensorLimit);

        //Range 4
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 27;
        newSensorLimit.EndValue = 28;
        newSensorLimit.Colour = Color.yellow;
        newValues.Add(newSensorLimit);

        //Range 5
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 28;
        newSensorLimit.EndValue = 35;
        newSensorLimit.Colour = Color.red;
        newValues.Add(newSensorLimit);


        _myGauge.ChartRanges = newValues;
        _myGauge.ReSetGauge();

    }

    public void SetGaugeValuesPhSensor()
    {
        List<ChartRange> newValues = new List<ChartRange>();
        ChartRange newSensorLimit;

        //Range 1
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 4;
        newSensorLimit.EndValue = 6;
        newSensorLimit.Colour = Color.red;
        newValues.Add(newSensorLimit);

        //Range 2
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 6;
        newSensorLimit.EndValue = 6.25f;
        newSensorLimit.Colour = Color.yellow;
        newValues.Add(newSensorLimit);

        //Range 3
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 6.25f;
        newSensorLimit.EndValue = 7.75f;
        newSensorLimit.Colour = Color.green;
        newValues.Add(newSensorLimit);

        //Range 4
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 7.25f;
        newSensorLimit.EndValue = 8;
        newSensorLimit.Colour = Color.yellow;
        newValues.Add(newSensorLimit);

        //Range 5
        newSensorLimit = new ChartRange();
        newSensorLimit.StartValue = 8;
        newSensorLimit.EndValue = 10;
        newSensorLimit.Colour = Color.red;
        newValues.Add(newSensorLimit);


        _myGauge.ChartRanges = newValues;
        _myGauge.ReSetGauge();

    }
}
