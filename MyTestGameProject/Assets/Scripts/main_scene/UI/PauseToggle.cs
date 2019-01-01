using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseToggle : MonoBehaviour
{
    [SerializeField] CanvasGroup darkscreen;

    Toggle t;

    private void Awake()
    {
        t = GetComponent<Toggle>();
        GameManager.Instance.OnGamePased += Instance_OnGamePased;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGamePased -= Instance_OnGamePased;
    }

    void Start ()
    {
        darkscreen.alpha = 0;
        darkscreen.blocksRaycasts = false;

        
        t.onValueChanged.AddListener(onValChanged);
	}

    private void Instance_OnGamePased(bool pause)
    {
        if (t != null)
        {
            t.isOn = pause;
            darkscreen.blocksRaycasts = pause;
        }
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
        GameManager.Instance.SetPauseGame(newVal);
    }
}
