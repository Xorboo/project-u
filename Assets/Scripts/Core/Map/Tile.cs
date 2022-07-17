﻿using System;
using UnityEngine;

namespace Core.Map
{
    [RequireComponent(typeof(TileRevealController))]
    public class Tile : MonoBehaviour
    {
        public static event Action<Tile> OnFightStarted = delegate { };
        public static event Action<Tile> OnFightFinished = delegate { };

        public event Action<int> OnEnemyHpChanged = delegate { };


        public TileData Data;

        Action VisitFinishedCallback;
        public int CurrentEnemyHp { get; private set; } = 0;

        public bool IsVisited { get; private set; } = false;

        public int RevealedMysticType { get; private set; } = -1;


        TileRevealController RevealController;


        #region Unity

        void Awake()
        {
            RevealController = GetComponent<TileRevealController>();
        }

        #endregion


        public bool IsRevealed => RevealController.IsRevealed;

        public void Reveal(float revealDelay)
        {
            RevealController.Reveal(revealDelay);
        }

        public void RevealImmediately()
        {
            RevealController.RevealImmediately();
        }

        public void Visit(Action onVisitFinished)
        {
            if (IsVisited)
            {
                onVisitFinished?.Invoke();
                return;
            }

            IsVisited = true;
            VisitFinishedCallback = onVisitFinished;

            if (Data.HasEnemy)
            {
                StartEnemyFight(Data.EnemyFullHp);
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

        #region Mystic Tile

        public void RevealMysticTile(int dieResult, Action onFinished)
        {
            RevealedMysticType = dieResult;
            GetComponent<MysticTile>().RevealTile(dieResult, onFinished);
        }

        #endregion
    }
}