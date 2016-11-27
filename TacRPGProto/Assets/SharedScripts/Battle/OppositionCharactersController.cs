using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OppositionCharactersController : MonoBehaviour {

    public bool isActive;
    private OppositionCharacterController activeCharacter;

    private BoardController boardController;
    private BattleController battleController;
    // private BattleUtils battleUtils;

    // Character objects and controllers
    private List<OppositionCharacterController> oppoCharacterControllers;
    public GameObject oppoWarriorPrefab;
    [HideInInspector]
    public OppositionCharacterController oppoWarriorController;

    public GameObject oppoHealerPrefab;
    [HideInInspector]
    public OppositionCharacterController oppoHealerController;

    public GameObject oppoRangerPrefab;
    [HideInInspector]
    public OppositionCharacterController oppoRangerController;

    public GameObject oppoMagePrefab;
    [HideInInspector]
    public OppositionCharacterController oppoMageController;

    public Vector2 gridStartLocation;

    void Awake() {
        oppoCharacterControllers = new List<OppositionCharacterController>();
        isActive = false;
        activeCharacter = null;
    }

    void Start() {
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        // battleUtils = GameObject.Find("BattleUtils").GetComponent<BattleUtils>();
        battleController.RegisterEnemyCharactersController(this);
        PlaceOppoCharacters();
    }

    void Update() {
        if (isActive) {
            if (activeCharacter == null) {
                isActive = false;
                return;
            } else if (!activeCharacter.isActive) {
                isActive = false;
                return;
            }
        }
    }

    public int GetHighestCharacterTurnInitiative() {
        int highestTurnInitiative = 0;
        foreach (OppositionCharacterController oppoCharController in oppoCharacterControllers) {
            if (oppoCharController.turnInitiative > highestTurnInitiative) {
                highestTurnInitiative = oppoCharController.turnInitiative;
            }
        }
        return highestTurnInitiative;
    }

    public int TopCharacterTurnSpeedWithTopInitiative() {
        int topInitiative = GetHighestCharacterTurnInitiative();
        int highestTurnSpeed = 0;
        foreach (OppositionCharacterController oppoCharController in oppoCharacterControllers) {
            if (oppoCharController.turnInitiative == topInitiative) {
                if (oppoCharController.turnSpeed > highestTurnSpeed) {
                    highestTurnSpeed = oppoCharController.turnSpeed;
                }
            }
        }
        return highestTurnSpeed;
    }

    public void UpdateCharacterTurnInitiatives() {
        foreach (OppositionCharacterController oppoCharController in oppoCharacterControllers) {
            oppoCharController.UpdateTurnInitiative();
        }
    }

    void PlaceOppoCharacters() {
        Vector3 warriorWorldLocation = boardController.GetWorldPositionFromTileGrid(gridStartLocation);
        GameObject warriorObject = Instantiate(
            oppoWarriorPrefab,
            warriorWorldLocation,
            Quaternion.identity
        ) as GameObject;
        oppoWarriorController = warriorObject.GetComponent<OppositionCharacterController>();
        oppoWarriorController.gridLocation = gridStartLocation;
        oppoCharacterControllers.Add(oppoWarriorController);

        Vector2 healerStartLocation = new Vector2(gridStartLocation.x + 1, gridStartLocation.y);
        Vector3 healerWorldLocation = boardController.GetWorldPositionFromTileGrid(healerStartLocation);
        GameObject healerObject = Instantiate(
            oppoHealerPrefab,
            healerWorldLocation,
            Quaternion.identity
        ) as GameObject;
        oppoHealerController = healerObject.GetComponent<OppositionCharacterController>();
        oppoHealerController.gridLocation = healerStartLocation;
        oppoCharacterControllers.Add(oppoHealerController);

        Vector2 rangerStartLocation = new Vector2(gridStartLocation.x + 2, gridStartLocation.y);
        Vector3 rangerWorldLocation = boardController.GetWorldPositionFromTileGrid(rangerStartLocation);
        GameObject rangerObject = Instantiate(
            oppoRangerPrefab,
            rangerWorldLocation,
            Quaternion.identity
        ) as GameObject;
        oppoRangerController = rangerObject.GetComponent<OppositionCharacterController>();
        oppoRangerController.gridLocation = rangerStartLocation;
        oppoCharacterControllers.Add(oppoRangerController);

        Vector2 mageStartLocation = new Vector2(gridStartLocation.x + 3, gridStartLocation.y);
        Vector3 mageWorldLocation = boardController.GetWorldPositionFromTileGrid(mageStartLocation);
        GameObject mageObject = Instantiate(
            oppoMagePrefab,
            mageWorldLocation,
            Quaternion.identity
        ) as GameObject;
        oppoMageController = mageObject.GetComponent<OppositionCharacterController>();
        oppoMageController.gridLocation = mageStartLocation;
        oppoCharacterControllers.Add(oppoMageController);
    }

    public void StartOppositionTurn() {
        isActive = true;
        int topInitiative = GetHighestCharacterTurnInitiative();
        List<OppositionCharacterController> topOppoInitiativeCharacters = new List<OppositionCharacterController>();
        foreach (OppositionCharacterController oppoController in oppoCharacterControllers) {
            if (oppoController.turnInitiative == topInitiative) {
                topOppoInitiativeCharacters.Add(oppoController);
            }
        }
        if (topOppoInitiativeCharacters.Count > 1) {
            List<OppositionCharacterController> topInitiativeAndTurnSpeedOppoChars = new List<OppositionCharacterController>();
            int topTurnSpeed = 0;
            foreach (OppositionCharacterController topInitiativeOppoChar in topOppoInitiativeCharacters) {
                if (topInitiativeOppoChar.turnSpeed > topTurnSpeed) {
                    topTurnSpeed = topInitiativeOppoChar.turnSpeed;
                }
            }
            foreach (OppositionCharacterController topInitiativeOppoChar in topOppoInitiativeCharacters) {
                if (topInitiativeOppoChar.turnSpeed == topTurnSpeed) {
                    topInitiativeAndTurnSpeedOppoChars.Add(topInitiativeOppoChar);
                }
            }
            if (topInitiativeAndTurnSpeedOppoChars.Count > 1) {
                int oppoCharIndexToSetTurn = Random.Range(0, topInitiativeAndTurnSpeedOppoChars.Count);
                activeCharacter = topInitiativeAndTurnSpeedOppoChars[oppoCharIndexToSetTurn];
                activeCharacter.SetCharacterTurn();
            } else {
                activeCharacter = topInitiativeAndTurnSpeedOppoChars[0];
                activeCharacter.SetCharacterTurn();
            }
        } else {
            activeCharacter = topOppoInitiativeCharacters[0];
            activeCharacter.SetCharacterTurn();
        }
    }
}
