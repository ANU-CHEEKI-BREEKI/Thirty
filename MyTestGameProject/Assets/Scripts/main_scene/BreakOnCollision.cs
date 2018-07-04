using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Durabilityable), typeof(Collider2D))]
public class BreakOnCollision : MonoBehaviour
{
    Durabilityable durabilityable;
    Collider2D[] colliders;

    private void Awake()
    {
        colliders = GetComponents<Collider2D>();
        durabilityable = GetComponent<Durabilityable>();
        durabilityable.OnBreak += Durabilityable_OnBreak;
    }

    private void OnDestroy()
    {
        if(durabilityable!= null)
            durabilityable.OnBreak -= Durabilityable_OnBreak;
    }

    private void Durabilityable_OnBreak()
    {
        Break();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Unit>() != null)
        {
            Break();
        }
    }

    void Break()
    {
        Destroy(this);

        durabilityable.BreakDown();

        int cnt = colliders.Length;
        for (int i = 0; i < cnt; i++)
            Destroy(colliders[i]);
    }
}
