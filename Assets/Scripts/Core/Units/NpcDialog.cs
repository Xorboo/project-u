using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDialog : MonoBehaviour
{
    StoryManager _manager;

    void Start()
    {
        _manager = FindObjectOfType<StoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _manager.StoryNPC();
            Destroy(gameObject);
        }
    }
}
