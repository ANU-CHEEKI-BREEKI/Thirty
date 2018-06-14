using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerEvent
{
    event Action OnPlayerTriggerEnter;
    event Action OnPlayerTriggerExit;
    event Action OnPlayerTriggerStay;
}
