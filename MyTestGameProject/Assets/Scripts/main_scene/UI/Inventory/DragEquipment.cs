using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragEquipment : Drag
{
    [SerializeField] Image mainImage;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] Image attentionImage;
    [Space]
    [SerializeField] EquipmentStack equipStack;
    public EquipmentStack EquipStack
    {
        get { return equipStack; }
        set
        {
            equipStack = value;
            if (showNewEquipment)
            {
                var pe = GameManager.Instance.PlayerProgress.Equipment;
                if (pe.IsThisEquipmantAllowed(value.EquipmentStats))
                    isNewItem = pe.GetEquipmantAllowed(value.EquipmentStats).IsNew;
            }
        }
    }

    bool isNewItem = false;
    [Space]
    public bool showNewEquipment = false;

    public override AStack Stack
    {
        get
        {
            return EquipStack;
        }
        set
        {
            if (value is EquipmentStack)
            {
                EquipStack = value as EquipmentStack;
            }
            else
                throw new ArgumentException("не тот стак засунуть пытаешься.");
        }
    }

    protected override void OnCantDrag()
    {
        if (EquipStack.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.WEAPON)
            Toast.Instance.Show(LocalizedStrings.toast_cant_drop_weapon);
        else
            base.OnCantDrag();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (CanCallClick)
        {
            TipsPanel.Instance.Show(EquipStack.GetDescription(), thisTransform.position);
            isNewItem = false;

            if (showNewEquipment)
            {
                var pe = GameManager.Instance.PlayerProgress.Equipment;
                if (pe.IsThisEquipmantAllowed(equipStack.EquipmentStats))
                {
                    var eqid = pe.GetEquipmantAllowed(equipStack.EquipmentStats);
                    pe.allowedEquipmentId[pe.allowedEquipmentId.IndexOf(eqid)] = new DSPlayerEquipment.EqId(eqid, isNew: false);
                }
                Present();
            }
        }
    }

    public override void Present()
    {
        mainImage.sprite = EquipStack.EquipmentMainProperties.Icon;
        countText.text = EquipStack.Count.ToString();

        if (showNewEquipment)
        {
            if (isNewItem) attentionImage.gameObject.SetActive(true);
            else attentionImage.gameObject.SetActive(false);
        }
    }
}
