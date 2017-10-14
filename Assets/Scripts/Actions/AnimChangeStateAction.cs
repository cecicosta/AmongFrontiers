using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimChangeStateAction : ToolKitAction {
	public enum ConditionType{INT, BOOL, FLOAT, TRIGGER};
	public string condition;
	public ConditionType type; 

	private int intValue;
	private bool boolValue;
	private float floatValue;

	public override void Execute( GameObject animator ){}
    
    public override bool isStarted(){
		return true;
	}
	public override bool isFinished(){
		return true;
	}

	//TODO: Exhibit dynamically on editor/inspector the value selector, according to the	
	//condition type chosen by the user


}