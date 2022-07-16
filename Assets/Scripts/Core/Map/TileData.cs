using UnityEngine;

namespace Core.Map
{
    [CreateAssetMenu]
    public class TileData : ScriptableObject
    {
        public bool IsPassable = true;

        [Space]
        public bool HasEnemy = false;

        public int EnemyBaseHp = 2;
    }
}