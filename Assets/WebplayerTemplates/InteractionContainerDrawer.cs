using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(InteractionContainer))]
public class InteractionContainerDrawer : PropertyDrawer {
    private string selectedIdentifier;
    private bool newIdentifierSelected = false;
    private Rect position;
    private string currentSelectedIdentifier;


    void OnEnable(){
	}

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel = 0;
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 15), new GUIContent(label));
        EditorGUI.EndProperty();
        
        //EditorGUILayout.PropertyField(property.FindPropertyRelative("tkAction.onInteractionTrigged"), true);
        SerializedProperty prop = property.Copy();
        while (prop.NextVisible(true)) {
            EditorGUILayout.PropertyField(prop, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        float extraHeight = 60.0f;
        return base.GetPropertyHeight(prop, label) + extraHeight;
    }
    
    

}
