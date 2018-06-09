using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorAplhaBlinker : MonoBehaviour
{
    [SerializeField] [Range(0.01f, 1f)] float frequensy = 0.01f;
    float timer = 0;
    [Space]
    [SerializeField] [Range(0f, 1f)] float minAlpha = 1;
    [SerializeField] [Range(0f, 1f)] float maxAlpha = 1;
    [Space]
    [SerializeField] DefferendRenderers[] allRenderers;
    

    void Start()
    {
        foreach (var item in allRenderers)
            item.Init();
    }

    void Update()
    {
        if (timer >= frequensy)
        {
            Blink();
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    void Blink()
    {
        var alpha = Mathf.Lerp(minAlpha, maxAlpha, UnityEngine.Random.value);

        foreach (var item in allRenderers)
        {
            var color = Color.Lerp(item.FirstColor, item.SecondColor, UnityEngine.Random.value);
            foreach (var rnds in item.Rnds)
            {
                var newColor = rnds.StartColor * color;
                newColor.a = alpha * item.PercentAplha;
                if(item.InverseAplha)
                    newColor.a = maxAlpha - newColor.a + minAlpha;
                rnds.Renderer.color = newColor;
            }
        }
    }

    [Serializable]
    class DefferendRenderers
    {
        [SerializeField] SpriteRenderer[] renderers;
        [SerializeField] Color firstColor;
        [SerializeField] Color secondColor;
        [Space]
        [Tooltip("Это кароч коефициент в проценктах")]
        [SerializeField] [Range(0, 1)] float percentAplha = 1;
        [SerializeField] bool inverseAplha;
        RendererAndStartColor[] rnds;
        
        public Color FirstColor
        {
            get
            {
                return firstColor;
            }
        }
        public Color SecondColor
        {
            get
            {
                return secondColor;
            }
        }
        public RendererAndStartColor[] Rnds
        {
            get
            {
                return rnds;
            }
        }
        public float PercentAplha
        {
            get
            {
                return percentAplha;
            }
        }
        public bool InverseAplha
        {
            get
            {
                return inverseAplha;
            }
        }

        public void Init()
        {
            int cnt = renderers.Length;
            rnds = new RendererAndStartColor[cnt];
            for (int i = 0; i < cnt; i++)
                rnds[i] = new RendererAndStartColor(renderers[i]);
        }
    }

    [Serializable]
    struct RendererAndStartColor
    {
        SpriteRenderer renderer;
        Color startColor;

        public SpriteRenderer Renderer
        {
            get
            {
                return renderer;
            }
        }
        public Color StartColor
        {
            get
            {
                return startColor;
            }
        }

        public RendererAndStartColor(SpriteRenderer renderer)
        {
            this.renderer = renderer;
            startColor = renderer.color;
        }
    }
}
