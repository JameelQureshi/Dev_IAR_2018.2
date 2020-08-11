using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPopupScript : MonoBehaviour {

    public GameObject callingObject;
    public bool updateParent;
    public string message;
    public Text PopText;
    public GameObject PopWindow;
    public GameObject closeButton;
    public string response;
    // Use this for initialization
    void Start () {


        if (PopWindow == null)
        {
            GameObject popupPrefab = (GameObject)Resources.Load("UIPopupText");
            PopWindow = (GameObject)Instantiate(popupPrefab);
        }


        RectTransform rectTransform = PopWindow.GetComponent<RectTransform>();
        GameObject floatingSpace = GameObject.Find("FloatingCanvas");
        floatingSpace.transform.SetAsLastSibling();
        if (floatingSpace!=null){
            PopWindow.transform.SetParent(floatingSpace.transform);
            PopWindow.transform.localPosition = Vector3.zero;
        }
        float xPos = rectTransform.rect.width;
        float yPos = rectTransform.rect.height;
        if (!string.IsNullOrEmpty(message)){
            PopText.text = message;
        }
        if(closeButton != null){
            Debug.Log("<color=green> ################## New xPos is  </color>"+ (xPos / 2 + 10));
            closeButton.GetComponent<RectTransform>().localScale = Vector3.one;
            closeButton.GetComponent<RectTransform>().localPosition = new Vector3(xPos / 2 + 10, yPos / 2 + 10, 0);
        }

        this.gameObject.transform.SetAsLastSibling();


    }

    public void closePopUp()
    {
        Destroy(PopWindow);
    }

}
