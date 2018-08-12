using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tools;

public class SquadWeight : MonoBehaviour
{
    [SerializeField] Squad squad;
    public Squad Squad { get { return squad; } set { squad = value; } }
    [Space]
    [SerializeField] Image[] weightIcons;
    [SerializeField] Sprite lihgtWeight;
    [SerializeField] Sprite mediumWeight;
    [SerializeField] Sprite heavyWeight;

    public bool Active { get; set; } = true;

    void Start()
    {
        if (squad != null)
        {
            squad.Inventory.OnEquipmentChanged += Inv_OnEquipmentChanged;

            Present();
        }
    }

    private void OnDestroy()
    {
        if (squad != null)
        {
            squad.Inventory.OnEquipmentChanged -= Inv_OnEquipmentChanged;
        }
    }

    private void Inv_OnEquipmentChanged(EquipmentStack obj)
    {
        Present();
    }

    void Present()
    {
        if (squad != null && gameObject.activeInHierarchy && Active)
        {
            var w = Others.GetWeightByMass(squad.UnitStats.EquipmentMass);
            Sprite ico = null;
            int cnt = 0;
            if (w == UnitStats.EquipmentWeight.VERY_LIGHT)
            {
                ico = lihgtWeight;
                cnt = 1;
            }
            else if (w == UnitStats.EquipmentWeight.VERY_HEAVY)
            {
                ico = heavyWeight;
                cnt = 3;
            }
            else
            {
                ico = mediumWeight;
                if (w == UnitStats.EquipmentWeight.HEAVY)
                    cnt = 3;
                else if (w == UnitStats.EquipmentWeight.MEDIUM)
                    cnt = 2;
                else
                    cnt = 1;
            }
            for (int i = 0; i < weightIcons.Length; i++)
            {
                if (i < cnt)
                {
                    weightIcons[i].sprite = ico;
                    weightIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    weightIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
