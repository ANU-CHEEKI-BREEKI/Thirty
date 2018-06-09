using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Durabilityable), typeof(Collider2D))]
public class BreakOnCollision : MonoBehaviour
{
    Durabilityable durabilityable;
    Collider2D[] colliders;

    void Start ()
    {
        colliders = GetComponents<Collider2D>();
        durabilityable = GetComponent<Durabilityable>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Unit>() != null)
        {
            Destroy(this);

            durabilityable.BreakDown();
    
            int cnt = colliders.Length;
            for (int i = 0; i < cnt; i++)
                Destroy(colliders[i]);
        }
    }
}
