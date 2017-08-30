using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractionAreaEventTrigger : ToolKitEventListener {

	public Condition listenTo;
	public Condition trigger;
	public GameObject interactWith;
	private ToolKitEventTrigger tkEventTrigger;
	private bool colliding;
	// Use this for initialization
	void Start () {
		tkEventTrigger = new ToolKitEventTrigger ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D( Collider2D collider ){
		if (collider.gameObject == interactWith) {
            colliding = true;
            //	Debug.Log ("collide");
		}
	}
	void OnTriggerExit2D( Collider2D collider ){
		if (collider.gameObject == interactWith)
			colliding = false;
			
	}

	void CallTrigger(){
		tkEventTrigger.TriggerEvent (new ToolKitEvent(trigger));
	}

	public override void onTKEvent(ToolKitEvent tkEvent){
		if (!colliding)
			return;
		if (listenTo.checkConditionVariable ()) 
			Invoke ("CallTrigger", 0);
		if (listenTo.checkConditionTrigger (tkEvent.condition.identifier)) 
			Invoke ("CallTrigger", 0);
		if (listenTo.checkConditionKey (tkEvent.condition))
			Invoke ("CallTrigger", 0);
	}


}
