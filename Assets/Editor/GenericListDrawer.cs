using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionsList))]
public class GenericListDrawer :  PropertyDrawer{

    ConditionContainer conditionContainer;
    float size;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        if(conditionContainer == null)
            conditionContainer = (ConditionContainer)ScriptableObject.CreateInstance<ConditionContainer>();

        ConditionsList list = Supyrb.SerializedPropertyExtensions.GetValue<ConditionsList>(property);
        size = 0;

        SerializedObject serializedObject = null;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        EditorGUI.LabelField(position, new GUIContent("Conditions"), style);
        //EditorGUILayout.Separator();
        position.y += 10;

        foreach (Condition c in list.conditions) {

            position.y += 15;
            int i = list.conditions.IndexOf(c);
            conditionContainer.condition = c;

            serializedObject = new UnityEditor.SerializedObject(conditionContainer);

            SerializedProperty p = serializedObject.FindProperty("condition");

            int countChildren = p.CountInProperty();
            
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, serializedObject.FindProperty("condition"));
            //EditorHelper.DrawPropertyExclusing(serializedObject, new string[] { "m_Script" });
            serializedObject.ApplyModifiedProperties();


            size += 15 * (countChildren - 2); //magic number
            position.y += 15 * (countChildren - 3); //magic number

        }

        position.y += 15;
        size += 15;
        if (GUI.Button(new Rect(position.width - 60, position.y, 60, 15), "Add")) {
            list.conditions.Add(new Condition());
        }

        position.y += 15;
        size += 15;
        if (GUI.Button(new Rect(position.width - 60, position.y, 60, 15), "Remove") && list.conditions.Count > 0) {
            list.conditions.RemoveAt(list.conditions.Count - 1);
        }

        if (GUI.changed) {
            serializedObject.ApplyModifiedProperties();
        }

    }



    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        float extraHeight = size + 30;
        return base.GetPropertyHeight(prop, label) + extraHeight;
    }

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
