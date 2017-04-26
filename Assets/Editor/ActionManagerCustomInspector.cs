using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ActionManager))]
public class ActionManagerCustomInspector : Editor {
	int index = 0;
	void OnEnable(){
	}
	
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		ActionManager actManager = (ActionManager)target;
		index = EditorGUILayout.Popup (index, actManager.AvaiableActions.ToArray());

		if (GUILayout.Button ("Create Action")) {
			ToolKitAction action = actManager.CreateActionAsset(actManager.AvaiableActions[index]);
			ActionWindowEditor.Init(action);
		}

	}

}
