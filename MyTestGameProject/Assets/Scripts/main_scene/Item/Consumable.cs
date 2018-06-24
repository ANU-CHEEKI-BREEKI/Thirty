using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : Executable
{
    public event Action<int, Squad> CallbackUsedCount;

    protected void CallBack(int count, Squad owner)
    {
        if(CallbackUsedCount != null)
            CallbackUsedCount(count, owner);
    }
}
