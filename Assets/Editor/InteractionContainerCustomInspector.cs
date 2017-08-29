using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(InteractionContainer))]
public class InteractionContainerCustomInspector : Editor {

    private bool newIdentifierSelected = false;
    private Condition selectedCondition;
    private MenuData selectedMenuData;

    struct MenuData {
        public Condition c;
        public string selectedIdentifier;
        public void Update() {
            c.identifier = selectedIdentifier;
        }
    }


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

        EditorGUILayout.LabelField(new GUIContent("Action"));
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

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        GUIStyle style = new GUIStyle(GUI.skin.label);
        EditorGUILayout.LabelField(new GUIContent("[Conditions]"), style);
        EditorGUILayout.Separator();
        foreach (Condition c in interaction.conditions) {
            DrawConditionIdentifierProperty("Identifier", c);
            EditorGUILayout.Separator();
        }

        //if(newIdentifierSelected) {
        //    selectedMenuData.Update();
        //    newIdentifierSelected = false;
        //}
		
		if (GUILayout.Button ("Add Condition")) {
			interaction.conditions.Add(new Condition());
		}

        if (GUILayout.Button("Remove Last Condition") && interaction.conditions.Count > 0) {
            interaction.conditions.RemoveAt(interaction.conditions.Count - 1);
        }
    }


    public void DrawConditionIdentifierProperty(string label, Condition c) {

        //Get Title Case Lable
        string title = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(label.ToLower());
        //Create label and update the available space for the dropdown menu
        EditorGUILayout.PrefixLabel(new GUIContent(title));
        Rect position = EditorGUILayout.BeginHorizontal();



        GenericMenu menu = new GenericMenu();
        //Get ALL the conditions declared so far
        List<Condition> conditions = GameVariablesManager.Instance.getAllVariables();
        List<string> identifiers = new List<string>();

        
        foreach (Condition v in conditions) {
            //Current selected identifier
            MenuData data = new MenuData();
            data.selectedIdentifier = v.identifier;
            data.c = c;
            AddMenuItem(menu, v.identifier, data);
        }

        menu.AddSeparator("");
        //Option that heads the user to the creation of more conditions
        menu.AddItem(new GUIContent("Add Condition"), false, OnAddConditionSelected, new MenuData());

        //Show up the selection buttom for the context menu
        if (EditorGUILayout.DropdownButton(new GUIContent(c.identifier), FocusType.Passive)) {
            menu.DropDown(position);
        }

        EditorGUILayout.EndHorizontal();
        ShowConditionEditing(c);

    }

    // a method to simplify adding menu items
    void AddMenuItem(GenericMenu menu, string menuPath, MenuData data) {
        // the menu item is marked as selected if it matches the current value of m_Color
        menu.AddItem(new GUIContent(menuPath), data.c.identifier.Equals(menuPath), OnMenuItemSelected, data);
    }

    // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
    void OnMenuItemSelected(object c) {
        MenuData data = (MenuData)c;
        newIdentifierSelected = true;
        data.Update();
        //selectedMenuData = data;
    }

    void OnAddConditionSelected(object identifier) {
        Selection.activeGameObject = GameVariablesManager.Instance.gameObject;
    }

    void ShowConditionEditing(Condition c) {
        EditorGUILayout.LabelField("Condition");

        EditorGUILayout.BeginHorizontal();
        c.type = (Condition.VariableType)EditorGUILayout.EnumPopup(c.type);

        switch (c.type) {
            case Condition.VariableType.Bool: {
                    int selected = 0;
                    selected = c.BoolValue ? 1 : 0;
                    selected = EditorGUILayout.Popup(selected, (new string[] { "false", "true" }));
                    c.BoolValue = selected == 1 ? true : false;
                }
                break;
            case Condition.VariableType.Float: {
                    int selected = 0;
                    selected = c.comparison == Condition.VariableCondition.Greater ? 0 : c.comparison == Condition.VariableCondition.Lower ? 1 : 2;
                    selected = EditorGUILayout.Popup(selected, (new string[] { "greater", "less", "equal" }));
                    c.comparison = selected == 0 ? Condition.VariableCondition.Greater : selected == 1 ? Condition.VariableCondition.Lower : Condition.VariableCondition.Equal;
                    c.FloatValue = EditorGUILayout.FloatField(c.FloatValue);
                }
                break;
            case Condition.VariableType.Int: {
                    int selected = 0;
                    selected = c.comparison == Condition.VariableCondition.Greater ? 0 : c.comparison == Condition.VariableCondition.Lower ? 1 : 2;
                    selected = EditorGUILayout.Popup(selected, (new string[] { "greater", "less", "equal" }));
                    c.comparison = selected == 0 ? Condition.VariableCondition.Greater : selected == 1 ? Condition.VariableCondition.Lower : Condition.VariableCondition.Equal;
                    c.IntValue = EditorGUILayout.IntField(c.IntValue);
                }
                break;
            case Condition.VariableType.Trigger:
                break;
            case Condition.VariableType.Input:
                c.InputValue = (KeyCode)EditorGUILayout.EnumPopup(c.InputValue);
                break;
            default:
                break;
        }
        EditorGUILayout.EndHorizontal();
        GUI.FocusControl(c.identifier);
    }

}
