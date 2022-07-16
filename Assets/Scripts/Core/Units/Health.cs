using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    int health_max;
    public int health = 10;
    [SerializeField] TMP_Text health_text;

    void Start()
    {
        health_max = health;
        HealthUpdate();
    }

    void HealthUpdate()
    {
        health_text.text = health + "/" + health_max;
    }

    public void GetDamage (int damage_value)
    {
        health -= damage_value;
        if (health < 0) health = 0;
        HealthUpdate();

        if (health == 0)
        {
            transform.Translate(0, 1, 0);
            transform.Rotate(0, 0, 180, Space.Self);
        }
    }
}
