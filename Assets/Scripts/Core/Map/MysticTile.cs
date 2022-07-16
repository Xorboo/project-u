using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Map
{
    [RequireComponent(typeof(TileRevealController))]
    public class MysticTile : MonoBehaviour
    {
        [SerializeField]
        GameObject InitialViewRoot;

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
            RevealedRoots[dieResult].SetActive(true);

            // Replay the same reveal animation again
            RevealController.ForceReveal(0f, onFinished);
        }
    }
}