using System;
using Core.Map;
using Core.Player;
using UnityEngine;
using UrUtils.Misc;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        public event Action<PlayerController> OnPlayerSpawned = delegate { };


        [SerializeField]
        PlayerController PlayerPrefab;


        public PlayerController Player { get; private set; }


        #region Unity

        void OnEnable()
        {
            MapManager.Instance.OnTileClicked += TileClicked;
            StartGame();
        }

        void OnDisable()
        {
            if (MapManager.Exists())
                MapManager.Instance.OnTileClicked -= TileClicked;
        }

        #endregion


        void StartGame()
        {
            var spawnPoint = MapManager.Instance.InitializeMap();

            if (Player)
                Destroy(Player.gameObject);

            Player = Instantiate(PlayerPrefab);
            Player.SetSpawnPoint(spawnPoint);

            OnPlayerSpawned(Player);
        }

        void TileClicked(Vector2Int coord, Tile tile)
        {
            if (Player.CurrentState != PlayerController.State.Idle || !Player.IsAdjacent(coord))
                return;

            if (tile == null)
            {
                tile = MapManager.Instance.RevealTile(coord);
            }

            if (tile.Data.IsPassable)
                Player.MoveToTile(coord, tile);
        }
    }
}