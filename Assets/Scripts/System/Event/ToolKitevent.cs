using UnityEngine;
using System.Collections;

[System.Serializable]
public class ToolKitEvent {

	public enum EventType{ ConditionUpdate, Input };
	public EventType type = EventType.ConditionUpdate;
	public Condition condition = new Condition();    
}
