using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] toControl;

    public void SetActive(bool value)
    {
        foreach (var item in toControl)
            if(item != null)
                item.SetActive(value);
    }

    public void SetActiveReverce(bool value)
    {
        SetActive(!value);
    }
}
