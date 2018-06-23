using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPresenter
{
    void Present(params object[] param);
}
