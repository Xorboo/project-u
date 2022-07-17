using UnityEngine;

namespace Core.Map
{
    [CreateAssetMenu]
    public class TileData : ScriptableObject
    {
        public bool IsPassable = true;

        [Space]
        public bool IsMystic = false;
    }
}