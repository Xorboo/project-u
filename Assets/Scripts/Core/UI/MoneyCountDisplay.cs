using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Core.Player;

namespace Core.UI
{
    public class MoneyCountDisplay : MonoBehaviour
    {
        [SerializeField]
        TMP_Text CountText;

        PlayerController TrackingPlayer = null;

        #region Unity

        void OnEnable()
        {
            GameManager.Instance.OnPlayerSpawned += PlayerSpawned;
            PlayerSpawned(GameManager.Instance.Player);
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnPlayerSpawned -= PlayerSpawned;
        }

        #endregion

        void PlayerSpawned(PlayerController player)
        {
            if (TrackingPlayer != null)
                TrackingPlayer.OnMoneyCountChanged -= MoneyCountChanged;

            TrackingPlayer = player;
            TrackingPlayer.OnMoneyCountChanged += MoneyCountChanged;
            MoneyCountChanged(TrackingPlayer.MoneyLeft);
        }


        void MoneyCountChanged(int count)
        {
            CountText.text = count.ToString();
        }
    }
}