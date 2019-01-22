using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class TextureInfoMessage : MessageBase {
    public byte[] textureData;
    public List<Trackable> activeTrackables;
    public TextureInfoMessage() { }
    public TextureInfoMessage(byte[] d) { textureData = d; }
    public TextureInfoMessage(byte[] d, List<Trackable> ts) { textureData = d; activeTrackables = ts; }
}
