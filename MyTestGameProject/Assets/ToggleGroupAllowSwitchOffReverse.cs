using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleGroupAllowSwitchOffReverse : MonoBehaviour
{
    private ToggleGroup group;
    
    private void Awake()
    {
        group = GetComponent<ToggleGroup>();
    }

    public void AllowSwitchOffReverce(bool allow)
    {
        group.allowSwitchOff = !allow;
    }
}
