using System;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class MysticInteractUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform RootPanel;

        [SerializeField]
        TMP_Text InteractText;

        Action CloseListener = null;

        public void OpenPanel(string interactText, Action onPanelClosed)
        {
            CloseListener = onPanelClosed;

            RootPanel.gameObject.SetActive(true);
            InteractText.text = interactText;
        }

        public void ClosePanel()
        {
            RootPanel.gameObject.SetActive(false);

            CloseListener?.Invoke();
            CloseListener = null;
        }
    }
}