using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour
{
    public static StatsPanelUI Instance { get; set; }
    
    [SerializeField] Toggle StatsToggle;
    [SerializeField] GameObject statsPanel;
    new Animation animation;
    List<string> animNames;

    string formatNumberPercent = "##0%";
    string formatNumber = "##0.#";
    string formatConnat = "({0})";
    string zero = "0";

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        animation = GetComponent<Animation>();
        if (animation != null)
        {
            animNames = new List<string>();
            foreach (AnimationState item in animation)
                animNames.Add(item.name);
        }

        if (StatsToggle != null)
        {
            StatsToggle.onValueChanged.AddListener(OnToggle);
        }

        Squad.playerSquadInstance.OnFormationChanged += OnFormationChanged;
        Squad.playerSquadInstance.Inventory.OnEquipmentChanged += OnEquipmentChanged;
        Squad.playerSquadInstance.OnTerrainModifiersListChanged += PlayerSquadInstance_OnTerrainModifiersListChanged;

        RefreshPanel();
    }

    private void PlayerSquadInstance_OnTerrainModifiersListChanged(SOTerrainStatsModifier[] obj)
    {
        OnEquipmentChanged(null);
    }

    private void OnDestroy()
    {
        if (Squad.playerSquadInstance != null)
        {
            Squad.playerSquadInstance.OnFormationChanged -= OnFormationChanged;
            Squad.playerSquadInstance.Inventory.OnEquipmentChanged -= OnEquipmentChanged;
            Squad.playerSquadInstance.OnTerrainModifiersListChanged -= PlayerSquadInstance_OnTerrainModifiersListChanged;
        }
    }

    void OnToggle(bool value)
    {
        if (value)
            animation.Play(animNames[0]);
        else
            animation.Play(animNames[1]);
    }

    void OnFormationChanged(FormationStats formation)
    {
        OnEquipmentChanged(null);
    }

    void OnEquipmentChanged(EquipmentStack eq)
    {
        if (Squad.playerSquadInstance != null)
        {
            if (Squad.playerSquadInstance.UnitCount > 0)
            {
                UnitStats stats = Squad.playerSquadInstance.UnitStats;

                StatsBlock sb;

                sb = statsPanel.transform.GetChild(1).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.health);
                sb.SetValue(stats.Health.ToString(formatNumber) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.Health));

                sb = statsPanel.transform.GetChild(2).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.armour);
                sb.SetValue(stats.Armour.ToString(formatNumber) + string.Format(formatConnat, zero));

                sb = statsPanel.transform.GetChild(3).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.baseDamage);
                sb.SetValue(stats.Damage.BaseDamage.ToString(formatNumber) + string.Format(formatConnat, zero));

                sb = statsPanel.transform.GetChild(4).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.armourDamage);
                sb.SetValue(stats.Damage.ArmourDamage.ToString(formatNumber) + string.Format(formatConnat, zero));

                sb = statsPanel.transform.GetChild(5).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.attack);
                sb.SetValue(stats.Attack.ToString(formatNumberPercent) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.Attack.ToString(formatNumberPercent)));

                sb = statsPanel.transform.GetChild(6).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.defence);
                sb.SetValue(stats.Defence.ToString(formatNumberPercent) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.Defence.ToString(formatNumberPercent)));

                sb = statsPanel.transform.GetChild(7).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.missileBlock);
                sb.SetValue(stats.MissileBlock.ToString(formatNumberPercent) + string.Format(formatConnat, zero));

                sb = statsPanel.transform.GetChild(8).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.attackDistance);
                sb.SetValue(stats.AttackDistance.ToString(formatNumber) + string.Format(formatConnat, zero));

                sb = statsPanel.transform.GetChild(9).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.speed);
                sb.SetValue(stats.Speed.ToString(formatNumber) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.Speed.ToString(formatNumber)));

                sb = statsPanel.transform.GetChild(10).GetComponent<StatsBlock>();
                sb.SetName(LocalizedStrings.rotationSpeed);
                sb.SetValue(stats.RotationSpeed.ToString(formatNumber) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.RotationSpeed.ToString(formatNumber)));
            }
        }
    }
        
    public void RefreshPanel()
    {
        OnEquipmentChanged(null);
    }
}
