using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingManager : MonoBehaviour
{
    [SerializeField] GameObject FightUI_Panel;
    [SerializeField] Health health_player, health_enemy;
    void Start()
    {
        FightUI_Panel.SetActive(false);
    }

    void StartBattle()
    {
        FightUI_Panel.SetActive(true);        
        Core.GameManager.Instance.WaitForDieThrowResult(DealDamage);
    }

    void DealDamage(int dieResult)
    {
        health_enemy.GetDamage(dieResult);
        health_player.GetDamage(Random.Range(1,4));

        if (health_enemy.health > 0 && health_player.health > 0)
            Core.GameManager.Instance.WaitForDieThrowResult(DealDamage);
        else
            FightUI_Panel.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartBattle();
    }
}
