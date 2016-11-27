using UnityEngine;
using System.Collections;

public class BattleController : MonoBehaviour {

    public static BattleController instance = null;
    private BoardController boardController;
    private PlayerCharactersController playerCharactersController;
    private OppositionCharactersController oppoCharactersController;
    // This value is what each character checks against to see if it's their turn by slowly adding their turnSpeed to their
    // turnInitiative until turnInitiative is greater than or equal to.
    public int characterTurnThreshold;

    public bool playersTurn;
    public bool oppositionTurn;

    public int characterZLevel = -1;

    // TEMP
    private bool playerCharactersPlaced = false;
    //

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        boardController = null;
        playerCharactersController = null;
        oppoCharactersController = null;
    }

    void Start () {
        playersTurn = true;
        characterTurnThreshold = 100;
	}

    void Update() {
        // Each update that:
        //      1. No character is moving
        //      2. It's not the player's turn/player is not making a decision with one of their characters
        //      3. No character's turnInitiative has met or exceeded characterTurnThreshold
        // - Have playerCharactersController and oppositionCharactersController iterate through each character they're controlling,
        // add each of their turnSpeed to their turnInitiative.
        // - Have playerCharacterController and oppositionCharacterController each return their highest character turnInitiative
        // - If playerCharacterController and oppositionCharacterController each have values that exceed characterTurnThreshold
        // the highest of the two goes first
        //      - If the two values are equal the controller with the character with the highest turnSpeed value goes first
        //      - If both of the above are equal it is randomly selected between the two who goes first
        //          - Both of the above are also true for matching values within each of the controllers
        // - If both controllers exceed characterTurnThreshold but are not equal the higher goes first
        // - If only one of playerCharacterController or oppositionCharacterController have values that exceed
        // characterTurnThreshold that controller goes first

        if (!playerCharactersController.isActive && !oppoCharactersController.isActive) {
            oppoCharactersController.UpdateCharacterTurnInitiatives();
            playerCharactersController.UpdateCharacterTurnInitiatives();
            // comparisons with enemyCharacterController here
            int playerCharInitiative = playerCharactersController.GetHighestCharacterTurnInitiative();
            int enemyCharInitiative = oppoCharactersController.GetHighestCharacterTurnInitiative();
            int highestInitiative = (playerCharInitiative >= enemyCharInitiative) ? playerCharInitiative : enemyCharInitiative;
            if (highestInitiative >= characterTurnThreshold) {
                if (playerCharInitiative == enemyCharInitiative) {
                    int playerTurnSpeedTopInitiative = playerCharactersController.TopCharacterTurnSpeedWithTopInitiative();
                    int enemyTurnSpeedTopInitiative = oppoCharactersController.TopCharacterTurnSpeedWithTopInitiative();
                    if (playerTurnSpeedTopInitiative == enemyTurnSpeedTopInitiative) {
                        // Then randomly pick one
                        if ((4 % Random.Range(2, 4)) == 0) {
                            playerCharactersController.StartPlayersTurn();
                        } else {
                            oppoCharactersController.StartOppositionTurn();
                        }
                    } else if (playerTurnSpeedTopInitiative > enemyTurnSpeedTopInitiative) {
                        playerCharactersController.StartPlayersTurn();
                    } else {
                        oppoCharactersController.StartOppositionTurn();
                    }
                } else if (playerCharInitiative > enemyCharInitiative) {
                    playerCharactersController.StartPlayersTurn();
                } else {
                    oppoCharactersController.StartOppositionTurn();
                }
            }
        }
    }

    void SetBoardController(BoardController controller) {
        boardController = controller;
    }

    public void RegisterPlayerCharacterController(PlayerCharactersController controller) {
        playerCharactersController = controller;
    }

    public void RegisterEnemyCharactersController(OppositionCharactersController controller) {
        oppoCharactersController = controller;
    }
}
