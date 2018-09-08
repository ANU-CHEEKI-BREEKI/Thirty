using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FillMarketInventory : MonoBehaviour
{
    [SerializeField] string pathToEquipmentResources;
    [Space]
    [SerializeField] MarketInventoryUI inventory;

    void Awake()
    {
        if(inventory == null)
            inventory = GetComponent<MarketInventoryUI>();
    }

    void Start()
    {
        Fill(inventory);
        inventory.RefreshUI();
    }

    public void Fill(MarketInventoryUI inventory)
    {
        var playerEq = GameManager.Instance.SavablePlayerData.PlayerProgress.Equipment;

        var eq = Resources.LoadAll<Equipment>(pathToEquipmentResources);
        List<Equipment> eqList = new List<Equipment>();
        foreach (var e in eq)
        {
            if (playerEq.IsThisEquipmantAllowed(e.Stats))
                eqList.Add(e);
        }

        var sortedList = eqList.OrderBy(e => e.Stats.Type);

        foreach (var item in sortedList)
            inventory.AddToInventory(new EquipmentStack(item, 30));
    }
}
