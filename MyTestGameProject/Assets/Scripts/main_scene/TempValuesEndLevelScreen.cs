using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TempValuesEndLevelScreen : MonoBehaviour
{
    public static TempValuesEndLevelScreen Instance { get; set; }

    [SerializeField] ScorePanel score;
    [SerializeField] AllowedEquipmantPanel equipment;
    [Space]
    [SerializeField] TextMeshProUGUI textEquipmantTooltip;

    CanvasGroup cg;

    private void Awake()
    {
        Instance = this;
        cg = GetComponent<CanvasGroup>();
        Hide();
    }

    public void Show()
    {
        score.gameObject.SetActive(true);
        equipment.gameObject.SetActive(true);

        GameManager.Instance.Pause();
        score.Reset(true, false, true, false);
        score.popUpTextSpeed = new Vector2(0.5f, -1f);
        score.popUpTextLifetime = 1.5f;
        equipment.Reset();

        if (GameManager.Instance.SavablePlayerData.PlayerProgress.Equipment.TempAllowedEquipmentIdCopy.Count == 0)
            textEquipmantTooltip.text = LocalizedStrings.allowed_equipmant_none_tooltip;
        else
            textEquipmantTooltip.text = LocalizedStrings.allowed_equipmant_tooltip;

        FadeScreen.FadeOn(cg, 0.2f, this, () => 
            {
                cg.blocksRaycasts = true;
                GameManager.Instance.ApplyPlayerTempProgressValues();
            }
        );
    }

    public void Hide()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;

        score.gameObject.SetActive(false);
        equipment.gameObject.SetActive(false);
    }
}
