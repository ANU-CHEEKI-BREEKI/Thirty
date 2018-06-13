using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI silver;
    [SerializeField] TextMeshProUGUI expirience;

    RectTransform rTransform;

    static public ScorePanel Instance { get; private set; }

    private void Awake()
    {
        rTransform = GetComponent<RectTransform>();
        Instance = this;

    }

    void Start ()
    {
        RefreshByEvent(0, 0, null);

        GameManager.Instance.PlayerProgress.Score.gold.OnValueChanged += RefreshByEvent;
        GameManager.Instance.PlayerProgress.Score.silver.OnValueChanged += RefreshByEvent;
        GameManager.Instance.PlayerProgress.Score.expirience.OnValueChanged += RefreshByEvent;
    }

    void OnDestroy()
    {
        GameManager.Instance.PlayerProgress.Score.gold.OnValueChanged -= RefreshByEvent;
        GameManager.Instance.PlayerProgress.Score.silver.OnValueChanged -= RefreshByEvent;
        GameManager.Instance.PlayerProgress.Score.expirience.OnValueChanged -= RefreshByEvent;
    }

    void RefreshByEvent(float oldValue, float newValue, DSPlayerScore.Score score)
    {        
        DSPlayerScore playerScore = GameManager.Instance.PlayerProgress.Score;

        if (score != null)
        {
            float deltaScore = score.Value - oldValue;

            if (deltaScore != 0)
            {
                Color color;

                string text = deltaScore.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);

                if (deltaScore <= 0)
                {
                    color = Color.red;
                }
                else
                {
                    color = Color.green;
                    text = "+" + text;
                }

                Vector2 pos = Vector2.zero;
                float fontSize = 20;

                if (score == playerScore.gold)
                {
                    pos = gold.transform.position;
                    fontSize = gold.fontSize;
                }
                else if (score == playerScore.silver)
                {
                    pos = silver.transform.position;
                    fontSize = silver.fontSize;
                }
                else if (score == playerScore.expirience)
                {
                    pos = expirience.transform.position;
                    fontSize = expirience.fontSize;
                }
                pos.y -= 1;

                PopUpTextController.Instance.AddTextLabel(
                    text,
                    pos,
                    1,
                    new Vector2(1, -2),
                    color,
                    color,
                    1,
                    fontSize - 10
                );
            }
        }

        gold.text = playerScore.gold.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
        silver.text = playerScore.silver.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
        expirience.text = playerScore.expirience.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rTransform);

    }

    IEnumerator WaitForeRebuildPanel()
    {
        bool active = gameObject.activeSelf;

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rTransform);
        gameObject.SetActive(false);
        gameObject.SetActive(active);

        yield return new WaitForEndOfFrame();
    }
}
