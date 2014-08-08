using UnityEngine;
using System;
using System.Collections;

public class PushTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		PushMsg.sInstance.MsgAction = msgAction;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,100,100) , "push local"))
		{
			//
		}
	}

	void OnApplicationPause( bool pause )
	{
		if(pause)
		{
			PushMsg.sInstance.PushLocalMsg(DateTime.Now.AddSeconds(5) , "hi,this is a test msessage");
		}
	}


	private void msgAction( IDictionary dc)
	{
		Debug.Log("dc");
	}
}
