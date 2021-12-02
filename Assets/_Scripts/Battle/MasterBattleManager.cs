using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Goal game-loop is the following:
//Player's turn begins ->   Player makes move ->                      dialog plays and Player's move animation plays ->       once both dialog and animation finished ->        Enemy plays receiving animation (dodging or receiving damage happens here) -> Enemy makes move -> dialog plays and Enemy's move animation plays -> once both dialog and animation finished -> Player plays receiving animation (dodging or receiving damage happens here). Repeat until one escapes or dies. Dodging
//SetUpNewBattle            PlayerInput.cs -> PlayerChoseAction      EX:  PlayerAttack                                      delayEndOfPlayerTurn(2, playerActionThisTurn)   

public class MasterBattleManager : MonoBehaviour
{



    //player stuffs
    [SerializeField]
    public GameObject playerRef;
    public CombatAttributes playerCombatAttributes;
    [SerializeField]
    public Animator playerAnim;
    private choiceAction playerActionThisTurn = choiceAction.unassigned;
    [SerializeField]
    private Image playerImage;
    [SerializeField]
    private GameObject playerStatsCanvas;

    //enemy stuffs
    public CombatAttributes enemyCombatAttributes;
    public GameObject enemyRef;
    [SerializeField]
    public Animator enemyAnim;
    private choiceAction enemyActionThisTurn;
    public EnemyResponse enemyResponseRef;
    [SerializeField]
    private Image enemyImage;

    [SerializeField]
    private GameObject enemyStatsCanvas;



    public Encounter initialEncounter;




    [SerializeField]
    private GameObject overworldGameGUI;

    [SerializeField]
    Animator sceneTransitionOut;

    //turn information
    public bool playerTurn = true;



    public void Start()
    {
        enemyActionThisTurn = choiceAction.unassigned;
        playerActionThisTurn = choiceAction.unassigned;
    }

    //STEP SETUP  A new battle has begun. Setup some standard stuff before we officially begin!
    public void SetUpNewBattle(GameObject passedEnemyRef, Encounter passedEncounter)
    {
        //assign some stuff
        enemyRef = passedEnemyRef;
        enemyCombatAttributes = enemyRef.GetComponent<CombatAttributes>();
        initialEncounter = passedEncounter;
        playerCombatAttributes = playerRef.GetComponent<CombatAttributes>();
        enemyResponseRef = GetComponent<EnemyResponse>();
        enemyResponseRef.enemyRef = enemyRef;
        enemyStatsCanvas.SetActive(true);       //stop rendering enemy stats canvas
        enemyStatsCanvas.GetComponentInChildren<HandleHearts>().SetPlayerRef(enemyRef);
        playerStatsCanvas.SetActive(true);       //stop rendering enemy stats canvas
        //first off, disable the player's overworld hearts UI
        overworldGameGUI.gameObject.SetActive(false);
        BeginPlayerTurn();

    }



    //STEP 01 Game awaits player to select an action. Step 2 is triggered by player pressing an action button.
    public void BeginPlayerTurn()
    {
        GetComponent<PlayerInput>().SetPlayerButtonsClickable(true);
        playerActionThisTurn = choiceAction.unassigned;
        playerActionThisTurn = choiceAction.unassigned;
    }


    //STEP 02 Triggered by player pressing an action button, we now begin to play the animation and text of that chosen action! At the same time we automatically go to stage 3 through the start of the delayEndOfPlayerTurn coroutine so that stage 4 can begin after the anim and text complete
    public void PlayerChoseAction(choiceAction actionOfChoice)
    {
        playerActionThisTurn = actionOfChoice;      //save the selected action the player chose. We may need it for later!
        GetComponent<PlayerInput>().SetPlayerButtonsClickable(false); //player action buttons no longer clickable


        switch (actionOfChoice)     //go to the corresponding function for handling each action
        {
            case choiceAction.attack:
                {
                    playerAnim.SetTrigger("Heal"); //NEED NEW ANIMATION HERE
                    DialogueManager.GetInstance().StartNewDialogue("Player Attacks!");
                    break;
                }
            case choiceAction.special:
                {
                    enemyAnim.SetTrigger("Normal"); //NEED NEW ANIMATION HERE
                    DialogueManager.GetInstance().StartNewDialogue("Player Attacks Special!");
                    break;
                }
            case choiceAction.heal:
                {
                    playerAnim.SetTrigger("Heal");
                    playerCombatAttributes.IncreaseHealth(playerCombatAttributes.GetHealAmount());
                    DialogueManager.GetInstance().StartNewDialogue("Player Heals!");
                    break;
                }
            case choiceAction.dodge:
                {
                    //playerAnim.SetTrigger("Heal"); //NEED NEW ANIMATION HERE
                    DialogueManager.GetInstance().StartNewDialogue("Player Prepares To Dodge!");
                    playerCombatAttributes.SetAttemptDodgeAttack(true);
                    break;
                }
            default:
                {
                    break;
                }
        }


        StartCoroutine(delayEndOfPlayerTurn(2));
    }


