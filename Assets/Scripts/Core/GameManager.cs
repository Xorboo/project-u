using System;
using Core.Map;
using Core.Player;
using UnityEngine;
using UnityEngine.Tilemaps;
using UrUtils.Misc;
using Tile = Core.Map.Tile;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        public event Action<PlayerController> OnPlayerSpawned = delegate { };
        public event Action<int> OnDiceCountChanged = delegate { };
        public event Action<bool> OnDieWaitingChanged = delegate { };


        [SerializeField]
        PlayerController PlayerPrefab;

        [SerializeField]
        GameParameters Data;


        public int DiceCount
        {
            get => DiceCountValue;
            private set
            {
                DiceCountValue = value;
                OnDiceCountChanged(DiceCountValue);
            }
        }

        int DiceCountValue = 0;

        public bool IsWaitingForDie
        {
            get => IsWaitingForDieValue;
            private set
            {
                IsWaitingForDieValue = value;
                OnDieWaitingChanged(IsWaitingForDieValue);
            }
        }

        bool IsWaitingForDieValue = false;

        public bool IsVisitingTile { get; private set; } = false;

        Action<int> DieResultListener = null;


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


        public void OnDieThrown(int dieResult)
        {
            if (!IsWaitingForDie)
            {
                Debug.LogError($"Die thrown when not waiting for it");
                return;
            }

            DieResultListener?.Invoke(dieResult);
        }

        void StartGame()
        {
            DiceCount = Data.StartingDice;
            IsWaitingForDie = false;
            DieResultListener = null;

            var spawnPoint = MapManager.Instance.InitializeMap();

            if (Player)
                Destroy(Player.gameObject);

            Player = Instantiate(PlayerPrefab);
            Player.SetSpawnPoint(spawnPoint);

            OnPlayerSpawned(Player);
        }

        void TileClicked(Vector2Int coord, Tile tile)
        {
            if (IsVisitingTile || IsWaitingForDie || Player.CurrentState != PlayerController.State.Idle || !Player.IsAdjacent(coord))
                return;

            if (tile != null)
            {
                if (tile.Data.IsPassable)
                    Player.MoveToTile(coord, tile);
                return;
            }

            if (!WaitForDie((value) => RevealDiceThrown(coord, value)))
            {
                RestartGame();
                return;
            }
        }

        void RevealDiceThrown(Vector2Int coord, int dieResult)
        {
            var tile = MapManager.Instance.RevealTile(coord, dieResult);

            if (tile.Data.IsPassable)
                Player.MoveToTile(coord, tile, () => PostPlayerMove(coord, tile));
        }

        public bool WaitForDieThrowResult(Action<int> onDiceThrown)
        {
            if (IsWaitingForDie)
            {
                Debug.LogError($"Can't wait for die, already pending");
                return false;
            }

            return WaitForDie(onDiceThrown);
        }

        bool WaitForDie(Action<int> onDiceThrown)
        {
            // Can't wait for die if we have none
            if (CheckGameEnd())
                return false;

            DieResultListener = (dieResult) =>
            {
                IsWaitingForDie = false;
                DieResultListener = null;

                DiceCount--;
                onDiceThrown?.Invoke(dieResult);
            };

            IsWaitingForDie = true;
            return true;
        }

        void PostPlayerMove(Vector2Int coord, Tile tile)
        {
            IsVisitingTile = true;
            tile.Visit(() =>
            {
                IsVisitingTile = false;
                if (CheckGameEnd())
                {
                    RestartGame();
                }
            });
        }

        void StartEnemyFight(int enemyBaseHp) { }

        bool CheckGameEnd()
        {
            if (DiceCount <= 0)
            {
                Debug.Log("Game over");
                return true;
            }

            return false;
        }

        void RestartGame()
        {
            StartGame();
        }
    }
}