using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Story
{
    [CreateAssetMenu]
    public class StoryData : ScriptableObject
    {
        public List<StoryEntry> Entries = new();
    }

    [Serializable]
    public class StoryEntry
    {
        public StoryNpcData Npc;
        public string Text;
        public bool isFinal = false;
    }
}