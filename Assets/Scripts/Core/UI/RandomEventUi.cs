using System;
using UnityEngine;
using TMPro;

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


        void Start()
        {
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
    }
}