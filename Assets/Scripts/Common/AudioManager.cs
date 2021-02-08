using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource sourceBgm = null;
    [SerializeField]
    private AudioSource sourceSe = null;

    [SerializeField]
    private AudioClip[] clipBgms;
    [SerializeField]
    private AudioClip[] clipSes;

    public void PlayBGM(string bgmName, bool loop = false)
    {
        sourceBgm.loop = loop;

        AudioClip playClip = FindClipToName(bgmName, clipBgms);

        sourceBgm.clip = playClip;
        sourceBgm.Play();
    }

    public void PlaySE(string seName)
    {
        if (sourceSe.clip == null) {

            AudioClip playClip = FindClipToName(seName, clipSes);
            sourceSe.clip = playClip;
        }
        else if (seName != sourceSe.clip.name) {

            AudioClip playClip = FindClipToName(seName, clipSes);
            sourceSe.clip = playClip;
        }

        sourceSe.Play();
    }

    public void PlayOneShotSE(string seName)
    {
        AudioClip playClip = FindClipToName(seName, clipSes);

        if (playClip == null) return;

        sourceSe.PlayOneShot(playClip);
    }

    public AudioClip FindClipToName(string clipName, AudioClip[] clips)
    {
        AudioClip playClip = null;

        foreach (AudioClip clip in clips) {

            if (clip.name == clipName) {

                playClip = clip;
                break;
            }
        }

        return playClip;
    }
}
