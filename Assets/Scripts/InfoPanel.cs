using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public string title;
    public string[] image_urls;
    public string description;

    public bool isReverse;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Desc;

    public GameObject descPage;

    public Button nextPage;
    public Button prevPage;

    private InfoPanelImageManager imageManager;

    //float x;
    //float y;
    //float z;

    // Start is called before the first frame update
    private void Start()
    {
        transform.position = new Vector3(0, 0, 2);
        imageManager = GetComponentInChildren<InfoPanelImageManager>();
        imageManager.curr_index = -1;
        imageManager.image_urls = image_urls;
        imageManager.downloadImages(image_urls);
        //StartCoroutine(DownloadImage(url));
    }

    // Update is called once per frame
    private void Update()
    {
        Desc.text = description;
        Title.text = title;
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

        //curr_index = imageManager.curr_index;

        if (imageManager.curr_index <= -1)
            prevPage.interactable = false;
        else
            prevPage.interactable = true;

        if (imageManager.curr_index + 1 >= imageManager.imageCount())
            nextPage.interactable = false;
        else
            nextPage.interactable = true;
    }

    public void showNextPage()
    {
        if (imageManager.curr_index + 1 >= imageManager.imageCount())
            return;

        if (imageManager.curr_index == -1)
        {
            descPage.SetActive(false);
        }

        imageManager.nextImage();
    }

    public void showPrevPage()
    {
        if (imageManager.curr_index - 1 < -1)
            return;

        if (imageManager.curr_index - 1 == -1)
        {
            imageManager.curr_index--;
            imageManager.hideAllImages();
            descPage.SetActive(true);
            return;
        }

        imageManager.prevImage();
    }

    //IEnumerator DownloadImage(string MediaUrl)
    //{
    //    UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
    //    yield return request.SendWebRequest();
    //    //request.result == UnityWebRequest.Result.ConnectionError
    //    if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //        Debug.Log(request.error);
    //    else
    //    {
    //        GetComponentInChildren<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //    }
    //}
}