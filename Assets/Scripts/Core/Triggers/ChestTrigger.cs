using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers
{
    public class ChestTrigger : PlayerTriggerBase
    {
        [SerializeField]
        int DiceAmount = 5;

        #region Unity

        #endregion


        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            UiManager.Instance.ShowChestEntry(DiceAmount, ChestUiClosed);

            void ChestUiClosed()
            {
                GameManager.Instance.ChangeDice(DiceAmount);
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}