using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class TextureInfoMessage : MessageBase {
    public byte[] textureData;
    public List<ActiveTrackableInfo> activeTrackables;
    public TextureInfoMessage() { }
    public TextureInfoMessage(byte[] d) { textureData = d; }
    public TextureInfoMessage(byte[] d, List<ActiveTrackableInfo> ts) { textureData = d; activeTrackables = ts; }

    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write(activeTrackables.Count);
        foreach(ActiveTrackableInfo ati in activeTrackables)
        {
            ati.NetworkSerialize(writer);
        }
    }
    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        activeTrackables = new List<ActiveTrackableInfo>(reader.ReadInt32());
        activeTrackables.Add(new ActiveTrackableInfo(reader.ReadInt32(), reader.ReadString(), reader.ReadTransform()));
    }
}

[System.Serializable]
public class ActiveTrackableInfo
{
    public int ID;
    public string name;
    public Transform transform;
    public ActiveTrackableInfo() { }
    public ActiveTrackableInfo(int id,string name,Transform transform)
    {
        this.ID = id;
        this.name = name;
        this.transform = transform;
    }
    public void NetworkSerialize(NetworkWriter writer)
    {
        writer.Write(ID);
        writer.Write(name);
        writer.Write(transform);
    }
}