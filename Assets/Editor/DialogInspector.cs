using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Dialog))]
public class DialogInspector : Editor {

	private Dictionary<string, Speaker>  register = new Dictionary<string, Speaker>();

	void OnEnable(){
		RegisterSpeechers();
	}

	public override void OnInspectorGUI () {
		Dialog dialog = (Dialog)target;
	
		//TEXT AREA FOR TRIGGER TAG
		dialog.dialogTag = EditorGUILayout.TextField("Activate Flag", dialog.dialogTag);

		//
		dialog.updateRoot = EditorGUILayout.Toggle("Update Root", dialog.updateRoot); 

		//IDENTIFIER FIELD
		int index = 0;
		bool find = false;
		List <string> identifiers = new List<string>();
		//Obtain all character's identifier and update if necessary the current dialog character's identifier 
		foreach( string id in register.Keys ){
			identifiers.Add(id);
			if( dialog.characterIdentifier.CompareTo( id ) == 0 ){
				find = true;
			}
			if(!find)
				index++;
		}
		//If find the reference, update if necessary. Otherwise, keep the old reference and show a empty option, but update if the reference value change
		if(find){
			index = EditorGUILayout.Popup("Character Identifier",index, identifiers.ToArray());
			dialog.characterIdentifier = identifiers[index];
		}else{
			identifiers.Add("");
			index = EditorGUILayout.Popup("Character Identifier", identifiers.Count-1, identifiers.ToArray());
			if( identifiers[index].CompareTo("") != 0 )
				dialog.characterIdentifier = identifiers[index];
		}

		//TEXT EDIT FIELD
		if( dialog.query.Count > 1 ){
			//EditorGUILayout.PropertyField( dialogs[windowID].query );
			//EditorGUILayout.BeginArea(  new Rect(0, 90,140,160)  );
			dialog.selectedQuery = EditorGUILayout.Popup("Query", dialog.selectedQuery, dialog.query.ToArray() );
			dialog.query[dialog.selectedQuery] = EditorGUILayout.TextField(dialog.query[dialog.selectedQuery] );
			//EditorGUILayout.EndArea();
		}

		//TEXT AREA FOR THE DIALOG
		EditorGUILayout.LabelField("Dialog Text");
		dialog.text = EditorGUILayout.TextArea(dialog.text );

	}

	public void RegisterSpeechers(){
		register.Clear();
		Speaker[] speechers = (Speaker[]) FindObjectsOfType( typeof(Speaker) );
		foreach( Speaker s in speechers ){
			if( !register.ContainsKey(s.identifier) )
				register.Add( s.identifier, s );
		}
	}

}
