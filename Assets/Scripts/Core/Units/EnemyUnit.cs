using System;
using Core.Player;
using Core.Triggers;
using Core.UI;
using TMPro;
using UnityEngine;

namespace Core.Units
{
    public class EnemyUnit : PlayerTriggerBase
    {
        [SerializeField]
        UnitData Data;

        [SerializeField]
        TMP_Text HpText;

        [SerializeField]
        TMP_Text NameText;

        public int MaxHealth => Data.Hp + Data.HpScaleFactor * GameManager.Instance.EnemyExtraHpFactor;
        int CurrentHealth = -1;

        public bool IsInFight { get; private set; }

        Action<bool> FightEndListener;


        #region Unity

        void OnEnable()
        {
            NameText.text = Data.UnitName;
            UpdateHealthUi();

            GameManager.Instance.OnEnemyHpIncreased += EnemyHpIncreased;
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnEnemyHpIncreased += EnemyHpIncreased;
        }

        #endregion


        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            StartFight(player, FightFinished);

            void FightFinished(bool playerWon)
            {
                if (playerWon)
                {
                    // TODO Death animation (but disable trigger immediately)
                    // transform.Translate(0, 1, 0);
                    // transform.Rotate(0, 0, 180, Space.Self);

                    Destroy(gameObject);
                    onCompleted?.Invoke();
                }
                else
                {
                    // TODO Lose (maybe do nothing, player will restart)
                }
            }
        }

        void StartFight(PlayerController player, Action<bool> onFightFinished)
        {
            if (IsInFight)
            {
                Debug.LogError($"Already in fight", gameObject);
                return;
            }

            CurrentHealth = MaxHealth;
            IsInFight = true;

            RequestPlayerAttack();

            #region Fight Logic

            void RequestPlayerAttack()
            {
                if (!GameManager.Instance.WaitForDieThrowResult(PerformPlayerAttack))
                {
                    onFightFinished?.Invoke(false);
                    GameManager.Instance.RestartGame();
                    return;
                }

                UiManager.Instance.ShowAttackThrow();
            }

            void PerformPlayerAttack(int dieResult)
            {
                UiManager.Instance.HideDieRequest();

                // TODO Animate attack
                CurrentHealth = Math.Max(0, CurrentHealth - dieResult);
                UpdateHealthUi();

                if (CurrentHealth <= 0)
                {
                    onFightFinished?.Invoke(true);
                }
                else
                {
                    PerformEnemyAttack();
                }
            }

            void PerformEnemyAttack()
            {
                // TODO Animate attack
                int damage = Data.RandomDamage;
                player.Health.ReceiveDamage(damage);
                if (!player.Health.IsAlive)
                {
                    // Fight lost
                    onFightFinished?.Invoke(false);
                    return;
                }

                RequestPlayerAttack();
            }

            #endregion
        }


        void EnemyHpIncreased()
        {
            UpdateHealthUi();
        }

        void UpdateHealthUi()
        {
            int realHealth = IsInFight ? CurrentHealth : MaxHealth;
            HpText.text = $"{realHealth} / {MaxHealth}";
        }
    }
}