using System;
using Core.Player;
using Core.UI;
using UnityEngine;
using UrUtils.Misc;

namespace Core.Story
{
    public class StoryManager : Singleton<StoryManager>
    {
        [SerializeField]
        StoryData StoryData;

        [SerializeField]
        bool ResetStoryOnRestart = true;


        int CurrentStoryIndex = 0;


        #region Unity

        void OnEnable()
        {
            GameManager.Instance.OnPlayerSpawned += GameStarted;
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnPlayerSpawned -= GameStarted;
        }

        #endregion


        void GameStarted(PlayerController obj)
        {
            if (ResetStoryOnRestart)
                CurrentStoryIndex = 0;
        }

        public void ShowNpcStoryEntry(Action onStoryClosed)
        {
            var story = StoryData.Entries[CurrentStoryIndex];

            UiManager.Instance.ShowStoryEntry(story, onStoryClosed);
            CurrentStoryIndex = Math.Min(CurrentStoryIndex + 1, StoryData.Entries.Count - 1);
        }
    }
}