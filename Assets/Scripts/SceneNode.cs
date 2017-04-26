using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UsableItem{
	public string item;
	public string itemSignal;
}

public class SceneNode : MonoBehaviour {


	[HideInInspector]
	public int id = -1;
	public string identifier = "";
	public string lookSignal = "";
	public string talkSignal = "";
	public string grabSignal = "";
	public List<UsableItem> usableItens; 
	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public DialogController dialogController;
	[HideInInspector]
	public AudioSource audioSource;

	// Use this for initialization
	void Start () {

		animator = GetComponent<Animator> ();
		dialogController = GetComponent<DialogController> ();
		audioSource = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
