using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Durabilityable : MonoBehaviour
{
    /// <summary>
    /// Случайный разброс поворота при уничтожении
    /// </summary>
    [SerializeField] [Range(0, 180)] float brokenRotationRange = 0;
    /// <summary>
    /// Поворот при уничтожении
    /// </summary>
    [SerializeField] [Range(-180, 180)] float brokenRrotation = 0;
    [Space]
    [SerializeField] float maximumDurability = 100;
    [ContextMenuItem("Refresh", "SetSprite")]
    [SerializeField] float durability = 100;
    /// <summary>
    /// Сопротивление этого объекта к урону
    /// <para>0 - урон проходит полностью, 1 - урон блокируется.</para>
    /// </summary>
    [Tooltip("Сопротивление этого объекта к урону.\r\n0 - урон проходит полностью, 1 - урон блокируется.")]
    [SerializeField]  [Range(0, 1)] float resistance;
    [Space]
    [SerializeField] Sprite[] durabilitySprites;
    [SerializeField] Sprite brokenSprite;

    int spritesCount;

    SpriteRenderer renderer;
    Transform thisTransform;

    public event Action OnBreak;

    bool alive;

    void Start()
    {
        alive = true;

        if (maximumDurability < durability)
            throw new System.Exception("maximumDurability должно быть больше или равно durability");
        if (maximumDurability == 0)
            throw new System.Exception("maximumDurability должно быть больше 0");

        thisTransform = transform;

        renderer = GetComponent<SpriteRenderer>();
        spritesCount = durabilitySprites.Length;

        SetSprite();

        if (durability <= 0)
            BreakDown();
    }

    /// <summary>
    /// Нанести урон по этому разрушаемому объекту.
    /// <para>Урон будет нанесен с учетом сопротивления к урону этого объекта.</para>
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>Если объект был уничтожен, возвратится true.</returns>
    public bool TakeDamage(float damage)
    {
        bool res = false;

        if (damage > 0 && durability > 0)
        {
            durability -= damage * (1 - resistance);

            if (durability <= 0)
            {
                BreakDown();
                res = true;
            }
            else
                SetSprite();
        }

        return res;
    }

    public void BreakDown()
    {
        if (alive)
        {
            alive = false;

            if (OnBreak != null)
            {
                OnBreak();
                OnBreak = null;
            }

            durability = 0;
            Destroy(this);
            thisTransform.rotation = thisTransform.rotation * Quaternion.Euler(0, 0, brokenRrotation);
            thisTransform.rotation = thisTransform.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-brokenRotationRange, brokenRotationRange));

            SetSprite(true);
        }
    }

    void SetSprite(bool broken = false)
    {
        if (!broken)
        {
            float t = durability / maximumDurability;
            int index = (int)Mathf.Round(Mathf.Lerp(spritesCount - 1, 0, t));

            if (index > spritesCount - 1)
                index = spritesCount - 1;

            renderer.sprite = durabilitySprites[index];
        }
        else
            renderer.sprite = brokenSprite;
    }
}
