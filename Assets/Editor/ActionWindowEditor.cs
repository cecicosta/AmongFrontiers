using UnityEngine;
using UnityEditor;
using System.Collections;

public class ActionWindowEditor : EditorWindow {

	public InteractionContainer selected;
	public static void Init ( InteractionContainer action) 
	{
		// Get existing open window or if none, make a new one:
		ActionWindowEditor window = (ActionWindowEditor)EditorWindow.GetWindow (typeof (ActionWindowEditor));
		window.selected = action;
	}
	
	void OnGUI () {
		
		if (selected != null) { 
		

            //AssetDatabase.OpenAsset(selected);
			var editor = Editor.CreateEditor(selected);
			editor.OnInspectorGUI();

            SerializedObject actionObj = new UnityEditor.SerializedObject(selected);
            SerializedProperty prop = actionObj.GetIterator();
            while (prop.NextVisible(true)) {
                EditorGUILayout.PropertyField(prop, true);
            }

            actionObj.ApplyModifiedProperties();
            
        }

    }
}
