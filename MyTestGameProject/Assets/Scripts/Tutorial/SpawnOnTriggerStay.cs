using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpawnOnTriggerStay : MonoBehaviour
{
    [SerializeField] ArrowsValley original;
    ArrowsValley instance;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        SquadTriggerInitiator initiator = null;

        if (collision != null)
            initiator = collision.GetComponent<SquadTriggerInitiator>();

        if (initiator == null && initiator.Squad == null)
            return;

        if (instance == null)
        {
            instance = Instantiate(original, Vector3.zero, transform.rotation) as ArrowsValley;
            instance.Init(initiator.transform.position, instance.damage, 9, 300);
            instance.StartValley();
        }
    }

}
