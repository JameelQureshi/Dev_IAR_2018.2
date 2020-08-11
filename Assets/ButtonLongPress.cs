using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using Michsky.UI.ModernUIPack;


public class ButtonLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEndDragHandler
{
    [SerializeField]
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    private float holdTime = 1f;

    private bool held = false;
    //public UnityEvent onClick = new UnityEvent();

    public UnityEvent onLongPress = new UnityEvent();

    public void OnPointerDown(PointerEventData eventData)
    {
        //held = false;
        Invoke("OnLongPress", holdTime);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelInvoke("OnLongPress");
        //NormalElements();
        //if (!held)
        //    onClick.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke("OnLongPress");
    }

    void OnLongPress()
    {
        //held = true;
        onLongPress.Invoke();
        if (!held)
        {
            LoosenElements();
            held = true;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        NormalElements();
        held = false;
    }


    void LoosenElements()
    {
        GameObject floatingObject;
        foreach (string buttonName in GlobalVariables.ButtonList)
        {
            floatingObject = GameObject.Find(buttonName);
            floatingObject.AddComponent<SwingFloating>();
            floatingObject.AddComponent<WindowDragger>();
            floatingObject.GetComponent<Button>().interactable = false;

        }
    }

    void NormalElements()
    {
        GameObject floatingObject;
        foreach (string buttonName in GlobalVariables.ButtonList)
        {
            floatingObject = GameObject.Find(buttonName);
            SwingFloating swingScript = floatingObject.GetComponent<SwingFloating>();
            WindowDragger dragScript = floatingObject.GetComponent<WindowDragger>();
            if (swingScript != null)
            {
                Destroy(swingScript);
            }
            if (dragScript != null)
            {
                Destroy(dragScript);
            }
            floatingObject.transform.eulerAngles = Vector3.zero;
            floatingObject.GetComponent<Button>().interactable = true;
        }
    }

}