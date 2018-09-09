using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class DSFlags : IResetable, IMergeable
{
    [SerializeField] bool isFirstStartGame;
    [SerializeField] bool needTraining;
    //для туториала костыль (
    [SerializeField] GameManager.SceneIndex avalaibleTutorialLevel;

    public bool IsFirstStartGame { get { return isFirstStartGame; } private set { isFirstStartGame = value; } }
    public bool NeedTraining { get { return needTraining; } private set { needTraining = value; } }
    public GameManager.SceneIndex AvalaibleTutorialLevel { get { return avalaibleTutorialLevel; } set { avalaibleTutorialLevel = value; } }

    public DSFlags()
    {
        Reset();
    }

    public void Reset()
    {
        isFirstStartGame = true;
        needTraining = true;

        avalaibleTutorialLevel = GameManager.SceneIndex.LEVEL_TUTORIAL_1;
    }

    public void Merge(object data)
    {
        var d = data as DSFlags;

        isFirstStartGame = d.isFirstStartGame || isFirstStartGame;
        needTraining = d.needTraining || needTraining;

        if (d.avalaibleTutorialLevel > avalaibleTutorialLevel)
            avalaibleTutorialLevel = d.avalaibleTutorialLevel;
    }
}
