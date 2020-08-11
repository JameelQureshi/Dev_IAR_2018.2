
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;

public class TargetImageMapping : MonoBehaviour
{

    public float xSplit;
    public float ySplit;
    public Button previousButton;
    public Button nextButton;
    public Image targetImage;
    public Canvas baseCanvas;
    public Canvas upperCanvas;
    public Canvas stagingCanvas;
    public Canvas lowerCanvas;


    private Vector2 cellSize;
    private int imageCounter = 1;
    private ButtonInfoMAP buttonInfoMAP;
    private string userID;
    private bool isPublic = true;

    // private string liveImagePath = "JITUMOBILETESTINGPFILE";
    private string initialMobileImagePath;


    // Use this for initialization
    void Start()
    {
        //For testing only===
        //TargetImage targetImageObject = ARUtilityTools.getTargetImageFromUID("2df5884b30c04651aa3dac9f56abe1c8");
        //TargetImage targetImageObject = ARUtilityTools.getTargetImageFromUID("6a47c83c5338460096c68ba91064fa3b");
        //ARUtilityTools.initializeGlobalVariables(targetImageObject);
        //// === end=========
        initializeUser();


        if (checkRequiredVariables())
        {

            if (isPublic)
            {
                buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
            }
            else
            {
                buttonInfoMAP = GlobalVariables.buttonInfoMAP;
            }
            fixCanvasSize();
            string imagePath = GlobalVariables.TARGET_IMAGE_DBX_URL;
            constructImage(imagePath);
            //fixCanvasSize();

        }
        else
        {
            Debug.Log("<color=green> ################## ITS A LIVE IMAGE  </color>");
            initialMobileImagePath = GlobalVariables.initialMobileImagePath;
            ARUtilityTools.initializeLimitedVariables();
            if (isPublic)
            {
                buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
            }
            else
            {
                buttonInfoMAP = GlobalVariables.buttonInfoMAP;
            }
            if (!string.IsNullOrEmpty(initialMobileImagePath))
            {
                fixCanvasSize();
                constructImage(initialMobileImagePath);

            }

        }

    }
    bool checkRequiredVariables()
    {
        bool allOK = true;
        if (string.IsNullOrEmpty(GlobalVariables.VUFORIA_UNIQUE_ID) ||
            string.IsNullOrEmpty(GlobalVariables.TARGET_IMAGE_DBX_URL) ||
            string.IsNullOrEmpty(GlobalVariables.INFO_JSON_URL))
        {
            return false;
        }
        return allOK;

    }
    void initializeUser()
    {
        AuthManager _authManager = AuthManager.Instance;
        if (_authManager.IsLoggedIn)
        {
            userID = _authManager.CurrentToken.username;
            userID = userID.Substring(0, userID.LastIndexOf("."));

        }
        else
        {
            userID = SystemInfo.deviceUniqueIdentifier;
        }

    }

    private void fixCanvasSize()
    {
        RectTransform rectTransform = baseCanvas.GetComponent<RectTransform>();
        float base_width = rectTransform.rect.width;
        float base_height = rectTransform.rect.height;
        GameObject innerCanvas = this.gameObject;
        rectTransform = innerCanvas.GetComponent<RectTransform>();
        float inner_width = base_width;
        float inner_height = base_height * 0.65f;
        inner_width = inner_height * GlobalVariables.ASPECT_RATIO; //ASPECT_RATIO is modified in LoadNewSprite method of IMG2Sprite
        if (inner_width > base_width)
        {
            inner_width = inner_width - ((inner_width - base_width) + 40);
            inner_height = inner_width / GlobalVariables.ASPECT_RATIO;
        }
        innerCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(inner_width, inner_height);

        //Adjust stagingCanvas position

        RectTransform rectstagingCanvas = stagingCanvas.GetComponent<RectTransform>();
        float yPos_staging = inner_height / 2 + rectstagingCanvas.rect.height / 2;
        stagingCanvas.transform.localPosition = new Vector3(0, yPos_staging, 0);

        //Adjust upperCanvas position

        RectTransform rectupperCanvas = upperCanvas.GetComponent<RectTransform>();
        float yPos_upper = yPos_staging + rectupperCanvas.rect.height;
        upperCanvas.transform.localPosition = new Vector3(0, yPos_upper, 0);
        upperCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(base_width - 20, rectupperCanvas.rect.height);

        //Adjust lowerCanvas position

        RectTransform rectlowerCanvas = lowerCanvas.GetComponent<RectTransform>();
        float yPos_lower = (inner_height / 2 + rectlowerCanvas.rect.height / 2 + 20) * -1;
        //Debug.Log("<color=red> $$$$$$$$$$  yPos_lower is  </color>" + yPos_lower);
        lowerCanvas.transform.localPosition = new Vector3(0, yPos_lower, 0);


    }



