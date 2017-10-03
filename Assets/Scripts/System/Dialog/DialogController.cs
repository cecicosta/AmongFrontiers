using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DialogController : ToolKitEventListener {
	enum State {STOP, PLAY, WAIT, QUERY ,SELECT, STANDBY, INACTIVE };

	//Controller States: Keep all controllers syncronized
	private State controllerState = State.INACTIVE;

	private static bool registerSpeachers = false;
	private static Dictionary<string, Speaker> register = new Dictionary<string, Speaker>();

	//Controller atributes
	public DialogShelf dialogShelf;

    public Speaker speakerTrigger;

    //Costumizable buttons for dialog options
    public List<Button> buttons;

	private List<string> options = new List<string>();

	private int selected = 0;

	//Player controls
	private DialogSet currentDialogSet;
	private Dialog current;
	private float timer;
	private bool skipped = false;
    public Condition skipDialogCondition;
    private Speaker speakerTarget;
    
    private ToolKitEventTrigger onDialogTrigger;

    public  bool Skip{
		set{ skipped = value; }
	}

    // Event Handler: Used to notify conditions triggered by finished dialogs
    public delegate void OnDialogStart(string tag);
	public event OnDialogStart onDialogStartEvent;
	
	public delegate void OnDialogEnd(string tag);
	public event OnDialogEnd onDialogEndEvent;

	void Start(){
        onDialogTrigger = new ToolKitEventTrigger();

		if( !registerSpeachers ){
			Speaker[] speechers = FindObjectsOfType<Speaker>();
			foreach( Speaker s in speechers ){
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
	
    public void SkipDialog() {
        Skip = true;
    }

	//Controller Method: Trigger on direct interactions
	public bool TriggerDialog(){
        if (controllerState != State.INACTIVE)
            return false;

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

                if (speakerTrigger != null)
                    speakerTrigger.OnDialogStartNotify(current.dialogTag);

                return true;
			}
		}
		return false;
	}

	public bool TriggerDialog( string layer ){
        if (controllerState != State.INACTIVE)
            return false;

        foreach ( DialogSet d in dialogShelf.layers ){
			if( d.layer.CompareTo(layer) == 0 )
			{
				d.Load();
				d.Reset();
				currentDialogSet = d;
				PlayDialog( d.Current );

                if (speakerTrigger != null)
                    speakerTrigger.OnDialogStartNotify(current.dialogTag);

                return true;
			}
		}
		Debug.Log ("TriggerDialog - Object does not have the specified Dialog");
		return false;
	}

    public bool TriggerDialog(Speaker speaker, string dialogLayer, string dialogTag) {
        if (controllerState != State.INACTIVE)
            return false;

        speakerTarget = speaker;

        foreach (DialogSet d in dialogShelf.layers) {
            if (dialogLayer != "" && d.layer.CompareTo(dialogLayer) != 0)
                continue;

            d.Load();
            d.Reset();

            if (dialogTag != "" && !d.SetCurrent(dialogTag)) {
                continue;                
            }

            currentDialogSet = d;
            PlayDialog(d.Current);

            if (speakerTarget != null)
                speakerTarget.OnDialogStartNotify(current.dialogTag);
            if (speakerTrigger != null)
                speakerTrigger.OnDialogStartNotify(current.dialogTag);

            return true;
        }
        Debug.Log("TriggerDialog - Object does not have the specified Dialog");
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
		Speaker dialogSpeaker = null;
		switch( controllerState ){
			
			case State.STOP:
				dialogSpeaker = register[current.characterIdentifier];
                DialogBox.Instance.StartRenderText(dialogSpeaker, "");
                DialogBox.Instance.SetVisible(false);

                if (speakerTarget != null)
                    speakerTarget.OnDialogEndNotify(current.dialogTag);
                if (speakerTrigger != null)
                    speakerTrigger.OnDialogEndNotify(current.dialogTag);
                //dialogbox.GetComponent<Renderer>().enabled = false;
                //dialogbox.text.text = "";
                //speecher.face.renderer.enabled = false;
                speakerTarget = null;
                controllerState = State.INACTIVE;

			break;
			case State.PLAY:
					
				timer = Time.time;
				//Diaplay faceset and dialogbox
				dialogSpeaker = register[current.characterIdentifier];
                
                //speecher.face.renderer.enabled = true;
                //Display Dialogbox
                DialogBox.Instance.SetVisible(true);
                DialogBox.Instance.StartRenderText(dialogSpeaker, current.text);

				//dialogbox.GetComponent<Renderer>().enabled = true;
				//dialogbox.text.text = current.text;	

				try{
					onDialogStartEvent(current.dialogTag);
				}catch( System.Exception e ){
					Debug.Log(e);
				}	

				controllerState = State.WAIT;
			break;
			case State.WAIT:
				//Waiting for something happens and change the state
				if(!DialogBox.Instance.IsWaitingInput() && skipped){
                    DialogBox.Instance.RenderTextImmediatelly();
				}

                if(DialogBox.Instance.IsWaitingInput() && skipped) {
                    DialogBox.Instance.ContinueRenderText();
                }

                if(DialogBox.Instance.IsRenderFinished() && skipped) {
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
					//dialogSpeaker = register[current.characterIdentifier];
					//speecher.face.renderer.enabled = false;
                    
                    if (current.isTrigger)
                        onDialogTrigger.TriggerEvent(new ToolKitEvent(current.toTrigger));

                    try {
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
					//dialogSpeaker = register[current.characterIdentifier];
					//speecher.face.renderer.enabled = false;
                    
                    if (current.isTrigger)
                        onDialogTrigger.TriggerEvent(new ToolKitEvent(current.toTrigger));

                    try {
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
					//dialogSpeaker = register[current.characterIdentifier];
					//speecher.face.renderer.enabled = false;

					if (current.isTrigger)
                        onDialogTrigger.TriggerEvent(new ToolKitEvent(current.toTrigger));

                    try {
						onDialogEndEvent(tag);
					}catch(System.Exception e){
						Debug.Log(e);
					}
					
					controllerState = State.STOP;
				}
			break;
		}
        skipped = false;
			
	}

	//Player Method
	void Update(){

		ControllerUpdate();

	}

    public override void onTKEvent(ToolKitEvent tkEvent) {
        if(skipDialogCondition.checkConditionKey(tkEvent.condition) || 
        skipDialogCondition.checkConditionTrigger(tkEvent.condition.identifier) || 
        skipDialogCondition.checkConditionVariable()) {
            SkipDialog();
        }
    }
}
