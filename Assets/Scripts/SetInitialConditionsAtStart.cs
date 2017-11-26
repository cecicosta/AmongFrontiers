using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInitialConditionsAtStart : MonoBehaviour {

    public Condition condition = new Condition();
    ToolKitEventTrigger trigger;
    public bool persistInSession = false;

    // Use this for initialization
    void Start () {
        StartCoroutine(Trigger());
	}

    IEnumerator Trigger() {
        yield return new WaitForEndOfFrame();
                trigger = new ToolKitEventTrigger();

        if (persistInSession) {
            GameVariablesManager.Instance.PersistConsitionInSession(condition);
        }else
            GameVariablesManager.Instance.ChangeConditionValue(condition);
        trigger.TriggerEvent(new ToolKitEvent(condition));
    }
}
