using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadDirectionIndicator : MonoBehaviour
{
    [SerializeField] Squad squadToFollow;
    [Space]
    [SerializeField] GameObject directionDefault;
    [SerializeField] GameObject directionCharge;
    [Space]
    [SerializeField] SpriteRenderer defaultIndicator;
    [SerializeField] SpriteRenderer chargeIndicator;
    [SerializeField] SpriteRenderer[] otherChargeIndicators;
    [Space]
    [Tooltip("Радиус по которому бкдет крутиться индикатор")]
    [SerializeField] float radius = 3;
    [SerializeField] [Range(0f,1f)] float colorAlpfa = 0.3f;

    Transform transformToFolow;
    Transform thisTransform;

    void Start()
    {
        thisTransform = transform;

        if (squadToFollow == null)
            squadToFollow = GetComponent<Squad>();

        if (squadToFollow == null)
            squadToFollow = thisTransform.parent.GetComponent<Squad>();

        if (squadToFollow == null)
        {
            Destroy(gameObject);
            Debug.Log("Нет отряда для следования. Поэтому, индикатор был удалён.");
            return;
        }

        transformToFolow = squadToFollow.PositionsTransform;

        var gs = GameManager.Instance.SavablePlayerData.Settings.graphixSettings;
        Color color = Color.white;
        if (squadToFollow.fraction == Squad.UnitFraction.ALLY)
            color = gs.AllyOutlineColor;
        else if (squadToFollow.fraction == Squad.UnitFraction.ENEMY)
            color = gs.EnemyOutlineColor;
        color.a = colorAlpfa;

        defaultIndicator.color = color;
        chargeIndicator.color = color;

        color = Color.white;
        color.a = colorAlpfa * 0.7f;
        foreach (var item in otherChargeIndicators)
            item.color = color;

        squadToFollow.OnBeginCharge += SquadToFollow_OnBeginCharge;
        squadToFollow.OnEndCharge += SquadToFollow_OnEndCharge;

        SquadToFollow_OnEndCharge(new UnitStatsModifier());
    }

    private void OnDestroy()
    {
        if (squadToFollow != null)
        {
            squadToFollow.OnBeginCharge += SquadToFollow_OnBeginCharge;
            squadToFollow.OnEndCharge += SquadToFollow_OnEndCharge;
        }
    }

    private void SquadToFollow_OnEndCharge(UnitStatsModifier obj)
    {
        directionDefault.SetActive(true);
        directionCharge.SetActive(false);
    }

    private void SquadToFollow_OnBeginCharge(UnitStatsModifier obj)
    {
        directionDefault.SetActive(false);
        directionCharge.SetActive(true);
    }

    void Update ()
    {
        Vector2 newPos = squadToFollow.CenterSquad;
        Quaternion rot = squadToFollow.FlipRotation ? 
            transformToFolow.rotation * Quaternion.Euler(0, 0, 180) : 
            transformToFolow.rotation;

        float newRadius = squadToFollow.UnitCount / squadToFollow.SQUAD_LENGTH / 2 + radius;

        //sin и cos поменял местами потому что нужен сдвиг в 90 градусов из за точки отсчета вверху.
        //так проще чем добавлять 90 градусов каждый раз
        newPos.x += Tools.Math.Cos(rot.eulerAngles.z + 90) * newRadius;
        newPos.y += Tools.Math.Sin(rot.eulerAngles.z + 90) * newRadius;
        thisTransform.position = newPos;
        thisTransform.rotation = rot;
    }
}
