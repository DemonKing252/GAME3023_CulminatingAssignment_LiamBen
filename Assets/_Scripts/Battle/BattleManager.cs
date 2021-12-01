using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private Button playerAttackSpecialButton;

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

    public Encounter battleRef;

    public void SetEnemyRef(GameObject go)
    {
        enemyRef = go;
    }


    public GameObject GetPlayerRef()
    {
        return playerRef;
    }


    #endregion

    #region turnInformation
    [Header("Turn Info")]
    [SerializeField]
    private float waitTimeForEnemyTurn = 1.5f;      //seconds to wait before enemy makes their move

    private bool playerTurn = true;

    private bool inBattle = true;
    #endregion

    #region animation

    [SerializeField]
    Animator sceneTransitionOut;

    // Mecanim animations
    [SerializeField]
    private Animator playerAnim;

    [SerializeField]
    private Animator enemyAnim;

    public Animator GetPlayerAnimator()
    {
        return playerAnim;
    }
    public Animator GetEnemyAnimator()
    {
        return enemyAnim;
    }
    #endregion

    #region startupShutdown

    void Start()
    {
        //first off, disable the player's overworld hearts UI
        overworldGameGUI.gameObject.SetActive(false);
        DialogueManager.GetInstance().dialogueEndEvent.AddListener(OnDialogueEnded);
    }
    public void StartUp()
    {
        playerTurn = true;
        inBattle = true;
        enemyImage.enabled = true;              //stop rendering enemy image
        enemyStatsCanvas.SetActive(true);       //stop rendering enemy stats canvas
        SetPlayerButtonsClickable(true);        //disable buttons clicability
    }
    public void ShutdownBattle()
    {
        StopAllCoroutines();                    //stop all coroutines (timer for enemy)
        inBattle = false;                       //battle has stopped. This bool disallows further actions from taking place in wait coroutine.
        enemyImage.enabled = false;             //stop rendering enemy image
        enemyStatsCanvas.SetActive(false);      //stop rendering enemy stats canvas
        SetPlayerButtonsClickable(false);       //disable buttons clicability
        playerRef.GetComponent<CombatAttributes>().SpecialWindUpReset();    //reset special wind-up count
        playerAttackSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Wind Up\nSpecial");    //reset special attack button to default text
        sceneTransitionOut.SetTrigger("Exit");  //play exit anim
        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() <= 0f)
        {
            // Switch back to the original camera, since this object wont call OnExit, since its getting destroyed, 
            // we need to switch camera priorities before destroying the battle.
            
            battleRef.camera.Priority -= 2;
            // Wait for the battle to shutdown before destroying the references used in this script
            Destroy(battleRef.gameObject, 0.5f);
        }
    }

    public void OnDialogueEnded()
    {
        Debug.Log("Dialogue ended.");
    }
    private bool quitAlready = false;
    void Update()
    {
        if (!quitAlready)
        {
            if (playerRef.GetComponent<CombatAttributes>().GetHealth() <= 0f)
            {
                playerRef.GetComponent<PlayerController>().LoseGame();
                quitAlready = true;
            }
        }
        
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

    #endregion

    #region PlayerInputs


    void SetPlayerButtonsClickable(bool newClickableState)
    {
        //set buttons interactability based on passed bool
        playerAttackButton.interactable = newClickableState;
        playerDodgeButton.interactable = newClickableState;
        playerFleeButton.interactable = newClickableState;
        playerAttackSpecialButton.interactable = newClickableState;

        //only set health button to be interactable if passed bool is true AND player is NOT at full health!
        if (newClickableState == true && !playerRef.GetComponent<CombatAttributes>().GetIsAtFullHealth())
            playerHealButton.interactable = true;
        else
            playerHealButton.interactable = false;
    }

    public void PlayerInputAttack()
    {
        if (!playerTurn) return;                //quick failsafe for if not the player turn, just return

        enemyAnim.SetTrigger("Normal");

        if (enemyRef.GetComponent<CombatAttributes>().DecreaseHealth(playerRef.GetComponent<CombatAttributes>().GetDamageDealNormal())) //decrease health of enemy by player's attack damage normal amount. DecreaseHealth returns a bool depicting if entity is alive, so if true (enemy is alive) then run coroutine as normal. If false, entity is dead so shutdown the battle scene.
            StartCoroutine(FinishPlayerTurn()); //this runs if enemy is not killed
        else
            ShutdownBattle();   //this runs if the enemy is killed.
    }

    public void PlayerInputAttackSpecial()
    {
        if (!playerTurn) return;                //quick failsafe for if not the player turn, just return

        if (playerRef.GetComponent<CombatAttributes>().GetSpecialAttackAllowed())
        {                               //attack is wound up! Ready to deal damage!
            enemyAnim.SetTrigger("Normal");         //play attack animation on the player
            if (enemyRef.GetComponent<CombatAttributes>().DecreaseHealth(playerRef.GetComponent<CombatAttributes>().GetDamageDealSpecial())) //decrease health of enemy by player's attack damage special amount. DecreaseHealth returns a bool depicting if entity is alive, so if true (enemy is alive) then run coroutine as normal. If false, entity is dead so shutdown the battle scene.
            {
                                        //this runs if enemy is not killed
                playerAttackSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Wind Up\nSpecial");
                DialogueManager.GetInstance().StartNewDialogue("Player Attacks Special");
                StartCoroutine(FinishPlayerTurn()); 
            }
            else
                ShutdownBattle();       //this runs if the enemy is killed by the attack
        }
        else                            //attack is not wound up. Player will sacrifce this turn in order to wind up their attack
        {
            playerAttackSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Attack\nSpecial");
            DialogueManager.GetInstance().StartNewDialogue("Player Winds Up!");
            playerRef.GetComponent<CombatAttributes>().WindUpSpecial();
            StartCoroutine(FinishPlayerTurn());
        }

    }

    public void PlayerInputDodge()
    {
        if (!playerTurn) return;                //quick failsafe for if not the player turn, just return
        playerAnim.SetTrigger("Dodge");         //play dodge animation on the player
        DialogueManager.GetInstance().StartNewDialogue("Player Dodges");
        playerRef.GetComponent<CombatAttributes>().SetAttemptDodgeAttack(true);
        StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputHeal()
    {
        if (!playerTurn) return;                //quick failsafe for if not the player turn, just return
        playerAnim.SetTrigger("Heal");          //play heal animation on the player
        DialogueManager.GetInstance().StartNewDialogue("Player heals");
        playerRef.GetComponent<CombatAttributes>().IncreaseHealth(playerRef.GetComponent<CombatAttributes>().GetHealAmount());  //heal player by their determined heal amount
        StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputFlee()
    {
        if (!playerTurn) return;                //quick failsafe for if not the player turn, just return
        PlayerAttemptFlee();
        StartCoroutine(FinishPlayerTurn());
    }

    private void PlayerAttemptFlee()            //player attempts to flee. Returns true if attempt succeeded, false if attempt failed.
    {

        if (Random.Range(0.0f, 1.0f) <= playerRef.GetComponent<CombatAttributes>().GetSuccessFleeingChance())    //if a random float between 0 inclusive and 1 inclusive is LESS OR EQUAL to successFleeingChance then fleeing is considered a success and battle should end.
        {
            DialogueManager.GetInstance().StartNewDialogue("Player escaped");
            ShutdownBattle();                   //stop the battle immediately
        }
        else
            DialogueManager.GetInstance().StartNewDialogue("Player failed to escape");
    }

    #endregion

}
