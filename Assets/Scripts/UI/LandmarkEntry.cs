using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandmarkEntry : MonoBehaviour
{
    public string id;
    public string title;
    public string short_desc;
    public string long_desc;
    public string image_url;

    public double distance;
    public double real_distance;




    public DirectionArrowManager dirArrow;

    public TextMeshProUGUI title_text_UI;
    public TextMeshProUGUI desc_text_UI;


    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().interactable = false;
        Debug.Log("New Entry Instantiated, title is: " + title);
        Debug.Log("New Entry Instantiated, title is: " + title);
        dirArrow = GetComponentInChildren<DirectionArrowManager>();

        foreach(TextMeshProUGUI c in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (c.name == "Title")
                title_text_UI = c;
            else if(c.name == "Description")
                desc_text_UI = c;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!GPS.Instance.landmarks_data.ContainsKey(id))
        {
            Debug.LogError("Could not find landmark in GPS!");
            return;
        }
        activated = GPS.Instance.landmarks_data[id].near;
        dirArrow.landmarkLocation = new Vector2(GPS.Instance.landmarks_data[id].locX, GPS.Instance.landmarks_data[id].locY);
        dirArrow.currentLocation = new Vector2(GPS.Instance.lat, GPS.Instance.lon);

        //landmarkNameUI.title = title;
        //landmarkNameUI.description = description;
        title_text_UI.text = title;
        desc_text_UI.text = short_desc;

        if (activated)
        {
            dirArrow.activate();
            GetComponent<Button>().interactable = true;
        }
        else
        {
            dirArrow.deactivate();
            GetComponent<Button>().interactable = false;
        }
        distance = dirArrow.distance;
        real_distance = dirArrow.real_distance;
    }

    public void ArMode()
    {
        SceneManager.LoadScene("ArMode");
    }
}
