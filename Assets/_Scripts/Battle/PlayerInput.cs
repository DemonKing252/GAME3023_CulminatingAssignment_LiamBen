using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum choiceAction
{
    attack,
    special,
    dodge,
    dodgeFail,
    heal,
    flee,
    none,
    unassigned
}
public class PlayerInput : MonoBehaviour
{
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


    #region PlayerInputs


    public void SetPlayerButtonsClickable(bool newClickableState)
    {

        //set buttons interactability based on passed bool
        playerAttackButton.interactable = newClickableState;
        playerDodgeButton.interactable = newClickableState;
        playerFleeButton.interactable = newClickableState;
        playerAttackSpecialButton.interactable = newClickableState;

        //only set health button to be interactable if passed bool is true AND player is NOT at full health!
        if (newClickableState == true && !GetComponent<MasterBattleManager>().playerRef.GetComponent<CombatAttributes>().GetIsAtFullHealth())
            playerHealButton.interactable = true;
        else
            playerHealButton.interactable = false;
    }

    public void PlayerInputAttack()
    {
        Debug.Log("Player Attack Pressed");
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.attack);
        //if (!masterBattleManagerRef.playerTurn) return;                //quick failsafe for if not the player turn, just return

        //masterBattleManagerRef.enemyAnim.SetTrigger("Normal");

        //if (masterBattleManagerRef.enemyRef.GetComponent<CombatAttributes>().DecreaseHealth(masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().GetDamageDealNormal())) //decrease health of enemy by player's attack damage normal amount. DecreaseHealth returns a bool depicting if entity is alive, so if true (enemy is alive) then run coroutine as normal. If false, entity is dead so shutdown the battle scene.
        //    StartCoroutine(FinishPlayerTurn()); //this runs if enemy is not killed
        //else
        //    ShutdownBattle();   //this runs if the enemy is killed.
    }

    public void PlayerInputAttackSpecial()
    {
        Debug.Log("Player Special Pressed");
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.special);
        //if (!masterBattleManagerRef.playerTurn) return;                //quick failsafe for if not the player turn, just return

        //if (masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().GetSpecialAttackAllowed())
        //{                               //attack is wound up! Ready to deal damage!
        //    masterBattleManagerRef.enemyAnim.SetTrigger("Normal");         //play attack animation on the player
        //    if (masterBattleManagerRef.enemyRef.GetComponent<CombatAttributes>().DecreaseHealth(masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().GetDamageDealSpecial())) //decrease health of enemy by player's attack damage special amount. DecreaseHealth returns a bool depicting if entity is alive, so if true (enemy is alive) then run coroutine as normal. If false, entity is dead so shutdown the battle scene.
        //    {
        //        //this runs if enemy is not killed
        //        playerAttackSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Wind Up\nSpecial");
        //        DialogueManager.GetInstance().StartNewDialogue("Player Attacks Special");
        //        StartCoroutine(FinishPlayerTurn());
        //    }
        //    else
        //        ShutdownBattle();       //this runs if the enemy is killed by the attack
        //}
        //else                            //attack is not wound up. Player will sacrifce this turn in order to wind up their attack
        //{
        //    playerAttackSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Attack\nSpecial");
        //    DialogueManager.GetInstance().StartNewDialogue("Player Winds Up!");
        //    masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().WindUpSpecial();
        //    StartCoroutine(FinishPlayerTurn());
        //}

    }

    public void PlayerInputDodge()
    {
        Debug.Log("Player Dodge Pressed");
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.dodge);
        //if (!masterBattleManagerRef.playerTurn) return;                //quick failsafe for if not the player turn, just return
        //playerAnim.SetTrigger("Dodge");         //play dodge animation on the player
        //DialogueManager.GetInstance().StartNewDialogue("Player Dodges");
        //masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().SetAttemptDodgeAttack(true);
        //StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputHeal()
    {
        Debug.Log("Player Heal Pressed");
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.heal);
        //if (!masterBattleManagerRef.playerTurn) return;                //quick failsafe for if not the player turn, just return
        //masterBattleManagerRef.playerAnim.SetTrigger("Heal");          //play heal animation on the player
        //DialogueManager.GetInstance().StartNewDialogue("Player heals");
        //masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().IncreaseHealth(masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().GetHealAmount());  //heal player by their determined heal amount
        //StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputFlee()
    {
        Debug.Log("Player Flee Pressed");
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.flee);
        //if (!masterBattleManagerRef.playerTurn) return;                //quick failsafe for if not the player turn, just return
        //PlayerAttemptFlee();
        //StartCoroutine(FinishPlayerTurn());
    }

    private void PlayerAttemptFlee()            //player attempts to flee. Returns true if attempt succeeded, false if attempt failed.
    {

        //if (Random.Range(0.0f, 1.0f) <= masterBattleManagerRef.playerRef.GetComponent<CombatAttributes>().GetSuccessFleeingChance())    //if a random float between 0 inclusive and 1 inclusive is LESS OR EQUAL to successFleeingChance then fleeing is considered a success and battle should end.
        //{
        //    DialogueManager.GetInstance().StartNewDialogue("Player escaped");
        //    ShutdownBattle();                   //stop the battle immediately
        //}
        //else
        //    DialogueManager.GetInstance().StartNewDialogue("Player failed to escape");
    }

    #endregion
}
