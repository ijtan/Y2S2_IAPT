using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImage))]

public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placebalePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedManager;

    private void Awake()
    {
        trackedManager = FindObjectOfType<ARTrackedImageManager>();

        foreach (GameObject pref in placebalePrefabs)
        {
            GameObject newPre = Instantiate(pref, Vector3.zero, Quaternion.identity);
            newPre.name = pref.name;
            newPre.SetActive(false);
            spawnedPrefabs.Add(pref.name, newPre);
        }
    }

    private void OnEnable()
    {
        trackedManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trImg in eventArgs.added)
        {
            UpdateImage(trImg);
        }

        foreach (ARTrackedImage trImg in eventArgs.updated)
        {
            UpdateImage(trImg);
        }

        foreach (ARTrackedImage trImg in eventArgs.removed)
        {
            UpdateImage(trImg);
        }
    }

    private void UpdateImage(ARTrackedImage trImg)
    {
        string name = trImg.referenceImage.name;
        GameObject prefab = spawnedPrefabs[name];

        if (trImg.trackingState != TrackingState.Tracking)
        {
            prefab.SetActive(false);
            return;
        }


        Vector3 pos = trImg.transform.position;
        prefab.transform.position = pos;
        prefab.SetActive(true);
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
