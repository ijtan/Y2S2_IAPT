using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandmarkEntry : MonoBehaviour
{
    public string Name;
    public DirectionArrowManager dirArrow;
    public TextMeshProUGUI landmarkNameUI;
    

    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("New Entry Instantiated, name is: " + Name);
        dirArrow = GetComponentInChildren<DirectionArrowManager>();
        landmarkNameUI =  GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GPS.Instance.nearLandmarks.ContainsKey(Name) || !GPS.Instance.landmarkLocations.ContainsKey(Name))
        {
            Debug.LogError("Could not find landmark in GPS!");
            return;
        }
        activated = GPS.Instance.nearLandmarks[Name];
        dirArrow.landmarkLocation = GPS.Instance.landmarkLocations[Name];
        dirArrow.currentLocation = new Vector2(GPS.Instance.lat, GPS.Instance.lon);

        landmarkNameUI.text = Name.Split('/').Last().Split('.').First();
        if (activated)
        {
            dirArrow.activate();
        }
        else
        {
            dirArrow.deactivate();
        }
    }
}
