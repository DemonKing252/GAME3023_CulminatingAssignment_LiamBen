using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Goal game-loop is the following:
//Player's turn begins ->   Player makes move ->                      dialog plays and Player's move animation plays ->       once both dialog and animation finished ->        Enemy plays receiving animation (dodging or receiving damage happens here) -> Enemy makes move -> dialog plays and Enemy's move animation plays -> once both dialog and animation finished -> Player plays receiving animation (dodging or receiving damage happens here). Repeat until one escapes or dies.


public static class AbilityDamageColor
{
    public static Color attackNormal = new Color(1f, 0f, 0f, 1f);
    public static Color attackSpecial = new Color(0.6f, 0f, 0f, 1f);
    public static Color dodge = new Color(0f, 1f, 1f, 1f);
    public static Color heal = new Color(0f, 1f, 0.2f, 1f);

}



public class MasterBattleManager : MonoBehaviour
{


    [SerializeField]
    public CombatAttributeModifier ScareRef;

    [SerializeField]
    public CombatAttributeModifier GlueRef;

    [SerializeField]
    public CombatAttributeModifier TrickRef;

    [SerializeField]
    public CombatAttributeModifier BleederRef;

    [SerializeField]
    public CombatAttributeModifier DefaultBehaviourRef;

    [SerializeField]
    private GameObject BattleUIRef;
    [SerializeField]
    private Image backgroundImageRef;

    public int turnsUntilPlayerSpecialAllowed = 0;

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

    public bool playerTurn = true;

    public AudioSource attackNormal;
    public AudioSource attackSpecial;
    public AudioSource dodge;
    public AudioSource heal;


    public void Start()
    {
        enemyActionThisTurn = choiceAction.unassigned;
        playerActionThisTurn = choiceAction.unassigned;
    }

    //STEP BOOT
    public void BootUp()
    {
        GetComponent<PlayerInput>().SetPlayerButtonsClickable(false);
        StartCoroutine(FirstTurnPlayer(2));
    }

    IEnumerator FirstTurnPlayer(float amountToDelay)
    {

        

        yield return new WaitForSeconds(1.5f);

        DialogueManager.GetInstance().StartNewDialogue("The battle begins! What will you do?");

        yield return new WaitForSeconds(2.0f);
        GetComponent<PlayerInput>().SetPlayerButtonsClickable(true);

    }

    //STEP SETUP  A new battle has begun. Setup some standard stuff before we officially begin!
    public void SetUpNewBattle(GameObject passedEnemyRef, Encounter passedEncounter)
    {
       
        initialEncounter = passedEncounter;
        enemyCombatAttributes = enemyRef.GetComponent<CombatAttributes>();
        playerCombatAttributes = playerRef.GetComponent<CombatAttributes>();

        if (OurAudioSource.instance != null)
            OurAudioSource.instance.ChangeTrack(Track.Battle);

        enemyStatsCanvas.GetComponentInChildren<HandleHearts>().SetPlayerRef(enemyRef);
        enemyStatsCanvas.SetActive(true);
        playerStatsCanvas.SetActive(true);

        enemyRef = passedEnemyRef;
        enemyImage.enabled = true;

        enemyResponseRef = GetComponent<EnemyResponse>();
        enemyResponseRef.enemyRef = enemyRef;
        overworldGameGUI.gameObject.SetActive(false);
        BeginPlayerTurn();
    }



    //STEP 01 Game awaits player to select an action. Step 2 is triggered by player pressing an action button.
    public void BeginPlayerTurn()
    {
        GetComponent<PlayerInput>().SetPlayerButtonsClickable(true);
        playerActionThisTurn = choiceAction.unassigned;
    }


