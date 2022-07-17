using Core.Player;
using UnityEngine;

namespace Core.UI
{
    public class MovesThrowUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform RequestPanel;

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
            {
                TrackingPlayer.OnMovesDieStarted -= MovesDieStarted;
                TrackingPlayer.OnMovesDieFinished -= MovesDieFinished;
            }

            TrackingPlayer = player;
            TrackingPlayer.OnMovesDieStarted += MovesDieStarted;
            TrackingPlayer.OnMovesDieFinished += MovesDieFinished;

            if (TrackingPlayer.WaitingForMovesDie)
                MovesDieStarted();
            else
                MovesDieFinished();
        }

        void MovesDieStarted()
        {
            RequestPanel.gameObject.SetActive(true);
        }

        void MovesDieFinished()
        {
            RequestPanel.gameObject.SetActive(false);
        }
    }
}