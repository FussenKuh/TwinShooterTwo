using UnityEngine;

namespace FKS
{
    public static class SceneUtils
    {

        /// <summary>
        /// Reloads the current scene
        /// </summary>
        static public void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(CurrentSceneName);
        }

        /// <summary>
        /// Loads the passed in scene
        /// </summary>
        /// <param name="sceneName">The name of the scene to load</param>
        static public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// The current scene's name
        /// </summary>
        static public string CurrentSceneName
        {
            get { return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name; }
        }

        /// <summary>
        /// Listens for the 'Escape' key. When received, calls the 'Application.Quit()' function 
        /// </summary>
        static public void CheckForQuit()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }
        }

    }
}
