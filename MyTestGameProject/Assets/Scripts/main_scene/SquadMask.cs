using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadMask : MonoBehaviour
{
    static public SquadMask Instance { get; private set; }
    Transform thisTransform;

    public bool Active
    {
        get
        {
            return gameObject.activeInHierarchy;
        }

        set
        {
            gameObject.SetActive(value);
        }
    }

    public float Size
    {
        get
        {
            return transform.localScale.x;
        }
        set
        {
            transform.localScale = Vector3.one * value;
        }
    }

    public Vector2 Position
    {
        get
        {
            return transform.position;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Active = false;
            thisTransform = transform;
        }
        else
            Destroy(gameObject);
    }

}
