using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void Update()
    {


        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            MainMenu();
        }

    }

    public void GPS_Scene()
    {
        SceneManager.LoadScene("GPS");
    }
    public void AR_Scene()
    {
        SceneManager.LoadScene("PlaneTrack");
    }
    public void IR_Scene()
    {
        SceneManager.LoadScene("MultiImageTrack");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
