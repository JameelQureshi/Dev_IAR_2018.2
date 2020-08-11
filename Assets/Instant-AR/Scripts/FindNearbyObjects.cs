using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindNearbyObjects : BaseScreen {

    //public GameObject scanLine;
    private GameObject MapPanel;
    private GameObject floatingSpace;
    private GameObject scanLine;
    private bool isClicked = false;
    // Use this for initialization
    void Start () {
        floatingSpace = GameObject.Find("FloatingCanvas");
        scanLine = GameObject.Find("ScanLine");

    }
	
	// Update is called once per frame
	public void clicked () {
        this.gameController.OpenCloudScreen();
        if (!isClicked)
        {
            //GameObject floatingSpace = GameObject.Find("FloatingCanvas"); 
            
            if (floatingSpace != null)
            {
                if (MapPanel == null)
                {
                    MapPanel = (GameObject)Instantiate(Resources.Load("ShowMapPanel"));
                    MapPanel.transform.SetParent(floatingSpace.transform);
                    MapPanel.transform.localScale = Vector3.one;

                    RectTransform mapRectTransform = MapPanel.GetComponent<RectTransform>();
                    mapRectTransform.anchorMin = new Vector2(0, 0);
                    mapRectTransform.anchorMax = new Vector2(1, 1);
                    mapRectTransform.pivot = new Vector2(0, 0);
                    mapRectTransform.offsetMin = new Vector2(0, 0);
                    mapRectTransform.offsetMax = new Vector2(0, 0);
                }
                else
                {
                    MapPanel.transform.localScale = Vector3.one;
                }
                if (scanLine != null)
                {
                    scanLine.SetActive(false);
                }
            }
            isClicked = true;
            Sprite targetImageSprite = Resources.Load<Sprite>("JituSprites/scanline");
            this.gameObject.GetComponent<Button>().image.sprite = targetImageSprite;
        }
        else
        {
            unClick();
        }
    }

    public void unClick()
    {
        if (MapPanel != null)
        {
            //Destroy(MapPanel);
            MapPanel.transform.localScale = Vector3.zero;
            isClicked = false;
        }
        if (scanLine != null)
        {
            scanLine.SetActive(true);
        }
        Sprite targetImageSprite = Resources.Load<Sprite>("JituSprites/CenterMarker");
        this.gameObject.GetComponent<Button>().image.sprite = targetImageSprite;

    }

}
