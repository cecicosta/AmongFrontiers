using UnityEngine;
using System.Collections;

[System.Serializable]
public class Variable{
	public enum VariableType{INT, BOOL, FLOAT, TIME, TRIGGER, INPUT};
	public string variable = "";
	public VariableType type;
}

[System.Serializable]
public class Condition: Variable{
	public int intValue;
	public int timeValue;
	public bool boolValue;
	public float floatValue;
	public KeyCode key;

	public bool greater;

	public void setIntValue( int value ){}
	public void setTimeValue( int value ){}
	public void setBoolValue( bool value ){}
	public void setFloatValue( float value ){}

	public Condition(){
		variable = "";
	}

	public int getIntValue( ){
		return intValue;
	}
	public int getTimeValue( ){
		return timeValue;
	}
	public bool getBoolValue( ){
		return boolValue;
	}
	public float getFloatValue( ){
		return floatValue;
	}
}

