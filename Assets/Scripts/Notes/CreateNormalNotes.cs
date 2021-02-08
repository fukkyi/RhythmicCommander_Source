using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNormalNotes : NormalNotes
{
    public int NoteNumber { get; private set; } = 0;

    public override void Init(NotesStruct notesStruct, ChartStruct chartStruct, Material shinyMaterial = null, int noteNumber = 0)
    {
        base.Init(notesStruct, chartStruct, shinyMaterial, noteNumber);
        NoteNumber = noteNumber;
    }
}
