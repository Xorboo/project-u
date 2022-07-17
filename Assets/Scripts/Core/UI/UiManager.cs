using System;
using Core.Player;
using Core.Story;
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

        [SerializeField]
        string MysticRevealThrowText;

        [Space]
        [SerializeField]
        RandomEventUi RandomEncounterUi;

        [SerializeField]
        DialogUi DialogUi;

        [SerializeField]
        ChestUi ChestUi;

        [SerializeField]
        MysticInteractUi MysticUi;


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

        public void ShowMysticRevealThrow()
        {
            ShowDieRequest(MysticRevealThrowText);
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
            DialogUi.CloseDialog();
            ChestUi.CloseChestInfo();
            MysticUi.ClosePanel();
        }

        public void ShowEncounterWindow(RandomEncounterData encounter, int diceChange, Action onEncounterWindowClosed)
        {
            RandomEncounterUi.OpenEncounter(encounter.Description, diceChange, onEncounterWindowClosed);
        }

        public void ShowStoryEntry(StoryEntry entry, Action onDialogClosed)
        {
            DialogUi.OpenDialog(entry, onDialogClosed);
        }

        public void ShowChestEntry(int diceAmount, Action onChestUiClosed)
        {
            ChestUi.OpenChestInfo(diceAmount, onChestUiClosed);
        }

        public void ShowMysticInteractText(string interactText, Action onMysticWindowClosed)
        {
            MysticUi.OpenPanel(interactText, onMysticWindowClosed);
        }
    }
}