using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAllUnitsInRangeOnAwake : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [SerializeField] float range;

    [SerializeField] bool onStart;

    private void Awake()
    {
        if (!onStart)
        {
            KillAll();
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        KillAll();
        Destroy(gameObject);
    }

    private void KillAll()
    {
        var units = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (var u in units)
        {
            var unit = u.GetComponent<Unit>();
            if (unit != null)
                unit.TakeHit(new Damage(0, unit.Stats.Health));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
