using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXHandler : MonoBehaviour
{
    public static SFXHandler Instance;

    [SerializeField] private AudioSource _sFXObj;

    private List<AudioSource> audioSourcePool;
    private List<AudioSource> activeAudioSources;

    private int _maxAudioSources = 300;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSourcePool = new List<AudioSource>();
            activeAudioSources = new List<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        if (audioSourcePool.Count + activeAudioSources.Count >= _maxAudioSources)
        {
            return null;
        }

        AudioSource newSource = Instantiate(_sFXObj, Vector3.zero, Quaternion.identity);
        newSource.gameObject.SetActive(false);
        audioSourcePool.Add(newSource);
        return newSource;
    }

    private void ReturnAudioSourceToPool(AudioSource source)
    {
        source.clip = null;
        source.gameObject.SetActive(false);
        activeAudioSources.Remove(source);
        audioSourcePool.Add(source);
    }

    private IEnumerator PlayAudioCoroutine(AudioSource audioSource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        ReturnAudioSourceToPool(audioSource);
    }

    public void PlaySFXClip(AudioClip audioClip, Vector3 pos, float volume)
    {
        AudioSource audioSource = GetPooledAudioSource();
        if (audioSource == null)
            return;
        audioSource.gameObject.SetActive(true);
        audioSource.transform.position = pos;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        activeAudioSources.Add(audioSource);
        StartCoroutine(PlayAudioCoroutine(audioSource, audioClip.length));
    }

    public void PlayRandomSFXClip(AudioClip[] audioClips, Vector3 pos, float volume)
    {
        int rand = Random.Range(0, audioClips.Length);
        PlaySFXClip(audioClips[rand], pos, volume);
    }

    public AudioSource StartLoopingSFXClip(AudioClip audioClip, Vector3 pos, float volume)
    {
        AudioSource audioSource = GetPooledAudioSource();
        if(audioSource == null)
            return null;
        audioSource.transform.position = pos;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.gameObject.SetActive(true);
        audioSource.Play();

        activeAudioSources.Add(audioSource);
        return audioSource;
    }

    public void StopLoopingSFXClip(AudioSource audioSource)
    {
        if (audioSource != null && audioSource.loop)
        {
            audioSource.Stop();
            audioSource.loop = false;
            ReturnAudioSourceToPool(audioSource);
        }
    }
}