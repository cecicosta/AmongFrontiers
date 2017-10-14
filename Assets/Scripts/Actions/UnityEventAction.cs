using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEditor.Events;
using UnityEditor;

[System.Serializable]
public class UnityEventAction : ToolKitAction, ISerializationCallbackReceiver {
    [SerializeField]
    public UnityEvent onInteractionTrigged;
    public float delay = 0;
    private SerializedObject so;
    private SerializedProperty persistentCalls;
    private float started;

    private void OnEnable() {


        EditorApplication.modifierKeysChanged += Callback;
    }

    public override void Execute( GameObject dialogController ){
        started = Time.time;
        onInteractionTrigged.Invoke();
    }

    public void Callback() {
        //so = new UnityEditor.SerializedObject(this);
        //so.Update();
        //persistentCalls = so.FindProperty("onInteractionTrigged.m_PersistentCalls.m_Calls");
        //so.ApplyModifiedProperties();
        //so.SetIsDifferentCacheDirty();
        //so.UpdateIfRequiredOrScript();

        ////Debug.Log(persistentCalls.FindPropertyRelative("m_Arguments.m_FloatArgument").ToString());
        ////SerializedProperty prop = so.GetIterator();
        ////do {
        ////    Debug.Log(persistentCalls.name);

        ////} while (persistentCalls.Next(true));


        //for (int i = 0; i < persistentCalls.arraySize; ++i)
        //    // in this example I'm just logging it
        //    Debug.Log(persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments.m_ObjectArgument"));
    }


    public override bool isStarted(){
		return true;
	}
	public override bool isFinished(){
		return Time.time - started > delay;
	}

    public void OnBeforeSerialize() {
        // assuming you have a reference to your Button component as m_Button

        //for (int i = 0; i < persistentCalls.arraySize; ++i)
        //    // in this example I'm just logging it
        //    Debug.Log(persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments.m_IntArgument").intValue);
        //persistentCalls.Dispose();
        //so.Dispose();


    }

    public void OnAfterDeserialize() {
    }
}
