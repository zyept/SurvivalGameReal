using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; set; }

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


        DontDestroyOnLoad(gameObject);
    }

    // Json project save path
    string jsonPathProject;
    // Json external/real save path
    string jsonPathPersistant;
    // Binary save path
    string binaryPath;

    string fileName = "SaveGame";

    public bool isSavingToJson;

    public bool isLoading;


    private void Start()
    {
        jsonPathProject = Path.Combine(Application.dataPath, fileName + ".json");
        jsonPathPersistant = Path.Combine(Application.persistentDataPath, fileName + ".json");
        binaryPath = Path.Combine(Application.persistentDataPath, fileName + ".bin");

        Debug.Log("jsonPathProject: " + jsonPathProject);
        Debug.Log("jsonPathPersistant: " + jsonPathPersistant);
        Debug.Log("binaryPath: " + binaryPath);

     
    }


    #region || -------- General Section -------- ||


    #region || -------- Saving -------- ||



    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();


        data.enviromentData = GetEnviromentData();

        SavingTypeSwitch(data, slotNumber);
    }




    private EnviromentData GetEnviromentData()
    {
        List<string> itemsPickedup = InventorySystem.Instance.itemsPickedup;

        return new EnviromentData(itemsPickedup);
    }



    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentCalories;
        playerStats[2] = PlayerState.Instance.currentHydrationPercent;

        float[] playerPosAndRot = new float[6];
        playerPosAndRot[0] = PlayerState.Instance.playerBody.transform.position.x;
        playerPosAndRot[1] = PlayerState.Instance.playerBody.transform.position.y;
        playerPosAndRot[2] = PlayerState.Instance.playerBody.transform.position.z;

        playerPosAndRot[3] = PlayerState.Instance.playerBody.transform.rotation.x;
        playerPosAndRot[4] = PlayerState.Instance.playerBody.transform.rotation.y;
        playerPosAndRot[5] = PlayerState.Instance.playerBody.transform.rotation.z;

        string[] inventory = InventorySystem.Instance.itemList.ToArray();


        string[] quickSlots = GetQuickSlotsContent();



        return new PlayerData(playerStats, playerPosAndRot, inventory, quickSlots);

    }

    private string[] GetQuickSlotsContent()
    {
        List<string> temp = new List<string>();


        foreach(GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if( slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string cleanName = name.Replace(str2, "");
                temp.Add(cleanName);
            }
        }

        return temp.ToArray();
    }








    public void SavingTypeSwitch(AllGameData gameData, int slotNumber)
    {
        if (isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData, slotNumber);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData, slotNumber);
        }
    }

    #endregion

    #region || -------- Loading -------- ||

    public AllGameData LoadingTypeSwitch(int slotNumber)
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile(slotNumber);
            if (gameData == null)
            {
                Debug.LogError($"JSON dosyas�ndan oyun verileri y�klenemedi: {jsonPathProject + fileName + slotNumber + ".json"}");
            }
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromBinaryFile(slotNumber);
            if (gameData == null)
            {
                Debug.LogError($"�kili dosyadan oyun verileri y�klenemedi: {binaryPath + fileName + slotNumber + ".bin"}");
            }
            return gameData;
        }
    }





    public void LoadGame(int slotNumber)
    {
        var gameData = LoadingTypeSwitch(slotNumber);
        if (gameData == null)
        {
            Debug.LogError("Oyun verileri y�klenemedi.");
            return;
        }

        // Oyuncu verileri
        SetPlayerData(gameData.playerData);

        // �evre verileri
        SetEnviromentData(gameData.enviromentData);

        isLoading = false;
    }



    private void SetEnviromentData(EnviromentData enviromentData)
    {
        foreach( Transform itemType in EnviromentManager.Instance.allItems.transform)
        {
            foreach(Transform item in itemType.transform)
            {
                if(enviromentData.pickedupItems.Contains(item.name))
                {
                    Destroy(item.gameObject);
                }
            }
        }

        InventorySystem.Instance.itemsPickedup = enviromentData.pickedupItems;




    }


    private void SetPlayerData(PlayerData playerData)
    {

        //Setting Player stats

        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentCalories = playerData.playerStats[1];
        PlayerState.Instance.currentHydrationPercent = playerData.playerStats[2];


        // setting player position

        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];

        PlayerState.Instance.playerBody.transform.position = loadedPosition;

        // Setting player rotation

        Vector3 loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];

        PlayerState.Instance.playerBody.transform.rotation = Quaternion.Euler(loadedRotation);



        //setting the inventory content

        foreach(string item in playerData.inventoryContent)
        {
            InventorySystem.Instance.AddToInventory(item);
        }

        foreach(string item in playerData.quickSlotsContent)
        {
            GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();

            var itemToAdd = Instantiate(Resources.Load<GameObject>(item));

            itemToAdd.transform.SetParent(availableSlot.transform, false);
        }








        isLoading = false;

    }

    public void StartLoadedGame(int slotNumber)
    {

        



        isLoading = true;



        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayedLoading(slotNumber));


    }

    private IEnumerator DelayedLoading(int slotNumber)
    {

        yield return new WaitForSeconds(1f);

        LoadGame(slotNumber);

    }

    #endregion

    #endregion


    #region || -------- To Binary Section -------- ||

    public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber)
    {

        BinaryFormatter formatter = new BinaryFormatter();

        
        FileStream stream = new FileStream(binaryPath + fileName + slotNumber + ".bin", FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to" + binaryPath + fileName + slotNumber + ".bin");

    }


    public AllGameData LoadGameDataFromBinaryFile(int slotNumber)
    {
        string filePath = binaryPath + fileName + slotNumber + ".bin";
        if (!File.Exists(filePath))
        {
            Debug.LogError("�kili dosya bulunamad�: " + filePath);
            return null;
        }

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                AllGameData data = formatter.Deserialize(stream) as AllGameData;
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�kili dosyay� y�klerken hata: " + e.Message);
            return null;
        }
    }



    #endregion

    #region || -------- To Json Section -------- ||

    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {
        string json = JsonUtility.ToJson(gameData);

        //string encrypted = EncryptionDecryption(json);

        using (StreamWriter writer = new StreamWriter(jsonPathProject + fileName + slotNumber + ".json"))
        {
            writer.Write(json);
            print("Saved Game to Json file at :" + jsonPathProject + fileName + slotNumber + ".json");
        };
        

    }


    public AllGameData LoadGameDataFromJsonFile(int slotNumber)
    {
        string filePath = jsonPathProject + fileName + slotNumber + ".json";
        if (!File.Exists(filePath))
        {
            Debug.LogError("JSON dosyas� bulunamad�: " + filePath);
            return null;
        }

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                // string decrypted = EncryptionDecryption(json);
                AllGameData data = JsonUtility.FromJson<AllGameData>(json);
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON dosyas�n� y�klerken hata: " + e.Message);
            return null;
        }
    }



    #endregion


    #region || -------- Setting Section -------- ||

    #region || -------- Volume Settings -------- ||
    [System.Serializable]
    public class VolumeSettings
    {
        public float music;
        public float effects;
        public float master;
    }

    public void SaveVolumeSettings(float _music, float _effects, float _master)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        {
            music = _music,
            effects = _effects,
            master = _master
        };

        PlayerPrefs.SetString("Volume", JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();

        print("Saved to Player Pref.");
    }

    public VolumeSettings LoadVolumeSettings()
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
        
    }

    #endregion
    #endregion


    #region || -------- Encryption -------- ||


    public string EncryptionDecryption(string jsonString)
    {
        string keyword = "1234567";

        string result = "";

        for (int i =0; i< jsonString.Length; i++)
        {
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);

        }

        return result;
    }


    #endregion





    #region || -------- Utility -------- ||


    public bool DoesFileExists(int slotNumber)
    {
        if (isSavingToJson)
        {
            if(System.IO.File.Exists(jsonPathProject + fileName + slotNumber + ".json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if(System.IO.File.Exists(binaryPath + fileName + slotNumber + ".bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsSlotEmpty(int slotNumber)
    {
        if (DoesFileExists(slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }



    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);

    }

    #endregion




}
