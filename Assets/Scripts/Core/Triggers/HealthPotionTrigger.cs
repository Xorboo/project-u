using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers
{
    public class HealthPotionTrigger : PlayerTriggerBase
    {
        [SerializeField, Range(0, 100)]
        float HealPercent = 50f;

        #region Unity

        #endregion


        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            UiManager.Instance.ShowHealthEntry(HealPercent, PotionUiClosed);

            void PotionUiClosed()
            {
                GameManager.Instance.Player.Health.ApplyHeal(HealPercent);
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}