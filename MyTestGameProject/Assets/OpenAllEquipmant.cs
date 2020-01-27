using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpenAllEquipmant : MonoBehaviour
{
    private void Start()
    {
        var allEquips = Resources.LoadAll<Equipment>("");
        var allStats = allEquips
            .Where(e => e != null)
            .Where(e => !e.Stats.Empty)
            .Select(e => e.Stats);

        foreach (var stat in allStats)
            GameManager.Instance.SavablePlayerData.PlayerProgress.Equipment.AllowThisEquipment(stat);

    }
}
