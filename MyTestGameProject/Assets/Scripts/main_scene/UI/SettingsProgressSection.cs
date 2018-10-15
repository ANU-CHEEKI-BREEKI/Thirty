using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsProgressSection : MonoBehaviour
{
    [SerializeField] Button resetScore;
    [SerializeField] Button resetStats;
    [SerializeField] Button resetSkills;
    [SerializeField] Button resetLevel;
    [SerializeField] Button resetEquipment;

    private void Start()
    {
        resetScore.onClick.AddListener(GameManager.Instance.ResetScore);
        resetStats.onClick.AddListener(GameManager.Instance.ResetStats);
        resetSkills.onClick.AddListener(GameManager.Instance.ResetSkills);
        resetLevel.onClick.AddListener(GameManager.Instance.ResetLevel);
        resetEquipment.onClick.AddListener(GameManager.Instance.ResetAllowedEquipment);
    }
}
