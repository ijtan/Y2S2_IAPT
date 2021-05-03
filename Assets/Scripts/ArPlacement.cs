using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArPlacement : MonoBehaviour
{
    public GameObject toSpawn;
    public GameObject indicator;

    private GameObject spawnedObj;
    private Pose pose;
    private ARRaycastManager rayman;
    bool  placementIsValid = false;

    // Start is called before the first frame update
    void Start()
    {
        rayman = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(placementIsValid&&Input.touchCount>0 && Input.GetTouch(0).phase==TouchPhase.Began)
            placeObject();

        updatePose();
        updateIndicator();
    }

    private void updatePose()
    {
        Vector3 midScreen = Camera.current.ViewportToScreenPoint(new Vector3(.5f, .5f));
        var hits = new List<ARRaycastHit>();
        rayman.Raycast(midScreen, hits, TrackableType.Planes);
        placementIsValid = hits.Count > 0;

        if (placementIsValid)
            pose = hits[0].pose;

        //indicator.transform.position
    }

    public void placeObject()
    {
        spawnedObj = Instantiate(toSpawn, pose.position, pose.rotation);
    }

    private void updateIndicator()
    {
        if(placementIsValid)
        {
            indicator.SetActive(true);
            indicator.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
        else
        {
            indicator.SetActive(false);
        }
    }
}
