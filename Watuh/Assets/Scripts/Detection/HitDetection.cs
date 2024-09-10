using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public bool Impenetrable;
    public float DamageMultiplier = 1;

    private bool _penetrating;
    private float _attackPower;

    private bool _attacking = false;
    private ObjectInfo _objectInfo;

    private List<Transform> _damagedEnemiesList = new();

    private void Awake()
    {
        _objectInfo = transform.root.GetComponent<ObjectInfo>();
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    public void SetAttacking(bool attackState, bool penetrating, float attackPower)
    {
        _attacking = attackState;
        _penetrating = penetrating;
        _attackPower = attackPower;
        _damagedEnemiesList.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( !_attacking)
            return;

        HitDetection ColliderHit = other.transform.GetComponent<HitDetection>();
        Transform thingHit = ColliderHit.transform.root;
        if (thingHit == transform.root)
            return;

        if (_damagedEnemiesList.Contains(thingHit))
            return;

        if (ColliderHit.Impenetrable) //This order makes it that if you hit (for example) an enemy arm before a shield the shield doesn't trigger and damage is dealt
        {
            if(_penetrating)
                return; // don't deal damage to this, but you will to the next thing you hit, probably something more squishy

            //Call Stun, this will change the state and cancel the attack right away anyways
            Debug.Log(this + " got stunned");
            return;
        }

        if (ColliderHit.DamageMultiplier == 0)
            return; // if it deals no damage but is penetrable just ignore it

        HealthManager health = thingHit.GetComponent<HealthManager>();

        health.TakeDamage(_attackPower * ColliderHit.DamageMultiplier);
        
        _damagedEnemiesList.Add(thingHit);

    }
}
