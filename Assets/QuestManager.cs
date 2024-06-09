using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; set; }

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

    public List<Quest> allActiveQuests;
    public List<Quest> allCompletedQuests;

    [Header("QuestMenu")]
    public GameObject questMenu;
    public bool isQuestMenuOpen;

    public GameObject activeQuestPrefab;
    public GameObject completedQuestPrefab;

    public GameObject questMenuContent;

    [Header("QuestTracker")]
    public GameObject questTrackerContent;
    public GameObject trackerRowPrefab;

    public List<Quest> allTrackedQuests;

    public void TrackQuest(Quest quest)
    {
        allTrackedQuests.Add(quest);
        RefreshTrackerList();
    }

    public void UnTrackQuest(Quest quest)
    {
        allTrackedQuests.Remove(quest);
        RefreshTrackerList();
    }

    public void RefreshTrackerList()
    {
        foreach (Transform child in questTrackerContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest trackedQuest in allTrackedQuests)
        {
            GameObject trackerPrefab = Instantiate(trackerRowPrefab, Vector3.zero, Quaternion.identity);
            trackerPrefab.transform.SetParent(questTrackerContent.transform, false);

            TrackerRow tRow = trackerPrefab.GetComponent<TrackerRow>();

            tRow.questName.text = trackedQuest.questName;
            tRow.description.text = trackedQuest.questDescription;

            var req1 = trackedQuest.info.firstRequirmentItem;
            var req1Amount = trackedQuest.info.firstRequirementAmount;
            var req2 = trackedQuest.info.secondRequirementItem;
            var req2Amount = trackedQuest.info.secondRequirementAmount;

            if (trackedQuest.info.secondRequirementItem != "")
            {
                tRow.requirements.text = $"{req1} " + InventorySystem.Instance.CheckItemAmount(req1) + "/" + $"{req1Amount}\n" +
                $"{req2} " + InventorySystem.Instance.CheckItemAmount(req2) + "/" + $"{req2Amount}\n";
            }
            else
            {
                tRow.requirements.text = $"{req1} " + InventorySystem.Instance.CheckItemAmount(req1) + "/" + $"{req1Amount}\n";
            }
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q) && !isQuestMenuOpen && !ConstructionManager.Instance.inConstructionMode)
        {
            questMenu.SetActive(true);

            questMenu.GetComponentInChildren<Canvas>().sortingOrder = MenuManager.Instance.SetAsFront();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

            isQuestMenuOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.Q) && isQuestMenuOpen)
        {
            questMenu.SetActive(false);

            if (!CraftingSystem.Instance.isOpen || !InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.Instance.EnableSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            }

            isQuestMenuOpen = false;
        }
    }

    public void AddActiveQuest(Quest quest)
    {
        allActiveQuests.Add(quest);
        TrackQuest(quest);
        RefreshQuestList();
    }

    public void MarkQuestCompleted(Quest quest)
    {
        allActiveQuests.Remove(quest);
        allCompletedQuests.Add(quest);
        UnTrackQuest(quest);

        RefreshQuestList();
    }

    public void RefreshQuestList()
    {
        foreach (Transform child in questMenuContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest activeQuest in allActiveQuests)
        {
            GameObject questPrefab = Instantiate(activeQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            qRow.thisQuest = activeQuest;

            qRow.questName.text = activeQuest.questName;
            qRow.questGiver.text = activeQuest.questGiver;

            qRow.isActive = true;
            qRow.isTracking = true;

            qRow.coinAmount.text = $"{activeQuest.info.coinReward}";

            //hata veriyor diye yoruma ald�m getSpriteForItem � da

            if (activeQuest.info.rewardItem1 != "")
            {
                //qRow.firstReward.sprite = GetSpriteForitem(activeQuest.info.rewardItem1);
                qRow.firstRewardAmount.text = ""; 
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmount.text = "";
            }

            if (activeQuest.info.rewardItem2 != "")
            {
                //qRow.secondReward.sprite = GetSpriteForitem(activeQuest.info.rewardItem2);
                qRow.secondRewardAmount.text = ""; 
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmount.text = "";
            }
        }

        foreach (Quest completedQuest in allCompletedQuests)
        {
            GameObject questPrefab = Instantiate(completedQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            qRow.questName.text = completedQuest.questName;
            qRow.questGiver.text = completedQuest.questGiver;

            qRow.isActive = true;
            qRow.isTracking = true;

            qRow.coinAmount.text = $"{completedQuest.info.coinReward}";
            if (completedQuest.info.rewardItem1 != "")
            {
                //qRow.firstReward.sprite = GetSpriteForitem(completedQuest.info.rewardItem1);
                qRow.firstRewardAmount.text = "";
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmount.text = "";
            }

            if (completedQuest.info.rewardItem2 != "")
            {
               // qRow.secondReward.sprite = GetSpriteForitem(completedQuest.info.rewardItem2);
                qRow.secondRewardAmount.text = "";
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmount.text = "";
            }
        }
    }
    /*
    private Sprite GetSpriteForitem(string item)
    {
        var itemToGet = Resources.Load<GameObject>(item);
        return itemToGet.GetComponent<Image>().sprite;

   }
    */


}
