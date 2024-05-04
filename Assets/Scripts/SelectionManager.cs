using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }
    public bool onTarget;


    public GameObject interaction_info_UI; 
    Text interaction_text;
    // Start is called before the first frame update
    void Start()
    {
        onTarget = false;
        interaction_text = interaction_info_UI.GetComponent<Text>();

    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        { Destroy(gameObject); }
        else { Instance = this; }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                interaction_text.text =interactable.GetItemName();
                interaction_info_UI.SetActive(true);
            }
            else //if there is a hit, but without an interactable script.
            {
                onTarget=false;
                interaction_info_UI.SetActive(false);
            }
        }
        else // if there is no hit at all.
        {
            onTarget=!false;
            interaction_info_UI.SetActive(false);
        }
    }
}