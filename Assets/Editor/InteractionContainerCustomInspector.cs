using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(InteractionContainer))]
public class InteractionContainerCustomInspector : Editor {

	void OnEnable(){
	}

	public override void OnInspectorGUI () {
		Interaction interaction = ((InteractionContainer)target).interaction;

		/*EditorGUI.BeginChangeCheck ();
		InteractionController previous = interaction.interactionController;
		interaction.interactionController = 
			(InteractionController)EditorGUILayout.ObjectField (interaction.interactionController, 
			                                                    typeof(InteractionController), true);
		if (EditorGUI.EndChangeCheck ()) {
			previous.Interactions.Remove(interaction);
			interaction.interactionController.Interactions.Add(interaction);
			//TODO: Change reference on the Transitions
		}*/

		//1. get all loaded actions from manager; 2. obtain the object current action
		//3. create a list with the actions name 4. create the popup controller 
		//5. do the necessary changes in case the selected action change
		List<ToolKitAction> actions = ActionManager.Instance.Actions;
		int index = actions.IndexOf( interaction.tkAction );
		List<string> action_names = new List<string> ();
		foreach (ToolKitAction a in actions)
						action_names.Add (a.action);
		index = EditorGUILayout.Popup (index, action_names.ToArray ());
		if (interaction.tkAction == null && index > -1) {
						interaction.tkAction = actions [index];
				}else if ( interaction.tkAction != null && !interaction.tkAction.action.Equals (action_names [index])) {
					interaction.tkAction = actions[index];
				}



		List<Condition> variables = GameVariablesManager.Instance.getAllVariables();
		List<string> variable_names = new List<string>();
		foreach (Condition v in variables) {
			variable_names.Add(v.identifier);	
		}
		
		
		foreach (Condition c in interaction.conditions) {
			EditorGUILayout.LabelField("Condition");
			//List all variable names on a popup and do overriding if the selected variable changes
			index = variable_names.FindIndex( x => x.Equals(c.identifier) );
			index = EditorGUILayout.Popup(index, variable_names.ToArray());
			if( index < 0 )
				continue ;
			
			if( !c.identifier.Equals( variables[index] ) ){
				c.identifier = variables[index].identifier;
				c.type = variables[index].type;
			}
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EnumPopup ( c.type);
			
			switch(c.type){
			case Condition.VariableType.BOOL:{
				int selected = 0;
				selected = c.BoolValue ?  1 : 0;
				selected = EditorGUILayout.Popup(selected,(new string[] { "false", "true" } ));
				c.BoolValue = selected == 1 ? true: false;
			}break;
			case Condition.VariableType.FLOAT:{
				int selected = 0;
				selected = c.variableCondition == Condition.VariableCondition.GREATER ?  0 : c.variableCondition == Condition.VariableCondition.LOWER ? 1 : 2;
				selected = EditorGUILayout.Popup(selected,(new string[] { "greater", "less", "equal" } ));
				c.variableCondition = selected == 0 ? Condition.VariableCondition.GREATER : selected == 1? Condition.VariableCondition.LOWER : Condition.VariableCondition.EQUAL;
				c.FloatValue = EditorGUILayout.FloatField(c.FloatValue) ;
			}break;
			case Condition.VariableType.INT:{
				int selected = 0;
				selected = c.variableCondition == Condition.VariableCondition.GREATER ?  0 : c.variableCondition == Condition.VariableCondition.LOWER ? 1 : 2;
				selected = EditorGUILayout.Popup(selected,(new string[] { "greater", "less", "equal" } ));
				c.variableCondition = selected == 0 ? Condition.VariableCondition.GREATER : selected == 1? Condition.VariableCondition.LOWER : Condition.VariableCondition.EQUAL;
				c.IntValue = EditorGUILayout.IntField(c.IntValue) ;
			}break;
			case Condition.VariableType.TRIGGER:
				break;
			case Condition.VariableType.INPUT:
				c.InputValue = (KeyCode)EditorGUILayout.EnumPopup (c.InputValue);
				break;
			default :
				break;
			}
			EditorGUILayout.EndHorizontal();
		}
		
		if (GUILayout.Button ("Add Condition")) {
			interaction.conditions.Add(new Condition());
		}

	}


}
