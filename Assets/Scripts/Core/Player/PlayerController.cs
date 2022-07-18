using System;
using Core.Map;
using Core.UI;
using Core.Units;
using DG.Tweening;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Player
{
    public class PlayerController : MonoBehaviour
    {
        public event Action<int> OnMovesCountChanged = delegate { };
        public event Action<int> OnMoneyCountChanged = delegate { };


        public enum State
        {
            Idle,
            Moving
        };


        [SerializeField]
        MapParameters MapParameters;

        [SerializeField]
        GameParameters GameParameters;

        [SerializeField]
        float MoveTime = 1f;

        public PlayerHealth Health;

        [FormerlySerializedAs("_anim")]
        [SerializeField]
        public Animator Animator;

        [SerializeField]
        PlayerAnimationListener AnimationListener;


        public Vector2Int Coordinates { get; private set; }
        public State CurrentState { get; private set; } = State.Idle;


        public int MovesLeft
        {
            get => MovesLeftValue;
            private set
            {
                MovesLeftValue = value;
                OnMovesCountChanged(MovesLeftValue);
            }
        }

        int MovesLeftValue = 0;

        public int Money
        {
            get => MoneyValue;
            private set
            {
                MoneyValue = value;
                OnMoneyCountChanged(MoneyValue);
            }
        }

        public int MoneyValue = 0;

        #region Unity

        void OnEnable() { }

        void Disable() { }

        #endregion


        public void SpawnPlayer(Vector2Int spawnPoint)
        {
            Debug.Log($"Spawning player at {spawnPoint}");
            Coordinates = spawnPoint;
            transform.position = MapParameters.GetTileCenter3D(Coordinates);

            Health.Initialize();
            CheckMovesDie();
        }

        public void MoveToTile(Vector2Int coord, Tile tile, Action onFinished = null)
        {
            if (CurrentState != State.Idle)
            {
                Debug.LogError($"Can't move when not idle");
                return;
            }

            MovePlayer(coord, tile, onFinished);
        }

        public void CheckMovesDie()
        {
            if (MovesLeft > 0)
                return;

            Debug.Log($"Requiring die throw for moves");
            UiManager.Instance.ShowMoveThrow();
            GameManager.Instance.WaitForDieThrowResult(MoveDieThrown);

            void MoveDieThrown(int dieResult)
            {
                Debug.Log($"Player now has {dieResult} moves");
                UiManager.Instance.HideDieRequest();
                MovesLeft = dieResult;
            }
        }

        Tween ActiveMoveTween = null;

        void MovePlayer(Vector2Int coord, Tile tile, Action onFinished = null)
        {
            if (ActiveMoveTween != null)
            {
                Debug.LogError($"Moving when older tween is still active!");
                ActiveMoveTween.Kill();
                ActiveMoveTween = null;
            }

            CurrentState = State.Moving;
            Animator.SetBool("isWalking", true);
            Vector2Int delta = coord - Coordinates;
            Coordinates = coord;
            MovesLeft--;

            transform.DOLocalRotate(new Vector3(0f, GetRotation(delta), 0f), 0.2f);

            Vector3 pos = MapParameters.GetTileCenter3D(Coordinates);
            ActiveMoveTween = transform
                .DOMove(pos, MoveTime)
                .SetEase(Ease.Linear)
                .OnComplete(VisitTile);

            void VisitTile()
            {
                tile.Visit(RevealTiles);
            }

            void RevealTiles()
            {
                MapManager.Instance.RevealTilesAround(coord, () =>
                {
                    CurrentState = State.Idle;
                    Animator.SetBool("isWalking", false);
                    ActiveMoveTween = null;
                    onFinished?.Invoke();
                });
            }
        }

        public void TriggerStarted()
        {
            if (ActiveMoveTween == null)
            {
                Debug.LogError($"Trigger started when not moving, ignoring");
                return;
            }

            ActiveMoveTween.Pause();
        }

        public void TriggerFinished()
        {
            if (ActiveMoveTween == null)
            {
                Debug.LogError($"Trigger finished when not moving, ignoring");
                return;
            }

            ActiveMoveTween.Play();
        }

        float GetRotation(Vector2Int delta) => delta.x == 1 ? 0 : delta.x == -1 ? 180 : delta.y == 1 ? -90 : 90;

        public bool IsAdjacent(Vector2Int coord)
        {
            Vector2Int delta = coord - Coordinates;
            return Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1;
        }

        public void AnimateAttack(EnemyUnit enemy, Action onDealDamage, Action onCompleted)
        {
            AnimationListener.SetAttackListener(onDealDamage, onCompleted);
            Animator.SetTrigger("Attack");
        }

        public void GetLoot(int money_value)
        {
            Money += money_value;
        }
    }
}