    private void constructImage(string imagePath)
    {
        if (targetImage == null)
        {
            targetImage = GetComponentInChildren<Image>();
        }
        Sprite targetImageSprite = IMG2Sprite.LoadNewSprite(imagePath, 100f, SpriteMeshType.Tight);
        targetImage.sprite = targetImageSprite;
        //attachButtons(targetImage); //To Attach 100(10x10) button on the image
        infoAttachButton(targetImage);


    }

    private void infoAttachButton(Image targetImage)
    {
        if (buttonInfoMAP == null)
        {
            Debug.Log("<color=red> ################## buttonInfoMAP is null, so NO INFO to show  </color>");
            return;
        }

        RectTransform rectTransform = targetImage.GetComponent<RectTransform>();
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        string tempButtonInfo;
        for (int i = 0; i < 100; i++)
        {
            string tempName = "Button_" + i.ToString();
            //Debug.Log("<color=red>>>><<<The temp Button Name is :  </color>" + tempName);
            tempButtonInfo = (string)buttonInfoMAP.GetType().GetField(tempName).GetValue(buttonInfoMAP);
            //Debug.Log("<color=red> ################## buttonInfoMAP  </color>" + buttonInfoMAP.ToString());
            //Debug.Log("<color=red> ################## tempButtonInfo  </color>"+ tempButtonInfo);
            float sizeRatio = 1;
           
            if (!string.IsNullOrEmpty(tempButtonInfo))
            {
                TargetInfo targetInfo = JsonUtility.FromJson<TargetInfo>(tempButtonInfo);
                GameObject buttonPrefab = (GameObject)Resources.Load("InfoButton");
                GameObject newButton = (GameObject)Instantiate(buttonPrefab);
                newButton.name = targetInfo.buttonID;
                newButton.transform.SetParent(targetImage.transform);
                float xFactor = float.Parse(targetInfo.buttonPosition_x);
                float yFactor = float.Parse(targetInfo.buttonPosition_y) * -1;
                //Debug.Log("<color=red>>>><<<The Button Name is :  </color>" + newButton.name);
                // Debug.Log("<color=red>>>><<<The xFactor and yFactor are  :  </color>" + xFactor+" , "+ yFactor);
                newButton.transform.localPosition = new Vector3(-width / 2, height / 2, 0) + new Vector3(width * xFactor, height * yFactor, 0);
                float widPos = (-width / 2) + (width * xFactor);
                //Debug.Log("<color=red>>>><<<The Image Width is :  </color>" + width);
                //Debug.Log("<color=red>>>><<<The Button X Position is :  </color>" + widPos);
                changeSprite(newButton, targetInfo.button_sprite);

                //Change the size of the button based on the sizeRatio set from the web page
                if (!string.IsNullOrEmpty(targetInfo.sizeRatio))
                {
                    sizeRatio = float.Parse(targetInfo.sizeRatio);
                    sizeRatio = sizeRatio / 100;
                }
                newButton.transform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);

            }

        }

    }
    public void changeSprite(GameObject go, string spritePath)
    {

        Button thisButton = go.GetComponent<Button>();
        Texture2D texture;
        texture = Resources.Load<Texture2D>(spritePath);
        if (texture == null)
        {
            texture = Resources.Load<Texture2D>("JituSprites/Button-Info-icon");
        }
        Rect rect = new Rect();
        rect.center = new Vector2(0, 0);
        rect.height = texture.height;
        rect.width = texture.width;
        Sprite tempSprite = UnityEngine.Sprite.Create(texture, rect, new Vector2(1, 1), 100f);
        thisButton.GetComponent<Image>().sprite = tempSprite;
    }


    private string getTargetImage()
    {
        string imagePath = "";
        if (GlobalVariables.TARGET_IMAGE_DBX_URL != null)
        {
            imagePath = GlobalVariables.TARGET_IMAGE_DBX_URL;
            Debug.Log("<color=red>>>><<< Got the image from GlobalVariables  </color>");
        }
        if (string.IsNullOrEmpty(imagePath))
        {
            imagePath = ARUtilityTools.getLatestImage(imageCounter); //Only for testing
            Debug.Log("<color=red>>>><<< GlobalVariables was NULL, so getting image from DB  </color>");
        }
        return imagePath;
    }

    private float getAspect()
    {
        float aspect = 1.0f;
        return aspect;
    }

    public void CloseButton()
    {
        if (isPublic)
        {
            buttonInfoMAP = GlobalVariables.buttonPublicInfoMAP;
        }
        else
        {
            buttonInfoMAP = GlobalVariables.buttonInfoMAP;
        }
        if (buttonInfoMAP != null)
        {
            string filePath = "";
            string publicFilePath = "";
            if (GlobalVariables.VUFORIA_UNIQUE_ID != null)
            {
                filePath = GlobalVariables.DROPBOX_EXPERIENCE_FOLDER_INFO + userID + "/" + GlobalVariables.VUFORIA_UNIQUE_ID + ".json";
                publicFilePath = GlobalVariables.DROPBOX_EXPERIENCE_FOLDER_INFO + "public/" + GlobalVariables.VUFORIA_UNIQUE_ID + ".json";
                StartCoroutine(uploadTargetImageJson(JsonUtility.ToJson(buttonInfoMAP), filePath));
                StartCoroutine(uploadTargetImageJson(JsonUtility.ToJson(buttonInfoMAP), publicFilePath));
                JituMessageBox.DisplayMessageBox("Info Status", "Info Loaded Successfully!", true, null);
            }
            else
            {
                // Initial Image Upload case
                if (!string.IsNullOrEmpty(initialMobileImagePath))
                {
                    string jsonFileName = initialMobileImagePath.Substring(initialMobileImagePath.LastIndexOf("/") + 1);
                    jsonFileName = jsonFileName + ".json";
                    filePath = GlobalVariables.DROPBOX_STAGING_FOLDER + userID + "/" + jsonFileName;
                    StartCoroutine(uploadTargetImageJson(JsonUtility.ToJson(buttonInfoMAP), filePath));
                    JituMessageBox.DisplayMessageBox("Info Status", "Initial Info Data Loaded Successfully!", true, null);

                }
            }

            //Debug.Log("<color=red> $$$$$$$$$$$$$$$  The filePath  is :  </color>" + filePath);

        }
        else
        {
            Debug.Log("<color=red> $$$$$$$$$$$$$$$ In CloseButton, buttonInfoMAP was NULL :  </color>");
        }
        Destroy(this.gameObject);
        GlobalVariables.INFO_PANEL_BUTTON_CLICKED = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("3-CloudReco");


    }


    public void NextButton()     {         if (imageCounter > 1)         {             imageCounter = imageCounter - 1;         }

        string imagePath = getTargetImage();
        //string imagePath = "file:///Users/jitenderhooda/Downloads/IMG_5408.PNG";
        constructImage(imagePath);
      }
    public void PrevButton()     {         Debug.Log("<color=red>>>><<<I AM INSIDE  PrevButton  </color>");         imageCounter = imageCounter + 1;
        //string imagePath = getTargetImage();
        string imagePath = "file:///Users/jitenderhooda/Downloads/IMG_5408.PNG";
        Debug.Log("<color=red>>>><<<imagePath in PrevButton  </color>" + imagePath);
        Debug.Log("<color=red>>>><<<imageCounter in PrevButton  </color>" + imageCounter);
        //string imagePath = "file:///Users/jitenderhooda/Downloads/IMG_5408.PNG";
        constructImage(imagePath);

    }

    private IEnumerator uploadTargetImageJson(string newTargetImage, string filepath)
    {
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        string param = "{\"path\": \"" + filepath + "\",\"mode\": \"overwrite\",\"autorename\": false,\"mute\": false}";
        postHeader.Add("Authorization", GlobalVariables.DROPBOX_TOCKEN);
        postHeader.Add("Dropbox-API-Arg", param);
        postHeader.Add("Content-Type", "application/octet-stream");
        byte[] myData = Encoding.ASCII.GetBytes(newTargetImage);
        //byte[] myData = File.ReadAllBytes(sourcePath);
        WWW www = new WWW("https://content.dropboxapi.com/2/files/upload", myData, postHeader);
        yield return www;
    }






}
