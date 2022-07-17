using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers.MysticTriggers
{
    public class VillageTrigger : PlayerTriggerBase
    {
        [SerializeField]
        string InteractText;

        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            UiManager.Instance.ShowMysticInteractText(InteractText, MysticWindowClosed);

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