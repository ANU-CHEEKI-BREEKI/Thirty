using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowAllyUnitsHp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float duration = 1.5f;
    Coroutine lasCocoutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Squad.playerSquadInstance != null)
        {
            if (!Squad.playerSquadInstance.DrawUnitHp)
                Squad.playerSquadInstance.DrawUnitHp = true;
            else if (lasCocoutine != null)
                Squad.playerSquadInstance.StopCoroutine(lasCocoutine);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(Squad.playerSquadInstance != null)
            lasCocoutine = Squad.playerSquadInstance.StartCoroutine(StopDrawing(duration));
    }

    IEnumerator StopDrawing(float duration)
    {
        yield return new WaitForSeconds(duration);

        if(Squad.playerSquadInstance != null)
            Squad.playerSquadInstance.DrawUnitHp = false;
    }
}
