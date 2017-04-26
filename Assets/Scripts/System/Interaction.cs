using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class Transition {	
	public InteractionController interactionController;
	public int toId;
	public int fromId;

	public Interaction To{
		get{ return interactionController.getInteractionById(toId); }
	}
	public Interaction From{
		get{ 
			InteractionController[] inter_ctrl = GameObject.FindObjectsOfType<InteractionController>();
			foreach( InteractionController ic in inter_ctrl ){
				Interaction interaction = ic.getInteractionById(fromId);
				if( interaction != null )
					return interaction;
			}
			return null;
		}
	}
	public int numberOfConnections;
	[SerializeField]
	public List<Condition> conditions = new List<Condition>();

	public Interaction GetInteraction(){
		return To;
	}
}

[System.Serializable]
public class Interaction {

	public int interactionId;
	public ToolKitAction tkAction;
	public ToolKitEvent tkEvent;
	public InteractionController interactionController;
	public List<Transition> parents = new List<Transition>();
	public List<Transition> children = new List<Transition>();

	public void ConnectTo(Interaction child){

		int numberOfConnections = 0;
		if( children.Find(x => x.To == child ) != null ){
			Transition tr = children[ children.FindLastIndex( x => x.To == child) ];
			numberOfConnections = tr.numberOfConnections + 1;
		}
		Transition toChild = new Transition ();
		toChild.numberOfConnections = numberOfConnections;
		toChild.interactionController = child.interactionController;
		toChild.toId = child.interactionId;
		toChild.fromId = this.interactionId;

		Transition toParent = new Transition ();
		toParent.numberOfConnections = numberOfConnections;
		toParent.interactionController = this.interactionController;
		toParent.toId = this.interactionId;
		toParent.fromId = child.interactionId;

		child.parents.Add(toParent);
		children.Add(toChild);
	}

	//Editor properties
	public Rect EditorWindowRect = new Rect (100, 100, 200, 72);
}
