using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationByEditor : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] string stringResourceName;

    /// <summary>
    /// get or set translaion string resource name without reloading tmpro.text
    /// </summary>
    public string ResourceName
    {
        get => stringResourceName;
        set => stringResourceName = value;
    }

    private void Start()
    {
        if (text == null)
            text = GetComponent<TMPro.TextMeshProUGUI>();
        ReloadText();
    }

    private void OnEnable()
    {
        ReloadText();
    }

    public void ReloadText()
    {
        if (text != null)
            text.text = Localization.GetString(stringResourceName);
    }
}
