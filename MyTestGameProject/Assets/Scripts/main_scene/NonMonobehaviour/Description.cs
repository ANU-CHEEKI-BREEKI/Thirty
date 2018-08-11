using UnityEngine;

public struct Description
{
    public Sprite Icon { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public DescriptionItem[] Constraints { get; set; }
    public DescriptionItem[] Stats { get; set; }
    public CostInfo? Cost { get; set; }
    public ConditionsInfo? Condition { get; set; }
    public string UseType { get; set; }

    //добавил так в тупую, потому что много где используется эта структура. и менять ооочень плохо
    public DescriptionItem[] SecondStats { get; set; }
    public string StatsName { get; set; }
    public string SecondStatsName { get; set; }

    public struct CostInfo
    {
        public int? CostPerOne { get; set; }
        public int? CostAll { get; set; }
        public DSPlayerScore.Currency? CostCurrency { get; set; }
    }

    public struct ConditionsInfo
    {
        public string Name { get; set; }
        public Conditions Value { get; set; }

        public enum Conditions { BAD, MEDIUM, GOOD }
    }

    public struct DescriptionItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Влияет на отображение на панели простотра статов (TipsPanel)
        /// </summary>
        public bool ItPositiveDesc { get; set; }
    }

}