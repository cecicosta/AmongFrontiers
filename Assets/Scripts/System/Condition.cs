﻿using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Condition{

    public enum VariableType { Int, Bool, Float, Trigger, Input };
    public string identifier = "";
    public VariableType type;

    public int IntValue;
    public bool BoolValue;
    public float FloatValue;
    public KeyCode InputValue;
    public InputState inputState;

    public enum VariableCondition{ Greater, Lower, Equal };
    public enum InputState { Down, Up, Press };

	public VariableCondition comparison = VariableCondition.Equal;

	public Condition(){
		//identifier = "";
        //type = VariableType.TRIGGER;
	}

    public void Copy(Condition from) {
        IntValue = from.IntValue;
        BoolValue = from.BoolValue;
        FloatValue = from.FloatValue;
        InputValue = from.InputValue;
        type = from.type;
        identifier = from.identifier;
        inputState = from.inputState;
    }

    public void setIntValue( int value ){ IntValue = value; }
	public void setBoolValue( bool value ){ BoolValue = value; }
	public void setFloatValue( float value ){ FloatValue = value; }

	public int getIntValue() {
		return IntValue;
	}
	public bool getBoolValue() {
		return BoolValue;
	}
	public float getFloatValue() {
		return FloatValue;
	}

	public bool checkConditionVariable() {
		bool isSatisfied = false;
        Condition condition = GameVariablesManager.Instance.GetVariable(identifier);
		if(condition == null) {
            //Debug.Log("Condition identifier not found: " + identifier);
            return isSatisfied;
        }

        if (condition.identifier.CompareTo (identifier) == 0) {
			switch (condition.type) {
				case VariableType.Int:
						if (comparison == VariableCondition.Greater)
					isSatisfied = this.IntValue < condition.IntValue ;
						else if (comparison == VariableCondition.Lower)
					isSatisfied = this.IntValue > condition.IntValue;
						else
					isSatisfied = this.IntValue == condition.IntValue;
						break;
				case VariableType.Float:

						if (comparison == VariableCondition.Greater)
					isSatisfied = this.FloatValue < condition.FloatValue;
						else if (comparison == VariableCondition.Lower)
					isSatisfied = this.FloatValue > condition.FloatValue;
						else
					isSatisfied = this.FloatValue == condition.FloatValue;
						break;
				case VariableType.Bool:
					isSatisfied = this.BoolValue == condition.BoolValue;
						break;
				}
		}

		return isSatisfied;
	}

	public bool checkConditionTrigger(string name){
		bool isSatisfied = false;
		if( type == VariableType.Trigger && identifier.CompareTo(name) == 0 )
			isSatisfied = true;
		return isSatisfied;
	}

	public bool checkConditionKey(Condition condition ){
		bool isSatisfied = false;
		if (type == VariableType.Input && condition.identifier.CompareTo (identifier) == 0 && condition.inputState == inputState) {
			KeyCode value = condition.InputValue;
			isSatisfied = this.InputValue == value ;
		}
		return isSatisfied;
	}

    public bool CheckCondition(Condition condition) {
        return checkConditionVariable() || checkConditionTrigger(condition.identifier) || checkConditionKey(condition);
    }

    internal bool checkConditionKey(object skipDialogCondition) {
        throw new NotImplementedException();
    }
}

