using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager dungeonManager;
    [SerializeField] private GameObject AllTheDoors;
    [SerializeField] private GameObject AllTheChests;
    public bool[] doorBools = new bool[25];
    string currentDungeonName;
    private Dungeons dungeons;
    public bool clearAllData;

    void Awake()
    {
        currentDungeonName = SceneManager.GetActiveScene().name;

        Debug.Log(PlayerPrefs.GetInt("FirstTime"));
        
        if(clearAllData)
        {
            PlayerPrefs.SetInt("FirstTime", 0);
        }

        if(PlayerPrefs.GetInt("FirstTime") == 0)
        {
            dungeons = new Dungeons();
            dungeons.ClearData();
            SaveToJson();
            PlayerPrefs.SetInt("FirstTime", 1);
        }
        else
        {
            LoadFromJson();
        }
        
        doorBools = dungeons.GetDungeon(currentDungeonName).doors;

        OpenDoors();
        OpenChests();

    }

    public void SaveToJson()
    {
        string dungeonData = JsonUtility.ToJson(dungeons) ;
        string filePath = Application.persistentDataPath + "/Dungeons.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, dungeonData);
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/Dungeons.json";
        string dungeonData = System.IO.File.ReadAllText (filePath);
        dungeons = JsonUtility.FromJson<Dungeons>(dungeonData);
    }

    public void OpenDoors()
    {

        DungeonData data = dungeons.GetDungeon(currentDungeonName);

        for(int i = 0; i < AllTheDoors.transform.childCount; i++)
        {

            AllTheDoors.transform.GetChild(i).GetComponent<LockedDoor>().SetNumber(i);

            if(data.doors[i])
            {
                AllTheDoors.transform.GetChild(i).GetComponent<LockedDoor>().SetToOpen();
            }
            
        }

    }

    public void OpenChests()
    {
        DungeonData data = dungeons.GetDungeon(currentDungeonName);

        for(int i = 0; i < AllTheChests.transform.childCount; i++)
        {

            AllTheChests.transform.GetChild(i).GetComponent<Chest>().SetNumber(i);

            if(data.chests[i])
            {
                AllTheChests.transform.GetChild(i).GetComponent<Chest>().SetToOpen();
            }

        }
    }

    public void SaveChest(int i)
    {
        dungeons.SaveChest(currentDungeonName, i);
        SaveToJson();
    }

    public void SaveDoor(int i)
    {
        dungeons.SaveDoor(currentDungeonName, i);
        SaveToJson();
    }


    [System.Serializable]
    public class DungeonData
    {
        public string name;
        public bool[] doors = new bool[10];
        public bool[] chests = new bool[10];
        public bool boss = false;
        public bool miniboss = false;
    }

    [System.Serializable]
    public class Dungeons
    {
        public static DungeonData DungeonOne =  new DungeonData
        {
            name = "One"
        };

        public DungeonData[] DungeonDatas = {DungeonOne};

        public void ClearData()
        {
            for(int i=0; i < DungeonDatas.Length; i++)
            {
                AssignLevelBools(DungeonDatas[i].name);
            }
        }

        public void AssignLevelBools(string name)
        {

            DungeonData data = GetDungeon(name);
            int i = 0;
            data.boss = false;
            data.miniboss = false;

            while(i < 10)
            {
                data.doors[i] = false;
                data.chests[i] = false;
                i++;
            }

        }

        public DungeonData GetDungeon(string name)
        {
            foreach(DungeonData data in DungeonDatas)
            {
                if(data.name == name)
                {
                    return data;
                }
            }

            Debug.Log("name does not exist");

            return null;

        }

        public void SaveDoor(string levelName, int index)
        {
            GetDungeon(levelName).doors[index] = true;
        }

        public void SaveChest(string levelName, int index)
        {
            GetDungeon(levelName).chests[index] = true;
        }

    }
    


}
