using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

    public Vector2 gridLocation;
    public int movementCost;
    [HideInInspector]
    public Vector2 worldPosition;

    private BoardController boardController;
    private BattleController battleController;

    [Header("Highlight Movement Range")]
    public GameObject highlightMoveRangePrefab;
    private bool highlightMoveRange;
    private GameObject highlightMovementTile;
    public float maxOpacityMoveRange;
    private bool decreasingOpacityMoveRange;

    [Header("Highlight Attack Range")]
    public GameObject highlightAttackRangePrefab;
    private bool highlightAttackRange;
    private GameObject highlightAttackTile;
    public float maxOpacityAttackRange;
    private bool decreasingOpacityAttackRange;

    private int characterOnTileValue = 5000;

    [HideInInspector]
    public bool tileOccupied;

    void Start () {
        worldPosition = transform.position;
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        boardController.RegisterTile(this);
        highlightMoveRange = false;
        tileOccupied = false;
        // TEMP
        //if (gridLocation.x == 0.0 && gridLocation.y == 0.0) {
        //    EnableMoveHighlight();
        //}
        //
    }

    void Update() {
        if (highlightMoveRange) {
            CycleMoveHighlight();
        }
        if (highlightAttackRange) {
            CycleAttackHighlight();
        }
    }

    public void SetCharacterOnTile() {
        movementCost += characterOnTileValue;
        tileOccupied = true;
    }

    public void SetCharaterLeftTile() {
        movementCost -= characterOnTileValue;
        tileOccupied = false;
    }

    public void EnableMoveHighlight() {
        highlightMoveRange = true;
        decreasingOpacityMoveRange = true;
        Vector3 highlightPosition = new Vector3(
            gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 1
        );
        highlightMovementTile = Instantiate(highlightMoveRangePrefab, highlightPosition, Quaternion.identity) as GameObject;
        Color tempColor = highlightMovementTile.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0;
        highlightMovementTile.GetComponent<SpriteRenderer>().color = tempColor;
    }

    public void DisableMoveHighlight() {
        highlightMoveRange = false;
        Destroy(highlightMovementTile);
    }

    void CycleMoveHighlight() {
        Color tempColor = highlightMovementTile.GetComponent<SpriteRenderer>().color;
        if (decreasingOpacityMoveRange) {
            tempColor.a = tempColor.a - (boardController.cycleStepMoveRange * Time.deltaTime);
            if (tempColor.a <= 0.0) {
                tempColor.a =0;
                decreasingOpacityMoveRange = false;
            }
        } else {
            tempColor.a = tempColor.a + (boardController.cycleStepMoveRange * Time.deltaTime);
            if (tempColor.a >= maxOpacityMoveRange) {
                tempColor.a = maxOpacityMoveRange;
                decreasingOpacityMoveRange = true;
            }
        }
        highlightMovementTile.GetComponent<SpriteRenderer>().color = tempColor;
    }

    public void EnableAttackRangeHighlight() {
        highlightAttackRange = true;
        decreasingOpacityAttackRange = true;
        Vector3 highlightPosition = new Vector3(
            gameObject.transform.position.x, gameObject.transform.position.y, battleController.characterZLevel - 1
        );
        highlightAttackTile = Instantiate(highlightAttackRangePrefab, highlightPosition, Quaternion.identity) as GameObject;
        Color tempColor = highlightAttackTile.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0;
        highlightAttackTile.GetComponent<SpriteRenderer>().color = tempColor;
    }

    public void DisableAttackHighlight() {
        highlightAttackRange = false;
        Destroy(highlightAttackTile);
    }

    void CycleAttackHighlight() {
        Color tempColor = highlightAttackTile.GetComponent<SpriteRenderer>().color;
        if (decreasingOpacityAttackRange) {
            tempColor.a = tempColor.a - (boardController.cycleStepAttackRange * Time.deltaTime);
            if (tempColor.a <= 0.0) {
                tempColor.a = 0;
                decreasingOpacityAttackRange = false;
            }
        } else {
            tempColor.a = tempColor.a + (boardController.cycleStepAttackRange * Time.deltaTime);
            if (tempColor.a >= maxOpacityAttackRange) {
                tempColor.a = maxOpacityAttackRange;
                decreasingOpacityAttackRange = true;
            }
        }
        highlightAttackTile.GetComponent<SpriteRenderer>().color = tempColor;
    }
}
