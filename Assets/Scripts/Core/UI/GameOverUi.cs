using System;
using UnityEngine;

namespace Core.UI
{
    public class GameOverUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform GameOverPanel;


        Action RestartListener = null;


        public void OpenGameOverPanel(Action onRestartPressed)
        {
            RestartListener = onRestartPressed;

            GameOverPanel.gameObject.SetActive(true);
        }

        public void RestartPressed()
        {
            ClosePanel();

            RestartListener?.Invoke();
            RestartListener = null;
        }

        public void ClosePanel()
        {
            GameOverPanel.gameObject.SetActive(false);
        }
    }
}