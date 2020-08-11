using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocationDataManager : MonoBehaviour
{
    public string JsonURL = "";
    public static LocationData locationData;
    // Start is called before the first frame update
    public static int Radius
    {
        set
        {
            PlayerPrefs.SetInt("Radius", value);
        }
        get
        {
            return PlayerPrefs.GetInt("Radius", 5);
        }
    }


    void Start()
    {
        string getLocatioApiURL = GlobalVariables.REST_SERVER + "getlocation/";
        string userName = GlobalVariables.CURRENT_USER;
        userName = userName.Substring(0, userName.LastIndexOf("."));

        Debug.Log("<color=red> @@@@########$$$$$$$$$ userName  : </color>" + userName);

        JsonURL = getLocatioApiURL + userName;
        Debug.Log("<color=red> @@@@########$$$$$$$$$ JsonURL  : </color>" + JsonURL);

        StartCoroutine(DownloadLocationData());
        LocationProviderFactory.Instance.DeviceLocationProvider.OnLocationUpdated += OnUpdateLocationCalled;
        
    }

    private void OnUpdateLocationCalled(Location location)
    {
        if (locationData!=null)
        {
            //Debug.Log(locationData.objectLocations.Count);
            MapPointsPlacement.instance.PlacePoints(locationData);
            ListDataCreator.instance.Populate(locationData);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        LocationProviderFactory.Instance.DeviceLocationProvider.OnLocationUpdated -= OnUpdateLocationCalled;
    }

    IEnumerator DownloadLocationData()
    {
        UnityWebRequest www = UnityWebRequest.Get(JsonURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {

            locationData = JsonUtility.FromJson<LocationData>(www.downloadHandler.text);
            Debug.Log("<color=red> @@@@########$$$$$$$$$ www.downloadHandler.text  : </color>" + www.downloadHandler.text);
            if (locationData.objectLocations != null && locationData.objectLocations.Count > 0)
            {
                Debug.Log("<color=red> @@@@########$$$$$$$$$ locationData : </color>" + locationData.objectLocations[0].location);
                Debug.Log("<color=red> @@@@########$$$$$$$$$ locationData : </color>" + locationData.objectLocations[0].objectID);
                Debug.Log("<color=red> @@@@########$$$$$$$$$ locationData : </color>" + locationData.objectLocations[0].description);
                Debug.Log("<color=red> @@@@########$$$$$$$$$ locationData : </color>" + locationData.objectLocations[0].thumbnailURL);
                Debug.Log("<color=red> @@@@########$$$$$$$$$ locationData : </color>" + locationData.userID);
            }
            //Debug.Log(locationData.ObjectLocations.Count);
            //ListDataCreator.instance.Populate(locationData);

        }
    }

  

}


[Serializable]
public class LocationData
{
    public string userID;
    public List<ObjectLocation> objectLocations;
}

[Serializable]
public class ObjectLocation
{
    public string objectID;
    public string thumbnailURL;
    public string description;
    public string location;

}
