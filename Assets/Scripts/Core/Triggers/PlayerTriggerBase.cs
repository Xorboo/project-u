using System;
using Core.Player;
using UnityEngine;

namespace Core.Triggers
{
    public abstract class PlayerTriggerBase : MonoBehaviour
    {
        PlayerController TriggeredPlayer = null;


        #region Unity

        void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (!player)
            {
                Debug.LogWarning($"[{gameObject.name}] should only collide with player, and not with [{other.gameObject.name}]",
                    gameObject);
                return;
            }

            TriggeredPlayer = player;
            TriggeredPlayer.TriggerStarted();
            ProcessTrigger(TriggeredPlayer, TriggerCompleted);
        }

        #endregion


        protected abstract void ProcessTrigger(PlayerController player, Action onCompleted);


        void TriggerCompleted()
        {
            if (!TriggeredPlayer)
            {
                Debug.LogWarning($"Trigger ended with no player around", gameObject);
                return;
            }

            TriggeredPlayer.TriggerFinished();
        }
    }
}