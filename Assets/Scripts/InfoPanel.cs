using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public string title;
    public string[] image_urls;
    public string description;

    public bool isReverse;



    //float x;
    //float y;
    //float z;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(DownloadImage(url));
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate towards target    
        var targetPoint = Camera.main.transform.position;
        targetPoint.y = transform.position.y;

        var targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);


        if (Camera.main)
        {
            var cameraTransform = Camera.main.gameObject.transform;
            transform.LookAt(cameraTransform);
            if (isReverse) transform.forward *= -1;
        }
    }



    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        //request.result == UnityWebRequest.Result.ConnectionError
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else
        {
            GetComponentInChildren<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}
