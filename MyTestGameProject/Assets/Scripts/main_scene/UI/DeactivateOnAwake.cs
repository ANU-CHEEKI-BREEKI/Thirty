using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnAwake : MonoBehaviour
{
    [SerializeField] bool OnStart = false;

    private void Awake()
    {
        if (!OnStart)
        { 
            gameObject.SetActive(false);
            Destroy(this);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
        Destroy(this);
    }
}
