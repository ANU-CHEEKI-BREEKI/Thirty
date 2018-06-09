using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image targetGraphix;
    RectTransform targetGraphixTransform;

    [SerializeField] TextMeshProUGUI textHp;
    [SerializeField] TextMeshProUGUI textCount;

    Squad playerSquad;

	void Start ()
    {
        playerSquad = Squad.playerSquadInstance;

        targetGraphixTransform = (RectTransform)targetGraphix.transform;
        targetGraphixTransform.anchorMax = Vector2.one;

        playerSquad.OnSumHealthChanged += DrawHpBar;
        playerSquad.OnUitCountChanged += DrawCountUnits;

        DrawHpBar(playerSquad.SquadHealth);
        DrawCountUnits(playerSquad.UnitCount);
    }

    void DrawCountUnits(int newCount)
    {
        textCount.text = newCount + "/" + playerSquad.FULL_SQUAD_UNIT_COUNT;
    }

    private void OnDestroy()
    {
        if (playerSquad != null)
        {
            playerSquad.OnSumHealthChanged -= DrawHpBar;
            playerSquad.OnUitCountChanged -= DrawCountUnits;
        }
    }

    void DrawHpBar(float squadHealth)
    {
        float maxHp = playerSquad.UnitStats.Health * playerSquad.FULL_SQUAD_UNIT_COUNT;

        float t = squadHealth / maxHp;
        Color color;
        if (t >= 0.5f)
            color = Color.Lerp(Color.yellow, Color.green, t*2 - 1 - 0.3f);
        else
            color = Color.Lerp(Color.red, Color.yellow, t*2 - 0.3f);

        targetGraphix.color = color;
        targetGraphixTransform.anchorMax = new Vector2(squadHealth / maxHp, 1);

        textHp.text = squadHealth.ToString("0") + "/" + maxHp;
    }
}

