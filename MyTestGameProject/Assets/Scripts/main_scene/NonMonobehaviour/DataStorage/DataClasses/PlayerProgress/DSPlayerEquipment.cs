using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DSPlayerEquipment : ISavable, IResetable, ITempValuesApplyable
{
    public List<EqId> allowedEquipmentId;
    [SerializeField] List<EqId> tempAllowedEquipmentId;
    public List<EqId> TempAllowedEquipmentIdCopy
    {
        get
        {
            return new List<EqId>(tempAllowedEquipmentId);
        }
    }

    public DSPlayerEquipment()
    {
        Reset();
    }

    public void Save()
    {
        GameManager.Instance.SavingManager.SaveData<DSPlayerEquipment>(this.GetType().Name, this);
    }

    public void Load()
    {
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
        var g = GameManager.Instance.SavingManager.LoadData<DSPlayerEquipment>(this.GetType().Name);
        var fields = this.GetType().GetFields(flags);
        foreach (var f in fields)
            f.SetValue(this, f.GetValue(g));
    }

    public void Reset()
    {
        if (allowedEquipmentId == null)
            allowedEquipmentId = new List<EqId>();
        else
            allowedEquipmentId.Clear();

        ResetTempValues();
    }

    public EqId GetEquipmantAllowed(EquipmentStats stats)
    {
        if (!IsThisEquipmantAllowed(stats))
            throw new Exception("Ну ты че. проверь сначала...");

        var eq = new EqId(stats.Id, stats.Type);
        return allowedEquipmentId.Find(e => e.Id == eq.Id && e.Type == eq.Type);
    }

    public bool IsThisEquipmantAllowed(EquipmentStats stats)
    {
        var eq = new EqId(stats.Id, stats.Type);
        return allowedEquipmentId.FindIndex(e => e.Id == eq.Id && e.Type == eq.Type) >= 0;
    }

    public void AllowThisEquipment(EquipmentStats stats)
    {
        var eq = new EqId(stats.Id, stats.Type);
        if (!IsThisEquipmantAllowed(stats))
            allowedEquipmentId.Add(eq);
    }

    public int DisallowThisEquipment(EquipmentStats stats)
    {
        return allowedEquipmentId.RemoveAll((e) => { return stats.Id == e.Id && stats.Type == e.Type; });
    }

    public bool IsThisEquipmantInTempValues(EquipmentStats stats)
    {
        var eq = new EqId(stats.Id, stats.Type);
        return tempAllowedEquipmentId.FindIndex(e => e.Id == eq.Id && e.Type == eq.Type) >= 0;
    }

    public void AddTempValue(EquipmentStats stats)
    {
        var eq = new EqId(stats.Id, stats.Type);
        if (!IsThisEquipmantInTempValues(stats))
            tempAllowedEquipmentId.Add(eq);
    }

    /// <summary>
    /// Применяет
    /// </summary>
    public void ApplyTempValues()
    {
        allowedEquipmentId.AddRange(tempAllowedEquipmentId);
    }
    
    /// <summary>
    /// Сбрасывает
    /// </summary>
    public void ResetTempValues()
    {
        if (tempAllowedEquipmentId == null)
            tempAllowedEquipmentId = new List<EqId>();
        else
            tempAllowedEquipmentId.Clear();
    }

    [Serializable]
    public class EqId
    {
        [SerializeField] int id;
        [SerializeField] EquipmentStats.TypeOfEquipment type;
        [SerializeField] bool isNew;

        public int Id { get { return id; } private set { id = value; } }
        public EquipmentStats.TypeOfEquipment Type { get { return type; } private set { type = value; } }
        public bool IsNew { get { return isNew; } private set { isNew = value; } }

        public EqId(int id, EquipmentStats.TypeOfEquipment type, bool isNew = true)
        {
            Id = id;
            Type = type;
            IsNew = isNew;
        }

        public EqId(EqId eqid, int? id = null, EquipmentStats.TypeOfEquipment? type = null, bool? isNew = null)
        {
            Id = eqid.Id;
            Type = eqid.Type;
            IsNew = eqid.IsNew;

            if (id != null)
                Id = id.Value;
            if (type != null)
                Type = type.Value;
            if (isNew != null)
                IsNew = isNew.Value;
        }
    }
}