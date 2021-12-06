using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum modifierStates { neverBehaviour, defaultBehaviour, certaintyBehaviour }

[CreateAssetMenu(fileName = "NewEnemyBehaviourModifier", menuName = "EnemyBehaviour", order = 1)]
public class CombatAttributeModifier : ScriptableObject
{
    public string nameOfModifier;
    public modifierStates fleeConsideration = modifierStates.defaultBehaviour;         //does this modifier force the enemy to try to flee, force the enemy to NOT flee, or not change its fleeing consideration at all?
    public modifierStates dodgeConsideration = modifierStates.defaultBehaviour;         //does this modifier force the enemy to try to dodge, force the enemy to NOT dodge, or not change its dodging consideration at all?
    public modifierStates attackConsideration = modifierStates.defaultBehaviour;         //does this modifier force the enemy to try to attack, force the enemy to NOT attack, or blah blah blah you get it by now....
    public modifierStates healConsideration = modifierStates.defaultBehaviour;
    public int numberOfTurnsAffectedByModifier = 0;



    public void decrementNumOfTurns()
    {
        if (numberOfTurnsAffectedByModifier > 0)
            numberOfTurnsAffectedByModifier--;
    }
}
