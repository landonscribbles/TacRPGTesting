using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleUtils : MonoBehaviour {

    public enum DamageTypes { opsol, fire, ice, electric, earth, physical, dark };
    public enum AttackType {  physical, ranged, magical };

    private static Dictionary<Vector2, int> GetNeighbors(Vector2 startPoint) {
        BoardController boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        List<Vector2> vectorAdds = new List<Vector2>();
        vectorAdds.Add(new Vector2(1, 0));
        vectorAdds.Add(new Vector2(0, 1));
        vectorAdds.Add(new Vector2(-1, 0));
        vectorAdds.Add(new Vector2(0, -1));

        int maxColumn = (int)boardController.boardXBound;
        int maxRows = (int)boardController.boardYBound;

        Dictionary<Vector2, int> neighbors = new Dictionary<Vector2, int>();

        foreach (Vector2 vectorAdd in vectorAdds) {
            Vector2 potentialNeighbor = startPoint + vectorAdd;
            if (potentialNeighbor.x > maxColumn) {
                continue;
            } else if (potentialNeighbor.x < 0) {
                continue;
            } else if (potentialNeighbor.y > maxRows) {
                continue;
            } else if (potentialNeighbor.y < 0) {
                continue;
            } else {
                //GameObject neighborTile = boardController.GetTile(potentialNeighbor);
                TileController neighborTileController = boardController.GetTile(potentialNeighbor);
                if (neighborTileController == null) {
                    continue;
                }
                neighbors[potentialNeighbor] = neighborTileController.movementCost;
            }
        }
        return neighbors;
    }

    public static List<Vector2> GetPathRoute(Vector2 startPoint, Vector2 endPoint, int totalMoves) {
        BoardController boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        HashSet<Vector2> allMovableTiles = new HashSet<Vector2>();
        foreach (TileController tile in boardController.tiles) {
            allMovableTiles.Add(tile.gridLocation);
        }
        return GetPathRoute(startPoint, endPoint, totalMoves, allMovableTiles);
    }

    public static List<Vector2> GetPathRoute(Vector2 startPoint, Vector2 endPoint, int totalMoves, HashSet<Vector2> moveableTiles) {
        // Needs to get the bounds of the map
        // Can't go above or below the upper and lower ranges of the map
        Debug.Log("Moveable tiles: " + moveableTiles.Count());
        GameObject boardControllerObject = GameObject.Find("BoardController");
        BoardController boardController = boardControllerObject.GetComponent<BoardController>();

        Dictionary<Vector2, int> unvisitedTiles = new Dictionary<Vector2, int>();
        List<Vector2> unvisitedPoints;
        Dictionary<Vector2, int> visistedTiles = new Dictionary<Vector2, int>();
        Dictionary<Vector2, Vector2> traveledPath = new Dictionary<Vector2, Vector2>();

        // PlayerCharactersController playerCharactersController = GameObject.Find("PlayerCharacterController").GetComponent<PlayerCharactersController>();

        //foreach (TileController tile in boardController.tiles) {
        foreach (Vector2 tile in moveableTiles) {

            unvisitedTiles[tile] = 10000;
            traveledPath[tile] = new Vector2(-1, -1);
        }

        Vector2 currentPoint = startPoint;
        int currentDistance = 0;
        unvisitedTiles[startPoint] = currentDistance;

        bool running = true;
        while (running) {
            Dictionary<Vector2, int> neighborDistance = GetNeighbors(currentPoint);
            unvisitedPoints = unvisitedTiles.Keys.ToList();
            foreach (KeyValuePair<Vector2, int> neighbor in neighborDistance) {
                if (!unvisitedPoints.Contains(neighbor.Key)) {
                    continue;
                }
                int newDistance = currentDistance + neighbor.Value;
                if (unvisitedTiles[neighbor.Key] > newDistance) {
                    unvisitedTiles[neighbor.Key] = newDistance;
                    traveledPath[neighbor.Key] = currentPoint;
                }
            }
            visistedTiles[currentPoint] = currentDistance;
            unvisitedTiles.Remove(currentPoint);
            if (unvisitedTiles.Count == 0) {
                running = false;
                continue;
            }
            List<KeyValuePair<Vector2, int>> candidates = unvisitedTiles.ToList();
            List<KeyValuePair<Vector2, int>> orderedCandidates = candidates.OrderBy(o => o.Value).ToList();
            KeyValuePair<Vector2, int> nextNode = orderedCandidates[0];
            currentPoint = nextNode.Key;
            currentDistance = nextNode.Value;
        }

        List<KeyValuePair<Vector2, int>> shortPathDistance = new List<KeyValuePair<Vector2, int>>();
        List<Vector2> shortPath = new List<Vector2>();

        running = true;
        Vector2 previousPoint = endPoint;

        while (running) {
            KeyValuePair<Vector2, int> pointAndCost = new KeyValuePair<Vector2, int>(previousPoint, visistedTiles[previousPoint]);
            shortPathDistance.Add(pointAndCost);
            if (previousPoint == startPoint) {
                running = false;
                continue;
            }
            previousPoint = traveledPath[previousPoint];
        }

        shortPathDistance.Reverse();
        foreach (KeyValuePair<Vector2, int> move in shortPathDistance) {
            // TODO: Add total move values made until this point and break if it exceeds totalMoves
            if (move.Value > totalMoves) {
                break;
            }
            shortPath.Add(move.Key);
        }
        return shortPath;
    }

    public static HashSet<Vector2> Calculate2DTileRange(Vector2 startingPoint, int range) {
        HashSet<Vector2> tileRange = new HashSet<Vector2>();
        Vector2 rangePoint;
        foreach (int i in Enumerable.Range(0, range)) {
            rangePoint = new Vector2(startingPoint.x + i, startingPoint.y);
            tileRange.Add(rangePoint);
            rangePoint = new Vector2(startingPoint.x - i, startingPoint.y);
            tileRange.Add(rangePoint);
            rangePoint = new Vector2(startingPoint.x, startingPoint.y + i);
            tileRange.Add(rangePoint);
            rangePoint = new Vector2(startingPoint.x, startingPoint.y - i);
            tileRange.Add(rangePoint);
        }
        int xVal;
        int yVal;
        foreach (int i in Enumerable.Range(0, range)) {
            if (i == 0) {
                continue;
            }
            xVal = i - 1;
            yVal = (range - i) + (int)startingPoint.y;
            while (yVal > startingPoint.y) {
                rangePoint = new Vector2(startingPoint.x + xVal, yVal);
                tileRange.Add(rangePoint);
                yVal -= 1;
            }

            xVal = (i - 1) * -1;
            yVal = (range - i) + (int)startingPoint.y;
            while (yVal > startingPoint.y) {
                rangePoint = new Vector2(startingPoint.x + xVal, yVal);
                tileRange.Add(rangePoint);
                yVal -= 1;
            }

            xVal = (i - 1) * -1;
            yVal = ((range - i) * -1) + (int)startingPoint.y;
            while (yVal < startingPoint.y) {
                rangePoint = new Vector2(startingPoint.x + xVal, yVal);
                tileRange.Add(rangePoint);
                yVal += 1;
            }

            xVal = i - 1;
            yVal = ((range - i) * -1) + (int)startingPoint.y;
            while (yVal < startingPoint.y) {
                rangePoint = new Vector2(startingPoint.x + xVal, yVal);
                tileRange.Add(rangePoint);
                yVal += 1;
            }
        }
        return tileRange;
    }

}
