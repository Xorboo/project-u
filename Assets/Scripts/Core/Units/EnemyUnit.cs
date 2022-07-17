using System;
using Core.Player;
using Core.Triggers;
using Core.UI;
using DG.Tweening;
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
                    AnimateEnemyDeath();
                }
                else
                {
                    // TODO Lose (maybe do nothing, player will restart)
                }
            }

            void AnimateEnemyDeath()
            {
                float fallDuration = 0.4f;
                float sinkDuration = 2f;
                DOTween.Sequence(gameObject)
                    .Append(transform.DOLocalMove(transform.localPosition + new Vector3(0f, 0.3f, 0f), fallDuration))
                    .Join(transform.DOLocalRotate(new Vector3(90, 0, 0), fallDuration))
                    .Append(transform.DOLocalMove(transform.localPosition + new Vector3(0, -0.3f, 0f), sinkDuration))
                    .Join(transform.DOScale(new Vector3(1.7f, 1.7f, 0f), sinkDuration))
                    .OnComplete(() =>
                    {
                        Destroy(gameObject);
                        onCompleted?.Invoke();
                    });
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
                player.AnimateAttack(this, () => ApplyPlayerAttack(dieResult), FinishPlayerAttack);
            }

            void ApplyPlayerAttack(int dieResult)
            {
                CurrentHealth = Math.Max(0, CurrentHealth - dieResult);
                UpdateHealthUi();

                if (CurrentHealth <= 0)
                {
                    onFightFinished?.Invoke(true);
                }
            }

            void FinishPlayerAttack()
            {
                if (CurrentHealth > 0)
                {
                    PerformEnemyAttack();
                }
            }

            void PerformEnemyAttack()
            {
                Vector3 playerDir = (player.transform.position - transform.position).normalized;
                playerDir.y = 0f;

                float shiftDistance = 0.7f;
                float shiftDuration = 0.4f;
                float shiftBackDuration = 0.7f;
                Vector3 originalPos = transform.localPosition;
                Vector3 attackPos = originalPos + playerDir * shiftDistance;
                DOTween.Sequence(gameObject)
                    .Append(transform.DOLocalMove(attackPos, shiftDuration).SetEase(Ease.OutElastic))
                    .AppendCallback(ApplyEnemyAttack)
                    .Append(transform.DOLocalMove(originalPos, shiftBackDuration).SetEase(Ease.InOutSine))
                    .OnComplete(FinishEnemyAttack);
            }

            void ApplyEnemyAttack()
            {
                int damage = Data.RandomDamage;
                player.Health.ReceiveDamage(damage);

                if (!player.Health.IsAlive)
                {
                    // Fight lost
                    onFightFinished?.Invoke(false);
                    return;
                }
            }

            void FinishEnemyAttack()
            {
                if (!player.Health.IsAlive)
                {
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