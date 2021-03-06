using UnityEngine;
using System;
using System.Collections;
using jp.nyatla.nyartoolkit.cs.markersystem;
using jp.nyatla.nyartoolkit.cs.core;
using NyARUnityUtils;
using System.IO;

public class SimpleLiteWebBehaviour : MonoBehaviour
{
	private NyARUnityMarkerSystem _ms;
	private NyARUnityWebCam _ss;
	private int mid;//marker id
	private GameObject _bg_panel;
	void Awake()
	{
		this._ss=null;
	}
    IEnumerator Start ()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);
		if(Application.HasUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone)){
			Debug.Log("Authorized");
		}else{
			Debug.Log("not Authorized");
		}
		//setup unity webcam
		WebCamDevice[] devices= WebCamTexture.devices;
		if (devices.Length <=0){
			Debug.LogError("No Webcam.");
			yield break;
		}
		WebCamTexture w=new WebCamTexture(640,480,30);
		//Make WebcamTexture wrapped Sensor.
		this._ss=NyARUnityWebCam.createInstance(w);

		//Make configulation by Sensor size.
		NyARMarkerSystemConfig config = new NyARMarkerSystemConfig(this._ss.width,this._ss.height);
		this._ms=new NyARUnityMarkerSystem(config);
		
		//mid=this._ms.addARMarker("./Assets/Data/patt.hiro",16,25,80);
		//This line loads a marker from texture
		mid=this._ms.addARMarker((Texture2D)(Resources.Load("MarkerHiro", typeof(Texture2D))),16,25,80);

		//setup background
		this._bg_panel=GameObject.Find("Plane");
		this._bg_panel.GetComponent<Renderer>().material.mainTexture=w;
		this._ms.setARBackgroundTransform(this._bg_panel.transform);
		
		//setup camera projection
		this._ms.setARCameraProjection(this.GetComponent<Camera>());		
		//start sensor
		this._ss.start();
	}
	// Update is called once per frame
	void Update ()
	{
		if(this._ss==null){
			return;
		}
		//Update SensourSystem
		this._ss.update();
		//Update marker system by ss
		this._ms.update(this._ss);
		//update Gameobject transform
		if(this._ms.isExist(mid)){
			this._ms.setTransform(mid,GameObject.Find("MarkerObject").transform);
		}else{
			// hide Game object
			GameObject.Find("MarkerObject").transform.localPosition=new Vector3(0,0,-100);
		}
	}

}
