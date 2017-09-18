using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class ToolKitAction: ScriptableObject {

    private string action = "";

    public string Action {
        get {
            return action;
        }

        set {
            action = value;
        }
    }

    public virtual void Execute(GameObject gameObject) { }
    public virtual void Execute(GameObject colliding, GameObject gameObject) { }
    public virtual bool isStarted() { return false;  }
    public virtual bool isFinished() { return false; }

    
}
