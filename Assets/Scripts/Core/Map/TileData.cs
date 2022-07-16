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
        public int EnemyHpScale = 1;

        [Space]
        public bool HasChest = false;

        [Space]
        public bool HasRandomEvent = false;

        [Space]
        public bool IsMystic = false;

        public int EnemyFullHp => EnemyBaseHp + EnemyHpScale * GameManager.Instance.EnemyExtraHpFactor;
    }
}