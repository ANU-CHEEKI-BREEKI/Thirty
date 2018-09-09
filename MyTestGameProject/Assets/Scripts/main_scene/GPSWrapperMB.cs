using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSWrapperMB : MonoBehaviour
{
    public void ShowAchivementsGUI()
    {
        GPSWrapper.ShowAchivementsGUI((b)=> { if (!b) Toast.Instance.Show("[non loc] Cant Open. Maybe Player Not Logged In"); });
    }

    public void ShowSavesGUI()
    {
        GPSWrapper.ShowSavedGamesUI(
            (s, g) => { Toast.Instance.Show("[non loc] ShowSavesGUI"); },
            (b) => { if (!b) Toast.Instance.Show("[non loc] Cant Open. Maybe Player Not Logged In"); }
        );
    }

    public void LogIn()
    {
        GPSWrapper.LogInPlayer(true, (b) =>
        {
            if(b)
                Toast.Instance.Show("[non loc] Hi");
            else
                Toast.Instance.Show("[non loc] Cant log in");
        });
    }

    public void LogOut()
    {
        GPSWrapper.LogOutPlayer();
        Toast.Instance.Show("[non loc] Logged out");
    }

    public void LogInOut()
    {
        if (GPSWrapper.PlayerLoggedIn)
            LogOut();
        else
            LogIn();
            
    }
}
