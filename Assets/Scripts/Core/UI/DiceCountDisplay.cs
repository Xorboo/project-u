using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class DiceCountDisplay : MonoBehaviour
    {
        [SerializeField]
        TMP_Text CountText;


        #region Unity

        void OnEnable()
        {
            GameManager.Instance.OnDiceCountChanged += DiceCountChanged;
            DiceCountChanged(GameManager.Instance.DiceCount);
        }

        void OnDisable()
        {
            if (GameManager.Exists())
                GameManager.Instance.OnDiceCountChanged -= DiceCountChanged;
        }

        #endregion


        void DiceCountChanged(int count)
        {
            CountText.text = count.ToString();
        }
    }
}