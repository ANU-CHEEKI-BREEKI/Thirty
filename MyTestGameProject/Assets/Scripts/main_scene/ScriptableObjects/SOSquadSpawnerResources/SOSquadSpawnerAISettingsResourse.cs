using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SOSquadSpawnerResourse/AISettings", fileName = "SO_SSR_AISettings")]
public class SOSquadSpawnerAISettingsResourse : ScriptableObject
{
    [SerializeField] AiSquadController.AiSquadBehaviour mode = AiSquadController.AiSquadBehaviour.DEFEND;
    public AiSquadController.AiSquadBehaviour Mode { get { return mode; } }

    [SerializeField] FormationStats.Formations startFormation = FormationStats.Formations.RANKS;
    public FormationStats.Formations StartFormation { get { return startFormation; } }

    [Space] [SerializeField] bool useDefaultDistancesOptions;
    public bool UseDefaultDistancesOptions { get { return useDefaultDistancesOptions; } }

    [ContextMenuItem("Reset values", "ResetDistansesSettings")]
    [SerializeField] AiSquadController.DistancesSettings distancesOptions;
    public AiSquadController.DistancesSettings DistancesOptions { get { return distancesOptions; } }

    [Space] [SerializeField] bool useDefaultReformOptions;
    public bool UseDefaultReformOptions { get { return useDefaultReformOptions; } }

    [ContextMenuItem("Reset values", "ResetReformSettings")]
    [SerializeField] AiSquadController.ReformSettings reformOptions;
    public AiSquadController.ReformSettings ReformOptions { get { return reformOptions; } }
    
    void ResetDistansesSettings()
    {
        distancesOptions.Reset();
    }

    void ResetReformSettings()
    {
        reformOptions.Reset();
    }
}
