using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkListManager : MonoBehaviour
{
    // Start is called before the first frame update
    Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();
    public GameObject entryPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var lm in GPS.Instance.nearLandmarks.Keys)
        {
            if (!buttons.ContainsKey(lm))
            {
                GameObject newLM = Instantiate(entryPrefab);
                newLM.transform.SetParent(this.transform,false);
                //newLM.transform.parent = this.transform;
                newLM.GetComponent<LandmarkEntry>().Name = lm;
                buttons.Add(lm, newLM); 
            }


        }
    }
}
