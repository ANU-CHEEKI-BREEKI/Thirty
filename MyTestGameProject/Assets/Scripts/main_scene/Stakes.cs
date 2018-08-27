using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Stakes : MonoBehaviour
{
    [SerializeField] Damage damage;
    /// <summary>
    /// время между активациями нанесения урона по стоящим юнитам
    /// </summary>
    [SerializeField] float damagingTickRate = 1.5f;
    /// <summary>
    /// время продолжительности нанесения урона по стоящим юнитам
    /// </summary>
    [SerializeField] float damagingDuration = 0.2f;

    Durabilityable durabilityble;
    Collider2D[] colliders;

    bool canDamage = false;

    public Squad owner = null;

    private void Awake()
    {
        if (damagingTickRate < damagingDuration)
            throw new System.Exception("damagingTickRate должно быть больше или равно damagingDuration");

        colliders = GetComponents<Collider2D>();

        durabilityble = GetComponent<Durabilityable>();

        if(durabilityble != null)
            durabilityble.OnBreak += Durabilityble_OnBreak;
    }

    private void Durabilityble_OnBreak()
    {
        Destroy(this);

        int cnt = colliders.Length;
        for (int i = 0; i < cnt; i++)
            Destroy(colliders[i]);

        Destroy(GetComponent<PlatformEffector2D>());
    }

    private void Start()
    {
        StartCoroutine(SwitchDamaging());
    }

    void OnDestroy()
    {
        if(durabilityble != null)
            durabilityble.OnBreak -= Durabilityble_OnBreak;
    }

    IEnumerator SwitchDamaging()
    {
        while (true)
        {
            canDamage = true;
            yield return new WaitForSeconds(damagingDuration);
            canDamage = false;
            yield return new WaitForSeconds(damagingTickRate - damagingDuration);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.enabled)
        {
            var unit = collision.gameObject.GetComponent<Unit>();
            if (unit != null)
                HitUnit(unit, damage);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.enabled)
        {
            var unit = collision.gameObject.GetComponent<Unit>();
            if (unit != null && !unit.Stanned && canDamage && unit.CurrentSpeed >= 20)
                HitUnit(unit, damage);
        }
    }

    void HitUnit(Unit unit, Damage damage)
    {
        unit.TakeHit(damage, owner: owner);

        if (durabilityble != null)
            durabilityble.TakeDamage(damage.BaseDamage);
    }
}
