using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [Tooltip("Те которые будут всегда в любом случае")]
    [SerializeField] Consumable[] donateOriginalConsumables;
    [Tooltip("Те которые будут появлятся рандомно")]
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
        if(autoFillMarket)
            AutoFill();
        RefreshUI();
    }

    public void AddToInventory(ConsumableStack stack)
    {
        if (inventory.Count >= items.Count)
            throw new System.Exception("больше нет места в инвентаре");

        inventory.Add(stack);
        RefreshUI();
    }

    public void RemoveFtomInventory(ConsumableStack stack)
    {
        if (!inventory.Remove(
            inventory.Find(
                (st) => { return st.Consumable.Id == stack.Consumable.Id && st.Count == stack.Count; }
            )
        ))
        {
            throw new System.Exception("Алё. Таких предметов в магазине нет!");
        }
        else
        {
            RefreshUI();
        }
    }

    override public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
            return;

        try
        {
            inventory.Sort((i1, i2) =>
            {
                return i1.Consumable.Id - i2.Consumable.Id;
            });
        }
        catch { }
        
        int cnt = inventory.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (items[i].childCount > 0)
                Destroy(items[i].GetChild(0).gameObject);
            SetImage(inventoryItemOriginal, items[i], inventory[i], true);
        }
        
        //очищаем "лишние(пустые)" ячейки
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
        int donateCnt = donateOriginalConsumables.Length;
        int randomCnt = originalConsumables.Length;
        int maxCnt = items.Count - 2;

        //добавляем с обязательного массива всё что влезет
        int cnt1 = Mathf.Min(maxCnt, donateCnt);
        for (int i = 0; i < cnt1; i++)
        {
            var cons = donateOriginalConsumables[i];
            ConsumableStack cs = new ConsumableStack(cons, cons.DefaultStats);
            if (cs.ConsumableStats is IStackCountConstraintable)
                cs.Count = (cs.ConsumableStats as IStackCountConstraintable).MaxCount;
            AddToInventory(cs);
        }

        //если осталось место, добавляем рандомно всё что в необязательном массиве
        int cnt2 = Mathf.Clamp(maxCnt - cnt1, 0, maxCnt);
        for (int i = 0; i < cnt2; i++)
        {
            Consumable cons = originalConsumables[Random.Range(0, randomCnt)];
            ConsumableStack cs = new ConsumableStack(cons, cons.DefaultStats);
            if (cs.ConsumableStats is IStackCountConstraintable)
                cs.Count = (cs.ConsumableStats as IStackCountConstraintable).MaxCount;
            AddToInventory(cs);
        }
    }
}
