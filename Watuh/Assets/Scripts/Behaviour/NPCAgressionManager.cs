using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectInfo), typeof(NPCBehaviourStateManager))]
public class NPCAgressionManager : MonoBehaviour
{
    [SerializeField]
    private DetectionSphere _detection;
    [SerializeField]
    private float _maxDistanceTillEscape = 0;

    private ObjectInfo _thisInfo;
    private NPCBehaviourStateManager _manager;

    private Dictionary<ObjectInfo, float> _enemies = new(); //the float would be an aggression value, when picking an attack this will decide who to attack
    private HashSet<ObjectInfo> _enemiesLeaving = new();
    private bool _attacking = false;
    private bool _attackCheck 
    {  
        get { return _attacking; } 
        set 
        {
            if (_attacking != value)
            {
                _attacking = value;
                if (_attacking)
                    PickNextTarget(this, EventArgs.Empty);
            }
        } 
    }

    private void Awake()
    {
        _thisInfo = GetComponent<ObjectInfo>();
        _manager = GetComponent<NPCBehaviourStateManager>();
        _manager.DoNewAttack += PickNextTarget;
        _detection.ObjectEntered += CheckEnemyEnter;
        _detection.ObjectExited += CheckEnemyExit;
        StartCoroutine(CheckLeavingEnemies());
    }

    private IEnumerator CheckLeavingEnemies()
    {
        bool maxDistanceSet = _maxDistanceTillEscape > 0;
        while (maxDistanceSet) 
        {
            yield return new WaitForSeconds(0.3f);
            List<ObjectInfo> leavingEnemies = new List<ObjectInfo>(_enemiesLeaving);
            foreach (ObjectInfo enemy in leavingEnemies)
            {
                yield return null; // only do 1 enemy a frame to minimize calculations

                if (enemy == null)
                {
                    _enemies.Remove(enemy);
                    _enemiesLeaving.Remove(enemy);
                    continue;
                }

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance > _maxDistanceTillEscape)
                {
                    _enemiesLeaving.Remove(enemy);
                    _enemies.Remove(enemy);
                }
            }
        }
    }

    private void CheckEnemyExit(object sender, GameObject e)
    {
        var enemyInfo = e.GetComponent<ObjectInfo>();
        if (enemyInfo == null) return;

        if (_enemies.ContainsKey(enemyInfo))
        {
            _enemiesLeaving.Add(enemyInfo);
        }

    }

    private void CheckEnemyEnter(object sender, GameObject e)
    {
        ObjectInfo enemy = e.GetComponent<ObjectInfo>();

        if (enemy == null) return;

        if (_enemies.ContainsKey(enemy)) return;

        
        if (enemy.TypeOfObject == ObjectTypes.Player)
        {
            _enemies[enemy] = 1f;
            _attackCheck = true;
        }
    }

    private void PickNextTarget(object sender, EventArgs e)
    {
        ObjectInfo target = null;
        float maxAggression = float.MinValue;

        foreach (var kvp in _enemies)
        {
            if (kvp.Value > maxAggression)
            {
                maxAggression = kvp.Value;
                target = kvp.Key;
            }
        }

        if (target != null)
        {
            _manager.SetNewState(NPCBehaviourStates.Attack, target.transform);
        }
        else
        {
            _attackCheck = false; 
            _manager.SetNewState(NPCBehaviourStates.Idle);
        }
    }

    private void OnDestroy()
    {
        _manager.DoNewAttack -= PickNextTarget;
        _detection.ObjectEntered -= CheckEnemyEnter;
        _detection.ObjectExited -= CheckEnemyExit;
    }
}
