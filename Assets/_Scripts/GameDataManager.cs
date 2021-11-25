using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    MaskGuy = 0,
    Ninja = 1,
    Pink = 2,
    VR = 3
}



// Vector2 and Vector3 are not serializable
[System.Serializable]
public class SerializedVector2
{
    public float x;
    public float y;
    public SerializedVector2(float x = 0f, float y = 0f)
    {
        this.x = x;
        this.y = y;
    }
    public SerializedVector2()
    {
        this.x = 0f;
        this.y = 0f;
    }
}


[System.Serializable]
public class EncounterAttributes
{
    public SerializedVector2 worldPosition = new SerializedVector2();
    public float health;

}

[System.Serializable]
public class EnemyCombatAttributes
{
    public float health = 0f;

    // Abilites go here
}



[System.Serializable]
public class GameData
{
    public SerializedVector2 playerPosition = new SerializedVector2();
    public float playerHealth;

    public List<EncounterAttributes> battleAttributes = new List<EncounterAttributes>();

    public List<EnemyCombatAttributes> enemyAttributes = new List<EnemyCombatAttributes>();

    // Add more save features to this class
    // abilities will go here
    // health will go here
    public GameData()
    {
    }
}



public static class PlayerPreference
{
    public static LoadType load = LoadType.Default;
    public static GameObject enemy;
}



public class GameDataManager : MonoBehaviour
{
    private const string keyCode = "_GameData";

    GameData gData = new GameData();
    PlayerController player;

    public CombatAttributes[] enemyAttributes = new CombatAttributes[4];

    // Prefabs
    public GameObject battlePrefab;

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
        gData.playerPosition = new SerializedVector2(player.transform.position.x, player.transform.position.y);
        gData.playerHealth = player.GetComponent<CombatAttributes>().GetHealth();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("BattleEncounter");

        //gData.enemyAttributes = new EnemyAttributes[enemies.Length];

        for(int i = 0; i < enemies.Length; i++)
        {
            EncounterAttributes e = new EncounterAttributes();
            e.worldPosition = new SerializedVector2(enemies[i].transform.position.x, enemies[i].transform.position.y);
            
            gData.battleAttributes.Add(e);
           
        }
        for (int i = 0; i < 4; i++)
        {
            EnemyCombatAttributes enemyCombatAttributes = new EnemyCombatAttributes();
            enemyCombatAttributes.health = enemyAttributes[i].GetHealth();

            gData.enemyAttributes.Add(enemyCombatAttributes);
        }

        PlayerPrefs.SetString(keyCode, JsonUtility.ToJson(gData));
        PlayerPrefs.Save();


        GameObject[] gos = GameObject.FindGameObjectsWithTag("BattleEncounter");
        foreach (var go in gos)
            Destroy(go);
    }
    public void LoadGame()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("BattleEncounter");
        foreach (var go in gos)
            Destroy(go);

        if (!PlayerPrefs.HasKey(keyCode))
        {
            SaveGame();
        }
        gData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString(keyCode));

        player.transform.position = new Vector2(gData.playerPosition.x, gData.playerPosition.y);
        player.GetComponent<CombatAttributes>().SetHealth(gData.playerHealth);

        int cntr = 0;
        foreach(var e in gData.battleAttributes)
        {
            GameObject go = Instantiate(battlePrefab, new Vector3(e.worldPosition.x, e.worldPosition.y), Quaternion.identity);
            go.name = "Encounter " + cntr.ToString();
            go.tag = "BattleEncounter";
            go.SetActive(true);
            cntr++;
        }
        for(int i = 0; i < 4; i++)
        {
            enemyAttributes[i].SetHealth(gData.enemyAttributes[i].health);
        }

        //Debug.Log("Serialized class: " + JsonUtility.ToJson(player.GetComponent<CombatAttributes>()));
        
    }
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
