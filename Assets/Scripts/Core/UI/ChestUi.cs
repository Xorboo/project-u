using System;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class ChestUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform ChestPanel;

        [SerializeField]
        TMP_Text ChestAmountText;

        [SerializeField]
        string DiceAmountString = "Found dice:";

        Action ChestCloseListener = null;

        public void OpenChestInfo(int diceAmount, Action onChestUiClosed)
        {
            ChestCloseListener = onChestUiClosed;

            ChestPanel.gameObject.SetActive(true);
            ChestAmountText.text = $"{DiceAmountString} {diceAmount}";
        }

        public void CloseChestInfo()
        {
            ChestPanel.gameObject.SetActive(false);

            ChestCloseListener?.Invoke();
            ChestCloseListener = null;
        }
    }
}