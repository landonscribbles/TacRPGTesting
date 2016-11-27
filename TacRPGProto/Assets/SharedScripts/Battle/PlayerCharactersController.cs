using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerCharactersController : MonoBehaviour {

    public bool isActive;
    private CharacterController activeCharacter;

    private BoardController boardController;
    private BattleController battleController;
    // private BattleUtils battleUtils;

    // Character objects and controllers
    public List<CharacterController> allyCharacterControllers;
    public GameObject allyWarriorPrefab;
    [HideInInspector]
    public CharacterController allyWarriorController;

    public GameObject allyHealerPrefab;
    [HideInInspector]
    public CharacterController allyHealerController;

    public GameObject allyRangerPrefab;
    [HideInInspector]
    public CharacterController allyRangerController;

    public GameObject allyMagePrefab;
    [HideInInspector]
    public CharacterController allyMageController;

    // Start location and offset
    public Vector2 gridStartLocation;

    void Awake() {
        allyCharacterControllers = new List<CharacterController>();
        isActive = false;
        activeCharacter = null;
    }

    void Start () {
        boardController = GameObject.Find("BoardController").GetComponent<BoardController>();
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();
        // battleUtils = GameObject.Find("BattleUtils").GetComponent<BattleUtils>();
        battleController.RegisterPlayerCharacterController(this);
        PlacePlayerCharacters();
    }
	
	void Update () {
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
        foreach (CharacterController allyController in allyCharacterControllers) {
            if (allyController.turnInitiative > highestTurnInitiative) {
                highestTurnInitiative = allyController.turnInitiative;
            }
        }
        return highestTurnInitiative;
    }

    public int TopCharacterTurnSpeedWithTopInitiative() {
        int topInitiative = GetHighestCharacterTurnInitiative();
        int highestTurnSpeed = 0;
        foreach (CharacterController allyController in allyCharacterControllers) {
            if (allyController.turnInitiative == topInitiative) {
                if (allyController.turnSpeed > highestTurnSpeed) {
                    highestTurnSpeed = allyController.turnSpeed;
                }
            }
        }
        return highestTurnSpeed;
    }

    public void UpdateCharacterTurnInitiatives() {
        // This could probably be merged with GetHighestCharacterTurnInitiative however for more flexibility they will remain
        // seperate for now
        foreach (CharacterController allyController in allyCharacterControllers) {
            allyController.UpdateTurnInitiative();
        }
    }

    void PlacePlayerCharacters() {
        Vector3 warriorWorldLocation = boardController.GetWorldPositionFromTileGrid(gridStartLocation);
        GameObject warriorObject = Instantiate(
            allyWarriorPrefab,
            warriorWorldLocation,
            Quaternion.identity
        ) as GameObject;
        allyWarriorController = warriorObject.GetComponent<CharacterController>();
        allyWarriorController.gridLocation = gridStartLocation;
        allyCharacterControllers.Add(allyWarriorController);

        Vector2 healerStartLocation = new Vector2(gridStartLocation.x - 1, gridStartLocation.y);
        Vector3 healerWorldLocation = boardController.GetWorldPositionFromTileGrid(healerStartLocation);
        GameObject healerObject = Instantiate(
            allyHealerPrefab,
            healerWorldLocation,
            Quaternion.identity
        ) as GameObject;
        allyHealerController = healerObject.GetComponent<CharacterController>();
        allyHealerController.gridLocation = healerStartLocation;
        allyCharacterControllers.Add(allyHealerController);

        Vector2 rangerStartLocation = new Vector2(gridStartLocation.x - 2, gridStartLocation.y);
        Vector3 rangerWorldLocation = boardController.GetWorldPositionFromTileGrid(rangerStartLocation);
        GameObject rangerObject = Instantiate(
            allyRangerPrefab,
            rangerWorldLocation,
            Quaternion.identity
        ) as GameObject;
        allyRangerController = rangerObject.GetComponent<CharacterController>();
        allyRangerController.gridLocation = rangerStartLocation;
        allyCharacterControllers.Add(allyRangerController);

        Vector2 mageStartLocation = new Vector2(gridStartLocation.x - 3, gridStartLocation.y);
        Vector3 mageWorldLocation = boardController.GetWorldPositionFromTileGrid(mageStartLocation);
        GameObject mageObject = Instantiate(
            allyMagePrefab,
            mageWorldLocation,
            Quaternion.identity
       ) as GameObject;
        allyMageController = mageObject.GetComponent<CharacterController>();
        allyMageController.gridLocation = mageStartLocation;
        allyCharacterControllers.Add(allyMageController);
    }

    public void StartPlayersTurn() {
        isActive = true;
        int topInitiative = GetHighestCharacterTurnInitiative();
        List<CharacterController> topTurnInitiativeCharacters = new List<CharacterController>();
        foreach (CharacterController allyController in allyCharacterControllers) {
            if (allyController.turnInitiative == topInitiative) {
                topTurnInitiativeCharacters.Add(allyController);
            }
        }
        if (topTurnInitiativeCharacters.Count > 1) {
            // If we have multiple characters with matching turnInitiative-s we want to let the one with the highest turnSpeed
            // to go first. If multiples have the same turnSpeed as well we'll pick one randomly
            List<CharacterController> topInitiativeAndTurnSpeedCharacters = new List<CharacterController>();
            int topTurnSpeed = 0;
            foreach (CharacterController topInitiativeCharacter in topTurnInitiativeCharacters) {
                if (topInitiativeCharacter.turnSpeed > topTurnSpeed) {
                    topTurnSpeed = topInitiativeCharacter.turnSpeed;
                }
            }
            foreach (CharacterController topInitiativeCharacter in topTurnInitiativeCharacters) {
                if (topInitiativeCharacter.turnSpeed == topTurnSpeed) {
                    topInitiativeAndTurnSpeedCharacters.Add(topInitiativeCharacter);
                }
            }
            if (topInitiativeAndTurnSpeedCharacters.Count > 1) {
                int characterIndexToSetTurn = Random.Range(0, topInitiativeAndTurnSpeedCharacters.Count);
                activeCharacter = topInitiativeAndTurnSpeedCharacters[characterIndexToSetTurn];
                activeCharacter.SetCharacterTurn();
            } else {
                activeCharacter = topInitiativeAndTurnSpeedCharacters[0];
                activeCharacter.SetCharacterTurn();
            }
        } else {
            activeCharacter = topTurnInitiativeCharacters[0];
            activeCharacter.SetCharacterTurn();
        }
    }
}
