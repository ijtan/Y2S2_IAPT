using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationRetainer : MonoBehaviour
{

    public landmark_info landmarkObject;
    public string id;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }


    // Update is called once per frame
    //void Update()
    //{

    //}
}
