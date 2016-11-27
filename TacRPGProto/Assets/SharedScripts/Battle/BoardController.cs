using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoardController : MonoBehaviour {

    public static BoardController instance = null;

    [HideInInspector]
    public float boardXBound = 0;
    [HideInInspector]
    public float boardYBound = 0;

    [HideInInspector]
    public List<TileController> tiles = new List<TileController>();

    [Header("Highlight MovementRange")]
    public float cycleStepMoveRange;

    [Header("Highlight AttackRange")]
    public float cycleStepAttackRange;

    public PlayerCharactersController playerCharactersController;

    private BattleController battleController;

    public bool tilesHighlighted;
    public bool characterMoving;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void Start () {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        tilesHighlighted = false;
        characterMoving = false;
    }

    public void RegisterTile(TileController tileInfo) {
        tiles.Add(tileInfo);
        if (tileInfo.gridLocation.x > boardXBound) {
            boardXBound = tileInfo.gridLocation.x;
        }
        if (tileInfo.gridLocation.y > boardYBound) {
            boardYBound = tileInfo.gridLocation.y;
        }
    }

    public Vector3 GetWorldPositionFromTileGrid(Vector2 tileGridLocation) {
        Vector3 returnLocation = new Vector3(-1, -1, -1000);
        foreach (TileController tile in tiles) {
            if (tile.gridLocation.x == tileGridLocation.x && tile.gridLocation.y == tileGridLocation.y) {
                returnLocation = tile.transform.position;
            }
        }
        if (returnLocation.z == -1000.0) {
            Debug.Log("Tile lookup at failed for tile at: " + tileGridLocation);
        }
        return returnLocation;
    }

    public TileController GetTile(Vector2 tileGridLocation) {
        TileController returnTile = null;
        foreach (TileController tile in tiles) {
            if (tile.gridLocation.x == tileGridLocation.x && tile.gridLocation.y == tileGridLocation.y) {
                returnTile = tile;
            }
        }
        return returnTile;
    }

    public void SetMoveRangeHighlightTiles(List<Vector2> tilesToHighlight) {
        foreach (Vector2 tileLocation in tilesToHighlight) {
            foreach (TileController tile in tiles) {
                if (tile.gridLocation.x == tileLocation.x && tile.gridLocation.y == tileLocation.y) {
                    tile.EnableMoveHighlight();
                }
            }
        }
        tilesHighlighted = true;
    }

    public void RemoveMoveRangeHighlightTiles(List<Vector2> tilesToHighlight) {
        foreach (Vector2 tileLocation in tilesToHighlight) {
            foreach (TileController tile in tiles) {
                if (tile.gridLocation.x == tileLocation.x && tile.gridLocation.y == tileLocation.y) {
                    tile.DisableMoveHighlight();
                }
            }
        }
        tilesHighlighted = false;
    }

    public void SetAttackRangeHighlightTiles(List<Vector2> tilesToHighlight) {
        foreach (Vector2 tileLocation in tilesToHighlight) {
            foreach (TileController tile in tiles) {
                if (tile.gridLocation.x == tileLocation.x && tile.gridLocation.y == tileLocation.y) {
                    tile.EnableAttackRangeHighlight();
                }
            }
        }
        tilesHighlighted = true;
    }

    public void RemoveAttackRangeHighlightTiles(List<Vector2> tilesToHighlight) {
        foreach (Vector2 tileLocation in tilesToHighlight) {
            foreach (TileController tile in tiles) {
                if (tile.gridLocation.x == tileLocation.x && tile.gridLocation.y == tileLocation.y) {
                    tile.DisableAttackHighlight();
                }
            }
        }
        tilesHighlighted = false;
    }
}
