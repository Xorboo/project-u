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


        #region Unity

        #endregion


        public void Visit(Action onVisitFinished)
        {
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

            VisitFinishedCallback?.Invoke();
        }

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
    }
}