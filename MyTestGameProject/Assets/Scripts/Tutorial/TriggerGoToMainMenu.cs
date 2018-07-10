using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGoToMainMenu : MonoBehaviour
{
    private void Update()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
