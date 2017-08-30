using UnityEngine;
using System.Collections;

[System.Serializable]
public class ToolKitEvent {
	public Condition condition = new Condition();

    public ToolKitEvent() {

    }

    public ToolKitEvent(Condition c) {
        condition = c;
    }

}
