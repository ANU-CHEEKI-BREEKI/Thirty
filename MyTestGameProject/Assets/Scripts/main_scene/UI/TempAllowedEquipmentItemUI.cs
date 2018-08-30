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

    public void Present(Sprite ico)
    {
        img.sprite = ico;
    }
}
