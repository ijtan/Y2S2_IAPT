using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPS : MonoBehaviour
{
    public float lat;
    public float lon;

    public static GPS Instance { set; get; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocServ());
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

        while(Input.location.status==LocationServiceStatus.Initializing && maxWait > 0)
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
            Debug.LogError("Unable ti dteermine dvice location!");
            yield break;
        }

        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;

        yield break;

    }


    //// Update is called once per frame
    void Update()
    {

    }
}
