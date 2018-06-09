using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundType { MUSIC, UI, FX }

    static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }

    public AudioClip MusicMainMenu
    {
        get
        {
            return musicMainMenu;
        }
    }
    public AudioClip MusicLevel
    {
        get
        {
            return musicLevel;
        }
    }
    public AudioClip MusicMarket
    {
        get
        {
            return musicMarket;
        }
    }
    public AudioClip MusicGameOver
    {
        get
        {
            return musicGameOver;
        }
    }
    public AudioClip BonfireMainMenu
    {
        get
        {
            return bonfireMainMenu;
        }
    }
    public AudioClip SharpingSwordMainMenu
    {
        get
        {
            return sharpingSwordMainMenu;
        }
    }

    public AudioClip[] TakeDamage
    {
        get
        {
            return takeDamage;
        }
    }
    public AudioClip[] TakeHit
    {
        get
        {
            return takeHit;
        }
    }
    public AudioClip[] BlockHit
    {
        get
        {
            return blockHit;
        }
    }
    public AudioClip Charge
    {
        get
        {
            return charge;
        }
    }
    public AudioClip Death
    {
        get
        {
            return death;
        }
    }
    public AudioClip DeathScreen
    {
        get
        {
            return deathScreen;
        }
    }
    public AudioClip DeathScreenWind
    {
        get
        {
            return deathScreenWind;
        }
    }

    [Header("Menu audio clips")]
    [SerializeField] AudioClip musicMainMenu;
    [SerializeField] AudioClip musicLevel;
    [SerializeField] AudioClip musicMarket;
    [SerializeField] AudioClip musicGameOver;
    [SerializeField] AudioClip bonfireMainMenu;
    [SerializeField] AudioClip sharpingSwordMainMenu;
    [Space]
    [Header("FX audio clips")]
    [SerializeField] AudioClip[] takeDamage;
    [SerializeField] AudioClip[] takeHit;
    [SerializeField] AudioClip[] blockHit;
    [SerializeField] AudioClip charge;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip deathScreen;
    [SerializeField] AudioClip deathScreenWind;
    [Space]
    [Header("UI audio clips")]
    [SerializeField] AudioClip buttonClick;

    [Space]
    [SerializeField] SoundChannel[] channels;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);


    }
    
    public void SetChannelAudioSourcesCount(SoundType type, int count)
    {
        var ch = channels[(int)type];
        var range = (RangeAttribute)Attribute.GetCustomAttribute(ch.maxAudioSourceCount.GetType(), typeof(RangeAttribute));

        if (count > range.max)
            count = (int)range.max;
        else if (count < range.min)
            count = (int)range.min;

        ch.maxAudioSourceCount = count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clipsQueue">список звуков, которые будут проигрываться по очереди</param>
    /// <param name="type">в какой "канал" закидвать звук</param>
    /// <param name="priority">приритет списка. чем больше число теб больше приоритет. список с большим приоритетом вытеснит список с более низким приоритетом</param>
    /// <param name="loop">повторять ли проигрывание списка</param>
    /// <param name="volumeDempfer">процент от макс громкости данного списка.</param>
    /// <returns></returns>
    public bool PlaySound(List<SoundChannel.ClipSet> clipsQueue, SoundType type, int priority = 0, bool loop = false, float volumeDempfer = 1)
    {
        return channels[(int)type].PlayClips(clipsQueue, type, priority, loop, volumeDempfer);
    }

    public bool PlaySound(SoundChannel.ClipSet clipSet, SoundType type, int priority = 0)
    {
        return PlaySound(new List<SoundChannel.ClipSet>() { clipSet }, type, priority);
    }

    public bool StopPlayingSound(AudioClip clip, SoundType type, bool loop)
    {
        throw new NotImplementedException();
        //return channels[(int)type].StopPlayingClipQueue(clip);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">название "канала" в котором надо остановить проигрывание</param>
    /// <param name="fade">длительность выключения</param>
    /// <returns></returns>
    public void StopPlayingChannel(SoundType type, float fade = 0)
    {
        channels[(int)type].StopPlayingAll(fade);
    }
}
