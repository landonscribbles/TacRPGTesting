using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

    public Vector2 gridLocation;
    public int movementCost;
    [HideInInspector]
    public Vector2 worldPosition;

    private BoardController boardController;

    [Header("Highlight Movement Range")]
    private bool highlightMoveRange;
    public GameObject highlightMoveRangePrefab;
    private GameObject highlightTile;
    public float maxOpacityMoveRange;
    private bool decreasingOpacityMoveRange;

    private int characterOnTileValue = 5000;
    [HideInInspector]
    public bool tileOccupied;

    void Start () {
        worldPosition = transform.position;
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
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
        highlightTile = Instantiate(highlightMoveRangePrefab, highlightPosition, Quaternion.identity) as GameObject;
        Color tempColor = highlightTile.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0;
        highlightTile.GetComponent<SpriteRenderer>().color = tempColor;
    }

    public void DisableMoveHighlight() {
        highlightMoveRange = false;
        Destroy(highlightTile);
    }

    void CycleMoveHighlight() {
        Color tempColor = highlightTile.GetComponent<SpriteRenderer>().color;
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
        highlightTile.GetComponent<SpriteRenderer>().color = tempColor;
    }

}
