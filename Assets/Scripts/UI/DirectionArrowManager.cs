using System;
using TMPro;
using UnityEngine;

public class DirectionArrowManager : MonoBehaviour
{
    public string distanceText;
    private TextMeshProUGUI distanceTextObject;

    private RectTransform arrow;

    public bool activated = false;
    public float rotateSpeedWhenActive = 30f;

    public Vector2 landmarkLocation;
    public Vector2 currentLocation;
    public double distance;


    public Quaternion offset = Quaternion.Euler(0f, 0f, 90f);
    public Quaternion correctionQuaternion = Quaternion.Euler(0f, 0f, 1f);
    public Gyroscope phoneGyro;

    // Start is called before the first frame update
    void Start()
    {
        phoneGyro = Input.gyro;
        phoneGyro.enabled = true;
        distanceTextObject = GetComponentInChildren<TextMeshProUGUI>();


        foreach (Transform child in this.transform)
        {
            if (child.tag == "DirectionArrowImage")
            {
                arrow = child.gameObject.GetComponent<RectTransform>();
                break;
            }
                
        }
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    // Update is called once per frame
    void Update()
    {

        

        string unit = "m";
        distance = CalculateDistance(currentLocation.x, landmarkLocation.x, currentLocation.y, landmarkLocation.y);
        if (distance > 1000)
        {
            distance /= 1000;
            unit = "km";
        }
        distance = Math.Round(distance, 2);
        distanceText = distance.ToString() + unit;
        distanceTextObject.text = distanceText;

        if (!activated)
        {

            Quaternion gyroQuaternion = GyroToUnity(Input.gyro.attitude);
            // rotate coordinate system 90 degrees. Correction Quaternion has to come first
            Quaternion calculatedRotation = correctionQuaternion * gyroQuaternion*offset;
            //transform.rotation = calculatedRotation;
            float bearing = angleFromCoordinate(currentLocation.x, currentLocation.y, landmarkLocation.x, landmarkLocation.y);

            


            calculatedRotation.z = 0;
            calculatedRotation.w = 0;
            //arrow.rotation = calculatedRotation*Quaternion.Euler(0, 0, Input.compass.magneticHeading + bearing);
            arrow.rotation = calculatedRotation * Quaternion.Euler(0, 0, 
                bearing);
            //arrow.rotation = Quaternion.Slerp(arrow.rotation, Quaternion.Euler(0, 0, Input.compass.magneticHeading + bearing), 100f)* calculatedRotation;
        }

        else
        {
            arrow.rotation *= Quaternion.Euler(0, 0, rotateSpeedWhenActive);
        }
    }


    private float CalculateDistance(float lat_1, float lat_2, float long_1, float long_2)
    {
        int R = 6371;
        var lat_rad_1 = Mathf.Deg2Rad * lat_1;
        var lat_rad_2 = Mathf.Deg2Rad * lat_2;
        var d_lat_rad = Mathf.Deg2Rad * (lat_2 - lat_1);
        var d_long_rad = Mathf.Deg2Rad * (long_2 - long_1);
        var a = Mathf.Pow(Mathf.Sin(d_lat_rad / 2), 2) + (Mathf.Pow(Mathf.Sin(d_long_rad / 2), 2) * Mathf.Cos(lat_rad_1) * Mathf.Cos(lat_rad_2));
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var total_dist = R * c * 1000; // convert to meters
        return total_dist;
    }

    private float angleFromCoordinate(float lat1, float long1, float lat2, float long2)
    {
        lat1 *= Mathf.Deg2Rad;
        lat2 *= Mathf.Deg2Rad;
        long1 *= Mathf.Deg2Rad;
        long2 *= Mathf.Deg2Rad;

        float dLon = (long2 - long1);
        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2);
        float x = (Mathf.Cos(lat1) * Mathf.Sin(lat2)) - (Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(dLon));
        float brng = Mathf.Atan2(y, x);
        brng = Mathf.Rad2Deg * brng;
        brng = (brng + 360) % 360;
        brng = 360 - brng;
        return brng;
    }

    public void activate()
    {
        activated = true;
    }
    public void deactivate()
    {
        activated = false;
    }
}
