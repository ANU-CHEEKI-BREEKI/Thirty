using System;
using UnityEngine;

[Serializable]
public class DSUnitStats : IResetable, IMergeable
{
    [SerializeField] Stat health = new Stat(50, 200, 50);
    public Stat Health { get { return health; } }

    [SerializeField] Stat attack = new Stat(0.2f, 0.7f, 0.2f);
    public Stat Attack { get { return attack; } }

    [SerializeField] Stat defence = new Stat(0.2f, 0.7f, 0.2f);
    public Stat Defence { get { return defence; } }

    [SerializeField] Stat speed = new Stat(35, 50, 35);
    public Stat Speed { get { return speed; } }

    [SerializeField] Stat aceleration = new Stat(35, 60, 35);
    public Stat Aceleration { get { return aceleration; } }

    [SerializeField] Stat rotationSpeed = new Stat(360, 460, 360);
    public Stat RotationSpeed { get { return rotationSpeed; } }

    [Serializable]
    public class Stat
    {
        [SerializeField] float minValue;
        public float MinValue { get { return minValue; } }

        [SerializeField] float maxValue;
        public float MaxValue { get { return maxValue; } }

        [SerializeField] float value;
        /// <summary>
        /// Значение не может быть больше максимального или меньше минимального, установленных при создании, значений
        /// </summary>
        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;
                if (this.value > maxValue) this.value = maxValue;
                if (this.value < minValue) this.value = minValue;
            }
        }

        [SerializeField] float progress;
        /// <summary>
        /// Свойство для контроля прогресса прокачки с предыдущего уровня на следующий. НЕ ПОЛНОЙ ПРОКАЧКИ, А БЛИзЛЕЖАЩИХУРОВНЕЙ.
        /// Короче говоря, для возможности вкинуть свободный опыт, если остался лишний.
        /// </summary>
        public float Progress
        {
            get { return progress; }
            set
            {
                if (value < 0)
                    progress = 0;
                else if (value > 1)
                    progress = 1;
                else
                    progress = value;
            }
        }

        public Stat(float min, float max, float val)
        {
            minValue = min;
            maxValue = max;
            Value = val;
            progress = 0;
        }
    }

    public DSUnitStats()
    {
        Reset();
    }
    
    public void RepairMaxMinRanges()
    {
        //get current data copy
        var dataCopy = JsonUtility.FromJson(JsonUtility.ToJson(this), this.GetType());
        //reset data to repair min and max ranges
        this.Reset();
        //merge data to restore val value
        this.Merge(dataCopy);
    }

    public void Reset()
    {
        health = new Stat(100, 200, 50);
        attack = new Stat(0.4f, 0.7f, 0.2f);
        defence = new Stat(0.4f, 0.7f, 0.2f);
        speed = new Stat(35, 50, 35);
        aceleration = new Stat(35, 60, 35);
        rotationSpeed = new Stat(360, 460, 360);
    }

    public void Merge(object data)
    {
        var d = data as DSUnitStats;

        health.Value = Mathf.Max(health.Value, d.health.Value);
        attack.Value = Mathf.Max(attack.Value, d.attack.Value);
        defence.Value = Mathf.Max(defence.Value, d.defence.Value);
        speed.Value = Mathf.Max(speed.Value, d.speed.Value);
        aceleration.Value = Mathf.Max(aceleration.Value, d.aceleration.Value);
        rotationSpeed.Value = Mathf.Max(rotationSpeed.Value, d.rotationSpeed.Value);
    }
}
