using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Tools
{
    public static class Others
    {
        public static UnitStats.EquipmentWeight GetWeightByMass(float equipmentMass)
        {
            if (equipmentMass <= (float)UnitStats.EquipmentWeight.VERY_LIGHT)
                return UnitStats.EquipmentWeight.VERY_LIGHT;
            else if (equipmentMass <= (float)UnitStats.EquipmentWeight.LIGHT)
                return UnitStats.EquipmentWeight.LIGHT;
            else if (equipmentMass <= (float)UnitStats.EquipmentWeight.MEDIUM)
                return UnitStats.EquipmentWeight.MEDIUM;
            else if (equipmentMass <= (float)UnitStats.EquipmentWeight.HEAVY)
                return UnitStats.EquipmentWeight.HEAVY;
            else return UnitStats.EquipmentWeight.VERY_HEAVY;
        }

        static public IEnumerator Cooldown(float duration, Action onStart, Action onEnd, TextMeshProUGUI text = null, string format = StringFormats.intNumber, float step = 0.1f)
        {
            float t = duration;

            onStart();

            while (t > 0)
            {
                if (text != null)
                    text.text = t.ToString(StringFormats.intNumber);

                yield return new WaitForSeconds(step);

                t -= step;
            }

            if (text != null)
                text.text = string.Empty;

            onEnd();
        }

        static public T[] GetAllComponentsWithAllChildrens<T>(Transform parent) where T : Component
        {
            List<T> maps = new List<T>();

            var mps = parent.GetComponent<T>();
            if (mps != null)
                maps.Add(mps);

            int cnt = parent.childCount;
            for (int i = 0; i < cnt; i++)
            {
                var ch = parent.GetChild(i);
                maps.AddRange(GetAllComponentsWithAllChildrens<T>(ch));
            }

            return maps.ToArray();
        }

        static public Transform FindChildWithNameContains(Transform parent, string containsStr)
        {
            Transform res = null;

            int cnt = parent.childCount;
            for (int i = 0; i < cnt; i++)
            {
                var ch = parent.GetChild(i);
                if (ch.name.Contains(containsStr))
                {
                    res = ch;
                    break;
                }
            }

            return res;
        }
    }
}
