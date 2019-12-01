using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SortEquipmentByType : MonoBehaviour
{
    [SerializeField] private MarketInventoryUI inventoryIU;
    [SerializeField] private EquipmentStats.TypeOfEquipment type;
    [SerializeField] private bool onlyIfToggleIsOn = true;
    [Space]
    [SerializeField] private bool initOnStart = true;
    [SerializeField] private bool initOnEnable = true;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(SetFilterToInventory);
    }

    private void Start()
    {
        if(initOnStart)
            SetFilterToInventory(toggle.isOn);
    }

    private void OnEnable()
    {
        if (initOnEnable)
            SetFilterToInventory(toggle.isOn);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(SetFilterToInventory);
    }

    public void SetFilterToInventory(bool togle)
    {
        if (togle || !onlyIfToggleIsOn)
            inventoryIU.SetCurrentEquipmentType(type);
    }

}
