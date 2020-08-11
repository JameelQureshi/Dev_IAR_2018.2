using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputField : BaseElement
{
    public TransitionAttributes transition;
    public UIText text;
    public int characterLimit;
    public string contentType;
    public string lineType;
    public UIText placeholder;
    public bool readOnly;
}
