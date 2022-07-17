using System;
using Core.Player;
using Core.Triggers.RandomEncounter;
using TMPro;
using UnityEngine;
using UrUtils.Misc;

namespace Core.UI
{
    public class UiManager : Singleton<UiManager>
    {
        [SerializeField]
        GameObject DieThrowRequestRoot;

        [SerializeField]
        TMP_Text DireThrowRequestText;

        [Header("Die requests")]
        //[SerializeField]
        //string RandomEncounterThrowText;
        [SerializeField]
        string MoveThrowText;

        [Space]
        [SerializeField]
        RandomEventUi RandomEncounterUi;


        #region Unity

        void OnEnable()
        {
            GameManager.Instance.OnPlayerSpawned += PlayerSpawned;
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnPlayerSpawned -= PlayerSpawned;
        }

        #endregion

        protected override bool IsPersistent => false;

        /*public void ShowRandomEncounterThrow()
        {
            ShowDieRequest(RandomEncounterThrowText);
        }*/

        public void ShowMoveThrow()
        {
            ShowDieRequest(MoveThrowText);
        }

        public void HideDieRequest()
        {
            DieThrowRequestRoot.SetActive(false);
        }

        void ShowDieRequest(string text)
        {
            DieThrowRequestRoot.SetActive(true);
            DireThrowRequestText.text = text;
        }

        void PlayerSpawned(PlayerController player)
        {
            // Reset all states
            HideDieRequest();
            RandomEncounterUi.CloseEncounter();
        }

        public void ShowEncounterWindow(RandomEncounterData encounter, int diceChange, Action onEncounterWindowClosed)
        {
            RandomEncounterUi.OpenEncounter(encounter.Description, diceChange, onEncounterWindowClosed);
        }
    }
}