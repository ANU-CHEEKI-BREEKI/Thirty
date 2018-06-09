using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnAwake : MonoBehaviour
{
    [SerializeField] bool ActiveOnAwake = true;

    private void Awake()
    {
        gameObject.SetActive(ActiveOnAwake);
        Destroy(this);
    }
}
