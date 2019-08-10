using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundChannel : MonoBehaviour
{
    [Range(1, 1000)] public int maxAudioSourceCount = 3;

    /// <summary>
    /// сурсы в пуле
    /// </summary>
    List<AudioSourseSet> poolSources;
    /// <summary>
    /// используемые сейчас сурсы
    /// </summary>
    List<AudioSourseSet> usedSources;

    /// <summary>
    /// используемые сейчас сурсы
    /// </summary>
    Dictionary<object, List<AudioSourseSet>> managedUsedSources;

    public SoundManager.SoundType Type { get; set; }

    void Awake()
    {
        poolSources = new List<AudioSourseSet>(maxAudioSourceCount);
        usedSources = new List<AudioSourseSet>(maxAudioSourceCount);
        for (int i = 0; i < maxAudioSourceCount; i++)
            poolSources.Add(CreateNewSource("Source" + (i + 1)));

        managedUsedSources = new Dictionary<object, List<AudioSourseSet>>(maxAudioSourceCount);
    }

    AudioSourseSet CreateNewSource(string name)
    {
        var go = new GameObject(name);
        go.transform.parent = transform;
        AudioSource res = go.AddComponent<AudioSource>();
        res.playOnAwake = false;

        if (Type == SoundManager.SoundType.FX)
        {
            res.spatialBlend = 1;
            res.minDistance = 7;
            res.maxDistance = 12;
        }
        else
        {
            res.spatialBlend = 0;
        }

        if (poolSources.Count + usedSources.Count > maxAudioSourceCount)
            Debug.Log("Было создано аудиосурсов больше чем установлено в настройках скрипта!!!");

        return new AudioSourseSet() { source = res };
    }
    
    AudioSourseSet GetSource()
    {
        AudioSourseSet res = new AudioSourseSet();
        if (poolSources.Count > 0)
        {
            res = poolSources[0];
            poolSources.RemoveAt(0);

            usedSources.Add(res);
        }
        else if(poolSources.Count + usedSources.Count + managedUsedSources.Count < maxAudioSourceCount)
        {
            res = CreateNewSource("Source" + (poolSources.Count + usedSources.Count + managedUsedSources.Count));

            usedSources.Add(res);
        }
        return res;
    }

    AudioSourseSet GetManagedSource(object key, Transform parent = null)
    {
        AudioSourseSet res = new AudioSourseSet();

        if (!managedUsedSources.ContainsKey(key))
            managedUsedSources.Add(key, new List<AudioSourseSet>(maxAudioSourceCount));

        if (poolSources.Count > 0)
        {
            res = poolSources[0];
            poolSources.RemoveAt(0);
            managedUsedSources[key].Add(res);
        }
        else if (poolSources.Count + usedSources.Count + managedUsedSources.Count < maxAudioSourceCount)
        {
            res = CreateNewSource("Source" + (poolSources.Count + usedSources.Count + managedUsedSources.Count));
            managedUsedSources[key].Add(res);
        }

        if(res.source != null)
            res.PosToUpdate = parent;

        return res;
    }

    internal void StopPlayingManagedClips(object key, float fade = 0)
    {
        GameManager.Instance.StartCoroutine(StopPlayingManagedClipsCoroutine(key, fade));
    }

    IEnumerator StopPlayingManagedClipsCoroutine(object key, float fade)
    {
        if (managedUsedSources.ContainsKey(key))
        {
            var srcs = managedUsedSources[key];

            foreach (var item in srcs)
            {
                item.fadeStartVolume = item.source.volume;
                item.needToStop = true;
            }

            float timer = fade;

            while (timer > 0)
            {
                foreach (var item in srcs)
                    if (item.needToStop)
                        item.source.volume = Mathf.Lerp(0, item.fadeStartVolume, timer / fade);

                timer -= Time.unscaledDeltaTime;
                yield return null;
            }
                        
            ReturnBackManagedSources(key);
        }
    }

    void ReturnBackManagedSources(object key)
    {
        if (!managedUsedSources.ContainsKey(key))
            return;

        var sources = managedUsedSources[key];

        foreach (var source in sources)
            ReturnBackSource(source);

        managedUsedSources.Remove(key);
    }

    void ReturnBackSource(AudioSourseSet source)
    {
        usedSources.Remove(source);

        poolSources.Add(source);
        if (source.coroutineClipQueue != null)
        {
            StopCoroutine(source.coroutineClipQueue);
            source.coroutineClipQueue = null;
        }
        source.clipQueue = null;
        source.fadeStartVolume = 1;
        source.priority = 0;
        source.needToStop = false;
        //source.source.transform.parent = transform;
        source.PosToUpdate = null;
        source.source.Stop();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clipsQueue">список аудио клипов, которые будут проигрываться по очереди</param>
    /// <param name="priority">приоритет. чем больше число, тем больше приоритет. если нет свободных источнико звука, то более приоритетная очередь клипов вытеснит самую менее приоритетную, из проигрываемых сейчас</param>
    /// <param name="loop">иповторять ли проигрывание данной очереди клипов</param>
    /// <returns></returns>
    public bool PlayClips(List<ClipSet> clipsQueue, SoundManager.SoundType type, int priority = 0, bool loop = false, float volume = 1)
    {
        bool res = true;
        var src = GetSource();
        if(src.source != null)
        {
            src.clipQueue = new Queue<ClipSet>(clipsQueue);
            src.coroutineClipQueue = StartCoroutine(PlayClipsQueue(src, loop, volume, type));
        }
        else
        {
            if(usedSources.Count > 0 && usedSources[0].priority < priority)
            {
                ReturnBackSource(usedSources[0]);
                src = GetSource();
                src.clipQueue = new Queue<ClipSet>(clipsQueue);
                src.coroutineClipQueue = StartCoroutine(PlayClipsQueue(src, loop, volume, type));
                Debug.Log("Был заменен источник звука с боле низким приоритетом");
            }
            else
            {
                Debug.Log("В пуле нет доступного сурса");
                res = false;
            }
        }

        return res;
    }

    public bool PlayManagedClip(object key, Transform parent, ClipSet clip, SoundManager.SoundType type, int priority = 0, float volume = 1, float updateDeltaTime = 0.5f)
    {
        bool res = true;
        var src = GetManagedSource(key, parent);
        if (src.source != null)
        {
            src.clipQueue = new Queue<ClipSet>();
            src.clipQueue.Enqueue(clip);
            src.coroutineClipQueue = StartCoroutine(PlayClipsQueue(src, false, volume, type, updateDeltaTime));
        }
        else
        {
            if (usedSources.Count > 0 && usedSources.Any(s=>s.priority < priority))
            {
                var prirsrc = usedSources.Where(s => s.priority < priority).OrderBy(s=>s.priority).First();
                ReturnBackSource(prirsrc);
                src = GetManagedSource(key, parent);
                src.clipQueue = new Queue<ClipSet>();
                src.clipQueue.Enqueue(clip);
                src.coroutineClipQueue = StartCoroutine(PlayClipsQueue(src, false, volume, type));
                Debug.Log("Был заменен источник звука с боле низким приоритетом");
            }
            else if (managedUsedSources.Count > 0 && managedUsedSources.Any(s => s.Value.Any(ss=>ss.priority < priority)))
            {
                var prirsrc = managedUsedSources.Where(s => s.Value.Any(ss => ss.priority < priority))
                    .SelectMany(s => s.Value.Where(ss => ss.priority < priority))
                    .OrderBy(s => s.priority)
                    .First();
                ReturnBackSource(prirsrc);
                src = GetManagedSource(key, parent);
                src.clipQueue = new Queue<ClipSet>();
                src.clipQueue.Enqueue(clip);
                src.coroutineClipQueue = StartCoroutine(PlayClipsQueue(src, false, volume, type));
                Debug.Log("Был заменен источник звука с боле низким приоритетом");
            }
            else
            {
                Debug.Log("В пуле нет доступного сурса");
                res = false;
            }
        }

        return res;
    }
        
    
    public void StopPlayingAll(float fade = 0)
    {
        GameManager.Instance.StartCoroutine(StopPlayingAllCoroutine(fade));
    }

    IEnumerator StopPlayingAllCoroutine(float fade)
    {
        foreach (var item in usedSources)
        {
            item.fadeStartVolume = item.source.volume;
            item.needToStop = true;
        }

        foreach (var key in managedUsedSources.Keys)
        {
            foreach (var item in managedUsedSources[key])
            {
                item.fadeStartVolume = item.source.volume;
                item.needToStop = true;
            }            
        }

        float timer = fade;

        while(timer > 0)
        {
            foreach (var item in usedSources)
                if(item.needToStop)
                    item.source.volume = Mathf.Lerp(0, item.fadeStartVolume, timer / fade);

            foreach (var key in managedUsedSources.Keys)
                foreach (var item in managedUsedSources[key])
                    if (item.needToStop)
                        item.source.volume = Mathf.Lerp(0, item.fadeStartVolume, timer / fade);

            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        for (int i = 0; i < usedSources.Count; i++)
        {
            if (usedSources[i].needToStop && usedSources[i].source.isPlaying)
            {
                if (usedSources[i].coroutineClipQueue != null)
                    StopCoroutine(usedSources[i].coroutineClipQueue);
                ReturnBackSource(usedSources[i]);
                i--;
            }
        }

        var tkeys = new object[managedUsedSources.Keys.Count];
        managedUsedSources.Keys.CopyTo(tkeys, 0);

        foreach (var key in tkeys)
            ReturnBackManagedSources(key);
    }

    IEnumerator PlayClipsQueue(AudioSourseSet src, bool loop, float volume, SoundManager.SoundType type, float updateDeltaTime = 0.5f)
    {
        while (src.clipQueue.Count > 0)
        {
            var clipSet = src.clipQueue.Peek();

            src.currentClipSet = clipSet;
            src.source.clip = clipSet.Clip;
            src.source.loop = clipSet.Loop;
            src.volumeDempfer = volume;
            src.Type = type;
            src.source.transform.position = src.clipQueue.Peek().Position;
            //src.source.transform.localPosition = src.clipQueue.Peek().Position;
            src.Play();

            if (!src.source.loop)
            {
                var clip = src.clipQueue.Dequeue();

                if (loop)
                    src.clipQueue.Enqueue(clip);
                
                //если установили позицию для апдейта, то обновляем регулярно...
                if (src.PosToUpdate != null)
                {
                    var timer = 0f;
                    var duration = src.source.clip.length;
                    while (timer < duration)
                    {
                        timer += Time.unscaledDeltaTime;
                        if(src.PosToUpdate != null)
                            src.source.transform.position = src.PosToUpdate.position;
                        yield return new WaitForSecondsRealtime(updateDeltaTime);
                    }
                }
                else
                {
                    yield return new WaitForSecondsRealtime(src.source.clip.length);
                }
            }
            else
            {
                if (src.PosToUpdate != null)
                {
                    while (true)
                    {
                        if (src.PosToUpdate != null)
                            src.source.transform.position = src.PosToUpdate.position;
                        yield return new WaitForSecondsRealtime(updateDeltaTime);
                    }
                }
                else break;
            }

        }

        if (src.clipQueue.Count == 0)
            ReturnBackSource(src);
    }

    class AudioSourseSet
    {
        public AudioSource source;
        public Coroutine coroutineClipQueue;
        public int priority;
        public Queue<ClipSet> clipQueue;
        public ClipSet currentClipSet;

        public Transform PosToUpdate = null;

        public float fadeStartVolume;
        public bool needToStop;

        public float volumeDempfer = 1;

        float chanelVolume;
        float generalVolume;

       
        
        public float Volume { get { return source.volume; } }

        SoundManager.SoundType type;
        public SoundManager.SoundType Type
        {
            get { return type; }
            set
            {
                type = value;

                switch (value)
                {
                    case SoundManager.SoundType.MUSIC:
                        GameManager.Instance.SavablePlayerData.Settings.audioSettings.musicVolume.OnValueChanged += OnVolumeChanged;
                        chanelVolume = GameManager.Instance.SavablePlayerData.Settings.audioSettings.musicVolume.Value;
                        break;
                    case SoundManager.SoundType.UI:
                        GameManager.Instance.SavablePlayerData.Settings.audioSettings.uiVolume.OnValueChanged += OnVolumeChanged;
                        chanelVolume = GameManager.Instance.SavablePlayerData.Settings.audioSettings.uiVolume.Value;
                        break;
                    case SoundManager.SoundType.FX:
                        GameManager.Instance.SavablePlayerData.Settings.audioSettings.fxVolume.OnValueChanged += OnVolumeChanged;
                        chanelVolume = GameManager.Instance.SavablePlayerData.Settings.audioSettings.fxVolume.Value;
                        break;
                }

                GameManager.Instance.SavablePlayerData.Settings.audioSettings.generalVolume.OnValueChanged += OnGeneralVolumeChanged;
                generalVolume = GameManager.Instance.SavablePlayerData.Settings.audioSettings.generalVolume.Value;
            }
        }
       
        void OnVolumeChanged(float val)
        {
            chanelVolume = val;
            SetVolume();
        }
       
        void OnGeneralVolumeChanged(float val)
        {
            generalVolume = val;
            SetVolume();
        }

        void SetVolume()
        {
            source.volume = generalVolume * chanelVolume * currentClipSet.Volume * volumeDempfer;
        }

        public void Play()
        {
            SetVolume();
            source.Play();
        }
    }

    public struct ClipSet
    {
        float volumeDempfer;

        public AudioClip Clip { get; }
        public bool Loop { get; }
        public float Volume { get { return volumeDempfer; } }
        public Vector2 Position { get; }

        public ClipSet(AudioClip clip, bool loop = false, float volumeDempfer = 1, Vector2? pos = null)
        {
            this.Clip = clip;
            this.volumeDempfer = volumeDempfer;
            this.Loop = loop;
            if (pos != null)
                Position = pos.Value;
            else
                Position = Vector2.zero;
        }
    }
}
