using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers
{
    public class MoneyTrigger : PlayerTriggerBase
    {
        [SerializeField]
        int DiceAmount = 5;

        #region Unity

        #endregion


        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            UiManager.Instance.ShowChestEntry(DiceAmount, ChestUiClosed, false);

            void ChestUiClosed()
            {
                GameManager.Instance.Player.GetLoot(DiceAmount);
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}