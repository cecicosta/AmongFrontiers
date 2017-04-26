using UnityEngine;
using System.Collections;

public class ItemAction : Trigger {

	private enum State {LISTENING, WAITING, MOVING, ACTIVE };
	private State state = State.LISTENING;
	public string defaultSignal = "";
	public string item = "";
	private Button itemButton;
	
	// Use this for initialization
	void Start () {
		itemButton = GetComponent<Button>();
		itemButton.OnClickEvent += OnTriggerItem;
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
						if( sn != null ){
							UsableItem ui = sn.usableItens.Find( x => x.item == item );
							if( ui != null ){
								TriggerFlag(ui.itemSignal, true);
								state = State.LISTENING;
								empty = false;
								break;
							}
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

	void OnTriggerItem( string identifier ){
		state = State.WAITING;
	}
	
	void OnTrigger(){
		
	}
}
