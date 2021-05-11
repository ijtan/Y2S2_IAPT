using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.Networking;

public class GPS : MonoBehaviour
{
    [SerializeField] private string _label = "remote";
    public float lat;
    public float lon;
    public float UPDATE_TIME = 1f;

    private Web_Pinger api;

    public float closest;

    public Dictionary<string, bool> nearLandmarks = new Dictionary<string, bool>();
    public Dictionary<string, Vector2> landmarkLocations = new Dictionary<string, Vector2>();

    private Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();


    public static GPS Instance { set; get; }
    // Start is called before the first frame update
    void Start()
    {


#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
#endif

        Debug.Log("Starting GPS!");
        api = FindObjectOfType<Web_Pinger>();

        Instance = this;
        DontDestroyOnLoad(gameObject);
        updateLandmarksFromApi();
        StartCoroutine(StartLocServ());
        StartCoroutine(updateGPS());
        
    }

    private IEnumerator StartLocServ()
    {
        //if (!Input.location.isEnabledByUser)
        //{
        //    Debug.LogError("Location (GPS) not enabled by user!");
        //    yield break;
        //}

        Input.location.Start();
        int maxWait = 40;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(0.5f);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.LogError("GPS timed out!");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location!");
            yield break;
        }

        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;

        yield break;

    }

    public class landmarkPingResponse
    {
        //Vector2 location = new Vector2();
        public bool near = false;
        public float locX;
        public float locY;
        public float distance;

    }

    //    int index = api.counter;
    //    api.pingAPI("isNear", args);
    //            int count = 0;
    //            while (api.responses.Count <= index)
    //            {
    //                await Task.Delay(10);
    //    //Debug.Log("Waiting:" + GPSServ.responses.Count + " index is:" + index);
    //    count++;
    //                if (count > 100)
    //                    break;
    //                //Debug.Log(GPSServ.responses.Count <= index);
    //            };
    ////Debug.Log("Done Waiting:" + GPSServ.responses.Count+" index is:"+index);
    //Debug.Log("Waiting done!");
    //            string resp = api.responses[index];
    //Debug.Log("Got resp:'" + resp + "'");
    //            isNearResponse nresp = new isNearResponse();
    //nresp = JsonUtility.FromJson<isNearResponse>(resp);
    //            Debug.Log("Got Nearness Response; isNear:" + nresp.near + " distance from landmark: " + nresp.distance);

    //            distances[landmark.ToString()] = nresp.distance;
    //            distances.OrderBy(key => key.Value);
    //            GPS_UI.closest_landmark = distances.First().Value.ToString();
    //            GPS_UI.isNear = nresp.near.ToString();

    public IEnumerator updateGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            yield break;
        }


        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);
        while (true)
        {

            var newlon = Input.location.lastData.longitude;
            var newlat = Input.location.lastData.latitude;

            if (newlon != lon || newlat != lat)
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("lat", newlat.ToString());
                args.Add("lon", newlon.ToString());
                args.Add("uid", SystemInfo.deviceUniqueIdentifier);
                api.pingAPI("getNear", args);

                updateLandmarksFromApi();
            }
        }
    }

    private async Task updateLandmarksFromApi()
    {
        Debug.Log("Updating Landmarks From Api Start!");
        var landmarks = await Addressables.LoadResourceLocationsAsync(_label).Task;
        Debug.Log("Got resource locations!");


        foreach (var landmark in landmarks)
        {
            if (!nearLandmarks.ContainsKey(landmark.ToString()))
                nearLandmarks[landmark.ToString()] = false;
            if (!landmarkLocations.ContainsKey(landmark.ToString()))
                landmarkLocations[landmark.ToString()] = new Vector2(0, 0);
            Debug.Log("Pinging for: "+ landmark.ToString());


            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("loc", landmark.ToString());
            args.Add("uid", SystemInfo.deviceUniqueIdentifier);
            int index = api.counter;
            api.pingAPI("isNear", args);


            int count = 0;
            while (api.responses.Count <= index)
            {
                await Task.Delay(10);

                count++;
                if (count > 100)
                {
                    Debug.LogError("GPS Nearity Ping Timed out!");
                    break;
                }


            };

            string resp = api.responses[index];
            Debug.Log("Got resp:'" + resp + "'");
            landmarkPingResponse nresp = new landmarkPingResponse();
            nresp = JsonUtility.FromJson<landmarkPingResponse>(resp);
            Debug.Log("Parsed; isNear:" + nresp.near + " landmark loc: (" + nresp.locX + "," + nresp.locY + ") distance: " + nresp.distance);


            landmarkLocations[landmark.ToString()] = new Vector2(nresp.locX, nresp.locY);
            nearLandmarks[landmark.ToString()] = nresp.near;

        }

    }
}



