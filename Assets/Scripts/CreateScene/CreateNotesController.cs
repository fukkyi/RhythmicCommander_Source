using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CreateNotesController : NotesController
{
    public int playTiming = 0;
    public float noteSpeed = 1.0f;

    [SerializeField]
    private Camera noteCamera = null;

    [SerializeField]
    private ChartDataController chartData = null;
    [SerializeField]
    private CreateController createController = null;
    [SerializeField]
    private CreateChartController chartController = null;
    [SerializeField]
    private CreateMusicController createMusicController = null;

    [SerializeField]
    private Slider speedSlider = null;
    [SerializeField]
    private Toggle[] shinyToggles = null;
    [SerializeField]
    private TMP_InputField[] speedIFs = null;
    [SerializeField]
    private TMP_InputField[] lengthIFs = null;
    [SerializeField]
    private TextMeshProUGUI speedText = null;
    [SerializeField]
    private GameObject tempoCangerObj = null;
    [SerializeField]
    private GameObject chartEffecterObj = null;
    [SerializeField]
    private RectTransform[] notesOptions = null;

    private List<TempoChangerBar> changerBars = new List<TempoChangerBar>();
    private List<ChartEffecterBar> effecterBars = new List<ChartEffecterBar>();

    private NotesType generateNoteType = NotesType.Normal;
    private int noteLength = 960;
    private int barrageCount = 5;
    private float notesOriginalSpeed = 1.0f;
    private bool isShiny = false;
    private float changeBpm = 150;
    private int changeBeat = 4;
    private int changeBeatParam = 4;
    private float effecterSpeed = 1.0f;

    private int[][] removeNotesNumber = null;
    private string[] inputName = null;
    private bool isRemoveMode = false;
    private bool isPlayClap = false;

    public void Init(GameObject[] laneObjs)
    {
        int laneLength = laneObjs.Length;

        Array.Resize(ref removeNotesNumber, laneLength);
        Array.Resize(ref notes, laneLength);
        Array.Resize(ref inputName, laneLength);

        for (int i = 0; i < laneLength; i++)
        {
            Array.Resize(ref removeNotesNumber[i], 0);
            Array.Resize(ref notes[i], 0);
            inputName[i] = Enum.GetName(typeof(InputButton), i);
        }
        speedText.SetText("{0:2}", speedSlider.value);
    }

    public void RefreshNotes(bool isPlaying = false)
    {
        isPlayClap = isPlaying;
        UpdateManage(noteSpeed);

        ChartStruct chart = chartData.GetSelectingChart();

        foreach (TempoChangerBar changerBar in changerBars)
        {
            //changerBar.RefreshCount(chart);
            changerBar.MovePos(timeCountroller.ShowTimingCount, noteSpeed, timeCountroller.EnableShowCount);
        }
        foreach (ChartEffecterBar effecterBar in effecterBars)
        {
            //effecterBar.RefreshCount(chart);
            effecterBar.MovePos(timeCountroller.ShowTimingCount, noteSpeed, timeCountroller.EnableShowCount);
        }
    }

    protected override void UpdateNotes(NormalNotes notes, int showTiming, float noteSpeed)
    {
        base.UpdateNotes(notes, showTiming, noteSpeed);

        if (!isPlayClap) return;

        notes.MightPlayClap(timeCountroller.PlayFlame, playTiming);
    }

    public void InputManage()
    {
        if (createMusicController.isOpen || chartController.isOpen) return;

        for (int i = 0; i < inputName.Length; i++)
        {
            if (Input.GetButtonDown(inputName[i]))
            {
                GenerateFromLane(i);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = noteCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                ClickManage(hit);
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
        }
    }

    private void ClickManage(RaycastHit hit)
    {
        GameObject noteObj = null;

        if (hit.collider.CompareTag("HoldNote"))
        {
            noteObj = hit.collider.transform.parent.gameObject;
        }
        else
        {
            noteObj = hit.collider.gameObject;
        }

        if (hit.collider.CompareTag("TempoChanger"))
        {
            TempoChangerBar tempoChanger = noteObj.GetComponent<TempoChangerBar>();

            if (isRemoveMode)
            {
                changerBars.Remove(tempoChanger);
                chartData.SetTempoChanger(changerBars.ToArray());
                Destroy(tempoChanger.gameObject);
                Find.GetAudioManager().PlayOneShotSE("Cancel");

                chartData.ReFreshChart();
            }
        }
        else if (hit.collider.CompareTag("ChartEffecter"))
        {
            ChartEffecterBar chartEffecter = noteObj.GetComponent<ChartEffecterBar>();

            if (isRemoveMode)
            {
                effecterBars.Remove(chartEffecter);
                chartData.SetChartEffecter(effecterBars.ToArray());
                Destroy(chartEffecter.gameObject);
                Find.GetAudioManager().PlayOneShotSE("Cancel");

                chartData.ReFreshChart();
            }
        }
        else
        {
            NormalNotes noteComponent = noteObj.GetComponent<NormalNotes>();
            NotesStruct notesStruct = noteComponent.GetNoteData();

            if (isRemoveMode)
            {
                int noteNumber = 0;
                if (noteComponent is CreateNormalNotes)
                {
                    noteNumber = ((CreateNormalNotes)noteComponent).NoteNumber;
                }
                else if (noteComponent is CreateHoldNotes)
                {
                    noteNumber = ((CreateHoldNotes)noteComponent).NoteNumber;
                }
                else if (noteComponent is CreateBarrageNotes)
                {
                    noteNumber = ((CreateBarrageNotes)noteComponent).NoteNumber;
                }

                RemoveNote(noteNumber, notesStruct.lane - 1);
                Find.GetAudioManager().PlayOneShotSE("Cancel");
            }
        }
    }

    public void GenerateNotesFromChart()
    {
        AllRemoveNote();

        ChartStruct chart = chartData.GetSelectingChart();
        if (chart.notes == null) return;

        for (int i = 0; i < notes.Length; i++)
        {
            Array.Resize(ref removeNotesNumber[i], 0);
            Array.Resize(ref notes[i], 0);
        }

        foreach (NotesStruct note in chart.notes)
        {
            GenerateFromLane(note.lane - 1, chart, note);
        }

        if (chart.tempoChangers != null)
        {
            foreach (TempoChangerStruct tempoChanger in chart.tempoChangers)
            {
                GenerateTempoChanger(tempoChanger);
            }
        }

        if (chart.chartEffecters != null)
        {
            foreach (ChartEffecterStruct chartEffecter in chart.chartEffecters)
            {
                GenerateChartEffecter(chartEffecter);
            }
        }

        chartData.SetTempoChanger(changerBars.ToArray());
        chartData.SetChartEffecter(effecterBars.ToArray());
        RefreshNotes();
    }

    // 引数に入れるレーン番号は1から
    public void GenerateFromLane(int lane, ChartStruct chart = null, NotesStruct notesStruct = null)
    {
        int timing = timeCountroller.PlayTimingCount;
        int showTiming = timeCountroller.ShowTimingCount;
        int longNoteLength = noteLength;
        int endTimingCount = timing + noteLength;
        int endShowTiming = showTiming + noteLength;
        int count = barrageCount;
        NotesType generateType = generateNoteType;
        float speed = notesOriginalSpeed;
        bool isShinyNote = isShiny;

        if (notesStruct != null)
        {
            timing = notesStruct.timing;
            endTimingCount = timing + notesStruct.length;
            longNoteLength = notesStruct.length;
            generateType = notesStruct.type;
            isShinyNote = notesStruct.shiny;
            count = notesStruct.count;
            speed = notesStruct.speed;
        }

        if (generateType == NotesType.TempoChanger)
        {
            GenerateTempoChanger();
            changerBars.Sort((x, y) => x.TimingCount - y.TimingCount);
            chartData.SetTempoChanger(changerBars.ToArray());
            chartData.ReFreshChart();

            return;
        }

        if (generateType == NotesType.ChartEffecter)
        {
            GenerateChartEffecter();
            effecterBars.Sort((x, y) => x.TimingCount - y.TimingCount);
            chartData.SetChartEffecter(effecterBars.ToArray());
            chartData.ReFreshChart();

            return;
        }

        if (chart == null) chart = chartData.GetSelectingChart();

        if (generateType == NotesType.Barrage) lane = 2;

        int timingFlame = Clac.TimingFlame(timing, chart);
        int endTimingFlame = 0;

        if (generateType != NotesType.Normal)
        {
            endTimingFlame = Clac.TimingFlame(endTimingCount, chart);
        }

        if (!ValidationNote(lane, timingFlame, endTimingFlame, timing, endTimingCount, generateType))
        {
            Find.GetAudioManager().PlayOneShotSE("Cancel");
            return;
        }

        GameObject note = GenerateNote(generateType, lane + 1, createController.laneObj);

        int generateCount = notes[lane].Length;
        if (removeNotesNumber[lane].Length > 0)
        {
            generateCount = PopRemoveNumber(lane);
        }
        else
        {
            Array.Resize(ref notes[lane], generateCount + 1);
        }

        if (notesStruct == null)
        {
            notesStruct = new NotesStruct();
            notesStruct.timing = timing;
            notesStruct.lane = lane + 1;
            notesStruct.length = longNoteLength;
            notesStruct.shiny = isShinyNote;
            notesStruct.type = generateType;
            notesStruct.count = count;
            notesStruct.speed = speed;
        }

        Debug.Log(count);

        notes[lane][generateCount] = note.GetComponent<NormalNotes>();

        // 光るノーツかどうか
        if (isShinyNote && generateType != NotesType.Barrage)
        {
            notes[lane][generateCount].Init(notesStruct, chart, shinyMaterial, generateCount);
        }
        else
        {
            notes[lane][generateCount].Init(notesStruct, chart, null, generateCount);
        }

        notes[lane][generateCount].MovePos(timing, noteSpeed, timeCountroller.EnableShowCount);
    }

    public void GenerateTempoChanger(TempoChangerStruct changerStruct = null)
    {
        ChartStruct chart = chartData.GetSelectingChart();
        TempoChangerBar tempoChanger = Instantiate(tempoCangerObj, createController.laneObj[0].transform.parent).GetComponent<TempoChangerBar>();

        int timing = timeCountroller.PlayTimingCount;
        float bpm = changeBpm;
        int beat = changeBeat;
        int beatParam = changeBeatParam;

        if (changerStruct != null)
        {
            timing = changerStruct.timing;
            bpm = changerStruct.bpm;
            beat = changerStruct.beat;
            beatParam = changerStruct.beatParam;
        }

        tempoChanger.Init(timing, bpm, beat, beatParam, chart);
        tempoChanger.MovePos(timeCountroller.PlayTimingCount, noteSpeed);
        tempoChanger.number = changerBars.Count;

        changerBars.Add(tempoChanger);
    }

    public void GenerateChartEffecter(ChartEffecterStruct effecterStruct = null)
    {
        ChartStruct chart = chartData.GetSelectingChart();
        ChartEffecterBar chartEffecter = Instantiate(chartEffecterObj, createController.laneObj[0].transform.parent).GetComponent<ChartEffecterBar>();

        int timing = timeCountroller.PlayTimingCount;
        float chartSpeed = effecterSpeed;

        if (effecterStruct != null)
        {
            timing = effecterStruct.timing;
            chartSpeed = effecterStruct.chartSpeed;
        }

        chartEffecter.Init(timing, chartSpeed, chart);
        chartEffecter.MovePos(timeCountroller.PlayTimingCount, noteSpeed);
        chartEffecter.number = effecterBars.Count;

        effecterBars.Add(chartEffecter);
    }

    private int PopRemoveNumber(int lane)
    {
        int popNumber = removeNotesNumber[lane].Length - 1;
        int resultNumber = removeNotesNumber[lane][popNumber];

        Array.Resize(ref removeNotesNumber[lane], popNumber);

        return resultNumber;
    }

    public void OnChangeSpeedSlide(float speed)
    {
        noteSpeed = speed;
        speedText.SetText("{0:2}", speed);
        chartData.ReFreshChart();
        RefreshNotes();
    }

    private bool ValidationNote(int lane, int timingFlame, int endTimingFlame, int timingCount, int endTimingCount, NotesType notesType)
    {
        bool result = true;

        if (notes == null) return result;

        if (timingCount == chartData.GetSelectingChart().chartLength) return false;

        if (notesType == NotesType.Hold || notesType == NotesType.Barrage)
        {
            if (timingFlame == endTimingFlame) return false;

            if (endTimingCount >= chartData.GetSelectingChart().chartLength) return false;
        }

        foreach (NormalNotes note in notes[lane])
        {
            if (note == null) continue;

            int noteTimingFlame = note.TimingFlame;
            int noteEndTimingFlame = 0;

            if (note.Type != NotesType.Normal)
            {
                LongNotes longNotes = (LongNotes)note;
                noteEndTimingFlame = longNotes.EndTimingFlame;
            }

            if (notesType == NotesType.Normal)
            {
                if (timingFlame == noteTimingFlame)
                {
                    result = false;
                    break;
                }

                if (noteEndTimingFlame == 0) continue;

                if (timingFlame >= noteTimingFlame && timingFlame <= noteEndTimingFlame)
                {
                    result = false;
                    break;
                }
            }
            else if (notesType == NotesType.Hold || notesType == NotesType.Barrage)
            {
                if (timingFlame <= noteTimingFlame && endTimingFlame >= noteTimingFlame)
                {
                    result = false;
                    break;
                }

                if (noteEndTimingFlame == 0) continue;

                if (timingFlame <= noteEndTimingFlame && endTimingFlame >= noteEndTimingFlame)
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }

    public void SetNotesType(int value)
    {
        generateNoteType = (NotesType)Enum.ToObject(typeof(NotesType), value);

        for (int i = 0; i < notesOptions.Length; i++)
        {
            if (i == value)
            {
                notesOptions[i].gameObject.SetActive(true);
            }
            else
            {
                notesOptions[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetShiny(bool shiny)
    {
        isShiny = shiny;

        foreach (Toggle toggle in shinyToggles)
        {
            toggle.isOn = shiny;
        }
    }

    public void SetLength(string length)
    {
        if (length == string.Empty) return;
        noteLength = int.Parse(length);

        foreach (TMP_InputField inputField in lengthIFs)
        {
            inputField.text = length;
        }
    }

    public void SetCount(string count)
    {
        if (count == string.Empty) return;
        barrageCount = int.Parse(count);
    }

    public void SetOriginalSpeed(string noteSpeed)
    {
        if (noteSpeed == string.Empty) return;
        notesOriginalSpeed = float.Parse(noteSpeed);

        foreach (TMP_InputField inputField in speedIFs)
        {
            inputField.text = notesOriginalSpeed.ToString("F2");
        }
    }

    public void ResetNotesClap()
    {
        foreach (NormalNotes[] laneNote in notes)
        {
            foreach (NormalNotes note in laneNote)
            {
                if (note == null) continue;
                note.ResetPlayedClap();
            }
        }
    }
    public void SetRemoveMode(bool removeMode)
    {
        isRemoveMode = removeMode;
    }

    public void RemoveNote(int noteNumber, int lane)
    {
        Destroy(notes[lane][noteNumber].gameObject);
        notes[lane][noteNumber] = null;

        Array.Resize(ref removeNotesNumber[lane], removeNotesNumber[lane].Length + 1);
        removeNotesNumber[lane][removeNotesNumber[lane].Length - 1] = noteNumber;
    }

    public void AllRemoveNote()
    {
        for (int i = 0; i < notes.Length; i++)
        {
            for (int j = 0; j < notes[i].Length; j++)
            {
                if (notes[i][j] == null) continue;
                Destroy(notes[i][j].gameObject);
            }
            Array.Resize(ref removeNotesNumber[i], 0);
        }

        foreach (TempoChangerBar changerBar in changerBars)
        {
            Destroy(changerBar.gameObject);
        }

        changerBars.Clear();
    }

    public NotesStruct[] makeNotesSrtucts()
    {
        if (notes == null) return null;

        List<NotesStruct> notesStructs = new List<NotesStruct>();

        foreach (NormalNotes[] noteList in notes)
        {
            foreach (NormalNotes note in noteList)
            {
                if (note == null) continue;
                notesStructs.Add(note.GetNoteData());
            }
        }

        notesStructs.Sort((x, y) => x.timing - y.timing);

        return notesStructs.ToArray();
    }

    public void SetChangeBpm(string bpm)
    {
        if (bpm == string.Empty) return;
        changeBpm = float.Parse(bpm);
    }

    public void SetChangeBeat(string beat)
    {
        if (beat == string.Empty) return;
        changeBeat = int.Parse(beat);
    }

    public void SetChangeBeatParam(string beatParam)
    {
        if (beatParam == string.Empty) return;
        changeBeatParam = int.Parse(beatParam);
    }

    public void SetEffecterSpeed(string speed)
    {
        if (speed == string.Empty) return;
        effecterSpeed = float.Parse(speed);
    }
}
