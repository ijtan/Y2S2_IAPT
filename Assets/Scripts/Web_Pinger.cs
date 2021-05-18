using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Web_Pinger : MonoBehaviour

{
    public string host = "http://192.168.0.7";
    public string port = "5000";
    public List<string> responses;
    public int counter = 0;

    private static Web_Pinger _instance;
    public static Web_Pinger Instance { get { return _instance; } }

    // Start is called before the first frame update
    public TMP_InputField hostname_input;

    public TMP_InputField port_input;

    public void hostUpdated(string text)
    {
        host = text;
        PlayerPrefs.SetString("host", host);
    }

    public void portUpdated(string text)
    {
        port = text;
        PlayerPrefs.SetString("port", port);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void setupPrefs()
    {
        if (PlayerPrefs.HasKey("host"))
            host = PlayerPrefs.GetString("host");
        else
            hostUpdated(host);

        if (PlayerPrefs.HasKey("port"))
            port = PlayerPrefs.GetString("port");
        else
            portUpdated(port);
        //port = PlayerPrefs.GetString("port");

        if (hostname_input != null)
        {
            hostname_input.text = host;
            hostname_input.onEndEdit.AddListener(hostUpdated);
        }
        if (port_input != null)
        {
            port_input.text = port;
            port_input.onEndEdit.AddListener(portUpdated);
        }

        //port_input.onEndEdit.AddListener(portUpdated);
    }

    // Start is called before the first frame update
    private void Start()
    {
        setupPrefs();
        DontDestroyOnLoad(gameObject);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}

    public void pingAPI(string page = "", Dictionary<string, string> args = null)
    {
        counter += 1;
        string urlToPing = host + ':' + port + '/' + page;
        Debug.Log("Recieved Ping Request: " + urlToPing);
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

        Debug.Log("Pinging: " + urlToPing);
        StartCoroutine(GetRequest(urlToPing));
    }

    //public void updateHost(string newHost)
    //{
    //    host = newHost;
    //}

    //public void updatePort(string newPort)
    //{
    //    port = newPort;
    //}

    private IEnumerator GetRequest(string uri)
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