
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapItem : MonoBehaviour
{
    public string ObjectID;
    public Image thumbnail;
    public bool isThumbnailLoaded = false;

    // Use this for initialization
    void Start()
    {

    }

    public void Init(string objectID, string thumbnailURL)
    {
        ObjectID = objectID;
        StartCoroutine(GetThumbnail(thumbnailURL));
        GetComponent<Button>().onClick.AddListener(delegate { TaskOnClick(objectID); });
    }


    private void TaskOnClick(string objectID)
    {
        Debug.Log("<color=red> @@@@ ListItem TaskOnClick,  objectID  : </color>" + objectID);
        GlobalVariables.VUFORIA_UNIQUE_ID = objectID;
        GlobalVariables.LocationMap_CLICKED = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    IEnumerator GetThumbnail(string uri)
    {

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.responseCode);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            Sprite thumbnailSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            thumbnail.sprite = thumbnailSprite;
            isThumbnailLoaded = true;
            SetListItemImage(thumbnailSprite);
        }
    }

    void SetListItemImage(Sprite sprite)
    {
        for (int i = 0; i < ListDataCreator.instance.transform.childCount; i++)
        {
            ListItem listItem = ListDataCreator.instance.transform.GetChild(i).GetComponent<ListItem>();
            if (listItem.ObjectID == ObjectID)
            {
                listItem.thumbnail.sprite = sprite;
            }
        }
    }

}
