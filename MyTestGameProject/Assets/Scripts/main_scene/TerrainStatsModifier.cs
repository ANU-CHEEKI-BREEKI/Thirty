using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TerrainStatsModifier : MonoBehaviour
{
    [SerializeField] SOTerrainStatsModifier soModyfier;
    
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var unit = collision.gameObject.GetComponent<Unit>();
        if (unit == null)
            return;

        unit.AddTerrainStatsModifyer(soModyfier);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var unit = collision.gameObject.GetComponent<Unit>();
        if (unit == null)
            return;

        unit.RemoveTerrainStatsModifyer(soModyfier);
        Debug.Log("exit");
    }
}
