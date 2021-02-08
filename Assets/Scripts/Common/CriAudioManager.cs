using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriAudioManager : MonoBehaviour
{
    public static CriAudioManager Instance { get; private set; }

    private CriAtomSource[] atomSources = null;

    private void Awake()
    {
        if (Instance != null) return;

        Instance = this;
        DontDestroyOnLoad(gameObject);

        atomSources = GetComponentsInChildren<CriAtomSource>();
    }

    public void PlaySE(string name)
    {
        foreach(CriAtomSource atomSource in atomSources)
        {
            if (atomSource.cueName == name)
            {
                atomSource.Play();
                break;
            }
        }
    }
}
