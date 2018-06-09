using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOutline : MonoBehaviour
{
	MaterialPropertyBlock mpb;
	new SpriteRenderer renderer;
	
	void Start ()
    {
		mpb = new MaterialPropertyBlock();
		renderer = GetComponent<SpriteRenderer>();
		if (renderer != null)
			renderer.GetPropertyBlock(mpb);
		
        ActivateOutline();
    }

    public void ActivateOutline(bool useGameManager = true)
    {
		if (renderer != null)
		{
			GraphixSettings gs = GameManager.Instance.Settings.graphixSettings;

            if (gs.OutlineType == GraphixSettings.OutlineTypes.BORDER)
            {
                //общй цвет, когда обводка откллючена. для того чтоб батчилось все в один вызов отрисовки
                Color defColor = Color.white;

                if (useGameManager)
                {
                    //общий цвет для батча трупов
                    if (gs.AllyOutline)
                        defColor = gs.AllyOutlineColor;
                    else if (gs.EnemyOutline)
                        defColor = gs.EnemyOutlineColor;
                    else if (gs.NeutralOutline)
                        defColor = gs.NeutralOutlineColor;

                    bool outline = false;

                    Transform parent = transform.parent;
                    if (parent == null)
                        return;

                    if (parent.gameObject.layer == LayerMask.NameToLayer(Squad.UnitFraction.ALLY.ToString()))
                    {
                        defColor = gs.AllyOutlineColor;
                        outline = gs.AllyOutline;
                    }
                    else if (parent.gameObject.layer == LayerMask.NameToLayer(Squad.UnitFraction.ENEMY.ToString()))
                    {
                        defColor = gs.EnemyOutlineColor;
                        outline = gs.EnemyOutline;
                    }
                    else if (parent.gameObject.layer == LayerMask.NameToLayer(Squad.UnitFraction.NEUTRAL.ToString()))
                    {
                        defColor = gs.NeutralOutlineColor;
                        outline = gs.NeutralOutline;
                    }

                    if (outline)
                    {
                        mpb.SetColor("_OutlineColor", defColor);
                        mpb.SetFloat("_Outline", 1);
                    }
                    else
                    {
                        mpb.SetColor("_OutlineColor", defColor);
                        mpb.SetFloat("_Outline", 0);
                    }
                }
                else
                {
                    mpb.SetColor("_OutlineColor", defColor);
                    mpb.SetFloat("_Outline", 0);
                }

                renderer.SetPropertyBlock(mpb);
            }
		}
    }
}
