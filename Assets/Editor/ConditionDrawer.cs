using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer {
    private string selectedIdentifier;
    private bool newIdentifierSelected = false;
    private Rect position;
    private string currentSelectedIdentifier;

    struct DataContainer {
        public SerializedProperty p;
        public string identifier;

        public DataContainer(string name, SerializedProperty p) : this() {
            this.identifier = name;
            this.p = p;
        }
    }

    void OnEnable(){
	}

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {


        //selectedIdentifier = "";
        //newIdentifierSelected = false;
        //propertyPath = "";
        //currentSelectedIdentifier = "";


        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.

        //TODO: Try an more directly access to the fields

        //EditorGUI.BeginProperty(position, label, property);
        //DrawEnumProperty(new Rect(position.x, position.y, position.width, 15), "type", property);
        //EditorGUI.EndProperty();
            EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel = 0;
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 15), new GUIContent(label));
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
        }

        //Get Title Case Lable
        string title = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(label.ToLower());
        EditorGUI.indentLevel = 2;
        //Create label and update the available space for the dropdown menu
        position = EditorGUI.PrefixLabel(position, new GUIContent(title));

        //Add the identifiers to the menu
        foreach(string name in identifiers) {
            //AddMenuItem(menu, name, new DataContainer(name, p));

            menu.AddItem(new GUIContent(name), p.FindPropertyRelative(label).Equals(name), () => {
                currentSelectedIdentifier = name;
                this.position = position;
                newIdentifierSelected = true;
            });
        }

        menu.AddSeparator("");
        //Option that heads the user to the creation of more conditions
        menu.AddItem(new GUIContent("Add Condition"), false, OnAddConditionSelected);

        selectedIdentifier = p.FindPropertyRelative(label).stringValue;

        //Show up the selection buttom for the context menu
        if (EditorGUI.DropdownButton(position, new GUIContent(selectedIdentifier), FocusType.Passive)) {
            menu.DropDown(position);
        }


        if (newIdentifierSelected && this.position == position) {
            if (p.FindPropertyRelative("identifier").stringValue != currentSelectedIdentifier) {
                Condition c = GameVariablesManager.Instance.getAllVariables().Find(x => x.identifier == currentSelectedIdentifier);
                p.FindPropertyRelative("type").intValue = (int)c.type;
                p.FindPropertyRelative("comparison").intValue = (int)c.comparison;
                p.FindPropertyRelative("IntValue").intValue = c.IntValue;
                p.FindPropertyRelative("FloatValue").floatValue = c.FloatValue;
                p.FindPropertyRelative("BoolValue").boolValue = c.BoolValue;
                p.FindPropertyRelative("InputValue").intValue = (int)c.InputValue;
                p.FindPropertyRelative("identifier").stringValue = c.identifier;
            }
            newIdentifierSelected = false;
        }

    }

    public void DrawEnumProperty(Rect position, string label, SerializedProperty p) {
        EditorGUI.indentLevel = 1;
        EditorGUI.PropertyField(position, p.FindPropertyRelative(label));
    }


    // a method to simplify adding menu items
    void AddMenuItem(GenericMenu menu, string menuPath, DataContainer data) {
        // the menu item is marked as selected if it matches the current value of m_Color
        string identifier = data.p.FindPropertyRelative("identifier").stringValue;
        menu.AddItem(new GUIContent(menuPath), data.identifier.Equals(identifier), (object d) => {
            return; }, data);
    }

    // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
    void OnMenuItemSelected(object data) {
        DataContainer pair = (DataContainer)data;
        SerializedProperty p = pair.p;
        //Current selected identifier
        if (p.FindPropertyRelative("identifier").stringValue != pair.identifier) {
            Condition c = GameVariablesManager.Instance.getAllVariables().Find(x => x.identifier == pair.identifier);
            p.FindPropertyRelative("type").intValue = (int)c.type;
            p.FindPropertyRelative("comparison").intValue = (int)c.comparison;
            p.FindPropertyRelative("IntValue").intValue = c.IntValue;
            p.FindPropertyRelative("FloatValue").floatValue = c.FloatValue;
            p.FindPropertyRelative("BoolValue").boolValue = c.BoolValue;
            p.FindPropertyRelative("InputValue").intValue = (int)c.InputValue;
            p.FindPropertyRelative("identifier").stringValue = c.identifier;
        }
    }

    void OnAddConditionSelected() {
        Selection.activeGameObject = GameVariablesManager.Instance.gameObject;
    }


    void ShowConditionEditing(Rect position, SerializedProperty p) {
        //EditorGUI.LabelField(position, "Condition");
     

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
                position.y += 15;
                EditorGUI.PropertyField(position, p.FindPropertyRelative("inputState"));
                break;
            default:
                break;
        }
        //GUI.FocusControl(c.identifier);
    }

}