    //STEP 03 Game waits while text is displayed and animation of player's action is played from step 2. Next step begins with BeginEnemyTurn and is run after waiting.
    IEnumerator delayEndOfPlayerTurn(float amountToDelay)
    {
        //Debug.Log("Started Player Wait");
        yield return new WaitForSeconds(amountToDelay);
        //Debug.Log("Finished Player Wait");
        BeginEnemyTurn();
    }





    //STEP 04a Enemy begins by attempting to dodge. Step 04b EnemyTryDodge() is where the enemy checks if it is allowed to dodge this turn. If so, the rest of the enemy's turn is skipped. If no dodging, we continue.
    public void BeginEnemyTurn()
    {
        if (enemyRef.GetComponent<CombatAttributes>().canDodge) //enemy will always try dodging!
        {
            ////Debug.Log("ENEMY CHECKING IF CAN DODGE");
            EnemyTryDodge();
        }

        ////Debug.Log("ENEMY DESCIDING ON ACTION!");
        enemyActionThisTurn = enemyResponseRef.EnemyRespond();  //got to enemyRespond and fid out enemy's move to make
                                                                ////Debug.Log("ENEMYRESPOND SAYS: " + enemyActionThisTurn);

        ////Debug.Log("ENEMY AFTER DESCIDING ON ACTION WANTS TO: " + enemyActionThisTurn);

        PlayerActionConsequencesForEnemy(); //enemy now knows what its turn will be. Still, we gotta actually go through the motions of the conseqeunces of the player's turn
    }


    //STEP 04b
    public void EnemyTryDodge()
    {
        if (enemyRef.GetComponent<CombatAttributes>().RollDiceForDodgeAttempt()) //are we dodging at ALL this turn?
        {
            //Debug.Log("Enemy allowed to try to dodge!");
            if (enemyRef.GetComponent<CombatAttributes>().RollDiceForDodgeSuccess())   //We're dodging! Lets see if the dodge works or not!
            {
                playerActionThisTurn = choiceAction.dodge;
            }
            else
            {
                playerActionThisTurn = choiceAction.dodgeFail;
            }
        }
        else
        {
            //Debug.Log("Enemy attempt for dodge failed!");
        }
    }



