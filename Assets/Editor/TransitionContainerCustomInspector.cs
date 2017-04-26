using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TransitionContainer))]
public class TransitionContainerCustomInspector : Editor {

	void OnEnable(){
	}
	
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		Transition transition = ((TransitionContainer)target).transition;
	
		List<Variable> variables = GameVariablesManager.Instance.getAllVariables();
		List<string> variable_names = new List<string>();
		foreach (Variable v in variables) {
			variable_names.Add(v.variable);	
		}


		foreach (Condition c in transition.conditions) {
				GUILayout.Label("Condition");
				//List all variable names on a popup and do overriding if the selected variable changes
				int index = variable_names.FindIndex( x => x.Equals(c.variable) );
				index = EditorGUILayout.Popup(index, variable_names.ToArray());
				if( index < 0 )
					continue ;

				if( !c.variable.Equals( variables[index] ) ){
					c.variable = variables[index].variable;
					c.type = variables[index].type;
				}

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.EnumPopup ( c.type);
				
				switch(c.type){
					case Condition.VariableType.BOOL:{
						int selected = 0;
						selected = c.boolValue ?  1 : 0;
						selected = EditorGUILayout.Popup(selected,(new string[] { "false", "true" } ));
						c.boolValue = selected == 1 ? true: false;
						}break;
					case Condition.VariableType.FLOAT:{
						int selected = 0;
						selected = c.boolValue ?  1 : 0;
						selected = EditorGUILayout.Popup(selected,(new string[] { "greater", "less" } ));
						c.greater = selected == 1 ? true: false;
						c.floatValue = EditorGUILayout.FloatField(c.floatValue) ;
						}break;
					case Condition.VariableType.INT:{
						int selected = 0;
						selected = c.boolValue ?  1 : 0;
						selected = EditorGUILayout.Popup(selected,(new string[] { "greater", "less" } ));
						c.greater = selected == 1 ? true: false;
						c.intValue = EditorGUILayout.IntField(c.intValue) ;
					}break;
					case Condition.VariableType.TIME:
						c.timeValue = EditorGUILayout.IntField(c.timeValue) ;
						break;
					case Condition.VariableType.TRIGGER:
						break;
					case Condition.VariableType.INPUT:
						c.key = (KeyCode)EditorGUILayout.EnumPopup (c.key);
						break;
					default :
						break;
				}
				EditorGUILayout.EndHorizontal();

			}

		if (GUILayout.Button ("Add Condition")) {
			transition.conditions.Add(new Condition());
		}




	}
}
