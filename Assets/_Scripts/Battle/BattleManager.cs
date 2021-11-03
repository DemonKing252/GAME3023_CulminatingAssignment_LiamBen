using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    #region references
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





    void Start()
    {
        overworldGameGUI.gameObject.SetActive(false);
        
    }

    void ShutdownBattle()
    {
        Debug.Log("battle scene should close here!");
        enemyImage.enabled = false; //stop rendering enemy image
        enemyStatsCanvas.SetActive(false);  //stop rendering enemy stats canvas
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

        //add a little wait to enemy's actions are not instantanious
        yield return new WaitForSeconds(waitTimeForEnemyTurn);

        //tell the enemy to make its turn here

        //players turn once again! set bool accordingly and re-enable buttons
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

        Debug.Log("player Dodges");
        StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputHeal()
    {
        if (!playerTurn) return; //quick failsafe

        playerRef.GetComponent<CombatAttributes>().IncreaseHealth(playerRef.GetComponent<CombatAttributes>().GetHealAmount());  //heal player by their determined heal amount

        StartCoroutine(FinishPlayerTurn());
    }

    public void PlayerInputFlee()
    {
        if (!playerTurn) return; //quick failsafe


        Debug.Log("player flees");
        StartCoroutine(FinishPlayerTurn());
    }

}
