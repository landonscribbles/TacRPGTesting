using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BasicAI : BattleAIInterface {

    private BattleController battleController;
    private BoardController boardController;
    private PlayerCharactersController playerCharactersController;

    public BasicAI() {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        playerCharactersController = GameObject.Find("PlayerCharactersController").GetComponent<PlayerCharactersController>();
    }

    public List<Vector2> GetMovementPath(Vector2 startLocation, int moveRange, CharacterController targetCharacter) {
        Vector2 targetPoint = GetClosestAdjacentPoint(targetCharacter, startLocation);
        List<Vector2> movementPath = BattleUtils.GetPathRoute(startLocation, targetPoint, moveRange);
        return movementPath;
    }

    public CharacterController PickOppositionTarget(Vector2 startLocation) {
        List<CharacterController> emptyCharList = new List<CharacterController>();
        CharacterController targetController = PickOppositionTarget(startLocation, emptyCharList);
        return targetController;
    }

    public CharacterController PickOppositionTarget(Vector2 startLocation, List<CharacterController> excludedTargets) {
        // Simple pick the nearest opposing unit
        Dictionary<CharacterController, int> targetDistances = new Dictionary<CharacterController, int>();
        foreach (CharacterController characterController in playerCharactersController.allyCharacterControllers) {
            if (excludedTargets.Contains(characterController)) {
                continue;
            }
            int newTargetDistance = GetTargetDistance(characterController, startLocation);
            if (targetDistances.Count == 0) {
                targetDistances[characterController] = newTargetDistance;
            } else {
                int trackedTargetDistance = targetDistances.Values.ElementAt(0);
                if (trackedTargetDistance > newTargetDistance) {
                    targetDistances.Clear();
                    targetDistances[characterController] = newTargetDistance;
                } else if (newTargetDistance == targetDistances.Values.ElementAt(0)) {
                    targetDistances[characterController] = newTargetDistance;
                }
            }
        }
        if (targetDistances.Count == 1) {
            return targetDistances.Keys.ElementAt(0);
        } else {
            return targetDistances.Keys.ElementAt(Random.Range(0, targetDistances.Count));
        }
    }

    private int GetTargetDistance(CharacterController target, Vector2 startLocation) {
        int xDistance = Mathf.Abs((int)target.gridLocation.x - (int)startLocation.x);
        int yDistance = Mathf.Abs((int)target.gridLocation.y - (int)startLocation.y);
        int totalDistance = xDistance + yDistance;
        return totalDistance;
    }

    private Vector2 GetClosestAdjacentPoint(CharacterController target, Vector2 startLocation) {
        List<Vector2> adjacentAddition = new List<Vector2> {new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1)};
        Dictionary<Vector2, int> adjacentPointsDistances = new Dictionary<Vector2, int>();
        int closestDistance = 10000;
        foreach (Vector2 adjacentAdd in adjacentAddition) {
            Vector2 adjacentPoint = target.gridLocation + adjacentAdd;
            TileController adjacentTile = boardController.GetTile(adjacentPoint);
            if (adjacentTile.tileOccupied) {
                continue;
            }
            int xDistance = Mathf.Abs((int)target.gridLocation.x - (int)adjacentPoint.x);
            int yDistance = Mathf.Abs((int)target.gridLocation.y - (int)adjacentPoint.y);
            int totalDistance = xDistance + yDistance;
            if (totalDistance < closestDistance) {
                closestDistance = totalDistance;
            }
            adjacentPointsDistances[adjacentPoint] = totalDistance;
        }
        List<Vector2> closestPoints = new List<Vector2>();
        foreach (KeyValuePair<Vector2, int> adjacentPoint in adjacentPointsDistances) {
            if (adjacentPoint.Value == closestDistance) {
                closestPoints.Add(adjacentPoint.Key);
            }
        }
        if (closestPoints.Count == 0) {
            // Opposition target is surrounded
            List<CharacterController> excludedCharacters = new List<CharacterController>();
            excludedCharacters.Add(target);
            CharacterController newTarget = PickOppositionTarget(startLocation, excludedCharacters);
            return GetClosestAdjacentPoint(newTarget, startLocation);
        }
        else if (closestPoints.Count > 1) {
            return closestPoints[Random.Range(0, closestPoints.Count)];
        } else {
            return closestPoints[0];
        }
    }
}
