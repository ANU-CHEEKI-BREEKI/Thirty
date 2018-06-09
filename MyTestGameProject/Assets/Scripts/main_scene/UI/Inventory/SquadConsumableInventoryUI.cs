using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadConsumableInventoryUI : AInventoryUI
{
    public static SquadConsumableInventoryUI Instance { get; private set; }

    [SerializeField] GameObject consumableItemOriginal;

    [Header("Consumable containers")]
    [SerializeField] Transform firstConsCell;
    [SerializeField] Transform secondConsCell;
    [SerializeField] bool canDrag = true;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        RefreshUI();
    }

    override public void RefreshUI()
    {
        if (Squad.playerSquadInstance == null)
            return;

        if (!gameObject.activeSelf)
            return;

        var squadConsumableStack = Squad.playerSquadInstance.Inventory.FirstConsumable;
        SetImage(consumableItemOriginal, firstConsCell, squadConsumableStack, canDrag);

        squadConsumableStack = Squad.playerSquadInstance.Inventory.SecondConsumable;
        SetImage(consumableItemOriginal, secondConsCell, squadConsumableStack, canDrag);
    }

    public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        var go = base.SetImage(origin, cell, stack, canDrag);
        if (go != null)
        {
            var st = stack as ConsumableStack;
            var drag = go.GetComponent<DragConsumable>();

            if (st.Consumable != null)
            {
                var image = go.GetComponent<Image>();
                if (st.ConsumableStats is IStackCountConstraintable
                && st.Count != (st.ConsumableStats as IStackCountConstraintable).MaxCount
                && st.Consumable.MainPropertie.Currency != DSPlayerScore.Currency.GOLD)
                    image.color = new Color(1, 0.8f, 0.8f);

                drag.ConsumableStack = new ConsumableStack();
                drag.ConsumableStack.Consumable = st.Consumable;
                drag.ConsumableStack.ConsumableStats = st.ConsumableStats;
                drag.ConsumableStack.Count = st.Count;

                var text = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = string.Empty;
                if (st.ConsumableStats is IStackCountConstraintable)
                {
                    if (st.Count > 0)
                        text.text = st.Count.ToString(StringFormats.intNumber);
                    else
                    {
                        if (st == Squad.playerSquadInstance.Inventory.FirstConsumable)
                            Squad.playerSquadInstance.Inventory.FirstConsumable.Consumable = null;
                        else if (st == Squad.playerSquadInstance.Inventory.SecondConsumable)
                            Squad.playerSquadInstance.Inventory.SecondConsumable.Consumable = null;
                        SetImage(origin, cell, stack, canDrag);
                    }
                }
            }
        }
        return go;
    }
}
