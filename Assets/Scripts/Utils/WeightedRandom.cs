using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    [Serializable]
    public class WeightedRandom<T>
    {
        [SerializeField]
        List<WeightedElement<T>> Elements = new();

        [NonSerialized]
        float ChancesSum = float.NaN;

        public T GetRandom()
        {
            if (float.IsNaN(ChancesSum))
                ChancesSum = Elements.Sum(e => e.Chance);

            var random = Random.value * ChancesSum;
            foreach (var element in Elements)
            {
                random -= element.Chance;
                if (random <= 0)
                    return element.Data;
            }

            Debug.LogError($"Failed to get random weighted element [{typeof(T).Name}]");
            return default;
        }

        public IEnumerable<T> GetElements()
        {
            return Elements.Select(e => e.Data);
        }
    }

    [Serializable]
    public class WeightedElement<T>
    {
        public T Data;
        public float Chance = 1;
    }
}