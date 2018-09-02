using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempAllowedEquipmentItemUI : MonoBehaviour
{
    Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void Present(Sprite ico, bool enabled = true)
    {
        img.sprite = ico;

        Color c = Color.white;
        if (enabled)
            c.a = 1;
        else
            c.a = 0.2f;

        img.color = c;
    }
}
