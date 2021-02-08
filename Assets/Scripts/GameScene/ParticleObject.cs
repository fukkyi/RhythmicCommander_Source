using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    private ParticleSystem particle = null;
    public bool isPlaying = false;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void CheckPlaying()
    {
        if (!particle.isPlaying) {

            isPlaying = false;
            transform.position = Vector3.zero;
        }
    }

    public void Play(Transform playPos)
    {
        isPlaying = true;
        transform.position = playPos.position;
        particle.Play();
    }
}
