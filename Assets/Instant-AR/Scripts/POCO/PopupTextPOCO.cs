using System;
using UnityEngine;

public class PopupTextPOCO
{
    public bool isBorder { get; set; }     public string borderColor { get; set; }     public string borderWidth { get; set; }     public bool isDraggale { get; set; }     public string dragArea { get; set; }     public bool touchSesitive { get; set; }     public string imageSource { get; set; }     public string sourceAsset { get; set; }     public string color { get; set; }
    public string label { get; set; }
    public float fontSize { get; set; }
    public string textColor { get; set; }
    public GameObject parentContainer { get; set; }
    public string PopupMessage { get; set; }
    public string PopupResponse { get; set; }
    public bool alignParentSize { get; set; }     public bool destroySiblings { get; set; }
}
