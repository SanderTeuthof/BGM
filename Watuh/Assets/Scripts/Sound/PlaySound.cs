using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField]
    private AudioClip _clipToPlay;
    [SerializeField]
    private GameEvent _eventToPlay;

    public void Play()
    {
        _eventToPlay.Raise(this, _clipToPlay);
    }
}
