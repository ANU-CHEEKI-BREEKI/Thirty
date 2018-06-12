using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public static class Extensions
{
    #region EquipmentStats.Durability

    static public string GetNameLocalise(this EquipmentStats.Durability durability)
    {
        string res = string.Empty;
        switch (durability)
        {
            case EquipmentStats.Durability.DAMAGED:
                res = Localization.durability_damaged;
                break;
            case EquipmentStats.Durability.WORN:
                res = Localization.durability_worn;
                break;
            case EquipmentStats.Durability.NEW:
                res = Localization.durability_new;
                break;
            default:
                res = "ERROR";
                break;
        }
        return res;
    }

    static public Color GetColor(this EquipmentStats.Durability durability)
    {
        Color res = Color.black;
        switch (durability)
        {
            case EquipmentStats.Durability.DAMAGED:
                res = new Color(133 / 255f, 0, 0);
                break;
            case EquipmentStats.Durability.WORN:
                res = new Color(133 / 255f, 102 / 255f, 0);
                break;
            case EquipmentStats.Durability.NEW:
                res = new Color(0, 110 / 255f, 0);
                break;
        }
        return res;
    }

    #endregion

    #region HospitalRoom.HealTarget

    public static string GetNameLocalise(this HospitalRoom.HealTarget healTarget)
    {
        string res = string.Empty;
        switch (healTarget)
        {
            case HospitalRoom.HealTarget.WEAKEST:
                res = Localization.healtarget_weakest;
                break;
            case HospitalRoom.HealTarget.STRONGEST:
                res = Localization.healtarget_strongest;
                break;
            default:
                res = Localization.error;
                break;
        }
        return res;
    }

    #endregion

    #region Description.ConditionsInfo.Conditions

    static public Color GetColor(this Description.ConditionsInfo.Conditions conditions)
    {
        Color res = Color.white;
        switch (conditions)
        {
            case Description.ConditionsInfo.Conditions.BAD:
                res = new Color(133 / 255f, 0, 0);
                break;
            case Description.ConditionsInfo.Conditions.MEDIUM:
                res = new Color(133 / 255f, 102 / 255f, 0);
                break;
            case Description.ConditionsInfo.Conditions.GOOD:
                res = new Color(0, 110 / 255f, 0);
                break;
        }
        return res;
    }

    #endregion

    #region Description.Currency

    public static string GetNameLocalise(this DSPlayerScore.Currency currency)
    {
        string res = string.Empty;
        switch (currency)
        {
            case DSPlayerScore.Currency.GOLD:
                res = Localization.gold;
                break;
            case DSPlayerScore.Currency.SILVER:
                res = Localization.silver;
                break;
            case DSPlayerScore.Currency.EXPIRIENCE:
                res = Localization.expirience;
                break;
            default:
                res = Localization.error;
                break;
        }
        return res;
    }

    #endregion

    #region

    public static string GetNameLocalise(this Skill.SkillUseType useType)
    {
        return useType.ToString();
    }

    #endregion

    static string exmes = "Такого поля не существует: ";
    public static object IncreaseNumberField(this object obj, float val, string[] pathFieldNames, int startIndex = 0, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    {
        if (startIndex >= pathFieldNames.Length)
            return null;

        Type firstType = obj.GetType();
        FieldInfo firstField = firstType.GetField(pathFieldNames[startIndex], flags);
        if (firstField == null)
        {
            StringBuilder sb = new StringBuilder(exmes);
            for (int i = 0; i <= startIndex; i++)
            {
                sb.Append(pathFieldNames[i]);
                sb.Append("/");
            }
            sb.Remove(sb.Length - 1, 1);
            throw new Exception(sb.ToString());
        }
        object firstObj = obj;

        if (startIndex < pathFieldNames.Length - 1)
        {
            firstField.SetValue(firstObj, IncreaseNumberField(firstField.GetValue(firstObj), val, pathFieldNames, startIndex + 1));
        }
        else
        {
            float currentValue = Convert.ToSingle(firstField.GetValue(firstObj));
            firstField.SetValue(firstObj, Convert.ChangeType(currentValue + val, firstField.FieldType));
        }
        return firstObj;
    }


    #region Tools

    static XmlSerializer blockSerializer = new XmlSerializer(typeof(MapBlock));

    public static void Serialize(string path, MapBlock block)
    {
        using (FileStream fstream = new FileStream(path, FileMode.Create))
            blockSerializer.Serialize(fstream, block);
    }

    public static MapBlock Deserialize(string path)
    {
        using (FileStream fstream = File.Open(path, FileMode.Open))
            return (MapBlock)blockSerializer.Deserialize(fstream);
    }

    public static MapBlock Deserialize(TextReader reader)
    {
        return (MapBlock)blockSerializer.Deserialize(reader);
    }

    #endregion
}
