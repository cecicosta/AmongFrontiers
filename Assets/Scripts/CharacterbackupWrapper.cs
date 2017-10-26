using UnityEngine;
public class CharacterbackupWrapper: MonoBehaviour {
    public void Backup() {
        CurrentCharacterBackup.Instance.BackupCurrentCharacter();
    }

    public void Restore() {
        CurrentCharacterBackup.Instance.RestoreCharacterBackup();
    }

    public void Invalidate() {
        CurrentCharacterBackup.Instance.InvalidateBackup();
    }
}
