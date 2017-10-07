using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;

public class DialogEditor : EditorWindow {

	private Event eventHandle;
	private int activeNodeID;
	private int m_focusingID;

	private Dictionary<int, Dialog> dialogs;
	private DialogShelf dialogShelf;
    private DialogController diagControl;

    private Dictionary<string, Speaker>  register = new Dictionary<string, Speaker>();
	
	//Flags
	private bool linking = false;
	private bool ableToReload = true;

	private Rect DialogWindowRect = new Rect(0,0,200,72);

	List<string> options = new List<string>();

	GUIStyle windowsStyleRoot = new GUIStyle();
	GUIStyle windowsStyleGeneral = new GUIStyle();
	private Texture2D root;
	private Texture2D general;

	public Dialog active;

    bool showPosition = false;
    

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Dialog Editor")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		DialogEditor window = (DialogEditor)EditorWindow.GetWindow(typeof(DialogEditor));
		window.root = Resources.Load("dialogbox003") as Texture2D;
		window.general = Resources.Load("dialogbox004") as Texture2D;

		window.windowsStyleRoot.normal.background = window.root;
		window.windowsStyleRoot.padding.left = 67;
		window.windowsStyleRoot.padding.top = 24;
		window.windowsStyleRoot.fixedWidth = 200;
		window.windowsStyleRoot.fixedHeight = 72;
		window.windowsStyleGeneral.normal.background = window.general;
		window.windowsStyleGeneral.padding.left = 67;
		window.windowsStyleGeneral.padding.top = 24;
		window.windowsStyleGeneral.fixedWidth = 200;
		window.windowsStyleGeneral.fixedHeight = 72;

