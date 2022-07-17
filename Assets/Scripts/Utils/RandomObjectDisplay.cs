using UnityEngine;

namespace Utils
{
    public class RandomObjectDisplay : MonoBehaviour
    {
        [SerializeField]
        WeightedRandom<GameObject> Objects = new();

        [SerializeField]
        bool ShowOnAwake = true;

        #region Unity

        void Awake()
        {
            if (ShowOnAwake)
            {
                ShowRandom();
                return;
            }

            // Otherwise hide all elements
            foreach (var element in Objects.GetElements())
                element.SetActive(false);
        }

        #endregion

        public void ShowRandom()
        {
            var selected = Objects.GetRandom();

            foreach (var element in Objects.GetElements())
                element.SetActive(element == selected);
        }
    }
}