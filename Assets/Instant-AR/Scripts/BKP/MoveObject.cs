using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveObject : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public GameObject canvas;
    //public Button tempButton2;
    public Button thisButton;
    private Vector3 offset;
    bool toggleButton = true;




    void Start()
    {
        toggleButton = false;

    }

    void Update()
    {
        if (toggleButton)
        {
            OnMouseDown(thisButton);
            OnMouseDrag(thisButton);
            Debug.Log("<color=red> Update button is CLICKED, name is:   </color>" + thisButton.name);

        }
        else
        {
            Debug.Log("<color=red> toggleButton false now, so stop dragging  </color>");

        }

    }

    public void tempClicked()
    {
        GlobalVariables.kickOff = false;
        //thisButton = this.gameObject.GetComponent<Button>();
        //thisButton.name = "JituButton";
        //thisButton.onClick.AddListener(tempClicked1);
        if (toggleButton)
        {
            toggleButton = false;
        }
        else
        {
            toggleButton = true;
        }
    }




    public void clicked()
    {

        GlobalVariables.kickOff = true;


    }

    void OnMouseDown(Button go)
    {

        if (go != null)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(Input.mousePosition);
            offset = Input.mousePosition - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, go.transform.position.z));
        }
    }

    void OnMouseDrag(Button go)
    {
        if (go != null)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, go.transform.position.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            go.transform.position = curPosition;
        }


    }
    void OnMouseUp()
    {
        // If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Drag ended!");
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("<color=red> OnBeginDrag   </color>");
        //OnMouseDown(DraggedInstance);
        toggleButton = true;

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        toggleButton = false;
        Debug.Log("<color=green> OnEndDrag   </color>");
        //OnMouseUp();
    }



}
