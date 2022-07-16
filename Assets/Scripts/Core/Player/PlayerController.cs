using Core.Map;
using UnityEngine;

namespace Core.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        MapParameters MapParameters;


        public Vector2Int Coordinates { get; private set; }


        #region Unity

        #endregion


        public void SetSpawnPoint(Vector2Int spawnPoint)
        {
            Debug.Log($"Spawning player at {spawnPoint}");
            Coordinates = spawnPoint;
            transform.position = MapParameters.GetTileCenter3D(Coordinates);
        }
    }
}