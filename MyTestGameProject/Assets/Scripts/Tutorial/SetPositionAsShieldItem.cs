using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionAsShieldItem : MonoBehaviour
{
    [SerializeField] Transform rootTransform;

    private void Start()
    {
        Invoke();
    }

    public void Invoke()
    {
        Stack<Transform> ch = new Stack<Transform>();
        ch.Push(rootTransform);
        while (ch.Count > 0)
        {
            var currTr = ch.Pop();
            int chCnt = currTr.childCount;
            var invCell = currTr.GetComponent<InventoryCellUI>();

            if (invCell != null)
            {
                if(chCnt > 0)
                {
                    bool finded = false;
                    for (int i = 0; i < chCnt; i++)
                    {
                        var chch = currTr.GetChild(i);
                        var equip = chch.GetComponent<DragEquipment>();
                        if(equip != null && equip.EquipStack != null)
                        {
                            if (equip.EquipStack.Count > 0 && equip.EquipStack.EquipmentStats.Type == EquipmentStats.TypeOfEquipment.SHIELD)
                            {
                                transform.position = chch.position;
                                finded = true;
                                break;
                            }
                        }                   
                    }
                    if (finded)
                        break;
                }
            }

            for(int i = 0; i < chCnt; i++)
                ch.Push(currTr.GetChild(i));
        }
    }
}
