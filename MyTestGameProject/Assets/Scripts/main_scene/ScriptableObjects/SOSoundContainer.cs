using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOSoundContainer : ScriptableObject
{
    [SerializeField] MusicAudioClips music;
    public MusicAudioClips Music { get { return music; } }

    [SerializeField] FXAudioClips fx;
    public FXAudioClips  FX { get { return fx; } }

    [SerializeField] UIAudioClips ui;
    public UIAudioClips UI { get { return ui; } }   

    [Serializable]
    public struct MusicAudioClips
    {
        [SerializeField] AudioClip musicMainMenu;
        public AudioClip MusicMainMenu
        {
            get
            {
                return musicMainMenu;
            }
        }

        [SerializeField] AudioClip musicLevel;
        public AudioClip MusicLevel
        {
            get
            {
                return musicLevel;
            }
        }

        [SerializeField] AudioClip musicMarket;
        public AudioClip MusicMarket
        {
            get
            {
                return musicMarket;
            }
        }

        [SerializeField] AudioClip musicGameOver;
        public AudioClip MusicGameOver
        {
            get
            {
                return musicGameOver;
            }
        }
    }

    [Serializable]
    public struct FXAudioClips
    {
        [SerializeField] AudioClip bonfireMainMenu;
        public AudioClip BonfireMainMenu
        {
            get
            {
                return bonfireMainMenu;
            }
        }

        [SerializeField] AudioClip sharpingSwordMainMenu;
        public AudioClip SharpingSwordMainMenu
        {
            get
            {
                return sharpingSwordMainMenu;
            }
        }

        [SerializeField] AudioClip[] takeDamage;
        public AudioClip[] TakeDamage
        {
            get
            {
                return takeDamage;
            }
        }

        [SerializeField] AudioClip[] takeHit;
        public AudioClip[] TakeHit
        {
            get
            {
                return takeHit;
            }
        }

        [SerializeField] AudioClip[] blockHit;
        public AudioClip[] BlockHit
        {
            get
            {
                return blockHit;
            }
        }

        [SerializeField] AudioClip charge;
        public AudioClip Charge
        {
            get
            {
                return charge;
            }
        }

        [SerializeField] AudioClip death;
        public AudioClip Death
        {
            get
            {
                return death;
            }
        }

        [SerializeField] AudioClip deathScreen;
        public AudioClip DeathScreen
        {
            get
            {
                return deathScreen;
            }
        }

        [SerializeField] AudioClip deathScreenWind;
        public AudioClip DeathScreenWind
        {
            get
            {
                return deathScreenWind;
            }
        }
    }

    [Serializable]
    public struct UIAudioClips
    {
        [SerializeField] AudioClip buttonClick;
        public AudioClip ButtonClick
        {
            get
            {
                return buttonClick;
            }
        }
    }
}
