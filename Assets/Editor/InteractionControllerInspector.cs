using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InteractionController))]
public class InteractionControllerInspector : Editor {
	
	void OnEnable(){
	}
	
	public override void OnInspectorGUI () {

		DrawDefaultInspector ();

		if (GUILayout.Button("Interaction Editor"))
            InteractionNodesEditor.ShowWindow ( (InteractionController)this.target );

	}
	// Update is called once per frame
	void Update () {
	
	}
}
