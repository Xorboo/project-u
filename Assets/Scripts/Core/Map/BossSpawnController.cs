using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnController : MonoBehaviour
{
    [SerializeField] GameObject common_enemy, boss;

    void Awake()
    {
        if (Core.GameManager.Instance.ReadyToBossSpawn)
        {
            common_enemy.SetActive(false);
            boss.SetActive(true);
            Core.GameManager.Instance.ReadyToBossSpawn = false;
        }
        else
        {
            common_enemy.SetActive(true);
            boss.SetActive(false);
        }
        
    }
}
