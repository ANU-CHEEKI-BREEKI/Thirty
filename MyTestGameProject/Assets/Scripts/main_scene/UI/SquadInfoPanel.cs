using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadInfoPanel : MonoBehaviour
{
    [SerializeField] SquadInfoCellPresenter[] cells;
    [SerializeField] Image hpBar;
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
    [Space]
    [SerializeField] Image[] weightIcons;
    [SerializeField] Sprite lihgtWeight;
    [SerializeField] Sprite mediumWeight;
    [SerializeField] Sprite heavyWeight;

    Transform thisTransform;

    static Action OnShowChanged;
    static bool show = true;
    static public bool Show
    {
        get { return show; }
        set
        {
            if (show != value)
            {
                show = value;
                if (OnShowChanged != null)
                    OnShowChanged();
            }
        }
    }

    static Camera camera;
    bool inCamera = true;
    bool InCamera
    {
        set
        {
            if (inCamera != value)
            {
                inCamera = value;
                Present();
            }
        }
    }
    float orthographicSize;

    bool inMask = true;
    bool InMask
    {
        set
        {
            if (inMask != value)
            {
                inMask = value;
                Present();
            }
        }
    }

    CanvasGroup cg;

    private void OnExecChanded(Executable obj)
    {
        Present();
    }

    private void Awake()
    {
        OnShowChanged += Present;
        if(camera == null)
            camera = Camera.main;

        orthographicSize = camera.orthographicSize;

        thisTransform = transform;

        cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (squad != null)
        {
            squad.OnFormationChanged += Squad_OnFormationChanged;
            var inv = squad.Inventory;
            inv.FirstSkill.OnSkillChanged += OnExecChanded;
            inv.SecondSkill.OnSkillChanged += OnExecChanded;
            inv.FirstConsumable.OnConsumableChanged += OnExecChanded;
            inv.SecondConsumable.OnConsumableChanged += OnExecChanded;
            inv.OnEquipmentChanged += Inv_OnEquipmentChanged;
            squad.OnTerrainModifiersListChanged += Squad_OnTerrainModifiersListChanged;            
        }

        Squad_OnFormationChanged(squad.CurrentFormationModifyers);

        Present();
    }

    private void OnDestroy()
    {
        OnShowChanged -= Present;
        if (squad != null)
        {
            squad.OnFormationChanged -= Squad_OnFormationChanged;
            var inv = squad.Inventory;
            inv.FirstSkill.OnSkillChanged -= OnExecChanded;
            inv.SecondSkill.OnSkillChanged -= OnExecChanded;
            inv.FirstConsumable.OnConsumableChanged -= OnExecChanded;
            inv.SecondConsumable.OnConsumableChanged -= OnExecChanded;
            inv.OnEquipmentChanged -= Inv_OnEquipmentChanged;
            squad.OnTerrainModifiersListChanged -= Squad_OnTerrainModifiersListChanged;
        }
    }


    //private void OnEnable()
    //{
    //    SetScaleAndVisible();
    //}

    private void Update()
    {
        SetScaleAndVisible();
    }

    void SetScaleAndVisible()
    {
        if (orthographicSize != camera.orthographicSize)
        {
            orthographicSize = camera.orthographicSize;
            thisTransform.localScale = orthographicSize * 0.0015f * Vector3.one; //orthographicSize * 0.05f * (Vector3.one * 0.03f); 
        }
                
        var v = camera.WorldToViewportPoint(thisTransform.position);
        if (v.x > 0 && v.x < 1 && v.y > 0 && v.y < 1)
            InCamera = true;
        else
            InCamera = false;

        var mask = SquadMask.Instance;
        if (inCamera)
        {
            if (squad.Hiding && mask != null)
            {
                var size = mask.Size / 2;
                if (Vector2.SqrMagnitude((Vector2)thisTransform.position - mask.Position) <= size * size)
                    InMask = true;
                else
                    InMask = false;
            }
            else
                InMask = true;
        }
    }

    private void Squad_OnTerrainModifiersListChanged(SOTerrainStatsModifier[] obj)
    {
        SetIcoPercent();
    }

    private void Inv_OnEquipmentChanged(EquipmentStack obj)
    {
        SetIcoPercent();
    }

    private void Squad_OnFormationChanged(FormationStats obj)
    {
        if (squad != null && cells.Length == 6 && show)
        {
            cells[1].Present(FormationButton.Instance.GetIcon(squad.CurrentFormation));
        }

        SetIcoPercent();
    }

    void Present()
    {
        if (squad != null && cells.Length == 6)
        {
            if (show && inCamera && inMask)
            {
                var w = Extensions.GetWeightByMass(squad.UnitStats.EquipmentMass);
                Sprite ico = null;
                int cnt= 0;
                if (w == UnitStats.EquipmentWeight.VERY_LIGHT)
                {
                    ico = lihgtWeight;
                    cnt= 1;
                }
                else if (w == UnitStats.EquipmentWeight.VERY_HEAVY)
                {
                    ico = heavyWeight;
                    cnt= 3;
                }
                else
                {
                    ico = mediumWeight;
                    if (w == UnitStats.EquipmentWeight.HEAVY)
                        cnt= 3;
                    else if (w == UnitStats.EquipmentWeight.MEDIUM)
                        cnt= 2;
                    else
                        cnt= 1;
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


                float t = squad.SquadHealth / (squad.DefaultUnitStats.Health * squad.FULL_SQUAD_UNIT_COUNT);
                hpBar.fillAmount = t;
                Color color;
                if (t >= 0.5f)
                    color = Color.Lerp(Color.yellow, Color.green, t * 2 - 1 - 0.3f);
                else
                    color = Color.Lerp(Color.red, Color.yellow, t * 2 - 0.3f);
                hpBar.color = color;

                cells[0].Present(squad.Inventory.Weapon.EquipmentMainProperties.Icon);
                cells[1].Present(FormationButton.Instance.GetIcon(squad.CurrentFormation));
                int q = 2;
                var skill = squad.Inventory.FirstSkill;
                if (skill.Skill != null)
                {
                    cells[q].Present(skill.MainProperties.Value.Icon);
                    q++;
                }
                skill = squad.Inventory.SecondSkill;
                if (skill.Skill != null)
                {
                    cells[q].Present(skill.MainProperties.Value.Icon);
                    q++;
                }
                var cons = squad.Inventory.FirstConsumable;
                if (cons.Consumable != null)
                {
                    cells[q].Present(cons.MainProperties.Value.Icon);
                    q++;
                }
                cons = squad.Inventory.SecondConsumable;
                if (cons.Consumable != null)
                {
                    cells[q].Present(cons.MainProperties.Value.Icon);
                    q++;
                }
                for (int i = q; i < 6; i++)
                {
                    cells[i].Present();
                }
            }

            //gameObject.SetActive(show && inCamera);
            cg.alpha = show && inCamera && inMask ? 1 : 0;
        }
    }

    void SetIcoPercent()
    {
        if (squad != null)
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
