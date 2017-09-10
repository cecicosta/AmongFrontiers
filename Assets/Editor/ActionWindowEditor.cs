using UnityEngine;
using UnityEditor;
using System.Collections;

public class ActionWindowEditor : EditorWindow {

	public ToolKitAction selected;
	public static void Init ( ToolKitAction action) 
	{
		// Get existing open window or if none, make a new one:
		ActionWindowEditor window = (ActionWindowEditor)EditorWindow.GetWindow (typeof (ActionWindowEditor));
		window.selected = action;
	}
	
	void OnGUI () {
		
		if (selected != null)
		{
			//var editor = Editor.CreateEditor(selected);
			//editor.OnInspectorGUI();     
			//if (GUILayout.Button ("Save")) {
			//	ActionManager.Instance.SaveActionAsset(selected);
			//}
		}
		
	}
}
