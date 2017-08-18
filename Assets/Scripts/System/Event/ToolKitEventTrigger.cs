using UnityEngine;
using System.Collections;

public class ToolKitEventTrigger {

	public delegate void OnTKEventTrigger(ToolKitEvent tkEvent);
	public event OnTKEventTrigger onEventTriger;

	public ToolKitEventTrigger(){
		onEventTriger += ToolKitEventHandle.Instance.OnEventTrigger;
	}

	public void TriggerEvent(ToolKitEvent tkEvent){
		onEventTriger (tkEvent);
	}

}