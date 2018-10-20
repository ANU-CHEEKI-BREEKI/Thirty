using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

[Serializable]
public class ConsumableStack : AExecutableStack, IStackCountConstraintable, IDescriptionable
{
    public event Action<Consumable> OnConsumableChanged;
    public event Action<object> OnStatsChanged;

    public override void ApplyLoadedData(object data)
    {
        var d = data as ConsumableStack;
        if (!string.IsNullOrEmpty(d.mainProperties.PathToPrefab))
            consumable = Resources.Load<Consumable>(d.mainProperties.PathToPrefab);

        count = d.count;
    }

    /// <summary>
    /// при ззагрузке сохранений это поле надо обновить
    /// </summary>
    [SerializeField] Consumable consumable;
    public Consumable Consumable
    {
        get
        {
            return consumable;
        }
        set
        {
            consumable = value;
            if(value != null)
                mainProperties = value.MainPropertie;
            if (OnConsumableChanged != null)
                OnConsumableChanged(value);
        }
    }

    object consumableStats;
    public object ConsumableStats
    {
        get
        {
            if (consumableStats == null && consumable != null)
                consumableStats = consumable.DefaultStats;
            return consumableStats;
        }
        set
        {
            consumableStats = value; if (OnStatsChanged != null) OnStatsChanged(value);
        }
    }

    [SerializeField] int count;
    public int Count { get { return count; } set { count = value; } }

    public int MaxCount
    {
        get
        {
            var c = consumableStats as IStackCountConstraintable;
            if (c != null) return c.MaxCount;
            else return -1;
        }
    }  

    public override Item Item
    {
        get
        {
            return consumable;
        }
    }

    public override object Stats
    {
        get
        {
            return ConsumableStats;
        }
    }

    public ConsumableStack() : this(null, null)
    {
    }

    public ConsumableStack(Consumable consumable, object consumableStats, int count = 1)
    {
        this.Consumable = consumable;
        this.consumableStats = consumableStats;
        this.count = count;
    }

    public ConsumableStack(ConsumableStack stack)
    {
        this.Consumable = stack.consumable;
        this.consumableStats = stack.consumableStats;
        this.count = stack.count;
    }

    public ConsumableStack(ConsumableStack stack, int count) : this(stack)
    {
        this.count = count;
    }

    public Description GetDescription()
    {
        Description consubableDesc = consumable.GetDescription();
        var desc = consumableStats as IDescriptionable;
        if (desc != null)
        {
            Description statsDesc = desc.GetDescription();
            consubableDesc.Constraints = statsDesc.Constraints;
            consubableDesc.Stats = statsDesc.Stats;

            if (consumableStats is IStackCountConstraintable 
            && Count != (consumableStats as IStackCountConstraintable).MaxCount 
            && consumable.MainPropertie.Currency != DSPlayerScore.Currency.GOLD)
                consubableDesc.Cost = null;
            else
                consubableDesc.Cost = new Description.CostInfo()
                {
                    CostPerOne = statsDesc.Cost.Value.CostPerOne,
                    CostAll = statsDesc.Cost.Value.CostPerOne * Count,
                    CostCurrency = consumable.MainPropertie.Currency
                };
        }
        consubableDesc.Icon = consumable.MainPropertie.Icon;
        consubableDesc.UseType = consumable.UseType.GetNameLocalise();
        
        return consubableDesc;
    }
}
