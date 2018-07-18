using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ActivateOnTriggerEnter : MonoBehaviour
{
    [SerializeField] GameObject[] toActivate;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (toActivate == null || toActivate.Length == 0)
        {
            toActivate = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                toActivate[i] = transform.GetChild(i).gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SquadTriggerInitiator initiator = null;

        if (collision != null)
            initiator = collision.GetComponent<SquadTriggerInitiator>();

        if (initiator == null && initiator.Squad == null)
            return;

        foreach (var go in toActivate)
            go.SetActive(true);
    }
}
