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


        protected override bool IsPersistent => false;


        [SerializeField]
        PlayerController PlayerPrefab;

        [SerializeField]
        GameParameters Data;

        [SerializeField]
        GameObject FinalScreen;

        // TODO Save to PlayerPrefs
        public int ExtraStartingDice { get; private set; } = 0;

        public bool ReadyToBossSpawn = false;

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
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

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
            if (PlayerPrefs.HasKey("StartDices"))
                DiceCount = PlayerPrefs.GetInt("StartDices");
            else
                DiceCount = Data.StartingDice + ExtraStartingDice;
            IsWaitingForDie = false;
            DieResultListener = null;
            EnemyExtraHpFactor = 0;
            VillagesCount = 0;

            var spawnPoint = MapManager.Instance.InitializeMap();

            FinalScreen.SetActive(false);

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
                Debug.LogWarning($"Die thrown when not waiting for it");
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
            PlayerPrefs.SetInt("StartDices", Data.StartingDice + ExtraStartingDice);
            PlayerPrefs.Save();
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

        public bool CanClickOnTile(Vector2Int coord, Tile tile)
        {
            if (IsWaitingForDie || Player.CurrentState != PlayerController.State.Idle || !Player.IsAdjacent(coord))
                return false;

            if (tile == null)
            {
                Debug.LogError($"Null tile on {coord}");
                return false;
            }

            if (!tile.Data.IsPassable)
                return false;

            return true;
        }

        void TileClicked(Vector2Int coord, Tile tile)
        {
            if (!CanClickOnTile(coord, tile))
                return;

            if (!tile.Data.IsMystic || tile.MysticRevealed)
            {
                MovePlayer(coord, tile); // Move to normal tile
                return;
            }

            // Special case
            CheckMysticTile(coord, tile);
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
            UiManager.Instance.ShowGameOverPanel(StartGame);
        }

        public void PlayFinal()
        {
            FinalScreen.SetActive(true);
        }
    }
}