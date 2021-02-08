using UnityEngine;

[CreateAssetMenu(fileName = "NotesObjectData", menuName = "ScriptableObject/NotesObjectData")]
public class NotesObjectData : ScriptableObject
{
    public GameObject[] normalNoteObj;

    public GameObject[] holdNoteObj;

    public GameObject barrageObj;
}
