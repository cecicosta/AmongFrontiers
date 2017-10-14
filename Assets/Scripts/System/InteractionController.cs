using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractionController : ToolKitEventListener {

	/*
	 * Handle trees of interactions.
	 * When a current interactions is a leaf from its tree, it is removed 
	 * from the tree. If the tree is prevented from playing until the end 
	 * either for the execution being canceled, or some condition for the 
	 * next node is not satisfied, the tree may be discarded  from the 
	 * execution list unless it is marked to stay on a wait state. Time 
	 * out conditions stays on wait state by default.*/
    [HideInInspector]
	[SerializeField]
	private List<Interaction> interactionsTrees = new List<Interaction>();
	private List<Interaction> playInteractionTrees = new List<Interaction> ();
    private GameObject colliding = null;
    private bool suspend = false;
    private Coroutine routine;

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
        routine = StartCoroutine(UpdateController());
	}

    private void OnEnable() {
        if (routine == null)
            routine = StartCoroutine(UpdateController());
    }

    private void OnDisable() {
        if (routine != null)
            StopCoroutine(routine);
        routine = null;
    }

    // Update is called once per frame
    IEnumerator UpdateController () {
        while (true) {
            List<Interaction> toRemove = new List<Interaction>();
            List<Interaction> toAdd = new List<Interaction>();
            foreach (Interaction i in playInteractionTrees.ToArray()) {
                if (!i.IsActive()) {


                    yield return new WaitUntil(() => { return i.tkAction.isFinished(); });
                    if (i.HasChild()) {

                        Interaction next = i.getNext();
                        //All conditions may not be satisfied
                        if (next != null) {
                            //playInteractionTrees.Remove (i);
                            toRemove.Add(i);
                            i.SetAsInactive();
                            next.SetAsActive();
                            //playInteractionTrees.Add (next);
                            toAdd.Add(next);
                        }
                    } else {
                        //playInteractionTrees.Remove (i);
                        toRemove.Add(i);
                    }
                } else if ((i.useInteractionArea && colliding != null)) {
                    i.ExecuteAction(colliding);
                } else if (!i.useInteractionArea) {
                    i.ExecuteAction();
                } else {
                    i.SetAsInactive();
                    toRemove.Add(i);
                }

            }
            foreach (Interaction i in toAdd)
                playInteractionTrees.Add(i);
            foreach (Interaction i in toRemove)
                playInteractionTrees.Remove(i);

            yield return null;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        colliding = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        colliding = null;
    }

    public void Suspend() {
        suspend = true;
    }

    public void Resume() {
        suspend = false;
    }

    public override void onTKEvent(ToolKitEvent tkEvent){
        if (suspend)
            return;
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
