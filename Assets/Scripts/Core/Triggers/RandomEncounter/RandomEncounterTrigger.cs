using System;
using Core.Player;
using Core.UI;
using UnityEngine;

namespace Core.Triggers.RandomEncounter
{
    public class RandomEncounterTrigger : PlayerTriggerBase
    {
        [SerializeField]
        GameParameters Data;

        [SerializeField]
        RandomEncountersData EncountersData;


        #region Unity

        #endregion

        protected override void ProcessTrigger(PlayerController player, Action onCompleted)
        {
            var encounter = EncountersData.GetRandomEncounter();
            int diceChange = 0;
            switch (encounter.Type)
            {
                case RandomEncounterData.EncounterType.AddPercent:
                    diceChange = Mathf.CeilToInt(GameManager.Instance.DiceCount * Data.DiceMultiplyFactor);
                    break;
                case RandomEncounterData.EncounterType.RemovePercent:
                    diceChange = Mathf.CeilToInt(GameManager.Instance.DiceCount * -Data.DiceMultiplyFactor);
                    break;
                case RandomEncounterData.EncounterType.VillageBonusA:
                    diceChange = GameManager.Instance.VillagesCount * Data.VillageBonusA;
                    break;
                case RandomEncounterData.EncounterType.VillageBonusB:
                    diceChange = GameManager.Instance.VillagesCount * Data.VillageBonusB;
                    break;
                case RandomEncounterData.EncounterType.IncreaseEnemyHp:
                    break;
                case RandomEncounterData.EncounterType.RandomText:
                    break;
                default:
                    Debug.LogError($"Unknown encounter type: {encounter.Type}");
                    break;
            }

            UiManager.Instance.ShowEncounterWindow(encounter, diceChange, EncounterWindowClosed);

            void EncounterWindowClosed()
            {
                ProcessEncounterResult(encounter, diceChange);

                // TODO Animate hide
                Destroy(gameObject);
                onCompleted?.Invoke();
            }
        }

        void ProcessEncounterResult(RandomEncounterData encounter, int diceChange)
        {
            if (diceChange != 0)
            {
                GameManager.Instance.ChangeDice(diceChange);
            }
            else
            {
                switch (encounter.Type)
                {
                    case RandomEncounterData.EncounterType.IncreaseEnemyHp:
                        GameManager.Instance.IncreaseEnemyHp();
                        break;
                    case RandomEncounterData.EncounterType.RandomText:
                        break;
                }
            }
        }
    }
}