using UnityEngine;
using System.Collections;

public class ButtonTrigger : Trigger {

	public ToolKitEvent trigger;
	public ToolKitEventTrigger eventTrigger;
	// Use this for initialization
	void Start () {
		eventTrigger = new ToolKitEventTrigger ();
		GetComponent<Button> ().OnClickEvent += Execute;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void Execute(string identifier){

		eventTrigger.TriggerEvent (trigger);
	}
}
