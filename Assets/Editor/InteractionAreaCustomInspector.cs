using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Condition))]
public class InteractionAreaCustomInspector : PropertyDrawer {
	int index = 0;
    private bool pressed = false;
    Condition placeholder;
    private string selectedIdentifier;
    private bool newIdentifierSelected = false;

    void OnEnable(){
	}

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.

        //TODO: Try an more directly access to the fields

        //EditorGUI.BeginProperty(position, label, property);
        //DrawEnumProperty(new Rect(position.x, position.y, position.width, 15), "type", property);
        //EditorGUI.EndProperty();
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel = 0;
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 15), new GUIContent("Condition"));
        EditorGUI.EndProperty();

        int inc = 15;        
        EditorGUI.BeginProperty(position, label, property);
        DrawConditionIdentifierProperty(new Rect(position.x, position.y + inc, position.width, 15), "identifier", property);
        EditorGUI.EndProperty();

        inc += 15;
        EditorGUI.BeginProperty(position, label, property);
        ShowConditionEditing(new Rect(position.x, position.y + inc, position.width, 15), property);
        EditorGUI.EndProperty();

    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        float extraHeight = 60.0f;
        return base.GetPropertyHeight(prop, label) + extraHeight;
    }

    public void DrawConditionIdentifierProperty(Rect position, string label, SerializedProperty p) {

        GenericMenu menu = new GenericMenu();

        //Get ALL the conditions declared so far
        List<Condition> conditions = GameVariablesManager.Instance.getAllVariables();
        List<string> identifiers = new List<string>();
        foreach (Condition v in conditions) {
            identifiers.Add(v.identifier);
            Debug.Log(v.identifier);
        }

        //Get Title Case Lable
        string title = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(label.ToLower());
        EditorGUI.indentLevel = 2;
        //Create label and update the available space for the dropdown menu
        position = EditorGUI.PrefixLabel(position, new GUIContent(title));

        //Current selected identifier
        selectedIdentifier = newIdentifierSelected? selectedIdentifier: p.FindPropertyRelative(label).stringValue;

        //Add the identifiers to the menu
        foreach(string name in identifiers) {
            AddMenuItem(menu, name, name);
        }
        menu.AddSeparator("");
        //Option that heads the user to the creation of more conditions
        menu.AddItem(new GUIContent("Add Condition"), false, OnAddConditionSelected, selectedIdentifier);

        //Show up the selection buttom for the context menu
        if (EditorGUI.DropdownButton(position, new GUIContent(selectedIdentifier), FocusType.Passive)) {
            menu.DropDown(position);
        }

        p.FindPropertyRelative(label).stringValue = selectedIdentifier;
    }

    public void DrawEnumProperty(Rect position, string label, SerializedProperty p) {
        EditorGUI.indentLevel = 1;
        EditorGUI.PropertyField(position, p.FindPropertyRelative(label));
    }


    // a method to simplify adding menu items
    void AddMenuItem(GenericMenu menu, string menuPath, string identifier) {
        // the menu item is marked as selected if it matches the current value of m_Color
        menu.AddItem(new GUIContent(menuPath), selectedIdentifier.Equals(identifier), OnMenuItemSelected, identifier);
    }

    // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
    void OnMenuItemSelected(object identifier) {
        selectedIdentifier = (string)identifier;
        newIdentifierSelected = true;
    }

    void OnAddConditionSelected(object identifier) {
        Selection.activeGameObject = GameVariablesManager.Instance.gameObject;
    }


    void ShowConditionEditing(Rect position, SerializedProperty p) {
        //EditorGUI.LabelField(position, "Condition");
        Rect space = new Rect();

        //c.type = (Condition.VariableType)EditorGUI.EnumPopup(position, c.type);
        EditorGUI.PropertyField(position, p.FindPropertyRelative("type"));

        position.y += 15;
        int type = p.FindPropertyRelative("type").intValue;
        switch ((Condition.VariableType)type) {
            case Condition.VariableType.Bool: {
                    EditorGUI.PropertyField(position, p.FindPropertyRelative("BoolValue"));
                }
                break;
            case Condition.VariableType.Float: {
                    EditorGUI.PropertyField(position, p.FindPropertyRelative("comparison"));
                    position.y += 15;
                    EditorGUI.PropertyField(position, p.FindPropertyRelative("IntValue"));
                }
                break;
            case Condition.VariableType.Int: {
                    EditorGUI.PropertyField(position, p.FindPropertyRelative("comparison"));
                    position.y += 15;
                    EditorGUI.PropertyField(position, p.FindPropertyRelative("IntValue"));
                }
                break;
            case Condition.VariableType.Trigger:
                break;
            case Condition.VariableType.Input:
                EditorGUI.PropertyField(position, p.FindPropertyRelative("InputValue"));
                break;
            default:
                break;
        }
        //GUI.FocusControl(c.identifier);
    }

}
