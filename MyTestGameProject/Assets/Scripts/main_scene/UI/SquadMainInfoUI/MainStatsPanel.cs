using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainStatsPanel : MonoBehaviour
{
    [SerializeField] Squad squad;
    public Squad Squad { get { return squad; } set { squad = value; } }
    [Space]
    [SerializeField] StatsIcon attackIco;
    [SerializeField] StatsIcon defenceIco;
    [SerializeField] StatsIcon missileIco;
    [SerializeField] StatsIcon damageIco;
    [SerializeField] StatsIcon armourIco;

    bool active = true;
    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            if (active)
            {
                Present();
            }
        }
    }

    private void Start()
    {
        if (squad == null)
            squad = Squad.playerSquadInstance;

        squad.OnFormationChanged += Squad_OnFormationChanged;
        squad.OnTerrainModifiersListChanged += Squad_OnTerrainModifiersListChanged;
        squad.OnModifiersListChanged += Squad_OnModifiersListChanged;
        squad.Inventory.OnEquipmentChanged += Inv_OnEquipmentChanged;

        Present();

    }

    private void OnDestroy()
    {
        if (squad != null)
        {
            squad.OnFormationChanged -= Squad_OnFormationChanged;
            squad.OnTerrainModifiersListChanged -= Squad_OnTerrainModifiersListChanged;
            squad.OnModifiersListChanged -= Squad_OnModifiersListChanged;
            squad.Inventory.OnEquipmentChanged -= Inv_OnEquipmentChanged;
        }
    }

    private void Squad_OnModifiersListChanged(UnitStatsModifier[] obj)
    {
        Present();
    }

    private void Squad_OnTerrainModifiersListChanged(SOTerrainStatsModifier[] obj)
    {
        Present();
    }

    private void Inv_OnEquipmentChanged(EquipmentStack obj)
    {
        Present();
    }

    private void Squad_OnFormationChanged(FormationStats obj)
    {
        Present();
    }

    void Present()
    {
        if (squad != null && gameObject.activeInHierarchy && Active)
        {
            var stats = squad.UnitStats;

            attackIco.Text = stats.Attack.ToString(StringFormats.intNumberPercent);
            SetIcoCnt(stats.Attack, attackIco);

            defenceIco.Text = stats.Defence.ToString(StringFormats.intNumberPercent);
            SetIcoCnt(stats.Defence, defenceIco);

            missileIco.Text = stats.MissileBlock.ToString(StringFormats.intNumberPercent);
            SetIcoCnt(stats.MissileBlock, missileIco);

            damageIco.Text = (stats.Damage.ArmourDamage + stats.Damage.BaseDamage).ToString(StringFormats.intNumber);
            SetIcoCnt((stats.Damage.ArmourDamage + stats.Damage.BaseDamage) / 200, damageIco);
            
            armourIco.Text = stats.Armour.ToString(StringFormats.intNumber);
            SetIcoCnt(stats.Armour / 200, armourIco);
        }
    }

    void SetIcoCnt(float val, StatsIcon icon)
    {
        if (icon != null)
        {
            int cnt = 0;
            if (val < 0.3f)
                cnt = 1;
            else if (val < 0.6f)
                cnt = 2;
            else
                cnt = 3;

            icon.DiaplayCount = cnt;
        }
    }


}