		window.LoadDialogs();
		window.RegisterSpeechers();

	}

	public Material mat;
	//Editor Main Function
	void OnGUI()
	{

		Vector3[] vert ={new Vector3(0,0,0),new Vector3(maxSize.x, 0,0),  new Vector3(maxSize.x, maxSize.y,0), new Vector3(0, this.maxSize.y,0)} ;
		Handles.DrawSolidRectangleWithOutline(vert,new Color(0,0,0,0.3f), new Color(1,1,1,0) );


		float w = 1;
		for(int i=0; i<maxSize.x; i+=12){
			Vector3[] vertex = {new Vector3(i, 0), new Vector3(i, maxSize.y), new Vector3(i+w, 0), new Vector3(i+w, maxSize.y) };
			if( i%120 == 0 )
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.07f), new Color(0,0,0,0f) );
			else
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.02f), new Color(0,0,0,0f) );
		}
		for(int i=0; i<maxSize.y; i+=12){
			Vector3[] vertex = {new Vector3(0, i), new Vector3(maxSize.x, i), new Vector3(0, i+w), new Vector3(maxSize.x, i+w)};
			if( i%120 == 0 )
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.07f), new Color(0,0,0,0f) );
			else
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.02f), new Color(0,0,0,0f) );
		}

        //GameObject selected = Selection.activeGameObject;
        if (diagControl == null) {
            return;  
        }
      
        //Create new DialogShelf File
        if (GUI.Button(new Rect(0, 0, 150, 20), GUIContent.none)) {
            SaveNewFile();
            diagControl.dialogShelf = dialogShelf;
        }
		GUILayout.Label ("Create New Dialog");

		//Dialog Shelf Layers Controller
		DisplayLayersController();

		//DialogSet Editing
		if( dialogs != null ){

			BeginWindows();
            //Create the window nodes for the dialogs
            Event eventHandle = Event.current;
            foreach ( KeyValuePair<int, Dialog> pair in dialogs ){

				Dialog dialog = pair.Value;
				if( dialog.id == dialogShelf.layers[dialogShelf.selectedLayer].Root.id ){
					dialog.EditorWindowRect = GUI.Window (dialog.id, dialog.EditorWindowRect, WindowNodeFunction, dialog.characterIdentifier, windowsStyleRoot);
				}else
					dialog.EditorWindowRect = GUI.Window (dialog.id, dialog.EditorWindowRect, WindowNodeFunction, dialog.characterIdentifier, windowsStyleGeneral);

				//Create conections between parents and children
				for( int i=0; i<dialog.children.Count; i++ ){
					
					Dialog child = dialogs[dialog.children[i]];
					//Draw conections between parent and child

					Handles.BeginGUI();
					Handles.DrawLine(dialog.EditorWindowRect.center, child.EditorWindowRect.center );
					Vector2 middle = (dialog.EditorWindowRect.center + child.EditorWindowRect.center)/2;
					
					//Create an arrow pointing to the child
					Vector2 shift = middle + (middle - child.EditorWindowRect.center ).normalized*30;
					Vector2 aux = middle - shift;
					Vector2 p2 = new Vector2( aux.x*Mathf.Cos( Mathf.PI/4 ) - aux.y*Mathf.Sin(Mathf.PI/4) , 
					                         aux.x*Mathf.Sin( Mathf.PI/4 ) + aux.y*Mathf.Cos(Mathf.PI/4)) + shift;
					Vector2 p3 = new Vector2( aux.x*Mathf.Cos( -Mathf.PI/4 ) - aux.y*Mathf.Sin(-Mathf.PI/4) , 
					                         aux.x*Mathf.Sin( -Mathf.PI/4 ) + aux.y*Mathf.Cos(-Mathf.PI/4)) + shift;

					//Handles.DrawLine(middle, (p2+shift)/2 );
					//Handles.DrawLine((p2+shift)/2, (p3+shift)/2 );
					//Handles.DrawLine((p3+shift)/2, middle );



					Vector3[] vector = {middle, (p2+shift)/2, (p3+shift)/2, middle};
					Handles.DrawSolidRectangleWithOutline(vector, new Color(255,255,255), new Color(0,0,0,0.0f));


                    Vector3 mousePos = Event.current.mousePosition;
                    if (eventHandle.type == EventType.ContextClick && isInsideTriangle(vector[0], vector[1], vector[2], mousePos)) {
                        GenericMenu menu = new GenericMenu();
                        KeyValuePair<Dialog, Dialog> selected = new KeyValuePair<Dialog, Dialog>(dialog, dialogs[dialog.children[i]]);
                        menu.AddItem(new GUIContent("Delete Transition"), false, DeleteLink, selected);
                        menu.ShowAsContext();
                        eventHandle.Use();
                    }


                    Handles.EndGUI();
                    if (dialog.children.Count > 1) {
                        GUILayout.BeginArea(new Rect(aux.x + shift.x, aux.y + shift.y, 150, 20));
                        GUILayout.Label(dialog.query[i]);
                        GUILayout.EndArea();
                    }
				}

			}
			EndWindows();
			
			//CAPTURE WINDOW EVENTS
			eventHandle = Event.current;
			Rect contextRect = new Rect ( 0,0, maxSize.x, maxSize.y);
			//Create a context menu for creating new dialog nodes
			if (eventHandle.type == EventType.ContextClick ){
				Vector2 mousePos = eventHandle.mousePosition;
				if (contextRect.Contains (mousePos) && !linking)
				{
					// Now create the menu, add items and show it
					GenericMenu menu = new GenericMenu ();
					menu.AddItem (new GUIContent ("Create New Node"), false, CreateNode);
					menu.ShowAsContext ();
					eventHandle.Use();
				}
				linking = false;
			}

			//Draw feedback line if linking parent function is active
			if( linking ){
				Dialog parent = dialogs[activeNodeID];
				Handles.BeginGUI();
				Handles.DrawBezier(eventHandle.mousePosition, parent.EditorWindowRect.center, eventHandle.mousePosition, parent.EditorWindowRect.center,Color.red,null,2f);
				Handles.EndGUI();
				Repaint();
			}


		}




		//Rect r = EditorGUILayout.BeginHorizontal("Button");
		/*GUILayout.Label ("I'm inside the button");


		//flags = EditorGUILayout.MaskField ("", flags, options);


		//EditorGUILayout.EndHorizontal ();*/

		
		//scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width (300), GUILayout.Height (300));
		if( dialogShelf != null ){

			DialogSet dialogSet = dialogShelf.layers[dialogShelf.selectedLayer];

			if( dialogSet != null ){
				GUILayout.BeginArea(new Rect(0, 300, 200, 300));
				EditorGUILayout.BeginVertical("");
				showPosition = EditorGUILayout.Foldout(showPosition, "Layer Properties");

				if( showPosition ){
					EditorGUILayout.BeginHorizontal("");
					GUILayout.Label("Name");
					dialogSet.layer = EditorGUILayout.TextField(dialogSet.layer);
					EditorGUILayout.EndHorizontal();

					dialogSet.disposable = EditorGUILayout.Toggle("Disposable", dialogSet.disposable);

					EditorGUILayout.BeginHorizontal("");
					GUILayout.Label("Conditions");
					if (GUILayout.Button("+", GUILayout.Width (17), GUILayout.Height (17)) ){
						dialogSet.conditions.Add("condition");
						dialogSet.originalValues.Add(false);
					}
					EditorGUILayout.EndHorizontal();

					for(int i=0;i<dialogSet.conditions.Count;i++){
						EditorGUILayout.BeginHorizontal("");
						dialogSet.conditions[i] = EditorGUILayout.TextField(dialogSet.conditions[i]);
						dialogSet.originalValues[i] = EditorGUILayout.Toggle(dialogSet.originalValues[i]);
						if (GUILayout.Button("-") ){
							dialogSet.conditions.RemoveAt(i);
							dialogSet.originalValues.RemoveAt(i);
						}
						EditorGUILayout.EndHorizontal();
					}
				}

				EditorGUILayout.EndVertical();
				GUILayout.EndArea();
				//EditorGUILayout.EndScrollView();


				GUILayout.BeginArea(new Rect(0, 35, 150, 200));
				EditorGUILayout.BeginVertical("");
				EditorGUILayout.BeginHorizontal("");

				GUILayout.Label(dialogSet.layer);
				if (GUILayout.Button("+") )
					CreateLayer();
				if (GUILayout.Button("-") ){
					DeleteLayer();
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				GUILayout.EndArea();
			}


		}


	}

    private void DeleteLink(object dialogs) {
        KeyValuePair<Dialog, Dialog> parentChild = (KeyValuePair<Dialog, Dialog>)dialogs;
        parentChild.Key.query.RemoveAt(parentChild.Key.children.IndexOf(parentChild.Value.id));
        parentChild.Key.children.Remove(parentChild.Value.id);
        parentChild.Value.parents.Remove(parentChild.Key.id);

    }



    /*int flags = 0;
string[] options  = {"Create New Dialog"};
public Object source;
string tagStr = "";
void SetTags() {
foreach(   GameObject go in Selection.gameObjects)
go.tag = tagStr;
}
*/

    void SaveNewFile () {
		string path = "";
		path = EditorUtility.SaveFilePanel( "Save Dialog", "Assets", "New Dialog", "asset" );

		//If cancel command
		if( path.CompareTo("") == 0 ){
			return;
		}

		bool isSubDirectory = false;
		if( path.StartsWith( Application.dataPath )  ){
			path = path.Substring( Application.dataPath.Length - 6 );
			isSubDirectory = true;
		}
		if (isSubDirectory) {
			CreateAsset(path);
		}else{
				EditorUtility.DisplayDialog(
					"Save Dialog",
					"You must save the dialog file inside the project Assets folder!",
					"Ok");
		}
	}

	//Window Function for the Dialog Node Windows
	void WindowNodeFunction (int windowID) 
	{
		//Vector3[] vert ={new Vector3(0,0,0),new Vector3(DialogWindowRect.width, 0,0),  new Vector3(DialogWindowRect.width, DialogWindowRect.height,0), new Vector3(0, this.DialogWindowRect.height,0)} ;	
		//Handles.DrawSolidRectangleWithOutline(vert,new Color(100,0,0,0.3f), new Color(1,1,1,0) );
		//WINDOW EVENT HANDLER
		eventHandle = Event.current;
		
		//if(eventHandle.type == EventType.mouseDown )
		if ((Event.current.button == 0) && (Event.current.type == EventType.MouseDown)) {
			m_focusingID = windowID;
			if( Selection.activeObject != dialogs[windowID] ){
				Selection.activeObject = dialogs[windowID];
				ableToReload = false;
			}


			Vector2 mousePos = eventHandle.mousePosition;
			//if (contextRect.Contains (mousePos) && !linking)

		}
        //	

        //Create a parent->child link to this Dialog Window
        if (eventHandle.type == EventType.mouseDown && linking) {
            Dialog parent = dialogs[activeNodeID];
            Dialog child = dialogs[windowID];
            child.parents.Add(parent.id);
            parent.children.Add(child.id);

            parent.query.Add(new Func<string>(() => {
                int i = 1;
                while (parent.query.Contains("Option " + i)) i++;
                return "Option " + i;
            })());


            linking = false;
		}


		if ((Event.current.button == 1) && (Event.current.type == EventType.MouseDown) ){
			{
				m_focusingID = windowID;
				if( Selection.activeObject != dialogs[windowID] ){
					Selection.activeObject = dialogs[windowID];
					ableToReload = false;
				}

				// Now create the menu, add items and show it
				GenericMenu menu = new GenericMenu ();
				menu.AddItem (new GUIContent ("Create Link"), false, CreateLink);
				menu.AddSeparator("");
				menu.AddItem (new GUIContent ("Delete"), false, DeleteNode);
				menu.ShowAsContext ();
				eventHandle.Use();
			}
		}

		//GUILayout.BeginArea(  new Rect(d_x, d_y, d_w, d_h)  );

		/*
		//IDENTIFIER FIELD
		int index = 0;
		bool find = false;
		List <string> identifiers = new List<string>();
		//Obtain all character's identifier and update if necessary the current dialog character's identifier 

		foreach( string id in register.Keys ){
			identifiers.Add(id);
			if( dialogs[windowID].characterIdentifier.CompareTo( id ) == 0 ){
				find = true;
			}
			if(!find)
				index++;
		}
		//If find the reference, update if necessary. Otherwise, keep the old reference and show a empty option, but update if the reference value change
		if(find){
			index = EditorGUILayout.Popup(index, identifiers.ToArray());
			dialogs[windowID].characterIdentifier = identifiers[index];
		}else{
			identifiers.Add("");
			index = EditorGUILayout.Popup(identifiers.Count-1, identifiers.ToArray());
			if( identifiers[index].CompareTo("") != 0 )
				dialogs[windowID].characterIdentifier = identifiers[index];
		}


		//TEXT EDIT FIELD
		if( dialogs[windowID].query.Count > 1 ){
			GUILayout.Label ("Query");
			//EditorGUILayout.PropertyField( dialogs[windowID].query );
			GUILayout.BeginArea(  new Rect(0, 90,140,160)  );
			dialogs[windowID].selectedQuery = EditorGUILayout.Popup(dialogs[windowID].selectedQuery, dialogs[windowID].query.ToArray() );
			dialogs[windowID].query[dialogs[windowID].selectedQuery] = GUI.TextArea(new Rect (0,20,140,15), dialogs[windowID].query[dialogs[windowID].selectedQuery] );
			GUILayout.EndArea();
		}

		//TEXT EDIT FIELD
		dialogs[windowID].text = GUI.TextArea(new Rect (0,40,180,40),dialogs[windowID].text );


		*/
		/*
		//CREATE LINK FIELD
		Texture svicon = Resources.Load("createlink") as Texture;
		if (GUI.Button (new Rect (140,20,20,20), svicon )){

		}

		//CREATE LINK FIELD
		if (GUI.Button (new Rect (150,5,15,15),"-")){
		}
		*/
		//GUILayout.EndArea();
		GUI.DragWindow();


	}

	void DeleteNode(){
		if( dialogShelf != null ){
			Dialog dialog = dialogs[m_focusingID];
			if( dialogShelf.layers[dialogShelf.selectedLayer].Remove(m_focusingID) ){
				dialogs.Remove(m_focusingID);

				foreach( int child in dialog.children ){
					dialogs[child].parents.Remove(m_focusingID);
				}
				foreach( int parent in dialog.parents ){
					int q_index =dialogs[parent].children.IndexOf(m_focusingID);
					dialogs[parent].query.RemoveAt(q_index);
					dialogs[parent].children.Remove(m_focusingID);
				}

				string path = AssetDatabase.GetAssetPath( dialogShelf );
				EditorUtility.SetDirty( dialogShelf.layers[dialogShelf.selectedLayer] );
				DestroyImmediate(dialog, true);
				AssetDatabase.SaveAssets();
				dialogShelf = (DialogShelf)AssetDatabase.LoadAssetAtPath( path, typeof(DialogShelf) );
				Debug.Log(dialogShelf);
				Selection.activeObject = dialogShelf ;
			}
		}
	}

	void DeleteLayer(){
		if( dialogShelf != null && dialogShelf.layers.Count > 1 ){

			DialogSet dialogSet = dialogShelf.layers[dialogShelf.selectedLayer];
			dialogShelf.layers.RemoveAt(dialogShelf.selectedLayer);
			string path = AssetDatabase.GetAssetPath( dialogShelf );
			EditorUtility.SetDirty( dialogShelf );
			DestroyImmediate(dialogSet, true);
			AssetDatabase.SaveAssets();
			dialogShelf = (DialogShelf)AssetDatabase.LoadAssetAtPath( path, typeof(DialogShelf) );
			dialogShelf.selectedLayer = dialogShelf.layers.Count-1;
			Selection.activeObject = dialogShelf;
	
		}
	}

	void CreateLink(){
		linking = true;
		activeNodeID = m_focusingID;
	}

	//Context Menu callback Function
	void CreateNode () {

		if( dialogs == null ){
			Debug.LogWarning( "Erro while creating new dialog: Dialog File was not loaded correctly" );
			return;
		}
		Dialog dialog = CreateDialog();
		dialog.EditorWindowRect = new Rect(eventHandle.mousePosition.x, eventHandle.mousePosition.y, DialogWindowRect.width , DialogWindowRect.height);
		dialog.id = dialogShelf.layers[dialogShelf.selectedLayer].newID();
		
		//Todo
		//Get the information from the last dialog created, or selected to copy some informations
		//Optional: List all characters and select an active to create his dialogs

		//Add dialog to assets
		dialogShelf.layers[dialogShelf.selectedLayer].AddDialog( dialog );
		AssetDatabase.AddObjectToAsset (dialog, dialogShelf.layers[dialogShelf.selectedLayer] );
		EditorUtility.SetDirty( dialogShelf.layers[dialogShelf.selectedLayer] );
		AssetDatabase.SaveAssets();

		dialogs.Add( dialog.id, dialog );
	}
	//Create a new dialog file asset
	void CreateAsset(string path){
		//Get the file name
		char[] separator = {'/','\\', '.'};
		string[] files = path.Split( separator );

		//Create Asset And Add Base Layer
		DialogShelf dialogShelf = (DialogShelf)ScriptableObject.CreateInstance<DialogShelf>();
	
		DialogSet dialogSet = (DialogSet)ScriptableObject.CreateInstance<DialogSet> ();
		dialogSet.name = "Layer Sub Asset";
		dialogSet.layer = "Base"; 
		dialogSet.hideFlags = HideFlags.HideInHierarchy;
		dialogShelf.layers.Add( dialogSet );

		AssetDatabase.CreateAsset(dialogShelf, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.AddObjectToAsset (dialogSet, dialogShelf);
		EditorUtility.SetDirty(dialogShelf);
		AssetDatabase.SaveAssets();
		//Set the new dialog as the active object
		this.dialogShelf = dialogShelf;
	}

	Dialog CreateDialog()
	{
		if (dialogShelf == null){
			Debug.LogWarning( "Erro while creating new dialog: Dialog File was not loaded correctly" );
			return (Dialog)ScriptableObject.CreateInstance<Dialog> ();
		}

		Dialog dialog = (Dialog)ScriptableObject.CreateInstance<Dialog> ();
		dialog.name = "Layer Sub Asset";
		dialog.hideFlags = HideFlags.HideInHierarchy;


		return dialog;

	}

	void CreateLayer(){
		if(dialogShelf != null){
			DialogSet dialogSet = (DialogSet)ScriptableObject.CreateInstance<DialogSet> ();
			dialogSet.name = "Layer Sub Asset";
			dialogSet.layer = "New Layer"; 
			dialogSet.hideFlags = HideFlags.HideInHierarchy;
			dialogShelf.layers.Add( dialogSet );
	
			AssetDatabase.AddObjectToAsset (dialogSet, dialogShelf);
			EditorUtility.SetDirty(dialogShelf);
			AssetDatabase.SaveAssets();

			dialogShelf.selectedLayer = dialogShelf.layers.Count-1;
			Selection.activeObject = dialogShelf;
		}
	}

	void OnLostFocus(){
	}

	//Called when the window editor is closed
	void OnDestroy(){
		SaveDialogs();
	}
	//Called whenever the window 
	void OnInspectorUpdate() {
		Repaint();
	}

	//If the active dialog change, save the modifications on the previous dialog and load the new one
	void OnSelectionChange(){
		if( ableToReload )
		{
			SaveDialogs();
			LoadDialogs();
			RegisterSpeechers();
		}
		ableToReload = true;
	}

	DialogShelf GetSelectedShelf(){
        
        if ( Selection.activeGameObject == null )
			return null;

        GameObject selected = Selection.activeGameObject;
        DialogController diagControlTmp = selected.GetComponent<DialogController>();

        if (diagControlTmp != null) {
            diagControl = diagControlTmp;
        }

        if(diagControl == null) {
            return null;
        }

        DialogShelf dialogShelf = diagControl.dialogShelf;
		return dialogShelf;
	}

	//Update Layers Selector Options
	void DisplayLayersController(){
		if( dialogShelf == null ){
			dialogs = null;
			return;
		}
		if( dialogShelf.layers == null || dialogShelf.layers.Count == 0){
			Debug.LogWarning( "Erro while updating: (DialogShelf)" + dialogShelf.name +
			                 "Object have no layers" );
			dialogs = null;
			return;
		}

		int index_before = dialogShelf.selectedLayer;
		GUILayout.BeginArea(new Rect( 0,20,150,60 ));
		dialogShelf.selectedLayer = EditorGUILayout.Popup(dialogShelf.selectedLayer, options.ToArray() );
		GUILayout.EndArea();
		if( index_before != dialogShelf.selectedLayer )
			LoadDialogs();
		
		options.Clear();
		foreach( DialogSet d in dialogShelf.layers )
			options.Add( d.layer );
		
	}

	//Load Dialogs From File
	public void LoadDialogs( ){


        //Get The selected Dialog Shelf
        if (  GetSelectedShelf() != null )
			dialogShelf = GetSelectedShelf(); 

		dialogs = new Dictionary<int ,Dialog >();

		if( dialogShelf == null )
			return;
		if( dialogShelf.layers == null || dialogShelf.layers.Count == 0){
			Debug.LogWarning( "Erro while loading: (DialogShelf)" + dialogShelf.name +
			                 "Object have no layers" );
			return;
		}

		DialogSet dialogSet = dialogShelf.layers[dialogShelf.selectedLayer];
		for(int i=0; i < dialogSet.Count; i++){
			Dialog d = dialogSet.Get(i);
			dialogs.Add( d.id, d );
		}
	}
	//Save Dialogs On File
	public void SaveDialogs( ){

		//Check for possible inconsistences
		if( dialogShelf == null )
			return;
		if( dialogShelf.layers == null || dialogShelf.layers.Count == 0){
			Debug.LogWarning( "Erro while saving: (DialogShelf)" + dialogShelf.name +
			                 "Object have no layers" );
			return;
		}
		if( dialogs == null ){
			Debug.LogWarning( "Erro while saving: (DialogShelf)" + dialogShelf.name +
			                 "File was not loaded correctly" );
			return;
		}

		DialogSet dialogSet;
		try{
			dialogSet = dialogShelf.layers[dialogShelf.selectedLayer];
			EditorUtility.SetDirty(dialogSet);
			//dialogSet.Clear();
		}catch( System.Exception e ){
			Debug.LogWarning( e.Message );
			return;
		}

		foreach ( KeyValuePair<int, Dialog> pair in dialogs ){
			//dialogSet.AddDialog( pair.Value );

		}

		EditorUtility.SetDirty(dialogShelf);
		AssetDatabase.SaveAssets();
	}


	public void RegisterSpeechers(){
		register.Clear();
		Speaker[] speechers = (Speaker[]) FindObjectsOfType( typeof(Speaker) );
		foreach( Speaker s in speechers ){
			if( !register.ContainsKey(s.identifier) )
				register.Add( s.identifier, s );
		}
	}


    private bool isInsideTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 intersection) {
        double area = Vector3.Cross(v2 - v1, v3 - v1).magnitude * 0.5f; ;
        Vector3 vet1 = intersection - v1;
        Vector3 vet2 = v2 - v1;
        double area1 = Vector3.Cross(vet1, vet2).magnitude * 0.5f / area;

        vet1 = intersection - v1;
        vet2 = v3 - v1;
        double area2 = Vector3.Cross(vet1, vet2).magnitude * 0.5 / area;

        vet1 = intersection - v3;
        vet2 = v2 - v3;
        double area3 = Vector3.Cross(vet1, vet2).magnitude * 0.5 / area;

        //Condi??o para que o ponto esteja contido no triangulo
        if (area1 + area2 + area3 <= 1.00001)
            return true;

        return false;
    }
}
