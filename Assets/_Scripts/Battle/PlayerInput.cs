using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum choiceAction
{
    attack,
    specialScare,
    specialGlue,
    dodge,
    dodgeFail,
    heal,
    flee,
    fleeFail,
    none,
    unassigned
}
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Button playerAttackButton;

    [SerializeField]
    private Button playerSpecialButton;

    [SerializeField]
    private Button playerDodgeButton;

    [SerializeField]
    private Button playerHealButton;

    [SerializeField]
    private Button playerFleeButton;

    [SerializeField]
    private GameObject playerSpecialMovesCanvas;
    #region PlayerInputs


    public void SetPlayerButtonsClickable(bool newClickableState)
    {
        if (GetComponent<MasterBattleManager>().turnsUntilPlayerSpecialAllowed <= 0)
        {
            playerSpecialButton.interactable = newClickableState;
            playerSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Use Special Move");
        }
        else
        {
            playerSpecialButton.interactable = false;
            playerSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Special Move Unavailable \n Unlocks in: " + GetComponent<MasterBattleManager>().turnsUntilPlayerSpecialAllowed);
        }

        //set buttons interactability based on passed bool
        playerAttackButton.interactable = newClickableState;
        playerDodgeButton.interactable = newClickableState;
        playerFleeButton.interactable = newClickableState;

        playerSpecialMovesCanvas.SetActive(false);

        //only set health button to be interactable if passed bool is true AND player is NOT at full health!
        if (newClickableState == true && !GetComponent<MasterBattleManager>().playerRef.GetComponent<CombatAttributes>().GetIsAtFullHealth())
        {
            playerHealButton.interactable = true;
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Heal");
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
        }
        else
        {
            playerHealButton.interactable = false;
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().SetText("You're at \n Full Health");
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 7;
        }
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

    public void PlayerInputSpecialMenuToggle()
    {
        playerSpecialMovesCanvas.SetActive(!playerSpecialMovesCanvas.activeInHierarchy);
    }

    public void PlayerInputSpecialMenuDisable()
    {
        playerSpecialMovesCanvas.SetActive(false);
    }

    public void PlayerInputSpecialScare()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.specialScare);
    }

    public void PlayerInputSpecialGlue()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.specialGlue);
    }


    public void PlayerInputDodge()
    {
        Debug.Log("Player Dodge Pressed");
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.dodge);
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

    #endregion
}
