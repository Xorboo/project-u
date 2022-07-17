using UnityEngine;

namespace Utils
{
    public class RandomObjectDisplay : MonoBehaviour
    {
        [SerializeField]
        WeightedRandom<GameObject> Objects = new();

        #region Unity

        void Awake()
        {
            var selected = Objects.GetRandom();

            foreach (var element in Objects.GetElements())
                element.SetActive(element == selected);
        }

        #endregion
    }
}