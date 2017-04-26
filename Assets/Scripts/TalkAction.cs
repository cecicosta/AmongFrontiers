using UnityEngine;
using System.Collections;

public class TalkAction : Trigger {

	private enum State {LISTENING, WAITING, MOVING, ACTIVE };
	private State state = State.LISTENING;
	
	private Button talkButton;
	private CharacterPlatformerController cpc;

	private Vector2 targetPosition;
	private Collider2D target;
	
	// Use this for initialization
	void Start () {
		talkButton = GameObject.FindGameObjectWithTag("TalkButtom").GetComponent<Button>();
		talkButton.OnClickEvent += OnTriggerTalk;

		cpc = GetComponent<CharacterPlatformerController>();
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
						
						foreach( RaycastHit2D hit in targets ){
						if( hit.collider != null ){
							if( hit.collider.GetComponent<DialogController>() != null ){
								target = hit.collider;
								targetPosition = InterfaceManager.cursorWorldPosition;
								cpc.MoveTo(targetPosition);
								state = State.MOVING;
							}	
							SceneNode sn = hit.collider.GetComponent<SceneNode>();
							if(  hit.collider != null && sn != null ){
								TriggerFlag( sn.talkSignal, true );
							}
						}
					}
				}
			}


			break;
		case State.MOVING:

			if( Mathf.Abs( transform.position.x - targetPosition.x ) <= 0.2 )
				if( target.GetComponent<DialogController>() != null ){
						target.GetComponent<DialogController>().TriggerDialog();
						state = State.ACTIVE;
					}else{
						//Cannot talk to it
					}
			break;
		case State.ACTIVE:
			break;
		}
		
		
	}
	
	void OnTriggerTalk( string identifier ){
		state = State.WAITING;
	}
}
