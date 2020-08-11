using System;
using UnityEngine;

public class PopupVideoPOCO
{
    public bool isBorder { get; set; }     public Color borderColor { get; set; }     public float borderWidth { get; set; }     public bool isDraggable { get; set; }     public string dragArea { get; set; }     public bool touchSesitive { get; set; }     public string imageSource { get; set; }     public string sourceAsset { get; set; }     public string videoURL { get; set; }     public bool playOnStart { get; set; }     public bool preserveAspect { get; set; }     public GameObject parentContainer { get; set; }
    public GameObject callingObject { get; set; }
    public bool alignParentSize { get; set; }     public bool destroySiblings { get; set; }

    public PopupVideoPOCO()
    {
        this.touchSesitive = true;
        this.playOnStart = true;
        this.preserveAspect = true;
        //this.alignParentSize = false;
        this.destroySiblings = true;
    }
}
