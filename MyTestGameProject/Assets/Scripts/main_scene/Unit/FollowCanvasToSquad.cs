using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCanvasToSquad : MonoBehaviour
{
    [SerializeField] Squad squad;

    Transform tr;

    private void Awake()
    {
        tr = transform;
    }

    void Update ()
    {
        if(squad != null)
            tr.position = squad.CenterSquad;
	}

    private void OnEnable()
    {
        if (squad != null)
            tr.position = squad.CenterSquad;
    }
}
