using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropingItemsManager : MonoBehaviour
{
    static DropingItemsManager instance;
    public static DropingItemsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(DropingItemsManager).Name).AddComponent<DropingItemsManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    static Transform droppedItemsContainer;
    static Transform DroppedItemsContainer
    {
        get
        {
            if (droppedItemsContainer == null)
            {
                var dropitcont = GameObject.Find("DroppedItemsContainer");
                if (dropitcont == null)
                    dropitcont = new GameObject("DroppedItemsContainer");
                droppedItemsContainer = dropitcont.transform;
            }
            return droppedItemsContainer;
        }
    }

    static Transform deatUnitsContainer;
    static Transform DeatUnitsContainer
    {
        get
        {
            if (deatUnitsContainer == null)
            {
                var alldeads = GameObject.Find("AllDeadUnits");
                if (alldeads == null)
                    alldeads = new GameObject("AllDeadUnits");
                deatUnitsContainer = alldeads.transform;
            }
            return deatUnitsContainer;
        }
    }

    GameObject droppedItemOriginal = null;
    const string droppedItemOriginalResourcePath = @"Prefabs/Inventory/DroppedItem";

    private void Awake()
    {
        if (droppedItemOriginal == null)
            droppedItemOriginal = Resources.Load<GameObject>(droppedItemOriginalResourcePath);
    }

    public void DropEquipment(EquipmentStack stack, Transform sender, float randmizePosition = 0)
    {
        Vector3 pos = sender.position;
        if (randmizePosition != 0)
        {
            pos.x += Random.Range(-1f, 1f) * randmizePosition;
            pos.y += Random.Range(-1f, 1f) * randmizePosition;
        }

        DroppedItem di = Instantiate(droppedItemOriginal, pos, sender.rotation, DroppedItemsContainer).GetComponent<DroppedItem>();
        di.Stack = new EquipmentStack(stack.EquipmentMainProperties, stack.EquipmentStats, stack.Count);
    }

    public void DropUnitCorp(Unit corp)
    {
        corp.transform.parent = DeatUnitsContainer;
    }
}
