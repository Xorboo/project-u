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
            TriggeredPlayer = other.GetComponent<PlayerController>();
            if (!TriggeredPlayer)
            {
                Debug.LogError($"[{gameObject.name}] should only collide with player");
                return;
            }

            TriggeredPlayer.TriggerStarted();
            ProcessTrigger(TriggeredPlayer, TriggerCompleted);
        }

        #endregion


        protected abstract void ProcessTrigger(PlayerController player, Action onCompleted);


        void TriggerCompleted()
        {
            if (!TriggeredPlayer)
            {
                Debug.LogWarning($"Trigger ended with no player around");
                return;
            }

            TriggeredPlayer.TriggerFinished();
        }
    }
}