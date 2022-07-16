using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [System.Serializable]
    class Dialogs
    {
        public string npc_name;
        public string npc_text;
    }

    [SerializeField] Dialogs[] dialogs;
    int current_dialog = 0;

    [SerializeField] DialogUi _ui;

    void Start()
    {
        
    }

    public void StoryNPC()
    {
        _ui.OpenDialog(dialogs[current_dialog].npc_name, dialogs[current_dialog].npc_text);
        
        if (current_dialog < dialogs.Length - 1)
            current_dialog++;
    }
}
