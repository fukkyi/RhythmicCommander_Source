using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MusicAudioController : MonoBehaviour
{
    [SerializeField]
    private MusicDataController musicData = null;
    [SerializeField]
    private AudioSource audioSource = null;
    [SerializeField]
    private float fadeTime = 0.5f;
    [SerializeField]
    private float playLength = 10f;
    [SerializeField]
    private float musicPlayTime = 0.5f;

    private bool isWait = false;

    private bool isReady = false;
    private bool isFadeing = false;

    private float startpreviewTime = 0;
    private float currentMusicPlayTime = 0;

    public IEnumerator Init()
    {
        yield return null;
        LoadAndPlayMusic();
    }

    public IEnumerator LoadAndPlayMusic()
    {
        if (isWait) yield break;
        if (musicData.selectMusicElement == 0) yield break;

        isWait = true;
        // 最低限の読み込みにするため、選択されてから少し時間をおいてから再生。
        currentMusicPlayTime = 0;
        while (musicPlayTime > currentMusicPlayTime) {

            currentMusicPlayTime += Time.deltaTime;
            if (!isWait) yield break;
            yield return null;
        }

        isReady = false;

        MusicInfoStruct selectingMusic = musicData.GetSelectingMusicStruct();

        startpreviewTime = selectingMusic.previewTime;
        StartCoroutine(StreamingLoadMusic(selectingMusic.folderName, selectingMusic.musicFileName));
    }

    public void ResetIsWait()
    {
        isWait = false;
    }

    public void UpdateManage()
    {
        if (!isReady) return;

        if (audioSource.isPlaying) {

            CheckPlayingMusic();
        }
        else {

            PlayPreview();
        }
    }

    public void PlayPreview()
    {
        audioSource.time = startpreviewTime;
        audioSource.Play();

        StartCoroutine(FadeInAudio(musicData.GetSelectingMusicStruct().musicVolume));
    }

    public void StopMusic()
    {
        isReady = false;
        StopCoroutine(StreamingLoadMusic(string.Empty, string.Empty));

        if (audioSource.clip != null) {
            audioSource.clip.UnloadAudioData();
        }

        audioSource.Stop();
    }

    private void CheckPlayingMusic()
    {
        if (isFadeing) return;

        float elapsedTime = audioSource.time - startpreviewTime;

        if (elapsedTime >= playLength) {

            StartCoroutine(FadeOutAudio(musicData.GetSelectingMusicStruct().musicVolume));
        }
    }

    private IEnumerator StreamingLoadMusic(string directoryName, string musicName)
    {
        if (musicName == string.Empty) yield break;

        if (Constant.CompareEnv(GameEnvironment.local)) {

            using (var request = new UnityWebRequest("File://" + MusicPath.getMusicDataPath(directoryName, musicName))) {

                var handler = new DownloadHandlerAudioClip(string.Empty, Constant.musicAudioType);

                handler.compressed = false;
                handler.streamAudio = true;
                request.downloadHandler = handler;

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError) {

                    Debug.LogError(request.error);
                }
                else {

                    AudioClip loadedAudioClip = handler.audioClip;
                    loadedAudioClip.name = musicName;

                    audioSource.clip = loadedAudioClip;

                    isReady = true;
                    PlayPreview();
                }
            }
        }
        else if (Constant.CompareEnv(GameEnvironment.webGL)) {

            ResourceRequest request = Resources.LoadAsync<AudioClip>(Constant.audioResourceDirectory + musicName);
            while (!request.isDone) {
                yield return null;
            }

            audioSource.clip = request.asset as AudioClip;
            audioSource.clip.LoadAudioData();
            while(audioSource.clip.loadState != AudioDataLoadState.Loaded && audioSource.clip.loadState != AudioDataLoadState.Failed) {
                yield return null;
            }
            
            isReady = true;
            PlayPreview();
        }
    }

    public IEnumerator FadeInAudio(float musicVolume)
    {
        float currentFadeTime = 0;
        isFadeing = true;

        while(currentFadeTime < fadeTime) {

            currentFadeTime += Time.deltaTime;
            audioSource.volume = (currentFadeTime / fadeTime) * musicVolume;
            yield return null;
        }

        isFadeing = false;
        audioSource.volume = musicVolume;
    }

    public IEnumerator FadeOutAudio(float musicVolume)
    {
        float currentFadeTime = 0;
        isFadeing = true;

        while (currentFadeTime < fadeTime) {

            currentFadeTime += Time.deltaTime;
            audioSource.volume = musicVolume - currentFadeTime / fadeTime;
            yield return null;
        }

        isFadeing = false;
        audioSource.volume = 0;
        audioSource.Stop();
    }
}
