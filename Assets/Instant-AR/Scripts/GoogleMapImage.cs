using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GoogleMapImage : MonoBehaviour {
    public GameObject mapPanel;
    public int zoom = 8;
    public string[] locations;
    public Slider zoomSlider;
    public Image MapImage;
    int localZoom;
    float width;
    float height;
    float mapHeight;
    // Use this for initialization
    void Start () {
        if (mapPanel != null)
        {
            mapHeight = mapPanel.transform.parent.GetComponent<RectTransform>().rect.height;
        }
        width = this.gameObject.transform.parent.GetComponent<RectTransform>().rect.width;
         height = this.gameObject.transform.parent.GetComponent<RectTransform>().rect.height;

        if (zoomSlider!=null){
            float sliderWidth = zoomSlider.transform.GetComponent<RectTransform>().rect.width;
            float posX = (width-sliderWidth)/2 - 10;
            float posY = (width / 2 - 30) * -1f;
            if (mapPanel != null){
                posY= (mapHeight / 2 - 30) * -1f;
            }
            zoomSlider.transform.localScale = Vector3.one;
            zoomSlider.transform.localPosition = new Vector3(posX,posY, 0);
            zoomSlider.value = zoom;
        }
        if(MapImage != null){
            MapImage.transform.localScale = Vector3.one;
        }

        GetMapImage();
        localZoom = zoom;

    }
	
	// Update is called once per frame
	void Update () {
        if(localZoom != (int)zoomSlider.value)
        {
            localZoom = (int)zoomSlider.value;
            Debug.Log("<color=green> @@@ Updaing Google Map Image</color>" );
            GetMapImage();
        }
		
	}

    void GetMapImage()
    {
        try
        {
            if (locations == null && locations.Length == 0)
            {
                return;
            }
            //float width = this.gameObject.transform.parent.GetComponent<RectTransform>().rect.width;
            //float height = this.gameObject.transform.parent.GetComponent<RectTransform>().rect.height;

            string baseURL = "https://maps.googleapis.com/maps/api/staticmap?";
            //string center = "37.777081,-121.967522";
            //Debug.Log("<color=green> @@@ locations[0] is </color>" + locations[0]);
            string center = locations[0];
            //int zoom = 8;
            string key = "AIzaSyCYhQrjBnkiAeHmW_IHZNMaXtAkI24qX6k";
            //string size = "1125x1836";
            string size = Convert.ToInt32(width).ToString() + "x" + Convert.ToInt32(height).ToString();
            // Debug.Log("<color=green> @@@ size is </color>" + size);
            string maptype = "roadmap";
            // string markers = "color:blue%7Clabel:9%7C37.777081,-121.967522&markers=color:green%7Clabel:250%7C37.777081,-122.967522&markers=color:red%7Clabel:C%7C37.777081,-123.967522";
            string markers = GetMarkers();
            //Debug.Log("<color=red> @@@ customMarkers string is: </color>" + markers);

            string imagePath = baseURL + "center=" + center + "&zoom=" + (int)zoomSlider.value +
                "&key=" + key +
                "&size=" + size +
                "&maptype=" + maptype +
                 markers
                ;
            Debug.Log("<color=red> @@@ imagePath is: </color>" + imagePath);
            //string imagePath = "https://maps.googleapis.com/maps/api/staticmap?center=37.777081,-121.967522&zoom=6&key=AIzaSyCYhQrjBnkiAeHmW_IHZNMaXtAkI24qX6k";
            // string imagePath = "https://dl.dropbox.com/s/1kg2dbrhacc11qc/tn-target1816---fountain.jpeg";
            Image img = this.gameObject.GetComponent<Image>();
            img.sprite = loadNewSprite(imagePath);
        }
        catch(Exception e){}
    }

        private static Sprite loadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        WWW request = new WWW(FilePath);
        while (!request.isDone) { }
        Texture2D SpriteTexture = request.texture;
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);
        return NewSprite;
    }


    string GetMarkers(){
        string markers = null;
        if (locations != null && locations.Length > 1)
        {
            markers = "&markers=color:red%7Clabel:0%7c"+ locations[0];
            string prefix = "&markers=color:blue%7Clabel:";
            for (int i = 1; i < locations.Length; i++)
            {
                markers = markers +prefix + i + "%7c" + locations[i];
            }
        }
        //string markers = "color:blue%7Clabel:9%7C37.777081,-121.967522&markers=color:green%7Clabel:250%7C37.777081,-122.967522&markers=color:red%7Clabel:C%7C37.777081,-123.967522";
        return markers;
    }
}
