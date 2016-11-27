using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OppositionCharacterController : BaseCharacterController, CharacterControllerInterface {

    private BoardController boardController;
    // private BattleUtils battleUtils;
    private BattleController battleController;

    private BattleAIInterface battleAI;

    void Start() {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        // battleUtils = GameObject.Find("BattleUtils").GetComponent<BattleUtils>();
        isMoving = false;
        isActive = false;
        hasMoved = false;
        currentHitPoints = maxHitPoints;
        currentSkillPoints = maxSkillPoints;
        battleAI = new BasicAI();
        currentStandingTile = boardController.GetTile(gridLocation);
        currentStandingTile.SetCharacterOnTile();
    }

    void Update() {
        // FIXME: Here is where the movement update information will go
        if (isActive&& !hasMoved && !isMoving) {
            CharacterController characterTarget = battleAI.PickOppositionTarget(gridLocation);
            movementPath = battleAI.GetMovementPath(gridLocation, moveRange, characterTarget);
            currentStandingTile.SetCharaterLeftTile();
            currentStandingTile = boardController.GetTile(movementPath[movementPath.Count - 1]);
            currentStandingTile.SetCharacterOnTile();
            // movementPath = battleAI.GetMovementPath(gridLocation, moveRange, battleAI.PickOppositionTarget(gridLocation));
            Vector3 gridWorldPosition = boardController.GetWorldPositionFromTileGrid(movementPath[0]);
            moveDestination = new Vector3(gridWorldPosition.x, gridWorldPosition.y, battleController.characterZLevel);
            isMoving = true;
        } else if (isActive && isMoving) {
            MoveToDestination();
        }
    }

    public void MoveToDestination() {
        transform.position = Vector3.MoveTowards(
            transform.position,
            moveDestination,
            worldMoveSpeed * Time.deltaTime
        );
        if (transform.position == moveDestination) {
            if (movementPath.Count == 1) {
                gridLocation = movementPath[0];
            }
            if (movementPath.Count == 0) {
                // set moveDestination to null?
                isMoving = false;
                hasMoved = true;
                // This bool later will be set in a different place
                isActive = false;
                turnInitiative = 0;
                return;
            } else {
                Vector3 gridWorldDestination = boardController.GetWorldPositionFromTileGrid(movementPath[0]);
                moveDestination = new Vector3(
                    gridWorldDestination.x, gridWorldDestination.y, battleController.characterZLevel
                );
                movementPath.RemoveAt(0);
            }
        }
    }

    public void UpdateTurnInitiative() {
        turnInitiative += turnSpeed;
    }

    public void SetCharacterTurn() {
        isActive = true;
        hasMoved = false;
    }

    // FIXME:
    // Each oppo character can have an ai type that can be set within the inspector, (strings to type of AI?)
    // then each turn the character uses that AI to compute it's actions that turn based on what the AI would target/value
}
