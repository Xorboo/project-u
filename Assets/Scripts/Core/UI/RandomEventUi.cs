using Core.Map;
using UnityEngine;
using TMPro;

namespace Core.UI
{
    public class RandomEventUi : MonoBehaviour
    {
        [SerializeField] GameObject RandomEncounterPanel;
        [SerializeField] TMP_Text RandomEncounter_text, RandomEncounter_dices_text;

        void Start()
        {
            CloseEncounter();
        }

        public void OpenEncounter(string encounter_text, int add_dices)
        {
            RandomEncounterPanel.SetActive(true);
            RandomEncounter_text.text = encounter_text;
            
            if (add_dices > 0)
                RandomEncounter_dices_text.text = "Получено кубиков: " + add_dices;
            else if (add_dices < 0)
                RandomEncounter_dices_text.text = "Потеряно кубиков: " + -add_dices;
            else
                RandomEncounter_dices_text.text = "Количество кубиков не изменилось";
        }

        public void CloseEncounter()
        {
            RandomEncounterPanel.SetActive(false);
        }
    }
}