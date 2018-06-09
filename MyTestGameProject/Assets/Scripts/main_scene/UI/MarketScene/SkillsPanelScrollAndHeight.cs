using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))] [ExecuteInEditMode]
public class SkillsPanelScrollAndHeight : MonoBehaviour
{
    LayoutElement yel;
    new RectTransform transform;
    [SerializeField] RectTransform followTransform;

    void Start ()
    {
        transform = (RectTransform)base.transform;
        yel = GetComponent<LayoutElement>();	
	}
	
	void Update ()
    {
        yel.minHeight = followTransform.rect.height;
        Vector3 pos = transform.position;
        pos.y = followTransform.position.y;
        transform.position = pos;
	}
}
