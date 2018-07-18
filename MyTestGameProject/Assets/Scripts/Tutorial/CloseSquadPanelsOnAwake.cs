using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseSquadPanelsOnAwake : MonoBehaviour
{
    private void Awake()
    {
        SquadInfoPanel.Show = false;
        Destroy(gameObject);
    }
}