    //STEP 05 Now we gotta display what happens to the enemy from the player's action
    public void PlayerActionConsequencesForEnemy()
    {
        switch (playerActionThisTurn)
        {
            case choiceAction.attack:
                {
                    if (enemyCombatAttributes.DecreaseHealth(playerCombatAttributes.GetDamageDealNormal()))
                    {
                        enemyAnim.SetTrigger("Normal");
                        DialogueManager.GetInstance().StartNewDialogue("Enemy Takes Normal Damage!");
                    }
                    else
                    {
                        DialogueManager.GetInstance().StartNewDialogue("Enemy Takes FATAL Damage!");
                        ShutdownBattle();
                    }
                    break;
                }
            case choiceAction.special:
                {
                    if (enemyCombatAttributes.DecreaseHealth(playerCombatAttributes.GetDamageDealSpecial()))
                    {
                        enemyAnim.SetTrigger("Normal");
                        DialogueManager.GetInstance().StartNewDialogue("Enemy Takes Special Damage!");
                    }
                    else
                    {
                        DialogueManager.GetInstance().StartNewDialogue("Enemy Takes FATAL Special Damage!");
                        ShutdownBattle();
                    }
                    break;
                }
            case choiceAction.dodge:
                {
                    //Debug.Log("TRYING DODGE");
                    enemyAnim.SetTrigger("Dodge");
                    DialogueManager.GetInstance().StartNewDialogue("Enemy dodges!");
                    break;
                }
            case choiceAction.dodgeFail:
                {
                    //Debug.Log("TRYING DODGEFAILED");
                    enemyAnim.SetTrigger("Dodge");

                    if (enemyCombatAttributes.DecreaseHealth(playerCombatAttributes.GetDamageDealSpecial()))
                    {
                        DialogueManager.GetInstance().StartNewDialogue("Enemy failed to dodge!");
                    }
                    else
                    {
                        DialogueManager.GetInstance().StartNewDialogue("Enemy Takes FATAL Special Damage!");
                        ShutdownBattle();
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }






        playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
        StartCoroutine(delayStartOfEnemyTurn(2));
    }

    IEnumerator delayStartOfEnemyTurn(float amountToDelay)
    {
        //Debug.Log("Started Start of enem turn Wait");
        yield return new WaitForSeconds(amountToDelay);
        //Debug.Log("Finished Start of enem turn Wait");

        switch (enemyActionThisTurn)
        {
            case choiceAction.attack:
                {
                    enemyAnim.SetTrigger("Heal");
                    DialogueManager.GetInstance().StartNewDialogue("Enemy Attacks Normal!");
                    break;
                }

        }

        StartCoroutine(delay2EndOfEnemyTurn(2));

    }


    IEnumerator delay2EndOfEnemyTurn(float amountToDelay)
    {
        //Debug.Log("Started Start of enem turn Wait");
        yield return new WaitForSeconds(amountToDelay);
        //Debug.Log("Finished Start of enem turn Wait");
        EnemyActionConsequencesForPlayer();
    }
    //STEP 06 Now we're free for the enemy to make their move. Enemy already knows what that move is, so we just have to apply it!
    public void EnemyActionConsequencesForPlayer()
    {
        switch (enemyActionThisTurn)
        {
            case choiceAction.attack:
                {
                    if (playerCombatAttributes.GetattemptDodgeAttack())
                    {
                        playerAnim.SetTrigger("Dodge");
                        DialogueManager.GetInstance().StartNewDialogue("Player Dodges Enemy's Attack!");
                        playerCombatAttributes.SetAttemptDodgeAttack(false);
                    }
                    else
                    {
                        if (playerCombatAttributes.DecreaseHealth(enemyCombatAttributes.GetDamageDealNormal()))
                        {
                            playerAnim.SetTrigger("Normal");
                            DialogueManager.GetInstance().StartNewDialogue("Player Takes Normal Damage!");
                        }
                        else
                        {
                            DialogueManager.GetInstance().StartNewDialogue("Player Takes FATAL Damage!");
                            ShutdownBattle();
                        }
                    }
                    break;



                }
            case choiceAction.special:
                {
                    if (enemyCombatAttributes.DecreaseHealth(playerCombatAttributes.GetDamageDealSpecial()))
                    {
                        playerAnim.SetTrigger("Normal");
                        DialogueManager.GetInstance().StartNewDialogue("Player Takes Special Damage!");
                    }
                    else
                    {
                        DialogueManager.GetInstance().StartNewDialogue("Player Takes FATAL Special Damage!");
                        ShutdownBattle();
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }





        playerCombatAttributes.SetAttemptDodgeAttack(false);
        enemyActionThisTurn = choiceAction.unassigned;
        StartCoroutine(delayEndOfEnemyTurn(2));
    }

    //STEP 07 Add a delay so that the enemy text and animation from step 06 have time to go through their motions.
    IEnumerator delayEndOfEnemyTurn(float amountToDelay)
    {
        //Debug.Log("Started Wait");
        yield return new WaitForSeconds(amountToDelay);
        //Debug.Log("Finished Wait");

        DialogueManager.GetInstance().StartNewDialogue("What will you do?");
        BeginPlayerTurn();
    }

























    public void ShutdownBattle()
    {
        OurAudioSource.instance.ChangeTrack(Track.OverworldFromBattle);
        StopAllCoroutines();                    //stop all coroutines (timer for enemy)
        //inBattle = false;                       //battle has stopped. This bool disallows further actions from taking place in wait coroutine.
        enemyImage.enabled = false;             //stop rendering enemy image
        enemyStatsCanvas.SetActive(false);      //stop rendering enemy stats canvas
        GetComponent<PlayerInput>().SetPlayerButtonsClickable(false);       //disable buttons clicability
        //sceneTransitionOut.SetTrigger("Exit");  //play exit anim
        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() <= 0f)
        {
            // Switch back to the original camera, since this object wont call OnExit, since its getting destroyed, 
            // we need to switch camera priorities before destroying the battle.

            initialEncounter.camera.Priority -= 2;
            // Wait for the battle to shutdown before destroying the references used in this script
            Destroy(initialEncounter.gameObject, 0.5f);
        }
    }

    ////public void ShutdownBattle()
////////    {
////////        OurAudioSource.instance.ChangeTrack(Track.OverworldFromBattle);
////////        StopAllCoroutines();                    //stop all coroutines (timer for enemy)
////////        inBattle = false;                       //battle has stopped. This bool disallows further actions from taking place in wait coroutine.
////////        enemyImage.enabled = false;             //stop rendering enemy image
////////        enemyStatsCanvas.SetActive(false);      //stop rendering enemy stats canvas
////////        SetPlayerButtonsClickable(false);       //disable buttons clicability
////////        playerRef.GetComponent<CombatAttributes>().SpecialWindUpReset();    //reset special wind-up count
////////        playerAttackSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Wind Up\nSpecial");    //reset special attack button to default text
////////        sceneTransitionOut.SetTrigger("Exit");  //play exit anim
////////        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() <= 0f)
////////        {
////////            // Switch back to the original camera, since this object wont call OnExit, since its getting destroyed, 
////////            // we need to switch camera priorities before destroying the battle.

    ////////            battleRef.camera.Priority -= 2;
    ////////            // Wait for the battle to shutdown before destroying the references used in this script
    ////////            Destroy(battleRef.gameObject, 0.5f);
    ////////        }
    ////////    }


    public void OnDialogueEnded()
    {
        //Debug.Log("Dialogue ended.");
    }
}
