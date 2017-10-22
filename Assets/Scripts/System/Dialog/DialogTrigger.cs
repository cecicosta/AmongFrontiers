using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(DialogController))]
public class DialogTrigger : ToolKitEventListener {
    
    public ConditionsList conditions = new ConditionsList();
    public Speaker speaker;
    public string dialogTag;
    public string dialogLayer;
    public bool ignoreSpeaker = false;

    private DialogController dialogController;
    private bool colliding;

    public string DialogTag {
        get {
            return dialogTag;
        }

        set {
            dialogTag = value;
        }
    }

    public string DialogLayer {
        get {
            return dialogLayer;
        }

        set {
            dialogLayer = value;
        }
    }

    private void Start() {
        dialogController = GetComponent<DialogController>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (speaker.gameObject != collision.gameObject)
            return;
        colliding = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (speaker.gameObject != collision.gameObject)
            return;
        colliding = false;
    }

    public override void onTKEvent(ToolKitEvent tkEvent) {
        if (!colliding && !ignoreSpeaker)
            return;
        Debug.Log(tkEvent.condition.identifier);
        bool trigger = true;
        foreach(Condition c in conditions.conditions)
        if (!c.CheckCondition(tkEvent.condition)) {
            trigger = false;
        }
        if(trigger)
            dialogController.TriggerDialog(speaker, dialogLayer, dialogTag);
    }

    public void TriggerDialog() {
        if (!colliding && !ignoreSpeaker)
            return;

        dialogController.TriggerDialog(speaker, dialogLayer, dialogTag);
    }

    public void TriggerDialog(string dialogTag) {
        if (!colliding && !ignoreSpeaker)
            return;

        dialogController.TriggerDialog(speaker, dialogLayer, dialogTag);
    }

}
