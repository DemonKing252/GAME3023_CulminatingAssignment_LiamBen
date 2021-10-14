using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Position
{
    public float x;
    public float y;
    public Position(float x = 0f, float y = 0f)
    {
        this.x = x;
        this.y = y;
    }
}
[System.Serializable]
public class GameData
{
    public Position playerPosition;
    // Add more save features to this class
    // abilities will go here
    // health will go here
}
public static class PlayerPreference
{
    public static LoadType load = LoadType.Default;
}



public class GameDataManager : MonoBehaviour
{
    GameData gData = new GameData();
    PlayerController player;

    public static GameDataManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        player = FindObjectOfType<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveGame()
    {
        gData.playerPosition = new Position(player.transform.position.x, player.transform.position.y);
        PlayerPrefs.SetString("_GameData", JsonUtility.ToJson(gData));
        PlayerPrefs.Save();
    }
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("_GameData"))
        {
            SaveGame();
        }
        gData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString("_GameData"));
        player.transform.position = new Vector2(gData.playerPosition.x, gData.playerPosition.y);
    }
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
