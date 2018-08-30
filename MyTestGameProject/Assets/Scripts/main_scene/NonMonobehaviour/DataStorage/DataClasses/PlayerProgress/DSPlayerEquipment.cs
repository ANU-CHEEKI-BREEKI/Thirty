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

    public event Action<EquipmentStack> OnTempEquipmentAdded;

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

    public bool IsThisEquipmantAllowed(EquipmentStats stats)
    {
        return allowedEquipmentId.Contains(new EqId() { id = stats.Id, type = stats.Type});
    }

    public void AllowThisEquipment(EquipmentStats stats)
    {
        var eq = new EqId() { id = stats.Id, type = stats.Type };
        if (!allowedEquipmentId.Contains(eq))
            allowedEquipmentId.Add(eq);
    }

    public int DisallowThisEquipment(EquipmentStats stats)
    {
        return allowedEquipmentId.RemoveAll((e) => { return stats.Id == e.id && stats.Type == e.type; });
    }

    public bool IsThisEquipmantInTempValues(EquipmentStats stats)
    {
        return tempAllowedEquipmentId.Contains(new EqId() { id = stats.Id, type = stats.Type });
    }

    public void AddTempValue(EquipmentStack stack)
    {
        var eq = new EqId() { id = stack.EquipmentStats.Id, type = stack.EquipmentStats.Type };
        if (!tempAllowedEquipmentId.Contains(eq))
        {
            tempAllowedEquipmentId.Add(eq);
            if (OnTempEquipmentAdded != null)
                OnTempEquipmentAdded(stack);
        }
    }

    /// <summary>
    /// Применяет и сбрасывает
    /// </summary>
    public void ApplyTempValues()
    {
        allowedEquipmentId.AddRange(tempAllowedEquipmentId);

        ResetTempValues();
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
    public struct EqId
    {
        public int id;
        public EquipmentStats.TypeOfEquipment type;
    }
}