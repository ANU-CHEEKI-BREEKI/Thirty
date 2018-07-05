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

    [SerializeField] SOSoundContainer soundClipsContainer;
    public SOSoundContainer SoundClipsContainer { get { return soundClipsContainer; } }
   
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

        var t = Enum.GetValues(typeof(SoundType)) as SoundType[];
        for (int i = 0; i < t.Length; i++)
        {
            channels[i].name = t[i].ToString();
            channels[i].Type = t[i];
        }
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
