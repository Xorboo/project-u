using Core.Map;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class FightUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform FightPanel;

        [SerializeField]
        TMP_Text EnemyHpText;

        #region Unity

        void OnEnable()
        {
            Tile.OnFightStarted += FightStarted;
            Tile.OnFightFinished += FightFinished;

            FightPanel.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            Tile.OnFightStarted -= FightStarted;
            Tile.OnFightFinished -= FightFinished;
        }

        #endregion


        void FightStarted(Tile tile)
        {
            FightPanel.gameObject.SetActive(true);

            tile.OnEnemyHpChanged += EnemyHpChanged;
            EnemyHpChanged(tile.CurrentEnemyHp);
        }

        void FightFinished(Tile tile)
        {
            FightPanel.gameObject.SetActive(false);

            tile.OnEnemyHpChanged -= EnemyHpChanged;
        }

        void EnemyHpChanged(int enemyHp)
        {
            EnemyHpText.text = enemyHp.ToString();
        }
    }
}