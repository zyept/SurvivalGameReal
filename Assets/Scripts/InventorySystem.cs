using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{

    public static InventorySystem Instance { get; set; }
    public GameObject inventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>(); //list of slots
    public List<string> itemList = new List<string>(); //name of item inside slots (slotlist)
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;


    public bool isOpen;
    //public bool isFull;

    //pickup Popup
    public GameObject pickupAlert;
    public Text pickupName;
    public Image pickupImage;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    void Start()
    {
        isOpen = false;
     
        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach(Transform child in inventoryScreenUI.transform)
        {
            if(child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {

            Debug.Log("i is pressed");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            if(!CraftingSystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            isOpen = false;
        }
    }

    public void AddToInventory(string itemName)
    {
        
            whatSlotToEquip = FindNextEmptySlot();
            itemToAdd =Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
            itemToAdd.transform.SetParent(whatSlotToEquip.transform);
            itemList.Add(itemName);


        TriggerPickupPopUp(itemName,itemToAdd.GetComponent<Image>().sprite);

        ReCalculeList();
        CraftingSystem.Instance.RefreshNeededItems();

    }

    void TriggerPickupPopUp(string itemName,Sprite itemSprite)
    {
        pickupAlert.SetActive(true);
        pickupName.text=itemName;
        pickupImage.sprite=itemSprite;
    }


    private GameObject FindNextEmptySlot()
    {
        foreach(GameObject slot in slotList)
        {

            if(slot.transform.childCount==0)
            {
                return slot;
            }

        }

        return new GameObject();
    }


    public bool CheckIfFull()
    {
        int counter = 0;
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount > 0)
            {
                counter += 1;
            }
        
        }
        if (counter == 21)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void RemoveItem(string nameToRemove,int amountToRemove)
    {
        int counter = amountToRemove;   
        for (var i=slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name== nameToRemove +"(Clone)" && counter!=0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter -= 1;
                }
            }
        }

        ReCalculeList();
        CraftingSystem.Instance.RefreshNeededItems();
    }



    public void ReCalculeList()
    {

        itemList.Clear();
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount>0)
            {
                string name = slot.transform.GetChild(0).name;
                string str1 = name;
                string str2 = "(Clone)";
                string result = name.Replace(str2, "");


                itemList.Add(result);
            }
        }
    }

}