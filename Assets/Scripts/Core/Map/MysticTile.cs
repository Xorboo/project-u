using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Map
{
    public class MysticTile : MonoBehaviour
    {
        [SerializeField]
        GameObject InitialViewRoot;

        [SerializeField]
        List<GameObject> RevealedRoots = new List<GameObject>();


        #region Unity

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
            onFinished();
        }
    }
}