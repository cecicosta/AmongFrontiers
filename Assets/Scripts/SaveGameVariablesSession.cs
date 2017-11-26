using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameVariablesSession : Singleton<SaveGameVariablesSession> {

    public Dictionary<string, Condition> variables = new Dictionary<string, Condition>();

	public void SaveVariable(Condition condition) {
        if (variables.ContainsKey(condition.identifier)) {
            variables.Remove(condition.identifier);
        }
        DontDestroyOnLoad(this.gameObject);
        variables.Add(condition.identifier, condition);
    }

    public Condition LoadGameVariable(string identifier) {
        Condition tmp;
        if (!variables.TryGetValue(identifier, out tmp)) {
            return null;
        }
        return tmp;
    }
}
