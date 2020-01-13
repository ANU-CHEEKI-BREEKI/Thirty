using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace Tools
{
    public static class Extensions
    {
        #region EquipmentStats.Durability

        static public string GetNameLocalise(this EquipmentStats.Durability durability)
        {
            string res = string.Empty;
            switch (durability)
            {
                case EquipmentStats.Durability.DAMAGED:
                    res = LocalizedStrings.durability_damaged;
                    break;
                case EquipmentStats.Durability.WORN:
                    res = LocalizedStrings.durability_worn;
                    break;
                case EquipmentStats.Durability.NEW:
                    res = LocalizedStrings.durability_new;
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

        public static string GetNameLocalize(this HospitalRoom.HealTarget healTarget)
        {
            string res = string.Empty;
            switch (healTarget)
            {
                case HospitalRoom.HealTarget.WEAKEST:
                    res = LocalizedStrings.healtarget_weakest;
                    break;
                case HospitalRoom.HealTarget.STRONGEST:
                    res = LocalizedStrings.healtarget_strongest;
                    break;
                default:
                    res = LocalizedStrings.error;
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

        public static string GetNameLocalize(this DSPlayerScore.Currency currency)
        {
            string res = string.Empty;
            switch (currency)
            {
                case DSPlayerScore.Currency.GOLD:
                    res = LocalizedStrings.gold;
                    break;
                case DSPlayerScore.Currency.SILVER:
                    res = LocalizedStrings.silver;
                    break;
                case DSPlayerScore.Currency.EXPIRIENCE:
                    res = LocalizedStrings.expirience;
                    break;
                default:
                    res = LocalizedStrings.error;
                    break;
            }
            return res;
        }

        #endregion

        #region Executable.ExecatableUseType

        public static string GetNameLocalise(this Executable.ExecatableUseType useType)
        {
            string res = LocalizedStrings.missing_string;

            switch (useType)
            {
                case Executable.ExecatableUseType.CLICK:
                    res = LocalizedStrings.executable_use_type_click;
                    break;
                case Executable.ExecatableUseType.DRAG_DROP_PLASE:
                    res = LocalizedStrings.executable_use_type_drag_drop_place;
                    break;
                case Executable.ExecatableUseType.DRAG_DIRECTION:
                    res = LocalizedStrings.executable_use_type_drag_direction;
                    break;
            }

            return res;
        }

        #endregion

        #region FormationStats.Formations

        public static string GetNamelocalize(this FormationStats.Formations formation)
        {
            string res = string.Empty;

            switch (formation)
            {
                case FormationStats.Formations.RANKS:
                    res = LocalizedStrings.formation_ranks_name;
                    break;
                case FormationStats.Formations.PHALANX:
                    res = LocalizedStrings.formation_phalanx_name;
                    break;
                case FormationStats.Formations.RISEDSHIELDS:
                    res = LocalizedStrings.formation_shields_name;
                    break;
            }

            return res;
        }

        #endregion

        public static string GetNameLocalize(this UnitStats.EquipmentWeight obj)
        {
            var res = LocalizedStrings.missing_string;
            switch (obj)
            {
                case UnitStats.EquipmentWeight.VERY_LIGHT:
                    res = LocalizedStrings.weight_very_light;
                    break;
                case UnitStats.EquipmentWeight.LIGHT:
                    res = LocalizedStrings.weight_light;
                    break;
                case UnitStats.EquipmentWeight.MEDIUM:
                    res = LocalizedStrings.weight_medium;
                    break;
                case UnitStats.EquipmentWeight.HEAVY:
                    res = LocalizedStrings.weight_heavy;
                    break;
                case UnitStats.EquipmentWeight.VERY_HEAVY:
                    res = LocalizedStrings.weight_very_heavy;
                    break;
            }
            return res;
        }


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


        public static T GetComponentInParentUpToHierarchy<T>(this Transform transporm) where T : Component
        {
            T comp = null;
            Transform parent = transporm;

            while(parent != null && comp == null)
            {
                parent = parent.parent;
                if (parent != null)
                    comp = parent.GetComponent<T>();
            }

            return comp;
        }
    }
}
