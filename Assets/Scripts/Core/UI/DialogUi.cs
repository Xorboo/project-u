using System;
using Core.Story;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class DialogUi : MonoBehaviour
    {
        [SerializeField]
        GameObject DialogPanel;

        [SerializeField]
        TMP_Text NpcNameText, DialogText;

        Action DialogCloseListener = null;


        void Start()
        {
            CloseDialog();
        }

        public void OpenDialog(StoryEntry entry, Action onDialogClosed)
        {
            DialogCloseListener = onDialogClosed;

            DialogPanel.SetActive(true);
            NpcNameText.text = entry.Npc.NpcName;
            DialogText.text = entry.Text;
        }

        public void CloseDialog()
        {
            DialogPanel.SetActive(false);

            DialogCloseListener?.Invoke();
            DialogCloseListener = null;
        }
    }
}