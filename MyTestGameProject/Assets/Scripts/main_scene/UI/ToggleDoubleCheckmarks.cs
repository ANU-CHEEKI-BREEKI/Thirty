using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleDoubleCheckmarks : MonoBehaviour
{
    [SerializeField] Image markToggleOn;
    [SerializeField] Image markToggleOff;

    Toggle toggle;

    void Start ()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);

        OnValueChanged(toggle.isOn);
    }
	
	void OnValueChanged(bool val)
    {
        markToggleOn.gameObject.SetActive(val);
        markToggleOff.gameObject.SetActive(!val);
	}
}
