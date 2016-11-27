using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterController : BaseCharacterController, CharacterControllerInterface {

    private BoardController boardController;
    // private BattleUtils battleUtils;
    private CharacterActionsMenuController characterActionsMenuController;
    private BattleController battleController;

    // If pathing becomes a major bottleneck a thread could be setup to compute pathing to a target,
    // also an event listener could be put in place for when the target moves to re-queue up the job to get a new pathing

    void Start() {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        // battleUtils = GameObject.Find("BattleUtils").GetComponent<BattleUtils>();
        characterActionsMenuController = GameObject.Find("CharacterActionsMenu").GetComponent<CharacterActionsMenuController>();
        isMoving = false;
        isActive = false;
        hasMoved = false;
        hasTakenAction = false;
        currentHitPoints = maxHitPoints;
        currentSkillPoints = maxSkillPoints;
        currentStandingTile = boardController.GetTile(gridLocation);
        currentStandingTile.SetCharacterOnTile();
    }

    void Update() {
        if (isActive && !hasMoved && !isMoving) {
            GameObject clickedTile = CheckMovementTileForClick();
            ToggleMoveIfTileIsReachable(clickedTile);
        } else if (isActive && isMoving) {
            MoveToDestination();
        } else if (isActive && !isMoving && hasMoved) {
            // FIXME: Take action here
        }
    }

    private void ToggleMoveIfTileIsReachable(GameObject clickedTile) {
        if (clickedTile != null) {
            TileController clickedTileController = clickedTile.GetComponent<TileController>();
            Vector2 clickedTileGridLoc = clickedTileController.gridLocation;
            if (moveableTiles.Contains(clickedTileGridLoc)) {
                currentStandingTile.SetCharaterLeftTile();
                movementPath = BattleUtils.GetPathRoute(gridLocation, clickedTileController.gridLocation, moveRange, moveableTiles);
                Vector3 gridWorldPosition = boardController.GetWorldPositionFromTileGrid(movementPath[0]);
                moveDestination = new Vector3(gridWorldPosition.x, gridWorldPosition.y, battleController.characterZLevel);
                boardController.RemoveMoveRangeHighlightTiles(moveableTiles.ToList());
                isMoving = true;
            }
        }
    }

    private GameObject CheckMovementTileForClick() {
        GameObject clickedTile = null;
        if (Input.GetMouseButtonDown(0)) {
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] rayHits = Physics2D.RaycastAll(ray, Vector2.zero);
            if (rayHits.Length != 0) {
                Debug.Log("Checking movement tile for click");
                foreach (RaycastHit2D singleHit in rayHits) {
                    if (singleHit.transform.tag == "MovementTile") {
                        string logline = string.Format(
                            "Hit tile at: {0},{1}",
                            singleHit.transform.position.x,
                            singleHit.transform.position.y
                        );
                        Debug.Log(logline);
                        clickedTile = singleHit.transform.gameObject;
                    } else if (singleHit.transform.tag == "Character") {
                        clickedTile = null;
                        return clickedTile;
                    }
                    return clickedTile;
                }
                return clickedTile;
            }
        }
        return clickedTile;
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
                currentStandingTile = boardController.GetTile(gridLocation);
                currentStandingTile.SetCharacterOnTile();
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
        // This should activate the character turn menu: Move, Attack, Etc.
        isActive = true;
        hasMoved = false;
        characterActionsMenuController.SetCharacterName(characterName);
        characterActionsMenuController.SetHitPoints(currentHitPoints, maxHitPoints);
        characterActionsMenuController.SetSkillPoints(currentSkillPoints, maxSkillPoints);
        // Temporary
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        characterActionsMenuController.SetPortrait(spriteRenderer.sprite);
        //

        moveableTiles = BattleUtils.Calculate2DTileRange(gridLocation, moveRange);
        boardController.SetMoveRangeHighlightTiles(moveableTiles.ToList());
        // TODO here
        Debug.Log(characterName + "'s Turn!");
    }
}
