using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private HandleHearts enemyHeartAttributesRef;

    [SerializeField]
//    private EnemyResponse enemyResponse;

    public Encounter selectedEncounter;
    public Canvas encounterCanvas;
    public GameObject playerui; // disable overworld ui while the player is in this encounter.
    // Animation event



    void Start()
    {
    }
    public void ShowEncounter()
    {
        encounterCanvas.gameObject.SetActive(true);
//        FindObjectOfType<BattleManager>().StartUp();    // brute force approach


        playerui.SetActive(false);

//        enemyResponse.enemyRef = selectedEncounter.selectedEnemy;
//        enemyResponse.gameObject.GetComponent<BattleManager>().SetEnemyRef(selectedEncounter.selectedEnemy);
        enemyHeartAttributesRef.SetPlayerRef(selectedEncounter.selectedEnemy);
    }
    public void HideEncounter()
    {
        encounterCanvas.gameObject.SetActive(false);
        playerui.SetActive(true);

        GameObject[] gos = GameObject.FindGameObjectsWithTag("BattleEncounter");
        if (gos.Length == 0)
            FindObjectOfType<PlayerController>().WinGame();
    }
}
