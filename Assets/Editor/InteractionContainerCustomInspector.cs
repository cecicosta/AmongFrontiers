using UnityEditor;
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



	}


}
