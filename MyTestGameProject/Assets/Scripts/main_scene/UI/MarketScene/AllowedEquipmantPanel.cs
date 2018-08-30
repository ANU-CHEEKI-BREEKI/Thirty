using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowedEquipmantPanel : MonoBehaviour
{
    [SerializeField] TempAllowedEquipmentItemUI itemOriginal;
    [SerializeField] bool tempValues;
    [Space]
    [SerializeField] string pathToEquipmant = @"Prefabs/Items/Equipment/";

    private void Awake()
    {
        if (tempValues)
            GameManager.Instance.PlayerProgress.Equipment.OnTempEquipmentAdded += OnTempAllowedEquimpantAdded;

        ClearAll();
    }

    private void OnDestroy()
    {
        if (tempValues)
            GameManager.Instance.PlayerProgress.Equipment.OnTempEquipmentAdded -= OnTempAllowedEquimpantAdded;
    }

    public void OnTempAllowedEquimpantAdded(EquipmentStack stack)
    {
        if (!gameObject.activeInHierarchy)
            return;

        AddEq(stack);
    }

    void AddEq(EquipmentStack stack)
    {
        var item = Instantiate(itemOriginal, transform);
        item.Present(stack.EquipmentMainProperties.Icon);
    }

    void ClearAll()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public void Reset(bool asTempValues = true)
    {
        tempValues = asTempValues;

        ClearAll();

        var playerEq = GameManager.Instance.PlayerProgress.Equipment;

        var eqs = Resources.LoadAll<Equipment>(pathToEquipmant);
        foreach (var e in eqs)
        {
            if(tempValues)
            {
                if (playerEq.IsThisEquipmantInTempValues(e.Stats))
                    AddEq(new EquipmentStack(e));
            }
            else
            {
                if (playerEq.IsThisEquipmantAllowed(e.Stats))
                    AddEq(new EquipmentStack(e));
            }
        }
    }

}
