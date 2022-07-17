using System;
using System.Collections.Generic;
using Core.Triggers.MysticTriggers;
using UnityEngine;

namespace Core.Map
{
    [RequireComponent(typeof(TileRevealController))]
    public class MysticTile : MonoBehaviour
    {
        [SerializeField]
        GameObject InitialViewRoot;

        [SerializeField]
        GameObject ExploredViewRoot;

        [SerializeField]
        List<GameObject> RevealedRoots = new();


        TileRevealController RevealController;


        #region Unity

        void Awake()
        {
            RevealController = GetComponent<TileRevealController>();
        }

        void OnEnable()
        {
            InitialViewRoot.SetActive(true);
            ExploredViewRoot.SetActive(false);

            foreach (var root in RevealedRoots)
            {
                if (root)
                    root.SetActive(false);
            }
        }

        #endregion


        public void RevealTile(int dieResult, Action onFinished)
        {
            InitialViewRoot.SetActive(false);
            ExploredViewRoot.SetActive(true);
            var revealedItem = RevealedRoots[dieResult];
            revealedItem.SetActive(true);

            // Replay the same reveal animation again
            RevealController.ForceReveal(0f, ProcessImmediateMysticTile);

            void ProcessImmediateMysticTile()
            {
                // Check if something should be changed before the player steps on a tile
                var immediateTrigger = revealedItem.GetComponent<VillageTrigger>();
                if (!immediateTrigger)
                {
                    onFinished?.Invoke();
                    return;
                }

                immediateTrigger.ImmediateProcessTrigger(onFinished);
            }
        }
    }
}