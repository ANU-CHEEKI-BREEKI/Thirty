using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Description;

public class SquadPropertyIndicator : MonoBehaviour, IDescriptionable, IPointerClickHandler
{
    [SerializeField] Type type;
    [SerializeField] FormationStats.Formations formation;

    FormationStats f;

    [SerializeField] string stringResourceName;
    [SerializeField] string stringResourceDescription;

    private void Start()
    {
        switch (formation)
        {
            case FormationStats.Formations.RANKS:
                f = new FormationStats.Ranks();
                break;
            case FormationStats.Formations.PHALANX:
                f = new FormationStats.Phalanx();
                break;
            case FormationStats.Formations.RISEDSHIELDS:
                f = new FormationStats.RisedShields();
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TipsPanel.Instance.Show(GetDescription(), transform.position);
    }

    public Description GetDescription()
    {
        DescriptionItem[] constraints = null;
        DescriptionItem[] stats = null;

        switch (type)
        {           
            case Type.FORMATION:
                               
                stats = f.GetModifiers();

                break;

            case Type.TERRAIN_MODIFIER:

                

                break;
        }

        return new Description()
        {
            Name = Localization.GetString(stringResourceName),
            Desc = Localization.GetString(stringResourceDescription),
            Constraints = constraints,
            Stats = stats,
            Icon = GetComponent<Image>().sprite
        };
    }

    public enum Type { CONDITION, FORMATION, TERRAIN_MODIFIER}
}
