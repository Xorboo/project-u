using System;
using UnityEngine;

namespace Core.Map
{
    public class Tile : MonoBehaviour
    {
        public static event Action<Tile> OnFightStarted = delegate { };
        public static event Action<Tile> OnFightFinished = delegate { };

        public static event Action<Tile> OnChestOpenStarted = delegate { };
        public static event Action<Tile> OnChestOpenFinished = delegate { };

        public static event Action<Tile> OnRandomEventStarted = delegate { };
        public static event Action<Tile> OnRandomEventFinished = delegate { };

        public event Action<int> OnEnemyHpChanged = delegate { };


        public TileData Data;

        Action VisitFinishedCallback;
        public int CurrentEnemyHp { get; private set; } = 0;

        public bool IsVisited { get; private set; } = false;

        public int RevealedMysticType { get; private set; } = -1;


        #region Unity

        #endregion


        public void Visit(Action onVisitFinished)
        {
            IsVisited = true;
            VisitFinishedCallback = onVisitFinished;

            if (Data.HasEnemy)
            {
                StartEnemyFight(Data.EnemyFullHp);
                return;
            }

            if (Data.HasChest)
            {
                StartChestOpen();
                return;
            }

            if (Data.HasRandomEvent)
            {
                StartRandomEvent();
                return;
            }

            if (Data.IsMystic)
            {
                StartMysticEvent();
                return;
            }

            VisitFinishedCallback?.Invoke();
        }

        #region Fight

        void StartEnemyFight(int enemyHp)
        {
            CurrentEnemyHp = enemyHp;
            OnFightStarted(this);

            DealDamage(0); // ugh, too laze to fix that
        }

        void DealDamage(int dieResult)
        {
            CurrentEnemyHp -= dieResult;
            OnEnemyHpChanged(CurrentEnemyHp);

            if (CurrentEnemyHp <= 0)
            {
                // Win fight
                OnFightFinished(this);
                VisitFinishedCallback?.Invoke();
                return;
            }

            // Throw a die
            bool isFetchingDie = GameManager.Instance.WaitForDieThrowResult(DealDamage);
            if (!isFetchingDie)
            {
                // Lose fight
                OnFightFinished(this);
                VisitFinishedCallback?.Invoke();
                return;
            }
        }

        #endregion

        #region Chest

        void StartChestOpen()
        {
            OnChestOpenStarted(this);

            // Throw a die
            bool isFetchingDie = GameManager.Instance.WaitForDieThrowResult(OpenChest);
            if (!isFetchingDie)
            {
                // Lose fight
                OnChestOpenFinished(this);
                VisitFinishedCallback?.Invoke();
            }
        }

        void OpenChest(int dieResult)
        {
            GameManager.Instance.AddDice(dieResult);
            OnChestOpenFinished(this);
            VisitFinishedCallback?.Invoke();
        }

        #endregion

        #region Random Event

        void StartRandomEvent()
        {
            OnRandomEventStarted(this);

            // Throw a die
            bool isFetchingDie = GameManager.Instance.WaitForDieThrowResult(ResolveRandomEvent);
            if (!isFetchingDie)
            {
                // Lose fight
                OnRandomEventFinished(this);
                VisitFinishedCallback?.Invoke();
            }
        }

        void ResolveRandomEvent(int dieResult)
        {
            GameManager.Instance.ResolveRandomEvent(dieResult);
            OnRandomEventFinished(this);
            VisitFinishedCallback?.Invoke();
        }

        #endregion

        #region Mystic Tile

        public void RevealMysticTile(int dieResult, Action onFinished)
        {
            RevealedMysticType = dieResult;
            GetComponent<MysticTile>().RevealTile(dieResult, onFinished);
        }

        void StartMysticEvent()
        {
            switch (RevealedMysticType)
            {
                case 1:
                    GameManager.Instance.AddPackOfDice();
                    VisitFinishedCallback?.Invoke();
                    break;

                case 2:
                    GameManager.Instance.AddPermanentDice();
                    VisitFinishedCallback?.Invoke();
                    break;

                // Village
                case 3:
                case 4:
                    GameManager.Instance.AddVillage();
                    VisitFinishedCallback?.Invoke();
                    break;

                // Story
                case 5:
                case 6:
                    Debug.LogWarning($"Story not implemented");
                    VisitFinishedCallback?.Invoke();
                    break;

                default:
                    Debug.LogError($"Unsupported mystic type: {RevealedMysticType}");
                    VisitFinishedCallback?.Invoke();
                    break;
            }
        }

        #endregion
    }
}