using UnityEngine;

public class buttonManager : MonoBehaviour
{
    public GameObject settingsMenu;

    //Start is called before the first frame update
    private void Start()
    {
        settingsMenu.SetActive(false);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}

    public void showSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void hideSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void refreshLandmarksFromApi()
    {
        GPS.Instance.forceOnceUpdateGPS();
        GPS.Instance.updateLandmarksFromApi();
    }
}