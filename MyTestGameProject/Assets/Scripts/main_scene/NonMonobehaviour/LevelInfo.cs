using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class LevelInfo : IResetable, IMergeable
{
    const int MAX_LEVEL = 3;

    [SerializeField] Ground.GroundType groundType = Ground.GroundType.GRASSLAND;
    [SerializeField] int level;
    [SerializeField] int maxLevel;
    [SerializeField] int wholeLevel;
    [SerializeField] int maxWholeLevel;

    public Ground.GroundType GroundType { get { return groundType; } private set { groundType = value; } }
    /// <summary>
    /// Текущий уровень на текущем типе уровней
    /// </summary>
    public int Level { get { return level; } private set { level = value; } }
    /// <summary>
    /// Максимальное количество уровней на одном типе уровней
    /// </summary>
    public int MaxLevel { get { return maxLevel; } private set { maxLevel = value; } }
    /// <summary>
    /// Текущий уровень включая все уровни всех типов (типа сквозная нумерация)
    /// </summary>
    public int WholeLevel { get { return wholeLevel; } private set { wholeLevel = value; } }
    /// <summary>
    /// Суммарное максимальное количество уровней
    /// </summary>
    public int MaxWholeLevel { get { return maxWholeLevel; } private set { maxWholeLevel = value; } }

    public float WholeLevelT { get { return (float)WholeLevel / MaxWholeLevel; } }

    public LevelInfo()
    {
        Reset();
    }

    public void SetValues(LevelInfo li)
    {
        GroundType = li.GroundType;
        MaxLevel = li.MaxLevel;
        Level = li.Level;
        WholeLevel = li.WholeLevel;
        MaxWholeLevel = li.MaxWholeLevel;
    }

    public void Reset()
    {
        GroundType = Ground.GroundType.GRASSLAND;
        MaxLevel = MAX_LEVEL;
        Level = 0;
        WholeLevel = 0;
        MaxWholeLevel = Enum.GetValues(typeof(Ground.GroundType)).Length * MaxLevel;
    }

    public  void Merge(object data)
    {
        var d = data as LevelInfo;

        GroundType = d.GroundType;
        MaxLevel = d.MaxLevel;
        Level = d.Level;
        WholeLevel = d.WholeLevel;
        MaxWholeLevel = d.WholeLevel;
    }

    public void NextLevel()
    {
        WholeLevel++;
        Level++;
        if (Level >= MaxLevel)
        {
            var v = (Enum.GetValues(typeof(Ground.GroundType)) as Ground.GroundType[]).ToList();
            var i = v.IndexOf(GroundType);
            if (i < v.Count - 1)
            {
                GroundType = v[i + 1];
                Level = 1;
            }
        }
    }
}
