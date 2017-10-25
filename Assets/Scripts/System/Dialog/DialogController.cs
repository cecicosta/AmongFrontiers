using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DialogController : ToolKitEventListener {
	enum State {STOP, PLAY, WAIT, QUERY ,SELECT, STANDBY, INACTIVE };

	//Controller States: Keep all controllers syncronized
	private State controllerState = State.INACTIVE;

	private static bool registerSpeachers = false;
	public static Dictionary<string, Speaker> register = new Dictionary<string, Speaker>();

	//Controller atributes
	public DialogShelf dialogShelf;

    public Speaker speakerTrigger;

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
    public bool overrideDialog;

    public  bool Skip{
		set{ skipped = value; }
	}

    public bool OverrideDialog {
        get {
            return overrideDialog;
        }

        set {
            overrideDialog = value;
        }
    }

    void Start(){
        onDialogTrigger = new ToolKitEventTrigger();

        //TODO: Transfer this reference to an manager
        register.Clear();
		Speaker[] speechers = Resources.FindObjectsOfTypeAll<Speaker>();
		foreach( Speaker s in speechers ){
            try {
                register.Add(s.identifier, s);
            }catch(System.Exception e) {
                Debug.Log(e.Message + s.identifier);
            }

		}
		registerSpeachers = true;
		
        //Listen for the buttom click events
        DialogBox.Instance.onOptionChoose += OnSelectedOption;
	}

	//Controller Method: Listen the buttons to choose one dialog option
	void OnSelectedOption(int id){
		if( controllerState != State.STANDBY )
			return;
        SelectOption(current.buttonOrder.IndexOf(id));
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
        if (controllerState != State.INACTIVE && !overrideDialog)
            return false;

        if (overrideDialog)
            DialogBox.Instance.StopCurrentRendering();

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
        if (controllerState != State.INACTIVE && !overrideDialog)
            return false;

        if (overrideDialog)
            DialogBox.Instance.StopCurrentRendering();

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
        if (controllerState != State.INACTIVE && !overrideDialog)
            return false;

        if (overrideDialog)
            DialogBox.Instance.StopCurrentRendering();

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

                speakerTarget = null;
                controllerState = State.INACTIVE;

			break;
			case State.PLAY:
					
				timer = Time.time;
				//Diaplay faceset and dialogbox
				dialogSpeaker = register[current.characterIdentifier];
                
                //Display Dialogbox
                DialogBox.Instance.SetVisible(true);
                DialogBox.Instance.StartRenderText(dialogSpeaker, current.text);
                

                if(current.text != "")
				    controllerState = State.WAIT;
                else
                    controllerState = State.QUERY;
            break;
			case State.WAIT:
                //Waiting for something happens and change the state
                if (DialogBox.Instance.IsRenderFinished()) {
                    controllerState = State.QUERY;
                }else if (!DialogBox.Instance.IsWaitingInput() && skipped){
                    DialogBox.Instance.RenderTextImmediatelly();
				}else if(DialogBox.Instance.IsWaitingInput() && skipped) {
                    DialogBox.Instance.ContinueRenderText();
                }
			break;
			case State.STANDBY:
			break;
			case State.QUERY:
                //Make the selection beetween dialog options 
				if( current.query.Count > 1 || (current.query.Count == 1 && current.showSingleOption)){
                    
                    //Temporary fix for compatibility of the button selection functionality
                    while (current.buttonOrder.Count < current.query.Count) {
                        current.buttonOrder.Add(current.buttonOrder.Count);
                    }
                    
                    for ( int i=0; i< current.query.Count; i++ ){
                        DialogBox.Instance.ShowOption(current.buttonOrder[i], current.query[i]);
					}
					controllerState = State.STANDBY;
				}else if(skipped)
					controllerState = State.SELECT;

			break;
			case State.SELECT:
				if( current.query.Count > 1 || (current.query.Count == 1 && current.showSingleOption)) {

                    DialogBox.Instance.HideAllOptions();

                    if (current.isTrigger)
                        onDialogTrigger.TriggerEvent(new ToolKitEvent(current.toTrigger));

					currentDialogSet.MoveNext(selected);
					current = currentDialogSet.Current;
					if(current.updateRoot)
						currentDialogSet.Root = current;
					controllerState = State.PLAY;
				}else if(current.query.Count == 1) {

                    DialogBox.Instance.HideAllOptions();

                    if (current.isTrigger)
                        onDialogTrigger.TriggerEvent(new ToolKitEvent(current.toTrigger));

                    currentDialogSet.MoveNext();
                    current = currentDialogSet.Current;
                    if (current.updateRoot)
                        currentDialogSet.Root = current;
                    controllerState = State.PLAY;
                }
                else{

					if (current.isTrigger)
                        onDialogTrigger.TriggerEvent(new ToolKitEvent(current.toTrigger));
					
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
