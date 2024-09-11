using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    protected float _maxHealth = 10;

    protected float _health;

    private IDestroyable _destroyable;
    private ObjectInfo _objectInfo;

    private NPCBehaviourStateManager _npcBehaviourIdleStatesManager;

    private bool _isHit;

    private void Awake()
    {
        _health = _maxHealth;
        _destroyable = GetComponent<IDestroyable>();
        _objectInfo = GetComponent<ObjectInfo>();
    }

    private void Start()
    {
        _npcBehaviourIdleStatesManager = GetComponent<NPCBehaviourStateManager>();
    }

    public virtual void TakeDamage(float damage)
    {
        StartCoroutine(IsTridentInside());
    }

    public virtual void Heal(float healAmount)
    {
        _health = Mathf.Min(_maxHealth, _health + healAmount);
    }

    private void CheckDeath()
    {
        _destroyable.Destroy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 7 || _isHit) return;
        _npcBehaviourIdleStatesManager.SetNewState(NPCBehaviourStates.GotHit, collision.gameObject);
        _isHit = true;
    }

    private IEnumerator IsTridentInside()
    {
        yield return new WaitForSeconds(1f);
        if(GetComponentInChildren<Trident>() != null) StartCoroutine(IsTridentInside());
        else CheckDeath();
    }
}
