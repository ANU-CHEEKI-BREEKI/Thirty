using UnityEngine;
using System.Collections;
using System;

public abstract class AExecutableBehaviour
{
    protected AiSquadController controller;

    public static event Action<AExecutableBehaviour> DisposeMePlease;

    public AExecutableBehaviour(AiSquadController controller)
    {
        this.controller = controller;
    }

    /// <summary>
    /// надо вызывать в Update или еще какой нибуть функции для апдейта (типа сопрограммы или ещё что)
    /// </summary>
    public abstract void Behave();

    protected void DisposeMe(AExecutableBehaviour me)
    {
        if (DisposeMePlease != null)
            DisposeMePlease(me);
    }
}
