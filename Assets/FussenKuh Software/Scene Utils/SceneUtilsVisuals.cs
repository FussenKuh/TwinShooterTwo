using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FKS
{
    public class SceneUtilsVisuals : MonoBehaviour
    {

        #region Public Interface
        [Tooltip("The curve defining what the fade will look like.\n\n" +
                 "This curve will be evaluated for the fade out, then evaluated backwards for the fade in.\n\n" +
                 "For best results, keep Y values between 0 and 1")]
        public AnimationCurve fadeCurve;

        [Tooltip("The Image to fade in/out")]
        public Image fadeImage;

        [Tooltip("The color of the fade")]
        public Color fadeColor = Color.black;

        [Tooltip("Enable to log fade in/out timing information to the console")]
        public bool debugPrint = false;

        /// <summary>
        /// Reload the current scene, fading out, then fading back in.
        /// NOTE: While loading, mouse/touch input is disabled. i.e. The fading image is set as a Raycast Target.
        /// </summary>
        public static void ReloadScene()
        {
            SceneUtilsVisuals.LoadScene(SceneUtils.CurrentSceneName);
        }

        /// <summary>
        /// Load the passed in scene, fading out, then fading back in.
        /// NOTE: While loading, mouse/touch input is disabled. i.e. The fading image is set as a Raycast Target.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load</param>
        public static void LoadScene(string sceneName)
        {
            fade.StartCoroutine(fade._LoadScene(sceneName));
        }
        #endregion

        #region Internal Items
        static SceneUtilsVisuals fade;
        bool sceneLoaded = false;

        float startTime;
        float fadeOutCompleteTime;
        float fadeInCompleteTime;

        private IEnumerator _LoadScene(string sceneName)
        {

            float fadeDuration = fadeCurve.keys[fadeCurve.length - 1].time;

            startTime = Time.time;

            sceneLoaded = false;
            fadeImage.raycastTarget = true;
            float t = 0;
            Color imgColor = fadeColor;

            if (debugPrint) { Debug.Log("[" + Time.time + "]" + "[FKS] Start Fade Out"); }
            // Fade the image to 100% opaque over 'fadeDuration' time
            while (t < fadeDuration)
            {
                imgColor.a = fadeCurve.Evaluate(t);
                fadeImage.color = imgColor;
                yield return new WaitForFixedUpdate();
                t += Time.fixedDeltaTime;
                //if (debugPrint) { Debug.Log(t); }
            }
            // Ensure that the image is 100% opaque
            imgColor.a = 1;
            fadeImage.color = imgColor;
            fadeOutCompleteTime = Time.time;

            // Actually attempt to load the new scene
            SceneUtils.LoadScene(sceneName);
            // Wait around until we're told the scene is loaded
            yield return new WaitUntil(() => sceneLoaded == true);

            t = fadeDuration;
            if (debugPrint) { Debug.Log("[" + Time.time + "]" + "[FKS] Start Fade In"); }
            // Fade the image to 100% transparent over 'fadeDuration' time
            while (t > 0)
            {
                imgColor.a = fadeCurve.Evaluate(t);
                fadeImage.color = imgColor;
                yield return new WaitForFixedUpdate();
                t -= Time.fixedDeltaTime;
                //if (debugPrint) { Debug.Log(t); }
            }
            // Ensure that the image is 100% transparent
            imgColor.a = 0;
            fadeImage.color = imgColor;
            fadeImage.raycastTarget = false;
            fadeInCompleteTime = Time.time;

            if (debugPrint)
            {
                //Debug.Log("Start Time: " + startTime + " Fade Out Complete: " + fadeOutCompleteTime + " Fade In Complete: " + fadeInCompleteTime);
                Debug.Log("[" + Time.time + "]" + "[FKS] Fade Out Duration: " + (fadeOutCompleteTime - startTime) + " Fade In Duration: " + (fadeInCompleteTime - fadeOutCompleteTime));
            }
        }

        private void Awake()
        {
            // Make sure that we only ever have one of these objects
            if (fade == null)
            {
                fade = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            // Register for the sceneLoaded notification
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // Unregister for the sceneLoaded notification
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneLoaded = true;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            Color imgColor = fadeColor;
            imgColor.a = 0;
            fadeImage.color = imgColor;
            fadeImage.raycastTarget = false;
        }

        #endregion

    }
}
