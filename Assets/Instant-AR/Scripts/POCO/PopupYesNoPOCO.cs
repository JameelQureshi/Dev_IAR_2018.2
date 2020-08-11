using System;
using UnityEngine;

public class PopupYesNoPOCO
{
    public bool isBorder { get; set; }     public Color borderColor { get; set; }     public float borderWidth { get; set; }     public bool isDraggable { get; set; }     public string dragArea { get; set; }     public bool touchSesitive { get; set; }     public string imageSource { get; set; }     public string sourceAsset { get; set; }     public Color color { get; set; }
    public string label { get; set; }
    public float fontSize { get; set; }
    public Color textColor { get; set; }
    public GameObject parentContainer { get; set; }
    public string popupMessage { get; set; }
    public string popupResponse { get; set; }
    public GameObject callingObject { get; set; }
    public bool alignParentSize { get; set; }     public bool destroySiblings { get; set; }

    public PopupYesNoPOCO()
    {
        this.touchSesitive = true;
        this.destroySiblings = true;
    }
}
