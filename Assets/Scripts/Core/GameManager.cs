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
            StartGame();
        }

        #endregion


        public void StartGame()
        {
            var spawnPoint = MapManager.Instance.InitializeMap();

            if (Player)
                Destroy(Player.gameObject);

            Player = Instantiate(PlayerPrefab);
            Player.SetSpawnPoint(spawnPoint);

            OnPlayerSpawned(Player);
        }
    }
}