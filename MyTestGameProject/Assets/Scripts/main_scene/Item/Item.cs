using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Item : MonoBehaviour
{
    [Serializable]
    public struct MainProperties
    {
        [Header("Item")]
        [SerializeField] Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField] string pathToPrefab;// = @"Prefabs/Items/";
        public string PathToPrefab { get { return pathToPrefab; } }

        [SerializeField] string stringResourceName;
        public string StringResourceName { get { return stringResourceName; } }

        [SerializeField] string stringResourceDescription;
        public string StringResourceDescription { get { return stringResourceDescription; } }

        [Space]
        [SerializeField] DSPlayerScore.Currency currency;
        public DSPlayerScore.Currency Currency { get { return currency; } }


        public void ResetIcon()
        {
            var eq = Resources.Load<Item>(pathToPrefab);            
            if(eq != null)
                icon = eq.MainPropertie.Icon;
        }
    }

    [Header("Item properties")]
    [SerializeField] MainProperties mainProperties = new MainProperties();
    public MainProperties MainPropertie { get { return mainProperties; } set { mainProperties = value; } }
}
