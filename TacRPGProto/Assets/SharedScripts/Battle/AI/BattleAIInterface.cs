using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface BattleAIInterface {

    List<Vector2> GetMovementPath(Vector2 startLocation, int moveRange, CharacterController targetCharacter);

    CharacterController PickOppositionTarget(Vector2 startLocation);
}
