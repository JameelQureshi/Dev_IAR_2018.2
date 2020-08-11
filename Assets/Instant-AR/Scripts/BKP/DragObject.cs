using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour
{
    public GameObject canvas;
    public Button ghostButton;


    private Button newButton;

    void Start()
    {

    }


    public void clicked()
    {
        newButton = (Button)Instantiate(ghostButton, ghostButton.transform.position, ghostButton.transform.rotation);
        newButton.transform.localScale = new Vector3(1, 1, 1);
        newButton.name = "NewButton";
        newButton.transform.localPosition = ghostButton.transform.localPosition;
        RectTransform rectTransform = ghostButton.GetComponent<RectTransform>();
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        newButton.transform.SetParent(canvas.transform);
        Debug.Log("<color=red> New Button Created is:   </color>" + newButton.name);

    }




    }
