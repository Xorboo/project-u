using UnityEngine;

namespace Core.Units
{
    [CreateAssetMenu]
    public class UnitData : ScriptableObject
    {
        public string UnitName = "Feral Hog";
        public int Hp = 2;
        public int HpScaleFactor = 1;

        [SerializeField]
        int MinDamage = 1;

        [SerializeField]
        int MaxDamage = 3;

        public int RandomDamage => Random.Range(MinDamage, MaxDamage + 1);

        [SerializeField]
        int MinLoot = 5;

        [SerializeField]
        int MaxLoot = 15;

        public int RandomLoot => Random.Range(MinLoot, MaxLoot + 1);
    }
}