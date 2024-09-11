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
        CheckDeath();
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
        if (collision.gameObject.layer != 7) return;
        _npcBehaviourIdleStatesManager.SetNewState(NPCBehaviourStates.GotHit, collision.gameObject);
    }
}
