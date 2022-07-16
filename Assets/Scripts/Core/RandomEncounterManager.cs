using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounterManager : MonoBehaviour
{
    [System.Serializable]
    class encounter
    {
        public string description;
        public int add_dices;
        public int event_type; //1 - потерять процент, 2 и 3 - получить  бонус с деревень, 4 - просто текст, 5 - увеличение хп противников, 6 - получить процент
        public int random_weight;
    }

    [SerializeField] Core.UI.RandomEventUi _ui;

    [SerializeField] encounter[] encounters;
    int total_weights;

    [SerializeField]
    Core.GameParameters Data;

    void Start()
    {
        for (int i = 0; i < encounters.Length; i++)
        {
            total_weights += encounters[i].random_weight;
        }
    }

    public void StartEncounter()
    {
        int encounter_num = GetRandomEcounterNumber();
        int dice_delta = encounters[encounter_num].add_dices;
        Core.GameManager.Instance.ResolveRandomEvent(encounters[encounter_num].event_type);

        if (encounters[encounter_num].event_type == 1)
            dice_delta = Mathf.CeilToInt(Core.GameManager.Instance.DiceCount * -Data.DiceMultiplyFactor);
        if (encounters[encounter_num].event_type == 2)
            dice_delta = Core.GameManager.Instance.VillagesCount * Data.VillageBonusA;
        if (encounters[encounter_num].event_type == 3)
            dice_delta = Core.GameManager.Instance.VillagesCount * Data.VillageBonusB;
        if (encounters[encounter_num].event_type == 6)
            dice_delta = Mathf.CeilToInt(Core.GameManager.Instance.DiceCount * Data.DiceMultiplyFactor);
        

        _ui.OpenEncounter(encounters[encounter_num].description, dice_delta);
    }

    int GetRandomEcounterNumber()
    {
        int random_value = Random.Range(1, total_weights + 1);
        int processed_weight = 0;
        int event_num = 0;
        for (int i = 0; i < encounters.Length; i++)
        {
            processed_weight += encounters[i].random_weight;
            if (random_value <= processed_weight)
            {
                event_num = i;
                break;
            }
        }
        return event_num;
    }
}
