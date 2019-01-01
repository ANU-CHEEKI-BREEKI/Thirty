using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquadInfoShowerButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject activeIndicator;

    private void Start()
    {
        if (activeIndicator != null)
            activeIndicator.SetActive(!SquadInfoPanel.Show);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SquadInfoPanel.Show = !SquadInfoPanel.Show;
        if(activeIndicator != null)
            activeIndicator.SetActive(!SquadInfoPanel.Show);
    }
}
