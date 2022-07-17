using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers
{
    public class BonusHealthPotionTrigger : PlayerTriggerBase
    {
        [SerializeField]
        int ExtraHealth = 5;

        #region Unity

        #endregion


        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            UiManager.Instance.ShowBonusHealthEntry(ExtraHealth, PotionUiClosed);

            void PotionUiClosed()
            {
                GameManager.Instance.Player.Health.AddMaxHealth(ExtraHealth);
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}