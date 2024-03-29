﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ArrowsValley : MonoBehaviour
{
    //система частиц для залпа стрел
	ParticleSystem realValleyParticleSystem;

    //система частиц для предупредительного залпа
    ParticleSystem warningValleyParticleSystem;

    //список для частиц в коллайдере из модуля системы частиц Triggers
    List<ParticleSystem.Particle> trigPartc;

    //список для частиц, которые уже остановились
    List<ParticleSystem.Particle> stayingParticles;

    //физический слой для каста стрел на юнитов
    int layerMask;

    //закешированая ссылка на юнита, в которого попала стрела (чтоб не создавать много мусора)
	Unit unit;

    //закешированая ссылка на результат каста
    RaycastHit2D rhit;

    //количество уже упавших на землю стрел. 
    //нужно для того чтобы не наваливать циклами по все частицам,когда они уже все остановились
    int hittedArrowsCount;
    //макс число стрел в залпе.
    //нужно для того чтобы не наваливать циклами по все частицам,когда они уже все остановились
    int particlesCount;

    //задержка между предупредительным и настоящим залпами
    [SerializeField] [Range(0, 10)] float dalayBeforeWarningValley;

	public Damage damage;

    [HideInInspector] public Squad owner = Squad.playerSquadInstance;

    [SerializeField] Transform endPoint;

    [SerializeField] LayerMask raycastTargetLayers;

    [SerializeField] bool runOnAwake;
    
    private void Awake()
    {
        realValleyParticleSystem = GetComponent<ParticleSystem>();
        trigPartc = new List<ParticleSystem.Particle>(realValleyParticleSystem.main.maxParticles);

        warningValleyParticleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();

        layerMask = raycastTargetLayers.value;

        if (runOnAwake)
            StartValley();
    }

    IEnumerator DestroyValleyObject()
    {
        while(realValleyParticleSystem.IsAlive())
        {
            yield return new WaitForSeconds(1);
        }
        Destroy(gameObject);
    }

    public void Init(Vector2 position, Damage damage, float radius, int countOfArrows, Squad owner = null, float dalayBeforeWarningValley = 2)
    {
        this.dalayBeforeWarningValley = dalayBeforeWarningValley;

        transform.position = position - (Vector2)endPoint.position;

        this.damage = damage;

        radius = radius < 2 ? 2 : radius;
        //radius = radius > 10 ? 10 : radius;

        var sh = realValleyParticleSystem.shape;
        sh.radius = radius - 1;
        sh = warningValleyParticleSystem.shape;
        sh.radius = radius - 1;

        var main = realValleyParticleSystem.main;
        var speed = main.startSpeed;
        speed.constantMin = 35 - radius / 2;
        speed.constantMax = 35 + radius / 2;
        main.startSpeed = speed;

        main = warningValleyParticleSystem.main;
        speed = main.startSpeed;
        speed.constantMin = 35 - radius / 2;
        speed.constantMax = 35 + radius / 2;
        main.startSpeed = speed;

        var emit = realValleyParticleSystem.emission;
        var rate = realValleyParticleSystem.emission.rateOverTime;
        rate.constant = countOfArrows + 1;
        emit.rateOverTime = rate;

        this.owner = owner;
    }

    public void StartValley()
    {
        hittedArrowsCount = 0;
        particlesCount = (int)(realValleyParticleSystem.emission.rateOverTime.constant * realValleyParticleSystem.main.duration);

        StartCoroutine(GoToLowLevelRendering(
            warningValleyParticleSystem,
            warningValleyParticleSystem.main.duration + warningValleyParticleSystem.main.startLifetime.constant * 0.4f
        ));
        StartCoroutine(WAitForWarningValley(dalayBeforeWarningValley));
        warningValleyParticleSystem.Play(false);

        StartCoroutine(DestroyValleyObject());

        if(owner!= null && owner == Squad.playerSquadInstance)
            GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_archer, 1, null);

    }

    IEnumerator WAitForWarningValley(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(GoToLowLevelRendering(
            realValleyParticleSystem,
            realValleyParticleSystem.main.duration + realValleyParticleSystem.main.startLifetime.constant * 0.4f
        ));
        realValleyParticleSystem.Play(false);
    }

    IEnumerator GoToLowLevelRendering(ParticleSystem pSys, float time)
	{
		yield return new WaitForSeconds(time);
        pSys.gameObject.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Item");
	}

    string PROFILING_NAME = "Залп Стрел -> OnParticleTrigger";

    void OnParticleTrigger()
    {
        UnityEngine.Profiling.Profiler.BeginSample(PROFILING_NAME);

        RealValley();

        UnityEngine.Profiling.Profiler.EndSample();
    }
    
    void RealValley()
    {
        trigPartc.Clear();
        int count = realValleyParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, trigPartc);

        if (hittedArrowsCount < particlesCount)
        {
            for (int i = 0; i < count; i++)
            {
                if (trigPartc[i].velocity.x == 0 && trigPartc[i].velocity.y == 0)
                {
                    //список для частиц в коллайдере из модуля системы частиц Triggers
                    float t = trigPartc[i].remainingLifetime - trigPartc[i].startLifetime * (1 - 0.4f);
                    t = t < 0 ? -t : t;
                    //если время жизни стрелы меньше 0,08f то она только что упала на землю
                    if (t <= 0.08f)
                    {
                        hittedArrowsCount++;

                        rhit = Physics2D.CircleCast(trigPartc[i].position, realValleyParticleSystem.trigger.radiusScale, Vector2.zero, 0, layerMask);

                        Debug.DrawLine(
                            trigPartc[i].position + new Vector3(0.2f, 0.2f, 0),
                            trigPartc[i].position + new Vector3(-0.2f, -0.2f, 0),
                            Color.red
                        );
                        Debug.DrawLine(
                            trigPartc[i].position + new Vector3(-0.2f, 0.2f, 0),
                            trigPartc[i].position + new Vector3(0.2f, -0.2f, 0),
                            Color.red
                        );

                        if (rhit)
                        {
                            unit = rhit.collider.GetComponent<Unit>();
                            if (unit != null)
                                unit.TakeHitFromArrow(damage, transform.position, owner);

                            ParticleSystem.Particle p = trigPartc[i];
                            p.remainingLifetime = -1;
                            trigPartc[i] = p;
                        }
                    }
                }
            }
        }
        realValleyParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, trigPartc);
    }
}
