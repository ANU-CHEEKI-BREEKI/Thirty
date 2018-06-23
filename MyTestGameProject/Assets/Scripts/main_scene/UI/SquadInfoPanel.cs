using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadInfoPanel : MonoBehaviour
{
    [SerializeField] SquadInfoCellPresenter[] cells;
    [SerializeField] Image hpBar;
    [SerializeField] Squad squad;

    static Action<bool> OnShowChanged;
    static bool show = true;
    static public bool Show
    {
        get { return show; }
        set { if (show != value) { show = value; if (OnShowChanged != null) OnShowChanged(value); } }
    }

    private void Awake()
    {
        OnShowChanged += Present;

        if(squad != null)
            squad.OnFormationChanged += Squad_OnFormationChanged;
    }

    private void Start()
    {
        Present(Show);
    }

    private void OnDestroy()
    {
        OnShowChanged -= Present;
        if (squad != null)
            squad.OnFormationChanged -= Squad_OnFormationChanged;
    }

    private void Squad_OnFormationChanged(FormationStats.Formations obj)
    {
        if (squad != null && cells.Length == 6 && show)
        {
            cells[1].Present(FormationButton.Instance.GetIcon(squad.CurrentFormation));
        }
    }

    void Present(bool value)
    {
        if (squad != null && cells.Length == 6)
        {
            if (value)
            {
                float t = squad.SquadHealth / (squad.UnitStats.Health * squad.FULL_SQUAD_UNIT_COUNT);
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
}
