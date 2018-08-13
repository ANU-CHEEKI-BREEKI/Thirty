using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStatesPanel : MonoBehaviour
{
    [SerializeField] Squad squad;
    public Squad Squad { get { return squad; } set { squad = value; } }
    [Space]
    [SerializeField] SquadInfoCellPresenter[] cells;

    public bool Active { get; set; } = true;

    private void Start()
    {
        if (squad == null)
            squad = Squad.playerSquadInstance;

        squad.OnFormationChanged += Squad_OnFormationChanged;

        var inv = squad.Inventory;
        inv.FirstSkill.OnSkillChanged += OnExecChanded;
        inv.SecondSkill.OnSkillChanged += OnExecChanded;
        inv.FirstConsumable.OnConsumableChanged += OnExecChanded;
        inv.SecondConsumable.OnConsumableChanged += OnExecChanded;
        inv.OnEquipmentChanged += Inv_OnEquipmentChanged;

        Present();
    }


    private void OnDestroy()
    {
        if (squad != null)
        {
            squad.OnFormationChanged -= Squad_OnFormationChanged;

            var inv = squad.Inventory;
            inv.FirstSkill.OnSkillChanged -= OnExecChanded;
            inv.SecondSkill.OnSkillChanged -= OnExecChanded;
            inv.FirstConsumable.OnConsumableChanged -= OnExecChanded;
            inv.SecondConsumable.OnConsumableChanged -= OnExecChanded;
            inv.OnEquipmentChanged -= Inv_OnEquipmentChanged;
        }
    }

    private void OnExecChanded(Executable obj)
    {
        Present();
    }

    private void Squad_OnFormationChanged(FormationStats obj)
    {
        Present();
    }

    private void Inv_OnEquipmentChanged(EquipmentStack obj)
    {
        Present();
    }

    void Present()
    {
        if (squad != null && cells.Length == 6 && gameObject.activeInHierarchy && Active)
        {
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
    }
}
