using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class GraphixSettings : ICopyabe
{
    public enum OutlineTypes { BORDER, UNDERLAYER }

    /// <summary>
    /// Показывать ли всплывающий урон
    /// </summary>
    public bool ShowDamage = true;
    /// <summary>
    /// Цвет урона по врагу (когда врагу игрока наносится урон)
    /// </summary>
    public Color32 DamageToEnemyColor = Color.blue;
    /// <summary>
    /// Цвет урона по игроку (когда игроку наносят урон)
    /// </summary>
    public Color32 DamageToAllyColor = Color.red;
    /// <summary>
    /// Цвет урона по союзникам (когда игрок наносит урон союзникам или самому себе)
    /// </summary>
    public Color32 FrieldlyDamageColor = Color.yellow;
    [Space]
    /// <summary>
    /// Обводить ли юнитов игрока
    /// </summary>
    public bool AllyOutline = true;
    /// <summary>
    /// Цвет обводки юнитов игрока
    /// </summary>
    public Color32 AllyOutlineColor = Color.blue;
    [Space]
    /// <summary>
    ///  Обводить ли юнитов врага
    /// </summary>
    public bool EnemyOutline = true;
    /// <summary>
    ///  Цвет обводки юнитов врага
    /// </summary>
    public Color32 EnemyOutlineColor = Color.red;
    [Space]
    /// <summary>
    ///  Обводить ли неитральных юнитов
    /// </summary>
    public bool NeutralOutline = true;
    /// <summary>
    ///  Цвет обводки союзных юнитов
    /// </summary>
    public Color32 NeutralOutlineColor = Color.yellow;

    public void Reset()
    {
        ShowDamage = true;
        DamageToEnemyColor = Color.blue;
        DamageToAllyColor = Color.red;
        FrieldlyDamageColor = Color.yellow;
        AllyOutline = true;
        AllyOutlineColor = Color.blue;
        EnemyOutline = true;
        EnemyOutlineColor = Color.red;
        NeutralOutline = true;
        NeutralOutlineColor = Color.yellow;
    }

    public object Copy()
    {
        BindingFlags f = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var res = new GraphixSettings();

        foreach (var fiels in GetType().GetFields(f))
            fiels.SetValue(res, fiels.GetValue(this));

        return res;
    }
}
