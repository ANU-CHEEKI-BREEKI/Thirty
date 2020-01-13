using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GpsGOActivator : MonoBehaviour
{
    [SerializeField] private string[] testerUserMail = new string[] { "titan.x7@gmail.com", "damki.thirty@gmail.com" };
    [Space]
    [SerializeField] private bool activation = true;
    [SerializeField] private GameObject[] toActivate;

    private void Start()
    {
        bool activate = false;

#if UNITY_EDITOR
        activate = true;
#else
        var mail = (PlayGamesPlatform.Instance.localUser as PlayGamesLocalUser).Email;
        activate = testerUserMail.Any(tmail => tmail.ToLower() == mail.ToLower());       
#endif
        if (activate)
            foreach (var go in toActivate)
                go?.SetActive(activation);
    }
}
