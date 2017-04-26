using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class InteractionNodesEditor : EditorWindow {

	private Dictionary<int, Interaction> interactions;
	private InteractionController primaryController;
	//window styles
	GUIStyle windowsStyleRoot = new GUIStyle();
	GUIStyle windowsStyleGeneral = new GUIStyle();
	GUIStyle windowsStyleSubMachine = new GUIStyle();
	private Texture2D root;
	private Texture2D general;
	private Texture2D subMachine;
	private Vector2 scrollPosition = Vector2.zero;
	//selected InteractionControllers
	private class ControllerSelect{
		public ControllerSelect(InteractionController i, bool active){
			interactionController = i;
			this.active = active;
		}
		public InteractionController interactionController;
		public bool active = false;
	}
	private List<ControllerSelect> selector = new List<ControllerSelect>();
	//Auxiliar variables
	int selectedNodeId = -1;
	bool isLinking = false;
	//Constant values
	const int node_width = 200;
	const int node_height = 72;
	const int interac_ctrl_selector_width = 200;
	const int interac_ctrl_selector_height = 100;
	const int interac_ctrl_label_width = 110;
	private float drag_offset_x = 0;
	private float drag_offset_y = 0;
	//
	private Event eventHandle;
	private Transition selected;

	public static void ShowWindow(InteractionController interactionController)
	{
		//Show existing window instance. If one doesn't exist, make one.
		InteractionNodesEditor window = 
		(InteractionNodesEditor)EditorWindow.GetWindow (typeof(InteractionNodesEditor));
		window.primaryController = interactionController;
		window.FindAllControllers ();
		window.SelectController (interactionController);
		window.LoadInteractionsToDictionary (interactionController);
		window.WindowStyleConfigs();
	}
	Vector2 scrollPos;
	void OnGUI(){

		//GUILayout.BeginArea (new Rect (drag_offset_x, drag_offset_y, position.width, position.height) );
		scrollPos = GUI.BeginScrollView(new Rect(0,0,position.width*2, position.height*2 ), 
		                                new Vector2(drag_offset_x, drag_offset_y),
		                                new Rect(drag_offset_x,drag_offset_y,position.width*3, position.height*3 ));
		//GUI.ScrollTo (new Rect (drag_offset_x, drag_offset_y, position.width, position.height) );

		DrawGrid ();
		eventHandle = Event.current;
		if( interactions != null ){
			try{
				DrawWindowsAndArrows();	
				if(IsLinking()){
					//Draw feedback line if linking parent function is active
					Rect parentRect = interactions[selectedNodeId].EditorWindowRect;
					Handles.BeginGUI();
					Handles.DrawBezier(eventHandle.mousePosition, parentRect.center, eventHandle.mousePosition, parentRect.center,Color.red,null,2f);
					Handles.EndGUI();
					Repaint();
					if (eventHandle.type == EventType.ContextClick ){
						CancelLinking();
						eventHandle.Use();
					}
				}
				//Create a context menu for creating new interaction nodes
				if (eventHandle.type == EventType.ContextClick ){
					Vector2 mousePos = eventHandle.mousePosition;
					if ((new Rect( drag_offset_x,drag_offset_y, maxSize.x+drag_offset_x, maxSize.y+drag_offset_y)).Contains (mousePos))
					{
						GenericMenu menu = new GenericMenu ();
						menu.AddItem (new GUIContent ("Create New Interaction"), false, CreateInteraction);
						menu.ShowAsContext ();
						eventHandle.Use();
					}
				}
			}catch( System.Exception e){
				interactions.Clear();
				interactions = null;
				Debug.LogException(e);
			}
		}

		GUI.EndScrollView ();

		if (Event.current.button == 2 && eventHandle.type == EventType.mouseDrag) {
			//drag_offset_x -= eventHandle.delta.x;
			//drag_offset_y -= eventHandle.delta.y;	
			Repaint();
		}





		//Draw and handle the InteractionController selection area
		GUILayout.BeginArea (new Rect(0, 
		                              this.position.height - interac_ctrl_selector_height, 
		                              interac_ctrl_selector_width, 
		                              interac_ctrl_selector_height ));
		//GUILayout.Box ("", GUILayout.Width(interac_ctrl_selector_width), GUILayout.Height(interac_ctrl_selector_height));
		GUILayout.BeginVertical(GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true); 
		GUILayout.Label("Interaction Controllers", GUI.skin.label, GUILayout.ExpandWidth(true));

		foreach (ControllerSelect item in selector)
		{
			GUILayout.BeginHorizontal ();
			EditorGUI.BeginChangeCheck ();
			GUILayout.Label(item.interactionController.name, GUI.skin.button, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(interac_ctrl_label_width));
			item.active = GUILayout.Toggle(item.active,"active");
			if (EditorGUI.EndChangeCheck ()){
				if(item.active)
					AddInteractionsToDictionary(item.interactionController);
				else
					LoadAllSelectedControllers();

			}
			GUILayout.EndHorizontal();			
		}
		

		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndArea ();

	}

	void WindowNodeFunction(int windowId){

		
		try{
			Event eventHandle = Event.current;

			if (eventHandle.type == EventType.mouseDown && IsLinking())
				CreateLink(selectedNodeId, windowId);

			if ((Event.current.button == 0) && (Event.current.type == EventType.MouseDown)) {
				selectedNodeId = windowId;
				InteractionContainer interCont = ScriptableObject.CreateInstance<InteractionContainer>();
				interCont.interaction = interactions[selectedNodeId];
				Selection.activeObject = interCont;
			}
			//Create right click context menu for the window s nodes
			if ((Event.current.button == 1) && (Event.current.type == EventType.MouseDown) ){
				{
					selectedNodeId = windowId;
					GenericMenu menu = new GenericMenu ();
					menu.AddItem (new GUIContent ("Make Transition"), false, MakeLink);
					menu.AddSeparator("");
					menu.AddItem (new GUIContent ("Delete"), false, DeleteInteraction);
					menu.ShowAsContext ();
					eventHandle.Use();
				}
			}
		}catch( System.Exception e ){
			Debug.LogException(e);
		}
		GUI.DragWindow();
	}

	void CreateInteraction(){
		if( interactions == null ) return;

		Interaction interaction = new Interaction();
		primaryController.Interactions.Add( interaction );
		interaction.interactionController = primaryController;
		interaction.EditorWindowRect = new Rect(eventHandle.mousePosition.x+drag_offset_x, eventHandle.mousePosition.y+drag_offset_y, node_width , node_height);
		interaction.interactionId = InteractionController.newId();
		interactions.Add( interaction.interactionId, interaction );
	}

	bool IsLinking(){
		return isLinking;
	}

	void MakeLink(){
		isLinking = true;
	}

	void CreateLink(int parentWindowId, int childWindowId){
				//Create a parent->child link to this Dialog Window
		if (parentWindowId == childWindowId){
			isLinking = false;
			return;
		}
		Interaction parent = interactions[parentWindowId];
		Interaction child = interactions[childWindowId];
		parent.ConnectTo(child);
		isLinking = false;

	}

	void CancelLinking(){
		isLinking = false;
	}
	
	void DeleteLink(){

		Interaction parent = selected.From;
		Interaction child = selected.To;

		parent.children.Remove (selected);
		child.parents.Remove ( child.parents.Find(x => x.toId == parent.interactionId) );
	}

	void DeleteInteraction(){
		if (interactions == null) return;

		Interaction interaction = interactions[selectedNodeId];
		interactions.Remove (selectedNodeId);
		InteractionController interCtrl = interaction.interactionController;
		//Delete connections to children
		foreach( Transition child in interaction.children ){
			Transition t = child.To.parents.Find(x => x.toId == interaction.interactionId);
			child.To.parents.Remove(t);
		}
		//Delete connections from parents
		foreach( Transition parent in interaction.parents ){
			Transition t = parent.To.children.Find(x => x.toId == interaction.interactionId);
			parent.To.children.Remove(t);
		}
		interCtrl.Interactions.Remove (interaction);
	}

	void DrawWindowsAndArrows(){
		Event eventHandle = Event.current;

		BeginWindows();
		//Create the window nodes for the dialogs
		foreach ( KeyValuePair<int, Interaction> pair in interactions ){
			
			Interaction interaction = pair.Value;
			
			//Draw nodes
			interaction.EditorWindowRect = GUI.Window (interaction.interactionId, interaction.EditorWindowRect, WindowNodeFunction, interaction.ToString(), windowsStyleGeneral);
			
			Vector3 mousePos = Event.current.mousePosition;
			
			//Create conections between parents and children
			for( int i=0; i<interaction.children.Count; i++ ){
				
				Interaction child = interaction.children[i].To;
				//Draw conections between parent and child
				Handles.BeginGUI();
				Handles.DrawLine(interaction.EditorWindowRect.center, child.EditorWindowRect.center );

				//Create an arrow pointing to the child
				Vector2 vertex1 = (interaction.EditorWindowRect.center + child.EditorWindowRect.center )/2;
				Vector2 currentConectionShift = (vertex1 - child.EditorWindowRect.center ).normalized*25*interaction.children[i].numberOfConnections;
				Vector2 triBaseMiddle = vertex1 + (vertex1 - child.EditorWindowRect.center ).normalized*25;
				Vector2 triHeight = vertex1 - triBaseMiddle;
				Vector2 vertex2 = new Vector2( triHeight.x*Mathf.Cos( Mathf.PI/4 ) - triHeight.y*Mathf.Sin(Mathf.PI/4) , 
				                              triHeight.x*Mathf.Sin( Mathf.PI/4 ) + triHeight.y*Mathf.Cos(Mathf.PI/4)) + triBaseMiddle ;
				Vector2 vertex3 = new Vector2( triHeight.x*Mathf.Cos( -Mathf.PI/4 ) - triHeight.y*Mathf.Sin(-Mathf.PI/4) , 
				                              triHeight.x*Mathf.Sin( -Mathf.PI/4 ) + triHeight.y*Mathf.Cos(-Mathf.PI/4)) + triBaseMiddle;
				Vector3[] vector = {vertex1 + currentConectionShift, (vertex2+triBaseMiddle)/2  + currentConectionShift, (vertex3+triBaseMiddle)/2  + currentConectionShift, vertex1  + currentConectionShift};
				Handles.DrawSolidRectangleWithOutline(vector, new Color(255,255,255), new Color(0,0,0,0.0f));
				//Test if there was a context click event on the triangle representing the transition
				if( isInsideTriangle( vector[0], vector[1], vector[2], mousePos ) ){
					if( eventHandle.type == EventType.mouseUp ){
						Handles.DrawSolidRectangleWithOutline(vector, new Color(0,55,155), new Color(0,0,0,0.0f));
						TransitionContainer tr = ScriptableObject.CreateInstance<TransitionContainer>();
						tr.transition = interaction.children[i];
						Selection.activeObject = tr;
						selected = tr.transition;
					}else if( eventHandle.type == EventType.ContextClick ){
						GenericMenu menu = new GenericMenu ();
						selected = interaction.children[i];
						menu.AddItem (new GUIContent ("Delete Transition"), false, DeleteLink);
						menu.ShowAsContext ();
						eventHandle.Use();
						
					}

				}
				
				Handles.EndGUI();						
			}
		}
		EndWindows();

	}
	
	void DrawGrid(){
		Vector3 offset = new Vector3 (drag_offset_x,drag_offset_y,0);
		Vector3[] vert ={new Vector3(0,0,0)+offset ,
						 new Vector3(maxSize.x, 0,0)+offset+new Vector3(200,0,0),  
						 new Vector3(maxSize.x, maxSize.y,0)+offset+new Vector3(200,0,0), 
						 new Vector3(0, this.maxSize.y,0)+offset} ;
		Handles.DrawSolidRectangleWithOutline(vert,new Color(0,0,0,0.3f), new Color(1,1,1,0) );


		
		float w = 1;
		for(float i=drag_offset_x; i< maxSize.x+drag_offset_x; i+=1){
			Vector3[] vertex = {new Vector3(i, drag_offset_y), 
								new Vector3(i, maxSize.y+drag_offset_y), 
								new Vector3(i+w, drag_offset_y) , 
								new Vector3(i+w, maxSize.y+drag_offset_y)  };
			if( Mathf.Abs( ((int)i )) %120 == 0 )
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.07f), new Color(0,0,0,0f) );
			else if( Mathf.Abs( ((int)i )) %12 == 0 )
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.02f), new Color(0,0,0,0f) );
		}
		for(float i=drag_offset_y; i<maxSize.y+drag_offset_y; i+=1){
			Vector3[] vertex = {new Vector3(drag_offset_x, i), 
								new Vector3(maxSize.x+drag_offset_x, i), 
								new Vector3(drag_offset_x, i+w) , 
								new Vector3(maxSize.x+drag_offset_x, i+w) };
			if( Mathf.Abs( ((int)i))%120 == 0 )
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.07f), new Color(0,0,0,0f) );
			else if( Mathf.Abs( ((int)i )) %12 == 0 )
				Handles.DrawSolidRectangleWithOutline(vertex,new Color(0,0,0,0.02f), new Color(0,0,0,0f) );
		}
	}

	void LoadInteractionsToDictionary(){
		if( primaryController == null )	return;

		interactions = new Dictionary<int ,Interaction >();
		Stack<Interaction> stack = new Stack<Interaction>();
		
		//Do search by deeph on the tree
		foreach (Interaction i in primaryController.Interactions) {
						if( !interactions.ContainsKey(i.interactionId) )
							stack.Push (i);
						while (stack.Count > 0) {
								Interaction next = stack.Pop ();
								interactions.Add (next.interactionId, next);
								foreach (Transition t in next.children) {
										
										if( !interactions.ContainsKey(t.To.interactionId) )
											stack.Push (t.To);
								}
						}
				}
	}

	void LoadInteractionsToDictionary(InteractionController interactionController){
		if( interactionController == null )	return;
		
		interactions = new Dictionary<int ,Interaction >();
		Stack<Interaction> stack = new Stack<Interaction>();
		
		//Do search by deeph on the tree
		foreach (Interaction i in interactionController.Interactions) {
			if( !interactions.ContainsKey(i.interactionId) )
				stack.Push (i);
			while (stack.Count > 0) {
				Interaction next = stack.Pop ();
				interactions.Add (next.interactionId, next);
				foreach (Transition t in next.children) {
					if( !interactions.ContainsKey(t.To.interactionId) )
						stack.Push (t.To);
				}
			}
		}
	}

	void AddInteractionsToDictionary(InteractionController interactionController){
		if( interactionController == null )	return;
		if( interactions == null )
			interactions = new Dictionary<int ,Interaction >();

		Stack<Interaction> stack = new Stack<Interaction>();
		
		//Do search by deeph on the tree
		foreach (Interaction i in interactionController.Interactions) {
			if( !interactions.ContainsKey(i.interactionId) )
				stack.Push (i);
			while (stack.Count > 0) {
				Interaction next = stack.Pop ();
				interactions.Add (next.interactionId, next);
				foreach (Transition t in next.children) {

					if( !interactions.ContainsKey(t.To.interactionId) )
						stack.Push (t.To);
				}
			}
		}
		Debug.Log ("Change primary");
		primaryController = interactionController;
	}

	private void FindAllControllers(){
		if (selector == null)
						selector = new List<ControllerSelect> ();
		//Use to keep active the previous selected Controllers before an update
		List<ControllerSelect> selector_backup = new List<ControllerSelect>(selector);
		selector.Clear();
		InteractionController[] controllers = (InteractionController[]) FindObjectsOfType( typeof(InteractionController) );
		foreach (InteractionController i in controllers) {
			if(selector_backup.Count > 0)
				selector.Add (new ControllerSelect (i, selector_backup.Find (x => x.interactionController == i).active));
			else
				selector.Add (new ControllerSelect (i,	false));
		}
	}

	private void LoadAllSelectedControllers(){
		interactions = new Dictionary<int, Interaction> ();
		foreach (ControllerSelect cs in selector)
			if( cs.active )
					AddInteractionsToDictionary (cs.interactionController);    

		this.primaryController = selector.Find(x => x.active == true) != null ? selector.Find(x => x.active == true).interactionController: null; 
	}

	public void SelectController( InteractionController interactionController ){
		ControllerSelect cs = selector.Find (x => x.interactionController == interactionController);
		cs.active = true;

	}

	private bool isInsideTriangle( Vector3 v1, Vector3 v2, Vector3 v3, Vector3 intersection )
	{
		double area = Vector3.Cross(v2-v1, v3-v1 ).magnitude*0.5f;;
		Vector3 vet1 = intersection - v1;
		Vector3 vet2 = v2 - v1;
		double area1 = Vector3.Cross(vet1, vet2).magnitude*0.5f/area;
		
		vet1 = intersection - v1;
		vet2 = v3 - v1;
		double area2 =  Vector3.Cross(vet1, vet2).magnitude*0.5/area;
		
		vet1 = intersection - v3;
		vet2 = v2 - v3;
		double area3 = Vector3.Cross(vet1, vet2).magnitude*0.5/area;
		
		//Condição para que o ponto esteja contido no triangulo
		if( area1 + area2 + area3 <= 1.00001 )
			return true;
		
		return false;
	}
	
	private void WindowStyleConfigs(){
		Texture2D root = Resources.Load("dialogbox003") as Texture2D;
		Texture2D general = Resources.Load("dialogbox004") as Texture2D;
		Texture2D subMachine = Resources.Load("dialogbox001") as Texture2D;
		
		windowsStyleRoot.normal.background = root;
		windowsStyleRoot.padding.left = 67;
		windowsStyleRoot.padding.top = 24;
		windowsStyleRoot.fixedWidth = 200;
		windowsStyleRoot.fixedHeight = 72;
		windowsStyleGeneral.normal.background = general;
		windowsStyleGeneral.padding.left = 67;
		windowsStyleGeneral.padding.top = 24;
		windowsStyleGeneral.fixedWidth = 200;
		windowsStyleGeneral.fixedHeight = 72;
		windowsStyleSubMachine.normal.background = subMachine;
		windowsStyleSubMachine.padding.left = 67;
		windowsStyleSubMachine.padding.top = 24;
		windowsStyleSubMachine.fixedWidth = 200;
		windowsStyleSubMachine.fixedHeight = 72;
	}

	void OnProjectChange(){
		FindAllControllers ();
		LoadAllSelectedControllers ();
	}

}
