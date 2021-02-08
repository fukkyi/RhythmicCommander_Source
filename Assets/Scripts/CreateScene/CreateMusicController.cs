using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SFB;
using TMPro;
using System.IO;

public class CreateMusicController : MonoBehaviour
{
    public bool isOpen = false;

    [SerializeField]
    private ChartDataController chartDataController = null;
    [SerializeField]
    private CreateChartController createChartController = null;

    [SerializeField]
    private GameObject playIcon = null;
    [SerializeField]
    private GameObject stopIcon = null;

    [SerializeField]
    private TextMeshProUGUI musicNameText = null;

    [SerializeField]
    private TMP_InputField titleIF = null;
    [SerializeField]
    private TMP_InputField composerIF = null;
    [SerializeField]
    private TMP_InputField previewTimeIF = null;
    [SerializeField]
    private TMP_InputField musicVolumeIF = null;

    [SerializeField]
    private Animator windowAnim = null;

    private AudioClip audioClip = null;

    public AudioSource audioSource;

    private string _musicDataPath = string.Empty;
    public string musicDataPath { get { return _musicDataPath; } private set { _musicDataPath = value; } }

    public void Init()
    {
        SetInputFieldText();
    }

    public void SetInputFieldText()
    {
        titleIF.text = chartDataController.musicStruct.musicName;
        composerIF.text = chartDataController.musicStruct.composer;
        previewTimeIF.text = chartDataController.musicStruct.previewTime.ToString("F3");
        musicVolumeIF.text = chartDataController.musicStruct.musicVolume.ToString("F3");
    }

    public void ImportMusic()
    {
        var extension = new[] { new ExtensionFilter("ogg file", "ogg") };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open MusicFile", Application.streamingAssetsPath, extension, false);

        if (paths.Length == 0) return;

        musicDataPath = paths[0];

        string fileName = Path.GetFileNameWithoutExtension(musicDataPath);        

        StartCoroutine(SetAudioClipFromPath(musicDataPath, fileName));
    }

    public IEnumerator SetAudioClipFromPath(string musicPath, string fileName, bool setLength = true)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(musicPath, Constant.musicAudioType)) {

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError) {

                Debug.LogError(request.error);
                yield break;
            }

            audioClip = DownloadHandlerAudioClip.GetContent(request);
            audioClip.name = fileName;
        }

        audioSource.clip = audioClip;
        chartDataController.SetMusicFileName(fileName);

        if (setLength) {
            createChartController.AdjustChartLength();
        }

        SetMusicText();
    }

    public void PreviewMusic()
    {
        if (audioSource.clip == null) return;

        if (audioSource.isPlaying) {

            playIcon.SetActive(true);
            stopIcon.SetActive(false);
            audioSource.Stop();
        }
        else {

            playIcon.SetActive(false);
            stopIcon.SetActive(true);
            PlayMusic();
        }
    }

    public void PlayMusic(float time = 0)
    {
        if (audioSource.clip == null) return;

        if (audioSource.clip.length <= time) return;

        audioSource.time = time;
        audioSource.Play();
    }

    public void SetMusicText()
    {
        musicNameText.SetText(audioSource.clip.name);
    }

    public void SetMusicTitle(string title)
    {
        chartDataController.SetMusicName(title);
    }

    public void SetMusicComposer(string composer)
    {
        chartDataController.SetComposer(composer);
    }

    public void SetPreviewTime(string previewTime)
    {
        if (previewTime == string.Empty) return;

        chartDataController.SetPreviewTime(float.Parse(previewTime));
    }

    public void SetMusicVolume(string volume)
    {
        if (volume == string.Empty) return;

        float musicVolume = float.Parse(volume);

        if (musicVolume > 1.0f || musicVolume < 0) {
            musicVolume = Mathf.Clamp(musicVolume, 0, 1.0f);
            musicVolumeIF.text = musicVolume.ToString("F3");
        }

        audioSource.volume = musicVolume;

        chartDataController.SetMusicVolume(musicVolume);
    }

    public void PlayWindowAnim(bool show)
    {
        isOpen = show;
        windowAnim.SetBool("Show", show);
    }

    public void RemoveMusic()
    {
        audioSource.clip = null;
        musicNameText.SetText("No Music");
    }
}
