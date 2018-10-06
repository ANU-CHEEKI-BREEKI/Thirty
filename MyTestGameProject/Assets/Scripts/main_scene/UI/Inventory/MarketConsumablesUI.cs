using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketConsumablesUI : AInventoryUI
{
    [SerializeField] GameObject inventoryItemOriginal;
    [Space]
    [SerializeField] List<Transform> items;

    [Header("Автозаполнение магазина")]
    [SerializeField] bool autoFillMarket = true;
    [SerializeField] Consumable[] originalConsumables;
    [Space]
    [SerializeField] AnimationCurve dependancyOfGameLevel;
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 20;


    List<ConsumableStack> inventory;

    public bool Fill { get { return inventory.Count == items.Count; } }

    public static MarketConsumablesUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        inventory = new List<ConsumableStack>();
    }

    private void Start()
    {
        AutoFill();
        RefreshUI();
    }

    public void AddToInventory(ConsumableStack stack)
    {
        if (inventory.Count < items.Count)
            inventory.Add(stack);
        else
            throw new System.Exception("больше нет места в инвентаре");
    }

    public void RemoveFtomInventory(ConsumableStack stack)
    {
        if (!inventory.Remove(
            inventory.Find(
                (st) => { return st.Consumable.Id == stack.Consumable.Id && st.Count == stack.Count; }
            )
        ))
            throw new System.Exception("Алё. Таких предметов в магазине нет!");
    }

    override public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
            return;

        int cnt = inventory.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (items[i].childCount > 0)
                Destroy(items[i].GetChild(0).gameObject);
            SetImage(inventoryItemOriginal, items[i], inventory[i], true);
        }


        //это хз зачем тут. влом вникать. это я копировал у другогго класса.....да уж...
        int cnt2 = items.Count;
        if (cnt2 > cnt)
            for (int i = cnt; i < cnt2; i++)
                if (items[i].childCount > 0)
                    Destroy(items[i].GetChild(0).gameObject);
    }

    public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        var go = base.SetImage(origin, cell, stack, canDrag);
        if (go != null)
        {
            var st = stack as ConsumableStack;
            var drag = go.GetComponent<DragConsumable>();
            var txt = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var img = go.GetComponent<Image>();

            drag.ConsumableStack = new ConsumableStack(st.Consumable, st.ConsumableStats, st.Count);
            txt.text = st.Count.ToString(StringFormats.intNumber);
            if (st.ConsumableStats is IStackCountConstraintable
            && st.Count != (st.ConsumableStats as IStackCountConstraintable).MaxCount
            && st.Consumable.MainPropertie.Currency != DSPlayerScore.Currency.GOLD)
                img.color = new Color(1, 0.8f, 0.8f);
        }
        return go;
    }

    void AutoFill()
    {
        int cnt = originalConsumables.Length;

        int cnt2 = items.Count - 2;
        for (int i = 0; i < cnt2; i++)
        {
            Consumable s = originalConsumables[Random.Range(0, cnt)];
            ConsumableStack cs = new ConsumableStack(s, s.DefaultStats);
            if (cs.ConsumableStats is IStackCountConstraintable)
                cs.Count = (cs.ConsumableStats as IStackCountConstraintable).MaxCount;
            AddToInventory(cs);
        }
    }
}
