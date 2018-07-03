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

    Camera camera;

    Transform thisTransform;

    static Action<bool> OnShowChanged;
    static bool show = true;
    static public bool Show
    {
        get { return show; }
        set { if (show != value) { show = value; if (OnShowChanged != null) OnShowChanged(value); } }
    }

    private void OnExecChanded(Executable obj)
    {
        Present(Show);
    }

    private void Awake()
    {
        OnShowChanged += Present;
        camera = Camera.main;
        thisTransform = transform;
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
        }

        Squad_OnFormationChanged(squad.CurrentFormation);

        Present(Show);
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
        }
    }

    private void OnEnable()
    {
        SetScale();
    }

    private void Update()
    {
        SetScale();
    }

    void SetScale()
    {
        thisTransform.localScale = camera.orthographicSize * 0.05f * (Vector3.one * 0.03f);
    }

    private void Squad_OnFormationChanged(FormationStats.Formations obj)
    {
        if (squad != null && cells.Length == 6 && show)
        {
            cells[1].Present(FormationButton.Instance.GetIcon(squad.CurrentFormation));
        }

        SetIcoPercent();
    }

    void Present(bool value)
    {
        if (squad != null && cells.Length == 6)
        {
            if (value)
            {
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

            gameObject.SetActive(value);
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
