using System;
using Core.Map;
using DG.Tweening;
using UnityEngine;

namespace Core.Player
{
    public class PlayerController : MonoBehaviour
    {
        public event Action<int> OnMovesCountChanged = delegate { };


        public enum State
        {
            Idle,
            Moving
        };


        [SerializeField]
        MapParameters MapParameters;

        [SerializeField]
        float MoveTime = 1f;


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


        #region Unity

        void OnEnable() { }

        void Disable() { }

        #endregion


        public void SetSpawnPoint(Vector2Int spawnPoint)
        {
            Debug.Log($"Spawning player at {spawnPoint}");
            Coordinates = spawnPoint;
            transform.position = MapParameters.GetTileCenter3D(Coordinates);

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
            GameManager.Instance.WaitForDieThrowResult(MoveDieThrown);

            void MoveDieThrown(int dieResult)
            {
                Debug.Log($"Player now has {dieResult} moves");
                MovesLeft = dieResult;
            }
        }


        void MovePlayer(Vector2Int coord, Tile tile, Action onFinished = null)
        {
            CurrentState = State.Moving;
            Vector2Int delta = coord - Coordinates;
            Coordinates = coord;
            MovesLeft--;

            transform.DOLocalRotate(new Vector3(0f, GetRotation(delta), 0f), 0.2f);

            Vector3 pos = MapParameters.GetTileCenter3D(Coordinates);
            transform
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
                    onFinished?.Invoke();
                });
            }
        }

        float GetRotation(Vector2Int delta) => delta.x == 1 ? 0 : delta.x == -1 ? 180 : delta.y == 1 ? -90 : 90;

        public bool IsAdjacent(Vector2Int coord)
        {
            Vector2Int delta = coord - Coordinates;
            return Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1;
        }
    }
}