using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

public class MapPointsPlacement : MonoBehaviour {

    [SerializeField]
    [Geocode]
    string[] _TrackPartsLatitudeLongitude;
   

    public Vector2d[] _locations;
    [SerializeField]
    AbstractMap _map;
    public int _spawnScale = 1;
    public static List<GameObject> _spawnedObjects;
    Vector2d[] _coordinates;
    bool instatiatedmap = false;
    public GameObject CheckpointIndicator;
    public Location currentLocation;

    public static MapPointsPlacement instance;
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
    public void PlacePoints(LocationData locationData)
    {
        _locations = new Vector2d[locationData.objectLocations.Count];
        _spawnedObjects = new List<GameObject>();

        int count = 0;
        Debug.Log(locationData.objectLocations.Count);
        for (int i = 0; i < locationData.objectLocations.Count; i++)
        {

            try
            {
                string[] location = locationData.objectLocations[i].location.Split(',');
               // if (DistanceCalculator.IsPointInTheRange(currentLocation.LatitudeLongitude.x,
               //     currentLocation.LatitudeLongitude.y, double.Parse(location[0]), double.Parse(location[1]), LocationDataManager.Radius))
               // {

                    _locations[count] = Conversions.StringToLatLon(locationData.objectLocations[i].location);
                    var mapPoint = Instantiate(CheckpointIndicator);
                    mapPoint.transform.position = _map.GeoToWorldPosition(_locations[count], false);
                    mapPoint.GetComponent<MapItem>().Init(locationData.objectLocations[i].objectID, locationData.objectLocations[i].thumbnailURL);
                    Debug.LogWarning(_map.GeoToWorldPosition(_locations[count], true));

                    _spawnedObjects.Add(mapPoint);
                    count++;
                //}
            }
            catch (Exception) { continue; }
        }
        instatiatedmap = true;

    }


    private void LateUpdate()
    {
        if (instatiatedmap)
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }
    }


}
