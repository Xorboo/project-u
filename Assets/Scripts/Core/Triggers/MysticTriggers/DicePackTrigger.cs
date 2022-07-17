using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers.MysticTriggers
{
    public class DicePackTrigger : PlayerTriggerBase
    {
        [SerializeField]
        string InteractText;

        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            UiManager.Instance.ShowMysticInteractText(InteractText, MysticWindowClosed);

            void MysticWindowClosed()
            {
                GameManager.Instance.AddPackOfDice();

                // TODO Animate
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}