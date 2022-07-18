using System;
using Core.Effects;
using TMPro;
using UnityEngine;

namespace Core.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        GameParameters GameParameters;

        [SerializeField]
        TMP_Text HealthText;

        [SerializeField]
        HitSpawner HitSpawner;


        PlayerController Player;


        public int Health
        {
            get => HealthValue;
            private set
            {
                HealthValue = value;
                UpdateHealthUi();
            }
        }

        int HealthValue = 0;


        public int MaxHealth
        {
            get => MaxHealthValue;
            private set
            {
                MaxHealthValue = value;
                UpdateHealthUi();
            }
        }

        int MaxHealthValue = 0;

        static int GatheredBonusHealth = 0;


        #region Unity

        void Awake()
        {
            Player = GetComponent<PlayerController>();
        }

        #endregion

        public void Initialize()
        {
            if (PlayerPrefs.HasKey("MaxHealth"))
                MaxHealth = Health = PlayerPrefs.GetInt("MaxHealth");
            else
                MaxHealth = Health = GameParameters.PlayerHealth + GatheredBonusHealth;
            UpdateHealthUi();
        }

        public bool IsAlive => Health > 0;

        public void ApplyHeal(float hpPercentage)
        {
            Health = Mathf.Min(Health + Mathf.CeilToInt(MaxHealth * hpPercentage / 100), MaxHealth);
        }

        public void AddMaxHealth(int amount)
        {
            GatheredBonusHealth += amount;
            MaxHealth += amount;
            Health += amount;
            PlayerPrefs.SetInt("MaxHealth", MaxHealth);
            PlayerPrefs.Save();
        }

        public void ReceiveDamage(int amount)
        {
            Health = Math.Max(0, Health - amount);

            HitSpawner.SpawnHit(transform);

            if (IsAlive)
            {
                Player.Animator.SetTrigger("GetHit");
            }
            else
            {
                Player.Animator.SetTrigger("Die");
                GameManager.Instance.RestartGame();
            }
        }

        void UpdateHealthUi()
        {
            HealthText.text = $"{Health} / {MaxHealth}";
        }
    }
}