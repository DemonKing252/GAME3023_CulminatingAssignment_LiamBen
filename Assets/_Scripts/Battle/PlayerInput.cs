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
    specialTrick,
    specialBleeder,
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
    private Button playerScareButton;

    [SerializeField]
    private Button playerGlueButton;

    [SerializeField]
    private Button playerTrickButton;

    [SerializeField]
    private Button playerBleederButton;

    [SerializeField]
    private GameObject playerSpecialMovesCanvas;
    #region PlayerInputs

    public void UnlockSpecialAbility()
    {
        int currentSpecialAbilityStat = 0;

        for (int i = 0; i < GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility.Length; i++)
        {
            if (GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility[i])
                currentSpecialAbilityStat++;
        }

        Debug.Log("NEW SPECIAL ABILITY ELEMENT: " + currentSpecialAbilityStat);
        if (currentSpecialAbilityStat < GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility.Length)
            GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility[currentSpecialAbilityStat] = true;
    }

    public void SetPlayerButtonsClickable(bool newClickableState)
    {
        if (GetComponent<MasterBattleManager>().GetTurnsUntilPlayerSpecialAllowed() <= 0)
        {
            playerSpecialButton.interactable = newClickableState;
            playerSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Use Special Move");
        }
        else
        {
            playerSpecialButton.interactable = false;
            playerSpecialButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Special Move Unavailable \n Unlocks in: " + GetComponent<MasterBattleManager>().GetTurnsUntilPlayerSpecialAllowed());
        }

        HandleSpecialAbilitiesButtonsAvailability();


        //set buttons interactability based on passed bool
        playerAttackButton.interactable = newClickableState;
        playerDodgeButton.interactable = newClickableState;
        playerFleeButton.interactable = newClickableState;
        playerHealButton.interactable = newClickableState;
        playerSpecialMovesCanvas.SetActive(false);

        //change button text and interactability depending if at full health
        if (GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().GetIsAtFullHealth())
        {
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().SetText("You're at \n Full Health");
            playerHealButton.interactable = false;
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 7;
        }
        else
        {
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Heal");
            playerHealButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
        }
    }

    private void HandleSpecialAbilitiesButtonsAvailability()
    {
        playerScareButton.interactable = GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility[0];
        playerGlueButton.interactable = GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility[1];
        playerTrickButton.interactable = GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility[2];
        playerBleederButton.interactable = GetComponent<MasterBattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().hasSpecialAbility[3];


        //this is really bad. Each special ability should be a scriptable object, but it's kinda too late to go back now...
        if (playerScareButton.interactable)
        {
            playerScareButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Scare");
            playerScareButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
        }
        else
        {
            playerScareButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Not\nUnlocked\nYet!");
            playerScareButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 7;
        }

        if (playerGlueButton.interactable)
        {
            playerGlueButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Glue");
            playerGlueButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
        }
        else
        {
            playerGlueButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Not\nUnlocked\nYet!");
            playerGlueButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 7;
        }

        if (playerTrickButton.interactable)
        {
            playerTrickButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Trick");
            playerTrickButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
        }
        else
        {
            playerTrickButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Not\nUnlocked\nYet!");
            playerTrickButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 7;
        }

        if (playerBleederButton.interactable)
        {
            playerBleederButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Large\nAttack");
            playerBleederButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
        }
        else
        {
            playerBleederButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Not\nUnlocked\nYet!");
            playerBleederButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 7;
        }
    }

    public void PlayerInputAttack()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.attack);
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

    public void PlayerInputSpecialTrick()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.specialTrick);
    }

    public void PlayerInputSpecialBleeder()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.specialBleeder);
    }


    public void PlayerInputDodge()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.dodge);
    }

    public void PlayerInputHeal()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.heal);
    }

    public void PlayerInputFlee()
    {
        GetComponent<MasterBattleManager>().PlayerChoseAction(choiceAction.flee);
    }

    #endregion
}
