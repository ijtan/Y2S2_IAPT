using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class refreshButtonCaller : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void refreshLandmarksFromApi()
    {
        GPS.Instance.updateLandmarksFromApi();
    }

}
