using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogUi : MonoBehaviour
{
    [SerializeField]
    GameObject DialogPanel;
    [SerializeField] TMP_Text npc_name_text, npc_text_text;
    
    void Start()
    {
        CloseDialog();
    }

    public void OpenDialog(string npc_name, string npc_text)
    {
        DialogPanel.SetActive(true);
        npc_name_text.text = npc_name;
        npc_text_text.text = npc_text;
    }

    public void CloseDialog()
    {
        DialogPanel.SetActive(false);
    }
}
