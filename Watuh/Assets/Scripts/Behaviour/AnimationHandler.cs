using System.Collections;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetAnimationBool(string name, bool value)
    {
        //_animator.SetBool(name, value);
    }

    public void SetAnimationBoolForDuration(string whichBool, bool newState, float duration)
    {
        //StartCoroutine(SetAnimationBoolCoroutine(whichBool, newState, duration));
    }

    private IEnumerator SetAnimationBoolCoroutine(string whichBool, bool newState, float duration)
    {
        SetAnimationBool(whichBool, newState);
        yield return new WaitForSeconds(duration);
        SetAnimationBool(whichBool, !newState);
    }
}
