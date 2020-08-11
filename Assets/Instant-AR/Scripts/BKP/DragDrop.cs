using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject refObject;
    public GameObject cloneParentObject;
    //public TargetInfo targetInfo;
    public string button_position_x;
    public string button_position_y;
    public string button_sprite;

    private Vector3 offset;
    private float image_start_x;
    private float image_start_y;
    private bool inside_Image_Boundary = false;
    private bool intial_Outside_Boundary = true;
    private bool button_Clickable = true;
    private float image_width;
    private float image_height;
    public float xFactor;
    public float yFactor;
    private ButtonInfoMAP buttonInfoMAP;
    void Start()
    {
        if (GlobalVariables.isPublic)
        {
            buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
        }
        else
        {
            buttonInfoMAP = GlobalVariables.buttonInfoMAP;
        }
        if (buttonInfoMAP != null && gameObject.name.Contains("Button_"))
        {
            string infoJsonString = (string)buttonInfoMAP.GetType().GetField(gameObject.name).GetValue(buttonInfoMAP);
            TargetInfo targetInfo = JsonUtility.FromJson<TargetInfo>(infoJsonString);
            button_position_x = targetInfo.buttonPosition_x;
            button_position_y = targetInfo.buttonPosition_y;
            button_sprite = targetInfo.button_sprite;
        }


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        button_Clickable = false;
        if (refObject == null)
        {
            refObject = transform.parent.parent.gameObject;
            string grandparentName = refObject.name;
            Debug.Log("<color=green> $$$$$$$$$$$$$$ grandparentName is:  </color>" + grandparentName);
        }
        RectTransform rectTransform = refObject.GetComponent<RectTransform>();
        image_width = rectTransform.rect.width;
        image_height = rectTransform.rect.height;
        image_start_x = refObject.transform.position.x - image_width / 2;
        image_start_y = refObject.transform.position.y + image_height / 2;



    }

    public void OnDrag(PointerEventData eventData)
    {
        adjustBoundaries();
        if (gameObject.name.Contains("Button_") || gameObject.name.Contains("Clone"))
        {
            if (inside_Image_Boundary || intial_Outside_Boundary)
            {
                offset = Input.mousePosition - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                transform.position = curPosition;
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        adjustBoundaries();
        button_Clickable = true;
        string xValue;
        string yValue;
        if (xFactor > 0.9f)
        {
            xValue = "10";
        }
        else
        {
            xValue = (xFactor + 0.1f).ToString().Substring(2, 1);
        }
        if (yFactor > 0.9f)
        {
            yValue = "10";
        }
        else
        {
            yValue = (yFactor + 0.1f).ToString().Substring(2, 1);
        }
        button_position_x = xFactor.ToString();
        button_position_y = yFactor.ToString();

        if (inside_Image_Boundary && transform.gameObject.name.Contains("Clone"))
        {
            string buttonName = getButtonName();
            Debug.Log("<color=green> OnEndDrag, Button Name is:  </color>" + buttonName);
            transform.gameObject.name = buttonName;
            button_sprite = "JituSprites/" + transform.gameObject.GetComponent<Image>().sprite.name;
            TargetInfo targetInfo = new TargetInfo();
            targetInfo.buttonPosition_x = button_position_x;
            targetInfo.buttonPosition_y = button_position_y;
            targetInfo.button_sprite = button_sprite;
            targetInfo.buttonID = buttonName;
            buttonInfoMAP.GetType().GetField(gameObject.name).SetValue(buttonInfoMAP, JsonUtility.ToJson(targetInfo));

        }
       
        if (transform.gameObject.name.Contains("Button_"))
        {
            ARUtilityTools.updateInfoButtonPoition(transform.gameObject, button_position_x, button_position_y, true);
        }

    }

    private string getButtonName()
    {
        string buttonName = (string)GlobalVariables.availableButtonNames[0];
        GlobalVariables.availableButtonNames.Remove(buttonName);
        //GlobalVariables.availableButtonNames.Sort();

        /*
        string infoJsonString = "";
        for (int i = 0; i < 100; i++)
        {
            string tempName = "Button_" + i.ToString();
            Debug.Log("<color=red>>>><<<The temp Button Name is :  </color>" + tempName);
            if (buttonInfoMAP != null)
            {
                infoJsonString = (string)buttonInfoMAP.GetType().GetField(tempName).GetValue(buttonInfoMAP);
            }
            Debug.Log("<color=red>>>><<<The infoJsonString is :  </color>" + infoJsonString);
            if (string.IsNullOrEmpty(infoJsonString))
            {
                buttonName = tempName;
                break;
            }
        }
        */

        return buttonName;

    }

    private void adjustBoundaries()
    {
        xFactor = (transform.position.x - image_start_x) / image_width;
        yFactor = (image_start_y - transform.position.y) / image_height;
       // Debug.Log("<color=green> xFactor, yFactor:  </color>" + xFactor + " x " + yFactor);

        if (xFactor > 0 && xFactor < 1 && yFactor > 0 && yFactor < 1)
        {
            intial_Outside_Boundary = false;
            inside_Image_Boundary = true;
        }
        else if (inside_Image_Boundary)
        {
            if (buttonInfoMAP != null && gameObject.name.Contains("Button_"))
            {
                buttonInfoMAP.GetType().GetField(gameObject.name).SetValue(buttonInfoMAP, JsonUtility.ToJson(null));
                Destroy(gameObject);
            }

        }
        /*
        else
        {
            inside_Image_Boundary = false;
            if (!intial_Outside_Boundary)
            {
                if (xFactor <= 0)
                {
                    //transform.position = transform.position + new Vector3(5, 0, 0);
                    transform.position = new Vector3(image_start_x + 5, transform.position.y, 0);
                }
                if (yFactor <= 0)
                {
                    transform.position = new Vector3(transform.position.x, image_start_y - 5, 0);
                }
                if (xFactor >= 1)
                {
                    transform.position = new Vector3(image_start_x + image_width - 5, transform.position.y, 0);
                }
                if (yFactor >= 1)
                {
                    transform.position = new Vector3(transform.position.x, image_start_y - image_height + 5, 0);
                }

            }

        }
        */
    }

    public void Clicked()
    {
        if (button_Clickable && !intial_Outside_Boundary)
        {
            Debug.Log("<color=green> OK Button Clicked  </color>");
            InfoEntryBoxScript.DisplayInfoEntryBox(this.gameObject);
            //changeSprite(this.gameObject);
        }
        else
        {
            Debug.Log("<color=red> NO ACTION  </color>");

        }
    }
    public void InsiderClicked()
    {
        if (button_Clickable)
        {
            Debug.Log("<color=green> OK Button Clicked  </color>");
            InfoEntryBoxScript.DisplayInfoEntryBox(this.gameObject);
            //changeSprite(this.gameObject);
        }
        else
        {
            Debug.Log("<color=red> NO ACTION  </color>");

        }
    }

    public void changeSprite(GameObject go)
    {

        Button thisButton = go.GetComponent<Button>();
        Texture2D texture = Resources.Load<Texture2D>("JituSprites/Bug");
        Rect rect = new Rect();
        rect.center = new Vector2(0, 0);
        rect.height = texture.height;
        rect.width = texture.width;
        Sprite tempSprite = UnityEngine.Sprite.Create(texture, rect, new Vector2(1, 1), 100f);
        thisButton.GetComponent<Image>().sprite = tempSprite;
    }

    public void cloneObject_test()
    {

        Debug.Log("<color=green> First Time clicked  </color>");
        Button button = transform.gameObject.GetComponent<Button>();
        //button.onClick.RemoveAllListeners();
        button.onClick.RemoveListener(delegate { cloneObject(); });
        button.onClick.AddListener(delegate { test(); });

    }
    public void test()
    {

        Debug.Log("<color=green> Second Time clicked  </color>");

    }

    public void cloneObject()
    {
        if (transform.gameObject.name.Contains("Clone") || transform.gameObject.name.Contains("Button_"))
        {
            Debug.Log("<color=green> Its already a cloned button so do not CLONE it again   </color>");
            Button button = transform.gameObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { Clicked(); });
        }
        else
        {
            Debug.Log("<color=red> Its original button  </color>");
            GameObject duplicate = Instantiate(transform.gameObject);
            RectTransform rectTransform = duplicate.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            duplicate.transform.SetParent(cloneParentObject.transform);
            //duplicate.transform.position = new Vector3(0, 0, 0);
            duplicate.transform.localPosition = new Vector3(0, 0, 0);
            //duplicate.name = "Button_"+ duplicate.name;
        }

    }
}