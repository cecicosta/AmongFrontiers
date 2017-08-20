using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameVariablesManager))]
public class ConditionListCustomEditor : Editor {

    List<Condition> values;

    string editingValue;
    string lastFocusedControl;

    void OnEnable() {
        values = GameVariablesManager.Instance.GetSceneConditions();
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.HelpBox("Editor to insert new game conditions to trigger events and guide alternative choices.\nPress Enter to apply field changes.", MessageType.Info);
        List<Condition> editedValues = new List<Condition>();
        Condition newValue = new Condition();


        foreach (Condition val in values) {
            newValue = val;
            string identifier = newValue.identifier;

            if (ShowField(val.identifier, ref newValue)) {
                //Do not allow empty identifiers
                if (newValue == null || newValue.identifier == "")
                    continue;

                //Check if there is any other except this one with the same identifier
                if (values.FindAll(x => x.identifier == newValue.identifier).Count > 1)
                    newValue.identifier = identifier; //restaure previous identifier
            }

            editedValues.Add(newValue);
        }

        newValue = new Condition();

        //Insert additional space for adding new condition
        if (ShowField("new field", ref newValue)) {
            if (newValue != null && newValue.identifier != "" && values.FindIndex(x => x.identifier == newValue.identifier) < 0)
                editedValues.Add(newValue);
        }

        
        Rect pos = EditorGUILayout.BeginHorizontal();
        
        if (GUI.Button(new Rect(pos.x + pos.width-20, pos.y, 20,20), new GUIContent("-", "Remove condition"))) {
            string name = GUI.GetNameOfFocusedControl();
            int index = editedValues.FindLastIndex(x => x.identifier == name);
            if(index >= 0) editedValues.RemoveAt(index);
            GUI.FocusControl("");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        newValue = editedValues.Find(x => x.identifier == GUI.GetNameOfFocusedControl());
        if (newValue != null){
            ShowConditionEditing(newValue);
        }


        values.Clear();
        values.AddRange(editedValues);

    }


    void ShowConditionEditing(Condition c) {
        EditorGUILayout.LabelField("Condition");
        
        EditorGUILayout.BeginHorizontal();
        c.type = (Condition.VariableType) EditorGUILayout.EnumPopup(c.type);

        switch (c.type) {
            case Condition.VariableType.BOOL: {
                    int selected = 0;
                    selected = c.BoolValue ? 1 : 0;
                    selected = EditorGUILayout.Popup(selected, (new string[] { "false", "true" }));
                    c.BoolValue = selected == 1 ? true : false;
                }
                break;
            case Condition.VariableType.FLOAT: {
                    int selected = 0;
                    selected = c.variableCondition == Condition.VariableCondition.GREATER ? 0 : c.variableCondition == Condition.VariableCondition.LOWER ? 1 : 2;
                    selected = EditorGUILayout.Popup(selected, (new string[] { "greater", "less", "equal" }));
                    c.variableCondition = selected == 0 ? Condition.VariableCondition.GREATER : selected == 1 ? Condition.VariableCondition.LOWER : Condition.VariableCondition.EQUAL;
                    c.FloatValue = EditorGUILayout.FloatField(c.FloatValue);
                }
                break;
            case Condition.VariableType.INT: {
                    int selected = 0;
                    selected = c.variableCondition == Condition.VariableCondition.GREATER ? 0 : c.variableCondition == Condition.VariableCondition.LOWER ? 1 : 2;
                    selected = EditorGUILayout.Popup(selected, (new string[] { "greater", "less", "equal" }));
                    c.variableCondition = selected == 0 ? Condition.VariableCondition.GREATER : selected == 1 ? Condition.VariableCondition.LOWER : Condition.VariableCondition.EQUAL;
                    c.IntValue = EditorGUILayout.IntField(c.IntValue);
                }
                break;
            case Condition.VariableType.TRIGGER:
                break;
            case Condition.VariableType.INPUT:
                c.InputValue = (KeyCode)EditorGUILayout.EnumPopup(c.InputValue);
                break;
            default:
                break;
        }
        EditorGUILayout.EndHorizontal();
        GUI.FocusControl(c.identifier);
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

        Debug.Log("Focusing " + GUI.GetNameOfFocusedControl());   // Uncomment to show which control has focus.

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