using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class StatsIcon : MonoBehaviour
{
    [SerializeField] string editorText;
    [SerializeField] Sprite sprite;
    [Space]
    [SerializeField] TextMeshProUGUI txtmesh;
    [SerializeField] Image[] images;

    bool initiated = false;

    public string Text
    {
        set
        {
            if (txtmesh != null)
                txtmesh.text = value;
            initiated = true;
        }
    }

    public int DiaplayCount
    {
        set
        {
            int cnt = Mathf.Clamp(value, 0, images.Length);

            for (int i = 0; i < images.Length; i++)
            {
                if (i < cnt)
                    images[i].gameObject.SetActive(true);
                else
                    images[i].gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (txtmesh != null && !initiated)
            txtmesh.text = editorText;

        foreach (var img in images)
            if(img != null)
                img.sprite = sprite;
    }

#if UNITY_EDITOR
    void Update()
    {
        Init();
    }
#endif
}
