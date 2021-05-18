using UnityEngine;

public class InformationRetainer : MonoBehaviour
{
    public landmark_info landmarkObject;
    public string id;

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    //void Update()
    //{
    //}
}