    //STEP 02 Triggered by player pressing an action button, we now begin to play the animation and text of that chosen action! At the same time we automatically go to stage 3 through the start of the delayEndOfPlayerTurn coroutine so that stage 4 can begin after the anim and text complete
    public void PlayerChoseAction(choiceAction actionOfChoice)
    {
        playerActionThisTurn = actionOfChoice;      //save the selected action the player chose. We may need it for later!
        


        if(playerActionThisTurn == choiceAction.flee)
        {
            if (Random.Range(0.0f, 1.0f) <= playerRef.GetComponent<CombatAttributes>().GetSuccessFleeingChance())    //if a random float between 0 inclusive and 1 inclusive is LESS OR EQUAL to successFleeingChance then fleeing is considered a success and battle should end.
                playerActionThisTurn = choiceAction.flee;
            else
                playerActionThisTurn = choiceAction.fleeFail;
        }

        switch (playerActionThisTurn)     //go to the corresponding function for handling each action
        {
            case choiceAction.attack:
                {
                    //attackNormal.Play();
                    //playerAnim.SetTrigger("Normal"); //NEED NEW ANIMATION HERE
                    DialogueManager.GetInstance().StartNewDialogue("Player Attacks!");
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
            case choiceAction.heal:
                {
                    heal.Play();
                    playerAnim.SetTrigger("Heal");
                    playerCombatAttributes.IncreaseHealth(playerCombatAttributes.GetHealAmount());
                    DialogueManager.GetInstance().StartNewDialogue("Player Heals!");
                    playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.heal);
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
            case choiceAction.dodge:
                {
                    //dodge.Play();
                    //playerAnim.SetTrigger("Dodge"); //NEED NEW ANIMATION HERE
                    DialogueManager.GetInstance().StartNewDialogue("Player Prepares To Dodge!");
                    playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.dodge);
                    playerCombatAttributes.SetAttemptDodgeAttack(true);
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
            case choiceAction.fleeFail:
                {
                    dodge.Play();
                    DialogueManager.GetInstance().StartNewDialogue("Player Flee Failed!");
                    StartCoroutine(delayEndOfPlayerTurn(1));
                    break;
                }
            case choiceAction.flee:
                {
                    dodge.Play();
                    DialogueManager.GetInstance().StartNewDialogue("Player Flees Successfully!");
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    ShutdownBattle();
                    break;
                }
            case choiceAction.specialScare:
                {
                    attackSpecial.Play();
                    playerAnim.SetTrigger("Normal"); //NEED NEW ANIMATION HERE
                    playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.dodge);


                    turnsUntilPlayerSpecialAllowed = 2;
                    enemyRef.GetComponent<CombatAttributes>().behaviourModifier = ScareRef;
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    DialogueManager.GetInstance().StartNewDialogue("Player SCARES the enemy! Enemy will try to run away this turn!");
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
            case choiceAction.specialGlue:
                {
                    attackNormal.Play();
                    playerAnim.SetTrigger("Normal"); //NEED NEW ANIMATION HERE
                    playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.attackNormal);


                    turnsUntilPlayerSpecialAllowed = 3;
                    enemyRef.GetComponent<CombatAttributes>().behaviourModifier = GlueRef;
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    DialogueManager.GetInstance().StartNewDialogue("Player GLUES the enemy! Enemy cannot dodge for next 3 turns!");
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
            case choiceAction.specialTrick:
                {
                    heal.Play();
                    playerAnim.SetTrigger("Heal"); //NEED NEW ANIMATION HERE
                    playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.heal);

                    turnsUntilPlayerSpecialAllowed = 3;
                    enemyRef.GetComponent<CombatAttributes>().behaviourModifier = TrickRef;
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    DialogueManager.GetInstance().StartNewDialogue("Player Tricks the enemy! Enemy will try to dodge for next 3 turns!");
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
            case choiceAction.specialBleeder:
                {

                    attackSpecial.Play();
                    playerAnim.SetTrigger("Normal"); //NEED NEW ANIMATION HERE
                    playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.attackSpecial);

                    turnsUntilPlayerSpecialAllowed = 3;
                    enemyRef.GetComponent<CombatAttributes>().behaviourModifier = BleederRef;
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    DialogueManager.GetInstance().StartNewDialogue("Player causes the enemy to bleed! Enemy cannot heal for next 3 turns!");
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
            default:
                {
                    StartCoroutine(delayEndOfPlayerTurn(2));
                    break;
                }
        }


    }


    //STEP 03 Game waits while text is displayed and animation of player's action is played from step 2. Next step begins with BeginEnemyTurn and is run after waiting.
    IEnumerator delayEndOfPlayerTurn(float amountToDelay)
    {
        GetComponent<PlayerInput>().SetPlayerButtonsClickable(false); //player action buttons no longer clickable
        //Debug.Log("Started Player Wait");
        yield return new WaitForSeconds(amountToDelay);
        //Debug.Log("Finished Player Wait");
        BeginEnemyTurn();
    }





    //STEP 04a Enemy begins by attempting to dodge. Step 04b EnemyTryDodge() is where the enemy checks if it is allowed to dodge this turn. If so, the rest of the enemy's turn is skipped. If no dodging, we continue.
    public void BeginEnemyTurn()
    {

        //playerActionThisTurn is used to tell the enemy how to react to the player's turn. If player dodges, we don't want the enemy to automatically dodge aswell. Therefore, we set the enemy's response via the var playerActionThisTurn to be none under this circumstance
        if (playerActionThisTurn == choiceAction.dodge || playerActionThisTurn == choiceAction.dodgeFail)
        {
            playerActionThisTurn = choiceAction.none;
        }

        Debug.Log(enemyRef.GetComponent<CombatAttributes>().behaviourModifier.dodgeConsideration);

        switch (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.dodgeConsideration)     //go to the corresponding function for handling each action
        {
            case modifierStates.neverBehaviour:
                {
                    break;
                }
            case modifierStates.defaultBehaviour:
                {
                    EnemyTryDodgeDiceRoll();
                    break;
                }
            case modifierStates.certaintyBehaviour:
                {
                    EnemyDodgeSuccessDiceRoll();
                    break;
                }
        }
        

        enemyActionThisTurn = enemyResponseRef.EnemyRespond();  //got to enemyRespond and fid out enemy's move to make
                                                                ////Debug.Log("ENEMYRESPOND SAYS: " + enemyActionThisTurn);



        PlayerActionConsequencesForEnemy(); //enemy now knows what its turn will be. Still, we gotta actually go through the motions of the conseqeunces of the player's turn
    }


    //STEP 04b
    public void EnemyTryDodgeDiceRoll()
    {

        float diceRoll = Random.Range(0.0f, 1.0f);
        //Debug.Log("Dice roll 1: " + diceRoll + " <= " + enemyRef.GetComponent<CombatAttributes>().likelihoodOfDodgeAttempt);
        if (diceRoll <= enemyRef.GetComponent<CombatAttributes>().likelihoodOfDodgeAttempt)
        {
            EnemyDodgeSuccessDiceRoll();
        }
        else
        {
            //Debug.Log("NO DODGE THIS TURN");
            return;
        }

    }

    //STEP 04c
    private void EnemyDodgeSuccessDiceRoll()
    {
        if (enemyRef.GetComponent<CombatAttributes>().RollDiceForDodgeSuccess())   //We're dodging! Lets see if the dodge works or not!
        {
            //Debug.Log("Dice roll for success dodge passed");
            playerActionThisTurn = choiceAction.dodge;
        }
        else
        {
            //Debug.Log("Dice roll for success dodge failed");
            playerActionThisTurn = choiceAction.dodgeFail;
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
                        attackNormal.Play();
                        enemyAnim.SetTrigger("Normal");
                        enemyImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.attackNormal);
                        DialogueManager.GetInstance().StartNewDialogue("Enemy Takes Normal Damage!");
                    }
                    else
                    {
                        attackNormal.Play();
                        enemyImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.attackNormal);
                        enemyAnim.SetTrigger("Normal");
                        GetComponent<PlayerInput>().UnlockSpecialAbility();
                        DialogueManager.GetInstance().StartNewDialogue("Enemy Takes FATAL Damage!");
                        ShutdownBattle();
                    }
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    StartCoroutine(delayStartOfEnemyTurn(2.0f));
                    break;
                }
            case choiceAction.dodge:
                {
                    dodge.Play();
                    enemyAnim.SetTrigger("Dodge");
                    DialogueManager.GetInstance().StartNewDialogue("Enemy dodges!");
                    enemyImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.dodge);
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    StartCoroutine(delayStartOfEnemyTurn(2.0f));
                    break;
                }
            case choiceAction.dodgeFail:
                {
                    if (enemyCombatAttributes.DecreaseHealth(playerCombatAttributes.GetDamageDealNormal()))
                    {
                        DialogueManager.GetInstance().StartNewDialogue("Enemy failed to dodge!");
                        attackNormal.Play();
                        enemyAnim.SetTrigger("Normal");
                        enemyImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.attackNormal);
                    }
                    else
                    {
                        DialogueManager.GetInstance().StartNewDialogue("Enemy failed to dodge and takes FATAL Damage!");
                        GetComponent<PlayerInput>().UnlockSpecialAbility();
                        ShutdownBattle();
                    }
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    StartCoroutine(delayStartOfEnemyTurn(2.0f));
                    break;
                }
            case choiceAction.heal:
                {
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    StartCoroutine(delayStartOfEnemyTurn(0.1f));
                }
                break;
            case choiceAction.none:
                {
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    StartCoroutine(delayStartOfEnemyTurn(0.1f));
                    break;
                }
            default:
                {
                    playerActionThisTurn = choiceAction.unassigned; //reset player action back to unassigned so it can be set during their turn
                    StartCoroutine(delayStartOfEnemyTurn(2));
                    break;
                }
        }
    }

    IEnumerator delayStartOfEnemyTurn(float amountToDelay)
    {
        //Debug.Log("Started Start of enem turn Wait");
        yield return new WaitForSeconds(amountToDelay);
        //Debug.Log("Finished Start of enem turn Wait");
        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() <= 0)
            yield break;

        switch (enemyActionThisTurn)
        {
            case choiceAction.attack:
                {
                    DialogueManager.GetInstance().StartNewDialogue("Enemy Attacks Normal!");
                    StartCoroutine(delay2EndOfEnemyTurn(2));
                    break;
                }
            case choiceAction.heal:
                {
                    heal.Play();
                    enemyAnim.SetTrigger("Heal");
                    enemyCombatAttributes.IncreaseHealth(playerCombatAttributes.GetHealAmount());
                    DialogueManager.GetInstance().StartNewDialogue("Enemy Heals!");
                    enemyImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.heal);
                    StartCoroutine(delay2EndOfEnemyTurn(1));
                    break;
                }
            case choiceAction.flee:
                {
                    DialogueManager.GetInstance().StartNewDialogue("Enemy Flees!");
                    ShutdownBattle();
                    break;
                }
            case choiceAction.fleeFail:
                {
                    DialogueManager.GetInstance().StartNewDialogue("Enemy tries to flee but fails!");
                    StartCoroutine(delay2EndOfEnemyTurn(2));
                    break;
                }
        }

 

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
        enemyRef.GetComponent<CombatAttributes>().behaviourModifier.decrementNumOfTurns();
        
        
        switch (enemyActionThisTurn)
        {
            case choiceAction.attack:
                {
                    if (playerCombatAttributes.GetattemptDodgeAttack())
                    {
                        dodge.Play();
                        playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.dodge);
                        playerAnim.SetTrigger("Dodge");
                        DialogueManager.GetInstance().StartNewDialogue("Player Dodges Enemy's Attack!");
                    }
                    else
                    {
                        if (playerCombatAttributes.DecreaseHealth(enemyCombatAttributes.GetDamageDealNormal()))
                        {
                            attackNormal.Play();
                            playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.attackNormal);
                            playerAnim.SetTrigger("Normal");
                            DialogueManager.GetInstance().StartNewDialogue("Player Takes Normal Damage!");
                        }
                        else
                        {
                            attackNormal.Play();
                            playerImage.GetComponentInChildren<UIParticleBurst>().StartBurst(AbilityDamageColor.attackNormal);
                            playerAnim.SetTrigger("Normal");
                            DialogueManager.GetInstance().StartNewDialogue("Player Takes FATAL Damage!");
                            ShutdownBattle();
                        }
                    }
                    playerCombatAttributes.SetAttemptDodgeAttack(false);
                    enemyActionThisTurn = choiceAction.unassigned;
                    StartCoroutine(delayEndOfEnemyTurn(2));
                    break;
                }
            default:
                {
                    playerCombatAttributes.SetAttemptDodgeAttack(false);
                    enemyActionThisTurn = choiceAction.unassigned;
                    StartCoroutine(delayEndOfEnemyTurn(0.01f));
                    break;
                }
        }





        
    }

    //STEP 07 Add a delay so that the enemy text and animation from step 06 have time to go through their motions.
    IEnumerator delayEndOfEnemyTurn(float amountToDelay)
    {

        if (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.numberOfTurnsAffectedByModifier <= 0)
            enemyRef.GetComponent<CombatAttributes>().behaviourModifier = DefaultBehaviourRef;

        if (turnsUntilPlayerSpecialAllowed > 0)
            turnsUntilPlayerSpecialAllowed--;
        //Debug.Log("Started Wait");
        yield return new WaitForSeconds(amountToDelay);
        //Debug.Log("Finished Wait");

        DialogueManager.GetInstance().StartNewDialogue("What will you do?");
        yield return new WaitForSeconds(1.5f);
        BeginPlayerTurn();
    }


    public void ShutdownBattle()
    {
        StopAllCoroutines();                    //stop all coroutines (timer for enemy)
        enemyImage.enabled = false;             //stop rendering enemy image
        enemyStatsCanvas.SetActive(false);      //stop rendering enemy stats canvas
        StartCoroutine(ShutdownWaiter());
    }


    IEnumerator ShutdownWaiter()
    {
        yield return new WaitForSeconds(3);
        DialogueManager.GetInstance().StartNewDialogue("The battle is over!");
        yield return new WaitForSeconds(2);



        GetComponent<PlayerInput>().SetPlayerButtonsClickable(false);       //disable buttons clicability
        sceneTransitionOut.SetTrigger("Exit");  //play exit anim

        if (OurAudioSource.instance != null)
            OurAudioSource.instance.ChangeTrack(Track.OverworldFromBattle);

        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() <= 0f)
        {
            // Switch back to the original camera, since this object wont call OnExit, since its getting destroyed, 
            // we need to switch camera priorities before destroying the battle.


            initialEncounter.camera.Priority -= 2;
            // Wait for the battle to shutdown before destroying the references used in this script
            Destroy(initialEncounter.gameObject, 0.5f);
        }
    }

    public void OnDialogueEnded()
    {
        //Debug.Log("Dialogue ended.");
    }
}
