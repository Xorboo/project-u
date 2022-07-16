using UnityEngine;

namespace Core
{
    [CreateAssetMenu]
    public class GameParameters : ScriptableObject
    {
        public int StartingDice = 20;

        public int VillageBonusA = 2;
        public int VillageBonusB = 3;
        public float DiceMultiplyFactor = 0.1f;
    }
}