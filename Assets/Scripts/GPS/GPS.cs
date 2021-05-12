using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.UI;

public class landmark_info
{
    //Vector2 location = new Vector2();
    public bool near = false;
    public float locX = 0;
    public float locY = 0;
    public float distance = 0;
    public string title = "";
    public string description = "";
    public string image_url = "";
}

public class landmark_list
{
    public string[] landmarks;
}

public class GPS : MonoBehaviour
{
    [SerializeField] private string _label = "remote";
    public float lat;
    public float lon;
    public float UPDATE_TIME = 1f;
    public Text txt;

    private Web_Pinger api;

    public float closest;

    //public Dictionary<string, bool> nearLandmarks = new Dictionary<string, bool>();
    //public Dictionary<string, Vector2> landmarkLocations = new Dictionary<string, Vector2>();
    public Dictionary<string, landmark_info> landmarks_data = new Dictionary<string, landmark_info>();

    //private Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();


    public static GPS Instance { set; get; }
    // Start is called before the first frame update
    void Start()
    {
        api = FindObjectOfType<Web_Pinger>();
        //updateLandmarksFromApi();


#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
#endif

        Debug.Log("Starting GPS!");
        StartCoroutine(StartLocServ());

        //showToast("GPS Object start", 2);
        

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Debug.Log("Starting Gps Service");
        //showToast("Starting Gps Service", 2);


    }

    public IEnumerator StartLocServ()
    {
        if (!Input.location.isEnabledByUser)
        {
            showToast("GPS not enabled by user! ", 2);
            Debug.LogError("Location (GPS) not enabled by user!");
            yield return new WaitForSeconds(3);
        }
        //showToast("Pre Gps Start", 2);
        Input.location.Start();


        int maxWait = 20;
        //showToast("Starting GPS Wait: ", 2);
        Debug.Log("Pre wait" + Input.location.status.ToString());
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("Waiting: " + Input.location.status.ToString());
            //showToast(("Waiting... " + maxWait.ToString() + " st:" + Input.location.status.ToString()), 1);
            showToast(("Waiting for GPS... "),1);
            maxWait--;
            yield return new WaitForSeconds(1f);
        }

        if (maxWait <= 0)
        {
            Debug.LogError("GPS timed out!");
            showToast(("GPS Timed out" + maxWait.ToString() + " st:" + Input.location.status.ToString()), 2);
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location!");
            showToast("Unable to determine device location!", 2);
            yield break;
        }
        else
        {
            //showToast(("Done! st=" + Input.location.status.ToString()), 2);
            //showToast(("Done! mw=" + maxWait.ToString()), 2);
            showToast(("GPS Connected"), 1);
            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;
            StartCoroutine(updateGPS());
            yield break;
        }

        showToast(("out of everything!! st=" + Input.location.status.ToString() + " mw = " + maxWait.ToString()), 2);

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
            Debug.Log("in update: User has not enabled GPS");
            yield return new WaitForSeconds(3);
        }
        updateLandmarksFromApi();

        showToast("Updating GPS Location...", 1);
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);
        while (true)
        {

            var newlon = Input.location.lastData.longitude;
            var newlat = Input.location.lastData.latitude;

            if (newlon != lon || newlat != lat)
            {
                showToast("Pos change!!", 2);
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("lat", newlat.ToString());
                args.Add("lon", newlon.ToString());
                args.Add("uid", SystemInfo.deviceUniqueIdentifier);
                api.pingAPI("getNear", args);

                updateLandmarksFromApi();
            }

            lon = newlon;
            lat = newlat;
            yield return updateTime;
        }
    }

    private async Task updateLandmarksFromApi()
    {
        showToast("Updating Landmarks from API!", 2);
        Debug.Log("Updating Landmarks From Api Start!");
        //var landmarks = await Addressables.LoadResourceLocationsAsync(_label).Task;
  


        int index = api.counter;
        api.pingAPI("getKeys", new Dictionary<string, string>());


        int count = 0;
        while (api.responses.Count <= index)
        {
            await Task.Delay(10);

            count++;
            if (count > 100)
            {
                Debug.LogError("Key Fetching Ping Timed out!");
                break;
            }


        };
        string[] landmarks;
        Debug.Log("Got resource locations!");
        string resp = api.responses[index];
        landmarks = JsonUtility.FromJson<landmark_list>(resp).landmarks;
        Debug.Log("Got entries:" + landmarks.ToString());
        //showToast(("Got " + landmarks.Length + " Entries!"), 2);




        foreach (var landmark in landmarks)
        {

            Debug.Log("Pinging for: " + landmark.ToString());


            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("loc", landmark.ToString());
            args.Add("uid", SystemInfo.deviceUniqueIdentifier);
            index = api.counter;
            api.pingAPI("isNear", args);


            count = 0;
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

            resp = api.responses[index];
            Debug.Log("Got resp:'" + resp + "'");
            landmark_info nresp = new landmark_info();
            nresp = JsonUtility.FromJson<landmark_info>(resp);
            Debug.Log("Parsed; isNear:" + nresp.near + " landmark loc: (" + nresp.locX + "," + nresp.locY + ") distance: " + nresp.distance);
            landmarks_data[landmark.ToString()] = nresp;

            foreach(var lm_data in landmarks_data)
            {
                if (!landmarks.Contains<string>(lm_data.Key))
                {
                    landmarks_data.Remove(lm_data.Key);
                }
            }

            //if (!landmarkLocations.ContainsKey(landmark.ToString()))
            //    landmarkLocations[landmark.ToString()] = new Vector2(0, 0);
            //landmarkLocations[landmark.ToString()] = new Vector2(nresp.locX, nresp.locY);
            //nearLandmarks[landmark.ToString()] = nresp.near;

        }
    }





    void showToast(string text,
        int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = txt.color;

        txt.text = text;
        txt.enabled = true;

        //Fade in
        yield return fadeInAndOut(txt, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(txt, false, 0.5f);

        txt.enabled = false;
        txt.color = orginalColor;
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }

}






