using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSWrapperMB : MonoBehaviour
{
    public void ShowAchivementsGUI()
    {
        GPSWrapper.ShowAchivementsGUI((b)=> { if (!b) Toast.Instance.Show(LocalizedStrings.cant_open_player_offline); });
    }

    public void ShowSavesGUI()
    {
        GPSWrapper.ShowSavedGamesUI(
            (s, g) => { Toast.Instance.Show(LocalizedStrings.show_saves_gui); },
            (b) => { if (!b) Toast.Instance.Show(LocalizedStrings.cant_open_player_offline); }
        );
    }

    public void LogIn()
    {
        GPSWrapper.LogInPlayer(true, (b) =>
        {
            if(b)
                Toast.Instance.Show(LocalizedStrings.hello);
            else
                Toast.Instance.Show(LocalizedStrings.cant_log_in_2);
        });
    }

    public void LogOut()
    {
        GPSWrapper.LogOutPlayer();
        Toast.Instance.Show(LocalizedStrings.logged_out);
    }

    public void LogInOut()
    {
        if (GPSWrapper.PlayerLoggedIn)
        {
            DialogBox.Instance
                .SetTitle(LocalizedStrings.log_out_title)
                .SetText(LocalizedStrings.log_out_assert)
                .AddCancelButton(LocalizedStrings.no)
                .AddButton(LocalizedStrings.yes, () =>
                {
                    LogOut();
                    DialogBox.Instance.Hide();
                })
                .Show();
        }
        else
        {
            LogIn();
        }            
    }
}
