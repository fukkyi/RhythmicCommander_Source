using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHoldNotes : HoldNotes
{
    public int NoteNumber { get; private set; } = 0;

    protected new void Awake() 
    {
        base.Awake();
        lineTrans.gameObject.AddComponent<BoxCollider>().isTrigger = true;
    }

    public override void Init(NotesStruct notesStruct, ChartStruct chartStruct, Material shinyMaterial = null, int noteNumber = 0)
    {
        base.Init(notesStruct, chartStruct, shinyMaterial, noteNumber);
        NoteNumber = noteNumber;
    }
}
