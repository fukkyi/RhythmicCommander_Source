using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;
using System.Text;

public class ChartDataController : MonoBehaviour
{
    [SerializeField]
    private CreateTimeController timeController = null;
    [SerializeField]
    private CreateMeasureController measureController = null;
    [SerializeField]
    private CreateMusicController musicController = null;
    [SerializeField]
    private CreateNotesController notesController = null;
    [SerializeField]
    private CreateChartController chartController = null;

    public MusicStruct musicStruct { get { return _musicStruct; } }
    private MusicStruct _musicStruct;

    public Difficulty selectingDif { get { return _selectingDif; } }
    private Difficulty _selectingDif = Difficulty.Standard;

    public void SetMusicName(string musicName) { _musicStruct.musicName = musicName; }
    public void SetComposer(string composer) { _musicStruct.composer = composer; }
    public void SetMusicFileName(string musicFileName) { _musicStruct.musicFileName = musicFileName; }
    public void SetPreviewTime(float previewTime) { _musicStruct.previewTime = previewTime; }
    public void SetMusicVolume(float musicVolume) { _musicStruct.musicVolume = musicVolume; }
    public void SetSelectingDifficulty(int dif) { _selectingDif = (Difficulty)System.Enum.ToObject(typeof(Difficulty), dif); }
    public void SetBpm(float bpm) {
        _musicStruct.charts[(int)selectingDif].bpm = bpm;
        ReFreshChart();
    }
    public void SetBeatParam(int beatParam) {
        _musicStruct.charts[(int)selectingDif].beatParam = beatParam;
        ReFreshChart();
    }
    public void SetBeat(int beat) {
        _musicStruct.charts[(int)selectingDif].beat = beat;
        ReFreshChart();
    }
    public void SetLevel(float level) { _musicStruct.charts[(int)selectingDif].level = level; }
    public void SetDifficult() {
        _musicStruct.charts[(int)selectingDif].difficulty = selectingDif;
    }
    public void SetMusicOffset(float offset) {
        _musicStruct.charts[(int)selectingDif].musicOffset = offset;
        ReFreshChart();
    }
    public void SetChartLength(int length) {
        _musicStruct.charts[(int)selectingDif].chartLength = length;
        ReFreshChart();
    }
    public void SetNotesDesigner(string designer) { _musicStruct.charts[(int)selectingDif].notesDesigner = designer; }

    public void SetNotesStruct(NotesStruct[] notesStructs = null)
    {
        if (notesStructs == null) notesStructs = notesController.makeNotesSrtucts();

        _musicStruct.charts[(int)selectingDif].notes = notesStructs;
    }

    public ChartStruct GetSelectingChart()
    {
        return musicStruct.charts[(int)selectingDif];
    }

    public void SetTempoChanger(TempoChangerBar[] changerBars)
    {
        TempoChangerStruct[] tempoChanger = new TempoChangerStruct[0];

        for (int i = 0; i < changerBars.Length; i++) {

            System.Array.Resize(ref tempoChanger, tempoChanger.Length + 1);
            tempoChanger[tempoChanger.Length - 1] = changerBars[i].GetChangerStruct();
        }

        _musicStruct.charts[(int)selectingDif].tempoChangers = tempoChanger;
        timeController.SetTimeLength();
    }

    public void SetChartEffecter(ChartEffecterBar[] effecterBars)
    {
        ChartEffecterStruct[] chartEffecters = new ChartEffecterStruct[0];

        for (int i = 0; i < effecterBars.Length; i++)
        {
            System.Array.Resize(ref chartEffecters, chartEffecters.Length + 1);
            chartEffecters[chartEffecters.Length - 1] = effecterBars[i].GetEffecterStruct();
        }

        _musicStruct.charts[(int)selectingDif].chartEffecters = chartEffecters;
        timeController.SetTimeLength();
    }

    public void Init()
    {
        _musicStruct = new MusicStruct();

        _musicStruct.charts = new ChartStruct[5];

        for (int i = 0; i < 5; i++) {
            _musicStruct.charts[i] = new ChartStruct();
        }
    }

    public void ReFreshChart()
    {
        timeController.SetTimeLength();
        measureController.OnChangeChart();
    }

    public void SaveCharts()
    {
        SetNotesStruct();
        Chart.ChartUtility.WirteMusicStructData(AdjustMusicStruct(musicStruct), musicController.musicDataPath);
    }

    public void LoadCharts()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Open MusicFile", Application.streamingAssetsPath, false);

        if (paths.Length == 0) return;
        string directoryPath = paths[0];

        string[] files = Directory.GetFiles(directoryPath, "*" + Constant.chartExtension, SearchOption.TopDirectoryOnly);
        string chartFilePath = string.Empty;

        if (files.Length != 0) chartFilePath = files[0];

        StreamReader reader = new StreamReader(chartFilePath);
        string chartJson = reader.ReadToEnd();
        reader.Close();

        MusicStruct loadStruct = JsonUtility.FromJson<MusicStruct>(chartJson);

        _musicStruct.composer = loadStruct.composer;
        _musicStruct.musicFileName = loadStruct.musicFileName;
        _musicStruct.musicName = loadStruct.musicName;
        _musicStruct.previewTime = loadStruct.previewTime;
        _musicStruct.musicVolume = loadStruct.musicVolume;

        for (int i = 0; i < musicStruct.charts.Length; i++) {
            _musicStruct.charts[i] = new ChartStruct();
        }

        for (int i = 0; i < loadStruct.charts.Length; i++) {

            int difficulty = (int)loadStruct.charts[i].difficulty;
            _musicStruct.charts[difficulty] = loadStruct.charts[i];
        }

        musicController.RemoveMusic();
        musicController.SetInputFieldText();
        chartController.SetValueToUI();
        notesController.GenerateNotesFromChart();
        timeController.SetTimeLength();
        measureController.RemoveAllMeasure();
        measureController.Init();
        timeController.SetTime(0);
        ReFreshChart();

        if (musicStruct.musicFileName == string.Empty) return;

        string musicPath = MusicPath.getMusicDataPath(Path.GetFileName(directoryPath), musicStruct.musicFileName);
        StartCoroutine(musicController.SetAudioClipFromPath(musicPath, musicStruct.musicFileName, false));
    }

    private MusicStruct AdjustMusicStruct(MusicStruct musicStruct)
    {
        MusicStruct adjustStruct = new MusicStruct();

        adjustStruct.composer = musicStruct.composer;
        adjustStruct.musicFileName = musicStruct.musicFileName;
        adjustStruct.musicName = musicStruct.musicName;
        adjustStruct.previewTime = musicStruct.previewTime;
        adjustStruct.musicVolume = musicStruct.musicVolume;

        adjustStruct.charts = new ChartStruct[0];

        for (int i = 0; i < musicStruct.charts.Length; i++) {

            if (musicStruct.charts[i].notes == null) continue;
            if (musicStruct.charts[i].notes.Length == 0) continue;

            System.Array.Resize(ref adjustStruct.charts, adjustStruct.charts.Length + 1);

            adjustStruct.charts[adjustStruct.charts.Length - 1] = musicStruct.charts[i];
        }

        return adjustStruct;
    }
}
