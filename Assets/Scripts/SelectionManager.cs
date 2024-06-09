using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }
    public bool onTarget;
    public GameObject selectedObject;

    public GameObject interaction_info_UI; 
    Text interaction_text;

    public Image centerDotImage;
    public Image handIcon;


    public bool handIsVisible;

    public GameObject selectedTree;
    public GameObject chopHolder;

    public GameObject selectedStorageBox;

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

            ChoppableTree choppabletree= selectionTransform.GetComponent<ChoppableTree>();

            NPC npc = selectionTransform.GetComponent<NPC>();
            if(npc&& npc.playerInRange)
            {
                interaction_text.text = "Talk";
                interaction_info_UI.SetActive(true);

                if (Input.GetMouseButtonDown(0) && npc.isTalkingWithPlayer == false)
                {
                    npc.StartConversation();
                }

                if (DialogSystem.Instance.dialogUIActive)
                {
                    interaction_info_UI.SetActive(false);
                    centerDotImage.gameObject.SetActive(false);
                }
                
            }

            if (choppabletree && choppabletree.playerInRange)
            {
                choppabletree.canBeChopped = true;
                selectedTree = choppabletree.gameObject;
                chopHolder.gameObject.SetActive(true);
            }
            else
            {
                if (selectedTree != null)
                {
                    selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;
                    chopHolder.gameObject.SetActive(false);
                }
            }


            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_info_UI.SetActive(true);

               // centerDotImage.gameObject.SetActive(false);
               // handIcon.gameObject.SetActive(true);
               // handIsVisible = true;
             

            }
            StorageBox storageBox = selectionTransform.GetComponent<StorageBox>();

            if(storageBox && storageBox.playerInRange && PlacementSystem.Instance.inPlacementMode==false)
            {
                interaction_text.text = "Open";
                interaction_info_UI.SetActive(true);
                selectedStorageBox = storageBox.gameObject;

                if(Input.GetMouseButtonDown(0))
                {
                    StorageManager.Instance.OpenBox(storageBox);
                }

            }
            else
            {
                if(selectedStorageBox != null)
                {
                    selectedStorageBox = null;
                }
            }





            Animal animal = selectionTransform.GetComponent<Animal>();
            if(animal && animal.playerInRange)
            {

                if(animal.isDead)
                {
                    interaction_text.text = "Loot";
                    interaction_info_UI.SetActive(true);
                    centerDotImage.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);
                    handIsVisible = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        Lootable lootable = animal.GetComponent<Lootable>();
                        Loot(lootable);
                    }
                }
                else
                {
                    interaction_text.text = animal.animalName;
                    interaction_info_UI.SetActive(true);
                     centerDotImage.gameObject.SetActive(true);
                      handIcon.gameObject.SetActive(false);
                     handIsVisible = false;

                    if (Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingWeapon() && EquipSystem.Instance.IsThereASwingLock()==false)
                    {
                        StartCoroutine(DealDamageTo(animal, 0.3f, EquipSystem.Instance.GetWeaponDamage()));
                    }

                }
              
            }
            if(!interactable && !animal)
            {
                onTarget = false;
                handIsVisible = false;
                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
            }
            if(!npc && !interactable && !animal && !choppabletree && !storageBox)
            {
                interaction_text.text = "";
                interaction_info_UI.SetActive(false);
                
            }





        }
        
    }

    private void Loot (Lootable lootable)
    {
        if(lootable.wasLootCalculated==false)
        {
            List<LootRecieved> recievedLoot = new List<LootRecieved>();

            foreach(LootPossibility loot in lootable.possibleLoot)
            {
                var lootAmount = UnityEngine.Random.Range(loot.amountMin, loot.amountMax + 1);
                if(lootAmount > 0)
                {
                    LootRecieved lt = new LootRecieved();
                    lt.item = loot.item;
                    lt.amount= lootAmount;
                    recievedLoot.Add(lt);
                }


            }
            lootable.finalLoot=recievedLoot;
            lootable.wasLootCalculated=true;
        }
        //Spawning the loot on the ground
        Vector3 lootSpawnPosition = lootable.gameObject.transform.position;
        foreach(LootRecieved lootRecieved in lootable.finalLoot)
        {
            for(int i=0; i< lootRecieved.amount; i++)
            {
                GameObject lootSpawn = Instantiate(Resources.Load<GameObject>(lootRecieved.item.name + "_Model"),
                   new Vector3(lootSpawnPosition.x, lootSpawnPosition.y + 0.2f, lootSpawnPosition.z),
                   Quaternion.Euler(0, 0, 0));
            }
        }
        Destroy(lootable.gameObject);
        //if chest dont destroy




    }


    IEnumerator DealDamageTo(Animal animal,float delay,int damage)
    {
        yield return new WaitForSeconds(delay);
        animal.TakeDamage(damage);

    }



    public void DisableSelection()
    {
        handIcon.enabled = false;
        centerDotImage.enabled = false;
        interaction_info_UI.SetActive(false);

        selectedObject = null;
    }


    public void EnableSelection()
    {
        handIcon.enabled = true;
        centerDotImage.enabled = true;
        interaction_info_UI.SetActive(true);
    }


}