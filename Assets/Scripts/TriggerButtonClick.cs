using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(UnityEngine.UI.Button))]
public class TriggerButtonClick : ToolKitEventListener {
    public Condition condition;

    UnityEngine.UI.Button button;
    private bool triggered;

    // Use this for initialization
    void Start () {
        button = GetComponent<UnityEngine.UI.Button>();
	}
	
	// Update is called once per frame
	void Update () {
        if(triggered)
            button.onClick.Invoke();
        triggered = false;
        
    }

    private void OnEnable() {
        triggered = false;
    }

    private void OnDisable() {
        triggered = false;
    }

    public override void onTKEvent(ToolKitEvent tkEvent) {
        triggered = condition.CheckCondition(tkEvent.condition);
    }
}
