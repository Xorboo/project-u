using System;
using UnityEngine;
using TMPro;
using Core.Player;

namespace Core.UI
{
    public class RandomEventUi : MonoBehaviour
    {
        [SerializeField]
        GameObject RandomEncounterPanel;

        [SerializeField]
        TMP_Text RandomEncounterText;

        [SerializeField]
        TMP_Text RandomEncounterDicesText;

        [SerializeField]
        string DicesAddedText = "Received dice:";

        [SerializeField]
        string DicesLostText = "Lost dice:";

        [SerializeField]
        string DicesNotChangedText = "Same dice count";


        Action CloseListener;

        [SerializeField]
        GameObject ShopPanel;

        void Start()
        {
            ShopPanel.SetActive(false);
            CloseEncounter();
        }

        public void OpenEncounter(string encounterText, int diceDelta, Action onEncounterWindowClosed)
        {
            CloseListener = onEncounterWindowClosed;

            RandomEncounterPanel.SetActive(true);
            RandomEncounterText.text = encounterText;

            if (diceDelta > 0)
                RandomEncounterDicesText.text = $"{DicesAddedText} {diceDelta}";
            else if (diceDelta < 0)
                RandomEncounterDicesText.text = $"{DicesLostText} {diceDelta}";
            else
                RandomEncounterDicesText.text = DicesNotChangedText;
        }

        public void CloseEncounter()
        {
            RandomEncounterPanel.SetActive(false);
            CloseListener?.Invoke();
        }

        public void OpenShop(bool state)
        {
            ShopPanel.SetActive(state);
            if (state)
                RandomEncounterDicesText.text = "";
        }

        public void BuyRestoreHealth(int cost)
        {
            if (GameManager.Instance.Player.MoneyValue >= cost)
            {
                GameManager.Instance.Player.GetLoot(-cost);
                GameManager.Instance.Player.Health.ApplyHeal(100);
            }
        }

        public void BuyMaxHealth1(int cost)
        {
            if (GameManager.Instance.Player.MoneyValue >= cost)
            {
                GameManager.Instance.Player.GetLoot(-cost);
                GameManager.Instance.Player.Health.AddMaxHealth(2);
            }
        }

        public void BuyMaxHealth2(int cost)
        {
            if (GameManager.Instance.Player.MoneyValue >= cost)
            {
                GameManager.Instance.Player.GetLoot(-cost);
                GameManager.Instance.Player.Health.AddMaxHealth(5);
            }
        }

        public void BuyMaxHealth3(int cost)
        {
            if (GameManager.Instance.Player.MoneyValue >= cost)
            {
                GameManager.Instance.Player.GetLoot(-cost);
                GameManager.Instance.Player.Health.AddMaxHealth(20);
            }
        }
    }
}