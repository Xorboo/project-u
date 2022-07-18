using System;
using Core.Player;
using Core.Story;
using Core.Units;
using UnityEngine;

namespace Core.Triggers.MysticTriggers
{
    public class NpcDialogTrigger : PlayerTriggerBase
    {
        [SerializeField]
        LookRotation LookRotation;


        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            LookRotation.RotateTo(player.transform);
            StoryManager.Instance.ShowNpcStoryEntry(StoryDialogClosed);

            void StoryDialogClosed()
            {
                // TODO Animate
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }
    }
}