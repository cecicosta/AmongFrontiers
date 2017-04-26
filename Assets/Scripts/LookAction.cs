using UnityEngine;
using System.Collections;

public class LookAction : Trigger {

	private enum State {LISTENING, WAITING, MOVING, ACTIVE };
	private State state = State.LISTENING;
	public string defaultSignal = "";
	private Button lookButton;
	
	// Use this for initialization
	void Start () {
		lookButton = GameObject.FindGameObjectWithTag("LookButtom").GetComponent<Button>();
		lookButton.OnClickEvent += OnTriggerLook;
	}
	
	// Update is called once per frame
	void Update () {
		
		switch(state){
		case State.LISTENING:
			break;
		case State.WAITING:

			Vector3 position = InterfaceManager.cursorPosition;
			RaycastHit2D[] targets = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(position), Vector2.zero, -999);
			if( targets != null ){

				if( InterfaceManager.worldRelease ){
					InterfaceManager.UseReleaseEvent();
					bool empty = true;
					foreach( RaycastHit2D hit in targets )
					if( hit.collider != null  ){
						SceneNode sn = hit.collider.GetComponent<SceneNode>();
						if( sn != null && sn.lookSignal.CompareTo("") != 0 ){
							TriggerFlag(sn.lookSignal, true);
							empty = false;
							state = State.WAITING;
							break;
						}
					}	
					if(empty){
						TriggerFlag(defaultSignal, true);
						state = State.LISTENING;

					}
				}
			}
			break;
		case State.MOVING:
			break;
		case State.ACTIVE:
			break;
		}
		
		
	}

	void OnTriggerLook( string identifier ){
		state = State.WAITING;
	}
	
	void OnTrigger(){
		
	}
}
