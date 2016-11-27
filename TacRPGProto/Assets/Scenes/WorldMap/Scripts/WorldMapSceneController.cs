using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorldMapSceneController : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("s")) {
            SceneManager.LoadScene("Scenes/Store/Store");
        } else if (Input.GetKeyDown("p")) {
            SceneManager.LoadScene("Scenes/PartyMenu/PartyMenu");
        } else if (Input.GetKeyDown("b")) {
            SceneManager.LoadScene("Scenes/IntroBattle/IntroBattle");
        }
    }
}
