using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadCerrentProp : MonoBehaviour
{
    [SerializeField] SquadPropertyIndicator iconOrginal;
    [Space]
    [SerializeField] Sprite inFightIco;
    [Space]
    [SerializeField] Image[] weightIcons;
    [Space]
    [SerializeField] Sprite lightWeight;
    [SerializeField] Sprite mediumWeight;
    [SerializeField] Sprite heavyWeight;
    Squad squad;

    Transform thisTransform;

    SquadPropertyIndicator formationInd;
    SquadPropertyIndicator inFightInd;
    List<SquadPropertyIndicator> modifiersInd = new List<SquadPropertyIndicator>();
    List<SquadPropertyIndicator> terrainModifiersInd = new List<SquadPropertyIndicator>();


    void Start ()
    {
        thisTransform = transform;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)thisTransform);

        formationInd = CreateNew();
        inFightInd = CreateNew();

        for (int i = 0; i < 5; i++)
            modifiersInd.Add(CreateNew());
        for (int i = 0; i < 5; i++)
            terrainModifiersInd.Add(CreateNew());
        
        squad = Squad.playerSquadInstance;

        squad.OnInFightFlagChanged += PlayerSquadInstance_OnInFightFlagChanged;
        squad.OnFormationChanged += PlayerSquadInstance_OnFormationChanged;
        squad.OnModifiersListChanged += Squad_OnModifiersListChanged;
        squad.OnTerrainModifiersListChanged += Squad_OnTerrainModifiersListChanged;
        squad.Inventory.OnEquipmentChanged += Inventory_OnEquipmentChanged;


        PlayerSquadInstance_OnFormationChanged(squad.CurrentFormationModifyers);
        PlayerSquadInstance_OnInFightFlagChanged(squad.InFight);
        Squad_OnModifiersListChanged(new UnitStatsModifier[] { });
        Squad_OnTerrainModifiersListChanged(new SOTerrainStatsModifier[] { });
        Inventory_OnEquipmentChanged(null);
    }

    void OnDestroy()
    {
        if (squad != null)
        {
            squad.OnInFightFlagChanged -= PlayerSquadInstance_OnInFightFlagChanged;
            squad.OnFormationChanged -= PlayerSquadInstance_OnFormationChanged;
            squad.OnModifiersListChanged -= Squad_OnModifiersListChanged;
            squad.OnTerrainModifiersListChanged -= Squad_OnTerrainModifiersListChanged;
            squad.Inventory.OnEquipmentChanged -= Inventory_OnEquipmentChanged;
        }
    }

    private void Inventory_OnEquipmentChanged(EquipmentStack obj)
    {
        var w = Extensions.GetWeightByMass(squad.UnitStats.EquipmentMass);
        Sprite ico = null;
        int cnt = 0;
        if (w == UnitStats.EquipmentWeight.VERY_LIGHT)
        {
            ico = lightWeight;
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

        for (int i = 0; i < 3; i++)
        {
            if(i < cnt)
            {
                var img = weightIcons[i].sprite = ico;
                weightIcons[i].gameObject.SetActive(true);
            }
            else
            {
                weightIcons[i].gameObject.SetActive(false);
            }
        }

        var ind = weightIcons[0].transform.parent.GetComponent<SquadPropertyIndicator>();
        var d = new Description();
        d.Name = w.GetLocalizeName();
        d.Desc = "Разная местность по разному влияет на отряды рязной тяжести.\r\n" +
            "Будте внимательны, местность может как дать вам преимущество, так и стать вашим местом захоронения.";
        ind.Present(null, d);

        Squad_OnModifiersListChanged(squad.StatsModifiers);
        Squad_OnTerrainModifiersListChanged(squad.TerrainStatsModifiers);
    }

    SquadPropertyIndicator CreateNew(int index = -1)
    {
        var res = Instantiate(iconOrginal, thisTransform);
        if(index > 0 && index < thisTransform.childCount)
            res.transform.SetSiblingIndex(index);
        return res;
    }

    private void PlayerSquadInstance_OnFormationChanged(FormationStats formation)
    {
        var d = formation.GetDescription();
        d.Icon = FormationButton.Instance.GetIcon(formation.FORMATION);
        formationInd.Present(d.Icon, d);
    }

    private void PlayerSquadInstance_OnInFightFlagChanged(bool newValue)
    {
        if(newValue)
            inFightInd.Present(inFightIco, new Description() {Name = LocalizedStrings.condition_inFight_name, Desc = LocalizedStrings.condition_inFight_description });
        else
            inFightInd.Present(null, null);
    }


    private void Squad_OnModifiersListChanged(UnitStatsModifier[] arr)
    {
        if (arr.Length > modifiersInd.Count)
            while (arr.Length != modifiersInd.Count)
                modifiersInd.Add(CreateNew(3));

        for (int i = 0; i < modifiersInd.Count; i++)
        {
            if (i < arr.Length)
            {
                var desc = arr[i].GetDescription();
                modifiersInd[i].Present(desc.Icon, desc);
            }
            else
                modifiersInd[i].Present(null, null);
        }
    }

    private void Squad_OnTerrainModifiersListChanged(SOTerrainStatsModifier[] arr)
    {
        if (arr.Length > terrainModifiersInd.Count)
            while(arr.Length != terrainModifiersInd.Count)
                terrainModifiersInd.Add(CreateNew());

        for (int i = 0; i < terrainModifiersInd.Count; i++)
        {
            if (i < arr.Length)
            {
                var mod = arr[i].GetModifierByEquipmentMass(squad.UnitStats.EquipmentMass);
                var desc = mod.GetDescription();
                terrainModifiersInd[i].Present(desc.Icon, desc);
            }
            else
                terrainModifiersInd[i].Present(null, null);
        }
    }
}
