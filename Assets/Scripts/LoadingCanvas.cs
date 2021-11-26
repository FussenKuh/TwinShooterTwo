using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
    public CanvasGroup fadeGroup;

    public float fadeTime = 1f;

    private void Awake()
    {
        Messenger<WorldSpawner>.AddListener("WorldSpawned", OnWorldSpawned);
    }

    private void OnDestroy()
    {
        Messenger<WorldSpawner>.RemoveListener("WorldSpawned", OnWorldSpawned);
    }

    void OnWorldSpawned(WorldSpawner world)
    {
        StartCoroutine(FadeCanvas());
    }

    IEnumerator FadeCanvas()
    {
        while (fadeGroup.alpha > 0)
        {
            fadeGroup.alpha = fadeGroup.alpha - (Time.unscaledDeltaTime / fadeTime);
            yield return new WaitForEndOfFrame();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
