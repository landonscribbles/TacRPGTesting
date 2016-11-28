using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterController : BaseCharacterController, CharacterControllerInterface {

    private BoardController boardController;
    private CharacterActionsMenuController characterActionsMenuController;
    private BattleController battleController;

    private bool movementTilesHighlighted;
    private bool attackTilesHighlighted;
    private List<Vector2> attackableTiles;

    // If pathing becomes a major bottleneck a thread could be setup to compute pathing to a target,
    // also an event listener could be put in place for when the target moves to re-queue up the job to get a new pathing

    void Start() {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        characterActionsMenuController = GameObject.Find("CharacterActionsMenu").GetComponent<CharacterActionsMenuController>();
        isMoving = false;
        isActive = false;
        hasMoved = false;
        hasTakenAction = false;
        movementTilesHighlighted = false;
        currentHitPoints = maxHitPoints;
        currentSkillPoints = maxSkillPoints;
        currentStandingTile = boardController.GetTile(gridLocation);
        currentStandingTile.SetCharacterOnTile();
        attackableTiles = new List<Vector2>();
    }

    void Update() {
        if(isActive) {
            if (!hasMoved && !isMoving && movementTilesHighlighted) {
                GameObject clickedTile = CheckMovementTileForClick();
                ToggleMoveIfTileIsReachable(clickedTile);
            } else if (isMoving) {
                MoveToDestination();
            } else if (!isMoving && attackTilesHighlighted) {
                OppositionCharacterController clickedOppoChar = CheckAttackTilesForClick();
                if (clickedOppoChar != null) {
                    Debug.Log("Clicked on: " + clickedOppoChar.characterName);
                    RemoveAttackableTilesHighlight();
                    clickedOppoChar.TakeDamage(GetMeleeAttackDamage(), BattleUtils.AttackType.physical, BattleUtils.DamageTypes.physical);
                }
            }
        }
    }

    private int GetMeleeAttackDamage() {
        return 10;
    }

    public void EndTurn() {
        // This bool later will be set in a different place
        isActive = false;
        turnInitiative = 0;
        RemoveAttackableTilesHighlight();
        characterActionsMenuController.Deactivate();
    }

    public void AttackButtonPressed() {
        if (!hasTakenAction && !attackTilesHighlighted) {
            HighlightAttackableTiles();
        }
    }

    public void HighlightAttackableTiles() {
        HashSet<Vector2> attackableSet = BattleUtils.Calculate2DTileRange(gridLocation, 2);
        foreach (Vector2 gridPoint in attackableSet.ToList()) {
            if (gridPoint.x == gridLocation.x && gridPoint.y == gridLocation.y) {
                continue;
            }
            attackableTiles.Add(gridPoint);
        }
        boardController.SetAttackRangeHighlightTiles(attackableTiles);
        attackTilesHighlighted = true;
    }

    public void RemoveAttackableTilesHighlight() {
        boardController.RemoveAttackRangeHighlightTiles(attackableTiles);
        attackTilesHighlighted = false;

    }

    public void HighlightMoveableTiles() {
        if (!hasMoved && !isMoving && !movementTilesHighlighted) {
            moveableTiles = BattleUtils.Calculate2DTileRange(gridLocation, moveRange);
            boardController.SetMoveRangeHighlightTiles(moveableTiles.ToList());
            movementTilesHighlighted = true;
        }
    }

    public void RemoveMoveableTilesHighlight() {
        boardController.RemoveMoveRangeHighlightTiles(moveableTiles.ToList());
        movementTilesHighlighted = false;
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
                RemoveMoveableTilesHighlight();
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

    private OppositionCharacterController CheckAttackTilesForClick() {
        OppositionCharacterController clickedTarget = null;
        if (Input.GetMouseButtonDown(0)) {
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] rayHits = Physics2D.RaycastAll(ray, Vector2.zero);
            if (rayHits.Length != 0) {
                foreach (RaycastHit2D singleHit in rayHits) {
                    GameObject clickedGameObject = singleHit.transform.gameObject;
                    OppositionCharacterController oppoTargetController = clickedGameObject.GetComponent<OppositionCharacterController>();
                    if (oppoTargetController != null) {
                        if (attackableTiles.Contains(oppoTargetController.gridLocation)) {
                            clickedTarget = oppoTargetController;
                            return clickedTarget;
                        }
                    }
                }
            }
        }
        return clickedTarget;
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
        movementTilesHighlighted = false;
        attackTilesHighlighted = false;
        isMoving = false;
        characterActionsMenuController.Activate();
        characterActionsMenuController.ResetMenu();
        characterActionsMenuController.SetCharacterName(characterName);
        characterActionsMenuController.SetHitPoints(currentHitPoints, maxHitPoints);
        characterActionsMenuController.SetSkillPoints(currentSkillPoints, maxSkillPoints);
        characterActionsMenuController.SetMoveButtonCallback(HighlightMoveableTiles);
        characterActionsMenuController.SetAttackButtonCallback(AttackButtonPressed);
        characterActionsMenuController.SetEndTurnButtonCallback(EndTurn);
        // Temporary
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        characterActionsMenuController.SetPortrait(spriteRenderer.sprite);
        //
        // TODO here
        Debug.Log(characterName + "'s Turn!");
    }
}
