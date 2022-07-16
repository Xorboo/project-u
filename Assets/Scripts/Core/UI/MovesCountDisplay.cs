using Core.Player;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class MovesCountDisplay : MonoBehaviour
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
                TrackingPlayer.OnMovesCountChanged -= MovesCountChanged;

            TrackingPlayer = player;
            TrackingPlayer.OnMovesCountChanged += MovesCountChanged;
            MovesCountChanged(TrackingPlayer.MovesLeft);
        }


        void MovesCountChanged(int count)
        {
            CountText.text = count.ToString();
        }
    }
}