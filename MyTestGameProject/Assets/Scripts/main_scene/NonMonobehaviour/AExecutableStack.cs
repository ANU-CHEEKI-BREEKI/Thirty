using UnityEngine;
using System.Collections;

public abstract class AExecutableStack : AStack, ILoadedDataApplyable
{
    abstract public void ApplyLoadedData(object data);
}
