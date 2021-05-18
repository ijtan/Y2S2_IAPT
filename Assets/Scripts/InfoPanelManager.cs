using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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
    private void Start()
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
        //newPanelInfo.image_urls = new string[1];
        newPanelInfo.image_urls = landmark.image_urls;

        Vector3 camPos = Camera.main.transform.position;

        newPanel.transform.SetParent(this.transform, true);
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
        spawned.Add(id, newPanel);
    }

    public void resetPositions()
    {
        foreach(GameObject g in spawned.Values)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3f + Camera.main.transform.up * 0.25f;
            transform.rotation = new Quaternion(0.0f, Camera.main.transform.rotation.y, 0.0f, Camera.main.transform.rotation.w);
        }
    }

}