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

        [SerializeField]
        string AttackThrowText;

        [Space]
        [SerializeField]
        RandomEventUi RandomEncounterUi;

        [SerializeField]
        DialogUi DialogUi;

        [SerializeField]
        ChestUi ChestUi;

        [SerializeField]
        MysticInteractUi MysticUi;

        [SerializeField]
        PotionUi PotionUi;

        [SerializeField]
        GameOverUi GameOverUi;

        [SerializeField]
        GameObject TutorialPanel;

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
        private void Start()
        {
            TutorialPanel.SetActive(true);
        }
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

        public void ShowAttackThrow()
        {
            ShowDieRequest(AttackThrowText);
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
            PotionUi.ClosePotionPanel();
            GameOverUi.ClosePanel();
        }

        public void ShowEncounterWindow(RandomEncounterData encounter, int diceChange, Action onEncounterWindowClosed)
        {
            RandomEncounterUi.OpenEncounter(encounter.Description, diceChange, onEncounterWindowClosed);
        }

        public void ShowShopPanel(bool state)
        {
            RandomEncounterUi.OpenShop(state);
        }

        public void ShowStoryEntry(StoryEntry entry, Action onDialogClosed)
        {
            DialogUi.OpenDialog(entry, onDialogClosed);
        }

        public void ShowChestEntry(int diceAmount, Action onChestUiClosed, bool isDice)
        {
            ChestUi.OpenChestInfo(diceAmount, onChestUiClosed, isDice);
        }

        public void ShowMysticInteractText(string interactText, Action onMysticWindowClosed)
        {
            MysticUi.OpenPanel(interactText, onMysticWindowClosed);
        }

        public void ShowHealthEntry(float healPercent, Action onPanelClosed)
        {
            PotionUi.OpenHealthPotionPanel(healPercent, onPanelClosed);
        }

        public void ShowBonusHealthEntry(int extraHealth, Action onPanelClosed)
        {
            PotionUi.OpenBonusHealthPotionPanel(extraHealth, onPanelClosed);
        }

        public void ShowGameOverPanel(Action onRestartPressed)
        {
            GameOverUi.OpenGameOverPanel(onRestartPressed);
        }
    }
}