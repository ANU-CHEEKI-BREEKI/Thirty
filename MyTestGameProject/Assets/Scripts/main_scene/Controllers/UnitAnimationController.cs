using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Unit))]
public class UnitAnimationController : MonoBehaviour
{
    [SerializeField] UnitOutline underlayerOutline;

    public int SortingOrger
    {
        get
        {
            if (group != null)
                return group.sortingOrder;
             return 0;
        }
        set
        {
            if (group != null)
                group.sortingOrder = value;
        }
    }

    new SpriteRenderer[] renderer = new SpriteRenderer[6];
    new UnitAnimation[] animation = new UnitAnimation[6];
    SpriteOutline[] outline = new SpriteOutline[6];

    enum EquipmentIndex { SHIELD, HELMET, WEAPON, BODY, ARMS, LEGS }

    Squad squad;
    Unit unit;

    SortingGroup group;

    FormationStats.Formations formation;
    bool hit;
    bool run;
    bool inFight;
    bool charging;

    bool fallen = false;

    public FormationStats.Formations Formation
    {
        get { return formation; }
        set { formation = value; SetAnimationStates(); }
    }
    public bool Run
    {
        get { return run; }
        set { run = value; SetAnimationStates(); }
    }
    public bool InFight
    {
        get { return inFight; }
        set { inFight = value; SetAnimationStates(); }
    }
    public bool Charging
    {
        get { return charging; }
        set { charging = value; SetAnimationStates(); }
    }

    private void Awake()
    {
        unit = GetComponent<Unit>();
        group = GetComponent<SortingGroup>();

        unit.OnArmsChanged += Unit_OnArmsChanged;
        unit.OnBodyChanged += Unit_OnBodyChanged;
        unit.OnHelmetChanged += Unit_OnHelmetChanged;
        unit.OnLegsChanged += Unit_OnLegsChanged;
        unit.OnShieldChanged += Unit_OnShieldChanged;
        unit.OnWeaponChanged += Unit_OnWeaponChanged;
        unit.OnChargingValueChanged += Unit_OnChargingValueChanged;

        unit.OnUnitFallDown += Unit_OnUnitFallDown;
        unit.OnUnitStandUp += Unit_OnUnitStandUp;

        unit.OnUnitDeath += Death;

        unit.OnRunValueChanged += Unit_OnRunValueChanged;
        unit.AfterInitiate += Unit_AfterInitiate;

        Unit_OnArmsChanged(unit.Arms);
        Unit_OnLegsChanged(unit.Legs);
    }
    
    void Unit_AfterInitiate()
    {
        squad = unit.squad;
        if (squad != null)
        {
            squad.OnInFightFlagChanged += Squad_OnInFightFlagChanged;
            squad.OnFormationChanged += Squad_OnFormationChanged;
        }
    }

    private void Unit_OnUnitStandUp()
    {
        Debug.Log("Unit_OnUnitStandUp");

        fallen = false;
        SetAnimationStates();
    }

    private void Unit_OnUnitFallDown()
    {
        Debug.Log("Unit_OnUnitFallDown");

        fallen = true;

        for (int i = 0; i < 6; i++)
        {
            if (animation[i] != null && renderer[i] != null)
            {
                renderer[i].sprite = animation[i].Fallen;

                renderer[i].sortingLayerName = "Item";
                Outline(i);
            }
        }

        unit.Shadow.SetActive(false);

        if (group != null)
            group.sortingLayerName = "Item";
    }


    void Unit_OnChargingValueChanged(bool value)
    {
        Charging = value;
    }

    void Squad_OnFormationChanged(FormationStats value)
    {
        Formation = value.FORMATION;

        if (group != null)
        {
            if (value.FORMATION == FormationStats.Formations.PHALANX)
                group.enabled = false;
            else
                group.enabled = true;
        }
    }
    void Squad_OnInFightFlagChanged(bool value)
    {
        InFight = value;
    }

    void Unit_OnRunValueChanged(bool value)
    {
        Run = value;
    }
    void Unit_OnWeaponChanged(GameObject obj)
    {
        SetNewEquipment(EquipmentIndex.WEAPON, obj);
    }
    void Unit_OnShieldChanged(GameObject obj)
    {
        SetNewEquipment(EquipmentIndex.SHIELD, obj);
    }
    void Unit_OnLegsChanged(GameObject obj)
    {
        SetNewEquipment(EquipmentIndex.LEGS, obj);
    }
    void Unit_OnHelmetChanged(GameObject obj)
    {
        SetNewEquipment(EquipmentIndex.HELMET, obj);
    }
    void Unit_OnBodyChanged(GameObject obj)
    {
        SetNewEquipment(EquipmentIndex.BODY, obj);
    }
    void Unit_OnArmsChanged(GameObject obj)
    {
        SetNewEquipment(EquipmentIndex.ARMS, obj);
    }

    void SetNewEquipment(EquipmentIndex index, GameObject equipment)
    {
        renderer[(int)index] = equipment.GetComponent<SpriteRenderer>();
        animation[(int)index] = equipment.GetComponent<UnitAnimation>();
        outline[(int)index] = equipment.GetComponent<SpriteOutline>();

        SetAnimationStates();
    }

