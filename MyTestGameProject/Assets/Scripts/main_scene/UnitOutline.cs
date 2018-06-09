using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitOutline : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start ()
    {
        ActivateOutline();
    }
	
    public void ActivateOutline()
    {
        var gs = GameManager.Instance.Settings.graphixSettings;
        var ot = gs.OutlineType;

        var enable = false;
        Color color = Color.white;

        if (gs.OutlineType == GraphixSettings.OutlineTypes.UNDERLAYER)
        {
            var layer = LayerMask.LayerToName(transform.parent.gameObject.layer);
            if (Squad.UnitFraction.ALLY.ToString() == layer)
            {
                enable = gs.AllyOutline;
                color = gs.AllyOutlineColor;
            }
            else if (Squad.UnitFraction.ENEMY.ToString() == layer)
            {
                enable = gs.EnemyOutline;
                color = gs.EnemyOutlineColor;
            }
            else if (Squad.UnitFraction.NEUTRAL.ToString() == layer)
            {
                enable = gs.NeutralOutline;
                color = gs.NeutralOutlineColor;
            }
        }

        if (!enable)
            color.a = 0;

        spriteRenderer.color = color;
    }
}
