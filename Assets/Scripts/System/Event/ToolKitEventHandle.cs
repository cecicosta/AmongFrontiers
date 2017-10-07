using UnityEngine;
using System.Collections;

public class ToolKitEventHandle : Singleton<ToolKitEventHandle> {

	public delegate void OnTKEvent(ToolKitEvent tkEvent);
	public event OnTKEvent onEvent;
	// Use this for initialization
	void Awake () {
		ToolKitEventListener[] eventListener = FindObjectsOfType<ToolKitEventListener>();
		foreach (ToolKitEventListener tkEventListener in eventListener) {
			onEvent += tkEventListener.onTKEvent;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //If a event is trigged, emmit event (seems only a additional step)
	public void OnEventTrigger(ToolKitEvent tkEvent){
		onEvent (tkEvent);
	}


}
