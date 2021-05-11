using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandmarkEntry : MonoBehaviour
{
    public string Name;
    public DirectionArrowManager dirArrow;
    public TextMeshPro landmarkNameUI;

    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        dirArrow = GetComponentInChildren<DirectionArrowManager>();
        landmarkNameUI =  GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {   

        landmarkNameUI.text = Name;
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
