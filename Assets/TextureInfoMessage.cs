using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class TextureInfoMessage : MessageBase {
    public byte[] textureData;
    public IEnumerable<TrackableBehaviour> activeTrackables;
    public TextureInfoMessage() { }
    public TextureInfoMessage(byte[] d) { textureData = d; }
    public TextureInfoMessage(byte[] d, IEnumerable<TrackableBehaviour> at) { textureData = d; activeTrackables = at; }
}
