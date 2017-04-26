using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionMonitor : MonoBehaviour {

	private Animator animator;
	private DialogController dialogController;
	
	// Event Handler: Used to notify conditions triggered by finished dialogs
	public delegate void OnActionNotify(string tag);
	//public static event OnActionNotify onActionNotifyEvent;

//	UnityEditorInternal.AnimatorController ac;

	// Use this for initialization
	void Start () {
	
		//LISTEN TO DIALOGS TO NOTIFY THE SCENE MANAGER
		dialogController = GetComponent<DialogController>();
		if( dialogController != null ){/*
			dialogController.onDialogStartEvent += OnDialogStartNotify;
			dialogController.onDialogEndEvent += OnDialogEndNotify;*/
		}
		/*
		Motion motion= null;
		//Obtain a List of All Animations From the Animator
		animator = GetComponent<Animator>();
		if( animator != null ){
			ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
			int layerCount = ac.layerCount;
			for (int layer = 0; layer < layerCount; layer++) {
				UnityEditorInternal.StateMachine sm = ac.GetLayer(layer).stateMachine;
				for( int i=0; i<sm.stateCount; i++ ){
					UnityEditorInternal.State s = sm.GetState(i);

					if(s.GetMotion() != null){
						motion = s.GetMotion();
					}
				}
			}
		}
	*/
		//animation.AddClip(motion, motion.name);
		//animation.Play("JonnyStanding");
	}
	
	// Update is called once per frame
	void Update () {
		//animator.enabled = false;
	}
	/*

	//Execute animation request
	public void OnAnimationRequest(string tag){
		for(int i=0; i< animator.layerCount; i++)
			animator.Play(tag, i);
		//animation.Play(tag);
	}
	//Execute dialog request
	public void OnDialogRequest(string tag){
		dialogController.TriggerDialog(tag);
	}

	//Notify of dialog event
	public void OnDialogStartNotify( string tag ){
		//onActionNotifyEvent(tag);
	}

	//Notify of dialog event
	public void OnDialogEndNotify( string tag ){
		onActionNotifyEvent(tag);
	}

	//Use manly to notify of animation event
	public void OnNotify(string tag){
		onActionNotifyEvent(tag);
	}
	*/
}
