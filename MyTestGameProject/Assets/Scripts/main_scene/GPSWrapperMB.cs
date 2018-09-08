using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSWrapperMB : MonoBehaviour
{
    public void ShowAchivementsGUI()
    {
        GPSWrapper.ShowAchivementsGUI((b)=> { if (!b) Toast.Instance.Show("Cant Open. Maybe Player Not Logged In"); });
    }

    public void ShowSavesGUI()
    {
        GPSWrapper.ShowSavedGamesUI(
            (s, g) => { Toast.Instance.Show("ShowSavesGUI"); },
            (b) => { if (!b) Toast.Instance.Show("Cant Open. Maybe Player Not Logged In"); }
        );
    }
}
