using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StatsBlock : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image icon;
    [Space]
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Value;
    [Space]
    [SerializeField] Color positiveValueColor;
    [SerializeField] Color negativeValueColor;

    string name;
    string value;
    string desc;

    public bool DisplayName { get; set; } = true;
    public bool DisplayIcon { get; set; } = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        TipsPanel.Instance.Show(
            new Description()
            {
                Name = name,
                Icon = icon.sprite, 
                Desc = desc,
            },
            this.transform.position
        );
    }

    public void SetDesc(string desc)
    {
        this.desc = desc;
    }

    public void SetIcon(Sprite icon)
    {
        if (DisplayIcon)
        {
            this.icon.sprite = icon;
            this.icon.gameObject.SetActive(true);
        }
        else
            this.icon.gameObject.SetActive(false);
    }

    public void SetName(string name)
    {
        this.name = name;

        if (name == null||!DisplayName)
            name = string.Empty;

        Name.text = name;
    }

    public void SetValue(string value, bool positive = true)
    {
        if(positive)
            Value.color = positiveValueColor;
        else
            Value.color = negativeValueColor;

        this.value = value;

        Value.text = value;
    }


}
