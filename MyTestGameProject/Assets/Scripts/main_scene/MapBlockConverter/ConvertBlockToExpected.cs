using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConvertBlockToExpected : MonoBehaviour
{
   
    void Awake()
    {
        Execute();
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        bool res = false;
        try
        {
            var c1 = gameObject.AddComponent<ConvertRuleTileToSimpleTile>();
            if(c1 != null)
                c1.Execute();
            res = true;
        }
        catch
        {
            res = false;
        }

        if (res)
        {
            var c2 = gameObject.AddComponent<DeleteAllSpriteMaskAndGroups>();
            if (c2 != null)
                c2.Execute();

            var c3 = gameObject.AddComponent<SetAllTilerenderersToNonmask>();
            if (c3 != null)
                c3.Execute();

            var c4 = gameObject.AddComponent<DeleteTilesOutOfBounds>();
            if (c4 != null)
                c4.Execute();
        }

        DestroyImmediate(this, true);
    }
}
