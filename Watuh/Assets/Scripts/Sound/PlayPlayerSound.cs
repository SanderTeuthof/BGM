using System.Collections;
using UnityEngine;

public class PlayPlayerSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip _jump;
    [SerializeField]
    private AudioClip _throw;
    [SerializeField]
    private AudioClip _dash;
    [SerializeField] 
    private AudioClip _teleport;
    [SerializeField] 
    private AudioClip _hit;
    [SerializeField] 
    private AudioClip[] _steps;
    [SerializeField] 
    private float _minStepSpeed = 0.1f;
    [SerializeField]
    private float _maxStepSpeed = 1.5f;
    [SerializeField]
    private EasingType _easingType = EasingType.Linear;
    [SerializeField]
    private GameEvent _eventToPlay;

    private Movement _movement;
    private CharacterController _characterController;

    private void Start()
    {
        _movement = GetComponent<Movement>();
        _characterController = GetComponent<CharacterController>();

        StartCoroutine(DoSteps());
    }

    private IEnumerator DoSteps()
    {
        while (true)
        {
            if (_movement.Momentum > 0.1f)
            {
                // Calculate the adjusted step speed using the easing function
                float adjustedStepInterval = GetEasedStepInterval(_movement.Momentum);

                Debug.Log("Walking with interval: " + adjustedStepInterval);
                yield return new WaitForSeconds(adjustedStepInterval);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (_characterController.isGrounded)
            {
                PlayStepSound();
            }
        }
    }

    private void PlayStepSound()
    {
        if (_steps.Length > 0)
        {
            AudioClip stepClip = _steps[Random.Range(0, _steps.Length)];
            _eventToPlay.Raise(this, stepClip);
        }
    }

    public void PlayJump()
    {
        _eventToPlay.Raise(this, _jump);
    }

    public void PlayThrow()
    {
        _eventToPlay.Raise(this, _throw);
    }

    public void PlayDash()
    {
        _eventToPlay.Raise(this, _dash);
    }

    public void PlayTeleport()
    {
        _eventToPlay.Raise(this, _teleport);
    }

    public void PlayHit()
    {
        _eventToPlay.Raise(this, _hit);
    }

    private float GetEasedStepInterval(float momentum)
    {
        float normalizedMomentum = Mathf.InverseLerp(1f, 40f, momentum);

        float easedMomentum = EasingFunctions.Ease(_easingType, normalizedMomentum);

        return Mathf.Lerp(_maxStepSpeed, _minStepSpeed, easedMomentum);
    }
}