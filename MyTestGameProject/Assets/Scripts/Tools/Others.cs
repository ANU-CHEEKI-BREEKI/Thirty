using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    }
}
