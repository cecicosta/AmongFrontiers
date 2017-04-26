using UnityEngine;
using System.Collections;

public class ToolKitEvent {

	public enum EventType{ VARIABLE_CHANGE, INPUT };
	public EventType type;
	Condition variable;
}
