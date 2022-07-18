using System;
using Core.Effects;
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
        Canvas OverheadCanvas;

        [SerializeField]
        TMP_Text HpText;

        [SerializeField]
        TMP_Text NameText;

        public int MaxHealth => Data.Hp + Data.HpScaleFactor * GameManager.Instance.EnemyExtraHpFactor;
        int CurrentHealth = -1;

        public bool IsInFight { get; private set; }

        [SerializeField]
        float DeathDelay = 0.3f;

        [SerializeField]
        float DeathSinkDelay = 0.7f;

        [SerializeField]
        Animator Animator;

        [SerializeField]
        EnemyAnimationListener AnimationListener;

        [SerializeField]
        HitSpawner HitSpawner;

        [SerializeField]
        LookRotation LookRotation;


        Action<bool> FightEndListener;


        #region Unity

        void Awake() { }

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
                bool isBoss = gameObject.CompareTag("Boss");

                AnimationListener.SetDeathCompletedListener(() =>
                {
                    float sinkDuration = 2f;
                    Vector3 sinkScale = new Vector3(1.7f, 0f, 1.7f);
                    Vector3 sinkShift = new Vector3(0, -0.3f, 0f);

                    DOTween.Sequence()
                        .AppendInterval(DeathDelay)
                        .AppendCallback(() =>
                        {
                            OverheadCanvas.gameObject.SetActive(false);

                            if (isBoss)
                                GameManager.Instance.PlayFinal();

                            onCompleted?.Invoke();
                        })
                        .AppendInterval(DeathSinkDelay)
                        .Append(transform.DOLocalMove(transform.localPosition + sinkShift, sinkDuration))
                        .Join(transform.DOScale(sinkScale, sinkDuration))
                        .OnComplete(() =>
                        {
                            if (gameObject)
                                Destroy(gameObject);
                        });
                });

                Animator.SetTrigger("Die");
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
            LookRotation.RotateTo(player.transform);

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
                player.AnimateAttack(this, () => ApplyPlayerAttack(dieResult), FinishPlayerAttack);
            }

            void ApplyPlayerAttack(int dieResult)
            {
                CurrentHealth = Math.Max(0, CurrentHealth - dieResult);
                UpdateHealthUi();

                HitSpawner.SpawnHit(transform);

                if (CurrentHealth > 0)
                {
                    Animator.SetTrigger("GetHit");
                }
                else
                {
                    int loot = Data.RandomLoot;
                    player.GetLoot(loot);
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
                AnimationListener.SetAttackListener(ApplyEnemyAttack, FinishEnemyAttack);
                Animator.SetTrigger("Attack");
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