using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounter : MonoBehaviour
{
    RandomEncounterManager _manager;

    void Start()
    {
        _manager = FindObjectOfType<RandomEncounterManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _manager.StartEncounter();
            Destroy(gameObject);
        }
    }
}
