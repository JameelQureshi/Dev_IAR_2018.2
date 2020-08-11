using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupButtons : MonoBehaviour
{
    public static List<ARReportInfo1> reportDetails = new List<ARReportInfo1>();
    public GameObject thisButton;
    public GameObject parentPopUp;
    public Text text;
    // Use this for initialization
    void Start()
    {
        GlobalVariables.CURRENT_USER = ARUtilityTools.getCurrentUser();
    }

    public void clicked()
    {
        GameObject callingObject = parentPopUp.transform.GetComponent<IAPopupScript>().callingObject;
        IAPopupScript popupScript = parentPopUp.transform.GetComponent<IAPopupScript>();
        if (callingObject != null)
        {
            Debug.Log("<color=green> The calling object is not null and it is </color>" + callingObject.name);
            Image img = callingObject.GetComponent<Image>();
            img.color = Color.white;
            Image backgroundPanel = callingObject.transform.GetChild(0).GetComponent<Image>();
            if (backgroundPanel != null)
            {
                backgroundPanel.gameObject.transform.localScale = Vector3.zero;
            }
            if ("yes".Equals(thisButton.name.ToLower()))
            {
                Debug.Log("<color=green> INSIDE YES </color>" + text.text);
                Sprite targetImageSprite = Resources.Load<Sprite>("JituSprites/green");
                callingObject.GetComponent<Image>().sprite = targetImageSprite;
                popupScript.response = "YES";
            }
            if ("no".Equals(thisButton.name.ToLower()))
            {
                Sprite targetImageSprite = Resources.Load<Sprite>("JituSprites/redcross");
                callingObject.GetComponent<Image>().sprite = targetImageSprite;
                popupScript.response = "NO";
            }
        }
        else
        {
            Debug.Log("<color=green> The calling object is null </color>");
        }
        Debug.Log("<color=green> The Button Clicked is " + thisButton.name + " The Message on Click is" + text.text + "</color>");
        System.DateTime theTime = System.DateTime.Now;
        string datetime = theTime.ToString("yyyy-MM-dd\\THH:mm:ss\\Z");
        Debug.Log("<color=green> @@@The datetimer is </color>" + datetime);
        GameObject popUpObject = thisButton.transform.parent.parent.gameObject;
        Debug.Log("<color=green> @@@ popUpObject is </color>" + popUpObject.name);
        List<ColumnValue> cvList = new List<ColumnValue>();
        cvList.Add(new ColumnValue("Message", text.text));
        cvList.Add(new ColumnValue("Reponse", thisButton.name));
        reportDetails.Add(new ARReportInfo1(cvList));
        //BlocklyEvents tbs = new BlocklyEvents();
        Destroy(popUpObject);
        //tbs.executePopUpNextBlock(callingObject, text.text, thisButton.name);
    }
}
