using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class information_panel_details
{
    public string title;
    public string[] image_urls;
    public string description;
    public float x;
    public float y;
    public float z;
}
public class InfoPanelManager : MonoBehaviour
{
    public GameObject InfoPanelPrefab;
    public string id;
    public Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        consumeRetainer();
        spawnFromRetainer();
        //spawnNewInfo(id);
    }
    private landmark_info landmark;
    public void consumeRetainer()
    {
        var ir = FindObjectOfType<InformationRetainer>();
        if (ir == null)
        {
            Debug.LogError("Could not find Info retainer!!");
            return;
        }
        landmark = ir.landmarkObject;
        id = ir.id;
        Destroy(ir.gameObject);
    }

    public void spawnFromRetainer()
    {
        GameObject newPanel = Instantiate(InfoPanelPrefab);
        InfoPanel newPanelInfo = newPanel.GetComponent<InfoPanel>();

        newPanelInfo.title = landmark.title;
        newPanelInfo.description = landmark.long_description;
        newPanelInfo.image_urls = new string[1];
        newPanelInfo.image_urls[0] = landmark.image_url;

        Vector3 camPos = Camera.main.transform.position;
        
        newPanel.transform.SetParent(this.transform, false);
        newPanel.transform.position = new Vector3(camPos.x + 2, camPos.y, camPos.z);
        //newPanel.transform.position = new Vector3(landmark.x, landmark.y, landmark.z);

        spawned.Add(id, newPanel);
    }

    public async Task spawnNewInfoFromApi(string id)
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        //args.Add("lat", newlat.ToString());
        //args.Add("lon", newlon.ToString());
        args.Add("landmark_id", id);
        args.Add("uid", SystemInfo.deviceUniqueIdentifier);


        int index = Web_Pinger.Instance.counter;
        Web_Pinger.Instance.pingAPI("getInfo", args);


        int count = 0;
        while (Web_Pinger.Instance.responses.Count <= index)
        {
            await Task.Delay(10);

            count++;
            if (count > 100)
            {
                Debug.LogError("Info fetching Timed out!");
                break;
            }


        };

        string resp = Web_Pinger.Instance.responses[index];
        information_panel_details ipd = JsonUtility.FromJson<information_panel_details>(resp);
        GameObject newPanel = Instantiate(InfoPanelPrefab);
        InfoPanel newPanelInfo = newPanel.GetComponent<InfoPanel>();

        newPanelInfo.title = ipd.title;
        newPanelInfo.description = ipd.title;
        newPanelInfo.image_urls = ipd.image_urls;
        newPanel.transform.SetParent(this.transform, false);
        newPanel.transform.position = new Vector3(ipd.x, ipd.y, ipd.z);

        spawned.Add(id, newPanel);
    }


    // Update is called once per frame
    //void Update()
    //{

    //}
}
