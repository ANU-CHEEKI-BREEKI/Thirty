using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadlessUnitSpawner : MonoBehaviour
{
    static Transform container;

    [SerializeField] Unit unitOriginal;

    [SerializeField] [Range(0, 30)] int count;
    [SerializeField] [Range(0, 10)] float radiusSpawn = 5;
    [SerializeField] [Range(0, 360)] float rangeRotation = 360;

    [SerializeField] Equipment helmet;
    [SerializeField] Equipment body;

    void Start ()
    {
        if(container == null)
        {
            var obj = GameObject.Find("NeutralUnitsContainer");
            if (obj == null)
                obj = new GameObject("NeutralUnitsContainer");
            container = obj.transform;
        }
        
        Vector2 pos = transform.position;

        int layer = LayerMask.NameToLayer(Squad.UnitFraction.NEUTRAL.ToString());

        for (int i = 0; i < count; i++)
        {
            Unit unit = Instantiate(
                unitOriginal,
                new Vector2(
                    pos.x + Random.value * radiusSpawn,
                    pos.y + Random.value * radiusSpawn
                ),
                Quaternion.identity * Quaternion.Euler(0, 0, Random.value * rangeRotation),
                container
            );

            unit.gameObject.layer = layer;
            unit.SetSelectionColor(Color.green);
            unit.delayToFindTargetAndAttack = (float)i / count;
            unit.SetBody(new EquipmentStack(body));
            unit.SetHelmet(new EquipmentStack(helmet));

            unit.name = "Neutral " + i;
        }

        Destroy(gameObject);
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, radiusSpawn);
    }
}
