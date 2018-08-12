using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Squad))]
public class AddInfoPanelToSquad : MonoBehaviour
{
    [SerializeField] SquadInfoPanel original;

    private void Awake()
    {
        Squad squad = GetComponent<Squad>();

        var panel = Instantiate(original, squad.transform).GetComponent<SquadInfoPanel>();
        if (panel != null)
        {
            panel.Squad = squad;

            var canvasFollow = panel.GetComponent<FollowCanvasToSquad>();
            if (canvasFollow != null)
                canvasFollow.SquadForFollow = squad;
        }
        Destroy(this);
    }
}
