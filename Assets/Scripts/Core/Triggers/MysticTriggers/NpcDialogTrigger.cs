using System;
using Core.Player;
using Core.Story;

namespace Core.Triggers.MysticTriggers
{
    public class NpcDialogTrigger : PlayerTriggerBase
    {
        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
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