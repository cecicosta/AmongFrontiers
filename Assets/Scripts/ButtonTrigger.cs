using UnityEngine;
using System.Collections;

public class ButtonTrigger : Trigger {

	public string activateFlag = "";
	// Use this for initialization
	void Start () {
		GetComponent<Button> ().OnClickEvent += Execute;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void Execute(string identifier){
		TriggerFlag (activateFlag, true);
	}
}
