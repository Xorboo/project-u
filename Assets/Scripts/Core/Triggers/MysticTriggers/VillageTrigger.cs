using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers.MysticTriggers
{
    public class VillageTrigger : ImmediatePlayerTriggerBase
    {
        [SerializeField]
        string RevealText;

        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            onCompleted?.Invoke();
        }

        public override void ImmediateProcessTrigger(Action onCompleted)
        {
            UiManager.Instance.ShowMysticInteractText(RevealText, MysticWindowClosed);

            void MysticWindowClosed()
            {
                GameManager.Instance.AddVillage();

                // Not destroying village
                //Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}