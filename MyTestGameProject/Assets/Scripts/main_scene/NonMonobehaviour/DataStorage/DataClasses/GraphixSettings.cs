using System;
using UnityEngine;

[Serializable]
public class GraphixSettings : ISavable, IResetable
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
    /// Как обводить юнитов (по контуру или подложкой под юнита)
    /// </summary>
    public OutlineTypes OutlineType = OutlineTypes.BORDER;
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

    public GraphixSettings()
    {
        Reset();
    }

    public void Save()
    {
        GameManager.Instance.SavingManager.SaveData<GraphixSettings>(this.GetType().Name, this);
    }

    public void Load()
    {
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
        var g = GameManager.Instance.SavingManager.LoadData<GraphixSettings>(this.GetType().Name);
        var fields = this.GetType().GetFields(flags);
        foreach (var f in fields)
            f.SetValue(this, f.GetValue(g));
    }

    public void Reset()
    {
        ShowDamage = true;
        DamageToEnemyColor = Color.blue;
        DamageToAllyColor = Color.red;
        FrieldlyDamageColor = Color.yellow;
        OutlineType = OutlineTypes.UNDERLAYER;
        AllyOutline = true;
        AllyOutlineColor = Color.blue;
        EnemyOutline = true;
        EnemyOutlineColor = Color.red;
        NeutralOutline = true;
        NeutralOutlineColor = Color.yellow;
    }
}
