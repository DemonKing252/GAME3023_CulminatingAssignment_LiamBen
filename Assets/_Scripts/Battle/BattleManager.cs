using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    #region references
    [Header("Refs")]
    [SerializeField]
    private GameObject playerRef;

    [SerializeField]
    private GameObject enemyRef;

    [SerializeField]
    private Image enemyImage;

    [SerializeField]
    private GameObject enemyStatsCanvas;

    [SerializeField]
    private Button playerAttackButton;

    [SerializeField]
    private Button playerDodgeButton;

    [SerializeField]
    private Button playerHealButton;

    [SerializeField]
    private Button playerFleeButton;

    [SerializeField]
    private GameObject overworldGameGUI;

    [SerializeField]
    private GameObject dialogBox;
    #endregion


    [SerializeField]
    private float waitTimeForEnemyTurn = 1.5f;

    private bool playerTurn = true;

    private bool inBattle = true;



    void Start()
    {
        //first off, disable the player's overworld hearts UI
        overworldGameGUI.gameObject.SetActive(false);
        
    }

    public void ShutdownBattle()
    {
        StopAllCoroutines();  //stop all coroutines (timer for enemy)
        Debug.Log("battle scene should close here!");
        inBattle = false;   //battle has stopped. This bool disallows further actions from taking place in wait coroutine.
        enemyImage.enabled = false; //stop rendering enemy image
        enemyStatsCanvas.SetActive(false);  //stop rendering enemy stats canvas
        SetPlayerButtonsClickable(false);   //disable buttons clicability
    }

    void SetPlayerButtonsClickable(bool newClickableState)
    {
        //set buttons interactability based on passed bool
        playerAttackButton.interactable = newClickableState;
        playerDodgeButton.interactable = newClickableState;
        playerFleeButton.interactable = newClickableState;

        //only set health button to be interactable if passed bool is true AND player is NOT at full health!
        if (newClickableState == true && !playerRef.GetComponent<CombatAttributes>().GetIsAtFullHealth())
            playerHealButton.interactable = true;
        else
            playerHealButton.interactable = false;
    }


    IEnumerator FinishPlayerTurn()
    {
        if (!playerTurn) yield break; //failsafe. This should never be run in the first place without playerTurn being true. If it breaks here then something has gone VERY wrong.

        //no longer players turn, set bool accordingly and disable buttons
        playerTurn = false;
        SetPlayerButtonsClickable(false);

        if (!inBattle) yield break; //if no longer in a battle, return. Stuff before this line would have to happen both for ending turn and ending the battle

        //add a little wait to enemy's actions so they're not instantanious
                    //NOTE: this is so dumb. If enemy successfully flees, I need the coroutine to stop and the shutdown func to start. However the enemy flee check completes AFTER the SetPlayerButtonsClickable() func completes if placed WaitForSeconds->EnemyRespond.
                    //If the Enemy response is done before the wait with EnemyRespond->WaitForSeconds, then we'd have to store the result of the enemy's descision(flee success, flee fail, attack, etc) and then apply it after the wait finished. But if we split the wait so the enemy
                    //flee check ALWAYS completes BEFORE both waits finish, we can skip having to add a delayed application of the enemy's descision.
                    //This is a temporary duct tape fix and I know it will make Joss cry.
        yield return new WaitForSeconds(waitTimeForEnemyTurn * 0.9f);
        GetComponent<EnemyResponse>().EnemyRespond();   //tell enemy to take its turn. If flee chosen and succeeds then only 90% of wait time will have been allowed to pass. Otherwise 100% will have been allowed to pass.
        yield return new WaitForSeconds(waitTimeForEnemyTurn * 0.1f);
        

        //player's turn once again! set bool accordingly and re-enable buttons
        playerTurn = true;
        SetPlayerButtonsClickable(true);

    }
    public void PlayerInputAttack()
    {
        if (!playerTurn) return; //quick failsafe

        if(enemyRef.GetComponent<CombatAttributes>().DecreaseHealth(playerRef.GetComponent<CombatAttributes>().GetDamageDealNormal())) //decrease health of enemy by player's attack damage normal amount. DecreaseHealth returns a bool depicting if entity is alive, so if true (enemy is alive) then run coroutine as normal. If false, entity is dead so shutdown the battle scene.
            StartCoroutine(FinishPlayerTurn()); //this runs if enemy is not killed
        else
            ShutdownBattle();   //this runs if the enemy is killed.
    }

    public void PlayerInputDodge()
    {
        if (!playerTurn) return; //quick failsafe


        DialogueManager.GetInstance().StartNewDialogue("Player Dodges");
        //Debug.Log("player Dodges");
        StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputHeal()
    {
        if (!playerTurn) return; //quick failsafe

        DialogueManager.GetInstance().StartNewDialogue("Player heals");
        //Debug.Log("player heals");
        playerRef.GetComponent<CombatAttributes>().IncreaseHealth(playerRef.GetComponent<CombatAttributes>().GetHealAmount());  //heal player by their determined heal amount

        StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputFlee()
    {
        if (!playerTurn) return; //quick failsafe


        //DialogueManager.GetInstance().StartNewDialogue("Player attempts to flee");
        //Debug.Log("player attempts to flee");
        PlayerAttemptFlee();
        StartCoroutine(FinishPlayerTurn());
    }

    private void PlayerAttemptFlee()     //player attempts to flee. Returns true if attempt succeeded, false if attempt failed.
    {

        if (Random.Range(0.0f, 1.0f) <= playerRef.GetComponent<CombatAttributes>().GetSuccessFleeingChance())    //if a random float between 0 inclusive and 1 inclusive is LESS OR EQUAL to successFleeingChance then fleeing is considered a success and battle should end.
        {
            DialogueManager.GetInstance().StartNewDialogue("Player escaped");
            //Debug.Log("Player escaped");
            ShutdownBattle();     //stop the battle immediately
        }
        else
        {

            DialogueManager.GetInstance().StartNewDialogue("Player failed to escape");
            //Debug.Log("Player failed to escape");
        }
    }
}
