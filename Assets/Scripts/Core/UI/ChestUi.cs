using Core.Map;
using UnityEngine;

namespace Core.UI
{
    public class ChestUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform ChestPanel;

        #region Unity

        void OnEnable()
        {
            Tile.OnChestOpenStarted += ChestStarted;
            Tile.OnChestOpenFinished += ChestFinished;

            ChestPanel.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            Tile.OnChestOpenStarted -= ChestStarted;
            Tile.OnChestOpenFinished -= ChestFinished;
        }

        #endregion


        void ChestStarted(Tile tile)
        {
            ChestPanel.gameObject.SetActive(true);
        }

        void ChestFinished(Tile tile)
        {
            ChestPanel.gameObject.SetActive(false);
        }
    }
}