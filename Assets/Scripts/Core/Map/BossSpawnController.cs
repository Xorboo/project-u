using UnityEngine;

namespace Core.Map
{
    public class BossSpawnController : MonoBehaviour
    {
        [SerializeField]
        GameObject common_enemy, boss;

        void Awake()
        {
            if (GameManager.Instance.ReadyToBossSpawn || true)
            {
                common_enemy.SetActive(false);
                boss.SetActive(true);
                GameManager.Instance.ReadyToBossSpawn = false;
            }
            else
            {
                common_enemy.SetActive(true);
                boss.SetActive(false);
            }
        }
    }
}