using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogController))]
public class DialogControllerInspector : Editor {

    void OnEnable() {
    }

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        if (GUILayout.Button("Dialog Editor"))
            DialogEditor.ShowWindow();

    }
    // Update is called once per frame
    void Update() {

    }
}
