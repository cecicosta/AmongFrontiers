using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInitialConditionsAtStart : MonoBehaviour {

    public Condition condition = new Condition();
    public bool persists = false;
    ToolKitEventTrigger trigger;
	// Use this for initialization
	void Start () {
        StartCoroutine(Trigger());
	}

    IEnumerator Trigger() {
        yield return new WaitForEndOfFrame();
                trigger = new ToolKitEventTrigger();
        if (persists)
            GameVariablesManager.Instance.PersistsCondition(condition);
        else
            GameVariablesManager.Instance.ChangeConditionValue(condition);
        trigger.TriggerEvent(new ToolKitEvent(condition));
    }
}
