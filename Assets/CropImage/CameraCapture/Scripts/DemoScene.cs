using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ImageCropperNamespace;

public class DemoScene : MonoBehaviour {

	public CameraCapture CamCap;
	public Image pickPreiveimage;

    public Text pathText;

    public GameObject PhotoPanel;

    public int m_Mode;

    private void Start()
	{
		if (CamCap == null) {
			CamCap = GameObject.FindObjectOfType<CameraCapture> ();
		}

		this.CamCap.TakePhotoCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.PickCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.Failed += new CameraCapture.ErrorDelegate(this.ErrorInfo);

        m_Mode = 0;
	}

	public void takePhoto()
	{
        m_Mode = 1;
		this.CamCap.takePhoto();
	}

	public void pickPhoto()
	{
        m_Mode = 2;
		this.CamCap.pickPhoto();
	}

	private void Completetd(string patha)
	{
		pathText.text = pathText.text + "\n" + patha;
        base.StartCoroutine(this.LoadImage(patha));
	}

	private void ErrorInfo(string errorInfo)
	{
		pathText.text = pathText.text + "\n<color=#ff0000>" + errorInfo +"</color>";
	}

	IEnumerator LoadImage(string path)
	{
		var url = "file://" + path;
		#if UNITY_EDITOR || UNITY_STANDLONE
		url = "file:/"+path;
		#endif
		Debug.Log ("current path is " + url);
		var www = new WWW(url);
		yield return www;

		var texture = www.texture;
		if (texture == null)
		{
			Debug.LogError("Failed to load texture url:" + url);
		}

        //DestroyImmediate(pickPreiveimage.texture);
        //pickPreiveimage.texture = texture;

        pickPreiveimage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one * 0.5f);
        pickPreiveimage.preserveAspect = true;

        

        PhotoPanel.SetActive(false);
        FindObjectOfType<ImageCropperDemo>().Crop(texture);
        texture = null;
    }
}
