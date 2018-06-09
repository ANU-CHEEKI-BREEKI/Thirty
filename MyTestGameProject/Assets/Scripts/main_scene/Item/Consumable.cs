using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : Skill
{
    public event Action<int> CallbackUsedCount;

    protected void CallBack(int count)
    {
        if(CallbackUsedCount != null)
            CallbackUsedCount(count);
    }
}
