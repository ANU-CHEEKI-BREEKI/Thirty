using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOTip : ScriptableObject
{
    public enum Directions { HORISONTAL, VERTICAL }

    [SerializeField] Sprite firstImage;
    [SerializeField] Sprite secondImage;
    [SerializeField] GameObject animatedImageOriginal;
    [TextArea(1, 10)]
    [SerializeField] string tipText;
    [SerializeField] bool isLocalisedText;
    [Space]
    [SerializeField] Directions direction;

    public Sprite FirstImage { get { return firstImage; } }
    public Sprite SecondImage { get { return secondImage; } }
    public GameObject AnimatedImageOriginal { get { return animatedImageOriginal; } }
    public string TipText { get { return tipText; } }
    public bool IsLocalisedText { get { return isLocalisedText; } }
    public Directions Direction { get { return direction; } }

}
