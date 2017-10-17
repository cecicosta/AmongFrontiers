using UnityEngine.Events;

public partial class PlayerInput : CharacterAttributes {
    [System.Serializable]
    public class OnValueChangesFloat : UnityEvent<float>{}

}
