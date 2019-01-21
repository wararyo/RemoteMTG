using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextureInfoMessage : MessageBase {
    public byte[] textureData;
    public TextureInfoMessage() { }
    public TextureInfoMessage(byte[] d) { textureData = d; }
}
