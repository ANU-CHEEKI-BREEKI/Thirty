using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ADropToMe: MonoBehaviour, IDropHandler
{
    [Header("ADropToMe")]    
    [SerializeField] bool canDrop = true;
    protected bool CanDrop { get { return canDrop; }}

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
    }
}
