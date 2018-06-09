using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseToggle : MonoBehaviour
{
    [SerializeField] CanvasGroup darkscreen;

    void Start ()
    {
        darkscreen.alpha = 0;
        darkscreen.blocksRaycasts = false;

        GetComponent<Toggle>().onValueChanged.AddListener(onValChanged);
	}

    public void onValChanged(bool newVal)
    {
        if(newVal)
        {
            darkscreen.alpha = 1;
        }
        else
        {
            darkscreen.alpha = 0;
            if (DialogBox.Instance.Showned && DialogBox.Instance.Owner == this)
                DialogBox.Instance.Hide();
        }

        darkscreen.blocksRaycasts = newVal;
        GameManager.Instance.SetPause(newVal);
    }
}
