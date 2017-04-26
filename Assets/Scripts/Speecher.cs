using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Frame{
	public Texture2D texture;
	public Rect textRect;
}
[RequireComponent(typeof(SpriteRenderer))]
public class Speecher : MonoBehaviour {

	public string character;
	public string identifier;
	public SpriteRenderer face;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Talk(){
	}

	public void OnDialogStartNotify( string tag ){

	}

	public void OnDialogEndNotify( string tag ){

	}
}
