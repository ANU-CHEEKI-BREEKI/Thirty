using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[ExecuteInEditMode]
public class SetLanguageButton : MonoBehaviour
{
    [SerializeField] SystemLanguage language;
    [SerializeField] Sprite ico;
    [SerializeField] Sprite maskImage;
    [Space]
    [SerializeField] TextMeshProUGUI txtMesh;
    [SerializeField] Image image;
    [SerializeField] Image mask;
    Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        Init();
    }

    void Init()
    {
        if (txtMesh != null)
            txtMesh.text = language.ToString();

        if (mask != null)
            mask.sprite = maskImage;

        if (image != null)
            image.sprite = ico;
    }

#if UNITY_EDITOR

    private void Update()
    {
        Init();
    }

#endif

    void OnClick()
    {
        GameManager.Instance.Language = language;
    }
}
