using UnityEngine;
using System.Collections;

[System.Serializable]
public class ToolKitAction: ScriptableObject {

	public string action;

	void OnEnable(){
		if (action == null)
						action = "";
	}
	public virtual void Execute( GameObject gameObject ){}
	public virtual bool isStarted(){
		return false;
	}
	public virtual bool isFinished(){
		return false;
	}


}
