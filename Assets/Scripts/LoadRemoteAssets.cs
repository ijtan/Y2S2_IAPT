using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadRemoteAssets : MonoBehaviour
{
    [SerializeField] private string _label;
    private GPS GPSServ;
    void Start()
    {
        GPSServ = FindObjectOfType<GPS>();
        Get(_label);
    }

    private async Task Get(string label)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;


        foreach (var location in locations)
        {

            //Debug.Log("Got location: " + location);
            //Debug.Log("Got Data: " + location.Data.ToString());
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("loc", location.ToString());

            int index = GPSServ.responses.Count;
            GPSServ.pingAPI("isValid", args);
            int count = 0;
            while (GPSServ.responses.Count <= index)
            {
                await Task.Delay(100);
                //Debug.Log("Waiting:" + GPSServ.responses.Count + " index is:" + index);
                count++;
                if (count > 10)
                    break;
                //Debug.Log(GPSServ.responses.Count <= index);
            };
            //Debug.Log("Done Waiting:" + GPSServ.responses.Count+" index is:"+index);
            Debug.Log("Waiting done!");
            string resp = GPSServ.responses[index].Trim(' ','\n');
            Debug.Log("Got resp:'" + resp+"'\t ==?"+(resp=="true").ToString());
            

            if (resp == "true")
            {
                Debug.Log("Instantiating!");
                await Addressables.InstantiateAsync(location).Task;
            }
            else
            {
                Debug.Log("not Instantiating!");
            }

        }
    }

}
