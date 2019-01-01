using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainStatsPanel : MonoBehaviour
{
    [SerializeField] Squad squad;
    public Squad Squad { get { return squad; } set { squad = value; } }
    [Space]
    [SerializeField] TextMeshProUGUI attackText;
    [SerializeField] GameObject[] attackIco;
    [SerializeField] TextMeshProUGUI defenceText;
    [SerializeField] GameObject[] defenceIco;
    [SerializeField] TextMeshProUGUI missileBlockText;
    [SerializeField] GameObject[] missileIco;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] GameObject[] damageIco;
    [SerializeField] TextMeshProUGUI armourBlockText;
    [SerializeField] GameObject[] armourIco;

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

            attackText.text = stats.Attack.ToString(StringFormats.intNumberPercent);
            SetIcoCnt(stats.Attack, attackIco);

            defenceText.text = stats.Defence.ToString(StringFormats.intNumberPercent);
            SetIcoCnt(stats.Defence, defenceIco);

            missileBlockText.text = stats.MissileBlock.ToString(StringFormats.intNumberPercent);
            SetIcoCnt(stats.MissileBlock, missileIco);

            damageText.text = (stats.Damage.ArmourDamage + stats.Damage.BaseDamage).ToString(StringFormats.intNumber);
            SetIcoCnt((stats.Damage.ArmourDamage + stats.Damage.BaseDamage) / 200, damageIco);

            armourBlockText.text = stats.Armour.ToString(StringFormats.intNumber);
            SetIcoCnt(stats.Armour / 200, armourIco);
        }
    }

    void SetIcoCnt(float val, GameObject[] arr)
    {
        if (arr != null)
        {
            int cnt = arr.Length;
            int cnt2 = 0;
            if (val < 0.3f)
                cnt2 = 1;
            else if (val < 0.6f)
                cnt2 = 2;
            else
                cnt2 = 3;

            for (int i = 0; i < cnt; i++)
            {
                if (i < cnt2)
                    arr[i].SetActive(true);
                else
                    arr[i].SetActive(false);
            }
        }
    }


}
