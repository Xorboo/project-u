using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Core.UI
{
    public class DieFakeThrower : MonoBehaviour
    {
        [SerializeField]
        GameObject CheatPanel;


        #region Unity

        void OnEnable()
        {
            GameManager.Instance.OnDieWaitingChanged += DieWaitingChanged;
            DieWaitingChanged(GameManager.Instance.IsWaitingForDie);
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnDieWaitingChanged -= DieWaitingChanged;
        }

        #endregion


        void DieWaitingChanged(bool isWaiting)
        {
            CheatPanel.SetActive(isWaiting);
        }

        public void OnFakeThrowClicked(int value)
        {
            int dieResult = 0;
            if (value == 0)
                dieResult = Random.Range(0, 6) + 1;
            else
                dieResult = value;
            Debug.Log($"Fake die result: {dieResult}");

            GameManager.Instance.OnDieThrown(dieResult);
        }

        public void OnChangeDiceClicked(int delta)
        {
            GameManager.Instance.ChangeDice(delta);
        }

        public void ClearSave()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            GameManager.Instance.RestartGame();
        }

        public void GetMoney()
        {
            GameManager.Instance.Player.GetLoot(100);
        }
    }
}