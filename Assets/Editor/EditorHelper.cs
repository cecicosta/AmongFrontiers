using UnityEditor;

public class EditorHelper : Editor {

    public static void DrawPropertyExclusing(SerializedObject obj, string[] exclude) {
        Editor.DrawPropertiesExcluding(obj, exclude);
    }

}
