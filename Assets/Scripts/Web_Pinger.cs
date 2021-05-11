using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Web_Pinger : MonoBehaviour


{
    
    public string host = "http://192.168.0.7";
    public string port = "5000";
    public List<string> responses;
    public int counter = 0;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}


    public void pingAPI(string page = "", Dictionary<string, string> args = null)
    {

        
        string urlToPing = host + ':' + port + '/' + page;
        if (args != null)
        {

            bool isfirst = true;
            foreach (KeyValuePair<string, string> arg in args)
            {
                if (isfirst)
                {
                    urlToPing += '?';
                    isfirst = false;

                }

                else
                    urlToPing += '&';

                urlToPing += arg.Key;
                urlToPing += '=';
                urlToPing += arg.Value;

            }
        }
        counter += 1;
        Debug.Log("Pinging: " + urlToPing);
        StartCoroutine(GetRequest(urlToPing));
    }

    IEnumerator GetRequest(string uri)
    {
        
        string response = "";
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    response = webRequest.downloadHandler.text;
                    Debug.Log(pages[page] + ":\nReceived: " + response);
                    break;
            }
        }
        responses.Add(response);

    }
}