    void OnDestroy()
    {
        unit.OnArmsChanged -= Unit_OnArmsChanged;
        unit.OnBodyChanged -= Unit_OnBodyChanged;
        unit.OnHelmetChanged -= Unit_OnHelmetChanged;
        unit.OnLegsChanged -= Unit_OnLegsChanged;
        unit.OnShieldChanged -= Unit_OnShieldChanged;
        unit.OnWeaponChanged -= Unit_OnWeaponChanged;
        unit.OnChargingValueChanged -= Unit_OnChargingValueChanged;

        unit.OnUnitFallDown += Unit_OnUnitFallDown;
        unit.OnUnitStandUp += Unit_OnUnitStandUp;

        unit.OnRunValueChanged -= Unit_OnRunValueChanged;
        unit.AfterInitiate -= Unit_AfterInitiate;

        unit.OnUnitDeath -= Death;

        if (squad != null)
        {
            squad.OnInFightFlagChanged -= Squad_OnInFightFlagChanged;
            squad.OnFormationChanged -= Squad_OnFormationChanged;
        }
    }

    void Death(Unit unit)
    {
        var t = UnityEngine.Random.Range(0, 2);
        
        for (int i = 0; i < 6; i++)
        {
            if (animation[i] != null && renderer[i] != null)
            {
                if (!fallen)
                {
                    if (t == 0)
                        renderer[i].sprite = animation[i].Death_1;
                    else
                        renderer[i].sprite = animation[i].Death_2;
                }
                else
                {
                    renderer[i].sprite = animation[i].Death_1;
                }
                
                renderer[i].sortingLayerName = "Item";
                Outline(i, false);

                Destroy(outline[i]);
                if (underlayerOutline != null)
                    Destroy(underlayerOutline.gameObject);
                Destroy(animation[i]);                
                animation[i] = null;//костыль для решения плавающего бага, когда после смерти юнита
                //ему ставились спрайты сражения
                Destroy(renderer[i].transform.GetComponent<Item>());
            }
        }

        if (group != null)
            group.sortingLayerName = "Item";

        Destroy(unit.Shadow);

        Destroy(this);
    }

    void SetAnimationStates()
    {
        if (fallen)
            return;

        switch (formation)
        {
            case global::FormationStats.Formations.RANKS:
                {
                    if (charging)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (animation[i] != null && renderer[i].sprite != animation[i].Rnk_fight)
                            {
                                renderer[i].sprite = animation[i].Rnk_fight;
                                renderer[i].sortingLayerName = "Unit";
                                Outline(i);
                            }
                        }
                    }
                    else
                    {
                        if (run)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                if (animation[i] != null && renderer[i].sprite != animation[i].Rnk_run)
                                {
                                    renderer[i].sprite = animation[i].Rnk_run;
                                    renderer[i].sortingLayerName = "Unit";
                                    Outline(i);
                                }
                            }
                        }
                        else
                        {
                            if (inFight)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    if (animation[i] != null && renderer[i].sprite != animation[i].Rnk_fight)
                                    {
                                        renderer[i].sprite = animation[i].Rnk_fight;
                                        renderer[i].sortingLayerName = "Unit";
                                        Outline(i);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    if (animation[i] != null && renderer[i].sprite != animation[i].Rnk_idle)
                                    {
                                        renderer[i].sprite = animation[i].Rnk_idle;
                                        renderer[i].sortingLayerName = "Unit";
                                        Outline(i);
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            case global::FormationStats.Formations.PHALANX:
                {
                    if (run)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (animation[i] != null && renderer[i].sprite != animation[i].Phx_run)
                            {
                                renderer[i].sprite = animation[i].Phx_run;
                                renderer[i].sortingLayerName = "Unit";
                                Outline(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (animation[i] != null && renderer[i].sprite != animation[i].Phx_idle)
                            {
                                renderer[i].sprite = animation[i].Phx_idle;
                                renderer[i].sortingLayerName = "Unit";
                                Outline(i);
                            }
                        }
                    }
                    break;
                }
            case global::FormationStats.Formations.RISEDSHIELDS:
                {
                    if (run)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (animation[i] != null && renderer[i].sprite != animation[i].Shl_run)
                            {
                                renderer[i].sprite = animation[i].Shl_run;
                                renderer[i].sortingLayerName = "Unit";
                                Outline(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (animation[i] != null && renderer[i].sprite != animation[i].Shl_idle)
                            {
                                renderer[i].sprite = animation[i].Shl_idle;
                                renderer[i].sortingLayerName = "Unit";
                                Outline(i);
                            }
                        }
                    }
                    break;
                }
        }

        unit.Shadow.SetActive(true);
    }

    void Outline(int itemIndex, bool anable = true)
    {
        if (outline[itemIndex] != null)
            outline[itemIndex].ActivateOutline(anable);

        if(underlayerOutline != null)
            underlayerOutline.ActivateOutline();
    }

}
