using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyTheGold : MonoBehaviour
{
    [SerializeField] float gold;
    [SerializeField] float cost;
    Button btn;
    Text text;

    void Start ()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
        if (btn.transform.childCount > 0)
            text = btn.transform.GetChild(0).GetComponent<Text>();
        if (text != null)
            text.text = "+" + gold.ToString();

    }

    void OnBtnClick()
    {
        AddGold(gold);
    }

    void AddGold(float gold)
    {
        GameManager.Instance.PlayerProgress.score.gold.Value += gold;
        GameManager.Instance.PlayerProgress.score.Save();
    }
}