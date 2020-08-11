using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseElement
{
    public string id;
    public string elementId;
    public string elementType;
    public UIRectTransform rectTransform;
    public ImageAttributes imageAttributes;
    public int sequence;
    public bool visible;
    public bool active;
    public ElementBorder border;
    public ElementDraggable draggable;
    public ElementScreenTouchable screenTouchable;
    public UILayout layout;
    public List<ChildElement> children;
    public string parentElementID;
    public string styleId;
    //public UIEnums.EventTypes eventType;
    //public ChildAsset elementAssets;

    public List<EventAction> eventActions;
}