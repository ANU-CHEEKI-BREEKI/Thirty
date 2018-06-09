using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOTip : ScriptableObject
{
    public Sprite[] images;
    [Multiline(2)]
    public string tipName;
}
