using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PilumsVolley : MonoBehaviour
{
    //система частиц
    [SerializeField] ParticleSystem pSys;

    //физический слой для коллизии
    LayerMask mask;

    //закешированая ссылка на юнита, в которого попал снаряд
    Unit unit;

    //урон, наносимый пилумами
    [SerializeField] Damage damage;

    //kем был инициирован залп
    [HideInInspector] public Squad owner = Squad.playerSquadInstance;

    Action<int, Squad> CallbackUsedCount;
    int countPilumsInValley = 0;
    
    public void Init(Vector2 positionOfTarget, Damage damage, float distance, float speed, int countOfPilumsToVolley, Squad owner = null, Action<int, Squad> callbackUsedCount = null)
    {
        //кол-во юнитов отряде. - то есть и кол во пилумов в залпе
        int countOfUnits = 30;
        if (owner != null)
            countOfUnits = owner.UnitCount;

        if (countOfPilumsToVolley > countOfUnits)
            countPilumsInValley = countOfUnits;
        else
            countPilumsInValley = countOfPilumsToVolley;

        this.owner = owner;
        
        //ширина строя  (скорее всего надо будет потом переделать. т.к. хочу в будущем переделать построения)
        int rowLength = owner.SQUAD_LENGTH;
        if (rowLength > countOfUnits)
            rowLength = countOfUnits;
        
        //макс кол-во пилумов
        var main = pSys.main;
        main.maxParticles = countPilumsInValley;

        //ставим наальную скорость в 0, так как контролировать скорость будем через velocityOverLifetime
        var ss = main.startSpeed;
        ss.constant = 0;
        main.startSpeed = ss;

        //длительность жисзни частиц и проигрывания анимации. для соответствия указанной дальности полёта пилумов
        var lt = main.startLifetime;
        lt.constant = distance / speed;
        main.startLifetime = lt;
        main.duration = main.startLifetime.constant;

        //ширина появления пилумов
        var shape = pSys.shape;
        shape.radius = rowLength / 2f;

        //направление полета пилумов. чтоб можно было не только впереди строя кидать, но и в бок
        var velosOvT = pSys.velocityOverLifetime;
        velosOvT.enabled = true;
        velosOvT.space = ParticleSystemSimulationSpace.World;

        var x = velosOvT.x;
        var y = velosOvT.y;

        var angle = positionOfTarget - (Vector2)transform.position;
        angle = angle.normalized * speed;

        x.constant = angle.x;
        y.constant = angle.y;

        velosOvT.x = x;
        velosOvT.y = y;

        //чтоб самаго себя не бил убираем колизию свого слоя
        var collis = pSys.collision;
        mask = collis.collidesWith;
        mask.value = mask.value & ~(1 << LayerMask.NameToLayer(owner.fraction.ToString()));
        collis.collidesWith = mask;

        this.damage = damage;

        CallbackUsedCount = callbackUsedCount;
    }

    public void StartVolley()
    {
        pSys.Play();
        Destroy(gameObject, pSys.main.duration);
        if(CallbackUsedCount!= null)
            CallbackUsedCount(countPilumsInValley, owner);

        if (owner != null && owner == Squad.playerSquadInstance)
            GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_velite, countPilumsInValley, null);
    }

    private void OnParticleCollision(GameObject other)
    {
        unit = other.GetComponent<Unit>();
        if (unit != null)
            unit.TakeHitFromArrow(damage, transform.position, owner);
    }
}
