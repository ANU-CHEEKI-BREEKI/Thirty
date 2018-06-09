using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationByEditor : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] string stringResourceName;

    private void Start()
    {
        if (text == null)
            text = GetComponent<TMPro.TextMeshProUGUI>();
        ReloadText();
    }

    public void ReloadText()
    {
        if (text != null)
            text.text = Localization.GetString(stringResourceName);
    }
}
