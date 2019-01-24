using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MyComponent : MonoBehaviour {

    private bool mAccessCameraImage = true;
    Texture2D tex;

    public bool isServer = false;
    float lastTime;
    int channel;
    private bool connected = false;
    NetworkClient myClient;

    public RawImage UIImage;
    public Text text;
    public string IPAddress;
    public List<ActiveTrackableInfo> trackables;
    public LayoutGroup layoutGroup;
    public GameObject UIImagePrefab;


    // The desired camera image pixel format
    private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGBA8888;// or RGBA8888, RGB888, RGB565, YUV
    // Boolean flag telling whether the pixel format has been registered
    private bool mFormatRegistered = false;
    void Start()
    {
        trackables = new List<ActiveTrackableInfo>();

        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);

        //Network
        if(isServer)
        {
            ConnectionConfig Config = new ConnectionConfig();
            Config.AddChannel(QosType.Reliable);
            Config.AddChannel(QosType.Unreliable);
            channel = Config.AddChannel(QosType.ReliableFragmented);
            HostTopology Topology = new HostTopology(Config, 1);
            NetworkServer.Configure(Topology);
            NetworkServer.RegisterHandler(1000, getInfoTexture);
            NetworkServer.Listen(4444);
        }
        else
        {
            myClient = new NetworkClient();
            ConnectionConfig Config = new ConnectionConfig();
            Config.AddChannel(QosType.Reliable);
            Config.AddChannel(QosType.Unreliable);
            channel = Config.AddChannel(QosType.ReliableFragmented);
            HostTopology Topology = new HostTopology(Config, 2);
            myClient.Configure(Topology);
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            myClient.RegisterHandler(1000, getInfoTexture);
            myClient.Connect(IPAddress, 4444);
        }
        UIImage.texture = new Texture2D(640, 480, TextureFormat.RGB24, false);
        lastTime = Time.time;
    }
    /// <summary>
    /// Called when Vuforia is started
    /// </summary>
    private void OnVuforiaStarted()
    {
        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register pixel format " + mPixelFormat.ToString() +
                "\n the format may be unsupported by your device;" +
                "\n consider using a different pixel format.");
            mFormatRegistered = false;
        }

        foreach (TrackableBehaviour tb in TrackerManager.Instance.GetStateManager().GetTrackableBehaviours())
        {
            tb.gameObject.AddComponent<MyTrackableEventHandler>();
        }
    }
    /// <summary>
    /// Called when network is connected
    /// </summary>
    /// <param name="netMsg"></param>
    void OnConnected(NetworkMessage netMsg) { connected = true; Debug.Log("connected"); }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    void getInfoTexture(NetworkMessage msg)
    {
        //Debug.Log("hoge");
        TextureInfoMessage msg2 = msg.ReadMessage<TextureInfoMessage>();
        //text.text = "";
        // Iterate through the list of active trackables
        //text.text += "List of trackables currently active (tracked): \n";
        foreach (ActiveTrackableInfo t in msg2.activeTrackables)
        {
            //text.text += "Trackable: " + t.name + "\n";

            Debug.Log("Trackable: " + t.name);
            GameObject go = Instantiate(UIImagePrefab, layoutGroup.transform);
            go.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load("CardImages/"+t.name) as Sprite;
        }
        if (msg2.textureData.Length > 0) ((Texture2D)(UIImage.texture)).LoadImage(msg2.textureData);
    }
    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    private void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }
    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    private void OnTrackablesUpdated()
    {
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
                if (image != null)
                {
                    string imageInfo = mPixelFormat + " image: \n";
                    imageInfo += " size: " + image.Width + " x " + image.Height + "\n";
                    imageInfo += " bufferSize: " + image.BufferWidth + " x " + image.BufferHeight + "\n";
                    imageInfo += " stride: " + image.Stride;
                    //Debug.Log(imageInfo);
                    tex = new Texture2D(image.Width, image.Height, TextureFormat.RGB24, false);
                    tex.filterMode = FilterMode.Point;
                    image.CopyToTexture(tex);
                    tex.Apply();
                    byte[] jpg = tex.EncodeToJPG(50);
                    //((Texture2D)(UIImage.texture)).LoadImage(jpg);

                    //Networking

                    // Get the Vuforia StateManager
                    /*StateManager sm = TrackerManager.Instance.GetStateManager();
                    IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours();
                    foreach (TrackableBehaviour tb in activeTrackables)
                    {
                        trackables.Add(new ActiveTrackableInfo(tb.Trackable.ID, tb.TrackableName, tb.transform));
                    }*/

                    TextureInfoMessage msg = new TextureInfoMessage(jpg, trackables);

                    if (isServer)
                    {
                        if (NetworkServer.connections.Count > 0 && Time.time - lastTime > 0.04)
                        {
                            lastTime = Time.time;
                            NetworkServer.connections[1].SendByChannel(1000, msg, channel);
                        }
                    }
                    else
                    {
                        if(myClient.connection.isConnected && Time.time - lastTime > 0.04)
                        {
                            lastTime = Time.time;
                            myClient.SendByChannel(1000, msg, channel);
                            
                        }
                    }
                    trackables.Clear();
                    Debug.Log("hoge" + trackables.Count);
                }
            }
        }
    }
    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    private void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }
    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    private void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }
}