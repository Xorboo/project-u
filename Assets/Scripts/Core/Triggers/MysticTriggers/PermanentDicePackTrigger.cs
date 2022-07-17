using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers.MysticTriggers
{
    public class PermanentDicePackTrigger : PlayerTriggerBase
    {
        [SerializeField]
        string InteractText;

        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            UiManager.Instance.ShowMysticInteractText(InteractText, MysticWindowClosed);

            void MysticWindowClosed()
            {
                GameManager.Instance.AddPermanentDice();

                // TODO Animate
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}