using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCanvasToSquad : MonoBehaviour
{
    [SerializeField] Squad squad;

    Transform tr;

    private void Start()
    {
        tr = transform;
    }

    void Update ()
    {
        if(squad != null)
            tr.position = squad.CenterSquad;
	}
}
