using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogController : MonoBehaviour {
	enum State {STOP, PLAY, WAIT, QUERY ,SELECT, STANDBY, INACTIVE };

	//Controller States: Keep all controllers syncronized
	private State controllerState = State.INACTIVE;

	private static bool registerSpeachers = false;
	private static Dictionary<string, Speecher> register = new Dictionary<string, Speecher>();

	//Controller atributes
	public DialogShelf dialogShelf;
	public float time = 5;

	//Costumizable buttons for dialog options
	public List<Button> buttons;
	public Button dialogbox;

	public bool talk = false;

	private List<string> options = new List<string>();

	private int selected = 0;

	//Player controls
	private DialogSet currentDialogSet;
	private Dialog current;
	private float timer;
	private bool skipped = false;
	public  bool Skip{
		set{ skipped = value; }
	}

	// Event Handler: Used to notify conditions triggered by finished dialogs
	public delegate void OnDialogStart(string tag);
	public event OnDialogStart onDialogStartEvent;
	
	public delegate void OnDialogEnd(string tag);
	public event OnDialogEnd onDialogEndEvent;

	void Start(){

		if( !registerSpeachers ){
			Speecher[] speechers = FindObjectsOfType<Speecher>();
			foreach( Speecher s in speechers ){
				//print(s.identifier);
				register.Add(s.identifier, s);
			}
			registerSpeachers = true;
		}
		//Listen for the buttom click events
		foreach( Button b in buttons ){
			b.OnClickEvent += OnSelectedOption;
		}
	}
	/*
	//Player Method
	void OnPlayNotify(Dialog d){
		//Capture mensage send by controller
		if( d.characterIdentifier.CompareTo(identifier) == 0 ){
			state = State.PLAY;
		}
	}
	*/

	//Controller Method: Listen the buttons to choose one dialog option
	void OnSelectedOption(string identifier){
		if( controllerState != State.STANDBY )
			return;
		int selected = 0;
		foreach(Button b in buttons){
			if( identifier.CompareTo(b.identifier) == 0 ){
				SelectOption(selected);
				return;
			}
			selected++;
		}
	}

	//ControllerMethod
	public void SelectOption( int option ){
		selected = option;
		controllerState = State.SELECT;
	}

	//Controller Method
	public void PlayDialog( Dialog d ){
		//Cause the controller to change its state
		current = d;
		controllerState = State.PLAY;
	}
	
	//Controller Method: Trigger on direct interactions
	public bool TriggerDialog(){
		Debug.Log ("Trigged!");
		foreach( DialogSet d in dialogShelf.layers ){
			int conditionCounter = 0;
			for( int i=0; i<d.conditions.Count; i++ ){
				if( d.originalValues[i] == d.currentValues[i] )
					conditionCounter++;
			}
			if( conditionCounter == d.conditions.Count ){
				d.Load();
				d.Reset();
				currentDialogSet = d;
				PlayDialog( d.Current );
				return true;
			}
		}
		return false;
	}

	public bool TriggerDialog( string flag ){
		foreach( DialogSet d in dialogShelf.layers ){
			if( d.layer.CompareTo(flag) == 0 )
			{
				d.Load();
				d.Reset();
				currentDialogSet = d;
				PlayDialog( d.Current );
				return true;
			}
		}
		Debug.Log ("TriggerDialog - Object does not have the specified Dialog");
		return false;
	}
	/*
	//Controller Method: Check for interactions the controller
	void OnTriggerStay2D(Collider2D other) {
		//buttons[0].text.text = "Entrou";
		if( talk && other.GetComponent<CharacterPlatformerController>() != null ){
			talk = false;
			TriggerDialog();
		}
	}

	void OnTriggerTalk(string identifier){
		print(identifier);
		talk = true;
	}
*/
	//Controller Method
	void ControllerUpdate(){

		skipped = false;
		if( Input.GetMouseButtonUp(0) ){
			Skip = true;
		}

		Speecher speecher = null;
		switch( controllerState ){
			
			case State.STOP:
				speecher = register[current.characterIdentifier];
				dialogbox.GetComponent<Renderer>().enabled = false;
				dialogbox.text.text = "";
				//speecher.face.renderer.enabled = false;
				controllerState = State.INACTIVE;

			break;
			case State.PLAY:
					
				timer = Time.time;
				//Diaplay faceset and dialogbox
				speecher = register[current.characterIdentifier];
				//speecher.face.renderer.enabled = true;
				//Display Dialogbox
				dialogbox.GetComponent<Renderer>().enabled = true;
				dialogbox.text.text = current.text;	

				speecher.OnDialogStartNotify(current.tag);

				try{
					onDialogStartEvent(current.tag);
				}catch( System.Exception e ){
					Debug.Log(e);
				}	

				controllerState = State.WAIT;
			break;
			case State.WAIT:
				//Waiting for something happens and change the state
				if( time < Time.time - timer || skipped ){
					
					controllerState = State.QUERY;
				}
			break;
			case State.STANDBY:
			break;
			case State.QUERY:
				//Make the selection beetween dialog options 
				//NOTE: Make sure the controller have enough buttons
				if( current.query.Count > 1 ){
					for( int i=0; i< current.query.Count; i++ ){
						Button b = buttons[i];
						b.GetComponent<Renderer>().enabled = true;
						b.text.text = current.query[i];
					}
					controllerState = State.STANDBY;
				}else
					controllerState = State.SELECT;

			break;
			case State.SELECT:
				if( current.query.Count > 1 ){

					for( int i=0; i< current.query.Count; i++ ){
						Button b = buttons[i];
						b.GetComponent<Renderer>().enabled = false;
						b.text.text = "";
					}
					speecher = register[current.characterIdentifier];
					//speecher.face.renderer.enabled = false;

					speecher.OnDialogEndNotify(current.tag);
					try{
						onDialogEndEvent(tag);
					}catch(System.Exception e){
						Debug.Log(e);
					}

					currentDialogSet.MoveNext(selected);
					current = currentDialogSet.Current;
					if(current.updateRoot)
						currentDialogSet.Root = current;
					controllerState = State.PLAY;
				}else if( current.query.Count == 1 ){
					speecher = register[current.characterIdentifier];
					//speecher.face.renderer.enabled = false;

					speecher.OnDialogEndNotify(current.tag);
					try{
						onDialogEndEvent(tag);
					}catch(System.Exception e){
						Debug.Log(e);
					}

					currentDialogSet.MoveNext();
					current = currentDialogSet.Current;
					if(current.updateRoot)
						currentDialogSet.Root = current;
					controllerState = State.PLAY;
				}else{
					speecher = register[current.characterIdentifier];
					//speecher.face.renderer.enabled = false;

					speecher.OnDialogEndNotify(current.tag);
					try{
						onDialogEndEvent(tag);
					}catch(System.Exception e){
						Debug.Log(e);
					}
					
					controllerState = State.STOP;
				}
			break;
		}
			
	}

	//Player Method
	void Update(){

		ControllerUpdate();

	}



	
}
