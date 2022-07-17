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

        [SerializeField]
        string CoinsAmountString = "Found coins:";

        Action ChestCloseListener = null;

        public void OpenChestInfo(int diceAmount, Action onChestUiClosed, bool dice)
        {
            ChestCloseListener = onChestUiClosed;

            ChestPanel.gameObject.SetActive(true);

            if (dice)
                ChestAmountText.text = $"{DiceAmountString} {diceAmount}";
            else
                ChestAmountText.text = $"{CoinsAmountString} {diceAmount}";
        }

        public void CloseChestInfo()
        {
            ChestPanel.gameObject.SetActive(false);

            ChestCloseListener?.Invoke();
            ChestCloseListener = null;
        }
    }
}