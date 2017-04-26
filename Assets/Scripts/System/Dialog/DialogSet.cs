using UnityEngine;
using System.Collections;
using System.Collections.Generic;




[System.Serializable]
public class DialogSet: ScriptableObject  {
	public string layer;
	public bool disposable = false;
	public List<string> conditions = new List<string>();
	public List<bool> originalValues = new List<bool>();
	public List<bool> currentValues = new List<bool>();
	[SerializeField]
	private int nodeIDs = 0;
	public int newID(){
		return nodeIDs++;
	}
	[SerializeField]
	private int root;
	[SerializeField]
	public int current;
	public Dialog Root{
		get{ 
			//return (Dialog)hash[root]; 
			foreach( Dialog d in list )
				if( d.id == root )
					return d;
			return null;
		}
		set{
			foreach( Dialog d in list )
				if( d == value )
					root = value.id;
		}
	}
	
	public static DialogSet CreateBaseLayer(){
		
		DialogSet dialogSet = new DialogSet();
		dialogSet.layer = "Base";
		return dialogSet;
	}
	
	[SerializeField]
	//[HideInInspector]
	private List<Dialog> list = new List<Dialog>();
	[SerializeField]
	private List<int> identifiers = new List<int>();
	[SerializeField]
	private Hashtable hash = new Hashtable();

	void OnEnable(){
		if( hash == null ){
			hash = new Hashtable();
			if( identifiers == null )
				identifiers = new List<int>();
			else
				identifiers.Clear();

			foreach(Dialog d in list){
				hash.Add(d.id, d);
				identifiers.Add(d.id);
			}
		}
	}

	public void Load(){
		if( hash == null )
		{
			hash = new Hashtable();
			if( identifiers == null )
				identifiers = new List<int>();
			else
				identifiers.Clear();
			
			foreach(Dialog d in list){
				hash.Add(d.id, d);
				identifiers.Add(d.id);
			}
		}else if( hash.Count == 0 ){
			hash = new Hashtable();
			if( identifiers == null )
				identifiers = new List<int>();
			else
				identifiers.Clear();
			
			foreach(Dialog d in list){
				hash.Add(d.id, d);
				identifiers.Add(d.id);
			}
		}
	}

	void OnValidate(){
		if( hash == null ){
			hash = new Hashtable();
			if( identifiers == null )
				identifiers = new List<int>();
			else
				identifiers.Clear();

			foreach(Dialog d in list){
				hash.Add(d.id, d);
				identifiers.Add(d.id);
			}
		}else{
			hash.Clear();
			if( identifiers == null )
				identifiers = new List<int>();
			else
				identifiers.Clear();

			foreach(Dialog d in list){
				hash.Add(d.id, d);
				identifiers.Add(d.id);
			}
		}
	}

	public int Count{
		//get{ return list.Count; }
		get{ return hash.Count; }
	}
	
	public void AddDialog( Dialog dialog ){
		if( Count == 0 ){
			root = dialog.id;
			current = root;
		}
		list.Add(dialog);
		identifiers.Add(dialog.id);
		hash.Add(dialog.id, dialog);
	}
	
	public Dialog Get(int index){
		//return list[index];
		return (Dialog)hash[identifiers[index]];
	}

	public Dialog Value(int key){
		return (Dialog)hash[key];
	}

	public Dialog Current{
		get{ return (Dialog)hash[current]; }
	}
	public bool MoveNext(){
		if( ((Dialog)hash[current]).children.Count > 0 ){
			current = ((Dialog)hash[current]).children[0];
			return true;
		}
		return false;
	}
	public bool MoveNext(int option){
		if( ((Dialog)hash[current]).children.Count > 0 ){
			if( ((Dialog)hash[current]).children.Count > option ){
				current = ((Dialog)hash[current]).children[option];
				return true;
			}
		}
		return false;
	}

	public void Reset(){
		current = root;
	}

	public bool Remove( int key ){
		if( key == root ){
			Debug.LogWarning( "The root cannot be removed from the set." );
			return false;
		}
		if( ((Dialog)hash[key] ).id == root ){
			Debug.LogWarning( "The root cannot be removed from the set." );
			return false;
		}
		list.Remove((Dialog)hash[key]);
		hash.Remove(key);
		identifiers.Remove(key);

		return true;
	}
	/*
	public bool Remove( int index ){
		if( list[index].id == root ){
			Debug.LogWarning( "The root cannot be removed from the set." );
			return false;
		}
		if( ((Dialog)hash[identifiers[index]]).id == root ){
			Debug.LogWarning( "The root cannot be removed from the set." );
			return false;
		}
		
		hash.Remove(identifiers[index]);
		identifiers.Remove( identifiers[index] );
		list.Remove(list[index]);
		return true;
	}
	*/
	public void Clear(){
		identifiers.Clear();
		hash.Clear();
		list.Clear();
	}
	
	
}
