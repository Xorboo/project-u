using UnityEngine;

namespace Core.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        float LerpSpeed = 20f;


        PlayerController TargetPlayer;


        #region Unity

        void OnEnable()
        {
            GameManager.Instance.OnPlayerSpawned += PlayerSpawned;

            if (GameManager.Instance.Player != null)
                PlayerSpawned(GameManager.Instance.Player);
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnPlayerSpawned -= PlayerSpawned;
        }

        void LateUpdate()
        {
            if (TargetPlayer == null)
                return;

            Vector3 targetPos = TargetPlayer.transform.position;
            transform.position = Vector3.Lerp(transform.position, targetPos, LerpSpeed * Time.deltaTime);
        }

        #endregion


        void PlayerSpawned(PlayerController player)
        {
            TargetPlayer = player;
        }
    }
}