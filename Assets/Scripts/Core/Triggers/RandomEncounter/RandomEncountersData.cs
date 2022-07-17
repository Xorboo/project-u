using System;
using UnityEngine;
using Utils;

namespace Core.Triggers.RandomEncounter
{
    [CreateAssetMenu]
    public class RandomEncountersData : ScriptableObject
    {
        [SerializeField]
        WeightedRandom<RandomEncounterData> Encounters = new WeightedRandom<RandomEncounterData>();


        #region Unity

        #endregion


        public RandomEncounterData GetRandomEncounter()
        {
            return Encounters.GetRandom();
        }
    }

    [Serializable]
    public class RandomEncounterData
    {
        public enum EncounterType
        {
            AddPercent,
            RemovePercent,
            VillageBonusA,
            VillageBonusB,
            IncreaseEnemyHp,
            RandomText,
            Shop
        };

        public string Description;
        public EncounterType Type;
        public int DiceChange;
    }
}