using UnityEngine;
using System.Collections;

public interface CharacterControllerInterface {

    void TakeDamage(int damageAmount, BattleUtils.AttackType attackType, BattleUtils.DamageTypes damageType);
}
