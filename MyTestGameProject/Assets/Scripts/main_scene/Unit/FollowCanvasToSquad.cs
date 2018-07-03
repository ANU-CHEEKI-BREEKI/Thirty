using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCanvasToSquad : MonoBehaviour
{
    [SerializeField] Squad squad;
    public Squad SquadForFollow { get { return squad; } set { squad = value; } }

    Transform tr;

    private void Awake()
    {
        tr = transform;
    }

    void Update ()
    {
        Follow();
    }

    private void OnEnable()
    {
        Follow();
    }

    void Follow()
    {
        if (squad != null)
        {
            Vector3 pos = squad.CenterSquad;
            pos.z = -2;
            tr.position = pos;
        }
    }
}
