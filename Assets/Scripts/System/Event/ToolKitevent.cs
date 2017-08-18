using UnityEngine;
using System.Collections;

[System.Serializable]
public class ToolKitEvent {

	public enum EventType{ VARIABLE_CHANGE, INPUT };
	public EventType type = EventType.VARIABLE_CHANGE;
	public Condition condition = new Condition();    
}
