using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour
{
    public static StatsPanelUI Instance { get; set; }
    
    [SerializeField] Toggle StatsToggle;
    [SerializeField] GameObject statsPanel;
    [Space]
    [SerializeField] bool dislayName = true;
    [Space]
    [SerializeField] StatsBlock health;
    [SerializeField] StatsBlock armour;
    [SerializeField] StatsBlock baseDamage;
    [SerializeField] StatsBlock armourDamage;
    [SerializeField] StatsBlock attack;
    [SerializeField] StatsBlock defence;
    [SerializeField] StatsBlock missileBlock;
    [SerializeField] StatsBlock attackDistance;
    [SerializeField] StatsBlock speed;
    [SerializeField] StatsBlock rotationSpeed;

    new Animation animation;
    List<string> animNames;

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

                SetValToStatsBlock
                (
                    health,
                    LocalizedStrings.health,
                    LocalizedStrings.health_description,
                    stats.Health.ToString(formatNumber) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.Health)
                );

                SetValToStatsBlock
                (
                    armour,
                    LocalizedStrings.armour,
                    LocalizedStrings.armour_description,
                    stats.Armour.ToString(formatNumber) + string.Format(formatConnat, zero)
                );

                SetValToStatsBlock
                (
                    baseDamage,
                    LocalizedStrings.baseDamage,
                    LocalizedStrings.baseDamage_description,
                    stats.Damage.BaseDamage.ToString(formatNumber) + string.Format(formatConnat, zero)
                );

                SetValToStatsBlock
                (
                    armourDamage,
                    LocalizedStrings.armourDamage,
                    LocalizedStrings.armourDamage_description,
                    stats.Damage.ArmourDamage.ToString(formatNumber) + string.Format(formatConnat, zero)
                );

                SetValToStatsBlock
                (
                    attack,
                    LocalizedStrings.attack,
                    LocalizedStrings.attack_description,
                    (stats.Attack * 100).ToString(StringFormats.intNumber) + string.Format(formatConnat, (Squad.playerSquadInstance.DefaultUnitStats.Attack * 100).ToString(StringFormats.intNumber)) + "%"
                );

                SetValToStatsBlock
                (
                    defence,
                    LocalizedStrings.defence,
                    LocalizedStrings.defence_description,
                    (stats.Defence * 100).ToString(StringFormats.intNumber) + string.Format(formatConnat, (Squad.playerSquadInstance.DefaultUnitStats.Defence * 100).ToString(StringFormats.intNumber)) + "%"
                );

                SetValToStatsBlock
                (
                    missileBlock,
                    LocalizedStrings.missileBlock,
                    LocalizedStrings.missileBlock_description,
                    (stats.MissileBlock * 100).ToString(StringFormats.intNumber) + string.Format(formatConnat, zero) + "%"
                );

                SetValToStatsBlock
                (
                    attackDistance,
                    LocalizedStrings.attackDistance,
                    LocalizedStrings.attackDistance_description,
                    stats.AttackDistance.ToString(formatNumber) + string.Format(formatConnat, zero)
                );

                SetValToStatsBlock
                (
                    speed,
                    LocalizedStrings.speed,
                    LocalizedStrings.speed_description,
                    stats.Speed.ToString(formatNumber) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.Speed.ToString(formatNumber))
                );

                SetValToStatsBlock
                (
                    rotationSpeed,
                    LocalizedStrings.rotationSpeed,
                    LocalizedStrings.rotationSpeed_description,
                    stats.RotationSpeed.ToString(formatNumber) + string.Format(formatConnat, Squad.playerSquadInstance.DefaultUnitStats.RotationSpeed.ToString(formatNumber))
                );
            }
        }
    }
        
    void SetValToStatsBlock(StatsBlock sb, string name, string desc, string val)
    {
        sb.DisplayName = dislayName;
        sb?.SetName(name);
        sb?.SetDesc(desc);
        sb?.SetValue(val);
    }

    public void RefreshPanel()
    {
        OnEquipmentChanged(null);
    }
}
