using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterActionsMenuController : MonoBehaviour {

    private Text characterName;
    private Text characterHP;
    private Text characterSP;
    private Image characterPortrait;
    private Action highlightTileAction;

    void Start() {
        characterName = GameObject.Find("CharacterActionsName").GetComponent<Text>();
        characterHP = GameObject.Find("CharacterActionsHP").GetComponent<Text>();
        characterSP = GameObject.Find("CharacterActionsSP").GetComponent<Text>();
        characterPortrait = GameObject.Find("CharacterActionsPortrait").GetComponent<Image>();
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }

    public void SetPortrait(Sprite newPortrait) {
        characterPortrait.sprite = newPortrait;
    }

    public void SetHitPoints(int currentHP, int maxHP) {
        characterHP.text = "HP: " + currentHP + "/" + maxHP;
    }

    public void SetSkillPoints(int currentSP, int maxSP) {
        characterSP.text = "SP: " + currentSP + "/" + maxSP;
    }

    public void SetCharacterName(string characterNameToSet) {
        characterName.text = characterNameToSet;
    }

    public void ResetMenu() {
        // Reset all setting on menu for new character to use this
    }

    public void SetMoveButtonCallback(Action highlightTilesMethod) {
        highlightTileAction = highlightTilesMethod;
    }

    public void HighlightMovementTiles() {
        highlightTileAction();
    }

}
