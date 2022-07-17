using System;
using UnityEngine;

namespace Core.Map
{
    [RequireComponent(typeof(TileRevealController))]
    public class Tile : MonoBehaviour
    {
        public TileData Data;

        Action VisitFinishedCallback;
        public int CurrentEnemyHp { get; private set; } = 0;

        public bool IsVisited { get; private set; } = false;

        public int RevealedMysticType { get; private set; } = -1;


        TileRevealController RevealController;


        #region Unity

        void Awake()
        {
            RevealController = GetComponent<TileRevealController>();
        }

        #endregion


        public bool IsRevealed => RevealController.IsRevealed;
        public bool MysticRevealed { get; private set; } = false;

        public void Reveal(float revealDelay)
        {
            RevealController.Reveal(revealDelay);
        }

        public void RevealImmediately()
        {
            RevealController.RevealImmediately();
        }

        public void Visit(Action onVisitFinished)
        {
            if (IsVisited)
            {
                onVisitFinished?.Invoke();
                return;
            }

            IsVisited = true;
            VisitFinishedCallback = onVisitFinished;

            VisitFinishedCallback?.Invoke();
        }

        #region Mystic Tile

        public void RevealMysticTile(int dieResult, Action onFinished)
        {
            MysticRevealed = true;
            RevealedMysticType = dieResult;
            GetComponent<MysticTile>().RevealTile(dieResult, onFinished);
        }

        #endregion
    }
}