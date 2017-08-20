using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[System.Serializable]
public class Condition{

    public enum VariableType { INT, BOOL, FLOAT, TRIGGER, INPUT };
    public string identifier = "";
    public VariableType type;

    public int IntValue;
    public bool BoolValue;
    public float FloatValue;
    public KeyCode InputValue;

    public enum VariableCondition{ GREATER, LOWER, EQUAL };

	public VariableCondition variableCondition = VariableCondition.EQUAL;

	public Condition(){
		identifier = "";
        type = VariableType.TRIGGER;
	}

    public void setIntValue( int value ){ IntValue = value; }
	public void setBoolValue( bool value ){ BoolValue = value; }
	public void setFloatValue( float value ){ FloatValue = value; }

	public int getIntValue( ){
		return IntValue;
	}
	public bool getBoolValue( ){
		return BoolValue;
	}
	public float getFloatValue( ){
		return FloatValue;
	}

	public bool checkConditionVariable( ){
		bool isSatisfied = false;
        Condition condition = GameVariablesManager.Instance.GetVariable(identifier);
		if (condition.identifier.CompareTo (identifier) == 0) {
			switch (condition.type) {
				case VariableType.INT:
						if (variableCondition == VariableCondition.GREATER)
					isSatisfied = this.IntValue < condition.IntValue ;
						else if (variableCondition == VariableCondition.LOWER)
					isSatisfied = this.IntValue > condition.IntValue;
						else
					isSatisfied = this.IntValue == condition.IntValue;
						break;
				case VariableType.FLOAT:

						if (variableCondition == VariableCondition.GREATER)
					isSatisfied = this.FloatValue < condition.FloatValue;
						else if (variableCondition == VariableCondition.LOWER)
					isSatisfied = this.FloatValue > condition.FloatValue;
						else
					isSatisfied = this.FloatValue == condition.FloatValue;
						break;
				case VariableType.BOOL:
					isSatisfied = this.BoolValue == condition.BoolValue;
						break;
				}
		}

		return isSatisfied;
	}

	public bool checkConditionTrigger(string name){
		bool isSatisfied = false;
		if( type == VariableType.TRIGGER && identifier.CompareTo(name) == 0 )
			isSatisfied = true;
		return isSatisfied;
	}

	public bool checkConditionKey(Condition condition ){
		bool isSatisfied = false;
		if (type == VariableType.INPUT && condition.identifier.CompareTo (identifier) == 0) {
			KeyCode value = condition.InputValue;
			isSatisfied = this.InputValue == value ;
		}
		return isSatisfied;
	}

}

