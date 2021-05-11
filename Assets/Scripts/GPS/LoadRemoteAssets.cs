using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.AddressableAssets;
using UnityEditor;
using System.Linq.Expressions;
using System.Linq;

public class LoadRemoteAssets : MonoBehaviour
{
    [SerializeField] private string _label;

    private GPS GPSServ;
    private Web_Pinger api;
    private UI_GPS_UPDATER GPS_UI;
    private Dictionary<string, bool> isNear = new Dictionary<string, bool>();
    private Dictionary<string, float> distances = new Dictionary<string, float>();

    private Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();

    private float oldLat = 0f;
    private float oldLon = 0f;
    private float lat = 0f;
    private float lon = 0f;

    void Start()
    {
        GPS_UI = FindObjectOfType<UI_GPS_UPDATER>();
        GPSServ = FindObjectOfType<GPS>();
        api = FindObjectOfType<Web_Pinger>();
        Get(_label);
    }

    private void Update()
    {
        lat = GPSServ.lat;
        lon = GPSServ.lon;

        if (oldLat != lat || oldLon != lon)
            Get(_label);

        oldLat = lat;
        oldLon = lon;
    }

    public class isNearResponse
    {
        public float distance;
        public bool near;
    }

    private async Task Get(string label)
    {
        var landmarks = await Addressables.LoadResourceLocationsAsync(label).Task;


        foreach (var landmark in landmarks)
        {
            if (!isNear.ContainsKey(landmark.ToString()))
                isNear[landmark.ToString()] = false;



            //Debug.Log("Got location: " + location);
            //Debug.Log("Got Data: " + location.Data.ToString());
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("loc", landmark.ToString());
            args.Add("uid", SystemInfo.deviceUniqueIdentifier);
            int index = api.counter;
            api.pingAPI("isNear", args);
            int count = 0;
            while (api.responses.Count <= index)
            {
                await Task.Delay(10);
                //Debug.Log("Waiting:" + GPSServ.responses.Count + " index is:" + index);
                count++;
                if (count > 100)
                    break;
                //Debug.Log(GPSServ.responses.Count <= index);
            };
            //Debug.Log("Done Waiting:" + GPSServ.responses.Count+" index is:"+index);
            Debug.Log("Waiting done!");
            string resp = api.responses[index];
            Debug.Log("Got resp:'" + resp + "'");
            isNearResponse nresp = new isNearResponse();
            nresp = JsonUtility.FromJson<isNearResponse>(resp);
            Debug.Log("Got Nearness Response; isNear:" + nresp.near + " distance from landmark: " + nresp.distance);

            distances[landmark.ToString()] = nresp.distance;
            distances.OrderBy(key => key.Value);
            GPS_UI.closest_landmark = distances.First().Value.ToString();
            GPS_UI.isNear = nresp.near.ToString();



            if (nresp.near && isNear[landmark.ToString()] != nresp.near)
            {

                Debug.Log("Instantiating!");
                isNear[landmark.ToString()] = nresp.near;
                GameObject spawn = await Addressables.InstantiateAsync(landmark).Task;
                spawned.Add(landmark.ToString(), spawn);
            }
            else if (!nresp.near && isNear[landmark.ToString()] != nresp.near)
            {
                Debug.Log("removing!");
                Destroy(spawned[landmark.ToString()]);

            }
            else
            {
                Debug.Log("Status not updated!");
            }

            isNear[landmark.ToString()] = nresp.near;

        }

    }


}
