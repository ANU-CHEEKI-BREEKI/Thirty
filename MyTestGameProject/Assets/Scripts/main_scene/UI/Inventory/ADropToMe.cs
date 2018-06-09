using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ADropToMe: MonoBehaviour, IDropHandler
{
    protected new Transform transform;

    private void Start()
    {
        transform = base.transform;
    }

    public abstract void OnDrop(PointerEventData eventData);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stack"></param>
    /// <returns>true если можно взять отсюда</returns>
    public abstract bool CanGetFromThisIventory(AStack aStack);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stack"></param>
    /// <returns>true если добавление произошло удачно</returns>
    public abstract bool AddToThisInventory(AStack aStack);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stack"></param>
    /// <returns>true если удаление произошло удачно</returns>
    public abstract bool RemoveFromThisInventory(AStack aStack);

    public void RefreshUI()
    {
        foreach (var inv in GameObject.FindObjectsOfType(typeof(AInventoryUI)) as AInventoryUI[])
            inv.RefreshUI();

        //if (SquadInventoryUI.Instance != null)
        //    SquadInventoryUI.Instance.RefreshUI();

        //if (MarketInventoryUI.Instance != null)
        //    MarketInventoryUI.Instance.RefreshUI();

        //if (EnvirinmantInventoryUI.Instance != null)
        //    EnvirinmantInventoryUI.Instance.RefreshUI();

        //if (SquadSkillsInventoryUI.Instance != null)
        //    SquadSkillsInventoryUI.Instance.RefreshUI();

        //if (SkillMarketInventoryUI.Instance != null)
        //    SkillMarketInventoryUI.Instance.RefreshUI();

        //if (SquadConsumableInventoryUI.Instance != null)
        //    SquadConsumableInventoryUI.Instance.RefreshUI();
    }
}
