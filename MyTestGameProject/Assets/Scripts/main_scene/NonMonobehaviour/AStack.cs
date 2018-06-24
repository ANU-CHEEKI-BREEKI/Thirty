using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AStack
{
    abstract public Item.MainProperties? MainProperties { get; }

    [Obsolete("Надо удалить это свойство. Но пока что впадлу удалять и все его реализации")]
    abstract public Item Item { get; }
}
