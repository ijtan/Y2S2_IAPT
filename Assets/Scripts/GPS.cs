using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class GPS : MonoBehaviour
{
    public float lat;
    public float lon;
    public float UPDATE_TIME = 1f;
    public string host = "http://192.168.0.7";
    public string port = "5000";
    //public List<string> vicinity;

    //int index = 0;
    public List<string> responses;

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

        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocServ());
        StartCoroutine(updateGPS());
    }

    private IEnumerator StartLocServ()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location (GPS) not enabled by user!");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
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
                pingAPI("getNear", args);
            }

            lon = newlon;
            lat = newlat;


            yield return updateTime;
        }
    }

    public void pingAPI(string page = "", Dictionary<string, string> args = null)
    {


        string urlToPing = host + ':' + port + '/' + page;
        if (args != null)
        {

            bool isfirst = true;
            foreach (KeyValuePair<string, string> arg in args)
            {
                if (isfirst)
                {
                    urlToPing += '?';
                    isfirst = false;

                }

                else
                    urlToPing += '&';

                urlToPing += arg.Key;
                urlToPing += '=';
                urlToPing += arg.Value;

            }
        }
        Debug.Log("Pinging: " + urlToPing);
        StartCoroutine(GetRequest(urlToPing));
    }

    IEnumerator GetRequest(string uri)
    {
        string response = "";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    response = webRequest.downloadHandler.text;
                    Debug.Log(pages[page] + ":\nReceived: " + response);
                    break;
            }
        }
        responses.Add(response);
    }


    //// Update is called once per frame
    //void Update()
    //{

    //}
}
