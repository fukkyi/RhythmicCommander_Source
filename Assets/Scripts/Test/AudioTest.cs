using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioManager = null;
    [SerializeField]
    private AudioSource audioSource = null;
    [SerializeField]
    private AudioClip seClip = null;
    [SerializeField]
    private CriAtomSource atomSource = null;

    private bool isPushed = false;
    private double pushTime = 0f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Lane3") || Input.GetButtonDown("Lane1") || Input.GetButtonDown("Lane2") || Input.GetButtonDown("Lane4") || Input.GetButtonDown("Lane5"))
        {
            //audioManager.PlayOneShotSE("Tap_5");
            //audioManager.PlayOneShotSE("Tap_5_ogg");
            //audioSource.PlayOneShot(seClip);
            atomSource.Play();
            isPushed = true;
            pushTime = Time.realtimeSinceStartup;
        }

        if (isPushed && atomSource.status == CriAtomSource.Status.Playing)
        {
            Debug.Log(Time.realtimeSinceStartup - pushTime);
            isPushed = false;
        }
    }
}
