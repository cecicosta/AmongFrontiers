using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Speecher: MonoBehaviour {

	public enum State { ACTIVE, INACTIVE };
	public State state = State.INACTIVE;

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
