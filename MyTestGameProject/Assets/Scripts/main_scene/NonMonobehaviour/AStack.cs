using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AStack
{
    abstract public Item.MainProperties? MainProperties { get; }

    /// <summary>
    /// Только в тех случаях, когда используется AStack !!!!!
    /// </summary>
    [Obsolete("Не надо использовать. Вроде как реализвано, но не нужно. и накладно. И сбивает с толку, например, в EquipmentStats всегда null")]
    abstract public Item Item { get; }

    /// <summary>
    /// Только в тех случаях, когда используется AStack !!!!!
    /// </summary>
    abstract public object Stats { get; }
}
