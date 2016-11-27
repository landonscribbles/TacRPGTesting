using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroStorySceneController : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene("Scenes/WorldMap/WorldMap");
        }
    }
}
