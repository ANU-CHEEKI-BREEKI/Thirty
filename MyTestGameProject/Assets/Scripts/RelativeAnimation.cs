using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeAnimation : MonoBehaviour
{
    public enum Mode { Simple, Loop, PingPong }
    public enum AutoPlay { None, Awake, Start }

    [SerializeField] Mode mode = Mode.Simple;

    [SerializeField] bool scaledTime = true;
    [Tooltip("Время между \"кадрами\" анимации")]
    [SerializeField] float frequensy = 0.001f;

    public AutoPlay autoPlay = AutoPlay.Start;

    public bool initWithFirstPont = true;

    [Tooltip("По умолчанию, анимируется объект на котором висит этот скрипт.\r\nЕсли этот массив не пустой, то анимируются только объекты в нем.")]
    [SerializeField] Transform[] toAnimate;

    [SerializeField] List<AnimationPoint> points;
    public List<AnimationPoint> Points { get { return points; } set { points = value; } }

    int index = 0;
    bool isPlaying;
    new Transform transform;
    int borderIndex;

    bool paused = false;

    Coroutine coroutine;

    void Awake()
    {
        transform = base.transform;
        borderIndex = points.Count - 1;

        if (autoPlay == AutoPlay.Awake)
            Play();
    }

    void Start()
    {
        if (autoPlay == AutoPlay.Start)
            Play();
    }

    void OnEnable()
    {
        if(paused)
        {
            Play();
            paused = false;
        }
    }

    void OnDisable()
    {
        if (isPlaying)
            paused = true;
        coroutine = null;
        Stop(true);
    }

    [ContextMenu("Play")]
    public void Play()
    {
        isPlaying = true;
        if (coroutine == null)
            coroutine = StartCoroutine(PlayMethod());
    }

    [ContextMenu("Stop")]
    public void Stop(bool immediately = false)
    {
        isPlaying = false;
        if (immediately && coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    [ContextMenu("Reset")]
    AnimationPoint Reset()
    {
        index = 0;
        var point = points[GetIndex()];
        transform.position = GetNewPosition(point);
        transform.rotation = GetNewRorartion(point);
        transform.localScale = GetNewScale(point);
        index++;
        return point;
    }

    int GetIndex()
    {
        return index < 0 ? -index : index;
    }

    [ContextMenu("Reverte")]
    public void Reverte()
    {
        var add = index < 0 ? -1 : 1;
        index = -index + add;
        borderIndex = index < 0 ? 0 : points.Count - 1;
    }

    void ApplyAnimation(Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
    {
        if (toAnimate == null || toAnimate.Length == 0)
            ApplyAnimationTo(transform, newPosition, newRotation, newScale);
        else
            foreach (var item in toAnimate)
                ApplyAnimationTo(item, newPosition, newRotation, newScale);

    }

    void ApplyAnimationTo(Transform transform, Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
    {
        transform.position = newPosition;
        transform.rotation = newRotation;
        transform.localScale = newScale;
    }

    Vector3 GetNewPosition(AnimationPoint point)
    {
        Vector3 newPos;
        if (point.positionTransform != null)
            newPos = point.positionTransform.position;
        else if (point.rotScaleTransform != null)
            newPos = point.rotScaleTransform.position;
        else
            newPos = transform.position;
        return newPos;
    }

    Quaternion GetNewRorartion(AnimationPoint point)
    {
        Quaternion newRotation;
        if (point.rotScaleTransform != null)
            newRotation = point.rotScaleTransform.rotation;
        else
            newRotation = transform.rotation;
        return newRotation;
    }

    Vector3 GetNewScale(AnimationPoint point)
    {
        Vector3 newScale;
        if (point.rotScaleTransform != null)
            newScale = point.rotScaleTransform.localScale;
        else
            newScale = transform.localScale;
        return newScale;
    }

    IEnumerator PlayMethod()
    {
        if(initWithFirstPont && !paused)
        {
            var point = Reset();
            if (scaledTime)
                yield return new WaitForSeconds(point.delay);
            else
                yield return new WaitForSecondsRealtime(point.delay);
        }

        while (isPlaying)
        {
            for (; index <= borderIndex; index++)
            {
                var point = points[GetIndex()];

                Vector3 oldPos = transform.position;
                Vector3 newPos = GetNewPosition(point);

                Quaternion oldRotation = transform.rotation;
                Quaternion newRotation = GetNewRorartion(point);

                Vector3 oldScale = transform.localScale;
                Vector3 newScale = GetNewScale(point);

                float duration = point.animationSped;
                float time = 0;

                while (time <= duration)
                {
                    time += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                    float lerpArg;
                    if (duration != 0)
                        lerpArg = time / duration;
                    else
                        lerpArg = 1;

                    transform.position = (Vector2)Vector3.Lerp(oldPos, newPos, lerpArg);
                    transform.rotation = Quaternion.Lerp(oldRotation, newRotation, lerpArg);
                    transform.localScale = (Vector2)Vector3.Lerp(oldScale, newScale, lerpArg);

                    if (scaledTime)
                        yield return new WaitForSeconds(frequensy);
                    else
                        yield return new WaitForSecondsRealtime(frequensy);
                }

                if (scaledTime)
                    yield return new WaitForSeconds(point.delay);
                else
                    yield return new WaitForSecondsRealtime(point.delay);
            }

            switch (mode)
            {
                case Mode.Simple:
                    Stop();
                    break;
                case Mode.Loop:
                    var point = Reset();
                    if (scaledTime)
                        yield return new WaitForSeconds(point.delay);
                    else
                        yield return new WaitForSecondsRealtime(point.delay);
                    break;
                case Mode.PingPong:
                    Reverte();
                    index++;
                    break;
            }
        }
        coroutine = null;
    }



    [Serializable]
    public class AnimationPoint
    {
        [Tooltip("Положение, поворот и скейл к которому будет стремится анимируемый Transform. Если задан otherTransformPosition, то позиция берется оттуда.")]
        public Transform rotScaleTransform;
        [Tooltip("Другой объект, к которому надо переместить анимируемый Transform. Задает только позицию.")]
        public Transform positionTransform;
        public float delay = 0;
        [Tooltip("Скорость анимиции в секундах (за это время выполнится полный переход между контрольными точками анимации)")]
        public float animationSped = 1;
    }
}
