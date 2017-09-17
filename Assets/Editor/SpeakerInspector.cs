using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(Speaker))]
public class SpeakerInspector : PropertyDrawer {

    private Dictionary<string, Speaker> register = new Dictionary<string, Speaker>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        RegisterSpeechers();

        Speaker speaker = Supyrb.SerializedPropertyExtensions.GetValue<Speaker>(property); 
        int index = 0;
        bool find = false;
        List<string> identifiers = new List<string>();
        //Obtain all character's identifier and update if necessary the current dialog character's identifier 
        foreach (string id in register.Keys) {
            identifiers.Add(id);
            if (speaker != null && speaker.identifier.CompareTo(id) == 0) {
                find = true;
            }
            if (!find)
                index++;
        }
        //If find the reference, update if necessary. Otherwise, keep the old reference and show a empty option, but update if the reference value change
        if (find) {
            index = EditorGUI.Popup(position, "Speaker", index, identifiers.ToArray());
            Supyrb.SerializedPropertyExtensions.SetValue<Speaker>(property, register[identifiers[index]]);
        } else {
            identifiers.Add("");
            index = EditorGUI.Popup(position, "Speaker", identifiers.Count - 1, identifiers.ToArray());
            if (identifiers[index].CompareTo("") != 0)
                Supyrb.SerializedPropertyExtensions.SetValue<Speaker>(property, register[identifiers[index]]);
        }
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        float extraHeight = 0f;
        return base.GetPropertyHeight(prop, label) + extraHeight;
    }

    public void RegisterSpeechers() {
        register.Clear();
        Speaker[] speechers = (Speaker[])GameObject.FindObjectsOfType(typeof(Speaker));
        foreach (Speaker s in speechers) {
            if (!register.ContainsKey(s.identifier))
                register.Add(s.identifier, s);
        }
    }
}
