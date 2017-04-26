using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class InteractionController : ToolKitEventListener {

	[SerializeField]
	private List<Interaction> interactions = new List<Interaction>();
	public List<Interaction> Interactions{
		get{ return interactions; }
	}

	public static int newId(){
		return Guid.NewGuid().GetHashCode();
	}

	public Interaction getInteractionById(int interactionId){
		if(interactions.Count > 0)
			return Interactions.Find (x => x.interactionId == interactionId);
		return null;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void onTKEvent(ToolKitEvent tkEvent){
	}
}
