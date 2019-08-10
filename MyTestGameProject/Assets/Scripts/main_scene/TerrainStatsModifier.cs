using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TerrainStatsModifier : MonoBehaviour
{
    [SerializeField] SOTerrainStatsModifier soModyfier;

    private void Awake()
    {
        //это всё на случай если я забываю выставить все настройки в редакторе. т.к. вложенных перфабов ещё нет и всё херится

        var cols = GetComponents<Collider2D>();
        foreach (var col in cols)
            col.isTrigger = true;

        gameObject.layer = 0;//default layer
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
