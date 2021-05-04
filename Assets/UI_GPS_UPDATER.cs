using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GPS_UPDATER : MonoBehaviour
{
    private GPS GPSService;
    private string lat,lon;
    public Text coords;
    // Start is called before the first frame update
    void Start()
    {
        GPSService = FindObjectOfType<GPS>();
        lat = GPS.Instance.lat.ToString();
        lon = GPS.Instance.lon.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        lat = GPS.Instance.lat.ToString();
        lon = GPS.Instance.lat.ToString();
        coords.text = "Lat: " + lat + "\nLon:" + lon;
    }
}
