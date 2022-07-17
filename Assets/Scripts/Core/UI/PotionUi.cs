using System;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class PotionUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform PotionPanel;

        [SerializeField]
        TMP_Text PotionText;

        [SerializeField]
        string PotionString = "You drank something weird and suddenly felt better";

        [SerializeField]
        string BonusPotionString = "You drank something weird and now feel younger!";

        Action PanelCloseListener = null;

        public void OpenHealthPotionPanel(float healPercent, Action onPanelClosed)
        {
            ShowPanel(PotionString, onPanelClosed);
        }

        public void OpenBonusHealthPotionPanel(int extraHealth, Action onPanelClosed)
        {
            ShowPanel(BonusPotionString, onPanelClosed);
        }

        public void ClosePotionPanel()
        {
            PotionPanel.gameObject.SetActive(false);

            PanelCloseListener?.Invoke();
            PanelCloseListener = null;
        }

        void ShowPanel(string text, Action onPanelClosed)
        {
            PanelCloseListener = onPanelClosed;

            PotionPanel.gameObject.SetActive(true);
            PotionText.text = text;
        }
    }
}