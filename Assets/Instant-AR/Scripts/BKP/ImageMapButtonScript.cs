using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageMapButtonScript : MonoBehaviour
{

    public float xPos;
    public float yPos;
    public float rowCount;

    // Use this for initialization
    void Start()
    {
        scaleButton();

    }
    public void scaleButton()
    {
        GameObject go = this.gameObject;
        Button button = go.GetComponent<Button>();
        Image[] image = button.GetComponentsInChildren<Image>();
        image[0].transform.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    public void ClickButton()
    {
        //Code to open text box
        InfoEntryBoxScript.DisplayInfoEntryBox(this.gameObject);

        //ChangeSprite();
    }

    public void ChangeSprite()
    {

        GameObject go = this.gameObject;
        Button button = go.GetComponent<Button>();
        Image[] image = go.GetComponentsInChildren<Image>();

        Image[] image1 = button.transform.GetComponentsInChildren<Image>();

        var grandChild = this.gameObject.transform.GetChild(1).gameObject;

        Debug.Log("<color=red>>>><<<The grandChild name is :  </color>" + grandChild);


        float xFact = xPos / rowCount;
        float yFact = yPos / rowCount;
        string buttonPosition = xFact + " x " + yFact;
        Debug.Log("<color=red>>>><<<JITU The Buton position is :  </color>" + buttonPosition);
        //Debug.Log("<color=red>>>><<<The Buton cell size is :  </color>" + GlobalVariables.row_count);
        grandChild.transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
        button.GetComponent<Button>().interactable = false;

    }
}
