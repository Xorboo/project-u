using System;
using Core.Map;
using DG.Tweening;
using UnityEngine;

namespace Core.Player
{
    public class PlayerController : MonoBehaviour
    {
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


        #region Unity

        void OnEnable() { }

        void Disable() { }

        #endregion


        public void SetSpawnPoint(Vector2Int spawnPoint)
        {
            Debug.Log($"Spawning player at {spawnPoint}");
            Coordinates = spawnPoint;
            transform.position = MapParameters.GetTileCenter3D(Coordinates);
        }

        public void MoveToTile(Vector2Int coord, Tile tile)
        {
            if (CurrentState != State.Idle)
            {
                Debug.LogError($"Can't move when not idle");
                return;
            }

            MovePlayer(coord);
        }

        void MovePlayer(Vector2Int coord)
        {
            CurrentState = State.Moving;
            Coordinates = coord;

            Vector3 pos = MapParameters.GetTileCenter3D(Coordinates);
            transform.DOMove(pos, MoveTime).SetEase(Ease.Linear).OnComplete(() => CurrentState = State.Idle);
        }

        public bool IsAdjacent(Vector2Int coord)
        {
            Vector2Int delta = coord - Coordinates;
            return Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1;
        }
    }
}