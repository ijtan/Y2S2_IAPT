using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandmarkListManager : MonoBehaviour
{
    // Start is called before the first frame update
    Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();
    public GameObject entryPrefab;
    private List<LandmarkEntry> sorted_button_list = new List<LandmarkEntry>();

    //private ScrollRect scrollRect;
    //[SerializeField] private GameObject loadMoreObjectPosition;
    //[SerializeField] private float scrollRefreshOffset = 30;

    void Start()
    {
        //scrollRect = GetComponentInParent<ScrollRect>();
        //scrollRect.onValueChanged.AddListener(OnScrollRectChange);

    }


    //private void OnScrollRectChange(Vector2 position)
    //{
    //    //if (loading || !enableScrollLoadMore)
    //    //    return;

    //    // Detect if more messages should be loaded
    //    Vector2 loadMorePivotBottom = new Vector2(transform.position.x, transform.position.y);
    //    Debug.Log("Scroll Change: "+(loadMoreObjectPosition.transform.position.y > loadMorePivotBottom.y - scrollRefreshOffset));
    //    Debug.Log("Scroll Change diff "+ loadMoreObjectPosition.transform.position.y +">"+ (loadMorePivotBottom.y - scrollRefreshOffset));
    //    if (loadMoreObjectPosition.transform.position.y < loadMorePivotBottom.y + scrollRefreshOffset)
    //    {
    //        Debug.Log("Refreshing Landmarks!");
    //    }
    }



    // Update is called once per frame


    void Update()
    {
        //GPS.Instance.landmark_
 
         //ADD NEW BUTTONS
        foreach (KeyValuePair<string, landmark_info> landmark in GPS.Instance.landmarks_data)
        {
            if (!buttons.ContainsKey(landmark.Key))
            {
                GameObject newLM = Instantiate(entryPrefab);
                newLM.transform.SetParent(this.transform, false);
                //newLM.transform.parent = this.transform;
                newLM.GetComponent<LandmarkEntry>().id = landmark.Key;
                newLM.GetComponent<LandmarkEntry>().title = landmark.Value.title;
                newLM.GetComponent<LandmarkEntry>().description = landmark.Value.description;
                newLM.GetComponent<LandmarkEntry>().image_url = landmark.Value.image_url;
                buttons.Add(landmark.Key, newLM);
            }
        }

        //ORDER LIST BY DISTANCE

        sorted_button_list.Clear();
        foreach (KeyValuePair<string, GameObject> butt in buttons)
        {
            if(!GPS.Instance.landmarks_data.ContainsKey(butt.Key))
            {
                Destroy(butt.Value);
                buttons.Remove(butt.Key);
            }
            LandmarkEntry blm = butt.Value.GetComponent<LandmarkEntry>();
            sorted_button_list.Add(blm);
        }

        sorted_button_list.Sort((x, y) => x.distance.CompareTo(y.distance));

        for (int i = 0; i < sorted_button_list.Count; i++)
        {
            buttons[sorted_button_list[i].id].transform.SetSiblingIndex(i);
        }
    }

    
}
