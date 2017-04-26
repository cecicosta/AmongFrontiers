using UnityEngine;
using System.Collections;

public class GrabAction : Trigger {

	private enum State {LISTENING, WAITING, MOVING, ACTIVE };
	private State state = State.LISTENING;
	public string defaultSignal = "";
	private Button grabButton;
	
	// Use this for initialization
	void Start () {
		grabButton = GameObject.FindGameObjectWithTag("GrabButtom").GetComponent<Button>();
		grabButton.OnClickEvent += OnTriggerGrab;
		print(grabButton);
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
						if( sn != null && sn.grabSignal.CompareTo("") != 0 ){
							TriggerFlag(sn.grabSignal, true);
							state = State.LISTENING;
							empty = false;
							break;
						}
					}
					if(empty){
						TriggerFlag(defaultSignal, true);
						state = State.LISTENING;
					}
				}
			}else{

			}
			break;
		case State.MOVING:
			break;
		case State.ACTIVE:
			break;
		}
	}

	void OnTriggerGrab( string identifier ){

		state = State.WAITING;
	}
	
	void OnTrigger(){
		
	}
}
