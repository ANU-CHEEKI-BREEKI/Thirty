using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConstraintGOInside : ATriggerConstraint
{

    [SerializeField] SquadTriggerInitiator[] toCheck;
    [SerializeField] bool all = true;
    List<SquadTriggerInitiator> inside;

    public override bool IsTrue
    {
        get
        {
            bool res;
            if (all)
            {
                res = true;
                foreach (var item in toCheck)
                {
                    if (!inside.Contains(item))
                    {
                        res = false;
                        break;
                    }
                }
            }
            else
            {
                res = false;
                foreach (var item in toCheck)
                {
                    if (inside.Contains(item))
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }
    }

    private void Awake()
    {
        inside = new List<SquadTriggerInitiator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var u = collision.GetComponent<SquadTriggerInitiator>();
        if(u != null)
            inside.Add(u);
    }

}
