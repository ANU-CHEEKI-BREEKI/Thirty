using UnityEngine;
using UnityEngine.EventSystems;

public class ShowSettingsSection : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.SetSiblingIndex(transform.parent.childCount);
    }
}
