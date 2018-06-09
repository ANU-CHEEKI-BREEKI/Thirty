using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToMarket : MonoBehaviour
{
    Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        FadeScreen ds = GameObject.FindWithTag("DarkScreen").GetComponent<FadeScreen>();
        ds.OnFadeOn += GameManager.Instance.LoadMarket;

        ds.FadeOn(0.5f);
    }
}
