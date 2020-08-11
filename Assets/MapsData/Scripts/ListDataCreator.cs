using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using UnityEngine;
using UnityEngine.UI;

public class ListDataCreator : MonoBehaviour {

    public GameObject prefab;
    public GameObject canvas;
    private Location currentLocation;
    public static ListDataCreator instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RePopulate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //foreach (GameObject obj in MapPointsPlacement._spawnedObjects)
        //{
        //    Destroy(obj);
        //}
        //MapPointsPlacement._spawnedObjects.Clear();
        Populate(LocationDataManager.locationData);
    }

    public void Populate(LocationData locationData)
    {
        GameObject newObj; // Create GameObject instance
        currentLocation = LocationProviderFactory.Instance.DeviceLocationProvider.CurrentLocation;

        //MapPointsPlacement.instance.currentLocation = currentLocation;
        //MapPointsPlacement.instance.PlacePoints(locationData);
        if (locationData.objectLocations != null & locationData.objectLocations.Count > 0)
        {
            for (int i = 0; i < locationData.objectLocations.Count; i++)
            {
                string[] location = locationData.objectLocations[i].location.Split(',');
                try
                {
                    if (DistanceCalculator.IsPointInTheRange(currentLocation.LatitudeLongitude.x,
                    currentLocation.LatitudeLongitude.y, double.Parse(location[0]), double.Parse(location[1]), LocationDataManager.Radius))
                    {
                        newObj = Instantiate(prefab, transform);
                        newObj.GetComponent<ListItem>().Init(locationData.objectLocations[i].thumbnailURL,
                                                              locationData.objectLocations[i].description,
                                                              locationData.objectLocations[i].objectID,
                                                              double.Parse(location[0]),
                                                              double.Parse(location[1]));
                    }

                }
                catch (Exception) { continue; }

            }
        }

        float width = canvas.GetComponent<RectTransform>().rect.width;


        Vector2 newSize = new Vector2(width,300);
        GetComponent<GridLayoutGroup>().cellSize = newSize;

    }

    



}
