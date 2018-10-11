using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundChannel : MonoBehaviour
{
    [Range(1, 30)] public int maxAudioSourceCount = 3;

    /// <summary>
    /// сурсы в пуле
    /// </summary>
    List<AudioSourseSet> poolSources;
    /// <summary>
    /// используемые сейчас сурсы
    /// </summary>
    List<AudioSourseSet> usedSources;

    public SoundManager.SoundType Type { get; set; }

    void Awake()
    {
        poolSources = new List<AudioSourseSet>(maxAudioSourceCount);
        usedSources = new List<AudioSourseSet>(maxAudioSourceCount);
        for (int i = 0; i < maxAudioSourceCount; i++)
            poolSources.Add(CreateNewSource("Source" + (i + 1)));
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
        else if(poolSources.Count + usedSources.Count < maxAudioSourceCount)
        {
            res = CreateNewSource("Source" + (poolSources.Count + usedSources.Count));

            usedSources.Add(res);
        }
        return res;
    }

    void ReturnBackSource(AudioSourseSet source)
    {
        if (usedSources.Remove(source))
        {
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
            source.source.Stop();
        }
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
            if(usedSources[0].priority < priority)
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
        
    /// <summary>
    /// останавливает проигрывание первого попавшегося клипа НЕ РЕАЛИЗОВАНО
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public bool StopPlayingClipQueue(List<AudioClip> clips)
    {
        throw new System.NotImplementedException();

        bool res = false;

        int cnt = usedSources.Count;
        for (int i = 0; i < cnt; i++)
        {
            //проверяем есть ли все клипы в очереди клипов
            
            if (clips.Count == usedSources[i].clipQueue.Count)
            {
                //создаемм два отсортированых списка и просто проверяэм все по порядку. если хоть что то не сходится - это не искомый список.
                List<AudioClip> clips1 = new List<AudioClip>(clips);
                clips1.Sort();
                List<AudioClip> clips2 = new List<AudioClip>(usedSources[i].clipQueue.Count);
                foreach (var item in usedSources[i].clipQueue)
                    clips2.Add(item.Clip);
                clips2.Sort();

                int cnt2 = clips1.Count;
                for (int j = 0; j < cnt2; j++)
                {

                }

            }
            //если все клипы есть, то останавливаем, иначе  -  проверяем следущий источник звука.


            //if (usedSources[i].clipQueue == clip && usedSources[i].source.isPlaying)
            //{
            //    if(usedSources[i].coroutineOneClip != null)
            //        StopCoroutine(usedSources[i].coroutineOneClip);
            //    ReturnBackSource(usedSources[i]);

            //    res = true;

            //    break;
            //}
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

        float timer = fade;

        while(timer > 0)
        {
            foreach (var item in usedSources)
                if(item.needToStop)
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
    }

    IEnumerator PlayClipsQueue(AudioSourseSet src, bool loop, float volume, SoundManager.SoundType type)
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
            src.Play();

            if (!src.source.loop)
            {
                var clip = src.clipQueue.Dequeue();

                if (loop)
                    src.clipQueue.Enqueue(clip);

                yield return new WaitForSecondsRealtime(src.source.clip.length);
            }
            else
                break;
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
