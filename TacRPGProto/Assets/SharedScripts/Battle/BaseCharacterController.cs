using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseCharacterController : MonoBehaviour {
    // Gameplay stats
    public string characterName;
    protected int baseResistance = 0;
    public int maxHitPoints;
    public int currentHitPoints;
    public int maxSkillPoints;
    public int currentSkillPoints;
    public int physicalDodge;
    public int magicDodge;
    public int moveRange;
    public int turnSpeed;
    public int turnInitiative;

    // World location and related movement information
    public float worldMoveSpeed;
    [HideInInspector]
    public bool isMoving;
    [HideInInspector]
    public Vector2 gridLocation;
    protected Vector3 moveDestination;
    protected List<Vector2> movementPath;
    protected HashSet<Vector2> moveableTiles;

    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public bool hasMoved;
    [HideInInspector]
    public bool hasTakenAction;

    protected TileController currentStandingTile;


    [System.Serializable]
    public class TypeResistance {
        public BattleUtils.DamageTypes damageType;
        public int resistanceAmount;

        public TypeResistance(BattleUtils.DamageTypes damageType, int resistanceAmount) {
            this.damageType = damageType;
            this.resistanceAmount = resistanceAmount;
        }
    }

    public TypeResistance[] typeResistances;

    // FIXME: Later create a method here to calculate resistances based on class/gear/etc.

    public void TakeDamage(int damageAmount, BattleUtils.AttackType attackType, BattleUtils.DamageTypes damageType) {
        int attackRole = Random.Range(0, 100);
        if (attackType == BattleUtils.AttackType.physical || attackType == BattleUtils.AttackType.ranged) {
            if (physicalDodge > attackRole) {
                // FIXME: Indicate the attack was dodged here
                return;
            }
        } else if (attackType == BattleUtils.AttackType.magical) {
            if (magicDodge > attackRole) {
                // FIXME: Indicate attack was dodged
                return;
            }
        }
        int actualDamage = CalculateDamageResistance(damageAmount, damageType);
        currentHitPoints -= actualDamage;
    }

    protected int CalculateDamageResistance(int rawDamageAmount, BattleUtils.DamageTypes damageType) {
        TypeResistance resistanceType = null;
        foreach (TypeResistance resType in typeResistances) {
            if (resType.damageType == damageType) {
                resistanceType = resType;
            }
        }
        // Better to return no resistance than break the game
        if (resistanceType == null) {
            Debug.Log(characterName + " HAS NO RESISTANCE SET FOR TYPE: " + damageType.ToString());
            resistanceType = new TypeResistance(damageType, 0);
        }
        float resistancePercent = resistanceType.resistanceAmount / 100.0f;
        float actualDamage = rawDamageAmount * resistancePercent;
        return (int)actualDamage;
    }
}
