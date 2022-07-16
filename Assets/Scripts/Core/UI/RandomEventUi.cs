using Core.Map;
using UnityEngine;

namespace Core.UI
{
    public class RandomEventUi : MonoBehaviour
    {
        [SerializeField]
        RectTransform RandomEventPanel;

        #region Unity

        void OnEnable()
        {
            Tile.OnRandomEventStarted += EventStarted;
            Tile.OnRandomEventFinished += EventFinished;

            RandomEventPanel.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            Tile.OnRandomEventStarted -= EventStarted;
            Tile.OnRandomEventFinished -= EventFinished;
        }

        #endregion


        void EventStarted(Tile tile)
        {
            RandomEventPanel.gameObject.SetActive(true);
        }

        void EventFinished(Tile tile)
        {
            RandomEventPanel.gameObject.SetActive(false);
        }
    }
}