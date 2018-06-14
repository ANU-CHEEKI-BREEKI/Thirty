using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GOTriggerEvent : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, ITriggerEvent
{
    public enum TriggerType { STAY, ENTER, EXIT }

    [SerializeField] bool deleteOnTriggerActivates = true;

    ATriggerConstraint[] constraints;

    public event Action OnPlayerTriggerEnter;
    public event Action OnPlayerTriggerExit;
    public event Action OnPlayerTriggerStay;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if(col != null)
            col.isTrigger = true;

        constraints = GetComponents<ATriggerConstraint>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Trigger(collision, TriggerType.EXIT);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Trigger(collision, TriggerType.ENTER);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Trigger(collision, TriggerType.STAY);
    }

    void Trigger(Collider2D collision, TriggerType type)
    {
        SquadTriggerInitiator initiator = null;
        if (collision != null)
            initiator = collision.GetComponent<SquadTriggerInitiator>();

        if ((collision == null || (initiator != null && initiator.Squad != null)) && ConstraintsAreTrue())
        {
            switch (type)
            {
                case TriggerType.STAY:
                    if (OnPlayerTriggerStay != null)
                    {
                        OnPlayerTriggerStay();
                        if (deleteOnTriggerActivates)
                            Destroy(gameObject);
                    }
                    break;
                case TriggerType.ENTER:
                    if (OnPlayerTriggerEnter != null)
                    {
                        OnPlayerTriggerEnter(); if (deleteOnTriggerActivates)
                            Destroy(gameObject);
                    }
                    break;
                case TriggerType.EXIT:
                    if (OnPlayerTriggerExit != null)
                    {
                        OnPlayerTriggerExit();
                        if (deleteOnTriggerActivates)
                            Destroy(gameObject);
                    }
                    break;
            }


        }
    }

    bool ConstraintsAreTrue()
    {
        var res = true;
        int cnt = constraints.Length;

        for (int i = 0; i < cnt; i++)
        {
            if(!constraints[i].IsTrue)
            {
                res = false;
                break;
            }
        }

        return res;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Trigger(null, TriggerType.EXIT);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Trigger(null, TriggerType.ENTER);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Trigger(null, TriggerType.STAY);
    }
}
