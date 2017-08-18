using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameVariablesManager))]
public class ListEditor : Editor {

    List<Condition> values;

    string editingValue;
    string lastFocusedControl;

    void OnEnable() {
        values = GameVariablesManager.Instance.getAllVariables();
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.HelpBox("Editor to insert new game conditions to trigger events and guide alternative choices.\nPress Enter to apply field changes.", MessageType.Info);
        List<Condition> editedValues = new List<Condition>();
        Condition newValue = new Condition();

        foreach (Condition val in values) {
            newValue = val;

            if (ShowField("field " + val.identifier, ref newValue)) {
                if (string.IsNullOrEmpty(newValue.identifier))
                    continue;

                if (values.IndexOf(newValue) >= 0)
                    newValue = val;
            }

            editedValues.Add(newValue);
        }

        newValue.identifier = "";

        if (ShowField("new field", ref newValue)) {
            if (!string.IsNullOrEmpty(newValue.identifier) || values.IndexOf(newValue) < 0) //check ||
                editedValues.Add(newValue);
        }

        values = editedValues;
    }

    bool ShowField(string name, ref Condition val) {
        GUI.SetNextControlName(name);

        if (GUI.GetNameOfFocusedControl() != name) {

            if (Event.current.type == EventType.Repaint && string.IsNullOrEmpty(val.identifier)) { //check &&
                GUIStyle style = new GUIStyle(GUI.skin.textField);
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
                EditorGUILayout.TextField("Enter a new item", style);
            }
            else
                EditorGUILayout.TextField(val.identifier);

            return false;
        }

        //Debug.Log("Focusing " + GUI.GetNameOfFocusedControl());   // Uncomment to show which control has focus.

        if (lastFocusedControl != name) {
            lastFocusedControl = name;
            editingValue = val.identifier;
        }

        bool applying = false;

        if (Event.current.isKey) {
            switch (Event.current.keyCode) {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    val.identifier = editingValue;
                    applying = true;
                    Event.current.Use();    // Ignore event, otherwise there will be control name conflicts!
                    break;
            }
        }

        editingValue = EditorGUILayout.TextField(editingValue);
        return applying;
    }
}