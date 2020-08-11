using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class GaugePOCO
{
    public bool isBorder { get; set; }     public Color borderColor { get; set; }     public float borderWidth { get; set; }     public bool isDraggable { get; set; }     public string dragArea { get; set; }     public bool touchSesitive { get; set; }     public string imageSource { get; set; }     public string sourceAsset { get; set; }     public Color color { get; set; }     public GameObject callingObject { get; set; }     public GameObject parentContainer { get; set; }     public bool alignParentSize { get; set; }     public bool flushOthers { get; set; }     public bool preserveAspect { get; set; }
    public float GaugeRadius { get; set; }
    public string m_Title { get; set; }
    public Color m_TitleColor { get; set; }
    public string m_Unit { get; set; }
    public float GaugeValue { get; set; }
    public List<ChartRange> ChartRanges { get; set; }
    public List<GaugeRange> gaugeRanges { get; set; }

    public GaugePOCO()
    {
        this.flushOthers = true;
    }
     public GaugePOCO(GameObject callingObject, GameObject parentContainer, bool flushOthers, bool preserveAspect,        float GaugeRadius, string m_Title, Color m_TitleColor, string m_Unit, float GaugeValue, List<ChartRange> ChartRanges)
    {
        this.callingObject = callingObject;
        this.parentContainer = parentContainer;
        this.flushOthers = flushOthers;
        this.preserveAspect = preserveAspect;
        this.GaugeRadius = GaugeRadius;
        this.m_Title = m_Title;
        this.m_TitleColor = m_TitleColor;
        this.m_Unit = m_Unit;
        this.GaugeValue = GaugeValue;
        this.ChartRanges = ChartRanges;

    }      public class GaugeRange
    {
        public Color color { get; set; }
        public float start { get; set; }
        public float end { get; set; }
    }     
}
