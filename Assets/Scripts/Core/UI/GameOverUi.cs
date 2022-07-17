using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            SceneManager.LoadScene("game");  //Ярослав: делаем перезагрузку по хардкору, чтобы не разбираться что мы откатили, а что забыли
            //RestartListener?.Invoke();
            //RestartListener = null;
        }

        public void ClosePanel()
        {
            GameOverPanel.gameObject.SetActive(false);
        }
    }
}