using UnityEngine;
using TMPro;

public class NotesSpeedOption : Option
{
    private float notesSpeed = 1.0f;
    private float[] bpm = null;

    [SerializeField]
    private MusicDataController musicData = null;

    [SerializeField]
    private TextMeshProUGUI bpmText = null;
    [SerializeField]
    private TextMeshProUGUI maxBpmText = null;
    [SerializeField]
    private TextMeshProUGUI speedText = null;
    [SerializeField]
    private TextMeshProUGUI resultText = null;
    [SerializeField]
    private TextMeshProUGUI maxResultText = null;

    public override void Init()
    {
        notesSpeed = StaticValue.noteSpeed;
        valueText.SetText("{0:2}", notesSpeed);
    }

    public override void ChangeValue(bool up, bool down, bool submit)
    {
        if (up || down) {

            float mulutiple = 1.0f;

            if (submit) mulutiple = 0.1f;

            if (up) notesSpeed += 0.1f * mulutiple;
            if (down) notesSpeed -= 0.1f * mulutiple;

            float clampNotesSpeed = Mathf.Clamp(notesSpeed, 0.5f, 10.0f);
            if (clampNotesSpeed == notesSpeed) Find.GetAudioManager().PlayOneShotSE("ValueMove");
            notesSpeed = clampNotesSpeed;

            StaticValue.noteSpeed = notesSpeed;

            valueText.SetText("{0:2}", notesSpeed);
            ChangeDitailUI();
        }
    }

    public override void Selected()
    {
        detailParent.gameObject.SetActive(true);
        bpm = musicData.GetSelectingMusicStruct().bpms[(int)musicData.selectingDif];

        if (bpm == null) {

            bpm = new float[1];
            bpm[0] = 150f;
        }

        if (bpm.Length == 2) {

            maxBpmText.enabled = true;
            maxResultText.enabled = true;
        }
        else {

            maxBpmText.enabled = false;
            maxResultText.enabled = false;
        }

        ChangeDitailUI();
    }

    public override void UnSelected()
    {
        detailParent.gameObject.SetActive(false);
    }

    public void ChangeDitailUI()
    {
        if (bpm.Length == 2) {

            float maxResultSpeed = bpm[1] * notesSpeed;

            maxBpmText.SetText("{0:0}", bpm[1]);
            maxResultText.SetText("{0:0}", maxResultSpeed);
        }

        bpmText.SetText("{0:0}", bpm[0]);
        speedText.SetText("{0:2}", notesSpeed);
        resultText.SetText("{0:0}", notesSpeed * bpm[0]);
    }
}
