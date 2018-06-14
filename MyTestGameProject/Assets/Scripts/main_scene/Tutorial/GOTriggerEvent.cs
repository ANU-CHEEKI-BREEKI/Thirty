using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GOTriggerEvent : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDropHandler
{
    public enum TriggerType { STAY, ENTER, EXIT, DISABLE, PDOWN, PUP, PCLICK, PDROP, DESTROY }

    [SerializeField] bool deleteOnTriggerActivates = true;
    [SerializeField] bool deleteOnlyScript = false;

    [SerializeField] ATriggerConstraint[] constraints;

    public event Func<bool> OnPlayerTriggerEnter;
    public event Func<bool> OnPlayerTriggerExit;
    public event Func<bool> OnPlayerTriggerStay;
    public event Func<bool> OnPlayerTriggerDestroy;
    public event Func<bool> OnPlayerTriggerDisable;
    public event Func<bool> OnPlayerTriggerPoinderDown;
    public event Func<bool> OnPlayerTriggerPoinderUp;
    public event Func<bool> OnPlayerTriggerPointerClick;
    public event Func<bool> OnPlayerTriggerPointerDrop;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if(col != null)
            col.isTrigger = true;

        if(constraints.Length == 0)
            constraints = GetComponents<ATriggerConstraint>();
    }

    void DestroyOnTriggerActivates()
    {
        if (deleteOnTriggerActivates)
        {
            if (deleteOnlyScript)
                Destroy(this);
            else
                Destroy(gameObject);
        }
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
                        if(OnPlayerTriggerStay())
                            DestroyOnTriggerActivates();
                    break;
                case TriggerType.ENTER:
                    if (OnPlayerTriggerEnter != null)
                        if(OnPlayerTriggerEnter())
                            DestroyOnTriggerActivates();
                    break;
                case TriggerType.EXIT:
                    if (OnPlayerTriggerExit != null)
                        if(OnPlayerTriggerExit())
                            DestroyOnTriggerActivates();
                    break;
                case TriggerType.PDOWN:
                    if (OnPlayerTriggerPoinderDown != null)
                        if(OnPlayerTriggerPoinderDown())
                            DestroyOnTriggerActivates();
                    break;
                case TriggerType.PUP:
                    if (OnPlayerTriggerPoinderUp != null)
                        if(OnPlayerTriggerPoinderUp())
                            DestroyOnTriggerActivates();
                    break;
                case TriggerType.PCLICK:
                    if (OnPlayerTriggerPointerClick != null)
                        if(OnPlayerTriggerPointerClick())
                            DestroyOnTriggerActivates();
                    break;
                case TriggerType.PDROP:
                    if (OnPlayerTriggerPointerDrop != null)
                        if(OnPlayerTriggerPointerDrop())
                            DestroyOnTriggerActivates();
                    break;
                case TriggerType.DISABLE:
                    if (OnPlayerTriggerDisable != null)
                        if (OnPlayerTriggerDisable())
                            DestroyOnTriggerActivates();
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (OnPlayerTriggerDestroy != null)
            OnPlayerTriggerDestroy();
    }

    private void OnDisable()
    {
        Trigger(null, TriggerType.DISABLE);
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

    bool ConstraintsAreTrue()
    {
        var res = true;
        int cnt = constraints.Length;

        for (int i = 0; i < cnt; i++)
        {
            if (!constraints[i].IsTrue)
            {
                res = false;
                break;
            }
        }
        return res;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Trigger(null, TriggerType.PUP);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Trigger(null, TriggerType.PDOWN);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Trigger(null, TriggerType.PCLICK);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Trigger(null, TriggerType.PDROP);
    }
}

