using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GOTriggerEvent : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDropHandler
{
    public enum TriggerType { STAY, ENTER, EXIT, DISABLE, PDOWN, PUP, PCLICK, PDROP, DESTROY, ENABLE }

    [SerializeField] bool deleteOnTriggerActivates = true;
    [SerializeField] bool deleteOnlyScript = false;

    [SerializeField] ATriggerConstraint[] constraints;

    SquadTriggerInitiator initiator = null;

    public event Func<bool> OnPlayerTriggerEnter;
    public event Func<bool> OnPlayerTriggerExit;
    public event Func<bool> OnPlayerTriggerStay;
    public event Func<bool> OnPlayerTriggerDestroy;
    public event Func<bool> OnPlayerTriggerDisable;
    public event Func<bool> OnPlayerTriggerPoinderDown;
    public event Func<bool> OnPlayerTriggerPoinderUp;
    public event Func<bool> OnPlayerTriggerPointerClick;
    public event Func<bool> OnPlayerTriggerPointerDrop;
    public event Func<bool> OnPlayerTriggerEnable;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if(col != null)
            col.isTrigger = true;

        if(constraints.Length == 0)
            constraints = GetComponents<ATriggerConstraint>();
    }

    void DestroyOnTriggerActivates(TriggerType type)
    {
        if ((initiator != null && initiator.Squad != null))
            initiator.TriggerCallBack(TriggerType.DESTROY);

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
        if (collision != null)
            initiator = collision.GetComponent<SquadTriggerInitiator>();

        if ((collision == null || (initiator != null && initiator.Squad != null && initiator.Squad == Squad.playerSquadInstance)) && ConstraintsAreTrue())
        {
            switch (type)
            {
                case TriggerType.STAY:
                    if (OnPlayerTriggerStay != null)
                        if(OnPlayerTriggerStay())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.ENTER:
                    if (OnPlayerTriggerEnter != null)
                        if(OnPlayerTriggerEnter())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.EXIT:
                    if (OnPlayerTriggerExit != null)
                        if(OnPlayerTriggerExit())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.PDOWN:
                    if (OnPlayerTriggerPoinderDown != null)
                        if(OnPlayerTriggerPoinderDown())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.PUP:
                    if (OnPlayerTriggerPoinderUp != null)
                        if(OnPlayerTriggerPoinderUp())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.PCLICK:
                    if (OnPlayerTriggerPointerClick != null)
                        if(OnPlayerTriggerPointerClick())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.PDROP:
                    if (OnPlayerTriggerPointerDrop != null)
                        if(OnPlayerTriggerPointerDrop())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.DISABLE:
                    if (OnPlayerTriggerDisable != null)
                        if (OnPlayerTriggerDisable())
                            DestroyOnTriggerActivates(type);
                    break;
                case TriggerType.ENABLE:
                    if (OnPlayerTriggerEnable != null)
                        if (OnPlayerTriggerEnable())
                            DestroyOnTriggerActivates(type);
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (OnPlayerTriggerDestroy != null)
            OnPlayerTriggerDestroy();
    }

    private void OnEnable()
    {
        Trigger(null, TriggerType.ENABLE);
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
            if (constraints[i] != null && !constraints[i].IsTrue)
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

