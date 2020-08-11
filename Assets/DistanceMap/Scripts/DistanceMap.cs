using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DistanceMap : MonoBehaviour 
{

    public Color TargetCircleColor;
    public Color MainCircleColor;
    public Color LineColor;
    public string JSON_String_URL;
    public string[] TargetImagesURL;
    public Dictionary<int, Texture2D> TargetCircleImages_URL;
    public string MainCircleImage_URL;
    public float MainCircleRadius;
    public float TargetCircleRadius;
    public float LineThickness; 

    void awake()
    {
        for (int i = 0; i < TargetImagesURL.Length; i++)
        {
            StartCoroutine(DownloadImage(TargetImagesURL[i], i));
        }
    }

    IEnumerator DownloadImage(string MediaUrl, int index)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            TargetCircleImages_URL.Add(index, ((DownloadHandlerTexture)request.downloadHandler).texture);

        }
    }

}
