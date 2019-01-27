using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DSPlayerEquipment : ITempValuesApplyable, IResetable, IMergeable
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
    
    public void Reset()
    {
        if (allowedEquipmentId == null)
            allowedEquipmentId = new List<EqId>();
        else
            allowedEquipmentId.Clear();

        allowedEquipmentId.Add(new EqId(1, EquipmentStats.TypeOfEquipment.SHIELD, false));
        allowedEquipmentId.Add(new EqId(1, EquipmentStats.TypeOfEquipment.WEAPON, false));

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

        int armourCount = 0;
        int weaponCount = 0;

        foreach (var item in tempAllowedEquipmentId)
        {
            if (item.Type == EquipmentStats.TypeOfEquipment.WEAPON)
                weaponCount++;
            else
                armourCount++;
        }

        GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_armourer_i, armourCount, null);
        GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_armourer_ii, armourCount, null);
        GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_armourer_iii, armourCount, null);

        GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_arms_dealer_i, weaponCount, null);
        GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_arms_dealer_ii, weaponCount, null);
        GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_arms_dealer_iii, weaponCount, null);
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

    public void Merge(object data)
    {
        var d = data as DSPlayerEquipment;

        foreach (var item in d.allowedEquipmentId)
            if (allowedEquipmentId.Find((e) => { return e.Id == item.Id && e.Type == item.Type; }) == null)
                allowedEquipmentId.Add(item);
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