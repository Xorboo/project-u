using System;
using Core.Map;
using Core.Player;
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

        public bool IsVisitingTile { get; private set; } = false;

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
            Player.SetSpawnPoint(spawnPoint);

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
            AddDice(Data.MysticDicePackCount);
        }

        public void AddDice(int diceCount)
        {
            Debug.Log($"Adding {diceCount} dice");
            DiceCount += diceCount;
        }

        public void ResolveRandomEvent(int dieResult)
        {
            switch (dieResult)
            {
                case 1:
                    MultiplyCurrentCubes(false);
                    break;
                case 2:
                    AddVillageCubes(Data.VillageBonusA);
                    break;
                case 3:
                    AddVillageCubes(Data.VillageBonusB);
                    break;
                case 4:
                    ShowRandomText();
                    break;
                case 5:
                    IncreaseEnemyHp();
                    break;
                case 6:
                    MultiplyCurrentCubes(true);
                    break;
                default:
                    Debug.LogError($"Unsupported die result: {dieResult}");
                    break;
            }
        }

        public void AddVillage()
        {
            Debug.Log("Adding new found village");
            VillagesCount++;
        }

        void MultiplyCurrentCubes(bool increase)
        {
            int delta = Mathf.CeilToInt(DiceCount * Data.DiceMultiplyFactor);
            Debug.Log($"Multiplying dice, delta: {delta}, increase: {increase}");
            DiceCount += (increase ? 1 : -1) * delta;
        }

        void AddVillageCubes(int cubesPerVillage)
        {
            Debug.Log($"Adding {cubesPerVillage} cubes for {VillagesCount} villages");
            AddDice(VillagesCount * cubesPerVillage);
        }

        void ShowRandomText()
        {
            Debug.LogWarning($"Random text not implemented");
        }

        void IncreaseEnemyHp()
        {
            Debug.Log("Enemy hp increased");
            EnemyExtraHpFactor++;
        }

        void TileClicked(Vector2Int coord, Tile tile)
        {
            if (IsVisitingTile || IsWaitingForDie || Player.CurrentState != PlayerController.State.Idle || !Player.IsAdjacent(coord))
                return;

            if (tile != null)
            {
                if (!tile.Data.IsPassable)
                    return; // Do nothing, wasted die

                if (!tile.Data.IsMystic)
                {
                    Player.MoveToTile(coord, tile); // Move to normal tile
                    return;
                }

                // Special case
                CheckMysticTile(coord, tile);
                return;
            }

            if (!WaitForDie((value) => RevealDiceThrown(coord, value)))
            {
                RestartGame();
                return;
            }
        }

        void CheckMysticTile(Vector2Int coord, Tile tile)
        {
            // Already opened mystic tile, just move to it
            if (tile.IsVisited)
            {
                Player.MoveToTile(coord, tile);
                return;
            }

            if (!WaitForDie((value) => MysticDiceThrown(coord, tile, value)))
            {
                RestartGame();
                return;
            }
        }

        void MysticDiceThrown(Vector2Int coord, Tile tile, int dieResult)
        {
            if (!tile.Data.IsMystic)
            {
                Debug.LogError($"Mystic die on normal tile");
                Player.MoveToTile(coord, tile, () => PostPlayerMove(coord, tile));
                return;
            }

            // Create a tile and then move the player there
            tile.RevealMysticTile(dieResult, () => Player.MoveToTile(coord, tile, () => PostPlayerMove(coord, tile)));
        }

        void RevealDiceThrown(Vector2Int coord, int dieResult)
        {
            var tile = MapManager.Instance.RevealTile(coord, dieResult);

            if (tile.Data.IsPassable)
                Player.MoveToTile(coord, tile, () => PostPlayerMove(coord, tile));
            else
            {
                if (CheckGameEnd())
                {
                    RestartGame();
                }
            }
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