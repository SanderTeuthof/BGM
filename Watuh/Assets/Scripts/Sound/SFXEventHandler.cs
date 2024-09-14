using UnityEngine;

public class SFXEventHandler: MonoBehaviour
{
    [SerializeField]
    private FloatReference _sfxVolume;
    private SFXHandler _handler;

    private void Start()
    {
        _handler = SFXHandler.Instance;
    }

    public void PlayAudio(Component sender, object data)
    {
        AudioClip clip = data as AudioClip;
        if (clip != null) 
            _handler.PlaySFXClip(clip, sender.transform.position, _sfxVolume.value);
    }
}