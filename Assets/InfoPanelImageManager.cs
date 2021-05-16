using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InfoPanelImageManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject imagePrefab;
    public string[] image_urls;
    public int curr_index = -1;
    public List<GameObject> spawnedImages;

    //void Start()
    //{
    //    downloadImages(image_urls);
    //}

    IEnumerator setImageWithUrl(GameObject image, string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else
            image.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }


    public void downloadImages(string[] urls)
    {
        foreach (string s in urls)
        {
            var newImage = Instantiate(imagePrefab);
            newImage.transform.SetParent(transform, false);
            newImage.SetActive(false);
            StartCoroutine(setImageWithUrl(newImage, s));
            spawnedImages.Add(newImage);
        }
    }    

    public bool nextImage()
    {
        if (curr_index + 1 >= imageCount())
        {
            Debug.LogError("Could not increment image further!");
            return false;
        }

        if(curr_index!=-1)
            deactivateImage(curr_index);
        curr_index++;
        activateImage(curr_index);

        return true;
    }

    public bool prevImage()
    {
        if (curr_index <= 0)
        {
            Debug.LogError("Could not decrement image further!");
            return false;
        }

        deactivateImage(curr_index);
        curr_index--;
        activateImage(curr_index);

        return true;
    }

    public void hideAllImages()
    {
        foreach(RawImage i in GetComponentsInChildren<RawImage>())
        {
            i.gameObject.SetActive(false);
        }
    }

    public int imageCount()
    {
        return transform.childCount;
    }
    public void activateImage(int index)
    {
        GetComponentsInChildren<RawImage>(true)[index].gameObject.SetActive(true);
    }
    public void deactivateImage(int index)
    {
        GetComponentsInChildren<RawImage>(true)[index].gameObject.SetActive(false);
    }
}
