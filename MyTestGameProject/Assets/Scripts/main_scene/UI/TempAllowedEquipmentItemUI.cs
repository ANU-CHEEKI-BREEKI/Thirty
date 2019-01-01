using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TempAllowedEquipmentItemUI : MonoBehaviour, IPointerClickHandler
{
    Image img;
    EquipmentStack stack;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void Present(EquipmentStack stack, bool enabled = true)
    {
        this.stack = stack;

        img.sprite = stack.EquipmentMainProperties.Icon;

        Color c = Color.white;
        if (enabled)
            c.a = 1;
        else
            c.a = 0.2f;

        img.color = c;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TipsPanel.Instance.Show(stack.GetDescription(), transform.position);
    }
}
