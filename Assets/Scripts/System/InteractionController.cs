using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionController : ToolKitEventListener {

	/*
	 * Handle trees of interactions.
	 * When a current interactions is a leaf from its tree, it is removed 
	 * from the tree. If the tree is prevented from playing until the end 
	 * either for the execution being canceled, or some condition for the 
	 * next node is not satisfied, the tree may be discarded  from the 
	 * execution list unless it is marked to stay on a wait state. Time 
	 * out conditions stays on wait state by default.*/

	[SerializeField]
	private List<Interaction> interactionsTrees = new List<Interaction>();
	private List<Interaction> playInteractionTrees = new List<Interaction> ();
	public List<Interaction> Interactions{
		get{ return interactionsTrees; }
	}

	public static int newId(){
		return Guid.NewGuid().GetHashCode();
	}

	public Interaction getInteractionById(int interactionId){
		if(interactionsTrees.Count > 0)
			return Interactions.Find (x => x.interactionId == interactionId);
		return null;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		List<Interaction> toRemove = new List<Interaction> ();
		List<Interaction> toAdd = new List<Interaction> ();
		foreach (Interaction i in playInteractionTrees.ToArray()) {
			if (!i.IsActive ()) {

				if (i.HasChild ()) {
						Interaction next = i.getNext ();
						//All conditions may not be satisfied
						if (next != null) {
								//playInteractionTrees.Remove (i);
								toRemove.Add (i);
								i.SetAsInactive ();
								next.SetAsActive ();
								//playInteractionTrees.Add (next);
								toAdd.Add(next);
						}
				} else
						//playInteractionTrees.Remove (i);
						toRemove.Add(i);
			}else {
				i.ExecuteAction();
			}
		}
		foreach (Interaction i in toAdd)
						playInteractionTrees.Add (i);
		foreach (Interaction i in toRemove)
						playInteractionTrees.Remove (i);
	}

	public override void onTKEvent(ToolKitEvent tkEvent){
		foreach( Interaction i in interactionsTrees ){
			//Is root
			if( i.parents.Count == 0 ){
				bool allConditionsSatisfied = true;
				foreach(Condition c in  i.conditions ){
					if( !c.checkConditionVariable( ) && !c.checkConditionTrigger( tkEvent.condition.identifier ) && !c.checkConditionKey(tkEvent.condition)) {
						allConditionsSatisfied = false;
					}
				}

				if(allConditionsSatisfied)
				{
					i.SetAsActive();	
					playInteractionTrees.Add(i);
				}
			}
		}		
	}

}
