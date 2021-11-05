using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;

public class SceneUtilsDemo : MonoBehaviour {

    public string sceneToLoad;

    private void Update () {
		if (Input.GetKeyUp(KeyCode.W))
        {
            SceneUtilsVisuals.LoadScene(sceneToLoad);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            SceneUtilsVisuals.ReloadScene();
        }

        SceneUtils.CheckForQuit();
	}

}
