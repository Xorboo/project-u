using System;
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
        }

        public void ReceiveDamage(int amount)
        {
            Health = Math.Max(0, Health - amount);

            if (!IsAlive)
            {
                // TODO Die animation
                GameManager.Instance.RestartGame();
            }
        }

        void UpdateHealthUi()
        {
            HealthText.text = $"{Health} / {MaxHealth}";
        }
    }
}