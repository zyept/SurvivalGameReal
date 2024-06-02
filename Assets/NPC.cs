using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NPC : MonoBehaviour
{
    public bool playerInRange;
    public bool isTalkingWithPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { playerInRange = false; }
    }
    public void StartConversation()
    {
        isTalkingWithPlayer = true;
        print("Conversation Started");


        DialogSystem.Instance.OpenDialogUI();
        DialogSystem.Instance.dialogText.text = "Hello There";
        DialogSystem.Instance.option1BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text="Bye";
        DialogSystem.Instance.option1BTN.onClick.AddListener(() =>
        {
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer=false;
        });
    }
}
