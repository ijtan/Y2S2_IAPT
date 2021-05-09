using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.AddressableAssets;
using UnityEditor;
using System.Linq.Expressions;

public class LoadRemoteAssets : MonoBehaviour
{
    [SerializeField] private string _label;

    private GPS GPSServ;
    private Dictionary<string, bool> isNear = new Dictionary<string, bool>();
    private Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();

    [SerializeField] private float oldLat = 0f;
    [SerializeField] private float oldLon = 0f;

    void Start()
    {

        GPSServ = FindObjectOfType<GPS>();
        Get(_label);
    }

    private void Update()
    {
        float lat = GPSServ.lat;
        float lon = GPSServ.lon;

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
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;


        foreach (var location in locations)
        {
            if (!isNear.ContainsKey(location.ToString()))            
                isNear[location.ToString()] = false;



            //Debug.Log("Got location: " + location);
            //Debug.Log("Got Data: " + location.Data.ToString());
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("loc", location.ToString());
            args.Add("uid", SystemInfo.deviceUniqueIdentifier);
            int index = GPSServ.responses.Count;
            GPSServ.pingAPI("isNear", args);
            int count = 0;
            while (GPSServ.responses.Count <= index)
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
            string resp = GPSServ.responses[index].Trim(' ', '\n');
            Debug.Log("Got resp:'" + resp + "'\t ==?" + (resp == "true").ToString());

            isNearResponse nresp = JsonUtility.FromJson<isNearResponse>(resp);
            Debug.Log("Got Nearness Response; isNear:" + nresp.near + " distance from landmark: " + nresp.distance);


            if (nresp.near && isNear[location.ToString()] != nresp.near)
            {
                
                Debug.Log("Instantiating!");
                isNear[location.ToString()] = nresp.near;
                GameObject spawn = await Addressables.InstantiateAsync(location).Task;
                spawned.Add(location.ToString(), spawn);
            }
            else if (!nresp.near && isNear[location.ToString()] != nresp.near)
            {
                Debug.Log("removing!");
                Destroy(spawned[location.ToString()]);

            }
            else
            {
                Debug.Log("Status not updated!");
            }

            isNear[location.ToString()] = nresp.near;

        }
    }


}
