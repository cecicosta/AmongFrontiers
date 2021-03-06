﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Transition {	
    [HideInInspector]
	public InteractionController interactionController;
    [HideInInspector]
    public int toId;
    [HideInInspector]
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

	private enum State{ ACTIVE, INACTIVE }; 
	private State state = State.INACTIVE;

    public UnityEngine.Events.UnityEvent eventTest;

    [HideInInspector]
    public int interactionId;
    [SerializeField]
	public ToolKitAction tkAction;
    [HideInInspector]
    public InteractionController interactionController;
    [HideInInspector]
    public List<Transition> parents = new List<Transition>();
    [HideInInspector]
    public List<Transition> children = new List<Transition>();
	[SerializeField]
    [HideInInspector]
    public List<Condition> conditions = new List<Condition>();


	private float timeTicker = 0;
    [HideInInspector]
    public float timeout = 0;
    [HideInInspector]
    public bool useInteractionArea = false;

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

    public Interaction() {
        iteractionEnumerator = GetNext().GetEnumerator();
    }

	public bool HasChild(){
		return children.Count > 0;
	}

	public bool IsReady(){
		bool allConditionsSatisfied = true;
		foreach (Condition c in  conditions) {
            Condition v = GameVariablesManager.Instance.GetVariable(c.identifier);
			bool isSatisfied = false;
			if ( c.checkConditionVariable( ) )
				isSatisfied = true;
			else if ( c.checkConditionKey( v ) )
				isSatisfied = true;
			if( !isSatisfied ){
				allConditionsSatisfied = false;
				break;
			}
		}
		return allConditionsSatisfied;
	}

	public void SetAsActive(){
		if( state == State.INACTIVE )
			timeTicker = Time.time;
		state = State.ACTIVE;
	}
	public void SetAsInactive() {
		state = State.INACTIVE;
		timeTicker = 0;
	}
	public bool IsActive(){
		return state == State.ACTIVE;
	}

	public bool TimeOut(){
		if (timeout <= Time.time - timeTicker) {
			return true;
		}
		return false;
	}

	public void ExecuteAction(){
		if ( tkAction != null && TimeOut () ) {
			tkAction.Execute (interactionController.gameObject);
			SetAsInactive();		
		}
	}

    public void ExecuteAction(GameObject colliding) {
        if (tkAction != null && TimeOut()) {
            tkAction.Execute(colliding, interactionController.gameObject);
            SetAsInactive();
        }
    }

    public IEnumerable<Interaction> GetNext(){
        foreach (Transition t in children) {
            Interaction i = t.interactionController.getInteractionById(t.toId);
            yield return i;
        }
	}

    IEnumerator<Interaction> iteractionEnumerator;

    //Editor properties
    [HideInInspector]
    public Rect EditorWindowRect = new Rect (100, 100, 200, 72);
    public Collider2D interactWith;
}
