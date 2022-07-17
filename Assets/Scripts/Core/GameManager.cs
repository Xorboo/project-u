using System;
using Core.Map;
using Core.Player;
using Core.UI;
using UnityEngine;
using UrUtils.Misc;
using Tile = Core.Map.Tile;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        public event Action<PlayerController> OnPlayerSpawned = delegate { };
        public event Action<int> OnDiceCountChanged = delegate { };
        public event Action<bool> OnDieWaitingChanged = delegate { };
        public event Action OnEnemyHpIncreased = delegate { };


        [SerializeField]
        PlayerController PlayerPrefab;

        [SerializeField]
        GameParameters Data;

        // TODO Save to PlayerPrefs
        public int ExtraStartingDice { get; private set; } = 0;


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

        public int EnemyExtraHpFactor { get; private set; } = 0;

        public int VillagesCount { get; private set; } = 0;


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


        void StartGame()
        {
            DiceCount = Data.StartingDice + ExtraStartingDice;
            IsWaitingForDie = false;
            DieResultListener = null;
            EnemyExtraHpFactor = 0;
            VillagesCount = 0;

            var spawnPoint = MapManager.Instance.InitializeMap();

            if (Player)
                Destroy(Player.gameObject);

            Player = Instantiate(PlayerPrefab);
            Player.SpawnPlayer(spawnPoint);

            OnPlayerSpawned(Player);
        }


        public void OnDieThrown(int dieResult)
        {
            if (!IsWaitingForDie)
            {
                Debug.LogError($"Die thrown when not waiting for it");
                return;
            }

            DieResultListener?.Invoke(dieResult);
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

        public void AddPermanentDice()
        {
            Debug.Log($"Adding permanent pack of {Data.MysticPermanentDiceCount} dice");
            ExtraStartingDice += Data.MysticPermanentDiceCount;
        }

        public void AddPackOfDice()
        {
            Debug.Log($"Adding pack of {Data.MysticDicePackCount} dice");
            ChangeDice(Data.MysticDicePackCount);
        }

        public void ChangeDice(int diceChange)
        {
            Debug.Log($"Adding {diceChange} dice");
            DiceCount = Mathf.Max(0, DiceCount + diceChange);
        }

        public void AddVillage()
        {
            Debug.Log("Adding new found village");
            VillagesCount++;
        }

        public void IncreaseEnemyHp()
        {
            // TODO Add to existing enemies
            Debug.Log("Enemy hp increased");
            EnemyExtraHpFactor++;
            OnEnemyHpIncreased();
        }

        void TileClicked(Vector2Int coord, Tile tile)
        {
            if (IsWaitingForDie || Player.CurrentState != PlayerController.State.Idle || !Player.IsAdjacent(coord))
                return;

            if (tile == null)
            {
                Debug.LogError($"Null tile clicked on {coord}");
                return;
            }

            if (!tile.Data.IsPassable)
                return; // Do nothing, wasted die

            if (!tile.Data.IsMystic || tile.MysticRevealed)
            {
                MovePlayer(coord, tile); // Move to normal tile
                return;
            }

            // Special case
            CheckMysticTile(coord, tile);
            return;
        }

        void CheckMysticTile(Vector2Int coord, Tile tile)
        {
            // Reveal the tile via die throw
            if (!WaitForDie((value) => MysticDiceThrown(coord, tile, value)))
            {
                RestartGame();
                return;
            }

            UiManager.Instance.ShowMysticRevealThrow();

            void MysticDiceThrown(Vector2Int pos, Tile mysticTile, int dieResult)
            {
                UiManager.Instance.HideDieRequest();
                if (!mysticTile.Data.IsMystic)
                {
                    Debug.LogError($"Mystic die on normal tile");
                    MovePlayer(pos, mysticTile);
                    return;
                }

                // Create a tile and then move the player there
                mysticTile.RevealMysticTile(dieResult, null);
            }
        }

        void MovePlayer(Vector2Int coord, Tile tile)
        {
            Player.MoveToTile(coord, tile, CheckPlayerMoves);

            void CheckPlayerMoves()
            {
                if (!PlayerCanMove)
                {
                    RestartGame();
                    return;
                }

                Player.CheckMovesDie();
            }
        }

        bool WaitForDie(Action<int> onDiceThrown)
        {
            // Can't wait for die if we have none
            if (!PlayerCanThrowDie)
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

        void StartEnemyFight(int enemyBaseHp) { }

        public bool PlayerCanMove => DiceCount > 0 || Player.MovesLeft > 0;
        public bool PlayerCanThrowDie => DiceCount > 0;

        public void RestartGame()
        {
            // TODO Replace with a UI
            StartGame();
        }
    }
}