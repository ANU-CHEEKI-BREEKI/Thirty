using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    [Header("Ranks animation states")]
    [SerializeField] Sprite rnk_idle;
    [SerializeField] Sprite rnk_run;
    [SerializeField] Sprite rnk_fight;
    [Space]
    [Header("Phalanx animation states")]
    [SerializeField] Sprite phx_idle;
    [SerializeField] Sprite phx_run;
    [Space]
    [Header("Shields animation states")]
    [SerializeField] Sprite shl_idle;
    [SerializeField] Sprite shl_run;
    [Space]
    [Header("Death animation states")]
    [SerializeField] Sprite death_1;
    [SerializeField] Sprite death_2;

    public Sprite Rnk_idle { get { return rnk_idle; } }
    public Sprite Rnk_run { get { return rnk_run; } }
    public Sprite Rnk_fight { get { return rnk_fight; } }

    public Sprite Phx_idle { get { return phx_idle; } }
    public Sprite Phx_run { get { return phx_run; } }

    public Sprite Shl_idle { get { return shl_idle; } }
    public Sprite Shl_run { get { return shl_run; } }

    public Sprite Death_1 { get { return death_1; } }
    public Sprite Death_2 { get { return death_2; } }
}
